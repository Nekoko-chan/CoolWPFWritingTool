using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace test
{
    /// <summary>
    /// Interaction logic for TextButton.xaml
    /// </summary>
    public partial class TextButton : Button
    {
        public TextButton()
        {
            InitializeComponent();
            Loaded+=(s,e)=>InitBoard();
        }

        private void InitBoard()
        {
            marquee.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            _board = new Storyboard();
            var doubleAnimation = new DoubleAnimation
            {
                From =0,
                To = -marquee.ActualWidth,
                Duration = new Duration(TimeSpan.FromSeconds(2)),
                RepeatBehavior = RepeatBehavior.Forever,
                AutoReverse = true
            };
            Storyboard.SetTarget(doubleAnimation, marquee);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(Canvas.Left)"));

            _board.Children.Add(doubleAnimation);
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register("Caption", typeof(string), typeof(TextButton), new UIPropertyMetadata("Beispieltext",CaptionChanged));

        private static void CaptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textButton = ((TextButton)d);
            textButton.InitBoard();
        }

        /// <summary>
        /// Get or set received Caption
        /// </summary>
        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty OverlayOpacityProperty =
            DependencyProperty.Register("OverlayOpacity", typeof(double), typeof(TextButton), new UIPropertyMetadata(0.5d));

        /// <summary>
        /// Get or set received OverlayOpacity
        /// </summary>
        public double OverlayOpacity
        {
            get { return (double)GetValue(OverlayOpacityProperty); }
            set { SetValue(OverlayOpacityProperty, value); }
        }

        private Storyboard _board;

        private void MouseEntered(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("Started");
            _board.Begin(this, true);
        }

        private void MouseLeaved(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("Stopped");
            _board.Stop(this);
            Canvas.SetLeft(marquee,0d);
        }
    }
}
