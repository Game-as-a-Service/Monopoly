using Client;
using Client.HttpClients;
using Client.Options;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Options;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddOptions<MonopolyApiOptions>()
    .Configure(options => { builder.Configuration.GetSection(nameof(MonopolyApiOptions)).Bind(options); });
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Register the MonopolyApiClient
builder.Services.AddHttpClient<MonopolyApiClient>(client =>
{
    var backendApiOptions = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<MonopolyApiOptions>>().Value;
    client.BaseAddress = new Uri(backendApiOptions.BaseUrl);
});
await builder.Build().RunAsync();