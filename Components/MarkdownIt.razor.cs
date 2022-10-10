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
                MdHtml = await Render(Source);
            }
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            MarkdownItJsObjectReference = await Js.InvokeAsync<IJSObjectReference>("import", "./js/markdown-it-proxy.min.js");
            await Init(TagClassMap, Key);
            MdHtml = await Render(Source);
            StateHasChanged();
        }
    }

    private async Task Init(Dictionary<string, string>? tagClassMap = null, string? key = null)
    {
        tagClassMap ??= new Dictionary<string, string>();
        await MarkdownItJsObjectReference!.InvokeVoidAsync("init", tagClassMap, key);
    }

    private async Task<string> Render(string? src)
    {
        return await MarkdownItJsObjectReference!.InvokeAsync<string>("render", src);
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
