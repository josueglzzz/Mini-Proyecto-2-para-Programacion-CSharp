namespace Logistica.Dominio
{
    /// <summary>
    /// Estado de la transacción de un paquete: se registra como envío
    /// (EnTransito) y pasa a Entregado cuando se registra su recepción.
    /// </summary>
    public enum EstadoEnvio
    {
        EnTransito,
        Entregado
    }
}
