using Application;
using Application.Common;
using Domain.Common;
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

var app = builder.Build();

app.MapHub<MonopolyHub>("/monopoly");

app.Run();