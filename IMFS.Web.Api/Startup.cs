using IMFS.BusinessLogic;
using IMFS.BusinessLogic.RoleManagement;
using IMFS.BusinessLogic.UserManagement;
using IMFS.DataAccess.DBContexts;
using IMFS.DataAccess.Repository;
using IMFS.DataAccess.UnitWork;
using IMFS.Services.IMWebServices;
using IMFS.Services.Models;
using IMFS.Services.Services;
using IMFS.Web.Api.Helper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Xml.Serialization;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace IMFS.Web.Api
{
    public class Startup
    {
        public Startup(Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {



            var builder = new ConfigurationBuilder()
       .SetBasePath(env.ContentRootPath)
        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false, reloadOnChange: true)

    .AddEnvironmentVariables();

            Configuration = builder.Build();

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "IMFS.Web.Api", Version = "v1" });
            });
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });
            services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(
            Configuration.GetConnectionString("IMFSDataContext")));
            services.AddDefaultIdentity<IdentityUser>()
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>();


            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.Authority = Configuration.GetValue<string>("OktaAuthority");
                    options.Audience = "api://tokenhook";
                    options.RequireHttpsMetadata = false;
                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async ctx =>
                        {
                            var idToken = ctx.HttpContext.Request.Headers["Token"];
                            if (!string.IsNullOrEmpty(idToken))
                            {
                                var handler = new JwtSecurityTokenHandler();
                                var idTokenObject = handler.ReadJwtToken(idToken);
                                var identity = ctx.Principal.Identities.FirstOrDefault();
                                identity.AddClaims(idTokenObject.Claims);
                            }

                            //Get EF context
                            try
                            {
                                var userManager = ctx.HttpContext.RequestServices.GetRequiredService<IUserManager>();
                                var roleManager = ctx.HttpContext.RequestServices.GetRequiredService<IRoleManager>();
                                var currentUserName = ctx.Principal.Claims.FirstOrDefault(x => x.Type == "preferred_username")?.Value.ToLower();
                                var userDetails = userManager.GetUserDetailsByUserName(currentUserName);
                                if (userDetails != null)
                                {
                                    var identity = ctx.Principal.Identities.FirstOrDefault();
                                    identity.AddClaim(new Claim("UserId", userDetails.Id));


                                    var roleDetails = roleManager.GetUserRole(userDetails.Id);
                                    if (roleDetails != null)
                                    {
                                        identity.AddClaim(new Claim(ClaimTypes.Role, roleDetails.Name));
                                    }

                                }
                            }
                            catch (Exception ex)
                            {
                                var temp = ex.ToString();
                            }
                        }
                    };
                });

            var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

            services.AddMvc(config =>
            {
                config.Filters.Add(new AuthorizeFilter(policy));
            });

            // services.AddHttpContextAccessor();
            // services.AddTransient<ICurrentUserManager, ContextHelper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || env.IsStaging() || env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IMFS.Web.Api v1"));
            }



            app.UseRouting();
            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public void ConfigureContainer(IUnityContainer container)
        {

            string connnectionString = Configuration.GetConnectionString("IMFSDataContext");
            string auORPConnnectionString = Configuration.GetConnectionString("AUORPDataContext");

            Func<string> getCountryCode = ContextHelper.GetCountryCode;

            var webServiceSettingsConfigPath = Configuration.GetValue<string>("WebServiceSettingsFilePath");
            var resourcesFolderPath = Configuration.GetValue<string>("ResourcesFolderPath");
            ProductSAPConfig productSAPConfig = new ProductSAPConfig();
            POCreateSAPConfig sapPOConfig = new POCreateSAPConfig();
            WebServiceSettings webServiceSettings = null;
            XmlSerializer serializer = new XmlSerializer(typeof(WebServiceSettings));

            StreamReader reader = new StreamReader(webServiceSettingsConfigPath);
            webServiceSettings = (WebServiceSettings)serializer.Deserialize(reader);
            reader.Close();

            var pnaConfig = webServiceSettings.Items.Where(x => x.Name == "PriceAndAvailability").FirstOrDefault();

            productSAPConfig.Url = pnaConfig.Url;
            productSAPConfig.Action = pnaConfig.Action;
            productSAPConfig.UserName = pnaConfig.UserName;
            productSAPConfig.Password = pnaConfig.Password;
            productSAPConfig.UserCredentialID = pnaConfig.UserCredentialID;
            productSAPConfig.PNAInputTemplatePath = resourcesFolderPath + pnaConfig.PNAInputTemplateName;
            productSAPConfig.DistributionChannel = Configuration.GetValue<string>("DistributionChannel");
            productSAPConfig.Division = Configuration.GetValue<string>("Division");
            productSAPConfig.CustomerPOType = Configuration.GetValue<string>("CustomerPOType");
            productSAPConfig.GenericResellerAccount = Configuration.GetValue<string>("GenericResellerAccount");
            productSAPConfig.TestResellerAccount = Configuration.GetValue<string>("TestResellerAccount");
            productSAPConfig.SalesOrganization = Configuration.GetValue<string>("SalesOrganization");

            container.RegisterType<IIMWebServicesManager, IMWebServicesManager>(new HierarchicalLifetimeManager(), new InjectionConstructor(productSAPConfig, sapPOConfig));

            SmtpConfig smtpConfig = new SmtpConfig();
            smtpConfig.Host = Configuration.GetValue<string>("SmtpHost");
            smtpConfig.Port = Configuration.GetValue<int>("SmtpPort");
            smtpConfig.UseDefaultCredentials = Configuration.GetValue<bool>("SmtpUseDefaultCredentials");

            container.RegisterType<IIMFSEmailService, IMFSEmailService>(new HierarchicalLifetimeManager(), new InjectionConstructor(smtpConfig));


            container.RegisterType<IUnitOfWork, UnitOfWork>(new HierarchicalLifetimeManager(), new InjectionConstructor(connnectionString, getCountryCode, string.Empty));

            container.RegisterType(typeof(IRepository<>), typeof(GenericIMFSRepository<>));

            //register ORPRepository
            container.RegisterType(typeof(IGenericORPrepository<>), typeof(GenericORPRepository<>));
            //register ORP IUnitOfWork
            container.RegisterType<IUnitOfWorkORP, UnitOfWorkORP>(new HierarchicalLifetimeManager(), new InjectionConstructor(auORPConnnectionString, getCountryCode, string.Empty));

            // Could be used to register more types    
            BusinessLogicUnityContainer.ConfigureContainer(container);

        }
    }
}
