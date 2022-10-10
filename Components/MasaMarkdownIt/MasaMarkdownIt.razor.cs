using System.Reflection;
using System.Text.RegularExpressions;
using BlazorComponent;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;

namespace BlazorMarkdownApp;

public partial class MasaMarkdownIt
{
    [Parameter]
    public string? Source { get; set; }

    private readonly List<RenderTemplate> _renderTemplates = new();

    private bool _analysing = true;

    protected override void OnInitialized()
    {
        base.OnInitialized();
    
        AnalyseSource(Source);
        _analysing = false;
    }

    private void AnalyseSource(string? src)
    {
        if (src is null)
        {
            return;
        }

        _renderTemplates.Clear();

        var regex = new Regex(@"\[\w+:*\w*\]\s*:\s*#\s*\(.+\)", RegexOptions.Multiline);

        var matches = regex.Matches(src);

        int startIndex = 0;

        foreach (Match match in matches)
        {
            var prev = src.Substring(startIndex, match.Index - startIndex);
            if (!string.IsNullOrWhiteSpace(prev))
            {
                UpsertRenderTemplate(prev);
            }

            var line = match.Value;
            var keyMatch = new Regex(@"\[\w+:*\w*\]\s*:\s*#\s*\(").Match(line);
            if (!keyMatch.Success)
            {
                //TODO: Logger
                continue;
            }

            var firstChar = 1;
            var lastChar = keyMatch.Value.LastIndexOf(']');
            var key = keyMatch.Value.Substring(firstChar, lastChar - 1);
            var value = line.Substring(keyMatch.Value.Length);
            value = value.TrimEnd(')');

            if (key.Contains(':'))
            {
                var headerSplits = key.Split(':');
                var component = headerSplits[0];
                var prop = headerSplits[1];

                UpsertRenderTemplate(component, prop, value);
            }
            else
            {
                UpsertRenderTemplate(key, value);
            }

            startIndex = match.Index + match.Length;
        }

        if (src.Length > startIndex)
        {
            var md = src.Substring(startIndex);
            if (!string.IsNullOrWhiteSpace(md))
            {
                UpsertRenderTemplate(md);
            }
        }
    }

    private void UpsertRenderTemplate(string key, string value)
    {
        key = key.ToLowerInvariant();

        if (key == "example")
        {
            var executingDemoTypeStr = $"{Assembly.GetExecutingAssembly().GetName().Name}.{value}";
            var executingDemoType = Type.GetType(executingDemoTypeStr);
            _renderTemplates.Add(new RenderTemplate(executingDemoType!));
        }
    }

    private void UpsertRenderTemplate(string mdSource)
    {
        _renderTemplates.Add(new RenderTemplate(mdSource));
    }

    private void UpsertRenderTemplate(string key, string propName, string propValue)
    {
        key = key.ToLowerInvariant();

        switch (key)
        {
            case "alert":
                GenerateAlert(propName, propValue);
                break;
        }
    }

    private void GenerateAlert(string propName, string propValue)
    {
        var type = typeof(MAlert);

        var last = _renderTemplates.LastOrDefault();
        if (last is not null && last.Type == type)
        {
            if (!last.Parameters!.ContainsKey(propName))
            {
                FormatAlertParameters(propName, propValue, last.Parameters);
                return;
            }
        }

        var parameters = new Dictionary<string, object>();
        FormatAlertParameters(propName, propValue, parameters);
        _renderTemplates.Add(new RenderTemplate(type, parameters));
    }

    private void FormatAlertParameters(string propName, string propValue, Dictionary<string, object> parameters)
    {
        if (propName.Equals(nameof(MAlert.Type), StringComparison.InvariantCultureIgnoreCase))
        {
            var alertType = propValue.ToLowerInvariant() switch
            {
                "error" => AlertTypes.Error,
                "warning" => AlertTypes.Warning,
                "info" => AlertTypes.Info,
                "success" => AlertTypes.Success,
                _ => AlertTypes.None
            };

            parameters.Add(propName, alertType);
        }
        else
        {
            FormatCommonParameters(propName, propValue, parameters);
        }
    }

    private void FormatCommonParameters(string propName, string propValue, Dictionary<string, object> parameters)
    {
        if (propName.Equals("ChildContent", StringComparison.InvariantCultureIgnoreCase))
        {
            parameters.Add(propName, (RenderFragment)(builder => { builder.AddContent(0, propValue); }));
        }
        else
        {
            parameters.Add(propName, propValue);
        }
    }
}
