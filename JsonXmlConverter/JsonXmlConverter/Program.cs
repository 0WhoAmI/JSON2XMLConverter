namespace JsonXmlConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            string json = "{\"name\": \"John\", \"age\": 30, \"city\": \"New York\", \"cars\": [\"Ford\", \"BMW\", \"Fiat\"]}";

            JsonXmlConverter converter = new JsonXmlConverter();

            string xml = converter.ConvertJSONtoXML(json);
            Console.WriteLine("JSON to XML:");
            Console.WriteLine(xml);

            string convertedJSON = converter.ConvertXMLtoJSON(xml);
            Console.WriteLine("\nXML to JSON:");
            Console.WriteLine(convertedJSON);
        }
    }
}
