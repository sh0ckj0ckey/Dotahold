using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI;
using Windows.Foundation;
using Dotahold.Core.DataShop;

namespace Dotahold.UI
{
    // https://github.com/sonnemaf/Outline/
    sealed class OutlinedTextBlock : UserControl
    {
        private CanvasControl _canvas;
        private long _tokenForeground;
        private long _tokenFontSize;
        private long _tokenFontFamily;
        private long _tokenFontStyle;
        private long _tokenFontWeight;
        private long _tokenHorizontalContentAlignment;

        /// <summary> 
        /// Get or Sets the TextColor dependency property.  
        /// </summary>
        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        /// <summary> 
        /// Identifies the TextColor dependency property. This enables animation, styling, binding, etc...
        /// </summary> 
        public static readonly DependencyProperty TextColorProperty =
            DependencyProperty.Register(nameof(TextColor),
                                        typeof(Color),
                                        typeof(OutlinedTextBlock),
                                        new PropertyMetadata(Colors.White, OnPropertyChanged));

        /// <summary> 
        /// Get or Sets the OutlineColor dependency property.  
        /// </summary> 
        public Color OutlineColor
        {
            get { return (Color)GetValue(OutlineColorProperty); }
            set { SetValue(OutlineColorProperty, value); }
        }

        /// <summary> 
        /// Identifies the OutlineColor dependency property. This enables animation, styling, binding, etc...
        /// </summary> 
        public static readonly DependencyProperty OutlineColorProperty =
            DependencyProperty.Register(nameof(OutlineColor),
                                        typeof(Color),
                                        typeof(OutlinedTextBlock),
                                        new PropertyMetadata(Colors.Black, OnPropertyChanged));

        /// <summary> 
        /// Get or Sets the TextWrapping dependency property.  
        /// </summary> 
        public CanvasWordWrapping TextWrapping
        {
            get { return (CanvasWordWrapping)GetValue(TextWrappingProperty); }
            set { SetValue(TextWrappingProperty, value); }
        }

        /// <summary> 
        /// Identifies the TextWrapping dependency property. This enables animation, styling, binding, etc...
        /// </summary> 
        public static readonly DependencyProperty TextWrappingProperty =
            DependencyProperty.Register(nameof(TextWrapping),
                                        typeof(CanvasWordWrapping),
                                        typeof(OutlinedTextBlock),
                                        new PropertyMetadata(CanvasWordWrapping.NoWrap, OnPropertyChanged));

        /// <summary> 
        /// Get or Sets the Text dependency property.  
        /// </summary> 
        public string Text
        {
            get { return (string)this.GetValue(TextProperty); }
            set { this.SetValue(TextProperty, value); }
        }

