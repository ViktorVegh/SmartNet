using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorAppSN;
using BlazorAppSN.Auth;
using Blazored.LocalStorage;
using IHttpClientsSN;
using HttpClientsSN;
using BlazorAppSN.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");



builder.Services.AddScoped(sp => new HttpClient());

// HTTP clients
builder.Services.AddTransient<IAuthHttpClient, AuthHttpClient>();
builder.Services.AddTransient<ILearningMaterialHttpClient, LearningMaterialHttpClient>();
builder.Services.AddTransient<IUserHttpClient, UserHttpClient>();

// LocalStorage
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddSingleton<UrlParserService>();


await builder.Build().RunAsync();