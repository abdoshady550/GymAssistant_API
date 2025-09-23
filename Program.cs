using GymAssistant_API.Data;
using GymAssistant_API.Handeler.Identity;
using GymAssistant_API.Handeler.User;
using GymAssistant_API.Infrastructure;
using GymAssistant_API.Model.Entities.User;
using GymAssistant_API.Model.Identity;
using GymAssistant_API.Repository.Interfaces.ExerciseExercise;
using GymAssistant_API.Repository.Interfaces.Identity;
using GymAssistant_API.Repository.Interfaces.User;
using GymAssistant_API.Repository.Services.Exercises;
using GymAssistant_API.Repository.Services.Identity;
using GymAssistant_API.Repository.Services.User;
using MechanicShop.Api.OpenApi.Transformers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = null;
            options.JsonSerializerOptions.WriteIndented = true;
        });
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddDbContext<AppDbContext>
(option => option.UseSqlServer((builder.Configuration.GetConnectionString("DefaultConnection"))));
builder.Services.AddScoped<ApplicationDbContextInitialiser>();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddIdentity<AppUser, IdentityRole>(
    options => { options.Password.RequireNonAlphanumeric = false; }
    ).AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
    opt.TokenLifespan = TimeSpan.FromHours(2));


builder.Services.Configure<JWT>(
    builder.Configuration.GetSection("JWT")
);
var jwtSettings = builder.Configuration.GetSection("JWT");
var key = Encoding.UTF8.GetBytes(jwtSettings.GetValue<string>("Key"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(
    options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
        };
    }
    );

builder.Services.AddAuthorization();
// Add services to the container
builder.Services.AddScoped<GenerateTokenQueryHandler>();          // Handler
builder.Services.AddScoped<RefreshTokenQueryHandler>();          // Handler
builder.Services.AddScoped<GetUserByIdQueryHanlder>();          // Handler
builder.Services.AddScoped<RegisterHandler>();                 // Handler
builder.Services.AddScoped<ResetPasswordHandler>();           // Handler
builder.Services.AddScoped<ForgotPasswordHandler>();         // Handler
builder.Services.AddScoped<CreateProfileHandler>();         // Handler
builder.Services.AddScoped<UpdateProfileHandler>();        // Handler
builder.Services.AddScoped<GetProfileHandler>();          // Handler
builder.Services.AddScoped<GetMeasurementHandler>();          // Handler



builder.Services.AddScoped<IIdentityService, IdentityService>();  // Service
builder.Services.AddScoped<ITokenProvider, TokenProvider>();     // Service
builder.Services.AddScoped<IUserCreate, UserCreateService>();   // Service
builder.Services.AddScoped<IProfile, ProfileService>();        // Service
builder.Services.AddScoped<IExercise, ExerciseService>();     // Service


builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<VersionInfoTransformer>();
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    options.AddOperationTransformer<BearerSecuritySchemeTransformer>();
});



// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClient", policy =>
    {
        policy.WithOrigins("https://fitrixapp.runasp.net")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

await app.InitialiseDatabaseAsync();


app.MapOpenApi();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "MechanicShop API V1");

    options.EnableDeepLinking();
    options.DisplayRequestDuration();
    options.EnableFilter();
});

app.UseExceptionHandler();

app.MapScalarApiReference();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
