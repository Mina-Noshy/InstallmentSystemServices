using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyWebModels.Database;
using MyWebAPI.Services;
using MyWebAPI.Services.Account;
using MyWebModels.Sittings;
using MyWebModels.Models.Account;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using MyWebAPI.Services.Mailing;

namespace MyWebAPI
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
            // for access-controle-allowed-origin
            services.AddCors();

            //Configuration for jwt
            services.Configure<JWT>(Configuration.GetSection("JWT"));

            //Configuration for sending email
            services.AddSingleton(Configuration.GetSection("MailConfiguration").Get<MailConfiguration>());
            services.AddScoped<IMailServices, MailServices>();

            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IClientServices, ClientServices>();
            services.AddScoped<IBillServices, BillServices>();
            services.AddScoped<IInstallmentServices, InstallmentServices>();

            //services.AddDbContext<AppDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            //Adding DB Context with MSSQL
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("MyWebAPI")));


            // add identity.
            services.AddIdentity<AppUser, IdentityRole>(option =>
            {
                option.Password.RequiredLength = 4;
                option.Password.RequireLowercase = false;
                option.Password.RequireUppercase = false;
                option.Password.RequiredUniqueChars = 0;
                option.Password.RequireDigit = false;
                option.Password.RequireNonAlphanumeric = false;
                option.Lockout.MaxFailedAccessAttempts = 5;
                option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

                //option.SignIn.RequireConfirmedEmail = true;
                //option.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();



            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims

            services.AddAuthentication(options =>
                {
                    ///TODO: If you plan to use both cookie and JWT auth on this API, you can use this attribute
                    ///[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                }).AddCookie(options =>
                {
                    options.Events.OnRedirectToLogin = options.Events.OnRedirectToAccessDenied = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    };
                }).AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = true;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,

                        ValidIssuer = Configuration["JWT:Issuer"],
                        ValidAudience = Configuration["JWT:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Key"]))
                    };

                });

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyWebAPI", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyWebAPI v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // for access-controle-allowed-origin
            app.UseCors(x => x
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(origin => true)
            .AllowCredentials());

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
