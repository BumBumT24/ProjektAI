using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ProjektCRUD20510.Models;
using ProjektCRUD20510.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    // Add global authorization filter
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter());
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddDbContext<ClassManagerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ClassManagerDatabase")));

builder.Services.AddScoped<IUzytkownikRepositorycs, UzytkownikRepository>();
builder.Services.AddScoped<IFiszkiRepository, FiszkiRepository>();
builder.Services.AddScoped<IKategorieRepository, KategorieRepository>();
builder.Services.AddScoped<ITestyRepository, TestyRepository>();
builder.Services.AddScoped<ITestyFiszkiRepository, TestyFiszkiRepository>();
builder.Services.AddScoped<ITestyUzytkownikRepository, TestyUzytkownikRepository>();
builder.Services.AddScoped<IWynikRepository, WynikRepository>();

// Add cookie authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Home/AccessDenied";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();