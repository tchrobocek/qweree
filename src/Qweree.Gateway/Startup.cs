namespace Qweree.Gateway;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {

    }
    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
    }

}