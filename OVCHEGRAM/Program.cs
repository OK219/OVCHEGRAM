using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using OVCHEGRAM;
using OVCHEGRAM.DBModels;
using OVCHEGRAM.Models;
using OVCHEGRAM.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<ConversationRepository>();
builder.Services.AddScoped<MessageRepository>();
builder.Services.AddScoped<FileManager>();
builder.Services.AddScoped<FileRepository>();
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<OvchegramDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(OvchegramDbContext))));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "AuthCoockie";
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/ZUEV";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
    });

builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}")
    .WithStaticAssets();
app.MapControllerRoute(
    name: "ZUEV",
    pattern: "{ZUEV}",
    defaults: new { controller = "Home", action = "ZUEV" });

app.Run();