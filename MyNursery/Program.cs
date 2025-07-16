using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyNursery.Areas.Welcome.Models;
using MyNursery.Data;
using MyNursery.Services; // EmailSender & EmailSettings
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews()
    .AddRazorOptions(options =>
    {
        options.ViewLocationFormats.Add("Views/{1}/{0}.cshtml");
        options.ViewLocationFormats.Add("Views/Shared/{0}.cshtml");
    });
builder.Services.AddRazorPages();

// DbContext with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

// Identity setup
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.ClaimsIdentity.RoleClaimType = ClaimTypes.Role;
});

// Cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
    options.Cookie.IsEssential = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// Email service registration
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();

var app = builder.Build();

// Error handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Route mapping
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Welcome}/{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// Role and User Seeding - Await async properly inside an async method
await using (var scope = app.Services.CreateAsyncScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger("Startup");

    try
    {
        logger.LogInformation("🔄 Starting role and user seeding...");
        await SeedRolesAsync(services, logger);
        await SeedUsersAsync(services, logger);
        logger.LogInformation("✅ Seeding completed.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ An error occurred during seeding.");
    }
}

app.Run();

// Role Seeder
async Task SeedRolesAsync(IServiceProvider services, ILogger logger)
{
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = { "Parent", "Staff", "NUUS", "NUAD", "NUSAD", "CSAD", "NUOUS", "Assistant Teacher", "Teacher", "Principal", "Head Teacher", "Other Staff" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            var result = await roleManager.CreateAsync(new IdentityRole(role));
            if (result.Succeeded)
            {
                logger.LogInformation($"✅ Created role: {role}");
            }
            else
            {
                logger.LogError($"❌ Failed to create role {role}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
        else
        {
            logger.LogInformation($"ℹ️ Role already exists: {role}");
        }
    }
}

// User Seeder
async Task SeedUsersAsync(IServiceProvider services, ILogger logger)
{
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    var predefinedUsers = new[]
    {
        new { Email = "nuus.user@littlesprouts.com", FirstName = "NUUS", LastName = "User", Role = "NUUS", Password = "Nuus@123" },
        new { Email = "nuad.user@littlesprouts.com", FirstName = "NUAD", LastName = "User", Role = "NUAD", Password = "Nuad@123" },
        new { Email = "nusad.user@littlesprouts.com", FirstName = "NUSAD", LastName = "User", Role = "NUSAD", Password = "Nusad@123" },
        new { Email = "csad.user@littlesprouts.com", FirstName = "CSAD", LastName = "User", Role = "CSAD", Password = "Csad@123" },
        new { Email = "nuous.user@littlesprouts.com", FirstName = "NUOUS", LastName = "User", Role = "NUOUS", Password = "Nuous@123" }
    };

    foreach (var u in predefinedUsers)
    {
        var existing = await userManager.FindByEmailAsync(u.Email);
        if (existing != null)
        {
            logger.LogInformation($"ℹ️ User already exists: {u.Email}");
            continue;
        }

        var user = new ApplicationUser
        {
            UserName = u.Email,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            EmailConfirmed = true,
            DateCreated = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(user, u.Password);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, u.Role);
            logger.LogInformation($"✅ Created user {u.Email} with role {u.Role}");
        }
        else
        {
            logger.LogError($"❌ Failed to create user {u.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
}
