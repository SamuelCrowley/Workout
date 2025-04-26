using MeetUp.Data;
using Microsoft.EntityFrameworkCore;
using MeetUp.wwwroot;
using MeetUp.Settings;
using Microsoft.Extensions.Options;
using MeetUp.Areas.Application.Services;
using Microsoft.AspNetCore.Identity;
using MeetUp.Data.User;

namespace MeetUp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            string connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection") ??
                throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");

            _ = builder.Services.Configure<ThemeSettings>(builder.Configuration.GetSection("ThemeSettings"));
            _ = builder.Services.AddSingleton(provider =>
            {
                ThemeSettings themeSettings = provider.GetRequiredService<IOptions<ThemeSettings>>().Value;
                return new ChatHubStateService(themeSettings.ChatColours);
            });

            _ = builder.Services.AddControllers(); 

            _ = builder.Services.AddDbContext<ApplicationDbContext>(x => x.UseSqlServer(connectionString));

            _ = builder.Services.AddDefaultIdentity<ApplicationUserEO>(options => options.SignIn.RequireConfirmedAccount = false)
                                .AddEntityFrameworkStores<ApplicationDbContext>()
                                .AddUserManager<UserManager<ApplicationUserEO>>();

            _ = builder.Services.AddRazorPages();

            _ = builder.Services.AddSignalR();

            WebApplication app = builder.Build();

            _ = app.MapControllers();

            if (!app.Environment.IsDevelopment())
            {   
                _ = app.UseExceptionHandler("/Error");
                _ = app.UseHsts();
            }
            else
            {
                app.MapGet("/debug/routes", () =>
                            string.Join("\n", app.Services.GetRequiredService<IEnumerable<EndpointDataSource>>()
                                .SelectMany(source => source.Endpoints)
                                .OfType<RouteEndpoint>()
                                .Select(e => $"{e.RoutePattern.RawText}")));
            }
            
            _ = app.UseHttpsRedirection();
            _ = app.UseStaticFiles();
            _ = app.UseRouting();
            _ = app.UseAuthorization();

            _ = app.MapRazorPages();
            _ = app.MapHub<ChatHub>("/chatHub");

            app.Run();
        }
    }
}