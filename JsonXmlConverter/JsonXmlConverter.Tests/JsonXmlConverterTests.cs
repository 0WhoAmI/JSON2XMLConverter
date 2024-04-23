namespace JsonXmlConverter.Tests
{
    public class JsonXmlConverterTests
    {
        private readonly Core.JsonXmlConverter _converter;

        public JsonXmlConverterTests()
        {
            _converter = new Core.JsonXmlConverter();
        }

        [Fact]
        public void ConvertFromFile_JsonInput_ReturnsValidXml()
        {
            // Arrange
            string inputFilePath = "test.json";
            string json = "{\"name\":\"John\",\"age\":30,\"city\":\"New York\"}";
            File.WriteAllText(inputFilePath, json);

            // Act
            string result = _converter.ConvertFromFile(inputFilePath);

            // Assert
            Assert.Contains("<name>John</name>", result);
            Assert.Contains("<age>30</age>", result);
            Assert.Contains("<city>New York</city>", result);

            // Clean up
            File.Delete(inputFilePath);
        }

        [Fact]
        public void ConvertFromFile_XmlInput_ReturnsValidJson()
        {
            // Arrange
            string inputFilePath = "test.xml";
            string xml = "<person><name>John</name><age>30</age><city>New York</city></person>";
            File.WriteAllText(inputFilePath, xml);

            // Act
            string result = _converter.ConvertFromFile(inputFilePath);

            // Assert
            Assert.Contains("\"name\": \"John\"", result);
            Assert.Contains("\"age\": \"30\"", result);
            Assert.Contains("\"city\": \"New York\"", result);

            // Clean up
            File.Delete(inputFilePath);
        }

        [Fact]
        public void ConvertFromFile_InvalidFile_ThrowsException()
        {
            // Arrange
            string inputFilePath = "invalid.json";
            File.WriteAllText(inputFilePath, "");

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _converter.ConvertFromFile(inputFilePath));
            Assert.Equal("B³¹d pobierania tekstu z pliku", ex.Message);

            // Clean up
            File.Delete(inputFilePath);
        }

        [Theory]
        [InlineData("{\"name\":\"John\",\"age\":30,\"city\":\"New York\"}", true)]
        [InlineData("{name:\"John\",age:30,city:\"New York\"}", false)]
        public void IsJson_ValidJson_ReturnsExpectedResult(string input, bool expected)
        {
            // Act
            bool result = _converter.IsJson(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("<person><name>John</name><age>30</age><city>New York</city></person>", true)]
        [InlineData("<person><name>John</name><age>30</age><city>New York</city>", false)]
        public void IsXml_ValidXml_ReturnsExpectedResult(string input, bool expected)
        {
            // Act
            bool result = _converter.IsXml(input);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}