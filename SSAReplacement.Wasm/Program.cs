using BlazorBlueprint.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SSAReplacement.Wasm;
using SSAReplacement.Wasm.Auth;
using SSAReplacement.Wasm.Client;
using SSAReplacement.Wasm.Layout;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazorBlueprintComponents();

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? builder.HostEnvironment.BaseAddress;

// Auth services
builder.Services.AddScoped<TokenStorageService>();
builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<JwtAuthenticationStateProvider>());
builder.Services.AddScoped<AuthTokenHandler>();
builder.Services.AddAuthorizationCore();

// Primary API HttpClient with auth token handler
builder.Services.AddHttpClient<SsaApiClient>(client => client.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<AuthTokenHandler>();

// Separate client for token refresh (no auth handler to avoid recursion)
builder.Services.AddHttpClient("AuthRefresh", client => client.BaseAddress = new Uri(apiBaseUrl));

builder.Services.AddScoped<BreadcrumbService>();

await builder.Build().RunAsync();
