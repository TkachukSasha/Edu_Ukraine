using Web.App.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient<SPARQLQueryService>(client =>
{
    string? sparqlConnectionString = builder.Configuration.GetConnectionString("Sparql") ?? string.Empty;

    if (!Uri.IsWellFormedUriString(sparqlConnectionString, UriKind.Absolute))
    {
        throw new InvalidOperationException("The provided SPARQL endpoint URI is not valid.");
    }

    client.BaseAddress = new Uri(sparqlConnectionString);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("User-Agent", "Edu.Ukraine");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=University}/{action=Index}/{id?}");

// Запускаємо додаток
app.Run();
