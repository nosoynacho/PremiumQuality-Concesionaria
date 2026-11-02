const CLAVE_TEMA = "pq_tema";

function aplicarTema(esOscuro) {
    document.body.classList.toggle("dark", esOscuro);
    document.documentElement.classList.toggle("dark", esOscuro);
}
// Esta función aplica el tema oscuro o claro al cuerpo del documento y al elemento raíz (html) según el valor de esOscuro. Si esOscuro es true, se agrega la clase "dark"; si es false, se elimina.
export function aplicarTemaGuardado() {
    const temaGuardado =
        localStorage.getItem(CLAVE_TEMA);

    const esOscuro =
        temaGuardado === "dark";

    aplicarTema(esOscuro);

    return esOscuro;
}

export function cambiarTema() {
    const esOscuro =
        !document.body.classList.contains("dark");

    aplicarTema(esOscuro);

    localStorage.setItem(
        CLAVE_TEMA,
        esOscuro ? "dark" : "light"
    );

    return esOscuro;
}

export function forzarTema(esOscuro) {
    aplicarTema(esOscuro);
}