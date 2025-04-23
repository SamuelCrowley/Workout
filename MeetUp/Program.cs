using MeetUp.wwwroot;
using MeetUp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MeetUp.Areas.Identity.User;

namespace MeetUp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            string connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

            builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddRazorPages();
            builder.Services.AddSignalR();

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