﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using LTRegistrator.BLL.Contracts.Contracts;
using LTRegistrator.BLL.Services;
using LTRegistrator.BLL.Services.Mappings;
using LTRegistrator.BLL.Services.Repositories;
using LTRegistrator.BLL.Services.Services;
using LTRegistrator.Domain.Entities;
using LTRegistratorApi.Mappings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;

namespace LTRegistratorApi
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
            string connectionString = "Server=(localdb)\\mssqllocaldb;Database=productsdb;Trusted_Connection=True;";
            //string connectionString = "Server=.\\SQLExpress;Database=productsdb.Project;Trusted_Connection=True;MultipleActiveResultSets=true";
            services.AddDbContext<DbContext, LTRegistratorDbContext>(options => options.UseSqlServer(connectionString));
            services.AddIdentity<User, IdentityRole>()
              .AddEntityFrameworkStores<DbContext>()
              .AddDefaultTokenProviders();

            services.AddCors(options =>
            {
                options.AddPolicy("local", p => p.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod());
            });

            // ===== Add Jwt Authentication ========
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
            services
              .AddAuthentication(options =>
              {
                  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                  options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

              })
              .AddJwtBearer(cfg =>
              {
                  cfg.RequireHttpsMetadata = false;
                  cfg.SaveToken = true;
                  cfg.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidIssuer = Configuration["JwtIssuer"],
                      ValidAudience = Configuration["JwtIssuer"],
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtKey"])),
                      ClockSkew = TimeSpan.Zero // remove delay of token when expire
                  };
              });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("IsEmployee", policy => policy.RequireClaim(ClaimTypes.Role, "Employee"));
                options.AddPolicy("IsManager", policy => policy.RequireClaim(ClaimTypes.Role, "Manager"));
                options.AddPolicy("IsAdministrator", policy => policy.RequireClaim(ClaimTypes.Role, "Administrator"));

                options.AddPolicy("IsManagerOrAdministrator", policy => 
                    policy.RequireAssertion(context =>
                        context.User.HasClaim(c => 
                            (c.Type == ClaimTypes.Role && (c.Value == "Manager" || c.Value == "Administrator")))));

                options.AddPolicy("AccessAllowed", policy =>
                    policy.RequireAssertion(context =>
                    {
                        var employeeId = Convert.ToInt32(context.User.FindFirstValue("EmployeeID"));
                        var authContext = (AuthorizationFilterContext)context.Resource;
                        var routeEmployeeId = Convert.ToInt32(authContext.HttpContext.GetRouteValue("employeeId"));
                        return employeeId == routeEmployeeId || context.User.HasClaim(c =>
                                   c.Type == ClaimTypes.Role && (c.Value == "Manager" || c.Value == "Administrator"));
                    }));

                options.AddPolicy("IsAdminOrCurrentManager", policy =>
                    policy.RequireAssertion(context =>
                    {
                        var managerId = Convert.ToInt32(context.User.FindFirstValue("EmployeeID"));
                        var authContext = (AuthorizationFilterContext) context.Resource;
                        var routeManagerId = Convert.ToInt32(authContext.HttpContext.GetRouteValue("managerId"));
                        return managerId == routeManagerId &&
                               context.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Manager")
                               || context.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Administrator");
                    }));
            });
            services.AddTransient<HttpClient>();
            services.AddTransient(typeof(IWorkCalendarRepository), typeof(WorkCalendarRepository));
            services.AddTransient<HttpContext>(s => s.GetService<IHttpContextAccessor>().HttpContext);
            services.AddTransient(typeof(IEmployeeService), typeof(EmployeeService));
            services.AddTransient(typeof(IReportService), typeof(ReportService));

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile<DataMappingProfileWeb>();
                mc.AddProfile<DataMappingProfile>();
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddMvc();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "LTSummerSchool2019", Version = "v1" });
                var filePath = Path.Combine(System.AppContext.BaseDirectory, "LTRegistratorApi.xml");
                c.IncludeXmlComments(filePath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, DbContext dbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseCors("local");
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Our API V1");
            });
        }
    }
}