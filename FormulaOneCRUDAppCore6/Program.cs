using FormulaOneCRUDAppCore6.Configurations;
using FormulaOneCRUDAppCore6.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/// <summary>
/// Connection String
/// </summary>
builder.Services.AddDbContext<AppDBContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Singleton);

/// <summary>
/// JWT Configuration
/// This will check our appsetting.json for the JwtConfig
/// It allow us to utilize our configuration within the DI Container(Dependency Injection Container).
/// </summary>
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));


/// <summary>
/// JWT Service
/// </summary>

builder.Services.AddAuthentication(options =>
{
    //Here we are defining that our Default Authentication Scheme is JWT Bearer in this application
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; 
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(jwt =>
{
    //Here we are defining that all the keys we are getting for JWT is from appsetting 
    var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JwtConfig:SecretKey").Value);
    jwt.SaveToken = true;

    //TokenValidationParameters is basically checking that token is not any random token
    jwt.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false, //for development purpose(Should contain SSL)
        ValidateAudience = false, //for development purpose(Should contain SSL)
        RequireExpirationTime = false, //for development purpose(needs to be updated when refresh token is added
        ValidateLifetime = true, //It will check the lifetime of the token i.e for how much time token should be validated and once time is up it will get rejected
    };
});


//Have to add default identity manager with user
//Need to understand
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedEmail = false)
    .AddEntityFrameworkStores<AppDBContext>();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
