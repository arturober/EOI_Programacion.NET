using System.Globalization;
using System.Text;

public static class TextoUtil
{
    public const string LetrasRosco = "ABCDEFGHIJKLMNÑOPQRSTUVWXYZ";

    public static string NormalizarParaComparar(string texto)
    {
        string textoCompuesto = texto.Normalize(NormalizationForm.FormC);
        string sinEspaciosExtra = string.Join(
            " ", textoCompuesto.Trim().Split(
                ' ', StringSplitOptions.RemoveEmptyEntries));
        StringBuilder resultado = new StringBuilder();

        foreach (char caracter in sinEspaciosExtra)
        {
            // La Ñ es una letra distinta de la N y debe conservarse.
            if (char.ToLowerInvariant(caracter) == 'ñ')
            {
                resultado.Append('ñ');
                continue;
            }

            string caracterDescompuesto = caracter.ToString().Normalize(
                NormalizationForm.FormD);

            foreach (char parte in caracterDescompuesto)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(parte) !=
                    UnicodeCategory.NonSpacingMark)
                {
                    resultado.Append(char.ToLowerInvariant(parte));
                }
            }
        }

        return resultado.ToString();
    }

    public static char NormalizarCaracter(char caracter)
    {
        return char.ToLowerInvariant(caracter) switch
        {
            'á' => 'a', 'é' => 'e', 'í' => 'i',
            'ó' => 'o', 'ú' => 'u', 'ü' => 'u',
            _ => char.ToLowerInvariant(caracter)
        };
    }

    public static bool SonIguales(string texto1, string texto2)
    {
        return NormalizarParaComparar(texto1) == NormalizarParaComparar(texto2);
    }

    public static bool EsLetraDelRosco(char letra)
    {
        return LetrasRosco.Contains(char.ToUpperInvariant(letra));
    }

    public static bool EsRespuestaValida(string respuesta, char letra)
    {
        bool caracteresValidos = respuesta.All(c => char.IsLetter(c) || c == ' ' || c == '-');
        return respuesta.Any(char.IsLetter) && caracteresValidos &&
               NormalizarParaComparar(respuesta).Contains(NormalizarCaracter(letra));
    }
}
