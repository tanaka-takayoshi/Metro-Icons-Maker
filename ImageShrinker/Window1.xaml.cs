using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Globalization;
using System.Windows.Media.Animation;

namespace ImageShrinker
{
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        #region InitializeText
        private string notice1 = "Trim area to be Icon by Mouse";
        private string notice2 = "Click [Save Icons] button if satisfied";
        private string notice3 = "Created folder and Save Completed";
        private string openFilter = "Image Files (*.png, *.jpg)|*.png;*.jpg|All files (*.*)|*.*";
        private string openFolder = "Icons with same project name are already exist, please change your project name";
        private string notice4 = "Please load .PNG or .JPG";
        private string notice5 = "Fail to make icons, Trim area to be Icon by Mouse";
        private bool IsPasted = false;
        private bool IsJapanese = false;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CultureInfo.CurrentUICulture.Name == "ja-JP" || App.Language == "Japanese") IsJapanese = true;
            if (App.Language == "English") IsJapanese = false;
            if (IsJapanese)
            {
                Save.Content = "ストア用にアイコン保存";
                Open.Content = "画像を開く";
                projectLabel.Content = "プロジェクト名";
                CompleteNotice.Content = "クリップボードからコピー＆貼り付けするか、画像をドラッグ＆ドロップするか、[画像を開く]をクリック";
                notice1 = "アイコンにしたい部分をマウスで囲む";
                notice2 = "気に入ったら[アイコン保存]ボタンをクリック";
                notice3 = "フォルダーを作成し、保存しました";
                notice4 = "PNG ファイルか JPG ファイルをロードしてください";
                notice5 = "アイコン作成に失敗、"+notice1;
                openFilter = "画像ファイル (*.png, *.jpg)|*.png;*.jpg|すべてのファイル (*.*)|*.*";
                openFolder = "同じプロジェクト名のアイコンが既にあります、別のプロジェクト名に変更してください";
            }
        }
        #endregion InitializeText

        #region OpenImage

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = openFilter;

            Nullable<bool> result = dlg.ShowDialog();

            if (result.Value)
            {
                ShowImage(dlg.FileName);
            }
            IsPasted = false;
        }

        /// <summary>
        /// Handle Copy & Past (ctrl+V)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl)) && (e.Key == Key.V))
            {
                if (Clipboard.ContainsImage())
                {
                    BitmapSource source = Clipboard.GetImage();
                    ShowImage(source);
                }
            }
            
        }
        #endregion OpenImage

        #region ShowImage

        // Show Image for Copy & Paste
        private void ShowImage(BitmapSource source)
        {
            projectName.Text = "MyProject";
            fileName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\MyProject";
            BmpBitmapEncoder encoder = new BmpBitmapEncoder();
            MemoryStream memoryStream = new MemoryStream();
            imageSource = new BitmapImage();
            encoder.Frames.Add(BitmapFrame.Create(source));
            encoder.Save(memoryStream);
            imageSource.BeginInit();
            imageSource.StreamSource = new MemoryStream(memoryStream.ToArray());
            imageSource.EndInit(); memoryStream.Close();

            grayImage.Source = imageSource;
            if (imageSource.PixelHeight < 600 && imageSource.PixelWidth < 800)
            {
                grayImage.Width = imageSource.PixelWidth;
                grayImage.Height = imageSource.PixelHeight;
            }
            else
            {
                grayImage.Width = 800;
                grayImage.Height = 600;
            }
            NewImageDisplayed();
        }
        
        // Show Image for file load and drag
        private void ShowImage(string filename)
        {
            fileName = filename;
            imageSource = new BitmapImage();
            imageSource.BeginInit();
            imageSource.UriSource = new Uri(fileName);
            imageSource.EndInit();

            grayImage.Source = imageSource;
            if (imageSource.PixelHeight < 600 && imageSource.PixelWidth < 800)
            {
                grayImage.Width = imageSource.PixelWidth;
                grayImage.Height = imageSource.PixelHeight;
            }
            else
            {
                grayImage.Width = 800;
                grayImage.Height = 600;
            }
            NewImageDisplayed();
        }
        private void DisplayIconNames()
        {
            string name = projectName.Text;
            name620_300.Text = name + "-Splash.png";
            name620_300.Visibility = Visibility.Visible;
            name310_150.Text = name + ".png";
            name310_150.Visibility = Visibility.Visible;
            name150.Text = name + "-single.png";
            name150.Visibility = Visibility.Visible;
            name50.Text = name + "-store.png";
            name50.Visibility = Visibility.Visible;
            name30.Text = name + "-small.png";
            name30.Visibility = Visibility.Visible;
        }
        private void projectName_TextChanged(object sender, TextChangedEventArgs e)
        {
            DisplayIconNames();
        }
        #endregion ShowImage

        #region SaveIcons
        private void Save_Click(object sender, RoutedEventArgs e)
        {

            if (Icon620_300.Fill != null)
            {
                string path = System.IO.Path.GetDirectoryName(this.fileName);
                path = path + "\\" + projectName.Text + " Icons";
                if (Directory.Exists(path))
                {
                    MessageBox.Show(openFolder);
                    return;
                }
                System.IO.Directory.CreateDirectory(path);
                EncodeCanvas(Canvas620_300, Icon620_300, name620_300.Text, path);
                EncodeCanvas(Canvas310_150, Icon310_150, name310_150.Text, path);
                EncodeAndSave(Icon150, name150.Text, path);
                EncodeAndSave(Icon50, name50.Text, path);
                EncodeAndSave(Icon30, name30.Text as string, path);
                string folder = "On same folder with your image";
                if (IsPasted) folder = "On your desktop";
                if (IsJapanese)
                {
                    folder = "画像と同じフォルダーに";
                    if (IsPasted) folder = "デスクトップに";
                }
                CompleteNotice.Content = folder + projectName.Text + " Icons" + notice3;
                CompleteNotice.Visibility = Visibility.Visible;

                Storyboard effect = (Storyboard)this.FindResource("Storyboard1");
                BeginStoryboard(effect);
            }
            else
            {
                CompleteNotice.Content = notice5;
            }
            
        }
        private void EncodeCanvas(Canvas canvas, Rectangle icon, string name, string filePath)
        {
            RenderTargetBitmap targetBitmap =
                new RenderTargetBitmap((int)canvas.Width,
                                       (int)canvas.Height,
                                       96d, 96d,
                                       PixelFormats.Pbgra32);
            var isolatedVisual = new DrawingVisual();
            var canvasLeft = (icon.GetValue(Canvas.LeftProperty) as double?) ?? 0d;
            canvasLeft = double.IsNaN(canvasLeft) ? 0d : canvasLeft;
            var canvasTop = (icon.GetValue(Canvas.TopProperty) as double?) ?? 0d;
            canvasTop = double.IsNaN(canvasTop) ? 0d : canvasTop;
            using (var drawing = isolatedVisual.RenderOpen())
            {
                drawing.DrawRectangle(new VisualBrush(icon), null, new Rect(new Point(canvasLeft, canvasTop), new Size((int)icon.Width, (int)icon.Height)));
            }
            targetBitmap.Render(isolatedVisual);

            // add the RenderTargetBitmap to a Bitmapencoder
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(targetBitmap));
            
            // save file to disk
            string fileOut = filePath + "\\" + name;
            using (var fs = File.Open(fileOut, FileMode.OpenOrCreate))
            {
                encoder.Save(fs);
            }
        }
        private void EncodeAndSave(FrameworkElement icon, string name, string filePath)
        {
            // Create BitmapFrame for Icon
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)icon.Width, (int)icon.Height, 96.0, 96.0, PixelFormats.Pbgra32);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(icon);
                dc.DrawRectangle(vb, null, new Rect(new Point(), new Size((int)icon.Width, (int)icon.Height)));
            }
            rtb.Render(dv);
            BitmapFrame bmf = BitmapFrame.Create(rtb);
            bmf.Freeze();
            //Icon200.Fill = new ImageBrush(bmf) ;
            //string filePath = System.IO.Path.GetDirectoryName(this.fileName);
            string fileOut = filePath + "\\" + name;
            FileStream stream = new FileStream(fileOut, FileMode.Create);

            // PNGにエンコード
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(bmf);
            encoder.Save(stream);
            stream.Close();
        }

        #endregion SaveIcons

        #region DragAndDrop

        private string fileName;
        BitmapImage imageSource;
        double scale = 1.0;

        private void myImage_Drop(object sender, DragEventArgs e)
        {
            e.Handled = true;
            string draggedFileName = IsSingleFile(e);

            if (draggedFileName.Contains(".png") || draggedFileName.Contains(".PNG") ||  draggedFileName.Contains(".jpg") || draggedFileName.Contains(".JPG"))
            {
                //MessageBoxResult mbr = MessageBoxResult.OK;
                //if (fileName != null)
                //    mbr = System.Windows.MessageBox.Show("表示されている画像と入れ替えますか？", "relace ?", MessageBoxButton.OKCancel);

                //if (mbr == MessageBoxResult.OK)
                //{
                ShowImage(draggedFileName);
                //}
                IsPasted = false;
            }
            else
            {
                System.Windows.MessageBox.Show(notice4, "Please drag png/jpg file", MessageBoxButton.OK);
            }

        }

        private void grayImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            NewImageDisplayed();
        }
        private void NewImageDisplayed()
        {
            scale = imageSource.Width / grayImage.ActualWidth;
            string name = System.IO.Path.GetFileNameWithoutExtension(fileName);
            projectName.Text = name;
            name620_300.Visibility = Visibility.Hidden;
            name310_150.Visibility = Visibility.Hidden;
            name150.Visibility = Visibility.Hidden;
            name50.Visibility = Visibility.Hidden;
            name30.Visibility = Visibility.Hidden;
            CompleteNotice.Content = notice1;
            Icon620_300.Fill = null;
            Icon310_150.Fill = null;
            Icon150.Fill = null;
            Icon50.Fill = null; 
            Icon30.Fill = null;
            myCanvas.Children.Remove(rectFrame);
        }

        private string IsSingleFile(DragEventArgs args)
        {
            // データオブジェクト内のファイルをチェック
            if (args.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                string[] fileNames = args.Data.GetData(DataFormats.FileDrop, true) as string[];
                // 単一ファイル/フォルダをチェック
                if (fileNames.Length == 1)
                {
                    // ファイルをチェック（ディレクトリはfalseを返す）
                    if (File.Exists(fileNames[0]))
                    {
                        // この時点で単一ファイルであることが分かる
                        return fileNames[0];
                    }
                }
            }
            return null;
        }

        private void myImage_PreviewDragOver(object sender, DragEventArgs args)
        {
            // 単一ファイルだけを処理したい
            if (IsSingleFile(args) != null) args.Effects = DragDropEffects.Copy;
            else args.Effects = DragDropEffects.None;

            // イベントをHandledに、するとDragOverハンドラがキャンセルされる
            args.Handled = true;
        }
        #endregion DragAndDrop

        #region MouseMove
        private Point p1 = new Point();
        private Point p2 = new Point();


        private void myCanvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CompleteNotice.Content = notice2;
        }
        /// <summary>
        /// マウス左ボタン押下用のコールバック
        /// </summary>
        private void UC_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            p1 = e.GetPosition(grayImage);
            CompleteNotice.Content = notice1;
        }
        private Rectangle rectFrame = new Rectangle();

        /// <summary>
        /// マウス移動用のコールバック
        /// </summary>
        private void UC_MouseMove(object sender, MouseEventArgs e)
        {
            p2 = e.GetPosition(grayImage);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                double w = Math.Abs(p1.X - p2.X);
                double h = Math.Abs(p1.Y - p2.Y);
                if (w > h) 
                    p2.Y = (p2.Y > p1.Y) ? p1.Y + w : p1.Y - w;
                else 
                    p2.X = (p2.X > p1.X) ? p1.X + h : p1.X - w;

                Point lt = new Point((p1.X > p2.X) ? p2.X : p1.X, (p1.Y > p2.Y) ? p2.Y : p1.Y);
                Point rb = new Point((p1.X > p2.X) ? p1.X : p2.X, (p1.Y > p2.Y) ? p1.Y : p2.Y);
                
                // rabber band
                myCanvas.Children.Remove(rectFrame);
                rectFrame.Stroke = Brushes.Gray;
                rectFrame.StrokeThickness = 1.0;
                rectFrame.Width = rb.X - lt.X;
                rectFrame.Height = rb.Y - lt.Y;
                rectFrame.RenderTransform = new TranslateTransform(lt.X, lt.Y);
                myCanvas.Children.Add(rectFrame);

                // Icon Images
                Point sourceLt = new Point(lt.X * scale, lt.Y * scale);
                Point sourceRb = new Point(rb.X * scale, rb.Y * scale);

                ImageBrush brush = new ImageBrush();
                brush.ImageSource = imageSource;
                brush.Viewbox = new Rect(sourceLt, sourceRb);
                brush.ViewboxUnits = BrushMappingMode.Absolute;
                brush.Stretch = Stretch.Fill;

                var br = new SolidColorBrush(Colors.Transparent);
                Canvas620_300.Background = br;
                Canvas310_150.Background = br;
                Icon620_300.Fill = brush;
                Icon310_150.Fill = brush;
                Icon150.Fill = brush;
                Icon50.Fill = brush;
                Icon30.Fill = brush;
                DisplayIconNames();
            }
        }

        #endregion MouseMove

    }
}
