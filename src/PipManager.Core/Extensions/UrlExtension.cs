namespace PipManager.Core.Extensions;

public static class UrlExtension
{
    public static bool CheckUrlValid(this string source)
        => Uri.TryCreate(source, UriKind.Absolute, out var uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
}