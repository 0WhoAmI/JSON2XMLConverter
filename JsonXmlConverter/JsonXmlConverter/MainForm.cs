using JsonXmlConverter.Core;

namespace JsonXmlConverter.Program
{
    public partial class MainForm : Form
    {
        private readonly IConverter _converter;

        public MainForm(IConverter converter)
        {
            InitializeComponent();
            _converter = converter;
        }

        private void buttonSelectFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFileName = openFileDialog.FileName;
                textBoxInput.Text = File.ReadAllText(selectedFileName);
            }
        }

        private void buttonConvert_Click(object sender, EventArgs e)
        {
            try
            {
                string inputCode = textBoxInput.Text;
                string covertedCode = "";

                if (_converter.IsJson(inputCode))
                {
                    covertedCode = _converter.ConvertJSONtoXML(inputCode);
                }
                else if (_converter.IsXml(inputCode))
                {
                    covertedCode = _converter.ConvertXMLtoJSON(inputCode);
                }
                else
                {
                    MessageBox.Show("Podany kod nie przypomina ani Jsona, ani Xml. Podaj poprawny kod.", "B³¹d", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                textBoxOutput.Text = covertedCode.Replace("\n", Environment.NewLine);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wyst¹pi³ b³¹d: '{ex.Message}'", "B³¹d", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonSaveAs_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"; // Definiujemy filtry, w tym przypadku tylko pliki tekstowe

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Zapisujemy zawartoœæ textBoxOutput do wybranego pliku
                try
                {
                    File.WriteAllText(saveFileDialog.FileName, textBoxOutput.Text);
                    MessageBox.Show("Plik zosta³ zapisany pomyœlnie.", "Zapisano", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Wyst¹pi³ b³¹d podczas zapisu pliku: {ex.Message}", "B³¹d", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}