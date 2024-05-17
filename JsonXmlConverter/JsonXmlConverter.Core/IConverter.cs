namespace JsonXmlConverter.Core
{
    public interface IConverter
    {
        /// <summary>
        /// Konwertuje zawartość pliku z formatu JSON na XML lub z XML na JSON.
        /// Automatycznie rozpoznaje format danych wejściowych.
        /// </summary>
        /// <param name="inputFilePath">Ścieżka do pliku wejściowego zawierającego dane w formacie JSON lub XML.</param>
        /// <returns>Skonwertowane dane w przeciwnym formacie (JSON na XML lub XML na JSON).</returns>
        string ConvertFromFile(string inputFilePath);

        /// <summary>
        /// Konwertuje dane w formacie JSON na format XML.
        /// </summary>
        /// <param name="json">Dane w formacie JSON.</param>
        /// <returns>Skonwertowane dane w formacie XML.</returns>
        string ConvertJSONtoXML(string json);

        /// <summary>
        /// Konwertuje dane w formacie XML na format JSON.
        /// </summary>
        /// <param name="xml">Dane w formacie XML.</param>
        /// <returns>Skonwertowane dane w formacie JSON.</returns>
        string ConvertXMLtoJSON(string xml);

        /// <summary>
        /// Sprawdza, czy dane wejściowe są w formacie JSON.
        /// </summary>
        /// <param name="text">Dane wejściowe do sprawdzenia.</param>
        /// <returns>True, jeśli dane są w formacie JSON; w przeciwnym razie False.</returns>
        bool IsJson(string text);

        /// <summary>
        /// Sprawdza, czy dane wejściowe są w formacie XML.
        /// </summary>
        /// <param name="text">Dane wejściowe do sprawdzenia.</param>
        /// <returns>True, jeśli dane są w formacie XML; w przeciwnym razie False.</returns>
        bool IsXml(string text);
    }
}