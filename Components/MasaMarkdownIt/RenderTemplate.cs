namespace BlazorMarkdownApp;

public class RenderTemplate
{
    public Type Type { get; }

    public string? Source { get; }

    public Dictionary<string, object>? Parameters { get; }

    public RenderTemplate(Type type)
    {
        Type = type;
    }

    public RenderTemplate(string source)
    {
        Type = typeof(string);
        Source = source;
    }

    public RenderTemplate(Type type, Dictionary<string, object> parameters) : this(type)
    {
        Parameters = parameters;
    }
}
