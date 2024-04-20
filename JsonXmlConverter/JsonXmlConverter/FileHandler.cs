using System.Diagnostics;

namespace JsonXmlConverter.Program
{
    public class FileHandler
    {
        public void OpenFile(string filePath)
        {
            Process.Start("notepad.exe", filePath);
        }

        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}