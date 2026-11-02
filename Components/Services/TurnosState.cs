using blazornacho.Models;

namespace blazornacho.Services;

public class TurnosState
{
    private readonly TurnoService
        _turnoService;


    private List<Turno> _turnos =
        new();


    private int _paginaActual = 1;

    private int _tamanioPagina = 5;

    private string _busqueda = "";

    private long _usuarioId;


    // ======================================================
    // ESTADO PÚBLICO
    // ======================================================

    public IReadOnlyList<Turno> Turnos =>
        _turnos;


    public int PaginaActual
        { get; private set; } = 1;


    public int TotalPaginas
        { get; private set; } = 1;


    public int TotalRegistros
        { get; private set; }


    public event Action? OnChange;


    public TurnosState(
        TurnoService turnoService)
    {
        _turnoService =
            turnoService;
    }


    // ======================================================
    // CARGAR
    // ======================================================

    public void Cargar(
        int pagina,
        int tamanioPagina,
        string? busqueda,
        long usuarioId)
    {
        _paginaActual =
            pagina;

        _tamanioPagina =
            tamanioPagina;

        _busqueda =
            busqueda ?? "";

        _usuarioId =
            usuarioId;


        Recargar();
    }


    // ======================================================
    // CREAR
    // ======================================================

    public void Agregar(
        Turno turno)
    {
        _turnoService
            .Agregar(turno);

        Recargar();
    }


    // ======================================================
    // ACTUALIZAR
    // ======================================================

    public void Actualizar(
        Turno turno)
    {
        _turnoService
            .Actualizar(turno);

        Recargar();
    }


    // ======================================================
    // ELIMINAR
    // ======================================================

    public void Eliminar(
        long id)
    {
        _turnoService.Eliminar(
            id,
            _usuarioId
        );

        Recargar();
    }


    // ======================================================
    // VALIDAR DUPLICADO
    // ======================================================

    public bool ExisteTurno(
        DateTime fecha,
        string hora,
        long? excluirId = null)
    {
        return _turnoService
            .ExisteTurno(
                fecha,
                hora,
                excluirId
            );
    }


    private void Recargar()
    {
        PagedResult<Turno> resultado =
            _turnoService
                .ObtenerPaginado(
                    _paginaActual,
                    _tamanioPagina,
                    _busqueda,
                    _usuarioId
                );


        _turnos =
            resultado.Items.ToList();


        PaginaActual =
            resultado.PaginaActual;


        TotalPaginas =
            resultado.TotalPaginas;


        TotalRegistros =
            resultado.TotalRegistros;


        NotificarCambio();
    }


    private void NotificarCambio()
    {
        OnChange?.Invoke();
    }
}