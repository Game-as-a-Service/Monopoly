using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Client.Pages;

public partial class Index
{
    [Inject] private IDialogService DialogService { get; set; }

    private string state = "Message box hasn't been opened yet";

    private async void OnButtonClicked()
    {
        bool? result = await DialogService.ShowMessageBox(
            "Warning",
            "Deleting can not be undone!",
            yesText: "Delete!", cancelText: "Cancel");
        state = result == null ? "Canceled" : "Deleted!";
        StateHasChanged();
    }
}