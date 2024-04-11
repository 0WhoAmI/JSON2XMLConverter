namespace JsonXmlConverter
{
    public interface IConverter
    {
        string ConvertJSONtoXML(string json);
        string ConvertXMLtoJSON(string xml);
    }
}
