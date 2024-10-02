using BlazorMultiUserLogin;
using BlazorMultiUserLogin.Components;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;

// Retrieve the client secret from an environment variable.
var environmentName = Environment.GetEnvironmentVariable("TestBlazorAuth", EnvironmentVariableTarget.Process);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container for Razor components and interactive server components.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure authentication with Azure AD using OpenID Connect
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(options =>
    {
        options.Instance = builder.Configuration["AzureAd:Instance"];
        options.TenantId = builder.Configuration["AzureAd:TenantId"];
        options.ClientId = builder.Configuration["AzureAd:ClientId"];
        options.ClientSecret = environmentName;
        options.CallbackPath = builder.Configuration["AzureAd:CallbackPath"];
        options.Prompt = "login";

        options.Events = new OpenIdConnectEvents
        {
            OnRemoteFailure = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Failure?.Message}");
                context.Response.Redirect("/Error");
                context.HandleResponse();
                return Task.CompletedTask;
            },
            OnRedirectToIdentityProviderForSignOut = context =>
            {
                // Build the logout URL with the post-logout redirect URI to send the user back to the app after sign-out
                var logoutUri = context.ProtocolMessage.IssuerAddress +
                $"?post_logout_redirect_uri={Uri.EscapeDataString("https://localhost:7180/")}";

                // Redirect the user to the identity provider's logout URL
                context.Response.Redirect(logoutUri);

                // Mark the response as handled to stop further processing
                context.HandleResponse();

                return Task.CompletedTask; // Return a completed task since no async work is needed
            }
        };
    });

// Add authorization services
builder.Services.AddAuthorization();

// Add Microsoft Identity UI for authentication-related Razor components and views
builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI();

var app = builder.Build();

// Middleware to prevent caching by adding cache-control headers to the response
app.Use(async (context, next) =>
{
    context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
    context.Response.Headers["Pragma"] = "no-cache";
    context.Response.Headers["Expires"] = "0";

    await next.Invoke();
});

// Configure the HTTP request pipeline for handling errors in non-development environments
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapControllers();

// Map Blazor components to the application's root and set interactive server render mode
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
