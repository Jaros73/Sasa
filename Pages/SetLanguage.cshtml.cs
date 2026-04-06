using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Sasa.Pages
{
    public class SetLanguageModel : PageModel
    {
        public IActionResult OnGet(string kultura, string? navratUrl = null)
        {
            if (string.IsNullOrWhiteSpace(kultura))
            {
                return LocalRedirect(navratUrl ?? "/");
            }

            var hodnotaCookie = CookieRequestCultureProvider.MakeCookieValue(
                new RequestCulture(kultura));

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                hodnotaCookie,
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    IsEssential = true,
                    HttpOnly = false,
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.Lax
                });

            return LocalRedirect(navratUrl ?? "/");
        }
    }
}