using System;
using System.Windows;
using System.Reflection;
using System.Globalization;
using System.Windows.Documents;
using ComplexWriter.Properties;

namespace ComplexWriter.SearchAndReplace
{
    [Flags]
    public enum FindFlags
    {
        FindInReverse = 2,
        FindWholeWordsOnly = 4,
        MatchAlefHamza = 0x20,
        MatchCase = 1,
        MatchDiacritics = 8,
        MatchKashida = 0x10,
        None = 0
    }

    // Never ever use this method in production code.
    public static class DocumentHelper
    {
        private static MethodInfo findMethod = null;

        public static TextRange FindText(TextPointer findContainerStartPosition, TextPointer findContainerEndPosition, String input, FindFlags flags, CultureInfo cultureInfo)
        {
            TextRange textRange = null;
            if (findContainerStartPosition.CompareTo(findContainerEndPosition) < 0)
            {
                try
                {
                    if (findMethod == null)
                    {
                        findMethod = typeof(FrameworkElement).Assembly.GetType("System.Windows.Documents.TextFindEngine").
                               GetMethod("Find", BindingFlags.Static | BindingFlags.Public);
                    }
                    Object result = findMethod.Invoke(null, new Object[] { findContainerStartPosition, 
                    findContainerEndPosition, 
                    input, flags, new CultureInfo(Settings.Default.Language) });
                    textRange = result as TextRange;
                }
                catch (ApplicationException)
                {
                    textRange = null;
                }
            }

            return textRange;
        }
    }
}
