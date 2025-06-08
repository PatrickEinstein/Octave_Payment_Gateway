using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using CentralPG.Data;



using Serilog;

using CentralPG.Infrasturcture.Interfaces.Utilities;
using CentralPG.Infrasturcture.Interfaces.IRepositories;
using CentralPG.Infrastructure.Services.Mains;
using CentralPG.Infrastructure.Services.Repositories;
using CentralPG.Infrastructure.Services.Tasks;
using CentralPG.Infrastructure.Sevices.Mains;
using CentralPG.Infrastructure.Interfaces.IMains;
using OCPG.Infrastructure.Interfaces.IManagers;
using OCPG.Infrastructure.Service.Managers;
using CentralPG.Interfaces.IProcessors;
using CentralPG.Models;
using CentralPG.Infrastructure.Sevices.Utilities;
using accessFT.Infrastructures.Services.Switches;
using OCPG.Infrastructure.Interfaces.ISwitches;
using OCPG.Infrastructure.Service.Switches;
using OCPG.Core.Models;
using OCPG.Infrastructure.Interfaces.IRepositories;
using OCPG.Infrastructure.Service.Repositories;
using OCPG.Infrastructure.Interfaces.ICryptographies;
using OCPG.Infrastructure;
using OCPG.Infrastructure.Service.Tasks;


namespace CentralPG
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
            services.AddSingleton(Configuration.GetSection(nameof(AppUrl)).Get<AppUrl>());
            services.AddSingleton(Configuration.GetSection(nameof(AuthConfig)).Get<AuthConfig>());
            services.AddSingleton(Configuration.GetSection(nameof(PaystackAuthConfig)).Get<PaystackAuthConfig>());
            services.AddSingleton(Configuration.GetSection(nameof(PayStackAppUrls)).Get<PayStackAppUrls>());
            services.AddSingleton(Configuration.GetSection(nameof(FlutterAuthConfig)).Get<FlutterAuthConfig>());
            services.AddSingleton(Configuration.GetSection(nameof(FlutterWaveAppUrls)).Get<FlutterWaveAppUrls>());
            services.AddSingleton(Configuration.GetSection(nameof(CryptographyConfig)).Get<CryptographyConfig>());

            services.AddDbContext<DataBaseContext>(options =>
                                  {
                                      var connectionString = Configuration.GetConnectionString("DefaultConnection");
                                      options.UseNpgsql(connectionString);
                                      //   b => b.MigrationsAssembly("DefaultConnection")
                                  });
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "Octave PaymentGateway", Version = "v2"});

            });

            services.AddCors(Options =>
            Options.AddPolicy("CorsPolicy",
                builder =>
                builder
                .WithOrigins("*")
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
            ));

            services.AddHttpClient<ApiCaller>();

            // Processors
            services.AddScoped<IPaymentProcessor, ChamsSwitch>();


            // Utilities
            services.AddTransient<IDapperContext, DapperContext>();
            services.AddScoped<ICryptoGraphies, Cryptographies>();
            services.AddTransient<IApiCaller, ApiCaller>();

            // Mains
            services.AddScoped<ITestService, TestService>();
            services.AddScoped<ICustomerService, CustomerService>();


            // Respositories
            services.AddScoped<ITestRepository, TestRespository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<ICardRepository, CardRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();


            // Tasks
            services.AddHostedService<TestCronJob>();
            services.AddHostedService<LoginTask>();
            services.AddHostedService<WalletTransferTask>();

            // Managers
            services.AddScoped<IPaymentManager, PaymentManager>();


            // Switches
            services.AddScoped<ICardSwitcher, CardSwitcher>();
            services.AddScoped<IBankTransferSwitcher, BankTransferSwitcher>();
            services.AddScoped<IDirectDebitSwitcher, DirectDebitSwitcher>();

            // UTILS
            services.AddSingleton<IFlutterCryptography, FlutterCryptographyCryptography>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
          
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v2/swagger.json", "Octave PaymentGateway v2"));


            app.UseHttpsRedirection();
            app.UseSerilogRequestLogging();

            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

