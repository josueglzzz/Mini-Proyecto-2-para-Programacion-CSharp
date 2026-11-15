namespace Logistica.Dominio
{
    /// <summary>
    /// Clasificación de los motivos por los que una línea del lote se descarta.
    /// La capa de presentación traduce cada valor al idioma activo.
    /// </summary>
    public enum TipoErrorLinea
    {
        LineaVacia,
        CamposIncompletos,
        CodigoInvalido,
        CodigoDuplicado,
        ClienteInvalido,
        ZonaInvalida,
        PesoNoNumerico,
        PesoNoPositivo,
        ValorNoNumerico,
        ValorNegativo,
        ErrorFinanciero
    }
}
