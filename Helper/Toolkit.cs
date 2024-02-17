namespace PdfFillerApp.Helper
{

    public static class Toolkit
    {
        public static string TrCharsToEngCharsSub200(this string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                var result = text.ToLower().Replace("'", "").Replace("&", "").Replace("<", "").Replace(">", "").Replace("<br />", "")
                        .Replace("«", "").Replace("»", "").Replace("ç", "c").Replace("ö", "o").Replace("ğ", "g").Replace("ü", "u").Replace("ş", "s")
                        .Replace("ı", "i").Replace("Ç", "C").Replace("Ö", "O").Replace("Ğ", "G").Replace("Ü", "U").Replace("Ş", "S").Replace("İ", "I")
                        .Replace("é", "e").Replace(":", "").Replace(";", "").Replace(",", "").Replace("'", "").Replace("^", "")
                        .Replace("@", "(at)").Replace("#", "").Replace("+", "").Replace("$", "").Replace("%", "").Replace("/", "").Replace("{", "")
                        .Replace("(", "").Replace(")", "").Replace("[", "").Replace("]", "").Replace("=", "").Replace("!", "").Replace("*", "")
                        .Replace("_", "-").Replace("€", "").Replace("~", "").Replace("<", "").Replace(">", "").Replace("|", "").Replace("&", "")
                        .Replace("ô", "o").Replace("ê", "e").Replace("â", "a").Replace("î", "i").Replace("û", "u").Replace("â", "a").Replace("’", "-")
                        .Replace("}", "").Replace("?", "").Replace("<b>", "").Replace("</b>", "").Replace("<strong>", "").Replace("</strong>", "")
                        .Replace(" ", "-").Replace("----", "-").Replace("---", "-").Replace("--", "-");
                return SafeSubstring(result, 0, 200);
            }
            else
            {
                return "";
            }
        }
        private static string SafeSubstring(string text, int startIndex, int length)
        {
            return text.Length <= startIndex ? ""
                : text.Length - startIndex <= length ? text.Substring(startIndex)
                : text.Substring(startIndex, length);
        }

    }

}
