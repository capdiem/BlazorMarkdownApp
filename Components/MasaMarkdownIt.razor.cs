using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Routing;

namespace BlazorMarkdownApp;

public partial class MasaMarkdownIt
{
    [Parameter]
    public string? Source { get; set; }

    private List<(string, object)> _renderList = new();

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Render(Source);
    }

    private void Render(string? src)
    {
        _renderList.Clear();

        var regex = new Regex(@"\[\w+:*\w*\]\s*:\s*#\s*\(.+\)", RegexOptions.Multiline);

        var matches = regex.Matches(src);

        int startIndex = 0;

        foreach (Match match in matches)
        {
            var prev = src.Substring(startIndex, match.Index - startIndex);
            if (!string.IsNullOrWhiteSpace(prev))
            {
                _renderList.Add(("md", prev));
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

                bool exists = false;
                
                // TODO: alert type

                if (_renderList.Any())
                {
                    var last = _renderList.Last();
                    if (last.Item1 == component && last.Item2 is Dictionary<string, object> dict)
                    {
                        if (!dict.ContainsKey(prop))
                        {
                            dict.Add(prop, FormatValue(prop, value));
                            exists = true;
                        }
                    }
                }

                if (!exists)
                {
                    var dict = new Dictionary<string, object>() { { prop, FormatValue(prop, value) } };
                    _renderList.Add((component, dict));
                }
            }
            else
            {
                _renderList.Add((key, value));
            }

            startIndex = match.Index + match.Length;
        }

        if (src.Length > startIndex)
        {
            var md = src.Substring(startIndex);
            if (!string.IsNullOrWhiteSpace(md))
            {
                _renderList.Add(("md", md.Trim()));
            }
        }
    }

    private object FormatValue(string prop, string value)
    {
        object newValue;
        if (prop == "childContent")
        {
            newValue = (RenderFragment)(builder => { builder.AddContent(0, value); });
        }
        else
        {
            newValue = value;
        }

        return newValue;
    }

    private RenderFragment GetRenderFragment(string demoType)
    {
        var executingDemoTypeStr = $"{Assembly.GetExecutingAssembly().GetName().Name}.{demoType}";
        var executingDemoType = Type.GetType(executingDemoTypeStr);

        return (builder =>
        {
            builder.OpenComponent(0, executingDemoType);
            builder.CloseComponent();
        });
    }
}
