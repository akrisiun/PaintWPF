using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
// using System.Windows.Media.Imaging;

class Load { void Load() 
{
    // Loads the images to tile (no need to specify PngBitmapDecoder, the correct decoder is automatically selected)
System.Windows.Media.Imaging.BitmapFrame frame1 = BitmapDecoder.Create(new Uri(path1), BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames.First();
BitmapFrame frame2 = BitmapDecoder.Create(new Uri(path2), BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames.First();
BitmapFrame frame3 = BitmapDecoder.Create(new Uri(path3), BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames.First();
BitmapFrame frame4 = BitmapDecoder.Create(new Uri(path4), BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames.First();

// Gets the size of the images (I assume each image has the same size)
int imageWidth = frame1.PixelWidth;
int imageHeight = frame1.PixelHeight;

// Draws the images into a DrawingVisual component
System.Windows.Media.DrawingVisual drawingVisual = new DrawingVisual();
using (DrawingContext drawingContext = drawingVisual.RenderOpen())
{
    drawingContext.DrawImage(frame1, new Rect(0, 0, imageWidth, imageHeight));
    drawingContext.DrawImage(frame2, new Rect(imageWidth, 0, imageWidth, imageHeight));
    drawingContext.DrawImage(frame3, new Rect(0, imageHeight, imageWidth, imageHeight));
    drawingContext.DrawImage(frame4, new Rect(imageWidth, imageHeight, imageWidth, imageHeight));
}

// Converts the Visual (DrawingVisual) into a BitmapSource
System.Windows.Media.Imaging.RenderTargetBitmap bmp = new RenderTargetBitmap(imageWidth * 2, imageHeight * 2, 96, 96, PixelFormats.Pbgra32);
bmp.Render(drawingVisual);

// Creates a PngBitmapEncoder and adds the BitmapSource to the frames of the encoder
PngBitmapEncoder encoder = new PngBitmapEncoder();
encoder.Frames.Add(BitmapFrame.Create(bmp));

// Saves the image into a file using the encoder
using (System.IO.Stream stream = File.Create(pathTileImage))
    encoder.Save(stream);
}

   // System.Windows.Media.LineGeometry MyLine1 =      new LineGeometry(        new Point(0, 0),         new Point(100, 50));
         
   public static BitmapSource ConvertBitmap(System.Drawing.Bitmap source)
   {
        return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                      source.GetHbitmap(),
                      IntPtr.Zero,
                      System.Windows.Int32Rect.Empty,
                      BitmapSizeOptions.FromEmptyOptions());
    }

    public static System.Drawing.Bitmap BitmapFromSource(BitmapSource bitmapsource)
    {   Bitmap bitmap;
        using (var outStream = new MemoryStream())
        {
            BitmapEncoder enc = new BmpBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(bitmapsource));
            enc.Save(outStream);
            bitmap = new Bitmap(outStream);
        }

    public void SetSize(Size newSize) {
        SpaceSize = newSize;
        Background = CreateDrawingVisualSpace(Brushes.DimGray, this.FillBrush, default(Point), SpaceSize);
    }

    public void CopyImage(RenderTargetBitmap image1)    {
            System.Windows.Media.Imaging.RenderTargetBitmap bmpCopied = 
            new System.Windows.Media.Imaging.RenderTargetBitmap(
                (int)Math.Round(width), (int)Math.Round(height), 96, 96, PixelFormats.Default);
            
        DrawingVisual dv = new DrawingVisual();
        using (DrawingContext dc = dv.RenderOpen())
        {
            VisualBrush vb = new VisualBrush(image1);
            dc.DrawRectangle(vb, null, new Rect(new Point(), new Size(width, height)));
        }
        bmpCopied.Render(dv);
        Clipboard.SetImage(bmpCopied);  }  } 