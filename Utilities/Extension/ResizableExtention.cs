using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Utilities.Extension
{
    [Flags]
    public enum ResizeWay
    {
        None = 0x0,
        Left = 0x1,
        Right = 0x2,
        Top = 0x4,
        Bottom = 0x8,
        LeftAndRight=Left|Right,
        TopAndBottom=Top|Bottom,
        All = 0xF
    }

    public static class ResizableExtension
    {
        private const double THRESHOLD = 10.0;
        private const double MINMUM = 12.0;

        public static DependencyProperty MinHeightProperty = DependencyProperty.RegisterAttached("MinHeight", typeof(double), typeof(ResizableExtension), new PropertyMetadata(MINMUM));
        public static double GetMinHeight(DependencyObject d)
        {
            return (double)d.GetValue(MinHeightProperty);
        }

        public static void SetMinHeight(DependencyObject d, double value)
        {
            d.SetValue(MinHeightProperty, value);
        }

        public static DependencyProperty MinWidthProperty = DependencyProperty.RegisterAttached("MinWidth", typeof(double), typeof(ResizableExtension), new PropertyMetadata(MINMUM));
        public static double GetMinWidth(DependencyObject d)
        {
            return (double)d.GetValue(MinHeightProperty);
        }

        public static void SetMinWidth(DependencyObject d, double value)
        {
            d.SetValue(MinHeightProperty, value);
        }

        public static DependencyProperty ResizeWayProperty = DependencyProperty.RegisterAttached("ResizeWay", typeof(ResizeWay), typeof(ResizableExtension), new PropertyMetadata(ResizeWay.None, ResizeWayChanged));
        public static ResizeWay GetResizeWay(DependencyObject d)
        {
            return (ResizeWay)d.GetValue(ResizeWayProperty);
        }
        public static void SetResizeWay(DependencyObject d, ResizeWay value)
        {
            d.SetValue(ResizeWayProperty, value);
        }

        public static DependencyProperty ResizableWayProperty = DependencyProperty.RegisterAttached("ResizableWay", typeof(ResizeWay), typeof(ResizableExtension));
        public static DependencyProperty CapturedProperty = DependencyProperty.RegisterAttached("Captured", typeof(bool), typeof(ResizableExtension));
        public static DependencyProperty LastPositionProperty = DependencyProperty.RegisterAttached("LastPosition", typeof(Point), typeof(ResizableExtension));

        private static void ResizeWayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement self = (FrameworkElement)d;

            if ((ResizeWay)e.NewValue != ResizeWay.None)
            {
                self.Loaded += self_Loaded;
                self.PreviewMouseMove += self_MouseMove;
                self.PreviewMouseDown += self_MouseDown;
                self.PreviewMouseUp += self_MouseUp;
                self.LostMouseCapture += self_LostMouseCapture;
            }
            else
            {
                self.Loaded -= self_Loaded;
                self.PreviewMouseMove -= self_MouseMove;
                self.PreviewMouseDown -= self_MouseDown;
                self.PreviewMouseUp -= self_MouseUp;
                self.LostMouseCapture -= self_LostMouseCapture;
            }
        }

        private static void self_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement self = (FrameworkElement)sender;

            //self.Height = self.ActualHeight;
        }

        private static void self_LostMouseCapture(object sender, MouseEventArgs e)
        {
            FrameworkElement self = (FrameworkElement)sender;

            bool captured = (bool)self.GetValue(CapturedProperty);
            if (captured)
            {
                self.CaptureMouse();
            }
        }

        private static void self_MouseUp(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement self = (FrameworkElement)sender;

            bool captured = (bool)self.GetValue(CapturedProperty);
            if (captured)
            {
                self.SetValue(CapturedProperty, false);
                self.ReleaseMouseCapture();
                e.Handled = true;
            }
        }

        private static void self_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement self = (FrameworkElement)sender;

            ResizeWay resizable = (ResizeWay)self.GetValue(ResizableWayProperty);
            if (resizable == ResizeWay.None)
            {
                return;
            }

            bool captured = self.CaptureMouse();
            if (captured)
            {
                Point lastPosition = self.PointToScreen(e.GetPosition(self));
                self.SetValue(CapturedProperty, true);
                self.SetValue(LastPositionProperty, lastPosition);
                e.Handled = true;
            }
        }

        private static void self_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                FrameworkElement self = (FrameworkElement)sender;
                double minHeight = (double)self.GetValue(MinHeightProperty);
                double minWidth = (double)self.GetValue(MinWidthProperty);
                //Debug.WriteLine(string.Format("self_MouseMove minHeight:{0}; minWidth:{1}", minHeight, minWidth));

                bool captured = (bool)self.GetValue(CapturedProperty);
                if (captured)
                {
                    var position = self.PointToScreen(e.GetPosition(self));
                    ResizeWay resizableWay = (ResizeWay)self.GetValue(ResizableWayProperty);
                    Point lastPosition = (Point)self.GetValue(LastPositionProperty);
                    var offsetX = position.X - lastPosition.X;
                    if ((resizableWay & ResizeWay.Left) == ResizeWay.Left)
                    {
                        offsetX = Math.Min(offsetX, self.ActualWidth - minWidth);
                        self.Width -= offsetX;
                        Window window = self as Window;
                        if (window != null)
                        {
                            window.Left += offsetX;
                        }
                    }
                    else if ((resizableWay & ResizeWay.Right) == ResizeWay.Right)
                    {
                        offsetX = Math.Max(offsetX, minWidth - self.ActualWidth);
                        self.Width += offsetX;
                    }
                    var offsetY = position.Y - lastPosition.Y;
                    if ((resizableWay & ResizeWay.Top) == ResizeWay.Top)
                    {
                        offsetY = Math.Min(offsetY, self.ActualHeight - minHeight);
                        self.Height -= offsetY;
                        Window window = self as Window;
                        if (window != null)
                        {
                            window.Top += offsetY;
                        }
                    }
                    else if ((resizableWay & ResizeWay.Bottom) == ResizeWay.Bottom)
                    {
                        offsetY = Math.Max(offsetY, minHeight - self.ActualHeight);
                        self.Height += offsetY;
                    }

                    lastPosition = position;
                    self.SetValue(LastPositionProperty, lastPosition);
                    e.Handled = true;
                }
                else
                {
                    ResizeWay resizeWay = GetResizeWay(self);
                    ResizeWay resizableWay = ResizeWay.None;
                    var position = e.GetPosition(self);
                    if (position.Y < THRESHOLD)
                    {
                        resizableWay |= ResizeWay.Top;
                    }
                    else if (position.Y > self.ActualHeight - THRESHOLD && position.Y < self.ActualHeight - 1)
                    {
                        resizableWay |= ResizeWay.Bottom;
                    }

                    if (position.X < THRESHOLD)
                    {
                        resizableWay |= ResizeWay.Left;
                    }
                    else if (position.X > self.ActualWidth - THRESHOLD && position.X < self.ActualWidth - 1)
                    {
                        resizableWay |= ResizeWay.Right;
                    }
                    resizableWay &= resizeWay;
                    self.SetValue(ResizableWayProperty, resizableWay);

                    if (resizableWay != ResizeWay.None)
                    {
                        if ((resizableWay & ResizeWay.Left) == ResizeWay.Left)
                        {
                            if ((resizableWay & ResizeWay.Top) == ResizeWay.Top)
                            {
                                self.Cursor = Cursors.SizeNWSE;
                            }
                            else if ((resizableWay & ResizeWay.Bottom) == ResizeWay.Bottom)
                            {
                                self.Cursor = Cursors.SizeNESW;
                            }
                            else
                            {
                                self.Cursor = Cursors.SizeWE;
                            }
                        }
                        else if ((resizableWay & ResizeWay.Right) == ResizeWay.Right)
                        {
                            if ((resizableWay & ResizeWay.Top) == ResizeWay.Top)
                            {
                                self.Cursor = Cursors.SizeNESW;
                            }
                            else if ((resizableWay & ResizeWay.Bottom) == ResizeWay.Bottom)
                            {
                                self.Cursor = Cursors.SizeNWSE;
                            }
                            else
                            {
                                self.Cursor = Cursors.SizeWE;
                            }
                        }
                        else if ((resizableWay & ResizeWay.Top) == ResizeWay.Top)
                        {
                            self.Cursor = Cursors.SizeNS;
                        }
                        else if ((resizableWay & ResizeWay.Bottom) == ResizeWay.Bottom)
                        {
                            self.Cursor = Cursors.SizeNS;
                        }
                        e.Handled = true;
                    }
                    else
                    {
                        self.Cursor = Cursors.Arrow;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("self_MouseMove exception:" + ex.Message);
            }
        }
    }
}
