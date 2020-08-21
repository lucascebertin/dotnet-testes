namespace Sample.Testes.API
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Features.v1.User.Service;
    using Serilog;

    public class Startup
    {
        public const string corsPolicyDefault = "CorsPolicyDefault";
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) =>
            Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNameCaseInsensitive = true);

            services.AddCors(options => options.AddPolicy(corsPolicyDefault, builder =>
                builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
            ));

            services
                .AddSwaggerDocument(configure =>
                {
                    configure.Title = "Sample API Documentation";
                    configure.Description = "Describe endpoints";
                    configure.Version = "v1";
                });

            services
                .AddScoped<IUserService, UserService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseSerilogRequestLogging()
               .UseOpenApi()
               .UseSwaggerUi3()
               .UseCors(corsPolicyDefault)
               .UseRouting()
               .UseAuthentication()
               .UseAuthorization()
               .UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
