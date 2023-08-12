using Microsoft.AspNetCore.Components;
namespace Client.Components;

public partial class Map : ComponentBase
{
    [Parameter, EditorRequired]
    public BlazorMap Data { get; set; } = default!;
}
