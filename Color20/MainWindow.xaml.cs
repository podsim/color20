using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
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
        private System.Drawing.Bitmap _bmp;
        private System.Drawing.Graphics _graphic;
        private System.Drawing.Color _color;

        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Init()
        {            
            var width = Convert.ToInt32(SystemParameters.PrimaryScreenWidth);
            var height = Convert.ToInt32(SystemParameters.PrimaryScreenHeight);
            _bmp = new System.Drawing.Bitmap(width, height);
            _point = new System.Drawing.Point(0, 0);            

            //timer1.Enabled = false;
            //_monitorCheck = false;
            //notifyIcon1.Visible = false;
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

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Init();
            _graphic = System.Drawing.Graphics.FromImage(_bmp);
            _graphic.CopyFromScreen(_point, _point, _bmp.Size);

            var p = Win32.Win32Utils.CurrentMousePosition;
            if (p.X < 0 || p.Y < 0)
                return;

            var cursorX = Convert.ToInt32(p.X);
            var cursorY = Convert.ToInt32(p.Y);
            _color = _bmp.GetPixel(cursorX, cursorY);
            
            UpdateColor(_color);
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
            var r = Convert.ToByte(currentRBox.Text);
            var g = Convert.ToByte(currentGBox.Text);
            var b = Convert.ToByte(currentBBox.Text);

            CurrentColor = MColor.FromRgb(r, g, b);
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

        #endregion

        private void ParseKey(Key key, TextBox tb)
        {
            if (key != Key.Up && key != Key.Down)
                return;

            int b;
            int.TryParse(tb.Text, out b);

            switch (key)
            {
                case Key.Up:
                    {
                        if (b == 255)
                            return;
                        b++;
                        tb.Text = b.ToString();
                        UpdateColor();
                        break;
                    }
                case Key.Down:
                    {
                        if (b == 0)
                            return;
                        b--;
                        tb.Text = b.ToString();
                        UpdateColor();
                        break;
                    }
            }
        }

    }
}
