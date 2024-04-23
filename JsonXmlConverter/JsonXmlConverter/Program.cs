using JsonXmlConverter.Core;

namespace JsonXmlConverter.Program
{
    public class Program
    {
        private readonly IConverter _converter;
        private readonly FileHandler _fileHandler;
        private string outputFilePath = "output.txt";

        public Program(IConverter converter,
            FileHandler fileHandler)
        {
            _converter = converter;
            _fileHandler = fileHandler;
        }

        [STAThread]
        public static void Main(string[] args)
        {
            ApplicationConfiguration.Initialize();

            Core.JsonXmlConverter converter = new Core.JsonXmlConverter();

            Application.Run(new MainForm(converter));

            //FileHandler fileHandler = new FileHandler();
            //Program program = new Program(converter, fileHandler);
            //program.RunCmd();
        }

        // Okno konsolowe
        private void RunCmd()
        {
            Console.WriteLine("Wybierz sposób konwersji:");
            Console.WriteLine("1. Konwersja z pliku");
            Console.WriteLine("2. Konwersja z wprowadzonych danych");

            int conversionSourceChoice;
            while (!int.TryParse(Console.ReadLine(), out conversionSourceChoice) || conversionSourceChoice < 1 || conversionSourceChoice > 2)
            {
                Console.WriteLine("Niepoprawny wybór. Wpisz 1 lub 2.");
            }

            while (true)
            {
                try
                {
                    if (conversionSourceChoice == 1)
                    {
                        Console.WriteLine("Podaj ścieżkę do pliku:");

                        string inputFilePath = "";
                        while (true)
                        {
                            inputFilePath = Console.ReadLine();
                            if (_fileHandler.FileExists(inputFilePath))
                                break;
                            else
                                Console.WriteLine("Plik nie istnieje. Podaj poprawną ścieżkę:");
                        }

                        string covertedCode = _converter.ConvertFromFile(inputFilePath);

                        //Console.WriteLine(covertedCode);
                        File.WriteAllText(outputFilePath, covertedCode);
                    }
                    else if (conversionSourceChoice == 2)
                    {
                        Console.WriteLine("Podaj kod do konwersji:");

                        string inputCode = "";
                        string covertedCode = "";
                        while (true)
                        {
                            inputCode = Console.ReadLine();
                            if (_converter.IsJson(inputCode))
                            {
                                covertedCode = _converter.ConvertJSONtoXML(inputCode);

                                break;
                            }
                            else if (_converter.IsXml(inputCode))
                            {
                                covertedCode = _converter.ConvertXMLtoJSON(inputCode);

                                break;
                            }
                            else
                            {
                                Console.WriteLine("Podany kod nie przypomina ani Jsona, ani Xml. Podaj poprawny kod:");
                            }
                        }

                        //Console.WriteLine(covertedCode);
                        File.WriteAllText(outputFilePath, covertedCode);
                    }

                    Console.WriteLine("Konwersja zakończona pomyślnie. Otwieram plik z wynikiem...");

                    _fileHandler.OpenFile(outputFilePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Wystąpił błąd: '{ex.Message}'");
                }
            }
        }
    }
}