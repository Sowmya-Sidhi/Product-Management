using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using Demo_Backend.Services;

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
builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddSingleton<JwtService>();
builder.Services.AddSingleton<AuditService>();

// ------------------ 3. JWT Authentication ------------------
var key = builder.Configuration["Jwt:Key"];
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

var app = builder.Build();

// ------------------ 5. Middleware ------------------
// Swagger in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAngularApp");
// JWT authentication & authorization
app.UseAuthentication();
app.UseAuthorization();

// Serilog middleware to log all API requests
//This extension method is used to store all API Calls(Default)
app.UseSerilogRequestLogging(); // logs HTTP method, path, status, duration

// Map controllers
app.MapControllers();

// Run the app
app.Run();
