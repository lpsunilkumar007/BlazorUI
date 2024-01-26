using Blazored.LocalStorage;
using Maui.Hyb.Data;
using Microsoft.Extensions.Logging;
using MudBlazor;
using MudBlazor.Services;
using Portal.Services;

namespace Maui.Hyb
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

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif
            builder.Services.AddHttpClient("PortalApiHttpClient", httpClient =>
            {
                var url = "https://localhost:5001";
                //builder.Configuration["PortalApiUrl"];
                if (string.IsNullOrEmpty(url))
                    throw new Exception("ApiUrl has not been correctly configured. Aborting startup...");
                httpClient.BaseAddress = new Uri(url);
            });
            builder.Services.AddAuthorizationCore();
            builder.Services.AddBlazoredLocalStorage();

            builder.Services.AddPortalServices();
            builder.Services
                .AddMudServices(configuration =>
                {
                    configuration.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
                    configuration.SnackbarConfiguration.HideTransitionDuration = 100;
                    configuration.SnackbarConfiguration.ShowTransitionDuration = 100;
                    configuration.SnackbarConfiguration.VisibleStateDuration = 3000;
                    configuration.SnackbarConfiguration.ShowCloseIcon = false;
                });
            return builder.Build();
        }
    }
}
