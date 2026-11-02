namespace blazornacho.Models;

public class PagedResult<T>
{
    public IReadOnlyList<T> Items
        { get; init; } =
            Array.Empty<T>();


    public int PaginaActual { get; init; }


    public int TamanioPagina { get; init; }


    public int TotalRegistros { get; init; }


    public int TotalPaginas { get; init; }
}