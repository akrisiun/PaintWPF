using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Paint.WPF.Controls;
using Paint.WPF.Tools;

namespace Paint.WPF
{
    /// <summary>
    /// Main window
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Init
        public MainWindow()
        {
            InitializeComponent();
            LayersWidgets = new ObservableCollection<LayerWidget>();
            LayerList.DataContext = this;

            GlobalState.ChangeInstrument += SetCursorStyle;
            GlobalState.ChangeColor += SetColorSample;

            GlobalState.Color = Brushes.Black;
            GlobalState.BrushSize = new Size(3, 3);

            MainCanvas.ClipToBounds = true; // LayersExpander.IsExpanded;
            LayersExpander.IsExpanded = false;
            LayersExpanded(null, null);
        }

        public ObservableCollection<LayerWidget> LayersWidgets { get; set; }

        private void Load(object sender, RoutedEventArgs e)
        {
            LayerAdd_Click(null, null);
            Arrow_Selected(null, null);

            SizeX.Text = Math.Round(MainCanvas.ActualWidth, 0).ToString();
            SizeY.Text = Math.Round(MainCanvas.ActualHeight, 0).ToString();
        }
        #endregion

        #region Layers

        /// <summary>
        /// new layer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LayerAdd_Click(object sender, RoutedEventArgs e)
        {
            var layer = new LayerControl();
            MainCanvas.Children.Add(layer);
            LayersWidgets.Add(layer.Widget);

            // new level ZOrder to top
            LayerWidget last = LayersWidgets.Last();
            for (int i = LayersWidgets.Count - 1; i > 0; i--)
            {
                LayersWidgets[i] = LayersWidgets[i - 1];
            }
            LayersWidgets[0] = last;

            GlobalState.LayersIndexes++;

            if (LayerList.Items.Count > 0)
                LayerList.SelectedIndex = 0;

            layer.CheckedChanged += SelectLayer;
            layer.Delete += DeleteLayer;
        }

        /// <summary>
        ///     delete layer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteLayer(Object sender, EventArgs e)
        {
            if (sender != null)
            {
                LayersWidgets.Remove(((LayerControl) sender).Widget);
                MainCanvas.Children.Remove((LayerControl) sender);
                for (int i = ((LayerControl) sender).LayerIndex; i < MainCanvas.Children.Count; i++)
                {
                    var upperLayerObj = MainCanvas.Children[i];
                    if (!(upperLayerObj is LayerControl))
                        break;
                    var upperLayer = upperLayerObj as LayerControl;
                    upperLayer.LayerIndex--;
                    int curZIndex = Panel.GetZIndex(upperLayer);
                    Panel.SetZIndex(upperLayer, --curZIndex);
                }
            }
        }

