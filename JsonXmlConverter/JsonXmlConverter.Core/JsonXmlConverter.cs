using System.Text.RegularExpressions;

namespace JsonXmlConverter.Core
{
    public class JsonXmlConverter : IConverter
    {
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
            List<string> keys = new List<string>();

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

                if (key == "root")
                    continue;

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
                        xml += $"{AddIndentsXml(keys.Count + 1)}<{key}>{element}</{key}>\n";
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
                        xml += $"{AddIndentsXml(keys.Count)}<{key}>\n";
                    }
                    else if (keys.Count > 0)
                    {
                        xml += $"{AddIndentsXml(keys.Count)}</{keys.Last()}>\n";
                        keys.RemoveAt(keys.Count - 1);
                    }

                    continue;
                }

                // Dodaj element XML
                xml += $"{AddIndentsXml(keys.Count + 1)}<{key}>{value}</{key}>\n";
            }

            // Zamknij korzeń XML
            xml += "</root>";

            return xml;
        }

        // TODO: zrobić tablice
        public string ConvertXMLtoJSON(string xml)
        {
            List<string> values = new List<string>();

            // Usuń białe znaki z XMLa
            xml = Regex.Replace(xml, @"(?<![a-zA-Z])\s(?![a-zA-Z])", "");

            // Remove XML declaration and root element
            xml = Regex.Replace(xml, @"<\?xml.*\?>", "").Trim();
            xml = Regex.Replace(xml, @"<root.*?>|</root>", "").Trim();

            // Regular expression to match XML tags and content
            Match[] matches = Regex.Matches(xml, @"<(\w+)(?:\s+[^>]*)*>([^<]*)<\/\1>|<(\w+)(?:\s+[^>]*)*>|<\/(\w+)>|\[((?:(?:""([^""]*)""|(\d+))(?:,|\s)*)*)\]|\{|\}").ToArray();

            int a = 2;
            int lastSearchedIndex;
            string json = $"{{\n{AddIndentsJson(a - 1)}\"root\": {{\n";

            for (int i = 0; i < matches.Length; i++)
            {
                Match match = matches[i];

                if (match.Groups[1].Success) // Opening tag with content
                {
                    string tagName = match.Groups[1].Value;
                    string content = match.Groups[2].Value;

                    if (i - 1 >= 0 && tagName == matches[i - 1].Groups[1].Value)
                    {
                        values.Add(content);

                        if (i + 1 < matches.Length && tagName == matches[i + 1].Groups[1].Value)
                        {
                            continue;
                        }

                        lastSearchedIndex = json.LastIndexOf(':');
                        // wycinamy +1 aby nie uciąć ':' tylko spacje, +2 aby uciąć zbędną spację
                        json = json.Substring(0, lastSearchedIndex + 1) + $" [\n{AddIndentsJson(a + 1)}" + json.Substring(lastSearchedIndex + 2);

                        foreach (string value in values)
                        {
                            json += $"{AddIndentsJson(a + 1)}\"{value}\",\n";
                        }

                        lastSearchedIndex = json.LastIndexOf(',');
                        if (lastSearchedIndex != -1)
                        {
                            json = json.Substring(0, lastSearchedIndex) + json.Substring(lastSearchedIndex + 1);
                        }

                        json += $"{AddIndentsJson(a)}],\n";
                    }
                    else
                    {
                        json += $"{AddIndentsJson(a)}\"{tagName}\": \"{content}\",\n";
                    }
                }
                else if (match.Groups[3].Success) // Opening tag without content
                {
                    string tagName = match.Groups[3].Value;

                    json += $"{AddIndentsJson(a)}\"{tagName}\": {{\n";
                    a++;
                }
                else if (match.Groups[4].Success) // Closing tag
                {
                    // jezeli ostatni element ma ',' a zamykamy obiekt to go usuwamy
                    lastSearchedIndex = json.LastIndexOf(',');
                    if (lastSearchedIndex != -1)
                    {
                        json = json.Substring(0, lastSearchedIndex) + json.Substring(lastSearchedIndex + 1);
                    }

                    json += $"{AddIndentsJson(a - 1)}}},\n";
                    a--;
                }
            }

            lastSearchedIndex = json.LastIndexOf(',');
            if (lastSearchedIndex != -1)
            {
                json = json.Substring(0, lastSearchedIndex) + json.Substring(lastSearchedIndex + 1);
            }

            json += $"{AddIndentsJson(a - 1)}}}\n{AddIndentsJson(a - 2)}}}";

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