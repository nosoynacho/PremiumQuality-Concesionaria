using blazornacho.Components;

using blazornacho.Data;

using blazornacho.Services;

using Microsoft.AspNetCore
    .Authentication.Cookies;

using Microsoft.EntityFrameworkCore;


var builder =
    WebApplication.CreateBuilder(args);


// ============================================================
// BLAZOR
// ============================================================

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();


// ============================================================
// AUTENTICACIÓN
// ============================================================

builder.Services
    .AddAuthentication(
        CookieAuthenticationDefaults
            .AuthenticationScheme
    )
    .AddCookie(options =>
    {
        options.Cookie.Name =
            "PremiumQuality.Auth";


        options.Cookie.HttpOnly =
            true;


        options.Cookie.IsEssential =
            true;


        options.Cookie.SameSite =
            SameSiteMode.Lax;


        options.Cookie.SecurePolicy =
            CookieSecurePolicy
                .SameAsRequest;


        options.LoginPath =
            "/iniciar-sesion";


        options.AccessDeniedPath =
            "/iniciar-sesion";


        options.ExpireTimeSpan =
            TimeSpan.FromDays(365);


        options.SlidingExpiration =
            true;
    });


builder.Services
    .AddAuthorization();


builder.Services
    .AddCascadingAuthenticationState();


builder.Services
    .AddAntiforgery();


builder.Services
    .AddHttpContextAccessor();


// ============================================================
// ENTITY FRAMEWORK CORE
// ============================================================


string connectionString =
    builder.Configuration
        .GetConnectionString("Default")
    ?? "Data Source=Data/concesionaria.db";

// En Azure guardamos los datos fuera
// de la carpeta que se vuelve a publicar.
if (builder.Environment.IsProduction())
{
    string home =
        Environment.GetEnvironmentVariable("HOME")
        ?? throw new InvalidOperationException(
            "No se encontró el directorio HOME del servidor."
        );

    string carpetaDatos =
        Path.Combine(home, "premium-quality-data");

    Directory.CreateDirectory(carpetaDatos);

    string rutaBase =
        Path.Combine(carpetaDatos, "concesionaria.db");

    connectionString = $"Data Source={rutaBase}";
}


builder.Services
    .AddDbContextFactory<
        ConcesionariaDbContext>(
            options =>
            {
                options.UseSqlite(
                    connectionString
                );
            }
        );


// ============================================================
// SERVICIOS
// ============================================================

builder.Services
    .AddScoped<VehiculoService>();


builder.Services
    .AddScoped<ResenaService>();


builder.Services
    .AddScoped<ChatbotService>();


builder.Services
    .AddScoped<TurnoService>();


builder.Services
    .AddScoped<ReclamoService>();


builder.Services
    .AddScoped<UsuarioService>();


builder.Services
    .AddScoped<TurnosState>();


builder.Services
    .AddScoped<SiteContextService>();


// ============================================================
// OLLAMA
// ============================================================

builder.Services
    .AddHttpClient<LocalAiService>(
        (sp, client) =>
        {
            IConfiguration configuration =
                sp.GetRequiredService<
                    IConfiguration>();


            string baseUrl =
                configuration[
                    "LocalAI:BaseUrl"
                ]
                ??
                "http://localhost:11434/";


            client.BaseAddress =
                new Uri(baseUrl);


            client.Timeout =
                System.Threading
                    .Timeout
                    .InfiniteTimeSpan;
        }
    );


var app =
    builder.Build();


// ============================================================
// CREAR CARPETA DATA
// ============================================================


if (app.Environment.IsDevelopment())
{
    Directory.CreateDirectory(
        Path.Combine(
            app.Environment.ContentRootPath,
            "Data"
        )
    );
}



// ============================================================
// APLICAR MIGRACIONES AUTOMÁTICAMENTE
// ============================================================

await using (
    AsyncServiceScope scope =
        app.Services
            .CreateAsyncScope()
)
{
    IDbContextFactory<
        ConcesionariaDbContext> factory =
            scope.ServiceProvider
                .GetRequiredService<
                    IDbContextFactory<
                        ConcesionariaDbContext>>();


    await using
        ConcesionariaDbContext db =
            await factory
                .CreateDbContextAsync();


    await db.Database
        .MigrateAsync();
}


// ============================================================
// PRECARGAR OLLAMA
// ============================================================



if (app.Configuration.GetValue<bool>(
        "LocalAI:Enabled", true))
{
    using IServiceScope scope =
        app.Services.CreateScope();

    try
    {
        LocalAiService localAi =
            scope.ServiceProvider
                .GetRequiredService<LocalAiService>();

        Console.WriteLine(
            "Cargando modelo de IA local..."
        );

        await localAi.PrecalentarAsync();

        Console.WriteLine("IA local lista.");
    }
    catch (Exception ex)
    {
        Console.WriteLine(
            $"No se pudo precargar Ollama: {ex.Message}"
        );
    }
}
else
{
    Console.WriteLine(
        "IA generativa desactivada en este entorno."
    );
}



// ============================================================
// PIPELINE HTTP
// ============================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(
        "/Error",
        createScopeForErrors: true
    );


    app.UseHsts();
}


app.UseStatusCodePagesWithReExecute(
    "/not-found",
    createScopeForStatusCodePages: true
);


app.UseHttpsRedirection();


app.UseAuthentication();


app.UseAuthorization();


app.UseAntiforgery();


// ============================================================
// ARCHIVOS ESTÁTICOS
// ============================================================

app.MapStaticAssets();


// ============================================================
// AUTENTICACIÓN
// ============================================================

app.MapAuthEndpoints();


// ============================================================
// BLAZOR
// ============================================================

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.Run();