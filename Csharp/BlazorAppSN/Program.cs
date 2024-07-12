using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorAppSN;
using Blazored.LocalStorage;
using IHttpClientsSN;
using HttpClientsSN;
using Microsoft.Extensions.Configuration;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


// Register HttpClient for API calls without setting the base address
builder.Services.AddScoped(sp => new HttpClient());

// Register your custom HTTP clients
builder.Services.AddTransient<IAuthHttpClient, AuthHttpClient>();

// Add Blazored LocalStorage
builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();