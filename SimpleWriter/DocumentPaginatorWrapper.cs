using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace ComplexWriter
{
    public class DocumentPaginatorWrapper : DocumentPaginator
    {
        private readonly DocumentPaginator _paginator;
        private Size _contentMargin;
        private double _footerHeight;

        public DocumentPaginatorWrapper(DocumentPaginator paginator)
        {
            _paginator = paginator;
        }

        public Size ContentPageSize { get; set; }

        public Size ContentMargin
        {
            get { return _contentMargin; }
            set { _contentMargin = value; }
        }

        public int FirstPritedPage { get; set; }
        public int LastPrintedPage { get; set; }

        public Typeface FooterTypeface { get; set; }
        public Watermark Mark { get; set; }
        public Brush ForegroundBrush { get; set; }
        public bool ShowNumbering { get; set; }
        public bool UseOldNumbering { get; set; }
        public bool UseLeadingZero { get; set; }
        public Brush BackgroundBrush { get; set; }
        public double FooterFontSize { get; set; }

        public override bool IsPageCountValid
        {
            get { return _paginator.IsPageCountValid; }
        }

        public override int PageCount
        {
            get
            {
                if (FirstPritedPage > _paginator.PageCount - 1)
                    return 0;
                if (FirstPritedPage > LastPrintedPage)
                    return 0;

                return LastPrintedPage - FirstPritedPage + 1;
            }
        }

        public override Size PageSize
        {
            get { return _paginator.PageSize; }

            set { _paginator.PageSize = value; }
        }

        public override IDocumentPaginatorSource Source
        {
            get { return _paginator.Source; }
        }

        public void ComputeValues()
        {
            _paginator.ComputePageCount();

            _paginator.PageSize = new Size(ContentPageSize.Width - ContentMargin.Width*2,
                ContentPageSize.Height - ContentMargin.Height*4);
            var text = new FormattedText("X",
                CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                FooterTypeface, FooterFontSize, ForegroundBrush);
            _footerHeight = text.Height;
        }

        public override DocumentPage GetPage(int pageNumber)
        {
            DocumentPage page = _paginator.GetPage(pageNumber + FirstPritedPage);

            var newpage = new ContainerVisual();
            if(BackgroundBrush != null)
                newpage.Children.Add(GetBackColorVisual(page, BackgroundBrush));

            if (Mark != null && Mark.ImageSource != null)
            {
                DrawingVisual background = GetBackgroundVisual(page);

                newpage.Children.Add(background);
            }

            ContainerVisual smallerPage = GetPageContentVisual(page);
            newpage.Children.Add(smallerPage);

            ContainerVisual footer = GetFooterVisual(page, pageNumber);
            if (footer != null)
                newpage.Children.Add(footer);

            return new DocumentPage(newpage, page.Size, page.BleedBox, page.ContentBox);
        }

        private ContainerVisual GetFooterVisual(DocumentPage page, int pageNumber)
        {
            if (!ShowNumbering) return null;

            var containerVisual = new DrawingVisual();

            using (DrawingContext ctx = containerVisual.RenderOpen())
            {
                int arg0 = pageNumber + 1 + (UseOldNumbering ? FirstPritedPage : 0);
                int pageCount = UseOldNumbering ? _paginator.PageCount : PageCount;
                string args = UseLeadingZero
                    ? arg0.CalculateGoodLead(pageCount)
                    : arg0.ToString(CultureInfo.InvariantCulture);
                string textToFormat = string.Format("{0}/{1}", args, pageCount);
                var text = new FormattedText(textToFormat,
                    CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                    FooterTypeface, FooterFontSize, ForegroundBrush);

                ctx.DrawText(text, new Point(page.ContentBox.Right - text.Width, page.ContentBox.Bottom - text.Height));
            }

            return containerVisual;
        }

        private ContainerVisual GetPageContentVisual(DocumentPage page)
        {
            var mat = new Matrix();
            double d = (page.ContentBox.Height - _footerHeight)/page.ContentBox.Height;
            double scale = d;

            mat.ScaleAt(1d, scale, 0, 0);

            var smallerPage = new ContainerVisual();
            smallerPage.Children.Add(page.Visual);

            if (ShowNumbering)
                smallerPage.Transform = new MatrixTransform(mat);
            return smallerPage;
        }

        private DrawingVisual GetBackgroundVisual(DocumentPage page)
        {
            var background = new DrawingVisual();

            using (DrawingContext ctx = background.RenderOpen())
            {
                var rect = new Rect(page.ContentBox.Right - Mark.Size.Width, page.ContentBox.Bottom - Mark.Size.Height,
                    Mark.Size.Width, Mark.Size.Height - (ShowNumbering ? 27 : 0));
                ctx.DrawImage(Mark.ImageSource, rect);
                ctx.DrawRectangle(new SolidColorBrush(Colors.White) {Opacity = 1 - Mark.Opacity}, null, rect);
            }
            return background;
        }


        private DrawingVisual GetBackColorVisual(DocumentPage page, Brush backgroundBrush)
        {
            var background = new DrawingVisual();

          
            using (DrawingContext ctx = background.RenderOpen())
            {
                ctx.DrawRectangle(backgroundBrush, null,
                    new Rect(0, 0, page.Size.Width, page.Size.Height+200));
            }
            return background;
        }
    }
}