using System.Collections.Generic;

namespace ComplexWriter
{
    public class OpenFiles
    {
        public OpenFiles()
        {
            OpenFileNames = new List<string>();
        }
        public List<string> OpenFileNames { get; set; }

        public void Add(string filename)
        {
            if (OpenFileNames.Contains(filename))
                return;
            OpenFileNames.Add(filename);
        }

        public void Clear()
        {
            OpenFileNames.Clear();
        }
    }
}