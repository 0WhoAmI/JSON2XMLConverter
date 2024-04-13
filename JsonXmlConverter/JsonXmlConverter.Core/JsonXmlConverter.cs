using System.Text;

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
            throw new NotImplementedException();

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