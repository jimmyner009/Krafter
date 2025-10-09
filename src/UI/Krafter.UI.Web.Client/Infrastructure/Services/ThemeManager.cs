using Microsoft.JSInterop;

namespace Krafter.UI.Web.Client.Infrastructure.Services
{
    public class ThemeManager
    {
        private readonly ThemeService _themeService;
        private readonly IJSRuntime _jsRuntime;

        public event Func<string, Task> ThemeChangeRequested;

        public enum ThemePreference
        {
            Auto,
            Dark,
            Light
        }

        public ThemePreference CurrentPreference { get; private set; } = ThemePreference.Auto;
        public string SystemTheme { get; set; }

        public ThemeManager(ThemeService themeService, IJSRuntime jsRuntime)
        {
            _themeService = themeService;
            _jsRuntime = jsRuntime;
        }

        public async Task SetThemePreference(ThemePreference preference)
        {
            CurrentPreference = preference;

            string preferenceValue = preference switch
            {
                ThemePreference.Auto => "auto",
                ThemePreference.Dark => "dark",
                ThemePreference.Light => "light",
                _ => "auto"
            };

            // Store the preference
            await _jsRuntime.InvokeVoidAsync("setStoredThemePreference", preferenceValue);

            // Apply the theme
            string _systemTheme = preferenceValue;
            if (preference == ThemePreference.Auto)
            {
                // For auto, get the current system theme
                var systemTheme = await _jsRuntime.InvokeAsync<string>("detectSystemTheme");
                _systemTheme = systemTheme;
                _themeService.SetTheme(MapSystemThemeToAppTheme(systemTheme));
            }
            else
            {
                // For explicit themes, apply directly
                _themeService.SetTheme(MapPreferenceToTheme(preference));
            }
            SystemTheme = _systemTheme;
            await (ThemeChangeRequested?.Invoke(_systemTheme) ?? Task.CompletedTask);
        }

        public async Task SetThemePreference(bool isDark)
        {
            if (isDark)
            {
                await _jsRuntime.InvokeVoidAsync("setStoredThemePreference", "dark");
                CurrentPreference = ThemePreference.Dark;
            }
            else
            {
                await _jsRuntime.InvokeVoidAsync("setStoredThemePreference", "light");
                CurrentPreference = ThemePreference.Light;
            }
        }

        public async Task OnSystemThemeChanged(string systemTheme)
        {
            // Only update theme if preference is set to auto
            if (CurrentPreference == ThemePreference.Auto)
            {
                var appTheme = MapSystemThemeToAppTheme(systemTheme);
                _themeService.SetTheme(appTheme);
                SystemTheme = systemTheme;
                await (ThemeChangeRequested?.Invoke(systemTheme) ?? Task.CompletedTask);
            }
        }

        private string MapSystemThemeToAppTheme(string systemTheme)
        {
            var currentTheme = _themeService.Theme;
            if (systemTheme == "dark" || systemTheme == "auto")
            {
                if (currentTheme.Contains("dark"))
                {
                    return currentTheme;
                }
                else
                {
                    return currentTheme + "-dark";
                }
            }
            else
            {
                return currentTheme;
            }
        }

        private string MapPreferenceToTheme(ThemePreference preference)
        {
            var currentTheme = _themeService.Theme;
            if (preference == ThemePreference.Dark || preference == ThemePreference.Auto)
            {
                if (currentTheme.Contains("dark"))
                {
                    return currentTheme;
                }
                else
                {
                    return currentTheme + "-dark";
                }
            }
            else
            {
                return currentTheme;
            }
        }
    }
}