using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorMarkdownApp;

public partial class MarkdownIt
{
    [Inject]
    private IJSRuntime Js { get; set; } = null!;

    [Parameter]
    public string? Source { get; set; }

    private string? _prevSource;

    private string? MdHtml { get; set; }

    IJSObjectReference? MarkdownItJsObjectReference { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if (_prevSource != Source)
        {
            _prevSource = Source;

            if (MarkdownItJsObjectReference is not null)
            {
                MdHtml = await Render(Source);
            }
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            MarkdownItJsObjectReference = await Js.InvokeAsync<IJSObjectReference>("import", "./js/markdown-it-proxy.min.js");
            MdHtml = await Render(Source);
            StateHasChanged();
        }
    }

    private async Task<string> Render(string? src)
    {
        return await MarkdownItJsObjectReference!.InvokeAsync<string>("render", src);
    }
}
