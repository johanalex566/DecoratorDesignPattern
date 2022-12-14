using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DecoratorDesignPattern.OpenWeatherMap;
using DecoratorDesignPattern.WeatherInterface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

namespace DecoratorDesignPattern
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
            services.AddControllersWithViews();

            services.AddMemoryCache();


            services.AddScoped<IWeatherService>(serviceProvider =>
            {
                String apiKey = Configuration.GetValue<String>("OpenWeatherMapApiKey");
                var logger = serviceProvider.GetService<ILogger<WeatherServiceLoggingDecorator>>();
                var memoryCache = serviceProvider.GetService<IMemoryCache>();

                IWeatherService concreteService = new WeatherService(apiKey);
                IWeatherService withLogginDecorator = new WeatherServiceLoggingDecorator(concreteService, logger);
                IWeatherService withCachingDecorator = new WeatherServiceCachingDecorator(withLogginDecorator, memoryCache);
                return withCachingDecorator;
            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
