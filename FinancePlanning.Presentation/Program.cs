using FinancePlanning.Application;
using FinancePlanning.Application.DTOs;
using FinancePlanning.Application.Interfaces;
using FinancePlanning.Application.Managers;
using FinancePlanning.Domain.Entities;
using FinancePlanning.Infrastructure;
using FinancePlanning.Infrastructure.Interfaces;
using FinancePlanning.Infrastructure.Repositories;
using FinancePlanning.Infrastructure.Services;
using FinancePlanning.Infrastructure.Services.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var cultureInfo = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Account/Login";
    options.AccessDeniedPath = "/Auth/Account/AccessDenied";
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAccountManager, AccountManager>();
builder.Services.AddScoped<IAdminUserManager, AdminUserManager>();
builder.Services.AddScoped<IInterestCalculatorManager<CompoundInterestDto>, CompoundInterestCalculatorManager>();
builder.Services.AddScoped<IInterestCalculatorManager<SimpleInterestDto>, SimpleInterestCalculatorManager>();
builder.Services.AddScoped<ICompoundInterestStorageManager, CompoundInterestStorageManager>();
builder.Services.AddScoped<ICompoundInterestCalculationRepository, CompoundInterestCalculationRepository>();
builder.Services.AddScoped<ISimpleInterestStorageManager, SimpleInterestStorageManager>();
builder.Services.AddScoped<ISimpleInterestCalculationRepository, SimpleInterestCalculationRepository>();
builder.Services.AddScoped<IMonteCarloSimulator, MonteCarloSimulator>();
builder.Services.AddAutoMapper(typeof(MappingProfile));


var env = builder.Environment;
var path = Path.Combine(env.ContentRootPath, "wwwroot", "data", "currencies.json");
var json = File.ReadAllText(path);
var currencies = JsonSerializer.Deserialize<Dictionary<string, CurrencyJsonDto>>(json)
               ?? new Dictionary<string, CurrencyJsonDto>();
builder.Services.AddSingleton<ICurrencyFormatter>(new CurrencyFormatter(currencies));

var app = builder.Build();

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(cultureInfo),
    SupportedCultures = new List<CultureInfo> { cultureInfo },
    SupportedUICultures = new List<CultureInfo> { cultureInfo }
}); 

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { "Admin", "Manager" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    var adminEmail = "admin@example.com";
    var adminPassword = "Admin123!";

    var admin = await userManager.FindByEmailAsync(adminEmail);
    if (admin == null)
    {
        var newAdmin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "Admin",
            LastName = "Uživatel"
        };
        if ((await userManager.CreateAsync(newAdmin, adminPassword)).Succeeded)
        {
            await userManager.AddToRoleAsync(newAdmin, "Admin");
        }
    }
}


if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
