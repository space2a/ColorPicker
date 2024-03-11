using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace space2a
{
    public partial class ColorPicker : UserControl
    {
        public event ColorUpdate OnColorUpdate;
        public delegate void ColorUpdate(Color color);

        public ColorPicker()
        {
            InitializeComponent();

            DataContext = this;

            this.Loaded += ColorPicker_Loaded;
        }

        private void ColorPicker_Loaded(object sender, RoutedEventArgs e)
        {
            SetSelectorsAccordingToColor();
        }

        public SolidColorBrush CurrentColorBrush
        {
            get { return new SolidColorBrush(CurrentColor); }
            set { SetValue(CurrentColorBrushProperty, value); }
        }

        private static readonly DependencyProperty CurrentColorBrushProperty =
            DependencyProperty.Register("CurrentColorBrush", typeof(SolidColorBrush), typeof(UserControl), new PropertyMetadata(new SolidColorBrush()));

        public Color CurrentColorNoTransparency
        {
            get { return (Color)GetValue(CurrentColorNoTransparencyProperty); }
            set { SetValue(CurrentColorNoTransparencyProperty, value); }
        }

        private static readonly DependencyProperty CurrentColorNoTransparencyProperty =
            DependencyProperty.Register("CurrentColorNoTransparency", typeof(Color), typeof(ColorPicker), new PropertyMetadata(Color.FromArgb(255, 255, 255, 255)));

        public Color CurrentColor
        {
            get { return (Color)GetValue(CurrentColorProperty); }
            private set { SetValue(CurrentColorProperty, value); CurrentColorNoTransparency = Color.FromArgb(255, value.R, value.G, value.B); OnColorUpdate?.Invoke(value); }
        }

        private static readonly DependencyProperty CurrentColorProperty =
            DependencyProperty.Register("CurrentColor", typeof(Color), typeof(ColorPicker), new PropertyMetadata(Color.FromArgb(255, 255, 255, 255)));

        public Color CurrentColorHue
        {
            get { return (Color)GetValue(CurrentColorHueProperty); }
            set { SetValue(CurrentColorHueProperty, value); }
        }

        private static readonly DependencyProperty CurrentColorHueProperty =
            DependencyProperty.Register("CurrentColorHue", typeof(Color), typeof(ColorPicker), new PropertyMetadata(Color.FromArgb(255, 255, 0, 0)));

        #region SelectorsMovements

        private bool anySelectorEllipseMoving = false;
        private Point selectorEllipseOriginalPosition;
        private void SelectorEllipse_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            anySelectorEllipseMoving = true;
            selectorEllipseOriginalPosition = e.GetPosition(HueSelector);

            (sender as FrameworkElement).CaptureMouse();
        }

        private void SelectorEllipse_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            anySelectorEllipseMoving = false;
            (sender as FrameworkElement).ReleaseMouseCapture();
        }

        private void MoveSelectorX(FrameworkElement element, FrameworkElement parent, System.Windows.Input.MouseEventArgs e)
        {
            Point currentPosition;
            currentPosition = e.GetPosition(parent);
            double offsetX = currentPosition.X - selectorEllipseOriginalPosition.X;

            Canvas.SetLeft(element, Canvas.GetLeft(element) + offsetX);

            if (Canvas.GetLeft(element) < 0)
            {
                Canvas.SetLeft(element, 0);
                currentPosition.X = 0;
            }
            else if (Canvas.GetLeft(element) > parent.ActualWidth - element.ActualWidth)
            {
                Canvas.SetLeft(element, parent.ActualWidth - element.ActualWidth);
                currentPosition.X = parent.ActualWidth;
            }

            selectorEllipseOriginalPosition = currentPosition;
        }

        private void MoveSelectorXY(FrameworkElement element, FrameworkElement parent, System.Windows.Input.MouseEventArgs e)
        {
            Point currentPosition;
            currentPosition = e.GetPosition(parent);
            double offsetX = currentPosition.X - selectorEllipseOriginalPosition.X;
            double offsetY = currentPosition.Y - selectorEllipseOriginalPosition.Y;

            Canvas.SetLeft(element, Canvas.GetLeft(element) + offsetX);
            Canvas.SetTop(element, Canvas.GetTop(element) + offsetY);

            if (Canvas.GetLeft(element) < 0)
            {
                Canvas.SetLeft(element, 0);
                currentPosition.X = 0;
            }
            else if (Canvas.GetLeft(element) > parent.ActualWidth - (element.ActualWidth - 3))
            {
                Canvas.SetLeft(element, parent.ActualWidth - (element.ActualWidth - 3));
                currentPosition.X = parent.ActualWidth;
            }

            if (Canvas.GetTop(element) < 0)
            {
                Canvas.SetTop(element, 0);
                currentPosition.Y = 0;
            }
            else if (Canvas.GetTop(element) > parent.ActualHeight - (element.ActualHeight - 3))
            {
                Canvas.SetTop(element, parent.ActualHeight - (element.ActualHeight - 3));
                currentPosition.Y = parent.ActualHeight;
            }

            selectorEllipseOriginalPosition = currentPosition;
        }

        private void HueSelectorEllipse_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (anySelectorEllipseMoving)
            {
                MoveSelectorX(element, HueSelector, e);
                CurrentColorHue = ColorHelper.ColorFromHue((selectorEllipseOriginalPosition.X / HueSelector.ActualWidth) * 360);
                CalculateColor();
            }
        }

        private void TransparencySelectorEllipse_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (anySelectorEllipseMoving)
            {
                MoveSelectorX(element, TransparencySelector, e);
                CalculateColor();
            }
        }

        private void ColorSelectorEllipse_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (anySelectorEllipseMoving)
            {
                MoveSelectorXY((sender as FrameworkElement), ColorSelector, e);
                CalculateColor();
            }
        }

        private void JumpPick(FrameworkElement element, FrameworkElement parent, System.Windows.Input.MouseButtonEventArgs e, bool setTop = true)
        {
            var position = e.GetPosition(parent);
            Canvas.SetLeft(element, position.X);
            if(setTop)
                Canvas.SetTop(element, position.Y);

            anySelectorEllipseMoving = true;
            selectorEllipseOriginalPosition = position;
            element.CaptureMouse();
        }

        private void ColorSelector_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            JumpPick(ColorSelectorEllipse, (sender as FrameworkElement), e);
        }

        private void HueSelector_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            JumpPick(HueSelectorEllipse, (sender as FrameworkElement), e, false);
        }

        private void TransparencySelector_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            JumpPick(TransparencySelectorEllipse, (sender as FrameworkElement), e, false);
        }

        #endregion SelectorsMovements


        private void CalculateColor()
        {
            byte transparency = (byte)((Canvas.GetLeft(TransparencySelectorEllipse) / TransparencySelector.ActualWidth) * 255);

            double hue = ColorHelper.GetHue(CurrentColorHue);
            double saturation = (Canvas.GetLeft(ColorSelectorEllipse) / (ColorSelector.ActualWidth - 12)) * 100;
            double brightness = (-(Canvas.GetTop(ColorSelectorEllipse) - (ColorSelector.ActualHeight - 12)) / (ColorSelector.ActualHeight - 12)) * 100;

            CurrentColor = ColorHelper.HsbToColor(hue, saturation, brightness, transparency);
        }

        public void SetColor(Color color)
        {
            CurrentColor = color;
            SetSelectorsAccordingToColor();
        }

        private void SetSelectorsAccordingToColor()
        {
            ColorHSB colorHSB = ColorHelper.ColorToHsb(CurrentColor);
            
            CurrentColorHue = ColorHelper.ColorFromHue(colorHSB.Hue);

            //Transparency selector
            Canvas.SetLeft(TransparencySelectorEllipse, (CurrentColor.A / 255.0f) * (TransparencySelector.ActualWidth - 12));

            //Hue selector
            Canvas.SetLeft(HueSelectorEllipse, (colorHSB.Hue / 360.0f) * (HueSelector.ActualWidth - 12));

            //Color selector
            Canvas.SetLeft(ColorSelectorEllipse, (colorHSB.Saturation / 100.0f) * (ColorSelector.ActualWidth - 12));
            Console.WriteLine((colorHSB.Brightness / 100.0f));
            Canvas.SetTop(ColorSelectorEllipse, (1 - (colorHSB.Brightness / 100.0f) )* (ColorSelector.ActualHeight - 12));

        }
    }

    public static class ColorHelper
    {
        public static double GetHue(Color color)
        {
            double min = Math.Min(Math.Min(color.R, color.G), color.B);
            double max = Math.Max(Math.Max(color.R, color.G), color.B);

            double hue;

            if (max == color.R)
            {
                hue = (color.G - color.B) / (max - min);
            }
            else if (max == color.G)
            {
                hue = 2 + (color.B - color.R) / (max - min);
            }
            else
            {
                hue = 4 + (color.R - color.G) / (max - min);
            }

            hue *= 60;
            if (hue < 0)
            {
                hue += 360;
            }

            return hue;
        }

        public static Color ColorFromHue(double hue)
        {
            hue = hue % 360;
            if (hue < 0)
                hue += 360;

            double saturation = 1.0;
            double lightness = 0.5;

            double chroma = (1 - Math.Abs(2 * lightness - 1)) * saturation;
            double huePrime = hue / 60;
            double x = chroma * (1 - Math.Abs(huePrime % 2 - 1));

            double r1, g1, b1;
            if (huePrime >= 0 && huePrime < 1)
            {
                r1 = chroma;
                g1 = x;
                b1 = 0;
            }
            else if (huePrime >= 1 && huePrime < 2)
            {
                r1 = x;
                g1 = chroma;
                b1 = 0;
            }
            else if (huePrime >= 2 && huePrime < 3)
            {
                r1 = 0;
                g1 = chroma;
                b1 = x;
            }
            else if (huePrime >= 3 && huePrime < 4)
            {
                r1 = 0;
                g1 = x;
                b1 = chroma;
            }
            else if (huePrime >= 4 && huePrime < 5)
            {
                r1 = x;
                g1 = 0;
                b1 = chroma;
            }
            else
            {
                r1 = chroma;
                g1 = 0;
                b1 = x;
            }

            double m = lightness - chroma / 2;
            byte r = Convert.ToByte((r1 + m) * 255);
            byte g = Convert.ToByte((g1 + m) * 255);
            byte b = Convert.ToByte((b1 + m) * 255);
            return Color.FromRgb(r, g, b);
        }

        public static Color HsbToColor(ColorHSB colorHSB)
        {
            return HsbToColor(colorHSB.Hue, colorHSB.Saturation, colorHSB.Brightness);
        }

        public static Color HsbToColor(double hue, double saturation, double brightness, byte transparency = 255)
        {
            brightness /= 100;
            saturation /= 100;
            double chroma = brightness * saturation;
            double hue1 = hue / 60;
            double x = chroma * (1 - Math.Abs(hue1 % 2 - 1));
            double m = brightness - chroma;

            double red = 0, green = 0, blue = 0;

            if (0 <= hue1 && hue1 < 1)
            {
                red = chroma;
                green = x;
                blue = 0;
            }
            else if (1 <= hue1 && hue1 < 2)
            {
                red = x;
                green = chroma;
                blue = 0;
            }
            else if (2 <= hue1 && hue1 < 3)
            {
                red = 0;
                green = chroma;
                blue = x;
            }
            else if (3 <= hue1 && hue1 < 4)
            {
                red = 0;
                green = x;
                blue = chroma;
            }
            else if (4 <= hue1 && hue1 < 5)
            {
                red = x;
                green = 0;
                blue = chroma;
            }
            else if (5 <= hue1 && hue1 < 6)
            {
                red = chroma;
                green = 0;
                blue = x;
            }

            red += m;
            green += m;
            blue += m;

            return Color.FromArgb(transparency, (byte)(red * 255), (byte)(green * 255), (byte)(blue * 255));
        }

        public static ColorHSB ColorToHsb(Color color)
        {
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            double delta = max - min;

            double brightness, saturation, hue;

            brightness = max;

            saturation = (max == 0) ? 0 : delta / max;

            if (delta == 0)
            {
                hue = 0;
            }
            else if (max == r)
            {
                hue = 60 * ((g - b) / delta % 6);
            }
            else if (max == g)
            {
                hue = 60 * ((b - r) / delta + 2);
            }
            else // max == b
            {
                hue = 60 * ((r - g) / delta + 4);
            }

            if (hue < 0)
            {
                hue += 360;
            }

            return new ColorHSB(hue, saturation * 100, brightness * 100);
        }


    }

    public struct ColorHSB
    {
        public double Hue;
        public double Saturation;
        public double Brightness;

        public ColorHSB(double hue, double saturation, double brightness)
        {
            Hue = hue;
            Saturation = saturation;
            Brightness = brightness;
        }

        public Color ToColor() { return ColorHelper.HsbToColor(this); }
    }
}
