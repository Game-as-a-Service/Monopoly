using Microsoft.AspNetCore.Components;
namespace Client.Components;

public partial class Map : ComponentBase
{
    [Parameter, EditorRequired]
    public BlazorMap Data { get; set; } = default!;

    [Parameter]
    public string Width { get; set; } = "100%";
    [Parameter]
    public string Height { get; set; } = "100%";
}
