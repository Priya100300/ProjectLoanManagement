using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProjectLoanManagement.API.Middleware;
using ProjectLoanManagement.API.Services;
using ProjectLoanManagement.Interfaces;
using ProjectLoanManagement.Models;
using ProjectLoanManagement.Repository.Data;
using ProjectLoanManagement.Repository.Repositories;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------
// 1. Settings (read from appsettings.json - nothing secret is hard-coded)
// ---------------------------------------------------------------------
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
IConfigurationSection jwtSection = builder.Configuration.GetSection("Jwt");
JwtSettings jwtSettings = jwtSection.Get<JwtSettings>() ?? new JwtSettings();

// HS256 needs a key of at least 256 bits (32 bytes); fail fast with a clear message instead of IDX10720 at login
if (string.IsNullOrWhiteSpace(jwtSettings.Key) || Encoding.UTF8.GetByteCount(jwtSettings.Key) < 32)
{
    throw new InvalidOperationException("Jwt:Key must be set to a secret of at least 32 characters.");
}

// The placeholder secrets in appsettings.json are only acceptable on a developer machine
if (!builder.Environment.IsDevelopment() &&
    (jwtSettings.Key.StartsWith("CHANGE_THIS", StringComparison.Ordinal) ||
     (builder.Configuration["Security:AadhaarHashKey"] ?? string.Empty).StartsWith("CHANGE_THIS", StringComparison.Ordinal)))
{
    throw new InvalidOperationException("Replace the CHANGE_THIS placeholder secrets (Jwt:Key, Security:AadhaarHashKey) " +
                                        "using environment variables, user-secrets or a key vault before running outside Development.");
}

builder.Services.Configure<JwtSettings>(jwtSection);
builder.Services.Configure<FileStorageOptions>(builder.Configuration.GetSection("FileStorage"));

// ---------------------------------------------------------------------
// 2. Dependency injection
//    DbHelper only holds the connection string, so one shared instance is fine.
//    Repositories are Scoped: one instance per HTTP request.
// ---------------------------------------------------------------------
builder.Services.AddSingleton(new DbHelper(connectionString));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ILoanRepository, LoanRepository>();
builder.Services.AddScoped<IAssessmentRepository, AssessmentRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IKYCRepository, KYCRepository>();
builder.Services.AddScoped<IVideoVerificationRepository, VideoVerificationRepository>();
builder.Services.AddScoped<IEMIRepository, EMIRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IAuditRepository, AuditRepository>();
builder.Services.AddScoped<IBusinessRuleRepository, BusinessRuleRepository>();

builder.Services.AddSingleton<JwtTokenService>();
builder.Services.AddSingleton<SensitiveDataProtector>();
builder.Services.AddSingleton<FileValidator>();
builder.Services.AddSingleton<IFileStorageService, LocalFileStorageService>();

// ---------------------------------------------------------------------
// 3. Controllers + validation errors in the ResultSet format
// ---------------------------------------------------------------------
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            Dictionary<string, string[]> errors = context.ModelState
                .Where(entry => entry.Value.Errors.Count > 0)
                .ToDictionary(entry => entry.Key, entry => entry.Value.Errors.Select(e => e.ErrorMessage).ToArray());

            return new BadRequestObjectResult(ResultSet.Failure("One or more fields are invalid.", "VALIDATION_400", errors));
        };
    });

// ---------------------------------------------------------------------
// 4. JWT authentication ("security guard checks the ID card")
// ---------------------------------------------------------------------
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;   // keep claim names exactly as issued: userId, name, role
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ClockSkew = TimeSpan.FromMinutes(1),
            NameClaimType = JwtTokenService.NameClaim,
            RoleClaimType = JwtTokenService.RoleClaim
        };

        // Return our ResultSet instead of an empty 401/403 body
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();
                return ApiResponseWriter.WriteAsync(context.HttpContext, StatusCodes.Status401Unauthorized,
                    ResultSet.Failure("Authentication required. Send a valid token as: Bearer {token}", "AUTH_401"));
            },
            OnForbidden = context =>
            {
                return ApiResponseWriter.WriteAsync(context.HttpContext, StatusCodes.Status403Forbidden,
                    ResultSet.Failure("You do not have permission to perform this action.", "AUTH_403"));
            }
        };
    });

builder.Services.AddAuthorization();

// ---------------------------------------------------------------------
// 5. Swagger with the Authorize button
// ---------------------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Project Loan Management API",
        Version = "v1",
        Description = "Loan origination, KYC, video KYC, disbursement, EMI and payments."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Type: Bearer {your JWT token}   (get the token from POST /api/auth/login)"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });

    string xmlFile = Path.Combine(AppContext.BaseDirectory, "ProjectLoanManagement.API.xml");
    if (File.Exists(xmlFile))
    {
        options.IncludeXmlComments(xmlFile);
    }
});

// ---------------------------------------------------------------------
// 6. Request pipeline (order matters)
// ---------------------------------------------------------------------
WebApplication app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();          // catches anything thrown below
app.UseMiddleware<RequestResponseMiddleware>();    // one masked log line per request

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Project Loan Management API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
