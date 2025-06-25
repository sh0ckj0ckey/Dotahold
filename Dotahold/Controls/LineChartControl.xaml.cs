using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dotahold.Models;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace Dotahold.Controls
{
    public sealed partial class LineChartControl : UserControl
    {
        public static readonly DependencyProperty ChartColorProperty = DependencyProperty.Register(nameof(ChartColor), typeof(Color), typeof(LineChartControl), new PropertyMetadata(Colors.Gray, ChartColorChanged));

        public static readonly DependencyProperty SeriesProperty = DependencyProperty.Register(nameof(Series), typeof(List<LineSeries>), typeof(LineChartControl), new PropertyMetadata(null, SeriesChanged));

        public static readonly DependencyProperty ShowNegativeAreaProperty = DependencyProperty.Register(nameof(ShowNegativeArea), typeof(bool), typeof(LineChartControl), new PropertyMetadata(true, ShowNegativeAreaChanged));

        public static readonly DependencyProperty XAxisStepProperty = DependencyProperty.Register(nameof(XAxisStep), typeof(int), typeof(LineChartControl), new PropertyMetadata(0, AxisStepChanged));

        public static readonly DependencyProperty YAxisStepProperty = DependencyProperty.Register(nameof(YAxisStep), typeof(int), typeof(LineChartControl), new PropertyMetadata(0, AxisStepChanged));

        private readonly List<UIElement> _highlightPoints = [];

        public Color ChartColor
        {
            get => (Color)GetValue(ChartColorProperty);
            set => SetValue(ChartColorProperty, value);
        }

        public List<LineSeries> Series
        {
            get => (List<LineSeries>)GetValue(SeriesProperty);
            set => SetValue(SeriesProperty, value);
        }

        public bool ShowNegativeArea
        {
            get => (bool)GetValue(ShowNegativeAreaProperty);
            set => SetValue(ShowNegativeAreaProperty, value);
        }

        public int XAxisStep
        {
            get => (int)GetValue(XAxisStepProperty);
            set => SetValue(XAxisStepProperty, value);
        }

        public int YAxisStep
        {
            get => (int)GetValue(YAxisStepProperty);
            set => SetValue(YAxisStepProperty, value);
        }

        public LineChartControl()
        {
            this.InitializeComponent();
        }

        private static void ChartColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LineChartControl)d).DrawChart();
        }

        private static void SeriesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LineChartControl)d).DrawChart();
        }

        private static void ShowNegativeAreaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LineChartControl)d).DrawChart();
        }

        private static void AxisStepChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LineChartControl)d).DrawChart();
        }

        private void ChartCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawChart();
        }

        private void DrawChart()
        {
            ChartCanvas.Children.Clear();
            _highlightPoints.Clear();
            Tooltip.Visibility = Visibility.Collapsed;

            if (this.Series is null || this.Series.Count <= 0)
            {
                return;
            }

            var strokeColorBrush = new SolidColorBrush(this.ChartColor);

            double width = ChartCanvas.ActualWidth;
            double height = ChartCanvas.ActualHeight;
            if (width <= 0 || height <= 0)
            {
                return;
            }

            // 计算最大值
            int minData = Series.SelectMany(s => s.Data).DefaultIfEmpty(0).Min();
            int maxData = Series.SelectMany(s => s.Data).DefaultIfEmpty(0).Max();
            int maxAbs = Math.Max(Math.Abs(minData), Math.Abs(maxData));

            if (maxAbs == 0)
            {
                maxAbs = 10000;
            }

            maxAbs = ((maxAbs + 9999) / 10000) * 10000;

            int min = this.ShowNegativeArea ? -maxAbs : 0;
            int max = maxAbs;

            // 绘制坐标轴
            double leftMargin = 20, bottomMargin = 20, topMargin = 20, rightMargin = 20;
            double chartWidth = width - leftMargin - rightMargin;
            double chartHeight = height - topMargin - bottomMargin;

            // 绘制和折线图区域一致的圆角矩形边框
            var borderRect = new Rectangle
            {
                Width = chartWidth,
                Height = chartHeight,
                Stroke = strokeColorBrush,
                StrokeThickness = 1,
                RadiusX = 2,
                RadiusY = 2,
                Fill = null,
            };

            Canvas.SetLeft(borderRect, leftMargin);
            Canvas.SetTop(borderRect, topMargin);
            ChartCanvas.Children.Add(borderRect);

            // Y轴
            ChartCanvas.Children.Add(new Line
            {
                X1 = leftMargin,
                Y1 = topMargin,
                X2 = leftMargin,
                Y2 = topMargin + chartHeight,
                Stroke = strokeColorBrush,
                StrokeThickness = 1,
            });

            // X轴
            ChartCanvas.Children.Add(new Line
            {
                X1 = leftMargin,
                Y1 = topMargin + chartHeight,
                X2 = leftMargin + chartWidth,
                Y2 = topMargin + chartHeight,
                Stroke = strokeColorBrush,
                StrokeThickness = 1,
            });

            // Y轴刻度
            if (this.YAxisStep > 0)
            {
                for (int value = min; value <= max; value += this.YAxisStep)
                {
                    double y = topMargin + chartHeight - (value - min) * chartHeight / (max - min);
                    ChartCanvas.Children.Add(new Line
                    {
                        X1 = leftMargin - 5,
                        Y1 = y,
                        X2 = leftMargin,
                        Y2 = y,
                        Stroke = strokeColorBrush,
                        StrokeThickness = 1,
                        Opacity = 0.5,
                    });

                    var label = new TextBlock
                    {
                        Text = value.ToString(),
                        FontSize = 12,
                        Foreground = strokeColorBrush,
                    };
                    Canvas.SetLeft(label, 0);
                    Canvas.SetTop(label, y - 10);
                    ChartCanvas.Children.Add(label);
                }
            }

            // X轴刻度
            if (this.XAxisStep > 0)
            {
                int maxLen = Series.Max(s => s.Data.Length);
                for (int i = 0; i < maxLen; i += this.XAxisStep)
                {
                    double x = leftMargin + i * chartWidth / (maxLen - 1);
                    var tick = new Line
                    {
                        X1 = x,
                        Y1 = topMargin + chartHeight,
                        X2 = x,
                        Y2 = topMargin + chartHeight + 5,
                        Stroke = strokeColorBrush,
                        StrokeThickness = 1,
                        Opacity = 0.5,
                    };
                    ChartCanvas.Children.Add(tick);

                    var label = new TextBlock
                    {
                        Text = i.ToString(),
                        FontSize = 12,
                        Foreground = strokeColorBrush,
                    };
                    Canvas.SetLeft(label, x - 10);
                    Canvas.SetTop(label, topMargin + chartHeight + 5);
                    ChartCanvas.Children.Add(label);
                }
            }

            // 绘制折线
            foreach (var series in this.Series)
            {
                if (series.Data.Length <= 1)
                {
                    continue;
                }

                double xStep = chartWidth / (series.Data.Length - 1);
                double yScale = chartHeight / (max - min);

                Polyline polyline = new()
                {
                    Stroke = new SolidColorBrush(series.LineColor),
                    StrokeThickness = 1,
                };

                for (int i = 0; i < series.Data.Length; i++)
                {
                    double x = leftMargin + i * xStep;
                    double y = topMargin + chartHeight - (series.Data[i] - min) * yScale;
                    polyline.Points.Add(new Point(x, y));
                }

                ChartCanvas.Children.Add(polyline);
            }
        }

        private void ChartCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (this.Series == null || this.Series.Count == 0)
            {
                return;
            }

            double width = ChartCanvas.ActualWidth;
            double height = ChartCanvas.ActualHeight;
            double leftMargin = 40, bottomMargin = 30, topMargin = 10, rightMargin = 10;
            double chartWidth = width - leftMargin - rightMargin;
            double chartHeight = height - topMargin - bottomMargin;

            Point pos = e.GetCurrentPoint(ChartCanvas).Position;
            if (pos.X < leftMargin || pos.X > leftMargin + chartWidth)
            {
                Tooltip.Visibility = Visibility.Collapsed;
                RemoveHighlightPoints();
                RemoveVerticalLine();
                return;
            }

            // 计算对称Y轴范围
            int minData = Series.SelectMany(s => s.Data).DefaultIfEmpty(0).Min();
            int maxData = Series.SelectMany(s => s.Data).DefaultIfEmpty(0).Max();
            int maxAbs = Math.Max(Math.Abs(minData), Math.Abs(maxData));
            if (maxAbs == 0) maxAbs = 1;
            int min = -maxAbs;
            int max = maxAbs;

            int maxLen = Series.Max(s => s.Data.Length);
            double xStep = chartWidth / (maxLen - 1);
            int index = (int)Math.Round((pos.X - leftMargin) / xStep);
            index = Math.Max(0, Math.Min(maxLen - 1, index));

            // 画竖线
            RemoveVerticalLine();
            double x = leftMargin + index * xStep;
            var vline = new Line
            {
                X1 = x,
                X2 = x,
                Y1 = topMargin,
                Y2 = topMargin + chartHeight,
                Stroke = new SolidColorBrush(this.ChartColor),
                StrokeDashArray = new DoubleCollection { 2, 2 },
                StrokeThickness = 1
            };
            vline.Tag = "VerticalLine";
            ChartCanvas.Children.Add(vline);

            // 显示所有线的数值
            StringBuilder sb = new StringBuilder();
            RemoveHighlightPoints();
            for (int i = 0; i < Series.Count; i++)
            {
                var s = Series[i];
                if (index < s.Data.Length)
                {
                    sb.AppendLine($"{s.Title ?? $"Line{i + 1}"}: {s.Data[index]}");

                    // 画高亮点
                    double yScale = chartHeight / (max - min);
                    double xStepThis = chartWidth / (s.Data.Length - 1);
                    double px = leftMargin + index * xStepThis;
                    double py = topMargin + chartHeight - (s.Data[index] - min) * yScale;

                    var ellipse = new Ellipse
                    {
                        Width = 12,
                        Height = 12,
                        Fill = new SolidColorBrush(s.LineColor),
                        Stroke = new SolidColorBrush(Colors.White),
                        StrokeThickness = 2
                    };
                    Canvas.SetLeft(ellipse, px - 6);
                    Canvas.SetTop(ellipse, py - 6);
                    ChartCanvas.Children.Add(ellipse);
                    _highlightPoints.Add(ellipse);
                }
            }
            TooltipText.Text = sb.ToString().TrimEnd();
            Tooltip.Visibility = Visibility.Visible;

            // Tooltip位置
            double tooltipX = x + 10;
            double tooltipY = topMargin + 10;
            if (tooltipX + Tooltip.ActualWidth > width)
                tooltipX = x - Tooltip.ActualWidth - 10;
            Tooltip.Margin = new Thickness(tooltipX, tooltipY, 0, 0);
        }

        private void ChartCanvas_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Tooltip.Visibility = Visibility.Collapsed;
            RemoveHighlightPoints();
            RemoveVerticalLine();
        }

        private void RemoveHighlightPoints()
        {
            foreach (var el in _highlightPoints)
                ChartCanvas.Children.Remove(el);
            _highlightPoints.Clear();
        }

        private void RemoveVerticalLine()
        {
            for (int i = ChartCanvas.Children.Count - 1; i >= 0; i--)
            {
                if (ChartCanvas.Children[i] is Line l && l.Tag as string == "VerticalLine")
                    ChartCanvas.Children.RemoveAt(i);
            }
        }
    }
}
