using System.Text;
using System.Xml;

namespace JsonXmlConverter
{
    public class JsonXmlConverter : IConverter
    {
        public string ConvertJSONtoXML(string json)
        {
            StringBuilder xmlBuilder = new StringBuilder();
            json = json.Trim();

            if (json.StartsWith("{") && json.EndsWith("}"))
            {
                Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();

                // Usuwamy nawiasy klamrowe
                json = json.Substring(1, json.Length - 2);

                // Dzielimy na pary klucz-wartość
                string[] pairs = json.Split(',');

                // Konwersja na XML
                xmlBuilder.Append("<root>");
                foreach (string pair in pairs)
                {
                    string[] keyValue = pair.Split(new char[] { ':' }, 2); // Poprawiamy podział tylko na pierwszym dwukropku
                    if (keyValue.Length == 2)
                    {
                        string key = keyValue[0].Trim().Trim('"');
                        string value = keyValue[1].Trim();
                        if (value.StartsWith("\"") && value.EndsWith("\""))
                        {
                            value = value.Substring(1, value.Length - 2);
                            xmlBuilder.Append($"<{key}>{value}</{key}>");
                        }
                        else if (value.StartsWith("[") && value.EndsWith("]"))
                        {
                            xmlBuilder.Append($"<{key}>");
                            string[] arrayValues = value.Substring(1, value.Length - 2).Split(',');
                            foreach (string arrayValue in arrayValues)
                            {
                                xmlBuilder.Append($"<{key}Item>{arrayValue.Trim().Trim('"')}</{key}Item>");
                            }
                            xmlBuilder.Append($"</{key}>");
                        }
                        else
                        {
                            xmlBuilder.Append($"<{key}>{value}</{key}>");
                        }
                    }
                }
                xmlBuilder.Append("</root>");
            }

            return xmlBuilder.ToString();
        }

        public string ConvertXMLtoJSON(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{");

            foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
            {
                if (node.ChildNodes.Count == 1)
                {
                    jsonBuilder.Append($"\"{node.Name}\": \"{node.InnerText}\", ");
                }
                else
                {
                    jsonBuilder.Append($"\"{node.Name}\": [");
                    foreach (XmlNode childNode in node.ChildNodes)
                    {
                        jsonBuilder.Append($"\"{childNode.InnerText}\", ");
                    }
                    jsonBuilder.Remove(jsonBuilder.Length - 2, 2); // Usuwamy ostatnią przecinek i spację
                    jsonBuilder.Append("], ");
                }
            }

            jsonBuilder.Remove(jsonBuilder.Length - 2, 2); // Usuwamy ostatnią przecinek i spację
            jsonBuilder.Append("}");

            return jsonBuilder.ToString();
        }
    }
}
