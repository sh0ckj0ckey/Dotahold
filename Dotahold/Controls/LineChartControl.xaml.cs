using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Dotahold.Data.Models;
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
    internal partial class LineSeriesTooltipModel : ObservableObject
    {
        private AsyncImage? _icon = null;

        private string _title = string.Empty;

        private int _data = 0;

        public AsyncImage? Icon
        {
            get => _icon;
            set => SetProperty(ref _icon, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public int Data
        {
            get => _data;
            set => SetProperty(ref _data, value);
        }
    }

    internal static class LineSeriesTooltipModelExtensions
    {
        /// <summary>
        /// Swap all properties between two LineSeriesTooltipModel instances
        /// </summary>
        public static void SwapPropertiesWith(this LineSeriesTooltipModel self, LineSeriesTooltipModel other)
        {
            (other.Icon, self.Icon) = (self.Icon, other.Icon);
            (other.Title, self.Title) = (self.Title, other.Title);
            (other.Data, self.Data) = (self.Data, other.Data);
        }
    }

    public sealed partial class LineChartControl : UserControl
    {
        public static readonly DependencyProperty ChartColorProperty = DependencyProperty.Register(nameof(ChartColor), typeof(Color), typeof(LineChartControl), new PropertyMetadata(Colors.Gray, ChartColorChanged));

        public static readonly DependencyProperty TooltipBackgroundColorProperty = DependencyProperty.Register(nameof(TooltipBackgroundColor), typeof(Color), typeof(LineChartControl), new PropertyMetadata(Color.FromArgb(140, 0, 0, 0)));

        public static readonly DependencyProperty SeriesProperty = DependencyProperty.Register(nameof(Series), typeof(List<LineSeries>), typeof(LineChartControl), new PropertyMetadata(null, SeriesChanged));

        public static readonly DependencyProperty ShowNegativeAreaProperty = DependencyProperty.Register(nameof(ShowNegativeArea), typeof(bool), typeof(LineChartControl), new PropertyMetadata(true, ShowNegativeAreaChanged));

        public static readonly DependencyProperty XAxisStepProperty = DependencyProperty.Register(nameof(XAxisStep), typeof(int), typeof(LineChartControl), new PropertyMetadata(0, AxisStepChanged));

        public static readonly DependencyProperty YAxisStepProperty = DependencyProperty.Register(nameof(YAxisStep), typeof(int), typeof(LineChartControl), new PropertyMetadata(0, AxisStepChanged));

        public static readonly DependencyProperty XAxisLabelFormatProperty = DependencyProperty.Register(nameof(XAxisLabelFormat), typeof(string), typeof(LineChartControl), new PropertyMetadata("{0}", AxisLabelFormatChanged));

        public static readonly DependencyProperty YAxisLabelFormatProperty = DependencyProperty.Register(nameof(YAxisLabelFormat), typeof(string), typeof(LineChartControl), new PropertyMetadata("{0}", AxisLabelFormatChanged));

        public static new readonly DependencyProperty FontFamilyProperty = DependencyProperty.Register(nameof(FontFamily), typeof(FontFamily), typeof(LineChartControl), new PropertyMetadata(Application.Current.Resources["ContentControlThemeFontFamily"] as FontFamily ?? new FontFamily("Segoe UI"), FontFamilyChanged));

        private readonly double _topMargin = 1;

        private readonly double _bottomMargin = 37;

        private readonly double _leftMargin = 1;

        private readonly double _rightMargin = 1;

        private int _maxYAxis = 0;

        private int _minYAxis = 0;

        /// <summary>
        /// Last hovered index in the chart to show tooltip
        /// </summary>
        private int? _hoverIndex = null;

        /// <summary>
        /// Last hovered points in the chart to highlight
        /// </summary>
        private readonly List<UIElement> _hoverHighlightPoints = [];

        /// <summary>
        /// Last vertial line in the chart to show hover position
        /// </summary>
        private UIElement? _hoverVerticalLine = null;

        /// <summary>
        /// Collection of tooltip data for each series at the hovered index
        /// </summary>
        private readonly ObservableCollection<LineSeriesTooltipModel> _tooltipData = [];

        /// <summary>
        /// Color of the chart lines and axes
        /// </summary>
        public Color ChartColor
        {
            get => (Color)GetValue(ChartColorProperty);
            set => SetValue(ChartColorProperty, value);
        }

        /// <summary>
        /// Color of tooltip background
        /// </summary>
        public Color TooltipBackgroundColor
        {
            get => (Color)GetValue(TooltipBackgroundColorProperty);
            set => SetValue(TooltipBackgroundColorProperty, value);
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
            this.DataContext = this;
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
            HideTooltip();

            if (this.Series is null || this.Series.Count <= 0 || this.Series.Max(s => s.Data.Length) <= 1)
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

            // Compute the Y-axis range based on the data in all series
            {
                int maxYData = this.Series.SelectMany(s => s.Data).DefaultIfEmpty(0).Max();
                int minYData = this.Series.SelectMany(s => s.Data).DefaultIfEmpty(0).Min();
                int maxAbs = Math.Max(Math.Abs(minYData), Math.Abs(maxYData));

                if (this.YAxisStep > 0)
                {
                    maxAbs = ((maxAbs + this.YAxisStep - 1) / this.YAxisStep) * this.YAxisStep;

                    if (maxAbs == 0)
                    {
                        maxAbs = this.YAxisStep;
                    }
                }

                _maxYAxis = maxAbs;
                _minYAxis = this.ShowNegativeArea ? -maxAbs : 0;
            }

            var strokeColorBrush = new SolidColorBrush(this.ChartColor);

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
            if (this.ShowNegativeArea && this.YAxisStep > 0)
            {
                double zeroY = _topMargin + chartHeight - (0 - _minYAxis) * chartHeight / (_maxYAxis - _minYAxis);
                ChartCanvas.Children.Add(new Line
                {
                    X1 = _leftMargin,
                    Y1 = zeroY,
                    X2 = _leftMargin + chartWidth,
                    Y2 = zeroY,
                    Stroke = strokeColorBrush,
                    StrokeThickness = 1,
                    Opacity = 0.1,
                });
            }

            // Draw Y-axis ticks
            if (this.YAxisStep > 0)
            {
                int yStep = this.YAxisStep;
                int yRange = _maxYAxis - _minYAxis;
                double minLabelSpacing = 24;
                int maxYLabels = (int)(chartHeight / minLabelSpacing);
                int yLabelCount = yRange / yStep;

                // Downsample the Y-axis ticks, double the step size until the labels fit
                while (yLabelCount > maxYLabels && yStep < yRange)
                {
                    yStep *= 2;
                    yLabelCount = yRange / yStep;
                }

                for (int value = _minYAxis + yStep; value <= _maxYAxis - yStep; value += yStep)
                {
                    if (value == 0)
                    {
                        continue;
                    }

                    double y = _topMargin + chartHeight - (value - _minYAxis) * chartHeight / (_maxYAxis - _minYAxis);

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

                    Canvas.SetLeft(label, _leftMargin + 4);
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
                        Opacity = 0.05,
                    });
                }
            }
            else
            {
                if (this.XAxisStep > 0 && !this.ShowNegativeArea)
                {
                    double zeroTick = _topMargin + chartHeight;
                    var zeroLabel = new TextBlock
                    {
                        Text = string.Format(this.YAxisLabelFormat, 0),
                        Opacity = 0.7,
                        FontSize = 14,
                        Foreground = strokeColorBrush,
                        FontFamily = this.FontFamily,
                    };

                    zeroLabel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    double zeroLabelHeight = zeroLabel.DesiredSize.Height;
                    Canvas.SetLeft(zeroLabel, _leftMargin + 4);
                    Canvas.SetTop(zeroLabel, zeroTick - zeroLabelHeight - 6);
                    ChartCanvas.Children.Add(zeroLabel);
                }

                if (this.ShowNegativeArea)
                {
                    double minTick = _topMargin + chartHeight;
                    var minLabel = new TextBlock
                    {
                        Text = string.Format(this.YAxisLabelFormat, _minYAxis),
                        Opacity = 0.7,
                        FontSize = 14,
                        Foreground = strokeColorBrush,
                        FontFamily = this.FontFamily,
                    };

                    minLabel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    double minLabelHeight = minLabel.DesiredSize.Height;
                    Canvas.SetLeft(minLabel, _leftMargin + 4);
                    Canvas.SetTop(minLabel, minTick - minLabelHeight - 6);
                    ChartCanvas.Children.Add(minLabel);
                }

                double maxTick = _topMargin;
                var maxLabel = new TextBlock
                {
                    Text = string.Format(this.YAxisLabelFormat, _maxYAxis),
                    Opacity = 0.7,
                    FontSize = 14,
                    Foreground = strokeColorBrush,
                    FontFamily = this.FontFamily,
                };

                Canvas.SetLeft(maxLabel, _leftMargin + 4);
                Canvas.SetTop(maxLabel, maxTick + 6);
                ChartCanvas.Children.Add(maxLabel);
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
                        Opacity = 0.05,
                    });
                }
            }
            else
            {
                int maxLen = this.Series.Max(s => s.Data.Length);

                double zeroTick = _leftMargin;
                var zeroLabel = new TextBlock
                {
                    Text = string.Format(this.XAxisLabelFormat, 0),
                    Opacity = 0.7,
                    FontSize = 14,
                    Foreground = strokeColorBrush,
                    FontFamily = this.FontFamily,
                };

                Canvas.SetLeft(zeroLabel, zeroTick + 4);
                Canvas.SetTop(zeroLabel, _topMargin + chartHeight + 6);
                ChartCanvas.Children.Add(zeroLabel);

                double maxTick = _leftMargin + chartWidth;
                var maxLabel = new TextBlock
                {
                    Text = string.Format(this.XAxisLabelFormat, maxLen),
                    Opacity = 0.7,
                    FontSize = 14,
                    Foreground = strokeColorBrush,
                    FontFamily = this.FontFamily,
                };

                maxLabel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                double maxLabelWidth = maxLabel.DesiredSize.Width;
                Canvas.SetLeft(maxLabel, maxTick - maxLabelWidth - 4);
                Canvas.SetTop(maxLabel, _topMargin + chartHeight + 6);
                ChartCanvas.Children.Add(maxLabel);
            }

            // Draw the chart lines for each series
            foreach (var series in this.Series)
            {
                if (series.Data.Length <= 1)
                {
                    continue;
                }

                double xStep = chartWidth / (series.Data.Length - 1);
                double yScale = chartHeight / (_maxYAxis - _minYAxis);

                Polyline polyline = new()
                {
                    Stroke = new SolidColorBrush(series.LineColor),
                    StrokeThickness = 1,
                };

                for (int i = 0; i < series.Data.Length; i++)
                {
                    double x = _leftMargin + i * xStep;
                    double y = _topMargin + chartHeight - (series.Data[i] - _minYAxis) * yScale;
                    polyline.Points.Add(new Point(x, y));
                }

                ChartCanvas.Children.Add(polyline);
            }
        }

        private void ChartCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (this.Series is null || this.Series.Count <= 0 || this.Series.Max(s => s.Data.Length) <= 1)
            {
                HideTooltip();
                return;
            }

            // Get the actual size of the chart area
            double width = ChartCanvas.ActualWidth;
            double height = ChartCanvas.ActualHeight;
            if (width <= 0 || height <= 0)
            {
                HideTooltip();
                return;
            }

            double chartWidth = width - _leftMargin - _rightMargin;
            double chartHeight = height - _topMargin - _bottomMargin;

            Point pos = e.GetCurrentPoint(ChartCanvas).Position;
            if (pos.X < _leftMargin || pos.X > _leftMargin + chartWidth)
            {
                HideTooltip();
                return;
            }

            int maxLen = this.Series.Max(s => s.Data.Length);
            double xStep = chartWidth / (maxLen - 1);
            int index = (int)Math.Round((pos.X - _leftMargin) / xStep);
            index = Math.Max(0, Math.Min(maxLen - 1, index));

            // Only update the tooltip if the hovered index has changed
            if (_hoverIndex.HasValue && _hoverIndex.Value == index)
            {
                return;
            }

            _hoverIndex = index;

            double x = _leftMargin + index * xStep;

            var strokeColorBrush = new SolidColorBrush(this.ChartColor);

            // Draw the vertical line at the hovered index
            {
                RemoveHoverVerticalLine();

                _hoverVerticalLine = new Line
                {
                    X1 = x,
                    Y1 = _topMargin,
                    X2 = x,
                    Y2 = _topMargin + chartHeight,
                    Stroke = strokeColorBrush,
                    StrokeDashArray = [2, 2],
                    StrokeThickness = 1,
                    Opacity = 0.2,
                };

                ChartCanvas.Children.Add(_hoverVerticalLine);
            }

            // Draw points of all series at the hovered index, and update the tooltip data
            {
                RemoveHoverHighlightPoints();

                // Ensure the tooltip data collection has the same number of series as the chart
                if (_tooltipData.Count != this.Series.Count)
                {
                    if (_tooltipData.Count > this.Series.Count)
                    {
                        for (int i = _tooltipData.Count - 1; i >= this.Series.Count; i--)
                        {
                            _tooltipData.RemoveAt(i);
                        }
                    }
                    else
                    {
                        for (int i = _tooltipData.Count; i < this.Series.Count; i++)
                        {
                            _tooltipData.Add(new LineSeriesTooltipModel());
                        }
                    }
                }

                for (int i = 0; i < this.Series.Count; i++)
                {
                    var series = this.Series[i];
                    if (series is not null && index < series.Data.Length)
                    {
                        _tooltipData[i].Icon = (series.Data[index] < 0 && series.NegativeIcon is not null) ? series.NegativeIcon : series.Icon;
                        _tooltipData[i].Title = (series.Data[index] < 0 && !string.IsNullOrEmpty(series.NegativeTitle)) ? series.NegativeTitle : series.Title;
                        _tooltipData[i].Data = (!string.IsNullOrEmpty(series.NegativeTitle)) ? Math.Abs(series.Data[index]) : series.Data[index];

                        // Draw the highlight point
                        double yScale = chartHeight / (_maxYAxis - _minYAxis);
                        double xStepThis = chartWidth / (series.Data.Length - 1);
                        double px = _leftMargin + index * xStepThis;
                        double py = _topMargin + chartHeight - (series.Data[index] - _minYAxis) * yScale;

                        var ellipse = new Ellipse
                        {
                            Width = 12,
                            Height = 12,
                            Fill = new SolidColorBrush(series.LineColor),
                            Stroke = strokeColorBrush,
                            StrokeThickness = 2
                        };

                        Canvas.SetLeft(ellipse, px - 6);
                        Canvas.SetTop(ellipse, py - 6);
                        ChartCanvas.Children.Add(ellipse);

                        _hoverHighlightPoints.Add(ellipse);
                    }
                }
            }

            // Sort tooltip data by Data value in descending order
            {
                int count = _tooltipData.Count;
                if (count > 1)
                {
                    bool swapped;
                    for (int i = 0; i < count - 1; i++)
                    {
                        swapped = false;
                        for (int j = 0; j < count - i - 1; j++)
                        {
                            if (_tooltipData[j].Data < _tooltipData[j + 1].Data)
                            {
                                _tooltipData[j].SwapPropertiesWith(_tooltipData[j + 1]);
                                swapped = true;
                            }
                        }

                        if (!swapped)
                        {
                            break;
                        }
                    }
                }
            }

            TooltipTitle.Text = string.Format(this.XAxisLabelFormat, index) + "sssssssssssssssssssssssss";
            Tooltip.Visibility = Visibility.Visible;

            // Update position of the tooltip
            {
                double tooltipX;
                double tooltipY = _topMargin + 8;
                double actualWidth = Tooltip.ActualWidth > 0 ? Tooltip.ActualWidth : 196;

                if (x <= width / 2)
                {
                    tooltipX = x + 8;
                }
                else
                {
                    tooltipX = x - 8 - actualWidth;
                }

                if (tooltipX + actualWidth + 8 > width)
                {
                    tooltipX = width - actualWidth - 8;
                }

                if (tooltipX < 0)
                {
                    tooltipX = 8;
                }

                Tooltip.Margin = new Thickness(tooltipX, tooltipY, 0, 0);
            }
        }

        private void ChartCanvas_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            HideTooltip();
        }

        private void HideTooltip()
        {
            Tooltip.Visibility = Visibility.Collapsed;
            RemoveHoverHighlightPoints();
            RemoveHoverVerticalLine();
            _hoverIndex = null;
        }

        private void RemoveHoverHighlightPoints()
        {
            foreach (var el in _hoverHighlightPoints)
            {
                ChartCanvas.Children.Remove(el);
            }

            _hoverHighlightPoints.Clear();
        }

        private void RemoveHoverVerticalLine()
        {
            if (_hoverVerticalLine is not null)
            {
                ChartCanvas.Children.Remove(_hoverVerticalLine);
                _hoverVerticalLine = null;
            }
        }
    }
}
