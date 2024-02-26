using Client.Pages.Ready.Entities;
using Microsoft.AspNetCore.Components;
using SharedLibrary.ResponseArgs;
using SharedLibrary.ResponseArgs.ReadyRoom;

namespace Client.Pages.Ready;

public partial class ReadyPage
{
    public IList<Player> Players { get; set; } = []; // 不要用 IEnumerable，因為會有問題(IEnumerable 會是延遲查詢)
    [Parameter] public string UserId { get; set; } = string.Empty;
    [CascadingParameter] internal TypedHubConnection Connection { get; set; } = default!;
    public Player? CurrentPlayer => Players.FirstOrDefault(x => x.Id == UserId);
    protected override async Task OnInitializedAsync()
    {
        Connection.GetReadyInfoEventHandler += OnGetReadyInfoEvent;
        await Connection.GetReadyInfo();
    }
    public void Update() => StateHasChanged();
    private void OnGetReadyInfoEvent(GetReadyInfoEventArgs e)
    {
        Players = e.Players.Select(x => new Player
        {
            Id = x.Id,
            Name = x.Name,
            IsReady = x.IsReady,
            IsHost = e.HostId == x.Id,
            Color = (ColorEnum)Enum.Parse(typeof(ColorEnum), x.Color.ToString()),
            Role = (RoleEnum)Enum.Parse(typeof(RoleEnum), x.Color.ToString())
        }).ToList();
        Update();
    }
}