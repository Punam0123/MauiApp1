using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace MauiApp1
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // Register UserService and BlazorWebView
            builder.Services.AddSingleton<UserService>();
            builder.Services.AddMauiBlazorWebView();


            // Configure HttpClient
            builder.Services.AddSingleton(new HttpClient
            {
                BaseAddress = new Uri("https://your-backend-api-url.com")
            });

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
