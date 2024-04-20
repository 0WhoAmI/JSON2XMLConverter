namespace JsonXmlConverter.Core
{
    public interface IConverter
    {
        string ConvertFromFile(string inputFilePath);

        string ConvertJSONtoXML(string json);

        string ConvertXMLtoJSON(string xml);

        bool IsJson(string text);

        bool IsXml(string text);
    }
}