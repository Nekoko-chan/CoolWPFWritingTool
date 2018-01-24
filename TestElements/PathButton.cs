using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TestElements
{
    [TemplatePart(Name = "PART_Path", Type = typeof (Path))]
    public class PathButton : Button
    {
        /// <summary>
        ///     DependencyProperty for 'Data'
        /// </summary>
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof (Geometry), typeof (PathButton), new UIPropertyMetadata());

        /// <summary>
        ///     DependencyProperty for 'Fill'
        /// </summary>
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof (Brush), typeof (PathButton),
                new UIPropertyMetadata(Brushes.Transparent));

        /// <summary>
        ///     DependencyProperty for 'Stroke'
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof (Brush), typeof (PathButton),
                new UIPropertyMetadata(Brushes.Transparent));

        /// <summary>
        ///     DependencyProperty for 'StrokeThickness'
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof (double), typeof (PathButton),
                new UIPropertyMetadata(0d));

        static PathButton()
        {
            // lookless control, get default style from generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof (PathButton),
                new FrameworkPropertyMetadata(typeof (PathButton)));
        }

        /// <summary>
        /// </summary>
        public Geometry Data
        {
            get { return (Geometry) GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        /// <summary>
        /// </summary>
        public Brush Fill
        {
            get { return (Brush) GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        /// <summary>
        /// </summary>
        public Brush Stroke
        {
            get { return (Brush) GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        /// <summary>
        /// </summary>
        public double StrokeThickness
        {
            get { return (double) GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }
    }
}