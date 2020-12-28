using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySqlConnector;

namespace myfinAPI
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public static IConfiguration Configuration { get; set; }
		public Startup(IConfiguration config)
		{
			Configuration = config;
			
		}
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddTransient<MySqlConnection>(_=> new MySqlConnection(Configuration.GetConnectionString("myfin")));
			services.AddControllers();
			services.AddCors(options =>
			{
				options.AddPolicy("AllowMyOrigin",
				builder => builder.AllowAnyOrigin());
			});
			
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				//app.UseCors(options => options.AllowAnyOrigin());
				app.UseCors(x => x
			   .AllowAnyMethod()
			   .AllowAnyHeader()
			   .SetIsOriginAllowed(origin => true) // allow any origin
			   .AllowCredentials()); // allow credentials

				app.UseDeveloperExceptionPage();
			}
			 

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});

		}
	}
}
