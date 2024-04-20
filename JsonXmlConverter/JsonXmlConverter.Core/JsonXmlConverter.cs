using System.Text;
using System.Text.RegularExpressions;

namespace JsonXmlConverter.Core
{
    public class JsonXmlConverter : IConverter
    {
        private List<string> keys = new List<string>();

        public JsonXmlConverter()
        {

        }

        public string ConvertFromFile(string inputFilePath)
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

            return convertedContent;
        }

        // Bez tablicy obiektów
        public string ConvertJSONtoXML(string json)
        {
            // Usuń białe znaki z JSONa
            json = Regex.Replace(json, @"(?<![a-zA-Z])\s(?![a-zA-Z])", "");

            json = json.Replace("}", "\"closeObjJsonXmlConverter\":}");

            // Utwórz korzeń XML
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\n<root>\n";

            ICollection<Match> matches = Regex.Matches(json, "\"(\\w+)\":(\"([^\"]*)\"|(\\d+)|\\[((?:(\"([^\"]*)\"|(\\d+))(?:,|\\s)*)*)\\]|\\{|\\})");

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
                        xml += $"{AddIndents(keys.Count + 1)}<{key}>{element}</{key}>\n";
                    }

                    continue;
                }

                // 3 - string, 4 - int
                string value = match.Groups[3].Success ? match.Groups[3].Value : match.Groups[4].Success ? match.Groups[4].Value : "";

                if (string.IsNullOrEmpty(value))
                {
                    if (key != "closeObjJsonXmlConverter")
                    {
                        keys.Add(key);
                        xml += $"{AddIndents(keys.Count)}<{key}>\n";
                    }
                    else if (keys.Count > 0)
                    {
                        xml += $"{AddIndents(keys.Count)}</{keys.Last()}>\n";
                        keys.RemoveAt(keys.Count - 1);
                    }

                    continue;
                }

                // Dodaj element XML
                xml += $"{AddIndents(keys.Count + 1)}<{key}>{value}</{key}>\n";
            }

            // Zamknij korzeń XML
            xml += "</root>";

            return xml;
        }

        public string ConvertXMLtoJSON(string xml)
        {
            var jsonBuilder = new StringBuilder();
            ConvertXmlNodeToJson(xml.Substring(xml.IndexOf('>') + 1), jsonBuilder); // Ignore the root element

            return jsonBuilder.ToString();
        }

        public bool IsJson(string text)
        {
            text = text.Trim();

            return text.StartsWith("{") && text.EndsWith("}");
        }

        public bool IsXml(string text)
        {
            text = text.Trim();

            if (text.StartsWith("["))
            {
                return text[1] == '<';
            }

            return text.StartsWith("<") && text.EndsWith(">");
        }

        private string AddIndents(int qty)
        {
            // 2 spację dla ładniejszych wcięć
            return new string(' ', qty * 2);
        }

        private static void ConvertXmlNodeToJson(string xml, StringBuilder jsonBuilder)
        {
            int index = 0;
            while (index < xml.Length)
            {
                if (xml[index] == '<')
                {
                    int closingIndex = xml.IndexOf('>', index + 1);
                    if (closingIndex == -1)
                        break;

                    string tagName = xml.Substring(index + 1, closingIndex - index - 1);
                    index = closingIndex + 1;

                    // Check if it's an opening or closing tag
                    if (tagName[0] != '/')
                    {
                        // Opening tag
                        jsonBuilder.Append($"\"{tagName}\": ");
                        if (xml[index] == '<')
                        {
                            jsonBuilder.Append("{");
                            index++;
                            ConvertXmlNodeToJson(xml.Substring(index), jsonBuilder);
                        }
                        else
                        {
                            // Text content
                            int endTagIndex = xml.IndexOf('<', index);
                            jsonBuilder.Append($"\"{xml.Substring(index, endTagIndex - index)}\"");
                            index = endTagIndex;
                        }
                    }
                    else
                    {
                        // Closing tag
                        jsonBuilder.Append("}");
                        return;
                    }
                }
                else
                {
                    index++;
                }
            }
        }
    }
}