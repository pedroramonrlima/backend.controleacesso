using ControleAcesso.Application.Services;
using ControleAcesso.Domain.Interfaces.Repositories;
using ControleAcesso.Domain.Interfaces.Services;
using ControleAcesso.Infrastructure.Data;
using ControleAcesso.Infrastructure.Interfaces;
using ControleAcesso.Infrastructure.Jwt.Models;
using ControleAcesso.Infrastructure.Jwt.Repositories;
using ControleAcesso.Infrastructure.Ldap.Connection;
using ControleAcesso.Infrastructure.Ldap.Interfaces;
using ControleAcesso.Infrastructure.Ldap.Models;
using ControleAcesso.Infrastructure.Ldap.Repositories;
using ControleAcesso.Infrastructure.Repositories;
using ControleAcesso.Web.Filter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Configurar Kestrel para usar a configuração do appsettings.json
builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.Configure(context.Configuration.GetSection("Kestrel"));
});

//Filters
builder.Services.AddTransient<ProblemDetailsFactory, CustomProblemDetailsFactory>();
builder.Services.AddTransient<ValidateModelStateFilter>();

//Serialize
builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ValidateModelStateFilter());
})

    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Connection My Database

var mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(mySqlConnection, sqlServerOptions => sqlServerOptions.CommandTimeout(60));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    options.EnableSensitiveDataLogging();
});

//Configuration Jwt
IConfiguration configuration = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration.GetSection("Jwt")["issuer"],
        ValidAudience = builder.Configuration.GetSection("Jwt")["audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Jwt")["secretKey"]!))
    };
});
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

//Configuration LDAP

builder.Services.Configure<LDAPSettings>(builder.Configuration.GetSection("LDAPSettings"));
builder.Services.AddScoped<ILdapConnectionContext, LdapConnectionContext>();

//Repositories My Application

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IGroupRespository,GroupRepository>();
builder.Services.AddScoped<IAcesseRequestRepository,AcesseRequestRepository>();
builder.Services.AddScoped<IAcesseRequestDetailRepository, AcesseRequestDetailRepository>();
builder.Services.AddScoped<ILdapManagerRepository, LdapManagerRepository>();
builder.Services.AddScoped<IJwtRepository, JwtRepository>();

//Service My Application

builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IAcesseRequestService, AcesseRequestService>();
builder.Services.AddScoped<IAcesseRequestDetailService, AcesseRequestDetailService>();
builder.Services.AddScoped<ILdapService, LdapService>();
builder.Services.AddScoped<IAuthenticateService, AuthenticateService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder => builder.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
);

app.UseAuthentication();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
