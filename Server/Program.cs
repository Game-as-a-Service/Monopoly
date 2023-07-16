using Application;
using Application.Common;
using Application.Usecases;
using Domain.Common;
using Microsoft.AspNetCore.Mvc;
using Server;
using Server.Hubs;
using Server.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IRepository, InMemoryRepository>();
builder.Services.AddSingleton<IEventBus<DomainEvent>, MonopolyEventBus>();
builder.Services.AddMonopolyApplication();
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});
builder.Services.AddCors(options => options.AddPolicy("CorsPolicy",
        builder =>
        {
            builder.AllowAnyHeader()
                   .AllowAnyMethod()
                   .SetIsOriginAllowed((host) => true)
                   .AllowCredentials();
        }));

var app = builder.Build();

app.UseCors("CorsPolicy");
app.MapHub<MonopolyHub>("/monopoly");

// 開始遊戲
app.MapPost("/", ([FromBody] string[] playerIds) =>
{
    CreateGameUsecase createGameUsecase = app.Services.GetRequiredService<CreateGameUsecase>();
    string gameId = createGameUsecase.Execute(new CreateGameRequest(null, playerIds[0]));
    return $@"https://localhost:7047/{gameId}";
});

app.Run();