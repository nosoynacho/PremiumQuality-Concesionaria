using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace blazornacho.Components.Pages;

public partial class Home : ComponentBase
{
    private bool isDarkMode = false;

    [Inject]
    private IJSRuntime? JSRuntime { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && JSRuntime != null)
        {
            try
            {
                // Obtener el estado guardado del localStorage
                var savedMode = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "darkMode");
                if (savedMode == "true")
                {
                    isDarkMode = true;
                    await JSRuntime.InvokeVoidAsync("toggleDarkMode", true);
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al inicializar tema: {ex.Message}");
            }
        }
    }

    public async Task ToggleDarkMode()
    {
        isDarkMode = !isDarkMode;
        
        if (JSRuntime != null)
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("toggleDarkMode", isDarkMode);
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cambiar tema: {ex.Message}");
            }
        }
    }

    public bool IsDarkMode => isDarkMode;
}
