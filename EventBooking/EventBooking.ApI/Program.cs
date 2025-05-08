
using EventBooking.ApI.Extentions;
using EventBooking.ApI.Helper;
using EventBooking.BLL.Repositories;
using EventBooking.BLL.Repositories.Contract;
using EventBooking.DAL.Data;
using EventBooking.DAL.Models;
using EventBooking.DAL.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace EventBooking.ApI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            //builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer(); // Required for Swagger

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Event Booking", Version = "v1" });
            });
            builder.Services.AddSwaggerGen(swagger =>
            {
                //This is to generate the Default UI of Swagger Documentation    
                swagger.SwaggerDoc("v2", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ASP.NET 9 Web API",
                    Description = "Event Booking"
                });

                // To Enable authorization using Swagger (JWT)    
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                    new OpenApiSecurityScheme
                    {
                    Reference = new OpenApiReference
                    {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                    }
                    },
                    new string[] {}
                    }
                });
            });


            //Sql Server Connection
            builder.Services.AddDbContext<EventBookingDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add Identity and JWT Authentication
            //Using Static class to add Identity services
            builder.Services.AddIdentityServices(builder.Configuration);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        //builder.WithOrigins("https://localhost:7295", "http://localhost:4200")
                        builder.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                            
                    });
            });
            //Register generic repository
            builder.Services.AddScoped(typeof( IGenericRepository<>),typeof(GenericRepository<>));

            //builder.Services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            //.AddJwtBearer(options =>
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuer = true,
            //        ValidateAudience = true,
            //        ValidateLifetime = true,
            //        ValidateIssuerSigningKey = true,
            //        ValidIssuer = "EventBooking",
            //        ValidAudience = "EventBooking",
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key_here_32_chars_at_least"))
            //    };
            //});
            var app = builder.Build();
            //HandlerPhotos is a static class that handles the photo upload and storage
            HandlerPhotos.Initialize(app.Services.GetRequiredService<IWebHostEnvironment>());
            //Seed Roles and Admin User
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

                // Add Roles
                string[] roles = { SD.AdminRole, SD.CustomerRole };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                //Add Admin User
                var adminUser = new AppUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    DisplayName = "Admin",
                    IsActive = true,
                    EmailConfirmed = true,

                };
                string adminPassword = "Admin@123";
                if (await userManager.FindByEmailAsync(adminUser.Email) == null)
                {
                    var result = await userManager.CreateAsync(adminUser, adminPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, SD.AdminRole);
                    }
                }
            }
            // Seed Categories
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<EventBookingDbContext>();
                if (!context.Categories.Any())
                {
                    context.Categories.AddRange(
                        new Category { Name = "Conferences", Description = "Conference events", Photo = "http://example.com/photo.jpg" },
                        new Category { Name = "Workshops", Description = "Workshop events", Photo = "http://example.com/photo2.jpg" }
                    );
                    context.SaveChanges();
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                //app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EventBooking v1"));
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors("AllowAllOrigins");
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
