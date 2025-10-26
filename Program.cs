using PupV1.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using PupV1.Models;

var builder = WebApplication.CreateBuilder(args);

//Connection string
var connectionstring = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseMySql(/*connectionstring*/builder.Configuration.GetConnectionString("DefaultConnection"),
new MySqlServerVersion(new Version(8, 0, 36))));

builder.Services.AddIdentity<ApplicationUser/*IdentityUser*/, IdentityRole>(options => { })
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.IsEssential = true;
    options.Cookie.HttpOnly = true;
    options.SlidingExpiration = true;
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/";
    options.Cookie.IsEssential = false;
});

var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//Set role for Trainer
static async Task SeedRolesAsync(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "Trainer","Client", "Breeder"};
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {await roleManager.CreateAsync(new IdentityRole(role));}
    }
}
//var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    if (!await roleManager.RoleExistsAsync("Trainer"))
    {
        await roleManager.CreateAsync(new IdentityRole("Trainer"));
    }

    if (!await roleManager.RoleExistsAsync("Client"))
    {
        await roleManager.CreateAsync(new IdentityRole("Client"));
    }
    if (!await roleManager.RoleExistsAsync("Breeder"))
    {
        await roleManager.CreateAsync(new IdentityRole("Breeder"));
    }

    /*try
    {
        await SeedRolesAsync(services);
    }
    catch (Exception ex) { Console.WriteLine("error"+ex.Message); }*/
}
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    BreedSeeder.SeeBreeds(context);
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

