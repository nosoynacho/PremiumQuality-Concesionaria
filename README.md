# Premium Quality Concesionaria

**Aplicación web de una concesionaria de vehículos**, desarrollada como proyecto integrador de la Tecnicatura en Programación. Permite consultar modelos, comparar características, gestionar turnos, enviar solicitudes de seguros, escribir reseñas y acceder con una cuenta de usuario.

**Repositorio:** https://github.com/nosoynacho/PremiumQuality-Concesionaria  
**Documentación técnica:** [DOCUMENTACION.md](DOCUMENTACION.md)

## ¿Qué se puede hacer en la página?

| Sección | Funcionalidad |
| --- | --- |
| **Inicio** | Pantalla de presentación de Premium Quality. |
| **Modelos** | Catálogo de seis vehículos, búsqueda por marca o modelo, filtros por marca y año, orden por precio, comparación, detalles y simulación de compra. |
| **Contacto** | Datos de contacto y mapa de ubicación. |
| **Registrarse / Iniciar sesión** | Crear una cuenta, ingresar y cerrar sesión mediante autenticación por cookies. |
| **Recuperar contraseña** | Restablecer la contraseña mediante correo y PIN de recuperación configurado al registrarse. |
| **Mi cuenta** | Consultar nombre, correo, fecha de registro y estado de la cuenta. |
| **Turnos** | Reservar, consultar, editar y eliminar turnos; buscar por nombre, motivo u horario y recorrer páginas de resultados. |
| **Seguros** | Enviar solicitudes y consultar el historial del usuario autenticado. |
| **Reseñas** | Ver reseñas por vehículo, publicar una calificación de 1 a 5 y eliminar reseñas propias desde la interfaz. |
| **Chatbot** | Asistencia sobre la concesionaria con respuestas basadas en datos del sitio e integración opcional con Ollama. |

> Las secciones de turnos, seguros, reseñas y cuenta requieren iniciar sesión. El proceso de compra del catálogo es **una simulación**, no una pasarela de cobros reales.

## Tecnologías utilizadas

- **C# y ASP.NET Core / Blazor Web App (.NET 10)**, con componentes interactivos del servidor (`InteractiveServer`).
- **Razor Components, HTML5, CSS3, CSS Isolation y Bootstrap** para la interfaz.
- **Entity Framework Core 10 + SQLite** para almacenar usuarios, vehículos, turnos, solicitudes y reseñas.
- **JavaScript ES6 + JS Interop** para el modo oscuro persistente.
- **Autenticación por cookies**, validaciones con DataAnnotations y servicios mediante inyección de dependencias.
- **Git, GitHub y GitHub Actions** para versionado e integración continua.
- **Ollama / Qwen3** como opción local de inteligencia artificial para el chatbot.

## Requisitos y ejecución local

Se necesita el **SDK de .NET 10**. Para utilizar las respuestas generativas del chatbot, también se necesita Ollama y el modelo configurado en `appsettings.json`; el resto de la aplicación no requiere Ollama para funcionar.

1. Clonar el repositorio y abrirlo en VS Code o Visual Studio:

   ```powershell
   git clone https://github.com/nosoynacho/PremiumQuality-Concesionaria.git
   cd PremiumQuality-Concesionaria
   ```

2. Restaurar dependencias y compilar:

   ```powershell
   dotnet restore
   dotnet build
   ```

3. Ejecutar:

   ```powershell
   dotnet run
   ```

4. Abrir en el navegador la dirección `http://localhost:...` o `https://localhost:...` que indique la terminal.

**Base de datos:** se utiliza SQLite. Al arrancar, `Program.cs` ejecuta `Database.MigrateAsync()` para aplicar las migraciones; en desarrollo, la configuración de conexión apunta a `Data/concesionaria.db`. El repositorio no necesita incluir una base con datos de usuarios. Para gestionar las migraciones manualmente se puede usar `dotnet ef database update` (con `dotnet-ef` instalado).

**IA local opcional:** iniciar Ollama y descargar el modelo de la configuración:

```powershell
ollama pull qwen3:4b-instruct
```

`LocalAI:BaseUrl` apunta por defecto a `http://localhost:11434/`. El proyecto admite `LocalAI:Enabled = false` para utilizar el chatbot sin generación por Ollama. **No se incluyen modelos de IA ni credenciales en el repositorio.**

## ¿Cómo cumple los criterios de evaluación?

La siguiente tabla relaciona **cada criterio** con la evidencia concreta del proyecto. Se describen implementaciones existentes; las verificaciones visuales y el acceso público dependen del entorno donde se ejecute.

### Unidad 1 — Componentes y fundamentos de Blazor

