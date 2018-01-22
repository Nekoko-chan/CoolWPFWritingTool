using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace ComplexWriter
{
    public static class Extensions
    {
        public static void AddRange<T>(this ObservableCollection<T> oc, IEnumerable<T> collection)
        {
            if (collection == null) return;
            foreach (var i in collection)
            { oc.Add(i); }
        }

        public static bool HasName(this FontFamily family, string name)
        {
            return family.FamilyNames.Any(elem => string.Equals(elem.Value, name));
        }
    }
}
