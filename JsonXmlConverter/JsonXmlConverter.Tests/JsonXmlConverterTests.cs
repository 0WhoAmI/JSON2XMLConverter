using JsonXmlConverter.Core;

namespace JsonXmlConverter.Tests
{
    public class JsonXmlConverterTests
    {
        [Fact]
        public void ConvertJSONtoXML_ValidJSON_ReturnsValidXML()
        {
            // Arrange
            string json = "{\"name\": \"John\", \"age\": 30, \"city\": \"New York\", \"cars\": [\"Ford\", \"BMW\", \"Fiat\"]}";
            IConverter converter = new Core.JsonXmlConverter();

            // Act
            string xml = converter.ConvertJSONtoXML(json);

            // Assert
            Assert.NotNull(xml);
            Assert.Contains("<name>John</name>", xml);
            Assert.Contains("<age>30</age>", xml);
            Assert.Contains("<city>New York</city>", xml);
            Assert.Contains("<cars><carsItem>Ford</carsItem><carsItem>BMW</carsItem><carsItem>Fiat</carsItem></cars>", xml);
        }

        [Fact]
        public void ConvertJSONtoXML_NullJSON_ThrowsArgumentException()
        {
            // Arrange
            string json = null;
            IConverter converter = new Core.JsonXmlConverter();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => converter.ConvertJSONtoXML(json));
        }

        [Fact]
        public void ConvertJSONtoXML_EmptyJSON_ThrowsArgumentException()
        {
            // Arrange
            string json = "";
            IConverter converter = new Core.JsonXmlConverter();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => converter.ConvertJSONtoXML(json));
        }
    }
}