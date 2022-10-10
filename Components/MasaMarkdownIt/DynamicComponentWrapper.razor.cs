using Microsoft.AspNetCore.Components;

namespace BlazorMarkdownApp;

public partial class DynamicComponentWrapper
{
    [Parameter, EditorRequired]
    public Type Type { get; set; } = default!;

    [Parameter]
    public IDictionary<string, object>? Parameters { get; set; }

    private bool _needRender;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            await Task.Delay(100);
            _needRender = true;
            StateHasChanged();
        }
    }
}
