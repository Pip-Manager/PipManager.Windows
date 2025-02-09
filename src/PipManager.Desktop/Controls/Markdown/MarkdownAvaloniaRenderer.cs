using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using ColorCode.Styling;
using Markdig.Extensions.Tables;
using Markdig.Extensions.TaskLists;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using AvaloniaDocs = Avalonia.Controls.Documents;

namespace PipManager.Desktop.Controls.Markdown;

public class MarkdownAvaloniaRenderer: IMarkdownAvaloniaRenderer
{
    public Control RenderDocument(MarkdownDocument document, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return new Control();

        var documentElement = new StackPanel();

        foreach (var renderedBlock in RenderBlocks(document, cancellationToken).TakeWhile(_ => !cancellationToken.IsCancellationRequested))
        {
            documentElement.Children.Add(renderedBlock);
        }

        return documentElement;
    }

    public void RenderDocumentTo(ContentControl target, MarkdownDocument document, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return;

        var documentElement = new StackPanel();
        target.Content = documentElement;

        foreach (var renderedBlock in RenderBlocks(document, cancellationToken).TakeWhile(_ => !cancellationToken.IsCancellationRequested))
        {
            documentElement.Children.Add(renderedBlock);
        }
    }

    private List<Control> RenderBlocks(IEnumerable<Block> blocks, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return [];

        var elements = new List<Control>();
        Control? tailElement = null;

        foreach (var block in blocks)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            var renderedBlock = RenderBlock(block, cancellationToken);

            elements.Add(renderedBlock);
            tailElement = renderedBlock;
        }

        if (tailElement != null)
            tailElement.Margin = new Thickness(tailElement.Margin.Left, tailElement.Margin.Top, tailElement.Margin.Right, 0);

