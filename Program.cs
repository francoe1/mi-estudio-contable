using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MiEstudio.Authorization;
using MiEstudio.Data.Contexts;
using MiEstudio.Data.Models;
using MiEstudio.Data.Seed;
using System.Text;
using System.Text.Json.Serialization;

internal class Program
{
    private static void Main(string[] args)
    {
        SetupDataDirectory();

        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<DataContext>(option =>
        {
            string connectionString = builder.Configuration["ConnectionStrings:SQLite"];
            option.UseSqlite(connectionString);
        });

        builder.Services.AddHttpContextAccessor()
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
                                   ValidIssuer = builder.Configuration["Jwt:Issuer"],
                                   ValidAudience = builder.Configuration["Jwt:Audience"],
                                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                               };
                           });

        builder.Services.AddSwaggerGen(options =>
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

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.AppRequireClient,
                policy => policy.RequireClaim("Type", new[] { UserType.Admin, UserType.User, UserType.Client }.Select(x => x.ToString())));

            options.AddPolicy(Policies.AppRequireUser,
                policy => policy.RequireClaim("Type", new[] { UserType.Admin, UserType.User }.Select(x => x.ToString())));

            options.AddPolicy(Policies.AppRequireAdmin,
                policy => policy.RequireClaim("Type", UserType.Admin.ToString()));
        });

        builder.Services.Configure<JsonOptions>(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: "default", policy =>
            {
                policy.WithOrigins("*")
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        WebApplication app = builder.Build();

        using (IServiceScope scope = app.Services.CreateScope())
        {
            DataContext context = scope.ServiceProvider.GetService<DataContext>();
            context.Database.Migrate();

            UserModel admin = context.Users.Where(x => x.User == "admin").FirstOrDefault();
            if (admin == null)
            {
                admin = context.Users.Add(new UserModel
                {
                    Id = Guid.NewGuid(),
                    Name = "Administrador",
                    User = "admin",
                    Password = "1234",
                    Type = UserType.Admin
                }).Entity;

                context.SaveChanges();
            }

            Console.WriteLine($"Login like admin \n#USER:{admin.User} \n#PASS:{admin.Password}");
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(x => x.EnablePersistAuthorization());
        };

        app.UseCors("default");
        app.UseRouting();
        app.UseAuthorization();
        app.UseAuthentication();
        app.MapControllers();
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.Run("https://localhost:7275");
    }

    private static void SetupDataDirectory()
    {
        AppDomain.CurrentDomain.SetData("DataDirectory", AppContext.BaseDirectory);
        string dataDirectory = Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "Data");
        if (!Directory.Exists(dataDirectory)) Directory.CreateDirectory(dataDirectory);
    }
}