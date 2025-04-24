using MeetUp.Data;
using Microsoft.EntityFrameworkCore;
using MeetUp.Areas.Identity.User;
using MeetUp.wwwroot;
using MeetUp.Settings;
using Microsoft.Extensions.Options;
using MeetUp.Services;

namespace MeetUp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            string connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");

            _ = builder.Services.Configure<ThemeSettings>(builder.Configuration.GetSection("ThemeSettings"));
            _ = builder.Services.AddSingleton(provider =>
            {
                var themeSettings = provider.GetRequiredService<IOptions<ThemeSettings>>().Value;
                return new ChatHubStateService(themeSettings.ChatColours);
            });

            _ = builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

            _ = builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<ApplicationDbContext>();

            _ = builder.Services.AddRazorPages();
            _ = builder.Services.AddSignalR();

            WebApplication app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {   
                _ = app.UseExceptionHandler("/Error");
                _ = app.UseHsts();
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