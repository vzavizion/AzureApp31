using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Nest;
using Microsoft.AspNetCore.Rewrite;

namespace AzureApp31
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
            services.AddControllers();

            //Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    name: "v1",
                    new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "Azure App",
                        Description = "Azure App"
                    });
            });

            //services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            //services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

            //services.AddAuthentication()
            //    .AddGoogle(options =>
            //    {
            //        options.ClientId = Constants.GoogleClientId;
            //        options.ClientSecret = Constants.GoogleClientSecret;
            //        options.SignInScheme = IdentityConstants.ExternalScheme;
            //    });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "AzureApp",
                        ValidAudience = "AzureApp",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("AzureAppTokenKey"))
                    };
                });

            //var multiplexer = ConnectionMultiplexer.Connect("localhost");
            //services.AddSingleton<IConnectionMultiplexer>(multiplexer);
            //services.AddStackExchangeRedisCache(options =>
            //{
            //    options.Configuration = "localhost:6379,abortConnect=false,connectTimeout=30000,responseTimeout=30000";
            //});
            //OR
            //ConfigurationOptions option = new ConfigurationOptions
            //{
            //    AbortOnConnectFail = false,
            //    ConnectRetry = 30000,
            //    EndPoints = { "localhost:6379" }
            //};
            //var multiplexer = ConnectionMultiplexer.Connect(option);
            //services.AddSingleton<IConnectionMultiplexer>(multiplexer);
            //services.AddStackExchangeRedisCache(options =>
            //{
            //    options.Configuration = "localhost:6379,abortConnect=false,connectTimeout=30000,responseTimeout=30000";
            //});

            //services.AddElasticsearch(Configuration);
            var connectionSettings = new ConnectionSettings(new Uri("http://localhost:9200"));
            var client = new ElasticClient(connectionSettings);
            services.AddSingleton<IElasticClient>(client);

            services.AddTransient<IElasticsearchHelper, ElasticsearchHelper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //Swagger
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint(url: "../swagger/v1/swagger.json", name: "Azure App");
                options.DefaultModelsExpandDepth(-1);
                options.SupportedSubmitMethods(new SubmitMethod[] { SubmitMethod.Get, SubmitMethod.Post });
                options.RoutePrefix = "";
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            //var option = new RewriteOptions();
            //option.AddRedirect("^$", "swagger");
            //app.UseRewriter(option);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
