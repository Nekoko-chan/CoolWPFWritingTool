using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ComplexWriter
{
    /// <summary>
    /// Interaction logic for MyPageViewer.xaml
    /// </summary>
    public partial class MyPageViewer : UserControl
    {
        public MyPageViewer()
        {
            InitializeComponent();
            Loaded += MyPageViewer_Loaded;
        }

        void MyPageViewer_Loaded(object sender, RoutedEventArgs e)
        {
            var binding = new Binding
            {
                Source = PageViewer,
                Path = new PropertyPath("PageCount"),
                Mode = BindingMode.OneWay
            };
            SetBinding(PageCountProperty, binding);
            ShowWholePage();
        }


        public static readonly DependencyProperty DocumentProperty =
       DependencyProperty.Register("Document", typeof(FlowDocument), typeof(MyPageViewer),
           new PropertyMetadata(null,UpdatePaginator));

        private static void UpdatePaginator(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var document = e.NewValue as FlowDocument;
            if (document == null)
                return;
        }

        public static readonly DependencyProperty PageCountProperty =
      DependencyProperty.Register("PageCount", typeof(int), typeof(MyPageViewer),
          new PropertyMetadata(0));

        public int PageCount
        {
            get { return (int)GetValue(PageCountProperty); }
            set { SetValue(PageCountProperty, value); }
        }

        public static readonly DependencyProperty PageBackgroundProperty =
     DependencyProperty.Register("PageBackground", typeof(Brush), typeof(MyPageViewer),
         new PropertyMetadata(null));

        public Brush PageBackground
        {
            get { return (Brush)GetValue(PageBackgroundProperty); }
            set { SetValue(PageBackgroundProperty, value); }
        }

        public FlowDocument Document
        {
            get { return (FlowDocument)GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }

        private void SetZoomAdobe(object sender, RoutedEventArgs e)
        {
            PageViewer.Zoom = 120;
        }

        private void SetZoom100(object sender, RoutedEventArgs e)
        {
            PageViewer.Zoom = 100;
        }

        private void SetPage(object sender, RoutedEventArgs e)
        {
            ShowWholePage();
        }

        private void ShowWholePage()
        {
            var possibleHeight = Viewer.ActualHeight - 10;
            var possibleWidth = Viewer.ActualWidth - 10;

            var hight = possibleHeight*100/1120;
            var width = possibleWidth*100/793;

            PageViewer.Zoom = Math.Min(hight, width);
        }

        private void GoToPage(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var number = e.NewValue;
            PageViewer.GoToPage((int)number);
        }
    }
}
