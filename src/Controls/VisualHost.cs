using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.IO;
using System.Text;
using Paint.WPF.Tools;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace Paint.WPF.Controls
{
    /// <summary>
    /// DrawingVisual elements class
    /// </summary>
    public class VisualHost : FrameworkElement
    {   
        public new bool IsFocused { get; set; }

        // collection DrawingVisual
        private readonly VisualCollection _visuals;
        public VisualCollection Visuals { get { return _visuals; } }
        public DrawingVisual Background 
        {
            get { return _visuals[0] as DrawingVisual; }
            set {
                if (_visuals.Count == 0)
                    _visuals.Add(value);
                else
                {
                    _visuals[0] = null;
                    if (_visuals[0] == null)
                        _visuals[0] = value;
                }
            }
        }

        #region Create 
        // Properties of root _visuals
        private Brush FillBrush { get; set; }
        private Point Position { get; set; }
        public Size SpaceSize { get; private set; }

        public VisualHost()
        {
            _visuals = new VisualCollection(this);
            _visuals.Add(ClearVisualSpace());

            this.MouseLeftButtonUp += new MouseButtonEventHandler(VisualHost_MouseLeftButtonUp);
            this.MouseLeftButtonDown += new MouseButtonEventHandler(VisualHost_MouseLeftButtonDown);
            this.MouseMove += new MouseEventHandler(VisualHost_MouseMove);
        }  

        /// <summary>
        /// Create
        /// </summary>
        private DrawingVisual CreateDrawingVisualSpace(Brush borderBrush, Brush backgroundBrush, Point position, Size size)
        {
            FillBrush = backgroundBrush;
            Position = position;
            SpaceSize = size;

            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                Rect rect = new Rect(Position, SpaceSize);
                Pen pen = new Pen(borderBrush, 1);

                drawingContext.DrawRectangle(FillBrush, pen, rect);
            }

            return drawingVisual;
        }

        #endregion

        #region Editor methods 1

        private DrawingVisual ClearVisualSpace()
        {
            return CreateDrawingVisualSpace(Brushes.Silver, Brushes.Transparent, new Point(0, 0), new Size(300, 300));
        }

        public void FocusSpace()
        {
            _visuals[0] = null;
            _visuals[0] = CreateDrawingVisualSpace(Brushes.DimGray, FillBrush, Position, SpaceSize);
        }

        public void UnFocusSpace()
        {
            _visuals[0] = null;
            _visuals[0] = CreateDrawingVisualSpace(Brushes.Silver, FillBrush, Position, SpaceSize);
        }

        public void ChangeFill(Brush backgroundBrush)
        {
            _visuals[0] = null;
            _visuals[0] = CreateDrawingVisualSpace(Brushes.DimGray, backgroundBrush, Position, SpaceSize);
        }

        public void ChangeSize(Size newSize)
        {
            _visuals[0] = null;
            _visuals[0] = CreateDrawingVisualSpace(Brushes.DimGray, FillBrush, Position, newSize);
        }

        public void HideWorkSpace()
        {
            _visuals[0] = null;
        }

        public void RestoreWorkSpace()
        {
            if (_visuals[0] == null)
                _visuals[0] = CreateDrawingVisualSpace(Brushes.Silver, FillBrush, Position, SpaceSize);
        }
        #endregion

        #region Points paint
        /// <summary>
        /// Paint points
        /// </summary>
        /// <param name="pt"></param>
        private void DrawPoint(Point pt)
        {
            var drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                Rect rect = new Rect(pt, GlobalState.BrushSize);
                drawingContext.DrawRoundedRectangle(GlobalState.Color, null, rect, GlobalState.BrushSize.Width, GlobalState.BrushSize.Height);
            }
            _visuals.Add(drawingVisual);
        }

        void VisualHost_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            GlobalState.PressLeftButton = true;
            VisualHost_MouseMove(sender, e);
        }

        void VisualHost_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            GlobalState.PressLeftButton = false;
            Point pt = e.GetPosition((UIElement)sender);
        }

        void VisualHost_MouseMove(object sender, MouseEventArgs e)
        {
            switch (GlobalState.CurrentTool)
            {
                case Instruments.Arrow:
                    break;
                case Instruments.Brush:
 
                    if (GlobalState.PressLeftButton && this.IsFocused)
                    {
                        Point pt = e.GetPosition((UIElement)sender);
                        DrawPoint(pt);
                    }
                    break;
            }
        }

        #endregion

        #region FrameworkElement implementation

        protected override int VisualChildrenCount
        {
            get { return _visuals.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _visuals.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return _visuals[index];
        }
        #endregion


        public Image PasteImage()
        { 
            System.Windows.Controls.Image img = new System.Windows.Controls.Image();
            img.RenderSize = SpaceSize;

            IDataObject obj = Clipboard.GetDataObject();
            ImageSource imgSource = null;

            if (obj.GetDataPresent(DataFormats.Bitmap))
            {
                imgSource = ImageConvert.PasteImage();
                // bitmap =  (System.Drawing.Bitmap)obj.GetData(DataFormats.Bitmap, true);
            }

            img.Source = imgSource; 
            return img;
        }

        public void SetSize(Size newSize)
        {
            SpaceSize = newSize;
            // Background = CreateDrawingVisualSpace(Brushes.DimGray, this.FillBrush, default(Point), SpaceSize);
            _visuals[0] = null;
            _visuals[0] = CreateDrawingVisualSpace(Brushes.DimGray, FillBrush, Position, newSize);
        }


        public void CopyImage(Canvas canvas) // RenderTargetBitmap image1)
        {
            double width = canvas.ActualWidth;
            double height = canvas.ActualHeight;

            int dpi = 96;
            var size = new Size(width, height);
            canvas.Measure(size);

            RenderTargetBitmap rtb = new RenderTargetBitmap(
                (int)width,
                (int)height,
                dpi, //dpi x 
                dpi, //dpi y 
                PixelFormats.Pbgra32 // pixelformat 
                );


            var whiteBrush = new SolidColorBrush(Colors.White); // (Color)ColorConverter.ConvertFromString("#FFFFFF"));
            DrawingVisual drawingVisual = new DrawingVisual();
            // double scale = 1.0;

            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                // drawingContext.PushTransform(new ScaleTransform(scale, scale));
                drawingContext.DrawRectangle(whiteBrush, null,
                    new Rect(new Point(0, 0), size));
            }
            rtb.Render(drawingVisual);
            rtb.Render(canvas);

            RenderTargetBitmap image1 = rtb;
            Clipboard.SetImage(image1);
        }

        public void Open(string file)
        {
            var stream = new MemoryStream();
            //  stream.Op
            // TODO:

            Image img = new System.Windows.Controls.Image();


        }

    }
}
