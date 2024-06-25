namespace MinimalAPIPeliculas.Validaciones
{
    public static class Utilidades
    {
        public static string CampoRequeridoMensaje = "El campo {PropertyName} es requerido";
        public static string MaximunLengthMensaje = "El campo {PropertyName} debe tener menos de {MaxLength} caracteres";
        public static string PrimeraLetraMayusculaMensaje = "El campo {PropertyName} debe comenzar con mayusculas";

        public static string GreaterThenOrEqualToMessage(DateTime fechaMinima)
        {
            return "El campo {PropertyName} debe ser posterior a " + fechaMinima.ToString("yyyy-MM-dd");
        }

        public static bool PrimeraLetraEnMayusculas(string valor)
        {
            if (string.IsNullOrEmpty(valor))
            {
                return true;
            }

            var primeraLetra = valor[0].ToString();

            return primeraLetra == primeraLetra.ToUpper();
        }
    }
}
