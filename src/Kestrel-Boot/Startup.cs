
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
using System.Text;

namespace HttpBroker
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Console.WriteLine(Configuration["SecurityKey"]);
        }

        public void ConfigureServices(IServiceCollection svcs)
        {
            //添加jwt验证：
            svcs.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,//是否验证Issuer
                        ValidateAudience = true,//是否验证Audience
                        ValidateLifetime = true,//是否验证失效时间
                        ValidateIssuerSigningKey = true,//是否验证SecurityKey
                        ValidAudience = "jwttest",//Audience
                        ValidIssuer = "jwttest",//Issuer，这两项和前面签发jwt的设置一致
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecurityKey"]))//拿到SecurityKey
                    };
                });


            svcs.AddScoped<ISysService<UserFile>,SysUserFileService>();

            svcs.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            svcs.AddTransient<IOption<Config>, AppOption>();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.Use(async (context, next) =>
            {
                context.Response.Headers["Server"] = "Bun Server 2098";
                await next();
            });

            var options = new StaticFileOptions();

            options.FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Public"));

            app.UseStaticFiles(options);

            app.UseAuthentication();//启用验证

            app.UseMvc();
        }
    }
}
