using System;
using System.Threading.Tasks;
using Xamarin.Forms;

using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Diagnostics;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace RadialGauge
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Gauge : ContentView
    {
        public static readonly BindableProperty CurrentValueProperty =
      BindableProperty.Create("CurrentValue", typeof(int), typeof(Gauge), propertyChanged: OnCurrentValueChanged, validateValue: ValidateCurrVal);
        public static readonly BindableProperty MaxValueProperty =
            BindableProperty.Create("MaxValue", typeof(int), typeof(Gauge), 100, propertyChanged: OnMaxValueChanged, validateValue: ValidateMaxVal);
        public static readonly BindableProperty MinValueProperty =
           BindableProperty.Create("MinValue", typeof(int), typeof(Gauge), 0, propertyChanged: OnMinValueChanged, validateValue: ValidateMinVal);
        public static readonly BindableProperty HasAnimationProperty =
            BindableProperty.Create("HasAnimation", typeof(bool), typeof(Gauge), false);
        public static readonly BindableProperty FromColorProperty =
            BindableProperty.Create("FromColor", typeof(System.Drawing.Color), typeof(Gauge), System.Drawing.Color.Red);
        public static readonly BindableProperty ToColorProperty =
            BindableProperty.Create("ToColor", typeof(System.Drawing.Color), typeof(Gauge), System.Drawing.Color.Green);
        public static readonly BindableProperty ViaColorProperty =
            BindableProperty.Create("ViaColor", typeof(System.Drawing.Color), typeof(Gauge), System.Drawing.Color.Gold);
        public static readonly BindableProperty EmptyFillColorProperty =
            BindableProperty.Create("EmptyFillColor", typeof(Color), typeof(Gauge), Color.FromHex("#e0dfdf"));
        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create("TextColor", typeof(Color), typeof(Gauge), Color.FromHex("#676a69"));
        public static readonly BindableProperty UnitOfMeasurementProperty =
            BindableProperty.Create("UnitOfMeasurement", typeof(string), typeof(Gauge), "");
        public static readonly BindableProperty BottomTextProperty =
            BindableProperty.Create("BottomText", typeof(string), typeof(Gauge), "");
        public static readonly BindableProperty TextFontProperty =
            BindableProperty.Create("TextFont", typeof(string), typeof(Gauge), "Helvetica");

        //UnitOfMeasurement
        public int CurrentValue
        {
            get => (int)GetValue(CurrentValueProperty);
            set => SetValue(CurrentValueProperty, value);
        }

        public bool HasAnimation
        {
            get => (bool)GetValue(HasAnimationProperty);
            set => SetValue(HasAnimationProperty, value);
        }

        public int MaxValue
        {
            get => (int)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }
        public int MinValue
        {
            get => (int)GetValue(MinValueProperty);
            set => SetValue(MinValueProperty, value);
        }
        public System.Drawing.Color FromColor
        {
            get => (System.Drawing.Color)GetValue(FromColorProperty);
            set => SetValue(FromColorProperty, value);
        }

        public System.Drawing.Color ToColor
        {
            get => (System.Drawing.Color)GetValue(ToColorProperty);
            set => SetValue(ToColorProperty, value);
        }

        public System.Drawing.Color ViaColor
        {
            get => (System.Drawing.Color)GetValue(ViaColorProperty);
            set => SetValue(ViaColorProperty, value);
        }

        public Color EmptyFillColor
        {
            get => (Color)GetValue(EmptyFillColorProperty);
            set => SetValue(EmptyFillColorProperty, value);
        }

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public string UnitOfMeasurement
        {
            get => (string)GetValue(UnitOfMeasurementProperty);
            set => SetValue(UnitOfMeasurementProperty, value);
        }

        public string BottomText
        {
            get => (string)GetValue(BottomTextProperty);
            set => SetValue(BottomTextProperty, value);
        }

        public string TextFont
        {
            get => (string)GetValue(TextFontProperty);
            set => SetValue(TextFontProperty, value);
        }


        public Gauge()
        {
            InitializeComponent();

            canvas.PaintSurface += (sender, e) => DrawGaugeAsync(e);
            canvas.BackgroundColor = BackgroundColor;

            sweepAngleSlider.ValueChanged += (sender, e) => canvas.InvalidateSurface();
        }

        static bool ValidateMaxVal(BindableObject b, object value) => true;// ((int)value) > 0;
        private static bool ValidateMinVal(BindableObject bindable, object value) => true;

        static bool ValidateCurrVal(BindableObject b, object value) => ((int)value) <= ((Gauge)b).MaxValue && ((int)value) >= ((Gauge)b).MinValue;

        static void OnCurrentValueChanged(BindableObject b, object oldValue, object newValue)
        {

            Gauge g = (Gauge)b;
            int endState = g.getSweepAngle(g.MaxValue, g.MinValue, (int)newValue);
            _ = g.AnimateProgress(endState);
        }

        static void OnMaxValueChanged(BindableObject b, object oldValue, object newValue)
        {

            Gauge g = (Gauge)b;
            if (g.CurrentValue > (int)newValue) g.CurrentValue = (int)newValue;
            int endState = g.getSweepAngle((int)newValue, g.MinValue, g.CurrentValue);
            _ = g.AnimateProgress(endState);
        }
        private static void OnMinValueChanged(BindableObject bindable, object oldValue, object newValue)
        {
            Gauge g = (Gauge)bindable;
            if (g.CurrentValue < (int)newValue) g.CurrentValue = (int)newValue;
            int endState = g.getSweepAngle(g.MaxValue, (int)newValue, g.CurrentValue);
            _ = g.AnimateProgress(endState);
        }
        // Animating the Progress of Radial Gauge
        private async Task AnimateProgress(int progress)
        {
            if (HasAnimation)
            {
                var depart = sweepAngleSlider.Value;
                var sens = 5;

                if (depart > progress)
                {
                    sens = -5;
                }
                while (Math.Abs( progress-depart ) > 5)
                {
                    depart += sens;
                    sweepAngleSlider.Value = depart;
                    await Task.Delay(3);
                }
            }
        sweepAngleSlider.Value = progress;
        }
    public int getSweepAngle(int max, int min, int achieved)
    {
        int SweepAngle = 260;
        float factor = (float)(achieved - min) / (max - min);
        Debug.WriteLine("SWEEP ANGLE : " + (int)(SweepAngle * factor));

        return (int)(SweepAngle * factor);

    }
    private float convertUnit2Px(float value)
    {
        return (float)(value * (float)DeviceDisplay.MainDisplayInfo.Density);
    }
    public void DrawGaugeAsync(SKPaintSurfaceEventArgs args)
    {
        /*
        // Radial Gauge Constants
        int uPadding = 150;
        int side = 500;
        int radialGaugeWidth = 25;

        // Line TextSize inside Radial Gauge
        int textSize1 = 220;
        int textSize2 = 70;
        int textSize3 = 80;

        // Line Y Coordinate inside Radial Gauge
        int lineHeight1 = 100;
        int lineHeight2 = 200;
        int lineHeight3 = 300;

        // Start & End Angle for Radial Gauge
        float startAngle = -220;
        float sweepAngle = 260;
         */

        // Radial Gauge Constants         

        // version originale ramenée à 300
        int side = 300;
        float scale = (float)(Math.Min(Width, Height) / side);// unité xamarin forms           

        float radialGaugeWidth = 30 * scale;
        float uPadding = 25 * scale;
        // centre
        float X_center = side * scale / 2;
        float Y_center = X_center;
        // Line TextSize inside Radial Gauge
        float textSize1 = 80 * scale;
        float textSize2 = 42 * scale;
        float textSize3 = 30 * scale;

        // Line Y Coordinate inside Radial Gauge
        float lineHeight1 = 50 * scale;
        float lineHeight2 = 80 * scale;

        // Start & End Angle for Radial Gauge
        float startAngle = -220;
        float sweepAngle = 260;

        try
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;
            canvas.Clear();

            // Top Padding for Radial Gauge

            float Padding_px = convertUnit2Px(uPadding);

            // Gauge center coordonates
            int Xc = (int)convertUnit2Px(X_center);
            int Yc = (int)convertUnit2Px(Y_center);
            // X1 Y1 are lefttop cordiates of rectange
            int X1 = (int)(Padding_px);
            int Y1 = (int)(Padding_px);

            // X2 Y2 are rightbottom cordiates of rectange
            int X2 = (int)(2 * Xc - Padding_px);
            int Y2 = (int)(2 * Yc - Padding_px);


            //  Empty Gauge Styling
            SKPaint paint1 = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = EmptyFillColor.ToSKColor(),
                StrokeWidth = convertUnit2Px(radialGaugeWidth),
                StrokeCap = SKStrokeCap.Round
            };

            double pct = (double)(CurrentValue - MinValue) / (MaxValue - MinValue);
            System.Drawing.Color interpolated = ColorInterpolator.InterpolateBetween(FromColor, ToColor, ViaColor, pct);

            // Filled Gauge Styling
            SKPaint paint2 = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = new SKColor(interpolated.R, interpolated.G, interpolated.B),
                StrokeWidth = convertUnit2Px(radialGaugeWidth),
                StrokeCap = SKStrokeCap.Round
            };

            // Defining boundaries for Gauge
            SKRect rect = new SKRect(X1, Y1, X2, Y2);

            // Rendering Empty Gauge
            SKPath path1 = new SKPath();
            path1.AddArc(rect, startAngle, sweepAngle);
            canvas.DrawPath(path1, paint1);

            // Rendering Filled Gauge
            SKPath path2 = new SKPath();
            path2.AddArc(rect, startAngle, (float)sweepAngleSlider.Value);
            canvas.DrawPath(path2, paint2);

            // Achieved Minutes
            using (SKPaint skPaint = new SKPaint())
            {
                skPaint.Style = SKPaintStyle.Fill;
                skPaint.IsAntialias = true;
                skPaint.Color = TextColor.ToSKColor();
                skPaint.TextAlign = SKTextAlign.Center;
                skPaint.TextSize = convertUnit2Px(textSize1);
                skPaint.Typeface = SKTypeface.FromFamilyName(
                                    TextFont,
                                    SKFontStyleWeight.Bold,
                                    SKFontStyleWidth.Normal,
                                    SKFontStyleSlant.Upright);
                canvas.DrawText(CurrentValue + "", Xc, Yc, skPaint);
            }

            //// Achieved Minutes Text Styling
            using (SKPaint skPaint = new SKPaint())
            {
                skPaint.Style = SKPaintStyle.Fill;
                skPaint.IsAntialias = true;
                skPaint.Color = TextColor.ToSKColor();
                skPaint.TextAlign = SKTextAlign.Center;
                skPaint.TextSize = convertUnit2Px(textSize2);
                canvas.DrawText(UnitOfMeasurement, Xc, Yc + convertUnit2Px(lineHeight1), skPaint);
            }

            // Goal Minutes Text Styling
            using (SKPaint skPaint = new SKPaint())
            {
                skPaint.Style = SKPaintStyle.Fill;
                skPaint.IsAntialias = true;
                skPaint.Color = TextColor.ToSKColor();
                skPaint.TextAlign = SKTextAlign.Center;
                skPaint.TextSize = convertUnit2Px(textSize3);
                canvas.DrawText(BottomText, Xc, Yc + convertUnit2Px(lineHeight2), skPaint);
            }
            // min et max
            using (SKPaint skPaint = new SKPaint())
            {
                skPaint.Style = SKPaintStyle.Fill;
                skPaint.IsAntialias = true;
                skPaint.Color = TextColor.ToSKColor();
                skPaint.TextAlign = SKTextAlign.Left;
                skPaint.TextSize = convertUnit2Px(textSize3);
                canvas.DrawText(MinValue.ToString(), X1 + convertUnit2Px(radialGaugeWidth), Y2, skPaint);
            }
            using (SKPaint skPaint = new SKPaint())
            {
                skPaint.Style = SKPaintStyle.Fill;
                skPaint.IsAntialias = true;
                skPaint.Color = TextColor.ToSKColor();
                skPaint.TextAlign = SKTextAlign.Right;
                skPaint.TextSize = convertUnit2Px(textSize3);
                canvas.DrawText(MaxValue.ToString(), X2 - convertUnit2Px(radialGaugeWidth), Y2, skPaint);
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.StackTrace);
        }
    }
}
}
