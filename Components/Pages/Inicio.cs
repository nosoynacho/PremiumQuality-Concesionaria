using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace blazornacho.Components.Pages;

public partial class Inicio : ComponentBase
{
    private bool isDarkMode = false;

    [Inject]
    private IJSRuntime? JSRuntime { get; set; }

    public async Task ToggleDarkMode()
    {
        isDarkMode = !isDarkMode;
        
        if (JSRuntime != null)
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("toggleDarkMode", isDarkMode);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    public bool IsDarkMode => isDarkMode;
}



