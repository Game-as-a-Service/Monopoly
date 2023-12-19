using Application;
using Application.Common;
using Application.Usecases;
using Domain.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Server;
using Server.DataModels;
using Server.Hubs;
using Server.Repositories;
using Server.Services;
using SharedLibrary.MonopolyMap;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IRepository, InMemoryRepository>();
builder.Services.AddScoped<IEventBus<DomainEvent>, MonopolyEventBus>();
builder.Services.AddMonopolyApplication();
builder.Services.AddSignalR();

builder.Services.AddCors(options => options.AddPolicy("CorsPolicy",
        builder =>
        {
            builder.AllowAnyHeader()
                   .AllowAnyMethod()
                   .SetIsOriginAllowed((host) => true)
                   .AllowCredentials();
        }));

// 如果 Bind Options 時需要依賴注入
// 如果是develop
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IPlatformService, DevelopmentPlatformService>();
}
else
{
    builder.Services.AddScoped<IPlatformService, PlatformService>();
}
builder.Services.AddSingleton<PlatformJwtEvents>();
builder.Services
    .AddOptions<JwtBearerOptions>("Bearer")
    .Configure<PlatformJwtEvents>((opt, jwtEvents) =>
    {
        builder.Configuration.Bind(nameof(JwtBearerOptions), opt);
        opt.Events = jwtEvents;
    });
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer");

var app = builder.Build();

app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<MonopolyHub>("/monopoly");

app.MapGet("/health", () => Results.Ok());

// 開始遊戲
app.MapPost("/games", async (context) =>
{
    string hostId = context.User.FindFirst(ClaimTypes.Sid)!.Value;
    CreateGameBodyPayload payload = (await context.Request.ReadFromJsonAsync<CreateGameBodyPayload>())!;
    CreateGameUsecase createGameUsecase = app.Services.CreateScope().ServiceProvider.GetRequiredService<CreateGameUsecase>();
    string gameId = createGameUsecase.Execute(new CreateGameRequest(hostId, payload.Players.Select(x => x.Id).ToArray()));

    string frontendBaseUrl = app.Configuration["FrontendBaseUrl"]!.ToString();

    var url = $@"{frontendBaseUrl}games/{gameId}";

    await context.Response.WriteAsync(url);
}).RequireAuthorization();

app.MapGet("/map", (string mapId) =>
{
    string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
    string jsonFilePath = Path.Combine(projectDirectory, "Maps", $"{mapId}.json");

    if (!File.Exists(jsonFilePath))
    {
        return Results.NotFound();
    }

    // read json file
    string json = File.ReadAllText(jsonFilePath);
    var data = MonopolyMap.Parse(json);
    return Results.Json(data, MonopolyMap.JsonSerializerOptions);
});

app.MapGet("/rooms", () =>
{
    var repository = app.Services.CreateScope().ServiceProvider.GetRequiredService<IRepository>();
    return Results.Json(repository.GetRooms());
});

#if DEBUG
app.MapGet("/users", () =>
{
    DevelopmentPlatformService platformService = (DevelopmentPlatformService)app.Services.CreateScope().ServiceProvider.GetRequiredService<IPlatformService>();
    var users = platformService.GetUsers().Select(user => new { Id = user.Id, Token = user.Token });
    return Results.Json(users);
});
#endif

app.Run();
