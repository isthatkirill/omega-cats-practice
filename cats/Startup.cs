using cats.auth;
using Microsoft.AspNetCore.Authentication;
using cats.Data;
using cats.services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace cats;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
        
        services.AddAuthentication("BasicAuthentication")
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

        services.AddScoped<ILogService, LogService>();
        
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
            options.AddPolicy("UserPolicy", policy => policy.RequireRole("User"));
        });
        
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "CatStoreAPI", Version = "v1" });
            c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "basic"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "basic"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
        
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "CatStoreAPI v1");
        });
        
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate(); 
            context.EnsureSeedData();
        }
    }
}