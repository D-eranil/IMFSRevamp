using IMFS.BusinessLogic;
using IMFS.BusinessLogic.Log;
using IMFS.BusinessLogic.Quote;
using IMFS.DataAccess.Context;
using IMFS.DataAccess.Repository;
using IMFS.DataAccess.UnitWork;
using IMFS.RateCalculator.API.Helpers;
using IMFS.Services.Models;
using IMFS.Services.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace IMFS.RateCalculator.API
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "IMFS.RateCalculator.API", Version = "v1" });
                c.OperationFilter<AddRequiredHeaderParameter>();
            });

            
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            //Add Filter globally for Country Code
            services.AddMvc(options =>
            {
                options.Filters.Add<InterceptBadRequestFilter>();
            });           

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || env.IsStaging() || env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IMFS.RateCalculator.API v1"));
            }

            //app.UseHttpsRedirection();

            //For logging every request coming
            app.UseRequestAndResponseMiddleware();

            app.UseRouting();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            
            app.UseCors("CorsPolicy");
           
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
        }


        public void ConfigureContainer(IUnityContainer container)
        {
            // Could be used to register more types
            BusinessLogicUnityContainer.ConfigureContainer(container);            
            string connnectionString = Configuration.GetConnectionString("IMFSDataContext");
            string nzconnnectionString = Configuration.GetConnectionString("NZIMFSDataContext");

            string auORPConnnectionString = Configuration.GetConnectionString("AUORPDataContext");
            string nzORPConnnectionString = Configuration.GetConnectionString("NZORPDataContext");

            Func<string> getCountryCode = ContextHelper.GetCountryCode;
            //register IUnitOfWork
            container.RegisterType<IUnitOfWork, UnitOfWork>(new HierarchicalLifetimeManager(), new InjectionConstructor(connnectionString, getCountryCode, nzconnnectionString));
            //register IRepository
            container.RegisterType(typeof(IRepository<>), typeof(GenericIMFSRepository<>));
            //register ORPRepository
            container.RegisterType(typeof(IGenericORPrepository<>), typeof(GenericORPRepository<>));

            container.RegisterType<IContextManager, ContextManager>(new HierarchicalLifetimeManager(), new InjectionConstructor(getCountryCode));

            //register IUnitOfWork
            container.RegisterType<IUnitOfWorkORP, UnitOfWorkORP>(new HierarchicalLifetimeManager(), new InjectionConstructor(auORPConnnectionString, getCountryCode, nzORPConnnectionString));

            SmtpConfig smtpConfig = new SmtpConfig();
            smtpConfig.Host = Configuration.GetValue<string>("SmtpHost");
            smtpConfig.Port = Configuration.GetValue<int>("SmtpPort");
            smtpConfig.UseDefaultCredentials = Configuration.GetValue<bool>("SmtpUseDefaultCredentials");

            container.RegisterType<IIMFSEmailService, IMFSEmailService>(new HierarchicalLifetimeManager(), new InjectionConstructor(smtpConfig));


        }
    }
}
