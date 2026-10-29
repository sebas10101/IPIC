using IPIC.Application.Services;
using IPIC.Domain;
using IPIC.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using IPIC.Application.UseCases;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 2. Registrar controladores y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "IPIC API",
        Version = "v1",
        Description = "API para el control de accesos escolares."
    });

    // Permitir utilizar tokens JWT desde Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Introduce el token JWT."
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] =
                new List<string>()
        });
});

// 3. Registrar dependencias del login
builder.Services.AddScoped<IUserRepository, InMemoryUserRepository>();
builder.Services.AddScoped<AuthService>();

// 3.1 Registrar dependencias de asistencia con QR
builder.Services.AddSingleton<IAttendanceRepository, InMemoryAttendanceRepository>();
builder.Services.AddScoped<ITokenService, QrTokenService>();
builder.Services.AddScoped<ValidateAttendanceUseCase>();
builder.Services.AddScoped<GetAttendanceHistoryUseCase>();

// 4. Configurar autenticación JWT
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? "clave_secreta_super_larga_para_seguridad_123!";

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,

                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey)
                ),

                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            };
    });

// 5. Registrar autorización
builder.Services.AddAuthorization();

// 6. Construir la aplicación
var app = builder.Build();

// 7. Habilitar Swagger
app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint(
        "/swagger/v1/swagger.json",
        "IPIC API v1"
    );

    options.RoutePrefix = string.Empty;
});

// 8. Configurar el flujo HTTP

// CORREGIDO: la redirección a HTTPS solo se aplica fuera de desarrollo.
// En desarrollo provocaba que Swagger fallara con "Failed to fetch".
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// 9. Publicar los controladores
app.MapControllers();

app.Run();
