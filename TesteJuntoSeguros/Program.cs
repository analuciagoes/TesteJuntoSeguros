using DbUp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using TesteJuntoSeguros.Data;
using TesteJuntoSeguros.Security;
using TesteJuntoSeguros.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSingleton<IUsuariosNegocio, UsuariosNegocio>();
builder.Services.AddSingleton<IUsuariosRepositorio, UsuariosRepositorio>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
    };
});

builder.Services.AddAuthorization();
builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var connectionString = builder.Configuration.GetConnectionString("UsuariosDatabase");
var upgrader = DeployChanges.To
           .SqlDatabase(connectionString)
           .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
           .LogToConsole()
           .Build();

var result = upgrader.PerformUpgrade();

if (!result.Successful)
{
    throw new InvalidOperationException("O banco de dados não pode ser migrado");
}

app.Run();

public partial class Program { }
