using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorMarkdownApp;

public partial class MarkdownIt : IAsyncDisposable
{
    [Inject]
    private IJSRuntime Js { get; set; } = null!;

    [Parameter]
    public string? Key { get; set; }

    [Parameter]
    public Action<MarkdownItOptions>? Options { get; set; }

    [Parameter]
    public string? Source { get; set; }

    [Parameter]
    public Dictionary<string, string>? TagClassMap { get; set; }

    private string? _prevSource;

    private string MdHtml { get; set; } = string.Empty;

    IJSObjectReference? MarkdownItJsObjectReference { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if (_prevSource != Source)
        {
            _prevSource = Source;

            if (MarkdownItJsObjectReference is not null)
            {
                await Render();
            }
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            MarkdownItJsObjectReference = await Js.InvokeAsync<IJSObjectReference>("import", "./js/markdown-it-proxy.min.js");
            await Init();
            await Render();
        }
    }

    private async Task Init()
    {
        var options = new MarkdownItOptions();
        Options?.Invoke(options);

        var tagClassMap = TagClassMap ?? new Dictionary<string, string>();

        await MarkdownItJsObjectReference!.InvokeVoidAsync("init", options, tagClassMap, Key);
    }

    private async Task Render()
    {
        MdHtml = await MarkdownItJsObjectReference!.InvokeAsync<string>("render", Source);
        StateHasChanged();
    }

    public async ValueTask DisposeAsync()
    {
        if (MarkdownItJsObjectReference is not null)
        {
            try
            {
                await MarkdownItJsObjectReference.DisposeAsync();
            }
            catch
            {
                // ignored
            }
        }
    }
}
