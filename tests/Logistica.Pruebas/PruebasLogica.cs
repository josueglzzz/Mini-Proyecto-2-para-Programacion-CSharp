using System;
using Logistica.Dominio;
using Logistica.Logica;

namespace Logistica.Pruebas
{
    /// <summary>Pruebas de los tres retos de lógica (Parte 1).</summary>
    public static class PruebasLogica
    {
        public static void Ejecutar(Verificador v)
        {
            ProbarReto1Parser(v);
            ProbarReto2Arreglos(v);
            ProbarReto3Calculadora(v);
            ProbarProcesadorCompleto(v);
            ProbarEncapsulamiento(v);
        }

        private static ConfiguracionTarifas CrearConfiguracion()
        {
            return new ConfiguracionTarifas(25.0m, 0.16m, 50.0m);
        }

        private static void ProbarReto1Parser(Verificador v)
        {
            Console.WriteLine(" Reto 1 - Sanitización y parseo");
            string lote =
                " PACK-001 ; Juan Perez ; Zona A ; 12.5 ; 150.0 \r\n" +
                "PACK-002; Maria Lopez; Zona C; -5.0; 200.0\r\n" +
                "PACK-003; Pedro Gomez; Zona B; ABC; 100.0\r\n" +
                "\r\n" +
                "pack-004;Ana   Ruiz;zona b;3;$1,500.00\r\n" +
                "PACK-005;Luis Diaz;Zona D;3;10\r\n" +
                "PACK-006;Solo tres campos;Zona A\r\n" +
                "PACK-001;Duplicado;Zona A;1;1\r\n" +
                "PACK-007;Carla Mora;Zona A;1;-10\r\n";

            ParserLotes parser = new ParserLotes();
            ResultadoParseo resultado = parser.Analizar(lote);

            v.Igual(9, resultado.TotalLineas, "Cuenta 9 líneas (ignora el salto final)");
            v.Igual(2, resultado.PaquetesValidos.Cantidad, "Sólo PACK-001 y PACK-004 son válidos");
            v.Igual(7, resultado.Errores.Length, "Guarda 7 errores en el arreglo paralelo");

            Paquete primero = resultado.PaquetesValidos.ObtenerEn(0);
            v.Igual("PACK-001", primero.Codigo, "Limpia espacios del código");
            v.Igual("Juan Perez", primero.Cliente, "Limpia espacios del cliente");
            v.Igual(12.5m, primero.PesoKg, "Convierte el peso 12.5");

            Paquete cuarto = resultado.PaquetesValidos.ObtenerEn(1);
            v.Igual("PACK-004", cuarto.Codigo, "Convierte el código a mayúsculas");
            v.Igual("Ana Ruiz", cuarto.Cliente, "Elimina espacios dobles internos");
            v.Verdadero(cuarto.Zona == ZonaEnvio.B, "Reconoce 'zona b' como Zona B");
            v.Igual(1500m, cuarto.ValorDeclarado, "Acepta '$1,500.00'");

            v.Verdadero(resultado.Errores[0].Tipo == TipoErrorLinea.PesoNoPositivo, "Línea 2: peso negativo");
            v.Verdadero(resultado.Errores[1].Tipo == TipoErrorLinea.PesoNoNumerico, "Línea 3: peso 'ABC' no numérico");
            v.Verdadero(resultado.Errores[2].Tipo == TipoErrorLinea.LineaVacia, "Línea 4: línea vacía");
            v.Verdadero(resultado.Errores[3].Tipo == TipoErrorLinea.ZonaInvalida, "Línea 6: zona D inexistente");
            v.Verdadero(resultado.Errores[4].Tipo == TipoErrorLinea.CamposIncompletos, "Línea 7: campos incompletos");
            v.Verdadero(resultado.Errores[5].Tipo == TipoErrorLinea.CodigoDuplicado, "Línea 8: código duplicado");
            v.Verdadero(resultado.Errores[6].Tipo == TipoErrorLinea.ValorNegativo, "Línea 9: valor negativo");
            v.Igual(3, resultado.Errores[1].NumeroLinea, "Conserva el número de línea original");

            v.Igual("Zona A", resultado.MatrizCampos[0, ParserLotes.CampoZona], "La matriz string[,] guarda campos limpios");

            ResultadoParseo vacio = parser.Analizar("   ");
            v.Igual(0, vacio.TotalLineas, "Texto vacío no genera líneas");

            decimal numero;
            v.Verdadero(!ParserLotes.IntentarConvertirNumero("12,5", out numero), "Rechaza coma decimal ambigua '12,5'");
            v.Verdadero(!ParserLotes.IntentarConvertirNumero("99999999999999999999999999999999", out numero), "Captura desbordamiento numérico");
        }

