using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});

builder.Services
    .AddRazorPages()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

var app = builder.Build();

var podporovaneKultury = new[]
{
    new CultureInfo("cs"),
    new CultureInfo("en"),
    new CultureInfo("de")
};

var lokalizaceOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("cs"),
    SupportedCultures = podporovaneKultury,
    SupportedUICultures = podporovaneKultury
};

lokalizaceOptions.RequestCultureProviders = new List<IRequestCultureProvider>
{
    new RouteDataRequestCultureProvider
    {
        RouteDataStringKey = "kultura",
        UIRouteDataStringKey = "kultura"
    },
    new CookieRequestCultureProvider(),
    new AcceptLanguageHeaderRequestCultureProvider
    {
        MaximumAcceptLanguageHeaderValuesToTry = 3
    }
};

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

/* TADY je to důležité */
app.UseRequestLocalization(lokalizaceOptions);

app.UseAuthorization();

app.MapGet("/", (HttpContext context) =>
{
    var requestCulture = context.Features.Get<IRequestCultureFeature>();
    var kultura = requestCulture?.RequestCulture.Culture.TwoLetterISOLanguageName ?? "cs";

    if (kultura != "cs" && kultura != "en" && kultura != "de")
    {
        kultura = "cs";
    }

    return Results.Redirect($"/{kultura}");
});

app.MapRazorPages();

app.Run();