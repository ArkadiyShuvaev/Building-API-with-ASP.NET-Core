
using System;
using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using CityInfo.Data;
using CityInfo.Data.Entities;
using CityInfo.Data.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace CityInfo.API
{
    public class Startup
    {
	    public static IConfiguration Configuration;

	    public Startup(IConfiguration configuration)
	    {
		    Configuration = configuration;
	    }
		
		// This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                //.AddMvcCore()
                //.AddAuthorization()
                //.AddJsonFormatters(j => j.Formatting = Formatting.Indented)
                .AddMvcOptions(o =>
                {
                    o.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                });

	        
	        services.AddOptions();
	        services.Configure<AppOptions>(Configuration);

	        var connectionString = Configuration.GetConnectionString("cityInfoConnectionString");
	        if (connectionString == null)
	        {
		        throw new ArgumentNullException(nameof(connectionString));
	        }
	        
			services.AddDbContext<CityInfoContext>(o =>
			{
				o.UseSqlServer(connectionString);
			});

	        services.AddScoped<ICityInfoRepository, CityInfoRepository>();

#if DEBUG
			services.AddTransient<IMailService, LocalMailService>();
#else
            services.AddTransient<IMailService, CloudMailService>();
#endif
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, 
			CityInfoContext cityInfoContext)
        {
            loggerFactory.AddConsole();
            loggerFactory.AddDebug(LogLevel.Information);
            //loggerFactory.AddProvider(new NLogLoggerProvider());
            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
            loggerFactory.ConfigureNLog("nlog.config");
            

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler();
            }

			cityInfoContext.EnsureSeedDataForContext();

            app.UseStatusCodePages();

            app.UseMvc();

	        
	        AutoMapper.Mapper.Initialize(conf =>
	        {
		        conf.CreateMap<City, CityWithoutPOintsOfInterestDto>();
		        conf.CreateMap<City, CityDto>();
		        conf.CreateMap<PointOfInterest, PointOfInterestDto>();
		        conf.CreateMap<PointOfInterestForCreationDto, PointOfInterest>();
		        conf.CreateMap<PointOfInterestForUpdateDto, PointOfInterest>();
			});

        }
    }
}
