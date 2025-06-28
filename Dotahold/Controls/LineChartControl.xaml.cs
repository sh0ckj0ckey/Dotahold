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

        public static readonly DependencyProperty XAxisLabelFormatProperty = DependencyProperty.Register(nameof(XAxisLabelFormat), typeof(string), typeof(LineChartControl), new PropertyMetadata("{0}", AxisLabelFormatChanged));

        public static readonly DependencyProperty YAxisLabelFormatProperty = DependencyProperty.Register(nameof(YAxisLabelFormat), typeof(string), typeof(LineChartControl), new PropertyMetadata("{0}", AxisLabelFormatChanged));

        public static new readonly DependencyProperty FontFamilyProperty = DependencyProperty.Register(nameof(FontFamily), typeof(FontFamily), typeof(LineChartControl), new PropertyMetadata(Application.Current.Resources["ContentControlThemeFontFamily"] as FontFamily ?? new FontFamily("Segoe UI"), FontFamilyChanged));

        private readonly List<UIElement> _highlightPoints = [];

        private readonly double _topMargin = 1;
        private readonly double _bottomMargin = 49;
        private readonly double _leftMargin = 1;
        private readonly double _rightMargin = 1;

        /// <summary>
        /// Color of the chart lines and axes
        /// </summary>
        public Color ChartColor
        {
            get => (Color)GetValue(ChartColorProperty);
            set => SetValue(ChartColorProperty, value);
        }

        /// <summary>
        /// Gets or sets the collection of line series to be displayed
        /// </summary>
        public List<LineSeries> Series
        {
            get => (List<LineSeries>)GetValue(SeriesProperty);
            set => SetValue(SeriesProperty, value);
        }

        /// <summary>
        /// Indicating whether the negative area of the chart should be displayed
        /// </summary>
        public bool ShowNegativeArea
        {
            get => (bool)GetValue(ShowNegativeAreaProperty);
            set => SetValue(ShowNegativeAreaProperty, value);
        }

        /// <summary>
        /// The minimal step size for the X-axis in the chart, when the chart size is insufficient to display all labels, downsampling will be applied
        /// </summary>
        public int XAxisStep
        {
            get => (int)GetValue(XAxisStepProperty);
            set => SetValue(XAxisStepProperty, value);
        }

        /// <summary>
        /// The minimal step size for the Y-axis in the chart, when the chart size is insufficient to display all labels, downsampling will be applied
        /// </summary>
        public int YAxisStep
        {
            get => (int)GetValue(YAxisStepProperty);
            set => SetValue(YAxisStepProperty, value);
        }

        /// <summary>
        /// The format string used to display labels on the X-axis, the value should be like "{}{0}:00" in XAML and "{0}:00" in C# code
        /// </summary>
        public string XAxisLabelFormat
        {
            get => (string)GetValue(XAxisLabelFormatProperty);
            set => SetValue(XAxisLabelFormatProperty, value);
        }

        /// <summary>
        /// The format string used to display labels on the Y-axis, the value should be like "{}{0}:00" in XAML and "{0}:00" in C# code
        /// </summary>
        public string YAxisLabelFormat
        {
            get => (string)GetValue(YAxisLabelFormatProperty);
            set => SetValue(YAxisLabelFormatProperty, value);
        }

        /// <summary>
        /// The font family used to render text.
        /// </summary>
        public new FontFamily FontFamily
        {
            get => (FontFamily)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
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

        private static void AxisLabelFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LineChartControl)d).DrawChart();
        }

        private static void FontFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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

            // Get the actual size of the chart area
            double width = ChartCanvas.ActualWidth;
            double height = ChartCanvas.ActualHeight;
            if (width <= 0 || height <= 0)
            {
                return;
            }

            double chartWidth = width - _leftMargin - _rightMargin;
            double chartHeight = height - _topMargin - _bottomMargin;

            var strokeColorBrush = new SolidColorBrush(this.ChartColor);

            // Compute the Y-axis range based on the data in all series
            int minData = this.Series.SelectMany(s => s.Data).DefaultIfEmpty(0).Min();
            int maxData = this.Series.SelectMany(s => s.Data).DefaultIfEmpty(0).Max();
            int maxAbs = Math.Max(Math.Abs(minData), Math.Abs(maxData));
            maxAbs = ((maxAbs + 9999) / 10000) * 10000;
            if (maxAbs == 0)
            {
                maxAbs = 10000;
            }

            int max = maxAbs;
            int min = this.ShowNegativeArea ? -maxAbs : 0;

            // Draw the chart border
            {
                var borderRect = new Rectangle
                {
                    Width = chartWidth + 2,
                    Height = chartHeight + 2,
                    Stroke = strokeColorBrush,
                    StrokeThickness = 1,
                    RadiusX = 4,
                    RadiusY = 4,
                    Fill = null,
                    Opacity = 0.1,
                };

                Canvas.SetLeft(borderRect, _leftMargin - 1);
                Canvas.SetTop(borderRect, _topMargin - 1);
                ChartCanvas.Children.Add(borderRect);
            }

            // Draw the zero line if negative area is shown
            if (this.ShowNegativeArea)
            {
                double zeroY = _topMargin + chartHeight - (0 - min) * chartHeight / (max - min);
                ChartCanvas.Children.Add(new Line
                {
                    X1 = _leftMargin,
                    Y1 = zeroY,
                    X2 = _leftMargin + chartWidth,
                    Y2 = zeroY,
                    Stroke = strokeColorBrush,
                    StrokeThickness = 1,
                    Opacity = 0.3,
                });
            }

            // Draw Y-axis ticks
            if (this.YAxisStep > 0)
            {
                int yStep = this.YAxisStep;
                int yRange = max - min;
                double minLabelSpacing = 24;
                int maxYLabels = (int)(chartHeight / minLabelSpacing);
                int yLabelCount = yRange / yStep;

                // Downsample the Y-axis ticks, double the step size until the labels fit
                while (yLabelCount > maxYLabels && yStep < yRange)
                {
                    yStep *= 2;
                    yLabelCount = yRange / yStep;
                }

                for (int value = min + yStep; value <= max - yStep; value += yStep)
                {
                    if (value == 0)
                    {
                        continue;
                    }

                    double y = _topMargin + chartHeight - (value - min) * chartHeight / (max - min);

                    // Y-axis tick label
                    var label = new TextBlock
                    {
                        Text = string.Format(this.YAxisLabelFormat, value),
                        Opacity = 0.7,
                        FontSize = 14,
                        Foreground = strokeColorBrush,
                        FontFamily = this.FontFamily,
                    };

                    // Measure the label size to position it correctly
                    label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    double labelWidth = label.DesiredSize.Width;
                    double labelHeight = label.DesiredSize.Height;

                    Canvas.SetLeft(label, 4);
                    Canvas.SetTop(label, y - labelHeight / 2);
                    ChartCanvas.Children.Add(label);

                    // Draw Y-axis tick line
                    ChartCanvas.Children.Add(new Line
                    {
                        X1 = _leftMargin + labelWidth + 8,
                        Y1 = y,
                        X2 = _leftMargin + chartWidth,
                        Y2 = y,
                        Stroke = strokeColorBrush,
                        StrokeThickness = 1,
                        Opacity = 0.1,
                    });
                }
            }

            // Draw X-axis ticks
            if (this.XAxisStep > 0)
            {
                int maxLen = this.Series.Max(s => s.Data.Length);
                int xStep = this.XAxisStep;
                double minLabelSpacing = 48;
                int maxXLabels = (int)(chartWidth / minLabelSpacing);
                int xLabelCount = (maxLen - 1) / xStep;

                // Downsample the X-axis ticks, double the step size until the labels fit
                while (xLabelCount > maxXLabels && xStep < maxLen)
                {
                    xStep *= 2;
                    xLabelCount = (maxLen - 1) / xStep;
                }

                for (int i = xStep; i < maxLen; i += xStep)
                {
                    double x = _leftMargin + i * chartWidth / (maxLen - 1);

                    if (x == _leftMargin + chartWidth)
                    {
                        continue;
                    }

                    // X-axis tick label
                    var label = new TextBlock
                    {
                        Text = string.Format(this.XAxisLabelFormat, i),
                        Opacity = 0.7,
                        FontSize = 14,
                        Foreground = strokeColorBrush,
                        FontFamily = this.FontFamily,
                    };

                    // Measure the label size to position it correctly
                    label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    double labelWidth = label.DesiredSize.Width;

                    Canvas.SetLeft(label, x - labelWidth / 2);
                    Canvas.SetTop(label, _topMargin + chartHeight + 6);
                    ChartCanvas.Children.Add(label);

                    // Draw X-axis tick line
                    ChartCanvas.Children.Add(new Line
                    {
                        X1 = x,
                        Y1 = _topMargin + chartHeight,
                        X2 = x,
                        Y2 = _topMargin,
                        Stroke = strokeColorBrush,
                        StrokeThickness = 1,
                        Opacity = 0.1,
                    });
                }
            }

            // Draw the chart lines for each series
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
                    double x = _leftMargin + i * xStep;
                    double y = _topMargin + chartHeight - (series.Data[i] - min) * yScale;
                    polyline.Points.Add(new Point(x, y));
                }

                ChartCanvas.Children.Add(polyline);
            }
        }

        private void ChartCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (this.Series is null || this.Series.Count <= 0)
            {
                return;
            }

            double width = ChartCanvas.ActualWidth;
            double height = ChartCanvas.ActualHeight;
            if (width <= 0 || height <= 0)
            {
                return;
            }

            double chartWidth = width - _leftMargin - _rightMargin;
            double chartHeight = height - _topMargin - _bottomMargin;

            var strokeColorBrush = new SolidColorBrush(this.ChartColor);

            Point pos = e.GetCurrentPoint(ChartCanvas).Position;
            if (pos.X <= _leftMargin || pos.X >= _leftMargin + chartWidth)
            {
                Tooltip.Visibility = Visibility.Collapsed;
                RemoveHighlightPoints();
                RemoveVerticalLine();
                return;
            }

            // 计算Y轴范围
            int minData = this.Series.SelectMany(s => s.Data).DefaultIfEmpty(0).Min();
            int maxData = this.Series.SelectMany(s => s.Data).DefaultIfEmpty(0).Max();
            int maxAbs = Math.Max(Math.Abs(minData), Math.Abs(maxData));
            maxAbs = ((maxAbs + 9999) / 10000) * 10000;
            if (maxAbs == 0)
            {
                maxAbs = 10000;
            }

            int max = maxAbs;
            int min = this.ShowNegativeArea ? -maxAbs : 0;

            int maxLen = this.Series.Max(s => s.Data.Length);
            double xStep = chartWidth / (maxLen - 1);
            int index = (int)Math.Round((pos.X - _leftMargin) / xStep);
            index = Math.Max(0, Math.Min(maxLen - 1, index));

            // 绘制竖线
            RemoveVerticalLine();
            double x = _leftMargin + index * xStep;
            ChartCanvas.Children.Add(new Line
            {
                X1 = x,
                Y1 = _topMargin,
                X2 = x,
                Y2 = _topMargin + chartHeight,
                Stroke = strokeColorBrush,
                StrokeDashArray = [2, 2],
                StrokeThickness = 1,
                Opacity = 0.5,
                Tag = "VerticalLine",
            });

            // 显示所有线的数值
            RemoveHighlightPoints();
            StringBuilder sb = new();
            for (int i = 0; i < this.Series.Count; i++)
            {
                var s = this.Series[i];
                if (index < s.Data.Length)
                {
                    sb.AppendLine($"{s.Title ?? $"Line{i + 1}"}: {s.Data[index]}");

                    // 绘制高亮点
                    double yScale = chartHeight / (max - min);
                    double xStepThis = chartWidth / (s.Data.Length - 1);
                    double px = _leftMargin + index * xStepThis;
                    double py = _topMargin + chartHeight - (s.Data[index] - min) * yScale;

                    var ellipse = new Ellipse
                    {
                        Width = 12,
                        Height = 12,
                        Fill = new SolidColorBrush(s.LineColor),
                        Stroke = strokeColorBrush,
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
            double tooltipY = _topMargin + 10;
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
