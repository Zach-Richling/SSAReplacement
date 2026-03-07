using BlazorBlueprint.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SSAReplacement.Wasm;
using SSAReplacement.Wasm.Client;
using SSAReplacement.Wasm.Layout;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazorBlueprintComponents();

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? builder.HostEnvironment.BaseAddress;
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });
builder.Services.AddScoped<SsaApiClient>();
builder.Services.AddScoped<BreadcrumbService>();

await builder.Build().RunAsync();
