using Application;
using Application.Common;
using Application.Usecases;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Server;
using Server.DataModels;
using Server.Hubs;
using Server.Services;
using SharedLibrary.MonopolyMap;
using System.Security.Claims;
using Server.Presenters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMonopolyServer();
builder.Services.AddMonopolyApplication();
builder.Services.AddSignalR();

builder.Services.AddCors(options => options.AddPolicy("CorsPolicy",
        configurePolicy =>
        {
            configurePolicy.AllowAnyHeader()
                   .AllowAnyMethod()
                   .SetIsOriginAllowed(_ => true)
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
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme);

var app = builder.Build();

app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<MonopolyHub>("/monopoly");

app.MapGet("/health", () => Results.Ok());

// 開始遊戲
app.MapPost("/games", async (context) =>
{
    var hostId = context.User.FindFirst(ClaimTypes.Sid)!.Value;
    var payload = (await context.Request.ReadFromJsonAsync<CreateGameBodyPayload>())!;
    var createGameUsecase = app.Services.CreateScope().ServiceProvider.GetRequiredService<CreateGameUsecase>();
    var presenter = new DefaultPresenter<CreateGameResponse>();
    await createGameUsecase.ExecuteAsync(
        new CreateGameRequest(hostId, payload.Players.Select(x => x.Id).ToArray()),
        presenter);

    var frontendBaseUrl = app.Configuration["FrontendBaseUrl"]!;

    var url = $@"{frontendBaseUrl}games/{presenter.Value.GameId}";

    await context.Response.WriteAsync(url);
}).RequireAuthorization();
app.MapGet("/map", (string mapId) =>
{
    var projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
    var jsonFilePath = Path.Combine(projectDirectory, "Maps", $"{mapId}.json");

    if (!File.Exists(jsonFilePath))
    {
        return Results.NotFound();
    }

    // read json file
    var json = File.ReadAllText(jsonFilePath);
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
    var platformService = app.Services.CreateScope().ServiceProvider.GetRequiredService<IPlatformService>() as DevelopmentPlatformService;
    var users = platformService?.GetUsers().Select(user => new { user.Id, user.Token });
    return Results.Json(users);
});
#endif

app.Run();