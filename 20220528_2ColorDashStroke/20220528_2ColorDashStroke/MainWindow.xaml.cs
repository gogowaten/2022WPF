using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

//2色の破線枠のテスト
namespace _20220528_2ColorDashStroke
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Test2VisualBrush();
            Test1RectangleRectangle();
            Test3ImageBrush();
        }

        //Rectangleを2つをそのまま使う
        private void Test1RectangleRectangle()
        {
            Rectangle r1 = new() { Width = 100, Height = 50, Stroke = Brushes.Red };
            Rectangle r2 = new()
            {
                Width = 100,
                Height = 50,
                Stroke = Brushes.White,
                StrokeDashArray = new DoubleCollection() { 1.0 }
            };

            Grid g = new(); RenderOptions.SetEdgeMode(g, EdgeMode.Aliased);
            Canvas.SetLeft(g, 120); Canvas.SetTop(g, 10);
            g.Children.Add(r1); g.Children.Add(r2);
            MyCanvas.Children.Add(g);
        }

        //Rectangleを2つをVisualBrushとして使う
        private void Test2VisualBrush()
        {
            double w = 100; double h = 50;
            Rectangle br1 = new() { Width = w, Height = h, Stroke = Brushes.Red };
            Rectangle br2 = new()
            {
                Width = w,
                Height = h,
                Stroke = Brushes.White,
                StrokeDashArray = new DoubleCollection() { 1.0 }
            };
            Grid g = new(); RenderOptions.SetEdgeMode(g, EdgeMode.Aliased);
            g.Children.Add(br1); g.Children.Add(br2);
            VisualBrush vb = new(g);

            Rectangle r = new() { Width = w, Height = h };
            Canvas.SetLeft(r, 120); Canvas.SetTop(r, 70);
            r.Stroke = vb;
            MyCanvas.Children.Add(r);
        }

        //bitmapで破線パターンを作成してImageBrushとして使う
        private void Test3ImageBrush()
        {
            ImageBrush? b = My2ColorDashBrush();
            Rectangle r = new() { Width = 100, Height = 50, Stroke = b, StrokeThickness = 1.0 };
            Canvas.SetLeft(r, 120); Canvas.SetTop(r, 130);
            MyCanvas.Children.Add(r);
        }
        private ImageBrush My2ColorDashBrush()
        {
            WriteableBitmap bitmap = MakeCheckPattern(1, Colors.Blue, Colors.White);
            ImageBrush brush = new(bitmap)
            {
                Stretch = Stretch.None,
                TileMode = TileMode.Tile,
                Viewport = new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight),
                ViewportUnits = BrushMappingMode.Absolute
            };
            return brush;
        }
        //       無限の透明市松模様をWriteableBitmapとImageBrushのタイル表示で作成 - 午後わてんのブログ
        //https://gogowaten.hatenablog.com/entry/15917385
        /// <summary>
        /// 指定した2色から市松模様のbitmapを作成
        /// </summary>
        /// <param name="cellSize">1以上を指定、1指定なら2x2ピクセル、2なら4x4ピクセルの画像作成</param>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        private WriteableBitmap MakeCheckPattern(int cellSize, Color c1, Color c2)
        {
            int width = cellSize * 2;
            int height = cellSize * 2;
            var wb = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
            int stride = wb.Format.BitsPerPixel / 8 * width;
            byte[] pixels = new byte[stride * height];
            Color iro;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if ((y < cellSize & x < cellSize) | (y >= cellSize & x >= cellSize))
                    {
                        iro = c1;
                    }
                    else { iro = c2; }

                    int p = y * stride + x * 4;
                    pixels[p] = iro.B;
                    pixels[p + 1] = iro.G;
                    pixels[p + 2] = iro.R;
                    pixels[p + 3] = iro.A;
                }
            }
            wb.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
            return wb;
        }

        //未使用
        //白と指定色を横に交互に並べた画像、縦1ピクセル
        private WriteableBitmap MakePattern2(int length, Color color)
        {
            int width = length * 2;
            WriteableBitmap wb = new(width, 1, 96, 96, PixelFormats.Rgb24, null);
            int stride = wb.Format.BitsPerPixel / 8 * width;
            byte[] pixels = new byte[stride];
            for (int x = 0; x < length; x += 3)
            {
                pixels[x] = color.R;
                pixels[x + 1] = color.G;
                pixels[x + 2] = color.B;
            }
            for (int x = length; x < width; x += 3)
            {
                pixels[x] = 255;
                pixels[x + 1] = 255;
                pixels[x + 2] = 255;
            }
            wb.WritePixels(new Int32Rect(0, 0, width, 1), pixels, stride, 0);
            return wb;
        }

    }
}
