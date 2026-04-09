using ADaxer.MvvmNav.Avalonia.Extensions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using LiveMarkdown.Avalonia;

namespace ADaxer.MvvmNav.Sample.Avalonia.Views;

public sealed class MarkdownView : UserControl
{
    public static readonly StyledProperty<string?> MarkdownProperty =
        AvaloniaProperty.Register<MarkdownView, string?>(nameof(Markdown));

    public static readonly StyledProperty<string?> ImageBasePathProperty =
        AvaloniaProperty.Register<MarkdownView, string?>(nameof(ImageBasePath));

    private readonly MarkdownRenderer _renderer;
    private readonly ObservableStringBuilder _builder = new();

    public MarkdownView()
    {
        _renderer = new MarkdownRenderer
        {
            MarkdownBuilder = _builder
        };

        // Das ist jetzt wirklich "Content ist ein MarkdownRenderer"
        Content = _renderer;

        // Re-render bei Änderungen
        this.GetObservable(MarkdownProperty).Subscribe(SetMarkdown);
        this.GetObservable(ImageBasePathProperty).Subscribe(p => _renderer.ImageBasePath = p);
    }

    public string? Markdown
    {
        get => GetValue(MarkdownProperty);
        set => SetValue(MarkdownProperty, value);
    }

    public string? ImageBasePath
    {
        get => GetValue(ImageBasePathProperty);
        set => SetValue(ImageBasePathProperty, value);
    }

    private void SetMarkdown(string? markdown)
    {
        _builder.Clear();
        if (!string.IsNullOrWhiteSpace(markdown))
            _builder.Append(markdown.Trim());
    }
}

