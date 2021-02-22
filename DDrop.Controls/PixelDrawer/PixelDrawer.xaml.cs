using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using DDrop.Controls.PixelDrawer.Enums;
using DDrop.Controls.PixelDrawer.Models;
using Brushes = System.Windows.Media.Brushes;

namespace DDrop.Controls.PixelDrawer
{
    /// <summary>
    ///     Interaction logic for PixelDrawer.xaml
    /// </summary>
    public partial class PixelDrawer
    {
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(PixelDrawer));

        public static readonly DependencyProperty DrawningIsEnabledProperty =
            DependencyProperty.Register("DrawningIsEnabled", typeof(bool), typeof(PixelDrawer));

        public static readonly DependencyProperty DrawningModeProperty =
            DependencyProperty.Register("DrawningMode", typeof(PixelDrawerMode), typeof(PixelDrawer));

        public static readonly DependencyProperty DrawnShapesProperty = 
            DependencyProperty.Register("DrawnShapes", typeof(DrawnShapes), typeof(PixelDrawer));

        private Line _selectedLine;
        private Ellipse _selectedEllipse;
        private Rectangle _rect;
        private Point _startPoint;

        public PixelDrawer()
        {
            InitializeComponent();
            DataContext = this;
        }

        public DrawnShapes DrawnShapes
        {
            get => (DrawnShapes) GetValue(DrawnShapesProperty);
            set => SetValue(DrawnShapesProperty, value);
        }

        public PixelDrawerMode DrawningMode
        {
            get => (PixelDrawerMode)GetValue(DrawningModeProperty);
            set => SetValue(DrawningModeProperty, value);
        }

        public bool DrawningIsEnabled
        {
            get => (bool) GetValue(DrawningIsEnabledProperty);
            set => SetValue(DrawningIsEnabledProperty, value);
        }

        public ImageSource ImageSource
        {
            get => (ImageSource) GetValue(ImageSourceProperty);

            set => SetValue(ImageSourceProperty, value);
        }

        private void canDrawing_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DrawningIsEnabled)
            {
                CanDrawing.MouseMove += canDrawing_MouseMove_Drawing;
                CanDrawing.MouseUp += canDrawing_MouseUp_Drawing;
                CanDrawing.MouseLeave += ReferenceImage_NewSegmentDrawingMouseLeave;
                Application.Current.Deactivated += ReferenceImage_NewSegmentDrawingLostFocus;

                switch (DrawningMode)
                {
                    case PixelDrawerMode.Rectangle:
                    {
                        _startPoint = e.GetPosition(CanDrawing);

                        _rect = new Rectangle
                        {
                            Stroke = Brushes.Yellow,
                            StrokeThickness = 1
                        };
                        Canvas.SetLeft(_rect, _startPoint.X);
                        Canvas.SetTop(_rect, _startPoint.Y);
                        CanDrawing.Children.Add(_rect);
                        break;
                    }
                    case PixelDrawerMode.Ellipse:
                    {
                        _selectedEllipse = new Ellipse
                        {
                            Stroke = Brushes.Yellow,
                            Fill = Brushes.Yellow,
                            Width = 1,
                            Height = 1,
                            StrokeThickness = 1,
                        };

                        CanDrawing.Children.Add(_selectedEllipse);

                        Canvas.SetLeft(_selectedEllipse, e.GetPosition(Image).X);
                        Canvas.SetTop(_selectedEllipse, e.GetPosition(Image).Y);
                        break;
                    }
                    case PixelDrawerMode.Line:
                    {
                        _selectedLine = new Line
                        {
                            Stroke = Brushes.Yellow,
                            X1 = e.GetPosition(Image).X,
                            Y1 = e.GetPosition(Image).Y,
                            X2 = e.GetPosition(Image).X,
                            Y2 = e.GetPosition(Image).Y
                        };

                        CanDrawing.Children.Add(_selectedLine);
                        break;
                    }
                }
            }
        }

        #region Drawing

        private void canDrawing_MouseMove_Drawing(object sender, MouseEventArgs e)
        {
            if (DrawningMode == PixelDrawerMode.Rectangle)
            {
                if (e.RightButton == MouseButtonState.Released || _rect == null)
                    return;

                var pos = e.GetPosition(CanDrawing);

                var x = Math.Min(pos.X, _startPoint.X);
                var y = Math.Min(pos.Y, _startPoint.Y);

                var w = Math.Max(pos.X, _startPoint.X) - x;
                var h = Math.Max(pos.Y, _startPoint.Y) - y;

                _rect.Width = w;
                _rect.Height = h;

                Canvas.SetLeft(_rect, x);
                Canvas.SetTop(_rect, y);
            }
            else if (DrawningMode == PixelDrawerMode.Line)
            {
                _selectedLine.X2 = e.GetPosition(Image).X;
                _selectedLine.Y2 = e.GetPosition(Image).Y;
            }
        }

        private void canDrawing_MouseUp_Drawing(object sender, MouseEventArgs e)
        {
            CanDrawing.MouseMove -= canDrawing_MouseMove_Drawing;
            CanDrawing.MouseUp -= canDrawing_MouseUp_Drawing;
            CanDrawing.MouseLeave -= ReferenceImage_NewSegmentDrawingMouseLeave;
            Application.Current.Deactivated -= ReferenceImage_NewSegmentDrawingLostFocus;

            DrawnShapes = new DrawnShapes()
            {
                Line = _selectedLine,
                Rectangle = _rect,
                Ellipse = _selectedEllipse
            };
        }

        #endregion Drawing

        #region Control Leave Handlers

        private void ReferenceImage_NewSegmentDrawingMouseLeave(object sender, MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed) canDrawing_MouseUp_Drawing(sender, e);
        }

        private void ReferenceImage_NewSegmentDrawingLostFocus(object sender, EventArgs e)
        {
            if (Mouse.RightButton == MouseButtonState.Pressed)
                canDrawing_MouseUp_Drawing(sender, new MouseEventArgs(Mouse.PrimaryDevice, 0));
        }

        #endregion
    }
}