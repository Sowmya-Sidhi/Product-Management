/*
    Program.cs
    - Application startup and middleware configuration for the backend API.
    - Registers services (MongoDbService, JwtService, AuditService), CORS, Swagger and Serilog.
    - Configures JWT authentication parameters and maps controllers.
    - Important: change CORS origins, JWT and Mongo configuration here when moving to production.
*/
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using Demo_Backend.Services;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
var fileName = $"logs/all-logs-{DateTime.Now:yyyy-MM-dd}.txt";
var filename1=$"logs/audit-logs-{DateTime.Now:yyyy-MM-dd}.txt";
// ------------------ 1. Configure Serilog ------------------
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File(fileName, outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    // daily logs
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(e => e.Properties.ContainsKey("Audit"))
        .WriteTo.File(filename1,
            
            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}] {Message:lj}{NewLine}{Exception}"))
    .WriteTo.Console()
    //.WriteTo.Seq("http://localhost:5341") //centralized dashboard 

    .CreateLogger();

builder.Host.UseSerilog();

// ------------------ 2. Add services ------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Enable CORS for Angular frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // Angular app URL
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});


// Your existing services
// Register IMongoClient and IMongoDbService via DI so services are testable and configurable
builder.Services.AddSingleton<IMongoClient>(sp =>
    new MongoClient(builder.Configuration.GetValue<string>("Mongo:ConnectionString") ?? "mongodb://localhost:27017"));

builder.Services.AddScoped<IMongoDbService, MongoDbService>();
builder.Services.AddSingleton<JwtService>();
builder.Services.AddSingleton<AuditService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// ------------------ 3. JWT Authentication ------------------
// Validate required JWT configuration and fail fast with a clear error if missing
var key = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(key))
{
    // Prevent startup with missing secret — fail fast with a clear message
    throw new InvalidOperationException("Configuration error: missing 'Jwt:Key'. Set it in appsettings or environment variables.");
}
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };
});

    // index creation will run after app is built to access the correct service provider

var app = builder.Build();

// ------------------ 5. Middleware ------------------
// Swagger in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ------------------ 6. Create DB indexes at startup ------------------
// Create unique indexes for Users.Email and Products.ProductCode to enforce uniqueness and improve query performance.
// This runs once at startup and will be idempotent. If duplicates exist index creation will fail and we log a warning.
try
{
    using (var scope = app.Services.CreateScope())
    {
        var client = scope.ServiceProvider.GetRequiredService<IMongoClient>();
        var dbName = builder.Configuration["Mongo:Database"] ?? "ProductDB";
        var db = client.GetDatabase(dbName);

        // Users.Email unique index
        var users = db.GetCollection<Demo_Backend.Models.User>("Users");
        var userIndex = new CreateIndexModel<Demo_Backend.Models.User>(Builders<Demo_Backend.Models.User>.IndexKeys.Ascending(u => u.Email), new CreateIndexOptions { Unique = true });
        users.Indexes.CreateOne(userIndex);

        // Products.ProductCode unique index
        var products = db.GetCollection<Demo_Backend.Models.Product>("Products");
        var productIndex = new CreateIndexModel<Demo_Backend.Models.Product>(Builders<Demo_Backend.Models.Product>.IndexKeys.Ascending(p => p.ProductCode), new CreateIndexOptions { Unique = true });
        products.Indexes.CreateOne(productIndex);
    }
}
catch (Exception ex)
{
    // Log to console/Serilog and continue; index creation can fail if duplicates exist.
    Console.WriteLine($"Warning: index creation failed at startup: {ex.Message}");
}

app.UseHttpsRedirection();

app.UseCors("AllowAngularApp");
// JWT authentication & authorization
app.UseAuthentication();
app.UseAuthorization();

// Global exception handling middleware
app.UseMiddleware<Demo_Backend.Middleware.GlobalExceptionMiddleware>();

// Serilog middleware to log all API requests
//This extension method is used to store all API Calls(Default)
app.UseSerilogRequestLogging(); // logs HTTP method, path, status, duration

// Map controllers
app.MapControllers();

// Run the app
app.Run();
