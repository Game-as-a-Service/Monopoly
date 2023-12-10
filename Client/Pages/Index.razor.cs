﻿using Client.Options;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using SharedLibrary;

namespace Client.Pages;

public partial class Index
{
    [Parameter] public string Id { get; set; } = null!;

    [Parameter, SupplyParameterFromQuery(Name = "token")]
    public string AccessToken { get; set; } = default!;
    [Inject] private IOptions<BackendApiOptions> BackendApiOptions { get; set; } = default!;

    private HubConnection Client { get; set; } = default!;

    private List<string> Messages { get; } = [];
    public bool IsGameStarted { get; set; } = false;

    protected override async Task OnInitializedAsync()
    {
        var baseUri = new Uri(BackendApiOptions.Value.BaseUrl);
        var url = new Uri(baseUri, $"/monopoly?gameid={Id}");
        Client = new HubConnectionBuilder()
            .WithUrl(url, options =>
            {
                options.AccessTokenProvider = async () => await Task.FromResult(AccessToken);
            })
            .Build();
        SetupHubConnection();
        try
        {
            await Client.StartAsync();
            Messages.Add("連線成功!");
        }
        catch (Exception ex)
        {
            Messages.Add(ex.Message);
        }
    }

    private void SetupHubConnection()
    {
        Client.Closed += async (exception) =>
        {
            var errorMessage = exception?.Message;
            Messages.Add($"中斷連線: {errorMessage}");

            await Task.CompletedTask;
        };

        TypedHubConnection<IMonopolyResponses> connection = new(Client);

        connection.On(x => x.PlayerJoinGameEvent, (string id) =>
        {
            Messages.Add($"player {id} joined game!");
            StateHasChanged();
        });
    }
}