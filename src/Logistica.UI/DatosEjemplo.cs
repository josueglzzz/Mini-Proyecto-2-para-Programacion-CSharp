namespace Logistica.UI
{
    /// <summary>
    /// Lote de demostración con casos válidos e inválidos (los del enunciado
    /// del Reto 1 más casos de sobrepeso, seguro, zona inexistente y duplicados).
    /// </summary>
    public static class DatosEjemplo
    {
        public const string LoteDemostracion =
            " PACK-001 ; Juan Perez ; Zona A ; 12.5 ; 150.0 \r\n" +
            "PACK-002; Maria Lopez; Zona C; -5.0; 200.0\r\n" +
            "PACK-003; Pedro Gomez; Zona B; ABC; 100.0\r\n" +
            "PACK-004; Ana Ruiz; Zona C; 25; 1500.00\r\n" +
            "PACK-005; Luis Diaz; Zona B; 8.75; 320.50\r\n" +
            "COD-101;Cliente Juan Perez;Zona A;15.5;200.0\r\n" +
            "PACK-006; Carla Mora; Zona A; 22; 1200\r\n" +
            "PACK-007; Jorge Rios; Zona D; 4; 90\r\n" +
            "\r\n" +
            "PACK-008; Sofia Vargas; Zona B; 3.2; 75\r\n" +
            "PACK-009; Raul Mendez; Zona C; 18; $2,500.00\r\n" +
            "PACK-001; Duplicado Prueba; Zona A; 1; 10\r\n";
    }
}
