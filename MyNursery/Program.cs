using MyNursery.DataAccess.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add services to the container.
// builder.Services.AddDbContext<ApplicationDbContext>(Options => Options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllersWithViews();

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
    pattern: "{area=NuAD}/{controller=Home}/{action=Index}/{id?}");

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