        return elements;
    }

    private Control RenderBlock(Block block, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return new Control();

        return block switch
        {
            ParagraphBlock paragraphBlock => RenderParagraphBlock(paragraphBlock, cancellationToken),
            HeadingBlock headingBlock => RenderHeadingBlock(headingBlock, cancellationToken),
            QuoteBlock quoteBlock => RenderQuoteBlock(quoteBlock, cancellationToken),
            FencedCodeBlock fencedCodeBlock => RenderFencedCodeBlock(fencedCodeBlock, cancellationToken),
            CodeBlock codeBlock => RenderCodeBlock(codeBlock, cancellationToken),
            HtmlBlock htmlBlock => RenderHtmlBlock(htmlBlock, cancellationToken),
            ThematicBreakBlock thematicBreakBlock => RenderThematicBreakBlock(thematicBreakBlock, cancellationToken),
            ListBlock listBlock => RenderListBlock(listBlock, cancellationToken),
            Table table => RenderTable(table, cancellationToken),
            ContainerBlock containerBlock => RenderContainerBlock(containerBlock, cancellationToken),
            _ => new TextBlock()
        };
    }

    private Control RenderContainerBlock(ContainerBlock containerBlock, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return new Control();

        var documentElement = new StackPanel
        {
            Margin = new Thickness(0, 0, 0, 12)
        };

        foreach (var renderedBlock in RenderBlocks(containerBlock, cancellationToken))
        {
            if (cancellationToken.IsCancellationRequested)
                return new Control();

            documentElement.Children.Add(renderedBlock);
        }

        return documentElement;
    }

    private Control RenderTable(Table table, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return new Control();

        var tableElement = new Border
        {
            BorderThickness = new Thickness(0, 0, 1, 1),
            Margin = new Thickness(0, 0, 0, 12)
        };

        var tableContentElement = new Grid();

        tableElement.Child = tableContentElement;

        foreach (var _ in table.ColumnDefinitions)
        {
            if (cancellationToken.IsCancellationRequested)
                return new Control();

            tableContentElement.ColumnDefinitions.Add(
                new ColumnDefinition
                {
                    Width = GridLength.Auto
                });
        }

        var rowIndex = 0;
        foreach (var block in table)
        {
            if (cancellationToken.IsCancellationRequested)
                return new Control();

            if (block is not TableRow row)
                continue;

            tableContentElement.RowDefinitions.Add(
                new RowDefinition
                {
                    Height = GridLength.Auto
                });

            var colIndex = 0;
            foreach (var colBlock in row)
            {
                if (colBlock is not TableCell cell)
                    continue;

                var cellElement = new Border
                {
                    BorderThickness = new Thickness(1, 1, 0, 0),
                    Padding = new Thickness(6, 3)
                };

                var cellContentElement = RenderBlock(cell, cancellationToken);

                cellElement.Child = cellContentElement;

                cellContentElement.Margin = new Thickness(0);

                Grid.SetRow(cellElement, rowIndex);
                Grid.SetColumn(cellElement, colIndex);

                tableContentElement.Children.Add(cellElement);

                colIndex++;
            }

            rowIndex++;
        }

        return tableElement;
    }

    private Control RenderListBlock(ListBlock listBlock, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return new Control();

        var itemCount = listBlock.Count;

        Func<int, string> markerTextGetter = listBlock.IsOrdered ?
            index => $"{index+1}." :
            _ => "-";

        var listElement = new Border
        {
            Margin = new Thickness(6, 0, 0, 12)
        };

        var listContentElement = new Grid();

        listElement.Child =
            listContentElement;

        listContentElement.ColumnDefinitions.Add(
            new ColumnDefinition
            {
                Width = GridLength.Auto,
            });

        listContentElement.ColumnDefinitions.Add(
            new ColumnDefinition());

        for (int i = 0; i < itemCount; i++)
        {
            if (cancellationToken.IsCancellationRequested)
                return new Control();

            listContentElement.RowDefinitions.Add(
                new RowDefinition()
                {
                    Height = GridLength.Auto
                });
        }

        int index = 0;
        Control? lastRenderedItemBlock = null;
        foreach (var itemBlock in listBlock)
        {
            if (cancellationToken.IsCancellationRequested)
                return new Control();

            if (RenderBlock(itemBlock, cancellationToken) is not { } renderedItemBlock) continue;
            lastRenderedItemBlock = renderedItemBlock;
            renderedItemBlock.Margin = new Thickness(renderedItemBlock.Margin.Left, renderedItemBlock.Margin.Top, renderedItemBlock.Margin.Right, renderedItemBlock.Margin.Bottom / 4);

            TextBlock marker = new TextBlock();
            Grid.SetRow(marker, index);
            Grid.SetColumn(marker, 0);
            marker.Text = markerTextGetter.Invoke(index);
            marker.Margin = new Thickness(0, 0, 6, 0);
            marker.TextAlignment = TextAlignment.Right;

            Grid.SetRow(renderedItemBlock, index);
            Grid.SetColumn(renderedItemBlock, 1);

            listContentElement.Children.Add(marker);
            listContentElement.Children.Add(renderedItemBlock);

            index++;
        }

        if (lastRenderedItemBlock != null)
            lastRenderedItemBlock.Margin = new Thickness(lastRenderedItemBlock.Margin.Left, lastRenderedItemBlock.Margin.Top, lastRenderedItemBlock.Margin.Right, 0);
        
        return listElement;
    }

    private static Control RenderThematicBreakBlock(ThematicBreakBlock _, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return new Control();

        var thematicBreakElement = new Border
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Height = 1,
            Margin = new Thickness(0, 0, 0, 12)
        };

        return thematicBreakElement;
    }

    private Control RenderFencedCodeBlock(FencedCodeBlock fencedCodeBlock, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return new Control();

        if (string.IsNullOrWhiteSpace(fencedCodeBlock.Info))
            return RenderCodeBlock(fencedCodeBlock, cancellationToken);

        var codeElement = new Border
        {
            CornerRadius = new CornerRadius(3),
            Margin = new Thickness(0, 0, 0, 12),
            Classes = { "Markdown", "FencedCodeBlock" }
        };

        var codeContentElement = new TextBlock
        {
            TextWrapping = TextWrapping.Wrap,
            Padding = new Thickness(15),
            FontFamily = GetCodeTextFontFamily(),
        };

        codeElement.Child =
            codeContentElement;

        if (fencedCodeBlock.Inline != null)
            codeContentElement.Inlines?.AddRange(
                RenderInlines(fencedCodeBlock.Inline, cancellationToken));

        var language = ColorCode.Languages.FindById(fencedCodeBlock.Info);

        var writer = new AvaloniaSyntaxHighLighting(StyleDictionary.DefaultDark);
        writer.FormatTextBlock(fencedCodeBlock.Lines.ToString(), language, codeContentElement);
        
        return codeElement;
    }

    private Control RenderCodeBlock(CodeBlock codeBlock, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return new Control();

        var codeElement = new Border
        {
            CornerRadius = new CornerRadius(3),
            Margin = new Thickness(0, 0, 0, 12)
        };

        var codeContentElement = new TextBlock
        {
            TextWrapping = TextWrapping.Wrap,
            Padding = new Thickness(6),
            FontFamily = GetCodeTextFontFamily(),
        };

        codeElement.Child = codeContentElement;

        if (codeBlock.Inline != null)
            codeContentElement.Inlines?.AddRange(
                RenderInlines(codeBlock.Inline, cancellationToken));

        codeContentElement.Inlines?.Add(
            new AvaloniaDocs.Run(codeBlock.Lines.ToString()));

        return codeElement;
    }

    private Control RenderQuoteBlock(QuoteBlock quoteBlock, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return new Control();

        var quoteElement = new Border
        {
            Classes = { "Markdown", "QuoteBlock" }
        };

        var quoteContentPanel = new StackPanel();
        
        quoteElement.Child = quoteContentPanel;

        foreach (var renderedBlock in RenderBlocks(quoteBlock, cancellationToken))
            quoteContentPanel.Children.Add(renderedBlock);

        return quoteElement;
    }

    private Control RenderHeadingBlock(HeadingBlock headingBlock, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return new Control();
        
        var headingElement = new TextBlock
        {
            FontWeight = FontWeight.DemiBold,
            Classes = { "Markdown", "Heading" + headingBlock.Level },
            Margin = new Thickness(0, 0, 0, 12)
        };

        if (headingBlock.Inline != null)
            headingElement.Inlines?.AddRange(
                RenderInlines(headingBlock.Inline, cancellationToken));

        return headingElement;
    }

    private Control RenderParagraphBlock(ParagraphBlock paragraphBlock, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return new Control();

        var paragraphElement = new TextBlock()
        {
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, 12)
        };

        if (paragraphBlock.Inline != null)
            paragraphElement.Inlines?.AddRange(
                RenderInlines(paragraphBlock.Inline, cancellationToken));

        return paragraphElement;
    }

    private List<AvaloniaDocs.Inline> RenderInlines(IEnumerable<Inline> inlines, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return [];

        List<AvaloniaDocs.Inline> inlineElements = [];

        foreach (var inline in inlines)
            if (RenderInline(inline, cancellationToken) is { } wpfInline)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                inlineElements.Add(wpfInline);
            }

        return inlineElements;
    }

    private AvaloniaDocs.Inline RenderInline(Inline inline, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return new AvaloniaDocs.Run();

        return inline switch
        {
            LiteralInline literalInline => RenderLiteralInline(literalInline, cancellationToken),
            LinkInline linkInline => RenderLinkInline(linkInline, cancellationToken),
            LineBreakInline lineBreakInline => RenderLineBreakInline(lineBreakInline, cancellationToken),
            HtmlInline htmlInline => RenderHtmlInline(htmlInline, cancellationToken),
            HtmlEntityInline htmlEntityInline => RenderHtmlEntityInline(htmlEntityInline, cancellationToken),
            EmphasisInline emphasisInline => RenderEmphasisInline(emphasisInline, cancellationToken),
            CodeInline codeInline => RenderCodeInline(codeInline, cancellationToken),
            AutolinkInline autolinkInline => RenderAutolinkInline(autolinkInline, cancellationToken),
            DelimiterInline delimiterInline => RenderDelimiterInline(delimiterInline, cancellationToken),
            ContainerInline containerInline => RenderContainerInline(containerInline, cancellationToken),
            TaskList taskListInline => RenderTaskListInline(taskListInline, cancellationToken),
            _ => new AvaloniaDocs.Run()
        };
    }

    private AvaloniaDocs.Inline RenderTaskListInline(TaskList taskListInline, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return new AvaloniaDocs.Run();

        return new CheckBox
        {
            IsChecked = taskListInline.Checked,
            IsEnabled = false,
        }.WrapWithContainer();
    }

    private AvaloniaDocs.Inline RenderAutolinkInline(AutolinkInline autolinkInline, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return new AvaloniaDocs.Run();

        return new AvaloniaDocs.Run(autolinkInline.Url);
    }

    private static AvaloniaDocs.Inline RenderCodeInline(CodeInline codeInline, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return new AvaloniaDocs.Run();

        var border = new Border
        {
            CornerRadius = new CornerRadius(5),
            Padding = new Thickness(5, 0),
            Margin = new Thickness(3, 0),
            Classes = { "Markdown", "CodeInline" }
        };
        var textBlock = new TextBlock();

        border.Child = textBlock;

        textBlock.Text = codeInline.Content;

        return border.WrapWithContainer();
    }

    private AvaloniaDocs.Inline RenderContainerInline(ContainerInline containerInline, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return new AvaloniaDocs.Run();

        var span = new AvaloniaDocs.Span();

        span.Inlines.AddRange(
            RenderInlines(containerInline, cancellationToken));

        return span;
    }

    private AvaloniaDocs.Inline RenderEmphasisInline(EmphasisInline emphasisInline, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return new AvaloniaDocs.Run();

        var span = new AvaloniaDocs.Span();

        switch (emphasisInline.DelimiterChar)
        {
            case '*' when emphasisInline.DelimiterCount == 2: // bold
            case '_' when emphasisInline.DelimiterCount == 2: // bold
                span.FontWeight = FontWeight.Bold;
                break;
            case '*': // italic
            case '_': // italic
                span.FontStyle = FontStyle.Italic;
                break;
            case '~': // 2x strike through, 1x subscript
                if (emphasisInline.DelimiterCount == 2)
                    span.TextDecorations = TextDecorations.Strikethrough;
                else
                    span.BaselineAlignment = BaselineAlignment.Subscript;
                break;
            case '^': // 1x superscript
                span.BaselineAlignment = BaselineAlignment.Superscript;
                break;
            case '+': // 2x underline
                span.TextDecorations = TextDecorations.Underline;
                break;
            case '=': // 2x Marked
                // TODO: Implement Marked
                break;
        }

        span.Inlines.AddRange(
            RenderInlines(emphasisInline, cancellationToken));

        return span;
    }

    private AvaloniaDocs.Inline RenderHtmlEntityInline(HtmlEntityInline htmlEntityInline, CancellationToken cancellationToken)
        => cancellationToken.IsCancellationRequested ? new AvaloniaDocs.Run() : new AvaloniaDocs.Run(htmlEntityInline.Transcoded.ToString());

    // TODO: Implement HTML rendering
    private AvaloniaDocs.Inline RenderHtmlInline(HtmlInline _, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return new AvaloniaDocs.Run();

        return new AvaloniaDocs.Run();
    }

    private Control RenderHtmlBlock(HtmlBlock _, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return new Control();

        return new TextBlock();
    }

    private AvaloniaDocs.Inline RenderLineBreakInline(LineBreakInline _, CancellationToken cancellationToken)
        => cancellationToken.IsCancellationRequested ? new AvaloniaDocs.Run() : new AvaloniaDocs.Run("\n");

    private AvaloniaDocs.Inline RenderDelimiterInline(DelimiterInline delimiterInline, CancellationToken cancellationToken)
        => cancellationToken.IsCancellationRequested ? new AvaloniaDocs.Run() : new AvaloniaDocs.Run(delimiterInline.ToLiteral());

    private AvaloniaDocs.Inline RenderLinkInline(LinkInline linkInline, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return new AvaloniaDocs.Run();

        Uri? uri = null;

        if (linkInline.Url != null && Uri.TryCreate(linkInline.Url, UriKind.RelativeOrAbsolute, out var result))
            uri = result;

        if (linkInline.IsImage)
        {
            var img = new Image
            {
                MaxWidth = 300
            };
            
            img.DoubleTapped += (_, _) =>
            {
                Console.WriteLine(uri?.ToString());
            };
            
            if (uri != null)
                AsyncImageLoader.ImageLoader.SetSource(img, uri.ToString());

            return img.WrapWithContainer();
        }
        else
        {
            // TODO: Workaround for Hyperlink not supporting nested inlines, Avalonia 12.0 will fix this
            var inlineHyperlink = new InlineHyperlink(linkInline.Url);

            AvaloniaDocs.Inline? linkContent = null;

            if (linkInline.Label != null)
                linkContent = new AvaloniaDocs.Run(linkInline.Label);

            if (linkContent != null)
                inlineHyperlink.Content.Inlines.Add(linkContent);
            if (RenderContainerInline(linkInline, cancellationToken) is { } extraInline)
                inlineHyperlink.Content.Inlines.Add(extraInline);

            return inlineHyperlink;
        }
    }

    private AvaloniaDocs.Inline RenderLiteralInline(LiteralInline literalInline, CancellationToken cancellationToken)
        => cancellationToken.IsCancellationRequested ? new AvaloniaDocs.Run() : new AvaloniaDocs.Run(literalInline.Content.ToString());

    private FontFamily GetCodeTextFontFamily()
    {
        return new FontFamily("ui-monospace,Cascadia Code,Segoe UI Mono,Liberation Mono,Menlo,Monaco,Consolas,monospace");
    }
}