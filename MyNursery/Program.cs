using Microsoft.EntityFrameworkCore;
using MyNursery.Areas.NUAD.Data; // ✅ FIXED: Correct namespace for ApplicationDbContext

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ✅ Register your actual DbContext from the NUAD area
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=NUAD}/{controller=Home}/{action=Index}/{id?}");


// In Program.cs, before "var app = builder.Build();"

//builder.Services.AddRouting(options =>
//{
//    // This tells your app what "NUAD" means
//    options.ConstraintMap.Add("NUAD", typeof(MyNursery.NuadConstraint)); 
//});

//app.UseEndpoints(static endpoints =>
//{
//    endpoints.MapControllerRoute(
//      name: "NUAD",
//      pattern: "{area:NUAD}/{controller=Home}/{action=Index}/{id?}"
//    );
//});

app.Run();
