using Domain.Helpers;
using Domain.Interface;
using Infrastructure.Maps;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Extensions.Logging;
using System.Text;

namespace Application
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.SetMinimumLevel(LogLevel.Trace);
                builder.AddNLog("NLog.config");
            });

            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp",
                    builder => builder
                        .WithOrigins("http://localhost:4200")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Application", Version = "v1" });
            });

            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                   .AddJwtBearer(x =>
                   {
                       x.RequireHttpsMetadata = false;
                       x.SaveToken = true;
                       x.TokenValidationParameters = new TokenValidationParameters
                       {
                           ValidateIssuerSigningKey = true,
                           IssuerSigningKey = new SymmetricSecurityKey(key),
                           ValidateIssuer = false,
                           ValidateAudience = false
                       };
                   });

            UserMapper.ConfigureMappings();
            LoanMapper.ConfigureMappings();

            services.AddScoped<IRegisterService>(provider =>
            {
                var appSettings = provider.GetRequiredService<IOptions<AppSettings>>().Value;
                var logger = provider.GetRequiredService<ILogger<IRegisterService>>();
                var emailService = new EmailService(appSettings);
                return new RegisterService(emailService, new UserRepository(), logger);
            });

            services.AddScoped<ILoginService>(provider =>
            {
                var appSettings = provider.GetRequiredService<IOptions<AppSettings>>().Value;
                var logger = provider.GetRequiredService<ILogger<ILoginService>>();
                return new LoginService(appSettings, new UserRepository(), logger);
            });

            services.AddScoped<ILoanService>(Provider =>
            {
                var logger = Provider.GetService<ILogger<ILoanService>>();
                return new LoanService(new LoanRepository(), logger);
            });

            services.AddScoped<IAccountantService>(Provider =>
            {
                var logger = Provider.GetService<ILogger<IAccountantService>>();
                return new AccountantService(new UserRepository(), new LoanRepository(), logger);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Application v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("AllowAngularApp");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
