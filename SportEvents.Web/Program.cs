using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
using SportEvents.Domain.Repositories;
using NetEscapades.Extensions.Logging.RollingFile;
using SportEvents.Infrastructure.Repositories;
using static SportEvents.Web.Constants;
using static SportEvents.Infrastructure.Constants;
using SportEvents.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

AppConfig.Configuration = builder.Configuration;

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<ApiRequest>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

//SERVICES
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOrganizerRepository, OrganizerRepository>();

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
    logging.AddFile("logs/app-{Date}.txt");
});

var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
