using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json.Serialization;
using SuccessArenaBAL.ExceptionHandler;
using SuccessArenaBAL.Foundation.Web;
using SuccessArenaConfig.Configuration;
using SuccessArenaDAL.Context;
using SuccessArenaFoundation.Extensions;

namespace SuccessArenaFoundation
{
    public partial class Startup
    {
        public IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            /*var builder = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .SetBasePath(Directory.GetCurrentDirectory()) // Ensure correct path
                .AddJsonFile("/etc/secrets/appSettings.Production.json", optional: true, reloadOnChange: true) // Load from Render Secret Files
                .AddEnvironmentVariables(); // Load env variables

            Configuration = builder.Build();*/
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var configObject = new APIConfiguration();
            var mailSettings = new SmtpMailSettings();
            //var externalIntegrations = new ExternalIntegrations();

            // Bind from appsettings.json or Environment Variables
            Configuration.GetRequiredSection("APIConfiguration").Bind(configObject);
            //Configuration.GetRequiredSection("ExternalIntegrations").Bind(externalIntegrations);
            Configuration.GetRequiredSection("SmtpMailSettings").Bind(mailSettings);

            configObject.SmtpMailSettings = mailSettings;
            //configObject.ExternalIntegrations = externalIntegrations;

            // Override values with environment variables from Render
            configObject.ApiDbConnectionString = Environment.GetEnvironmentVariable("ApiDbConnectionString") ?? configObject.ApiDbConnectionString;
            configObject.JwtTokenSigningKey = Environment.GetEnvironmentVariable("JwtTokenSigningKey") ?? configObject.JwtTokenSigningKey;

            services.AddSingleton(configObject);
            services.ConfigureCommonApplicationDependencies(Configuration, configObject);
            RegisterAllThirdParties(services);

            var mvcBuilder = services.AddControllers(x =>
            {
                x.Filters.Add<APIExceptionFilter>();
            })
            .AddNewtonsoftJson(opt =>
            {
                opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                opt.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.None;
                opt.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            });

            if (configObject.IsOdataEnabled)
            {
                mvcBuilder.AddOData((opt, x) =>
                {
                    opt.AddRouteComponents("v1", x.GetEdmModel())
                    .Filter().Select().Expand().OrderBy().SetMaxTop(100).SkipToken().Count();
                });
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, APIConfiguration configObject)
        {
            app.ConfigureCommonInPipeline(configObject);
            EnsureDirectoriesExist(env);

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.WebRootPath, "content")),
            });

            app.Use(async (context, next) =>
            {
                context.Request.GetOrAddTracingId();
                await next.Invoke();
            });

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void RegisterAllThirdParties(IServiceCollection services)
        {
            services.AddDbContextPool<ApiDbContext>((provider, options) =>
            {
                var configuration = provider.GetService<APIConfiguration>();
                var connectionString = Environment.GetEnvironmentVariable("ApiDbConnectionString") ?? configuration.ApiDbConnectionString;

                options.UseSqlServer(connectionString);
                options.EnableSensitiveDataLogging();
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
            });
        }

        private void EnsureDirectoriesExist(IWebHostEnvironment env)
        {
            string[] directories = new string[]
            {
                Path.Combine(env.WebRootPath, "website"),
                Path.Combine(env.WebRootPath, "website/superadmin"),
                Path.Combine(env.WebRootPath, "website/superadmin/browser"),
                Path.Combine(env.WebRootPath, "website/end-user"),
                Path.Combine(env.WebRootPath, "website/end-user/browser")
            };

            foreach (var directory in directories)
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
        }
    }
}
