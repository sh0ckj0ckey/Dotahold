using System;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace Dotahold.Controls
{
    public partial class UniformGridLayout : NonVirtualizingLayout
    {
        private double _minItemWidth = 296.0;

        public double MinItemWidth
        {
            get => _minItemWidth;
            set
            {
                if (_minItemWidth != value)
                {
                    _minItemWidth = value;
                    InvalidateMeasure();
                }
            }
        }

        protected override Size MeasureOverride(NonVirtualizingLayoutContext context, Size availableSize)
        {
            double width = availableSize.Width;
            int itemsCount = context.Children.Count;

            if (itemsCount == 0 || width == 0 || double.IsInfinity(width))
            {
                return new Size(width, 0);
            }

            int columns = Math.Max(1, Math.Min(itemsCount, (int)(width / MinItemWidth)));
            double itemWidth = width / columns;

            double totalHeight = 0;
            double rowHeight = 0;
            int columnIndex = 0;

            foreach (var child in context.Children)
            {
                child.Measure(new Size(itemWidth, double.PositiveInfinity));
                rowHeight = Math.Max(rowHeight, child.DesiredSize.Height);

                columnIndex++;
                if (columnIndex == columns)
                {
                    totalHeight += rowHeight;
                    rowHeight = 0;
                    columnIndex = 0;
                }
            }

            if (columnIndex != 0)
            {
                totalHeight += rowHeight;
            }

            return new Size(width, totalHeight);
        }

        protected override Size ArrangeOverride(NonVirtualizingLayoutContext context, Size finalSize)
        {
            double width = finalSize.Width;
            int itemsCount = context.Children.Count;

            if (itemsCount == 0 || width == 0)
            {
                return finalSize;
            }

            int columns = Math.Max(1, Math.Min(itemsCount, (int)(width / MinItemWidth)));
            int rows = (itemsCount + columns - 1) / columns;
            double itemWidth = width / columns;

            double[] rowHeights = new double[rows];

            for (int i = 0; i < context.Children.Count; i++)
            {
                int row = i / columns;
                rowHeights[row] = Math.Max(rowHeights[row], context.Children[i].DesiredSize.Height);
            }

            double y = 0;
            for (int row = 0; row < rows; row++)
            {
                double x = 0;

                for (int col = 0; col < columns; col++)
                {
                    int index = row * columns + col;
                    if (index >= itemsCount)
                    {
                        break;
                    }

                    context.Children[index].Arrange(new Rect(x, y, itemWidth, rowHeights[row]));
                    x += itemWidth;
                }

                y += rowHeights[row];
            }

            return finalSize;
        }
    }
}