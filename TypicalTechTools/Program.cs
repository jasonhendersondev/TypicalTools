using Microsoft.EntityFrameworkCore;
using TypicalTechTools.DataAccess;
using TypicalTechTools.Models.Data;
using TypicalTechTools.Models.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Ganss.Xss;
using TypicalTechTools.Models.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("Default");

//Add context class to Services
builder.Services.AddDbContext<TypistTechToolsDBContext>(options =>
{
    options.UseSqlServer(connectionString);
});

//Add our class to the dependency injection so we can request them in our other classes.
//By adding them using the interface name as the key, we can change the class associated with
//it when needed without updating our other classes.
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
builder.Services.AddScoped<EncryptionService>();
builder.Services.AddScoped<HtmlSanitizer>();

builder.Services.AddSession(c =>
{
    c.IdleTimeout = TimeSpan.FromSeconds(600);
    c.Cookie.HttpOnly = true;
    c.Cookie.IsEssential = true;
});

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSingleton<CsvParser>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    //Sets how long the cookie lasts before deleted
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
                    //Allows the timespan to reset when still in use.Only happens if the timer has lapsed over half its time.
                    options.SlidingExpiration = true;
                    //Sets the default redirection locations for failed access attempts
                    options.LoginPath = "/Authentication/Login";
                    options.AccessDeniedPath = "/Authentication/AccessDenied";
                });

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

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// Custom Middleware (inline)
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; " +
                                 "img-src 'self'; " +
                                 "script-src 'self'; " +
                                 "style-src 'self'; " +
                                 "frame-ancestors 'self';" +
                                 "form-action 'self';");
    context.Response.Headers.Add("Referrer-Policy", "no-referrer");
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("Strict-Transport-Security", "max-age=63072000; includeSubDomains;");

    await next(context);
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}/{id?}");

app.Run();
