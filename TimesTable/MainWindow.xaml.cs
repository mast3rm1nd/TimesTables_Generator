using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
//using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
//using static System.Drawing.Point;

namespace TimesTable
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static int width = 2000;
        static int height = 2000;

        static float circleRadius = 900f;
        static float pointsRadius = 2f;

        static System.Drawing.Point center = new System.Drawing.Point(width / 2, height / 2);

        static int pointsCount = 200;
        static int multiplier = 51;
        static System.Drawing.Point[] points = new System.Drawing.Point[pointsCount];

        static Bitmap image = new Bitmap(width, height);

        public MainWindow()
        {
            InitializeComponent();
        }

        void DrawMainCircle()
        {
            using (Graphics g = Graphics.FromImage(image))
            {
                g.Clear(Color.White);
                //g.DrawLine(new Pen(Color.Red), new System.Drawing.Point(1, 1), new System.Drawing.Point(50, 50));
                var circlePen = new Pen(Brushes.Black);
                var circleRectangle = new RectangleF(
                    new PointF(width / 2 - circleRadius, height / 2 - circleRadius),
                    new SizeF(circleRadius * 2, circleRadius * 2));

                g.DrawEllipse(circlePen, circleRectangle);
            }
        }

        void DrawPoints()
        {
            points = new System.Drawing.Point[pointsCount];

            points[0] = new System.Drawing.Point(
                (int)(center.X - circleRadius),
                center.Y);

            for(int pointIdx = 1; pointIdx < pointsCount; pointIdx++)
            {
                //points[pointIdx] = RotatePoint(points[pointIdx - 1], center, 360.0 / pointsCount);
                points[pointIdx] = RotatePoint(points[0], center, (360.0 / pointsCount) * pointIdx);
            }

            if (!(bool)IsDrawPoints_checkBox.IsChecked) return;

            foreach(var point in points)
                DrawSinglePoint(point);

            //DrawSinglePoint(points[0]);
        }

        void NumeratePoints()
        {
            var textCoords = new System.Drawing.Point[pointsCount];
            textCoords[0] = new System.Drawing.Point(points[0].X - 28, points[0].Y);
            //textCoords[0] = new System.Drawing.Point(points[0].X - 12, points[0].Y - 12);
            //textCoords[0] = points[0];

            for (int labelIdx = 1; labelIdx < textCoords.Length; labelIdx++)
                textCoords[labelIdx] = RotatePoint(textCoords[0], center, (360.0 / pointsCount) * labelIdx);
            //textCoords[labelIdx] = RotatePoint(textCoords[labelIdx - 1], center, 360.0 / pointsCount);

            using (Graphics g = Graphics.FromImage(image))
            {
                for (int pointIdx = 0; pointIdx < points.Length; pointIdx++)
                {
                    Brush textBrush = Brushes.Black;
                    Font textFont = new Font("Arial", 12);

                    var text = pointIdx.ToString();

                    SizeF size = g.MeasureString(text, textFont);

                    PointF drawPoint = new PointF(
                        -size.Width / 2f + textCoords[pointIdx].X,
                        -size.Height / 2f + textCoords[pointIdx].Y);

                    //g.DrawString(text, textFont, textBrush, textCoords[pointIdx]);
                    g.DrawString(text, textFont, textBrush, drawPoint);
                }
            }
        }

        void DrawTimesTable()
        {
            using (Graphics g = Graphics.FromImage(image))
            {
                for(int pointIdx = 0; pointIdx < pointsCount; pointIdx++)
                {
                    var pen = new Pen(Color.Black);

                    var distanationPoint = points[pointIdx * multiplier % pointsCount];

                    g.DrawLine(pen, points[pointIdx], distanationPoint);
                }
            }
        }

        void DrawSinglePoint(System.Drawing.Point atCoord)
        {
            using (Graphics g = Graphics.FromImage(image))
            {
                //g.DrawLine(new Pen(Color.Red), new System.Drawing.Point(1, 1), new System.Drawing.Point(50, 50));
                SolidBrush pointBrush = new SolidBrush(Color.Red);
                var pointRectangle = new RectangleF(
                    new PointF(atCoord.X - pointsRadius, atCoord.Y - pointsRadius),
                    new SizeF(pointsRadius * 2, pointsRadius * 2));

                //g.DrawEllipse(pointPen, pointRectangle);
                g.FillEllipse(pointBrush, pointRectangle);
                //g.FillEllipse()
            }
        }

        /// <summary>
        /// Rotates one point around another
        /// </summary>
        /// <param name="pointToRotate">The point to rotate.</param>
        /// <param name="centerPoint">The center point of rotation.</param>
        /// <param name="angleInDegrees">The rotation angle in degrees.</param>
        /// <returns>Rotated point</returns>
        static System.Drawing.Point RotatePoint(System.Drawing.Point pointToRotate, System.Drawing.Point centerPoint, double angleInDegrees)
        {
            double angleInRadians = angleInDegrees * (Math.PI / 180);
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            return new System.Drawing.Point
            {
                X =
                    (int)
                    (cosTheta * (pointToRotate.X - centerPoint.X) -
                    sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X),
                Y =
                    (int)
                    (sinTheta * (pointToRotate.X - centerPoint.X) +
                    cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y)
            };
        }

        private void IsDrawCircle_checkBox_Click(object sender, RoutedEventArgs e)
        {
            //CircleRadius_TextBox.IsEnabled = !CircleRadius_TextBox.IsEnabled;
        }

        private void IsDrawPoints_checkBox_Click(object sender, RoutedEventArgs e)
        {
            PointsRadius_TextBox.IsEnabled = !PointsRadius_TextBox.IsEnabled;
        }

        private void Generate_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var size = int.Parse(ImageSizeN_TextBox.Text);
                width = size;
                height = size;

                center = new System.Drawing.Point(width / 2, height / 2);

                ClearImage();

                circleRadius = float.Parse(CircleRadius_TextBox.Text);

                if ((bool)IsDrawCircle_checkBox.IsChecked)
                    DrawMainCircle();


                pointsCount = int.Parse(PointsCount_TextBox.Text);

                if ((bool)IsDrawPoints_checkBox.IsChecked)
                    pointsRadius = float.Parse(PointsRadius_TextBox.Text);


                DrawPoints();

                if ((bool)IsLabelPoints_checkBox.IsChecked)
                    NumeratePoints();


                multiplier = int.Parse(Multiplier_TextBox.Text);

                DrawTimesTable();

                //imageControl.SnapsToDevicePixels
                imageControl.Source = BitmapToImageSource(image);
            }
            catch
            {
                MessageBox.Show("Something went wrong. Check input parameters.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            Save_button.IsEnabled = true;
        }

        private void ClearImage()
        {
            image = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(image))
                g.Clear(Color.White);
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private void Save_button_Click(object sender, RoutedEventArgs e)
        {
            var filename = $"{width}x{height}, {pointsCount} points, multiplier = {multiplier}, radius = {circleRadius}.png";

            image.Save(filename, ImageFormat.Png);

            MessageBox.Show($"Saved to \"{filename}\"", "Done!", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
