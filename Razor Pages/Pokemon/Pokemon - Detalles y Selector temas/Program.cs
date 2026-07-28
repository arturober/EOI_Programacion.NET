using System.Net.Http.Headers;
using Pokemon.Services;

var builder = WebApplication.CreateBuilder(args);

// Registramos Razor Pages y el servicio que consulta PokeAPI.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient<PokeApiService>(cliente =>
{
    cliente.BaseAddress = new Uri("https://pokeapi.co/api/v2/");
    cliente.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
    cliente.DefaultRequestHeaders.UserAgent.ParseAdd(
        "Pokemon-Educativo/1.0");
    cliente.Timeout = TimeSpan.FromSeconds(20);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();
app.Run();
