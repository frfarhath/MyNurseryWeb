using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyNursery.Areas.Welcome.Models; // ApplicationUser
using MyNursery.Areas.NUSAD.Models;   // ApplicationRole
using MyNursery.Data;
using MyNursery.Services;
using MyNursery.Utility;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add MVC with Razor Pages, configure custom view locations for Areas and Content folder
builder.Services.AddControllersWithViews()
    .AddRazorOptions(options =>
    {
        // You can customize locations for views here if needed
        options.ViewLocationFormats.Add("/Views/{1}/{0}.cshtml");
        options.ViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
    });

builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    options.AreaViewLocationFormats.Clear();

    options.AreaViewLocationFormats.Add("/Areas/{2}/Views/{1}/{0}.cshtml");          // Default Area views
    options.AreaViewLocationFormats.Add("/Areas/{2}/Views/Content/{1}/{0}.cshtml");  // Custom Content folder inside Areas
    options.AreaViewLocationFormats.Add("/Areas/{2}/Views/Shared/{0}.cshtml");       // Shared views inside Areas
    options.AreaViewLocationFormats.Add("/Views/Shared/{0}.cshtml");                 // Global shared views
});

builder.Services.AddRazorPages();

// Configure EF Core with SQL Server using connection string from appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

// Add Identity with ApplicationUser and ApplicationRole, configure password and sign-in options
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;

    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Set Role claim type for consistency across identity and authorization
builder.Services.Configure<IdentityOptions>(options =>
{
    options.ClaimsIdentity.RoleClaimType = ClaimTypes.Role;
});

// Configure token lifespan for password reset tokens, etc.
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(2);
});

// Configure authentication cookie and set redirect URLs based on user role after login
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

    options.Events = new CookieAuthenticationEvents
    {
        OnSigningIn = context =>
        {
            var principal = context.Principal;
            var roleClaim = principal?.FindFirst(ClaimTypes.Role)?.Value;

            if (roleClaim == SD.Role_Admin)
                context.Properties.RedirectUri = "/NUAD";
            else if (roleClaim == SD.Role_OtherUser)
                context.Properties.RedirectUri = "/NUUS";   // Admin added users → NUUS 
            else if (roleClaim == SD.Role_User)
                context.Properties.RedirectUri = "/NUOUS";  // Registered users → NUOUS
            else if (roleClaim == SD.Role_SuperAdmin)
                context.Properties.RedirectUri = "/NUSAD";
            else if (roleClaim == SD.Role_AdminCSAD)
                context.Properties.RedirectUri = "/CSAD";
            else
                context.Properties.RedirectUri = "/"; // fallback

            return Task.CompletedTask;
        }
    };
});

// Configure EmailSettings from appsettings.json and register EmailSender service
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();

var app = builder.Build();

// Middleware pipeline setup
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

// Configure area routing support
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Welcome}/{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// Seed roles and users inside a scope during startup
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


// Seed roles method
static async Task SeedRolesAsync(IServiceProvider services, ILogger logger)
{
    var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

    var roles = new[]
    {
        new ApplicationRole { Name = SD.Role_Admin, NormalizedName = SD.Role_Admin.ToUpper(), Description = "Admin Role" },
        new ApplicationRole { Name = SD.Role_User, NormalizedName = SD.Role_User.ToUpper(), Description = "Registered User Role" },
        new ApplicationRole { Name = SD.Role_OtherUser, NormalizedName = SD.Role_OtherUser.ToUpper(), Description = "Admin Added User Role" },
        new ApplicationRole { Name = SD.Role_SuperAdmin, NormalizedName = SD.Role_SuperAdmin.ToUpper(), Description = "Super Admin Role" },
        new ApplicationRole { Name = SD.Role_AdminCSAD, NormalizedName = SD.Role_AdminCSAD.ToUpper(), Description = "Admin CSAD Role" }
    };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role.Name))
        {
            var result = await roleManager.CreateAsync(role);
            if (result.Succeeded)
                logger.LogInformation($"✅ Role created: {role.Name}");
            else
                logger.LogError($"❌ Failed to create role {role.Name}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
        else
        {
            logger.LogInformation($"ℹ️ Role already exists: {role.Name}");
        }
    }
}

// Seed predefined users method
static async Task SeedUsersAsync(IServiceProvider services, ILogger logger)
{
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    var predefinedUsers = new[]
    {
        new { Email = "nuad.user@littlesprouts.com", FirstName = "NUAD", LastName = "User", Role = SD.Role_Admin, Password = "Nuad@123" },
        new { Email = "nusad.user@littlesprouts.com", FirstName = "NUSAD", LastName = "User", Role = SD.Role_SuperAdmin, Password = "Nusad@123" },
        new { Email = "csad.user@littlesprouts.com", FirstName = "CSAD", LastName = "User", Role = SD.Role_AdminCSAD, Password = "Csad@123" }
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
            DateCreated = DateTime.UtcNow,
            Area = u.Role == SD.Role_User ? "NUOUS" :   // Registered user area
                   u.Role == SD.Role_OtherUser ? "NUUS" : // Admin added user area
                   u.Role == SD.Role_Admin ? "NUAD" :
                   u.Role == SD.Role_SuperAdmin ? "NUSAD" :
                   u.Role == SD.Role_AdminCSAD ? "CSAD" : ""
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
