using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using ColorCode;
using ColorCode.Common;
using ColorCode.Parsing;
using ColorCode.Styling;

namespace PipManager.Desktop.Controls.Markdown;

public class AvaloniaSyntaxHighLighting(StyleDictionary? styles = null, ILanguageParser? languageParser = null)
    : CodeColorizerBase(styles, languageParser)
{
    private InlineCollection? InlineCollection { get; set; }

    public void FormatTextBlock(string sourceCode, ILanguage? language, TextBlock textBlock)
    {
        FormatInlines(sourceCode, language, textBlock.Inlines!);
    }

    private void FormatInlines(string sourceCode, ILanguage? language, InlineCollection inlines)
    {
        InlineCollection = inlines;

        if (language != null)
        {
            languageParser.Parse(sourceCode, language, Write);
        }
        else
        {
            CreateSpan(sourceCode, null);
        }
    }

    protected override void Write(string parsedSourceCode, IList<Scope> scopes)
    {
        var styleInsertions = new List<TextInsertion>();

        foreach (var scope in scopes)
            GetStyleInsertionsForCapturedStyle(scope, styleInsertions);

        styleInsertions.SortStable((x, y) => x.Index.CompareTo(y.Index));

        var offset = 0;

        Scope? previousScope = null;

        foreach (var styleInsertion in styleInsertions)
        {
            var text = parsedSourceCode.Substring(offset, styleInsertion.Index - offset);
            CreateSpan(text, previousScope);
            if (!string.IsNullOrWhiteSpace(styleInsertion.Text))
            {
                CreateSpan(text, previousScope);
            }
            offset = styleInsertion.Index;

            previousScope = styleInsertion.Scope;
        }

        var remaining = parsedSourceCode.Substring(offset);
        // Ensures that those loose carriages don't run away!
        if (remaining != "\r")
        {
            CreateSpan(remaining, null);
        }
    }

    private void CreateSpan(string text, Scope? scope)
    {
        var span = new Span();
        var run = new Run
        {
            Text = text
        };

        // Styles and writes the text to the span.
        if (scope != null)
            StyleRun(run, scope);
        span.Inlines.Add(run);

        InlineCollection?.Add(span);
    }

    private void StyleRun(Run run, Scope scope)
    {
        string? foreground = null;
        var italic = false;
        var bold = false;

        if (Styles.Contains(scope.Name))
        {
            var style = Styles[scope.Name];

            foreground = style.Foreground;
            italic = style.Italic;
            bold = style.Bold;
        }

        if (!string.IsNullOrWhiteSpace(foreground))
        {
            try
            {
                if (Color.TryParse(foreground, out var color))
                {
                    run.Foreground = new SolidColorBrush(color);
                }
            }
            catch
            {
                // ignored
            }
        }

        //Background isn't supported, but a workaround could be created.

        if (italic)
            run.FontStyle = FontStyle.Italic;

        if (bold)
            run.FontWeight = FontWeight.Bold;
    }

    private static void GetStyleInsertionsForCapturedStyle(Scope scope, ICollection<TextInsertion> styleInsertions)
    {
        styleInsertions.Add(new TextInsertion
        {
            Index = scope.Index,
            Scope = scope
        });

        foreach (var childScope in scope.Children)
            GetStyleInsertionsForCapturedStyle(childScope, styleInsertions);

        styleInsertions.Add(new TextInsertion
        {
            Index = scope.Index + scope.Length
        });
    }
}