        private static void ProbarReto2Arreglos(Verificador v)
        {
            Console.WriteLine(" Reto 2 - Redimensionamiento, ordenamiento e inversión");
            ArregloPaquetes arreglo = new ArregloPaquetes();
            v.Igual(5, arreglo.Capacidad, "Capacidad inicial de 5");

            decimal[] pesos = { 3m, 30m, 1m, 12m, 8m, 25m, 2m };
            CalculadoraTarifas calculadora = new CalculadoraTarifas(CrearConfiguracion());
            for (int i = 0; i < pesos.Length; i++)
            {
                Paquete paquete = new Paquete("PACK-" + (i + 1).ToString("000"), "Cliente " + i, ZonaEnvio.A, pesos[i], 100m);
                paquete.Desglose = calculadora.Calcular(paquete);
                arreglo.Agregar(paquete);
            }

            v.Igual(7, arreglo.Cantidad, "Almacena 7 paquetes");
            v.Igual(10, arreglo.Capacidad, "Se duplicó la capacidad a 10");
            v.Igual(1, arreglo.Redimensionamientos, "Un redimensionamiento");
            v.Igual("PACK-001", arreglo.ObtenerEn(0).Codigo, "Copia elemento por elemento conserva el orden");

            arreglo.OrdenarPorTotalDescendente();
            bool ordenado = true;
            for (int i = 0; i < arreglo.Cantidad - 1; i++)
            {
                if (arreglo.ObtenerEn(i).TotalAPagar < arreglo.ObtenerEn(i + 1).TotalAPagar)
                {
                    ordenado = false;
                }
            }
            v.Verdadero(ordenado, "Burbuja ordena de mayor a menor total");
            v.Igual("PACK-002", arreglo.ObtenerEn(0).Codigo, "El más caro (30 kg) queda primero");

            arreglo.Invertir();
            v.Igual("PACK-003", arreglo.ObtenerEn(0).Codigo, "Invertir deja primero al más barato (1 kg)");
            v.Igual("PACK-002", arreglo.ObtenerEn(arreglo.Cantidad - 1).Codigo, "Invertir deja al más caro al final");

            v.Igual(7, arreglo.ConvertirAArreglo().Length, "La copia no incluye posiciones vacías");
            v.Verdadero(arreglo.ContieneCodigo("pack-004"), "Búsqueda lineal ignora mayúsculas");
            v.LanzaExcepcion<IndexOutOfRangeException>(() => arreglo.ObtenerEn(7), "ObtenerEn fuera de rango lanza excepción");

            ArregloPaquetes grande = new ArregloPaquetes();
            for (int i = 0; i < 21; i++)
            {
                grande.Agregar(new Paquete("PK-" + i, "Cliente", ZonaEnvio.B, 1m, 1m));
            }
            v.Igual(40, grande.Capacidad, "21 elementos: 5→10→20→40");
            v.Igual(3, grande.Redimensionamientos, "Tres redimensionamientos");
        }

