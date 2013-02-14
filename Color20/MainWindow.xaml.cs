using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Threading;
using MColor = System.Windows.Media.Color;
using DColor = System.Drawing.Color;

namespace Color20
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private Variables

        private System.Drawing.Point _point;
        private System.Drawing.Color _color;
        private DispatcherTimer _timer = new DispatcherTimer();

        #endregion

        public MainWindow()
        {
            InitializeComponent();            
            _timer.IsEnabled = false;

            _timer.Interval = TimeSpan.FromMilliseconds(10);
            _timer.Tick += new EventHandler(_timer_Tick);
        } 

        void _timer_Tick(object sender, EventArgs e)
        {
            GetColorFromCursor();
        }

        private MColor CurrentColor
        {
            set
            {
                currentColorBox.Background = new SolidColorBrush(value);
                currentHexBox.Text = String.Format("#{0}", value.ToString().Substring(3));
                currentRBox.Text = value.R.ToString();
                currentGBox.Text = value.G.ToString();
                currentBBox.Text = value.B.ToString();
            }
        }

        private void GetColorFromCursor()
        {
            var width = Convert.ToInt32(SystemParameters.PrimaryScreenWidth);
            var height = Convert.ToInt32(SystemParameters.PrimaryScreenHeight);
            _point = new System.Drawing.Point(0, 0);

            using (System.Drawing.Bitmap _bmp = new System.Drawing.Bitmap(width, height))
            {
                using (var _graphic = System.Drawing.Graphics.FromImage(_bmp))
                {
                    _graphic.CopyFromScreen(_point, _point, _bmp.Size);
                }

                var p = Win32.Win32Utils.CurrentMousePosition;
                if (p.X < 0 || p.Y < 0)
                    return;

                var cursorX = Convert.ToInt32(p.X);
                var cursorY = Convert.ToInt32(p.Y);
                _color = _bmp.GetPixel(cursorX, cursorY);
            }
            UpdateColor(_color);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            GetColorFromCursor();
        }

        #region Methods

        #region Update Methods

        private void UpdateColorFromHex(string hex)
        {
            byte r, g, b;
            if (hex.StartsWith("#"))
                hex = hex.Substring(1);
            r = (byte)int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            g = (byte)int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            b = (byte)int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);

            CurrentColor = MColor.FromRgb(r, g, b);
        }

        private void UpdateColor(DColor color)
        {
            CurrentColor = MColor.FromArgb(color.A, color.R, color.G, color.B);
        }

        private void UpdateColor()
        {
            if (currentBBox == null || currentRBox == null || currentGBox == null)
                return;
            var r = Convert.ToByte(currentRBox.Text);
            var g = Convert.ToByte(currentGBox.Text);
            var b = Convert.ToByte(currentBBox.Text);

            CurrentColor = MColor.FromRgb(r, g, b);
        }

        private static string ParseValue(string value)
        {
            int val;
            int.TryParse(value, out val);
            if (val >= 255)
                val = 255;
            if (val <= 0)
                val = 0;
            return val.ToString();
        }

        private static void ParseKey(Key key, TextBox tb)
        {
            if (key != Key.Up && key != Key.Down)
                return;

            int b;
            int.TryParse(tb.Text, out b);

            switch (key)
            {
                case Key.Up:
                    {
                        b++;
                        tb.Text = ParseValue(b.ToString());
                        break;
                    }
                case Key.Down:
                    {
                        b--;
                        tb.Text = ParseValue(b.ToString());
                        break;
                    }
            }
        }

        #endregion

        #endregion

        #region Event Handlers

        private void currentBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            ParseKey(e.Key, sender as TextBox);
        }

        private void currentHexBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var hex = (sender as TextBox).Text;
                if (hex.StartsWith("#"))
                    hex = hex.Substring(1);

                UpdateColorFromHex(hex);
            }
        }

        private void alwaysOnTopCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Topmost = true;
        }

        private void alwaysOnTopCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Topmost = false;
        }
        
        private void monitorMouseCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            button1.IsEnabled = false;
            _timer.IsEnabled = true;
        }

        private void monitorMouseCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            button1.IsEnabled = true;
            _timer.IsEnabled = false;
        }

        #endregion

        private void currentBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = (TextBox) sender;
            tb.Text = ParseValue(tb.Text);
            UpdateColor();
        }
    }
}
