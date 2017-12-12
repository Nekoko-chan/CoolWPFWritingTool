using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using BlendModeEffects;

namespace ComplexWriter.MessageBoxes
{
    public class MessageResultWindow :Window
    {
        public MessageResultWindow()
        {
            Loaded += MessageResultWindow_Loaded;
            Closed += MessageResultWindow_Closed;
        }

        void MessageResultWindow_Closed(object sender, EventArgs e)
        {
            if (Owner == null) return;
                Owner.Opacity = 1;
            Owner.Effect = null;
        }

        void MessageResultWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (Owner == null) return;

            Owner.Opacity = 0.5;
            Owner.Effect = new BlurEffect();
        }
        private MessageBoxResult _result;

        public MessageBoxResult Result
        {
            get { return _result; }
            set { _result = value; DialogResult = true; }
        }

        public void Cancel()
        {
            Result = MessageBoxResult.Cancel;
        }

        public void Ok()
        {
            Result = MessageBoxResult.OK;
        }
    }
}
