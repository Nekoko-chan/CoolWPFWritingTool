using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using ComplexWriter.Annotations;

namespace ComplexWriter
{
    public class TagHandler :INotifyPropertyChanged
    {
        private List<string> _suggestions = new List<string>();

        public TagHandler()
        {
            var exeDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            TagPath = Path.Combine(exeDir, "Tags.lex");
        }

        public TagHandler(string path)
        {
			TagPath = Path.Combine(path, "Tags.lex");
        }

        public string TagPath { get; }
        //public ObservableCollection<string> Tags { get; set; } = new ObservableCollection<string>();

        public List<string> Suggestions 
        {
            get { return _suggestions; }
            set
            {
                if (Equals(value, _suggestions)) return;
                _suggestions = value;
                OnPropertyChanged();
            }
        }

        public void LoadTags()
        {
            if (!File.Exists(TagPath)) return;

            string[] lines = File.ReadAllLines(TagPath);
            Suggestions.AddRange(lines);
        }
	
		public bool AddTag(string tag)
		{
			return AddTag(tag,Suggestions,TagPath);
		}
		
        private bool AddTag(string tag, List<string> collection, string path)
        {
            if (collection.Contains(tag)) return false;

            collection.Add(tag);

            try
            {
                using (var sw = File.AppendText(path))
                {
                    sw.WriteLine(tag);
                }
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                return false;
            }
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