        private static void ProbarReto3Calculadora(Verificador v)
        {
            Console.WriteLine(" Reto 3 - Tarifas escalonadas");
            CalculadoraTarifas calculadora = new CalculadoraTarifas(CrearConfiguracion());

            DesgloseCosto a = calculadora.Calcular(new Paquete("PACK-001", "Juan Perez", ZonaEnvio.A, 12.5m, 150m));
            v.Igual(312.50m, a.SubtotalPeso, "Zona A: subtotal 12.5 kg x $25");
            v.Igual(15.63m, a.RecargoZona, "Zona A: recargo 5%");
            v.Igual(0m, a.PenalizacionSobrepeso, "Sin sobrepeso");
            v.Igual(0m, a.Seguro, "Sin seguro (valor <= 1000)");
            v.Igual(378.13m, a.BaseImponible, "Base imponible incluye costo fijo $50");
            v.Igual(60.50m, a.Iva, "IVA 16%");
            v.Igual(438.63m, a.Total, "Total zona A");

            DesgloseCosto c = calculadora.Calcular(new Paquete("PACK-010", "Rosa Vega", ZonaEnvio.C, 25m, 1500m));
            v.Igual(112.50m, c.RecargoZona, "Zona C: recargo 18%");
            v.Igual(5m, c.KilosExcedentes, "5 kg excedentes sobre 20 kg");
            v.Igual(31.25m, c.PenalizacionSobrepeso, "Penalización 5 kg x $25 x 25%");
            v.Igual(30.00m, c.Seguro, "Seguro 2% de $1,500");
            v.Igual(848.75m, c.BaseImponible, "Base imponible zona C");
            v.Igual(984.55m, c.Total, "Total zona C con sobrepeso y seguro");

            DesgloseCosto b = calculadora.Calcular(new Paquete("PACK-020", "Eva Sol", ZonaEnvio.B, 20m, 1000m));
            v.Igual(50.00m, b.RecargoZona, "Zona B: recargo 10%");
            v.Igual(0m, b.PenalizacionSobrepeso, "Exactamente 20 kg no paga penalización");
            v.Igual(0m, b.Seguro, "Exactamente $1,000 no paga seguro");
            v.Igual(696.00m, b.Total, "Total zona B en los límites");

            v.LanzaExcepcion<ArgumentNullException>(() => new CalculadoraTarifas(null), "Calculadora exige configuración");
        }

        private static void ProbarProcesadorCompleto(Verificador v)
        {
            Console.WriteLine(" Procesador de lotes");
            ProcesadorLotes procesador = new ProcesadorLotes(new CalculadoraTarifas(CrearConfiguracion()));
            string lote =
                "PACK-001;Juan Perez;Zona A;12.5;150.0\n" +
                "PACK-002;Maria Lopez;Zona C;-5.0;200.0\n" +
                "PACK-010;Rosa Vega;Zona C;25;1500\n" +
                "PACK-020;Eva Sol;Zona B;20;1000\n" +
                "PACK-030;Peso Enorme;Zona A;9999999999999999999999999999;10\n";

            ResultadoLote resultado = procesador.Procesar(lote);
            v.Igual(3, resultado.CantidadValidos, "3 paquetes calculados");
            v.Igual(2, resultado.CantidadErrores, "2 errores (peso negativo y error financiero)");
            v.Verdadero(resultado.Errores[1].Tipo == TipoErrorLinea.ErrorFinanciero, "El desbordamiento se reporta como error financiero");
            v.Igual("PACK-010", resultado.Paquetes[0].Codigo, "El lote queda ordenado de mayor a menor");
            v.Igual(438.63m + 984.55m + 696.00m, resultado.SumaTotal, "Total del lote");
            v.Igual(60.50m + 135.80m + 96.00m, resultado.SumaIva, "IVA del lote");

            Paquete[] invertidos = procesador.InvertirOrden(resultado.Paquetes);
            v.Igual("PACK-001", invertidos[0].Codigo, "InvertirOrden pone primero al más barato");
        }

        private static void ProbarEncapsulamiento(Verificador v)
        {
            Console.WriteLine(" Encapsulamiento");
            v.LanzaExcepcion<ArgumentOutOfRangeException>(() => new Paquete("PACK-1", "X", ZonaEnvio.A, -1m, 1m), "Paquete rechaza peso negativo");
            v.LanzaExcepcion<ArgumentException>(() => new Paquete(" ", "X", ZonaEnvio.A, 1m, 1m), "Paquete rechaza código vacío");
            v.LanzaExcepcion<ArgumentOutOfRangeException>(() => new ConfiguracionTarifas(25m, 1.5m, 50m), "Configuración rechaza IVA > 100%");

            Paquete paquete = new Paquete("PACK-9", "Cliente", ZonaEnvio.A, 1m, 1m);
            paquete.RegistrarRecepcion(DateTime.Now);
            v.Verdadero(paquete.Estado == EstadoEnvio.Entregado, "Registrar recepción cambia el estado");
            v.LanzaExcepcion<InvalidOperationException>(() => paquete.RegistrarRecepcion(DateTime.Now), "No se puede recibir dos veces");
        }
    }
}
