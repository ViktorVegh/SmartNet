using BlazorAppSN;
using BlazorAppSN.Auth;
using BlazorAppSN.Services;
using Blazored.LocalStorage;
using IWebSocketService;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebSocketService;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Logging.ClearProviders();


builder.Logging.SetMinimumLevel(LogLevel.Debug);


builder.Services.AddTransient<IAuthWebSocketService, AuthWebSocketService>();
builder.Services.AddTransient<ILearningMaterialWebSocketService, LearningMaterialWebSocketService>();
builder.Services.AddTransient<IUserWebSocketService, UserWebSocketService>();

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddSingleton<UrlParserService>();


await builder.Build().RunAsync();
