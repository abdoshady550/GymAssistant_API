using GymAssistant_API.Data;
using GymAssistant_API.Extensions;
using GymAssistant_API.Handeler.Exercise;
using GymAssistant_API.Handeler.Exercise.Workout;
using GymAssistant_API.Handeler.Identity;
using GymAssistant_API.Handeler.Identity.Trainer;
using GymAssistant_API.Handeler.Progress;
using GymAssistant_API.Handeler.User;
using GymAssistant_API.Infrastructure;
using GymAssistant_API.Model.Entities.User;
using GymAssistant_API.Model.Identity;
using GymAssistant_API.Repository.Interfaces.Exercise;
using GymAssistant_API.Repository.Interfaces.ExerciseExercises;
using GymAssistant_API.Repository.Interfaces.Exercises;
using GymAssistant_API.Repository.Interfaces.Identity;
using GymAssistant_API.Repository.Interfaces.User;
using GymAssistant_API.Repository.Interfaces.User.Trainer;
using GymAssistant_API.Repository.Services.Exercise;
using GymAssistant_API.Repository.Services.Exercises;
using GymAssistant_API.Repository.Services.Identity;
using GymAssistant_API.Repository.Services.Progress;
using GymAssistant_API.Repository.Services.User;
using GymAssistant_API.Repository.Services.User.Trainer;
using MechanicShop.Api.OpenApi.Transformers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = null;
            options.JsonSerializerOptions.WriteIndented = true;
            options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
        });

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddDbContext<AppDbContext>
(option => option.UseSqlServer((builder.Configuration.GetConnectionString("DefaultConnection"))));
builder.Services.AddScoped<ApplicationDbContextInitialiser>();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddIdentity<AppUser, IdentityRole>(
    options =>
    {
        options.Password.RequireNonAlphanumeric = false;
        options.User.RequireUniqueEmail = true;

    }
    ).AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<SignInManager<AppUser>>();

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
    ).AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]
            ?? throw new InvalidOperationException("Google ClientId not configured");
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]
            ?? throw new InvalidOperationException("Google ClientSecret not configured");
        options.CallbackPath = "/signin-google";

        // إضافة Scopes
        options.Scope.Add("profile");
        options.Scope.Add("email");

        options.SaveTokens = true;


        options.Events.OnCreatingTicket = context =>
        {
            // يمكنك إضافة معالجة إضافية هنا
            return Task.CompletedTask;
        };
    })
    .AddFacebook(options =>
    {
        options.AppId = builder.Configuration["Authentication:Facebook:AppId"]
            ?? throw new InvalidOperationException("Facebook AppId not configured");
        options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"]
            ?? throw new InvalidOperationException("Facebook AppSecret not configured");
        options.CallbackPath = "/signin-facebook";

        // إضافة Permissions
        options.Scope.Add("email");
        options.Scope.Add("public_profile");

        // إضافة Fields
        options.Fields.Add("name");
        options.Fields.Add("email");
        options.Fields.Add("first_name");
        options.Fields.Add("last_name");
        options.Fields.Add("picture");

        options.SaveTokens = true;

        options.Events.OnCreatingTicket = context =>
        {
            return Task.CompletedTask;
        };
    });

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/api/auth/login";
    options.SlidingExpiration = true;
    options.Cookie.SameSite = SameSiteMode.Lax; // مهم للـ External Login
});

builder.Services.AddAuthorization();
// Add services to the container
builder.Services.AddScoped<GenerateTokenQueryHandler>();                // Handler
builder.Services.AddScoped<RefreshTokenQueryHandler>();                // Handler
builder.Services.AddScoped<GetUserByIdQueryHanlder>();                // Handler
builder.Services.AddScoped<RegisterHandler>();                       // Handler
builder.Services.AddScoped<ResetPasswordHandler>();                 // Handler
builder.Services.AddScoped<ForgotPasswordHandler>();               // Handler
builder.Services.AddScoped<CreateProfileHandler>();               // Handler
builder.Services.AddScoped<UpdateProfileHandler>();              // Handler
builder.Services.AddScoped<GetProfileHandler>();                // Handler
builder.Services.AddScoped<GetMeasurementHandler>();           // Handler
builder.Services.AddScoped<CustomExerciseHandler>();          // Handler
builder.Services.AddScoped<ExerciseHandler>();               // Handler
builder.Services.AddScoped<WorkoutHandler>();               // Handler
builder.Services.AddScoped<ProgressHandler>();             // Handler
builder.Services.AddScoped<RecordsHandler>();             // Handler
builder.Services.AddScoped<TrainerHandler>();            // Handler
builder.Services.AddScoped<TrainerRequestHandler>();    // Handler
builder.Services.AddScoped<ExternalLoginHandler>();






builder.Services.AddScoped<IIdentityService, IdentityService>();                      // Service
builder.Services.AddScoped<ITokenProvider, TokenProvider>();                         // Service
builder.Services.AddScoped<IUserCreate, UserCreateService>();                       // Service
builder.Services.AddScoped<IProfile, ProfileService>();                            // Service
builder.Services.AddScoped<IExercise, ExerciseService>();                         // Service
builder.Services.AddScoped<IWorkoutService, WorkoutService>();                   // Service
builder.Services.AddScoped<IPersonalRecordService, PersonalRecordService>();    // Service
builder.Services.AddScoped<IProgressService, ProgressService>();               // Repository
builder.Services.AddScoped<IRecordsService, RecordsService>();                // Repository
builder.Services.AddScoped<ITrainerService, TrainerService>();               // Repository
builder.Services.AddScoped<ITrainerRequestService, TrainerRequestService>();// Repository




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
app.UseStaticFiles(); // لازم يكون موجود


app.Run();
