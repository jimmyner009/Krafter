using Microsoft.AspNetCore.Components;
using Radzen;

namespace Krafter.UI.Web.Components
{
    public partial class App(ThemeService themeService
    )
    {
        [CascadingParameter]
        private HttpContext HttpContext { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (HttpContext != null)
            {
                var theme = HttpContext.Request.Cookies["KrafterTheme"];

                if (!string.IsNullOrEmpty(theme))
                {
                    themeService.SetTheme(theme, false);
                }
            }
        }
    }
}