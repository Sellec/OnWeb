using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OnWeb
{
    /// <summary>
    /// Must be used as base class when calling UseStartup{TStartup}(IWebHostBuilder).
    /// </summary>
    public abstract class Startup
    {
        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();//.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddRouting();

            OnConfigureServices(services);

            services.AddSingleton<CoreBind.ApplicationCore>(s =>
            {
                var instance = new CoreBind.ApplicationCore(Environment.CurrentDirectory, "Data Source=localhost;Initial Catalog=fabrikae_fabrikanew;Integrated Security=True;");
                return instance;
            });

            var sp = services.BuildServiceProvider();
            var appCore = sp.GetService<CoreBind.ApplicationCore>();
            appCore.Start();

            services.Configure<MvcViewOptions>(options =>
            {
                //  options.ViewEngines.Insert(0, appCore.Get<CoreBind.Providers.ResourceProvider>());
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            OnConfigure(app, env);

            //   app.UseRouter()

            app.UseMvc(routes =>
            {
                //   routes.MapRoute()
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

        }

        /// <summary>
        /// Use this method to add services to the container.
        /// </summary>
        protected abstract void OnConfigureServices(IServiceCollection services);

        /// <summary>
        /// Use this method to configure the HTTP request pipeline.
        /// </summary>
        protected abstract void OnConfigure(IApplicationBuilder app, IHostingEnvironment env);

    }

}
