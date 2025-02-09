using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Markdig;

namespace PipManager.Desktop.Controls.Markdown;

public class MarkdownViewer : TemplatedControl
{
    private readonly IMarkdownAvaloniaRenderer _renderer = App.GetService<IMarkdownAvaloniaRenderer>();

    public static readonly StyledProperty<string> ContentProperty =
        AvaloniaProperty.Register<MarkdownViewer, string>(nameof(Content), string.Empty);

    public string Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public static readonly StyledProperty<object?> RenderedContentProperty =
        AvaloniaProperty.Register<MarkdownViewer, object?>(nameof(RenderedContent));

    private object? RenderedContent
    {
        get => GetValue(RenderedContentProperty);
        set => SetValue(RenderedContentProperty, value);
    }
    
    private async Task RenderProcessAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var content = Content;

            var doc =
                await Task.Run(() =>
                {
                    var doc = Markdig.Markdown.Parse(
                        content,
                        new MarkdownPipelineBuilder()
                            .UseEmphasisExtras()
                            .UseGridTables()
                            .UsePipeTables()
                            .UseTaskLists()
                            .UseAutoLinks()
                            .Build());

                    return doc;
                });

            var contentControl = new ContentControl();

            RenderedContent = contentControl;

            Dispatcher.UIThread.InvokeAsync(() =>
            {
                _renderer.RenderDocumentTo(contentControl, doc, cancellationToken);
            });
        }
        catch
        {
            // ignored
        }
    }

    private Task? _renderProcessTask;
    private CancellationTokenSource? _renderProcessCancellation;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Sender is not MarkdownViewer markdownViewer || change.Property != ContentProperty)
            return;

        if (markdownViewer._renderProcessCancellation is { } cancellation)
            cancellation.Cancel();

        cancellation = 
            markdownViewer._renderProcessCancellation =
                new CancellationTokenSource();

        markdownViewer._renderProcessTask = markdownViewer.RenderProcessAsync(cancellation.Token);
    }
}