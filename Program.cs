using System.IdentityModel.Tokens.Jwt;
﻿using invoice.Core.Entities;
using invoice.Helpers;
using invoice.Repo;
using invoice.Repo.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using System.Text;
using Stripe;
using Repo;

namespace invoice
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add configuration sources
            builder
                .Configuration.AddJsonFile(
                    "appsettings.json",
                    optional: false,
                    reloadOnChange: true
                )
                .AddJsonFile(
                    $"appsettings.{builder.Environment.EnvironmentName}.json",
                    optional: true
                )
                .AddEnvironmentVariables();

            builder
                .Services.AddControllers()
                .AddJsonOptions(x =>
                {
                    x.JsonSerializerOptions.ReferenceHandler = System
                        .Text
                        .Json
                        .Serialization
                        .ReferenceHandler
                        .IgnoreCycles;
                    x.JsonSerializerOptions.PropertyNamingPolicy = System
                        .Text
                        .Json
                        .JsonNamingPolicy
                        .CamelCase;
                    x.JsonSerializerOptions.WriteIndented = true;
                });

            builder.Services.AddEndpointsApiExplorer();

            // Swagger/OpenAPI configuration
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Title = "Invoice API",
                        Version = "v1",
                        Description = "API for Invoice Management System"
                    }
                );

                options.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "Enter 'Bearer {token}'",
                    }
                );

                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer",
                                },
                            },
                            Array.Empty<string>()
                        },
                    }
                );

                options.MapType<IFormFile>(() =>
                    new OpenApiSchema { Type = "string", Format = "binary" }
                );

                options.MapType<List<IFormFile>>(() =>
                    new OpenApiSchema
                    {
                        Type = "array",
                        Items = new OpenApiSchema { Type = "string", Format = "binary" },
                    }
                );

                options.OperationFilter<FormFileOperationFilter>();

                // Include XML comments if available
                var xmlFile =
                    $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (System.IO.File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            });

            // Prevent default claim mapping
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            // Adding AutoMapper
            builder.Services.AddAutoMapper(typeof(Program).Assembly);

            // Generic Repository
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));  
            
            // invoice Repository
            builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();

            // Adding Scopes
            builder.Services.AddHttpClient();

            // Adding Scope Services
            builder.Services.AddAllApplicationServices(builder.Configuration);

            // DbContext with retry logic for production
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("Connection"),
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null
                        );
                        sqlOptions.CommandTimeout(60);
                    }
                );

                // Only enable sensitive data logging in development
                if (builder.Environment.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                }
            });

            // Identity with more secure defaults
            builder
                .Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequiredLength = 8;
                    options.Password.RequireNonAlphanumeric = true;

                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.AllowedForNewUsers = true;

                    options.User.RequireUniqueEmail = true;
                    options.SignIn.RequireConfirmedEmail = false; // true for production
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // CORS with more restrictive defaults
            var allowedOrigins =
                builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[]
                {
                    "http://localhost:7290",
                    "https://localhost:7290",
                };

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllowSpecificOrigins",
                    policy =>
                    {
                        policy
                            .WithOrigins(allowedOrigins)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    }
                );

                options.AddPolicy(
                    "AllowAll",
                    policy =>
                    {
                        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    }
                );
            });

            // JWT Authentication with enhanced security
            var jwtKey = builder.Configuration["Jwt:Key"];
            var jwtIssuer = builder.Configuration["Jwt:Issuer"];
            var jwtAudience = builder.Configuration["Jwt:Audience"];

            if (string.IsNullOrEmpty(jwtKey) || jwtKey.Length < 32)
            {
                throw new ArgumentException("JWT Key must be at least 32 characters long");
            }

            builder
                .Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = !string.IsNullOrEmpty(jwtAudience),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(1),
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    };
                });

            // Stripe Configuration
            var stripeSecretKey = builder.Configuration["Stripe:SecretKey"];
            if (!string.IsNullOrEmpty(stripeSecretKey))
            {
                StripeConfiguration.ApiKey = stripeSecretKey;
                StripeConfiguration.MaxNetworkRetries = 3;
            }

            // Health checks
            builder
                .Services.AddHealthChecks()
                .AddDbContextCheck<ApplicationDbContext>()
                .AddCheck<StorageHealthCheck>("storage");

            // Response compression
            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Invoice API v1");
                    options.RoutePrefix = "swagger";
                    options.DocumentTitle = "Invoice - New ASP.NET WEB API";
                });
                app.UseDeveloperExceptionPage();

                app.UseCors("AllowAll");
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();

                app.UseCors("AllowSpecificOrigins");
            }
         
            app.UseHttpsRedirection();

            // Response compression
            app.UseResponseCompression();

            // Health check endpoint
            app.UseHealthChecks("/health");

            app.UseAuthentication();
            app.UseAuthorization();

            // Static files with cache control
            app.UseStaticFiles(
                new StaticFileOptions
                {
                    OnPrepareResponse = ctx =>
                    {
                        const int durationInSeconds = 60 * 60 * 24; // 24 hours
                        ctx.Context.Response.Headers.Append(
                            "Cache-Control",
                            $"public, max-age={durationInSeconds}"
                        );
                    },
                }
            );

            // Serve static files from subfolders
            var assetsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets");
            if (Directory.Exists(assetsPath))
            {
                var subFolders = Directory.GetDirectories(assetsPath);
                foreach (var folder in subFolders)
                {
                    var folderName = Path.GetFileName(folder);
                    app.UseStaticFiles(
                        new StaticFileOptions
                        {
                            FileProvider = new PhysicalFileProvider(folder),
                            RequestPath = $"/assets/{folderName}",
                            OnPrepareResponse = ctx =>
                            {
                                const int durationInSeconds = 60 * 60 * 24 * 7; // 1 week for assets
                                ctx.Context.Response.Headers.Append(
                                    "Cache-Control",
                                    $"public, max-age={durationInSeconds}"
                                );
                            },
                        }
                    );
                }
            }

            if (app.Environment.IsDevelopment())
            {
                using var scope = app.Services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.Migrate();
            }

            app.MapControllers();

            app.Run();
        }
    }

    // Simple health check for storage
    public class StorageHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default
        )
        {
            var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            if (Directory.Exists(wwwrootPath))
            {
                try
                {
                    var testFile = Path.Combine(wwwrootPath, "healthcheck.txt");
                    System.IO.File.WriteAllText(testFile, DateTime.UtcNow.ToString());
                    System.IO.File.Delete(testFile);
                    return Task.FromResult(HealthCheckResult.Healthy("Storage is accessible"));
                }
                catch (Exception ex)
                {
                    return Task.FromResult(
                        HealthCheckResult.Unhealthy("Storage is not accessible", ex)
                    );
                }
            }
            return Task.FromResult(HealthCheckResult.Healthy("Storage directory exists"));
        }
    }
}
