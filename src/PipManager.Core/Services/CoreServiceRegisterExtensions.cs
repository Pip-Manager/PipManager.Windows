using System.Net;
using Microsoft.Extensions.DependencyInjection;

namespace PipManager.Core.Services;

public static class CoreServiceRegisterExtensions
{
    public static void AddHttpClient(this IServiceCollection services, string appVersion)
    {
        services.AddTransient(_ =>
        {
            var httpClientHandler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = new CookieContainer(),
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
                AutomaticDecompression = DecompressionMethods.All
            };
            var client = new HttpClient(httpClientHandler) { DefaultRequestVersion = HttpVersion.Version20 };
            client.DefaultRequestHeaders.Add("User-Agent", "null");
            client.Timeout = TimeSpan.FromSeconds(10);
            return client;
        });
    }
}