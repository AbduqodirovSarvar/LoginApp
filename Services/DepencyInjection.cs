using AutoMapper;
using LoginApp.DB;
using LoginApp.DB.Enums;
using LoginApp.Services.Security;
using LoginApp.Services.UseCases;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SQLitePCL;
using System.Security.Claims;
using System.Text;

namespace LoginApp.Services
{
    public static class DependencyInjection
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<AppDbContext>();
            services.AddScoped<HashService>();
            services.AddScoped<TokenService>();
            services.AddHttpContextAccessor();
            services.AddScoped<CurrentUserService>();
            services.AddScoped<UserService>();
            services.AddScoped<AuthService>();

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
                options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
            });


            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new Mapper());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });

            /*raw.SetProvider(new SQLite3Provider_e_sqlite3());
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite(configuration.GetConnectionString("SQLiteConnection"));
            });
            Batteries.Init();*/

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configurations.SecretKey))
                    };
                });

            services.AddAuthorizationBuilder()
                .AddPolicy("AdminActions", policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role, UserRole.Admin.ToString());
                });

            return services;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Login",
                    Description = "Login app"
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Authorization",
                    Type = SecuritySchemeType.Http
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }
    }
}
