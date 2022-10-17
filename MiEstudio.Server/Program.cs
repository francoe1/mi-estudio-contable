using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MiEstudio.Server.Authorization;
using MiEstudio.Server.Data.Contexts;
using MiEstudio.Server.Data.Models;
using MiEstudio.Shared.Data.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.FileProviders;

internal class Program
{
    private static void Main(string[] args)
    {
        SetupDataDirectory();

        IHostBuilder host = Host.CreateDefaultBuilder();

        host.ConfigureWebHostDefaults(builder =>
        {
            builder.ConfigureServices((context, services) =>
            {
                services.AddControllers();
                services.AddSwaggerGen();
                services.AddDbContext<DataContext>(option =>
                {
                    string connectionString = context.Configuration["ConnectionStrings:SQLite"];
                    option.UseSqlite(connectionString);
                });

                services.AddHttpContextAccessor()
                                   .AddAuthorization()
                                   .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                                   .AddJwtBearer(options =>
                                   {
                                       options.TokenValidationParameters = new TokenValidationParameters
                                       {
                                           ValidateIssuer = true,
                                           ValidateAudience = true,
                                           ValidateLifetime = true,
                                           ValidateIssuerSigningKey = true,
                                           ValidIssuer = context.Configuration["Jwt:Issuer"],
                                           ValidAudience = context.Configuration["Jwt:Audience"],
                                           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(context.Configuration["Jwt:Key"]))
                                       };
                                   });

                services.AddSwaggerGen(options =>
                {
                    options.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                    });

                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }
                });
                });

                services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policies.AppRequireClient,
                        policy => policy.RequireClaim("Type", new[] { UserType.Admin, UserType.User, UserType.Client }.Select(x => x.ToString())));

                    options.AddPolicy(Policies.AppRequireUser,
                        policy => policy.RequireClaim("Type", new[] { UserType.Admin, UserType.User }.Select(x => x.ToString())));

                    options.AddPolicy(Policies.AppRequireAdmin,
                        policy => policy.RequireClaim("Type", UserType.Admin.ToString()));
                });

                services.Configure<JsonOptions>(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

                services.AddCors(options =>
                {
                    options.AddPolicy(name: "default", policy =>
                    {
                        policy.WithOrigins("*")
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
                });
            });

            builder.Configure((context, app) =>
            {
                using (IServiceScope scope = app.ApplicationServices.CreateScope())
                {
                    DataContext dataContext = scope.ServiceProvider.GetService<DataContext>();
                    dataContext.Database.Migrate();

                    UserModel admin = dataContext.Users.Where(x => x.User == "admin").FirstOrDefault();
                    if (admin == null)
                    {
                        admin = dataContext.Users.Add(new UserModel
                        {
                            Id = Guid.NewGuid(),
                            Name = "Administrador",
                            User = "admin",
                            Password = "1234",
                            Type = UserType.Admin
                        }).Entity;

                        dataContext.SaveChanges();
                    }

                    Console.WriteLine($"Login like admin \n#USER:{admin.User} \n#PASS:{admin.Password}");
                }

                app.UseSwagger();
                app.UseSwaggerUI(x => x.EnablePersistAuthorization());

                app.UseCors("default");
                app.UseRouting();
                app.UseAuthorization();
                app.UseAuthentication();
                app.UseHttpsRedirection();

                app.UseFileServer(new FileServerOptions
                {
                    FileProvider = new PhysicalFileProvider(
                    Path.Combine(AppContext.BaseDirectory, "wwwroot")),
                    EnableDefaultFiles = true
                });

                app.UseDefaultFiles();
                app.UseStaticFiles();
                app.UseEndpoints(ep => ep.MapControllers());
                //app.Run();
            });
        });

        host.Build().Run();
        Console.ReadLine();
    }

    private static void SetupDataDirectory()
    {
        AppDomain.CurrentDomain.SetData("DataDirectory", AppContext.BaseDirectory);
        string dataDirectory = Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "Data");
        if (!Directory.Exists(dataDirectory)) Directory.CreateDirectory(dataDirectory);
    }
}