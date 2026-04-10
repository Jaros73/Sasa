using System.Globalization;
using System.Net;
using Microsoft.AspNetCore.HttpOverrides;
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

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto |
        ForwardedHeaders.XForwardedHost;

    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();

    options.KnownIPNetworks.Add(
        new System.Net.IPNetwork(IPAddress.Parse("10.89.0.0"), 24)
    );

    options.ForwardLimit = 2;
});

var app = builder.Build();

app.UseForwardedHeaders();

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
app.MapGet("/health", () => Results.Ok("OK"));
app.MapRazorPages();

app.Run();