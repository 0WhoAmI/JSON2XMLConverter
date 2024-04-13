namespace JsonXmlConverter
{
    public interface IConverter
    {
        string BeautyJson(string json);

        string BeautyXml(string xml);

        void ConvertAndSaveToFile(string inputFilePath, string outputFilePath);

        string ConvertJSONtoXML(string json);

        string ConvertXMLtoJSON(string xml);

        bool IsJson(string text);

        bool IsXml(string text);
    }
}