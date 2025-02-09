using System.Threading;
using Avalonia.Controls;
using Markdig.Syntax;

namespace PipManager.Desktop.Controls.Markdown;

public interface IMarkdownAvaloniaRenderer
{
    public void RenderDocumentTo(ContentControl target, MarkdownDocument document, CancellationToken cancellationToken);
}