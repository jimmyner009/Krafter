namespace Krafter.UI.Web.Client.Features.Appearance;

public partial class AppearancePage(
        ThemeService themeService,
        CookieThemeService cookieThemeService
    ) : ComponentBase
    {
        void ChangeTheme(string value)
        {
            themeService.SetTheme(value);
            //  CookieThemeService.
        }

        void ChangeRightToLeft(bool value)
        {
            themeService.SetRightToLeft(value);
        }

        void ChangeWcag(bool value)
        {
            themeService.SetWcag(value);
        }
    }