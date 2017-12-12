using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace ComplexWriter.CharacterNames
{
    /// <summary>
    /// Interaction logic for TextButton.xaml
    /// </summary>
    public partial class TextButton
    {
        public TextButton()
        {
            InitializeComponent();
            Loaded+=(s,e)=>_board =InitBoard();
        }

        private Storyboard InitBoard()
        {
            var move = Canvas.ActualWidth - marquee.ActualWidth;
            if (move >= 0)
                return null;


            var board = new Storyboard();
            var doubleAnimation = new DoubleAnimation
            {
                From =0,
                To = move - 20,
                Duration = new Duration(TimeSpan.FromMilliseconds(100 * Caption.Length)),
                RepeatBehavior = RepeatBehavior.Forever,
                AutoReverse = true
            };
            Storyboard.SetTarget(doubleAnimation, marquee);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(Canvas.Left)"));

            board.Children.Add(doubleAnimation);
            return board;
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register("Caption", typeof(string), typeof(TextButton), new UIPropertyMetadata("Beispieltext",CaptionChanged));

        private static void CaptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textButton = ((TextButton)d);
            textButton._board = textButton.InitBoard();
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
            if (_board == null)
                _board = InitBoard();
            _board?.Begin(this, true);
        }

        private void MouseLeaved(object sender, MouseEventArgs e)
        {
            if(_board == null) return;

            _board.Stop(this);
            Canvas.SetLeft(marquee,0d);
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TextTopProperty =
            DependencyProperty.Register("TextTop", typeof(double), typeof(TextButton), new UIPropertyMetadata(0d));

        /// <summary>
        /// Get or set received TextTop
        /// </summary>
        public double TextTop
        {
            get { return (double)GetValue(TextTopProperty); }
            set { SetValue(TextTopProperty, value); }
        }
    }
}
