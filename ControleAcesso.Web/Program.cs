using ControleAcesso.Application.Services;
using ControleAcesso.Domain.Interfaces.Repositories;
using ControleAcesso.Domain.Interfaces.Services;
using ControleAcesso.Infrastructure.Data;
using ControleAcesso.Infrastructure.Repositories;
using ControleAcesso.Web.Filter;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddSingleton<ProblemDetailsFactory, CustomProblemDetailsFactory>();
builder.Services.AddTransient<ProblemDetailsFactory, CustomProblemDetailsFactory>();
builder.Services.AddTransient<ValidateModelStateFilter>();
//Serialize
builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ValidateModelStateFilter());
})

    .AddNewtonsoftJson()
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
});

//Repositories My Application

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

//Service My Application

builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));
builder.Services.AddScoped<IGroupService, GroupService>();


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
