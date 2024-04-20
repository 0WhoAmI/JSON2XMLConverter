using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

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

        // TODO: zrobić tablice
        public string ConvertXMLtoJSON(string xml)
        {
            // Usuń białe znaki z XMLa
            xml = Regex.Replace(xml, @"(?<![a-zA-Z])\s(?![a-zA-Z])", "");

            //xml = xml.Replace("}", "\"closeObjJsonXmlConverter\":}");

            // Remove XML declaration and root element
            xml = Regex.Replace(xml, @"<\?xml.*\?>", "").Trim();
            xml = Regex.Replace(xml, @"<root.*?>|</root>", "").Trim();

            // Regular expression to match XML tags and content
            ICollection<Match> matches = Regex.Matches(xml, @"<(\w+)(?:\s+[^>]*)*>([^<]*)<\/\1>|<(\w+)(?:\s+[^>]*)*>|<\/(\w+)>|\[((?:(?:""([^""]*)""|(\d+))(?:,|\s)*)*)\]|\{|\}");

            int a = 2;
            string json = $"{{\n{AddIndentsJson(a - 1)}\"root\": {{\n";

            foreach (Match match in matches)
            {
                if (match.Groups[1].Success) // Opening tag with content
                {
                    string tagName = match.Groups[1].Value;
                    string content = match.Groups[2].Value;

                    //json += $"{AddIndentsJson(a)}Opening Tag: <{tagName}>, Content: {content}\n";

                    json += $"{AddIndentsJson(a)}\"{tagName}\": \"{content}\",\n";
                }
                else if (match.Groups[3].Success) // Opening tag without content
                {
                    string tagName = match.Groups[3].Value;

                    //json += $"{AddIndentsJson(a)}Opening Tag: <{tagName}>\n";

                    json += $"{AddIndentsJson(a)}\"{tagName}\": {{\n";
                    a++;
                }
                else if (match.Groups[4].Success) // Closing tag
                {
                    string tagName = match.Groups[4].Value;

                    //json += $"{AddIndentsJson(a)}Closing Tag: </{tagName}>\n";

                    // jezeli ostatni element ma ',' a zamykamy obiekt to go usuwamy
                    int lastCommaIndex = json.LastIndexOf(',');
                    if (lastCommaIndex != -1)
                    {
                        json = json.Substring(0, lastCommaIndex) + json.Substring(lastCommaIndex + 1);
                    }

                    json += $"{AddIndentsJson(a-1)}}},\n";
                    a--;
                }
                else if (match.Groups[5].Success) // Array
                {
                    string arrayContent = match.Groups[5].Value;

                    //json += $"{AddIndentsJson(a)}Array: [{arrayContent}]\n";
                }
                else if (match.Value == "{") // Opening curly brace
                {
                    //json += $"{AddIndentsJson(a)}Opening Curly Brace: {{\n";
                }
                else if (match.Value == "}") // Closing curly brace
                {
                    //json += $"{AddIndentsJson(a)}Closing Curly Brace: }}\n";
                }
            }

            return json;
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

        private string AddIndentsXml(int newLineIndentQty)
        {
            // 2 spację dla ładniejszych wcięć
            return new string(' ', newLineIndentQty * 2);
        }

        private string AddIndentsJson(int newLineIndentQty)
        {
            // 3 spację dla ładniejszych wcięć
            return new string(' ', newLineIndentQty * 3);
        }
    }
}