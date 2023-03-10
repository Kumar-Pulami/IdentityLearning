using System.Text;
using IdentityLearningAPI.ApplicationDbContext;
using IdentityLearningAPI.Configurations;
using IdentityLearningAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));

builder.Services.AddDbContext<ApplicationDatabaseContext>(option =>
        option.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnectionString"))
        );

builder.Services.AddIdentity<User, IdentityRole>(options =>
    options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDatabaseContext>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

    }).AddJwtBearer( jwt =>
    {
        var secretKey = Encoding.ASCII.GetBytes(builder.Configuration["JwtConfig:SecretKey"]);
        jwt.SaveToken = true;
        jwt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["JwtConfig:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JwtConfig:SecretKey"])),
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true
        };
    }
);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
