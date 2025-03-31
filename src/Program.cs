using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

// Add authentication services
services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? throw new ArgumentNullException("Authentication:Google:ClientId");
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? throw new ArgumentNullException("Authentication:Google:ClientSecret");
});

services.AddAuthorization();

services.AddControllers();

var app = builder.Build();

// Add authentication middleware
app.UseAuthentication();
app.UseAuthorization();

// Configure static files with authentication check
const string protectedFilePath = "members";
const string protectedFileRequestPath = "/members";
const string protectedFileRequestPathWithTrailingSlash = "/members/";

app.Use(async (context, next) =>
{
    if (context.Request.Path.Equals(protectedFileRequestPath, StringComparison.OrdinalIgnoreCase))
    {
        // Redirect to /members/ with a 301 Moved Permanently status
        context.Response.Redirect(protectedFileRequestPathWithTrailingSlash, permanent: true);
        return;
    }

    await next();
});

app.Map(protectedFileRequestPath, [Authorize] (protectedApp) =>
{
    var protectedFileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), protectedFilePath));

    protectedApp.UseDefaultFiles(new DefaultFilesOptions
    {
        FileProvider = protectedFileProvider,
        RequestPath = "",
        RedirectToAppendTrailingSlash = true
    });

    protectedApp.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = protectedFileProvider,
        RequestPath = ""
    });

    protectedApp.Run(context => protectedFileProvider.GetFileInfo("index.html").ServeFile(context));
});

Console.WriteLine($"Serving protected static files from: {protectedFilePath}");

app.MapGet("/login", context => context.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties { RedirectUri = "/api/callback" }));
app.MapGet("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/");
});

app.MapControllers();

// Enable static files
const string wwwrootPath = "public/browser";
var wwwrootFileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), wwwrootPath));
app.UseDefaultFiles(new DefaultFilesOptions { RequestPath = "", FileProvider = wwwrootFileProvider });
app.UseStaticFiles(new StaticFileOptions { RequestPath = "", FileProvider = wwwrootFileProvider });
Console.WriteLine($"Serving static files from: {wwwrootPath}");

// any request that isn't /api or /members will be returned the index.html file
// i.e. handled by the angular router
app.MapFallback(async context =>
{
    if (context.Request.Path.StartsWithSegments("/api") ||
        context.Request.Path.StartsWithSegments(protectedFileRequestPath) ||
        context.Request.Path.StartsWithSegments("/login") ||
        context.Request.Path.StartsWithSegments("/logout")
        )
    {
        // do nothing
        return;
    }

    // Serve the index.html file for all other requests
    await wwwrootFileProvider.GetFileInfo("index.html").ServeFile(context);
});

app.Run();
