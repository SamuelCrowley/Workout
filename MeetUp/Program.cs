using MeetUp.Data;
using Microsoft.EntityFrameworkCore;
using MeetUp.wwwroot;
using MeetUp.Settings;
using Microsoft.Extensions.Options;
using MeetUp.Areas.Application.Services;
using Microsoft.AspNetCore.Identity;
using MeetUp.Data.User;
using MeetUp.Filters;
using Microsoft.AspNetCore.SignalR; // Important
using Microsoft.Azure.SignalR;     // Important

namespace MeetUp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            string connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection")
                ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");

            builder.Services.Configure<ThemeSettings>(builder.Configuration.GetSection("ThemeSettings"));

            builder.Services.AddSingleton(provider =>
            {
                ThemeSettings themeSettings = provider.GetRequiredService<IOptions<ThemeSettings>>().Value;
                return new ChatHubStateService(themeSettings.ChatColours);
            });

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ExceptionFilter>();
            });

            builder.Services.AddDbContext<ApplicationDbContext>(x => x.UseSqlServer(connectionString));

            builder.Services.AddDefaultIdentity<ApplicationUserEO>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserManager<UserManager<ApplicationUserEO>>();

            builder.Services.AddRazorPages();

            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddSignalR();
            }
            else
            {
                builder.Services.AddSignalR();
                //builder.Services.AddSignalR().AddAzureSignalR(); SEC 25-Apr-2025 - If scaling was involved
            }

            WebApplication app = builder.Build();

            app.MapControllers();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                app.MapGet("/debug/routes", () =>
                    string.Join("\n", app.Services.GetRequiredService<IEnumerable<EndpointDataSource>>()
                        .SelectMany(source => source.Endpoints)
                        .OfType<RouteEndpoint>()
                        .Select(e => $"{e.RoutePattern.RawText}")));
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.MapRazorPages();

            app.MapHub<ChatHub>("/gymChatHub");

            app.Run();
        }
    }
}