| Criterio | Dónde se implementa / demuestra |
| --- | --- |
| **CRUD completo** | `Components/Pages/Turnos.razor`, `Components/Services/TurnosState.cs` y `TurnoService.cs`: crear, leer, editar y eliminar reservas. |
| **Al menos 4 componentes reutilizables** | `TurnoFormulario.razor`, `TurnoLista.razor`, `TurnoItem.razor`, `ConfirmacionModal.razor`, además de `AlertError.razor` y `Paginacion.razor`. |
| **Validaciones y errores claros** | `TurnoFormModel.cs`, `RegistroUsuarioModel.cs`, `RecuperarPasswordModel.cs`, `Resena.cs`; uso de `EditForm`, `DataAnnotationsValidator`, `ValidationMessage` y mensajes de error en las páginas. |
| **State Service reactivo** | `TurnosState.cs` expone `OnChange`; `Turnos.razor` se suscribe al evento y se desuscribe mediante `Dispose()`. |
| **Código organizado** | Separación en `Components/Pages`, `Components/Shared`, `Components/Models`, `Components/Services`, `Data` y `Migrations`. |
| **GitHub y README** | Repositorio enlazado arriba y este `README.md`. |

### Unidad 2 — HTML5, CSS, Bootstrap y JavaScript

| Criterio | Dónde se implementa / demuestra |
| --- | --- |
| **HTML5 semántico** | `MainLayout.razor`, `NavMenu.razor` y páginas con `header`, `nav`, `main`, `aside`, `article`, `section` y `footer`. |
| **CSS Isolation en 3 o más componentes** | `MainLayout.razor.css`, `NavMenu.razor.css`, `TurnoFormulario.razor.css`, `TurnoItem.razor.css` y `ConfirmacionModal.razor.css`. |
| **Bootstrap responsive** | `Modelos.razor` usa `row`, `col-12`, `col-sm-6`, `col-lg-4` y `col-xl-3`; estilos responsive en `wwwroot/concesionaria.css`. |
| **JS Interop con ES6 y DisposeAsync** | `ThemeToggle.razor` importa el módulo `wwwroot/dark-mode.js`, invoca sus funciones y libera `IJSObjectReference` mediante `DisposeAsync()`. |
| **Modo oscuro persistente** | `wwwroot/dark-mode.js` guarda y recupera la preferencia `pq_tema` en `localStorage`; estilos en `dark-mode.css`. |

### Unidad 3 — Persistencia de datos

| Criterio | Dónde se implementa / demuestra |
| --- | --- |
| **Base de datos con entidades y relaciones** | `Data/ConcesionariaDbContext.cs`: entidades `Usuario`, `Vehiculo`, `Turno`, `Reclamo` y `Resena`, relacionadas mediante claves foráneas. |
| **Migraciones versionadas** | `Migrations/20261005223702_CreacionInicial.cs`, archivo Designer y snapshot; `Program.cs` aplica `MigrateAsync()`. |
| **CRUD desde la interfaz** | `Turnos.razor` → `TurnosState.cs` → `TurnoService.cs` → EF Core → SQLite. |
| **Búsqueda y paginación contra BD** | `TurnoService.ObtenerPaginado()` aplica `Where`, `EF.Functions.Like`, `Count`, `Skip` y `Take` antes de materializar resultados. |
| **Excepciones mostradas en UI** | `AlertError.razor` y bloques `try/catch` en `Turnos.razor`, `Seguros.razor` y `Resenas.razor`. |

### Unidad 4 — Git, integración continua y publicación

| Criterio | Evidencia concreta |
| --- | --- |
| **Historial y ramas** | Ramas `main`, `develop` y `feature/validaciones-avanzadas`, con commits descriptivos y Pull Requests integrados. |
| **GitHub Actions / CI** | `.github/workflows/ci.yml` restaura, compila y genera una publicación `Release` con .NET 10. [Ver Actions](https://github.com/nosoynacho/PremiumQuality-Concesionaria/actions). |
| **CD (despliegue continuo)** | La generación de artefactos está automatizada en CI; la publicación automática en un servidor requiere configurar el destino de alojamiento. |
| **Aplicación accesible por Internet** | El proyecto está preparado para publicarse en un servidor ASP.NET Core; la dirección pública debe incorporarse aquí cuando esté disponible. |
| **README y documentación** | Este archivo y [DOCUMENTACION.md](DOCUMENTACION.md), que contiene arquitectura, datos, rutas, seguridad, pruebas y despliegue. |

## Evidencias de GitHub

- [Código fuente](https://github.com/nosoynacho/PremiumQuality-Concesionaria)
- [Historial de commits](https://github.com/nosoynacho/PremiumQuality-Concesionaria/commits/main)
- [Ramas](https://github.com/nosoynacho/PremiumQuality-Concesionaria/branches)
- [Pull Requests](https://github.com/nosoynacho/PremiumQuality-Concesionaria/pulls?q=is%3Apr+is%3Aclosed)
- [GitHub Actions](https://github.com/nosoynacho/PremiumQuality-Concesionaria/actions)

## Documentación

Para conocer la arquitectura interna, los modelos de datos, los flujos de usuario, el funcionamiento de cada servicio y las comprobaciones del proyecto, consultar **[DOCUMENTACION.md](DOCUMENTACION.md)**.

---

**Premium Quality Concesionaria — Proyecto integrador desarrollado con Blazor y .NET 10.**