        /// <summary> 
        /// Identifies the Text dependency property. This enables animation, styling, binding, etc...
        /// </summary> 
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text),
                                        typeof(string),
                                        typeof(OutlinedTextBlock),
                                        new PropertyMetadata(string.Empty, OnPropertyChanged));


        /// <summary> 
        /// Get or Sets the OutlineThickness dependency property.  
        /// </summary> 
        public double OutlineThickness
        {
            get { return (double)this.GetValue(OutlineThicknessProperty); }
            set { this.SetValue(OutlineThicknessProperty, value); }
        }

        /// <summary> 
        /// Identifies the OutlineThickness dependency property. This enables animation, styling, binding, etc...
        /// </summary> 
        public static readonly DependencyProperty OutlineThicknessProperty =
            DependencyProperty.Register(nameof(OutlineThickness),
                                        typeof(double),
                                        typeof(OutlinedTextBlock),
                                        new PropertyMetadata(2D, OnPropertyChanged));

        private void OnPropertyChanged2(DependencyObject sender, DependencyProperty dp)
        {
            _canvas.Invalidate();
        }

        public OutlinedTextBlock()
        {
            this.Loaded += this.OutlinedTextBlock_Loaded;
            this.Unloaded += this.OutlinedTextBlock_Unloaded;
        }

        private void OutlinedTextBlock_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _canvas = new CanvasControl();
            _canvas.Draw += OnDraw;
            Content = _canvas;

            _tokenForeground = this.RegisterPropertyChangedCallback(ForegroundProperty, OnPropertyChanged2);
            _tokenFontSize = this.RegisterPropertyChangedCallback(FontSizeProperty, OnPropertyChanged2);
            _tokenFontFamily = this.RegisterPropertyChangedCallback(FontFamilyProperty, OnPropertyChanged2);
            _tokenFontStyle = this.RegisterPropertyChangedCallback(FontStyleProperty, OnPropertyChanged2);
            _tokenFontWeight = this.RegisterPropertyChangedCallback(FontWeightProperty, OnPropertyChanged2);
            _tokenHorizontalContentAlignment = this.RegisterPropertyChangedCallback(HorizontalContentAlignmentProperty, OnPropertyChanged2);
        }

        private void OutlinedTextBlock_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // Explicitly remove references to allow the Win2D controls to get garbage collected
            if (_canvas != null)
            {
                _canvas.RemoveFromVisualTree();
                _canvas = null;

                this.UnregisterPropertyChangedCallback(ForegroundProperty, _tokenForeground);
                this.UnregisterPropertyChangedCallback(FontSizeProperty, _tokenFontSize);
                this.UnregisterPropertyChangedCallback(FontFamilyProperty, _tokenFontFamily);
                this.UnregisterPropertyChangedCallback(FontStyleProperty, _tokenFontStyle);
                this.UnregisterPropertyChangedCallback(FontWeightProperty, _tokenFontWeight);
                this.UnregisterPropertyChangedCallback(HorizontalContentAlignmentProperty, _tokenHorizontalContentAlignment);
            }
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((d is OutlinedTextBlock instance) && instance._canvas != null)
            {
                instance._canvas.Invalidate();
            }
        }

        private void OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            try
            {
                //Outlined text
                using (var textLayout = this.CreateTextLayout(args.DrawingSession, sender.Size))
                {
                    var offset = (float)(OutlineThickness / 2);
                    using (var geometry = CanvasGeometry.CreateText(textLayout))
                    {
                        using (var dashedStroke = new CanvasStrokeStyle()
                        {
                            DashStyle = CanvasDashStyle.Solid,
                        })
                        {
                            args.DrawingSession.DrawGeometry(geometry, offset, offset, this.OutlineColor, (float)OutlineThickness, dashedStroke);
                        }
                    }
                    args.DrawingSession.DrawTextLayout(textLayout, offset, offset, this.TextColor);
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            // https://github.com/Microsoft/Win2D-Samples/blob/master/ExampleGallery/GlowTextCustomControl.cs
            // CanvasTextLayout cannot cope with infinite sizes, so we change infinite to some-large-value.
            if (double.IsInfinity(availableSize.Width))
            {
                availableSize.Width = 6000;
            }
            if (double.IsInfinity(availableSize.Height))
            {
                availableSize.Height = 6000;
            }

            var device = CanvasDevice.GetSharedDevice();

            using (var layout = CreateTextLayout(device, availableSize))
            {
                var bounds = layout.LayoutBounds;

                var desiredSize = new Size(Math.Min(availableSize.Width, bounds.Width + ExpandAmount),
                                           Math.Min(availableSize.Height, bounds.Height + ExpandAmount));

                _canvas?.Measure(desiredSize);

                return desiredSize;
            }
        }

        private CanvasTextLayout CreateTextLayout(ICanvasResourceCreator resourceCreator, Size size)
        {

            var format = new CanvasTextFormat()
            {
                HorizontalAlignment = GetCanvasHorizontalAlignemnt(),
                //VerticalAlignment = GetCanvasVerticalAlignment()
                FontSize = (float)this.FontSize,
                FontFamily = this.FontFamily.Source,
                FontStyle = this.FontStyle,
                FontWeight = this.FontWeight,
                WordWrapping = this.TextWrapping,
            };

            return new CanvasTextLayout(resourceCreator, this.Text, format, (float)size.Width, (float)size.Height);
        }


        private double ExpandAmount => OutlineThickness * 2; // { get { return Math.Max(GlowAmount, MaxGlowAmount) * 4; } }

        private CanvasHorizontalAlignment GetCanvasHorizontalAlignemnt()
        {

            switch (HorizontalContentAlignment)
            {
                case HorizontalAlignment.Center:
                    return CanvasHorizontalAlignment.Center;
                case HorizontalAlignment.Left:
                    return CanvasHorizontalAlignment.Left;
                case HorizontalAlignment.Right:
                    return CanvasHorizontalAlignment.Right;
                default:
                    return CanvasHorizontalAlignment.Left;
            }
        }
    }
}
