using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Workout.Web.Settings;
using Workout.Application.Services.Chat;
using Workout.Web.Filters;
using Workout.Infrastructure.Data;
using Workout.Infrastructure.Data.User;
using Workout.Web.wwwroot;
using Workout.Application.Services.Gym;
using Workout.Infrastructure.Services;

namespace Workout
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

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IWorkoutService, WorkoutService>();

            builder.Services.AddRazorPages();

            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddSignalR();
            }
            else
            {
                builder.Services.AddSignalR();
                //builder.Services.AddSignalR().AddAzureSignalR(); SEC 25-Apr-2025 - won't be necessary, unless scaling became a concern
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
