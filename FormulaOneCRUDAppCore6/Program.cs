using FormulaOneCRUDAppCore6.Configurations;
using FormulaOneCRUDAppCore6.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/// <summary>
/// Connection String
/// </summary>
builder.Services.AddDbContext<AppDBContext>(options=>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")),ServiceLifetime.Singleton);

/// <summary>
/// JWT Configuration
/// This will check our appsetting.json for the JwtConfig
/// It allow us to utilize our configuration within the DI Container(Dependency Injection Container).
/// </summary>
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));


/// <summary>
/// Setting up the CORS Policy
/// </summary>
builder.Services.AddCors(options => options.AddPolicy("corspolicy", build =>
{
    build.AllowAnyOrigin().AllowAnyHeader().AllowAnyHeader();
}));

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
}

app.UseCors("corspolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
