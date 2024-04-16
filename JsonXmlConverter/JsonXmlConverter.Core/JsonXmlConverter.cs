using System.Text;
using System.Text.RegularExpressions;

namespace JsonXmlConverter
{
    public class JsonXmlConverter : IConverter
    {
        public string BeautyJson(string json)
        {
            return FormatJson(json, indentLevel: 0);
        }

        public string BeautyXml(string xml)
        {
            return FormatXml(xml, indentLevel: 0);
        }

        public void ConvertAndSaveToFile(string inputFilePath, string outputFilePath)
        {
            string fileContent = File.ReadAllText(inputFilePath);

            if (fileContent == null || fileContent.Length <= 1)
            {
                throw new Exception("Błąd pobierania tekstu z pliku");
            }

            string convertedContent;

            //if (IsJson($"{fileContent[0]}{fileContent[^1]}"))
            //    convertedContent = ConvertJSONtoXML(fileContent);
            //else if (IsXml($"{fileContent[0]}{fileContent[^1]}"))
            //    convertedContent = ConvertXMLtoJSON(fileContent);
            //else
            //    throw new Exception("Tekst z pliku nie przypomina ani Jsona, ani Xml");

            if (IsJson(fileContent))
                convertedContent = ConvertJSONtoXML(fileContent);
            else if (IsXml(fileContent))
                convertedContent = ConvertXMLtoJSON(fileContent);
            else
                throw new Exception("Tekst z pliku nie przypomina ani Jsona, ani Xml");

            File.WriteAllText(outputFilePath, convertedContent);
        }

        public string ConvertJSONtoXML(string json)
        {
            // Usuń białe znaki z JSONa
            json = Regex.Replace(json, @"(?<!\\)(?:\\\\)*\s+", "");

            // Utwórz korzeń XML
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\n<root>\n";

            ICollection<Match> matches = Regex.Matches(json, "\"(\\w+)\":(\"([^\"]*)\"|(\\d+)|\\[((?:(\"([^\"]*)\"|(\\d+))(?:,|\\s)*)*)\\])");

            // Użyj wyrażenia regularnego, aby znaleźć wszystkie pary klucz-wartość JSON
            foreach (Match match in matches)
            {
                string key = match.Groups[1].Value;

                // Jeśli wartość jest tablicą, przetwórz ją
                if (match.Groups[5].Success)
                {
                    string[] arrayValues;

                    // Sprawdź, czy przechwycono wartości stringów w tablicy
                    if (match.Groups[7].Success)
                    {
                        arrayValues = match.Groups[7].Captures.Select(m => m.Value).ToArray();
                    }
                    // Jeśli nie przechwycono wartości stringów, sprawdź, czy przechwycono wartości liczbowe w tablicy
                    else if (match.Groups[8].Success)
                    {
                        arrayValues = match.Groups[8].Captures.Select(m => m.Value).ToArray();
                    }
                    else
                    {
                        arrayValues = [];
                    }

                    foreach (string element in arrayValues)
                    {
                        // Dodaj element XML
                        xml += $"<{key}>{element}</{key}>\n";
                    }

                    continue;
                }

                // 3 - string, 4 - int
                string value = match.Groups[3].Success ? match.Groups[3].Value : match.Groups[4].Success ? match.Groups[4].Value : "";

                // Dodaj element XML
                xml += $"<{key}>{value}</{key}>\n";
            }

            // Zamknij korzeń XML
            xml += "</root>";

            return xml;
        }

        public string ConvertXMLtoJSON(string xml)
        {
            throw new NotImplementedException();
        }

        public bool IsJson(string text)
        {
            text = text.Trim();

            return text.StartsWith("{") && text.EndsWith("}");
        }

        public bool IsXml(string text)
        {
            text = text.Trim();

            return text.StartsWith("<") && text.EndsWith(">");
        }

        private string FormatJson(string json, int indentLevel)
        {
            var indentString = new string(' ', indentLevel * 2);
            var result = new StringBuilder();
            var isInString = false;
            var isNewLine = false;
            var isEscaped = false;
            char? lastChar = null;

            foreach (var ch in json)
            {
                if (!isEscaped && ch == '"')
                {
                    isInString = !isInString;
                }

                if (!isInString)
                {
                    if (ch == '}' || ch == ']')
                    {
                        indentLevel--;
                        result.AppendLine();
                        result.Append(new string(' ', indentLevel * 2));
                    }

                    if (lastChar == '{' || lastChar == '[' || ch == ',' || (lastChar == ':' && ch != '{' && ch != '['))
                    {
                        result.AppendLine();
                        result.Append(new string(' ', indentLevel * 2));
                    }

                    if (ch == '{' || ch == '[')
                    {
                        indentLevel++;
                    }
                }

                result.Append(ch);

                if (ch == '\\' && !isEscaped)
                {
                    isEscaped = true;
                }
                else
                {
                    isEscaped = false;
                }

                if (!isInString && (ch == '{' || ch == '[' || ch == '}' || ch == ']'))
                {
                    isNewLine = true;
                }
                else
                {
                    isNewLine = false;
                }

                lastChar = ch;
            }

            return result.ToString();
        }

        private string FormatXml(string xml, int indentLevel)
        {
            var indentString = new string(' ', indentLevel * 2);
            var result = new StringBuilder();
            var isInString = false;
            var isNewLine = false;
            var isEscaped = false;
            var tagStart = false;
            char? lastChar = null;

            foreach (var ch in xml)
            {
                if (!isInString)
                {
                    if (ch == '<')
                    {
                        if (tagStart)
                        {
                            result.AppendLine();
                            result.Append(new string(' ', indentLevel * 2));
                        }

                        tagStart = true;
                    }
                    else if (ch == '>')
                    {
                        tagStart = false;
                    }

                    if (lastChar == '>' && ch != '<' && !Char.IsWhiteSpace(ch))
                    {
                        result.AppendLine();
                        result.Append(new string(' ', indentLevel * 2));
                    }
                }

                result.Append(ch);

                if (ch == '\\' && !isEscaped)
                {
                    isEscaped = true;
                }
                else
                {
                    isEscaped = false;
                }

                if (ch == '"' && !isEscaped)
                {
                    isInString = !isInString;
                }

                lastChar = ch;
            }

            return result.ToString();
        }
    }
}