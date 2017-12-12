using System.Windows;
using System.Windows.Controls.Primitives;
using CustomControls;

namespace ComplexWriter.MessageBoxes
{
    /// <summary>
    /// Interaction logic for CloseSettingsMessageBox.xaml
    /// </summary>
    public partial class CloseSettingsMessageBox 
    {
        private EndSettingsResult _endResult;

        public CloseSettingsMessageBox(Window parent) : this()
        {
            Owner = parent;
        }

        public CloseSettingsMessageBox()
        {
            InitializeComponent();
        }

        public EndSettingsResult EndResult
        {
            get { return _endResult; }
            set
            {
                _endResult = value;
                DialogResult = true;
            }

        }

        public void CancelMe()
        {
            EndResult = EndSettingsResult.Cancel;
        }

        private void MoveWindow(object sender, DragDeltaEventArgs e)
        {
            var win = ((Thumb)sender).Tag as Window;
            win.Left = win.Left + e.HorizontalChange;
            win.Top = win.Top + e.VerticalChange;
        }

        private void CloseWithTag(object sender, RoutedEventArgs e)
        {
            EndResult = (EndSettingsResult) ((GlassButton3) sender).Tag;
        }

        private void CloseSettingsMessageBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            ForFocus.Focus();
        }
    }


    public enum EndSettingsResult
    {
        Cancel,
        StoreName,
        ClearName,
        DoNothing
    }

}
