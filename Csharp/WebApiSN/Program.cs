using System.IdentityModel.Tokens.Jwt;
using Clients;
using IClients;
using IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Services;
using System.Text;
using Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.WebSockets;
using WebApiSN.Handlers;


var builder = WebApplication.CreateBuilder(args);
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

// Logging config
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Custom ConetntJsonConverter config
var jsonOptions = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = true,
    ReferenceHandler = ReferenceHandler.IgnoreCycles
};
jsonOptions.Converters.Add(new ContentJsonConverter());

builder.Services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true)
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new ContentJsonConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddSingleton(jsonOptions);
builder.Services.AddHttpClient();

// Swagger/OpenAPI config
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// Authentication config
var secretKey = Encoding.UTF8.GetBytes("new_secret_key_of_at_least_32_characters_long");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(Convert.ToBase64String(secretKey)))
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError(context.Exception, "Authentication failed.");
                context.Response.Headers.Add("Authentication-Failed", "true");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                var userIdClaim = context.Principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                logger.LogInformation("Program - Token validated successfully for user {UserId}", userIdClaim ?? "null");
                return Task.CompletedTask;
            },
            OnMessageReceived = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("Program - Token received: {Token}", context.Token ?? "null");
                return Task.CompletedTask;
            }
        };
    });

// Custom services registration
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IAuthClient, AuthClient>();
builder.Services.AddTransient<ILearningMaterialService, LearningMaterialService>();
builder.Services.AddTransient<ILearningMaterialClient, LearningMaterialClient>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IUserClient, UserClient>();
builder.Services.AddTransient<ITokenHelper, TokenHelper>();
builder.Services.AddTransient<ISubscriptionClient, SubscriptionClient>();
builder.Services.AddTransient<INotificationClient, NotificationClient>();

// CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient",
        corsBuilder => corsBuilder
            .WithOrigins("http://localhost:5296") 
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

var app = builder.Build();

// HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseWebSockets();

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/ws"))
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            var webSocket = await context.WebSockets.AcceptWebSocketAsync();

            if (context.Request.Path == "/ws/user")
            {
                var userService = context.RequestServices.GetRequiredService<IUserService>();
                var userWebSocketHandler = new UserWebSocketHandler(userService);
                await userWebSocketHandler.HandleWebSocketAsync(context, webSocket);
            }
            else if (context.Request.Path == "/ws/learning-material")
            {
                var learningMaterialService = context.RequestServices.GetRequiredService<ILearningMaterialService>();
                var logger = context.RequestServices.GetRequiredService<ILogger<LearningMaterialWebSocketHandler>>();
                var webSocketHandler = new LearningMaterialWebSocketHandler(learningMaterialService, logger);
                await webSocketHandler.HandleWebSocketAsync(context, webSocket);
            }
            else if (context.Request.Path == "/ws/auth")
            {
                var authService = context.RequestServices.GetRequiredService<IAuthService>();
                var logger = context.RequestServices.GetRequiredService<ILogger<AuthWebSocketHandler>>();
                var authWebSocketHandler = new AuthWebSocketHandler(authService, logger);
                await authWebSocketHandler.HandleWebSocketAsync(context, webSocket);
            }
            else if (context.Request.Path == "/ws/notification")
            {
                var notificationService = context.RequestServices.GetRequiredService<INotificationService>();
                var notificationWebSocketHandler = new NotificationWebSocketHandler(notificationService);
                await notificationWebSocketHandler.HandleWebSocketAsync(context, webSocket);
            }
            else if (context.Request.Path == "/ws/subscription")
            {
                var subscriptionService = context.RequestServices.GetRequiredService<ISubscriptionService>();
                var subscriptionWebSocketHandler = new SubscriptionWebSocketHandler(subscriptionService);
                await subscriptionWebSocketHandler.HandleWebSocketAsync(context, webSocket);
            }
            else
            {
                context.Response.StatusCode = 404; // Not Found
            }
        }
        else
        {
            context.Response.StatusCode = 400; // Bad Request
        }
    }
    else
    {
        await next();
    }
});


app.UseHttpsRedirection();
app.UseCors("AllowBlazorClient");
app.UseAuthentication();
app.UseAuthorization();
app.Run();
