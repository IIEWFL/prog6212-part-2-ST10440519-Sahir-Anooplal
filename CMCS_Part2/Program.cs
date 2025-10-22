using CMCS_Part2.Models;
using CMCS_Part2.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(); // [1]

// Add DbContext with InMemory database
builder.Services.AddDbContext<CMCSDbContext>(options =>
    options.UseInMemoryDatabase("CMCSDatabase")); // [2]

// Add session for user management
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IUserService, UserService>();

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
app.UseAuthorization();

// Use session
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

/*
[1] Microsoft Docs. "Controllers with Views in ASP.NET Core." https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/views
[2] Microsoft Docs. "UseInMemoryDatabase Method (Entity Framework Core)." https://learn.microsoft.com/en-us/ef/core/providers/in-memory/
*/
