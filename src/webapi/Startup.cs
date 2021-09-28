using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Security.Claims;

namespace webapi
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

            
            // 1. Add Authentication Services
            services.AddAuthentication(options =>
            {
              options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
              options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
              options.Authority = "https://dev-coductivity.us.auth0.com/";
              options.Audience = "https://api.example.com";
            });

            //Add policies to the api
            services.AddAuthorization(options => {
              
              //Since a "scope" claim is a list of scopes, need to split the claims and look for the desired one.
              options.AddPolicy("weather:read", policy => policy.RequireAssertion(ctx =>
              {
                var scopeClaim = ctx.User.FindFirst("scope");
                if (scopeClaim == null) { return false; }
                return scopeClaim.Value.Split(' ').Any(s => s.Contains("weather:read", System.StringComparison.OrdinalIgnoreCase));
              }));
            });
          }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