        /// <summary>
        ///     select layer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectLayer(Object sender, LayerControl.CheckedEventArgs e)
        {
            if (sender != null && e.IsChecked)
            {
                for (int i = 0; i < LayersWidgets.Count; i++)
                {
                    if (LayersWidgets[i].ThisLayer.LayerIndex == ((LayerControl) sender).LayerIndex)
                    {
                        LayerList.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        #endregion

        #region brush

        /// <summary>
        ///     brush change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeBrushColor(object sender, RoutedEventArgs e)
        {
            GlobalState.Color = ((Button) sender).Background;
            Brush_Selected(null, null);
        }


        /// <summary>
        ///    clear borders
        /// </summary>
        private void ClearToolsBorders()
        {
            ArrowButton.BorderThickness = new Thickness(0);
            BrushButton.BorderThickness = new Thickness(0);
        }
        #endregion

        #region cursor 

        /// <summary>
        ///     mouse cursor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetCursorStyle(Object sender, EventArgs e)
        {
            switch (GlobalState.CurrentTool)
            {
                case Instruments.Brush:
                    MainCanvas.Cursor = Cursors.Cross;
                    break;
                default:
                    MainCanvas.Cursor = Cursors.Arrow;
                    break;
            }
        }

        /// <summary>
        ///     current color
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetColorSample(Object sender, EventArgs e)
        {
            CurBrushSample.Fill = GlobalState.Color;
        }

        #endregion

        #region Layer clicks

        private void LayerUp_Click(object sender, RoutedEventArgs e)
        {
            if (LayerList.SelectedIndex > 0)
                SwapLayers(LayerList.SelectedIndex, LayerList.SelectedIndex - 1);
        }

        private void LayerDown_Click(object sender, RoutedEventArgs e)
        {
            if (LayerList.SelectedIndex < LayerList.Items.Count - 1)
                SwapLayers(LayerList.SelectedIndex, LayerList.SelectedIndex + 1);
        }

        private void SwapLayers(int curIndx, int nextIndx)
        {
            LayerWidget curWidget = LayersWidgets[curIndx];
            LayerWidget nextWidget = LayersWidgets[nextIndx];

            LayersWidgets[curIndx] = LayersWidgets[nextIndx];
            LayersWidgets[nextIndx] = curWidget;
            LayerList.SelectedIndex = nextIndx;

            int curZIndex = Panel.GetZIndex(MainCanvas.Children[curWidget.ThisLayer.LayerIndex]);
            int nextZIndex = Panel.GetZIndex(MainCanvas.Children[nextWidget.ThisLayer.LayerIndex]);

            Panel.SetZIndex(MainCanvas.Children[curWidget.ThisLayer.LayerIndex], nextZIndex);
            Panel.SetZIndex(MainCanvas.Children[nextWidget.ThisLayer.LayerIndex], curZIndex);
        }

        private void LayerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LayerList.SelectedItems.Count > 0)
            {
                LayerWidget selectedWidget = LayersWidgets[LayerList.SelectedIndex];
                object layer = null;
                if (MainCanvas.Children.Count > selectedWidget.ThisLayer.LayerIndex)
                    layer = MainCanvas.Children[selectedWidget.ThisLayer.LayerIndex];
                if (layer != null && layer is UIElement)
                    (layer as UIElement).Focus();
                else
                    return;

                foreach (var child in MainCanvas.Children)
                {
                    if (child is LayerControl && child != layer)
                    {
                        LayerControl layCtrl = child as LayerControl;
                        layCtrl.NonFocus(null, null);
                    }
                }
            }

            if (LayersWidgets.Count == 0)
                return;
            if (LayerList.SelectedIndex < 0)
                LayerList.SelectedIndex = 0;

            var selectedObj = LayersWidgets[LayerList.SelectedIndex];
            var host = selectedObj.ThisLayer.VisualHost;
            SizeX.Text = Math.Round(host.SpaceSize.Width, 0).ToString();
            SizeY.Text = Math.Round(host.SpaceSize.Height, 0).ToString(); 
        }

        private void HideAllWorkSpaces()
        {
            foreach (LayerControl layer in MainCanvas.Children)
            {
                layer.VisualHost.HideWorkSpace();
            }
        }

        private void RestoreAllWorkSpaces()
        {
            foreach (LayerControl layer in MainCanvas.Children)
            {
                layer.VisualHost.RestoreWorkSpace();
            }
        }

        #endregion
        
        // Expanded/Collapsed
        private void LayersExpanded(object sender, RoutedEventArgs e)
        {
            if (MainCanvas == null) return;

            // MainCanvas.ClipToBounds = true; // LayersExpander.IsExpanded;

            MainCanvas.SetValue(Grid.ColumnSpanProperty, LayersExpander.IsExpanded ? 1 : 3);

            LayersSplitter.Visibility = LayersExpander.IsExpanded ? System.Windows.Visibility.Visible
                : System.Windows.Visibility.Hidden;
        }

        #region export

        private void SaveCanvas(Canvas canvas, int dpi, string filename)
        {
            var width = canvas.ActualWidth;
            var height = canvas.ActualHeight;

            RenderTargetBitmap rtb = RenderLib.Render(canvas, new Size(width, height));
            SaveAsPng(rtb, filename);
        }

        private static void SaveAsPng(RenderTargetBitmap bmp, string filename)
        {
            var enc = new PngBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(bmp));

            using (FileStream stm = File.Create(filename))
            {
                enc.Save(stm);
            }
        }

        #endregion

        #region Brush tools

        private void Brush_Selected(object sender, RoutedEventArgs e)
        {
            GlobalState.CurrentTool = Instruments.Brush;
            ClearToolsBorders();
            BrushButton.BorderThickness = new Thickness(0.5);
        }

        private void Arrow_Selected(object sender, RoutedEventArgs e)
        {
            GlobalState.CurrentTool = Instruments.Arrow;
            ClearToolsBorders();
            ArrowButton.BorderThickness = new Thickness(0.5);
        }

        #endregion

        private void SavePicture(object sender, RoutedEventArgs e)
        {
            var timeLocal =  DateTime.Now.ToLocalTime();
            string file = "Picture_" + timeLocal.ToShortDateString().Replace(".", "").Replace("-", "").Replace(" ", "")
                        + "_" + timeLocal.ToShortTimeString().Replace(":", "").Substring(0, 4);

            var saveDlg = new SaveFileDialog
            {
                FileName = file,
                DefaultExt = ".png",
                Filter = "PNG (.png)|*.png"
            };

            if (saveDlg.ShowDialog() == true)
            {
                HideAllWorkSpaces();

                SaveCanvas(MainCanvas, 96, saveDlg.FileName);

                RestoreAllWorkSpaces();
            }
        }

        public VisualHost Host
        {
            get
            {
                if (LayerList.SelectedIndex < 0)
                    LayerList.SelectedIndex = 0;

                if (LayersWidgets.Count == 0)
                    LayerAdd_Click(null, null);

                LayerWidget selectedObj = LayersWidgets[LayerList.SelectedIndex];
                return selectedObj.ThisLayer.VisualHost;
            }
        }

        private void Size_Click(object sender, RoutedEventArgs e)
        {
            if (LayerList.SelectedIndex < 0)
                LayerList.SelectedIndex = 0;

            var selectedObj = LayersWidgets[LayerList.SelectedIndex];
            var host = selectedObj.ThisLayer.VisualHost;
            double newWidth = Math.Round(host.ActualWidth, 0);
            double newHeight = Math.Round(host.ActualHeight, 0);

            if (Convert.ToDouble(SizeX.Text) > 0)
                newWidth = Convert.ToDouble(SizeX.Text);
            else
                SizeX.Text = newWidth.ToString();

            if (Convert.ToDouble(SizeY.Text) > 0)
                newHeight = Convert.ToDouble(SizeY.Text);
            else 
                SizeY.Text = newWidth.ToString();

            host.SetSize(new Size(newWidth, newHeight));

            if (MainCanvas.ActualWidth < selectedObj.ActualWidth)
                MainCanvas.Width = selectedObj.ActualWidth;
            if (MainCanvas.ActualHeight < selectedObj.ActualHeight)
                MainCanvas.Height = selectedObj.ActualHeight;

            // var host = Host;
            // var newSize = selectedObj.RenderSize;
            // if (newSize.Height > host.SpaceSize.Height || newSize.Width > host.SpaceSize.Width)
            //  host.SetSize(newSize);
        }

        private void Button_Copy(object sender, RoutedEventArgs e)
        {
            VisualHost host = Host;  
            Canvas canvas = MainCanvas;
            host.CopyImage(canvas);

        }

        private Microsoft.Win32.OpenFileDialog dlg = null;

        private void Button_Open(object sender, RoutedEventArgs e)
        {
            if (dlg == null)
            {
                dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.RestoreDirectory = false;
                dlg.ShowReadOnly = true;
                dlg.Filter = "Images|*.png;*.jpg";
            }

            bool? ok = dlg.ShowDialog();
            if (!ok.HasValue || !File.Exists(dlg.FileName))
                return;

            string fileName = dlg.FileName;
            var host = Host;
            host.Open(fileName);
        }

        private void Button_Paste(object sender, RoutedEventArgs e)
        {
            var host = Host;
            Image img = host.PasteImage();
            if (img == null || img.Source == null)
                return;

            var canvas = MainCanvas;
            img.Stretch = Stretch.None;

            Point zeroPos = new Point(0, 0);
            Size sizeCanv = new Size(MainCanvas.ActualWidth, MainCanvas.ActualHeight);
            Size sizeImg = new Size(img.Source.Width, img.Source.Height);
            
            //var rect = new System.Windows.Shapes.Rectangle();
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                // drawingContext.DrawRectangle(System.Windows.Media.Brushes.LightBlue, (System.Windows.Media.Pen)null, new Rect(new Point(0, 0), sizeCanv));
                drawingContext.DrawImage(img.Source, new Rect(zeroPos, sizeImg));
            }

            host.Visuals.Add(drawingVisual);
        }
    }

    public static class RenderLib
    {
        public static RenderTargetBitmap Render(Canvas canvas, Size size, int dpi = 96)
        {
            canvas.Measure(size);
            RenderTargetBitmap rtb = new RenderTargetBitmap(
                (int)size.Width,
                (int)size.Height,
                dpi, //dpi x 
                dpi, //dpi y 
                PixelFormats.Pbgra32 // pixelformat 
                );

            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                var whiteBrush = new SolidColorBrush(Colors.White);
                drawingContext.DrawRectangle(whiteBrush, null,
                    new Rect(new Point(0, 0), size));
            }

            rtb.Render(drawingVisual);
            rtb.Render(canvas);

            return rtb;
        }
    }

}