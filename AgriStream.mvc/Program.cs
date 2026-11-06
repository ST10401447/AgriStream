using AgriStream.mvc.Data;
using AgriStream.mvc.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// CHANGED: Application Insights only starts when a monitoring connection string exists.
// On your PC there is none, so it is skipped. In Azure it is set automatically.
string insightsConnection = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"] ?? "";
if (insightsConnection != "")
{
    builder.Services.AddApplicationInsightsTelemetry();
}

// ADDED: Azure SQL database connection (the connection string comes from user secrets / Azure settings)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ADDED: our own storage and function services
builder.Services.AddSingleton<BlobStorageService>();
builder.Services.AddSingleton<TableStorageService>();
builder.Services.AddHttpClient<FunctionApiService>();

// ADDED: cookie login (stateless, nothing is stored on the server)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
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

// ADDED: must come before UseAuthorization so the app knows who is logged in
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();