using System.Text;
using Integral.Api.Data.Contexts;
using Integral.Api.Features.Identity.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SharedKernel;
using SharedKernel.Abstraction.Ef;
using SharedKernel.Abstraction.Web;

namespace Integral.Api;

public static class Extensions
{
    public static async Task WarmUp(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PrintingDbContext>();

        await db.Database.ExecuteSqlRawAsync("SELECT 1");
    }

    public static void AddAppSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                Description =
                    "Enter 'Bearer' [space] and then your token in the text input below.\n\nExample: \"Bearer xxxxxx\""
            });

            options.AddSecurityRequirement(document =>
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecuritySchemeReference("Bearer", document),
                        []
                    }
                }
            );
            
        });
    }

    public static void UseAppSwagger(this WebApplication app)
    {
        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    public static void AddJwtAuth(this WebApplicationBuilder builder)
    {
        builder.Services.AddConfig<JwtSettings>(builder.Configuration, "Jwt");
        builder.Services.AddSingleton<JwtProvider>();

        var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()!;

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
                };
            });

        builder.Services.AddAuthorization();
    }

    public static void AddAppDbContexts(this WebApplicationBuilder builder)
    {
        builder.Services.AddAppDbContext<PrintingDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            options.EnableSensitiveDataLogging();
        });

        builder.Services.AddAppDbContext<PublishingDbContext>(options =>
        {
            var connectionString = builder.Configuration.GetConnectionString("CVIO");

            options
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                .UseSnakeCaseNamingConvention();
        });
    }
}