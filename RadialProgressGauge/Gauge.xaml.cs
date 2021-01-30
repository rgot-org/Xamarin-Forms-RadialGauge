using System;
using System.Threading.Tasks;
using Xamarin.Forms;

using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Diagnostics;
using Xamarin.Forms.Xaml;

namespace RadialProgress
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Gauge : ContentView
    {
        readonly ProgressUtils progressUtils = new ProgressUtils();

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
            BindableProperty.Create("TextFont", typeof(string), typeof(Gauge), "Arial");

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

        static bool ValidateMaxVal(BindableObject b, object value) => ((int)value) > 0;
        private static bool ValidateMinVal(BindableObject bindable, object value)
        {
            throw new NotImplementedException();
        }
        static bool ValidateCurrVal(BindableObject b, object value) => ((int)value) <= ((Gauge)b).MaxValue && ((int)value) >= ((Gauge)b).MinValue;

        static void OnCurrentValueChanged(BindableObject b, object oldValue, object newValue)
        {

            Gauge g = (Gauge)b;
            int endState = g.progressUtils.getSweepAngle(g.MaxValue-g.MinValue, (int)newValue);
            _ = g.AnimateProgress(endState);
        }

        static void OnMaxValueChanged(BindableObject b, object oldValue, object newValue)
        {

            Gauge g = (Gauge)b;
            if (g.CurrentValue > (int)newValue) g.CurrentValue = (int)newValue;
            int endState = g.progressUtils.getSweepAngle((int)newValue-g.MinValue, g.CurrentValue);
            _ = g.AnimateProgress(endState);
        }
        private static void OnMinValueChanged(BindableObject bindable, object oldValue, object newValue)
        {
            Gauge g = (Gauge)bindable;
            if (g.CurrentValue < (int)newValue) g.CurrentValue = (int)newValue;
            int endState = g.progressUtils.getSweepAngle(g.MaxValue-(int)newValue , g.CurrentValue);
            _ = g.AnimateProgress(endState);
        }
        // Animating the Progress of Radial Gauge
        private async Task AnimateProgress(int progress)
        {
            if(HasAnimation)
            {
                sweepAngleSlider.Value = 1;

                // Looping at data interval of 5
                for (int i = 0; i < progress; i = i + 5)
                {
                    sweepAngleSlider.Value = i;
                    await Task.Delay(3);
                }
            }

            sweepAngleSlider.Value = progress;
        }

        public void DrawGaugeAsync(SKPaintSurfaceEventArgs args)
        {
            // Radial Gauge Constants
            int uPadding = 150;
            int side = 500;
            int radialGaugeWidth = 25;

            // Line TextSize inside Radial Gauge
            int lineSize1 = 220;
            int lineSize2 = 70;
            int lineSize3 = 80;

            // Line Y Coordinate inside Radial Gauge
            int lineHeight1 = 100;
            int lineHeight2 = 200;
            int lineHeight3 = 300;

            // Start & End Angle for Radial Gauge
            float startAngle = -220;
            float sweepAngle = 260;

            try
            {
                SKImageInfo info = args.Info;
                SKSurface surface = args.Surface;
                SKCanvas canvas = surface.Canvas;
                progressUtils.SetDevice(info.Height, info.Width);
                canvas.Clear();

                // Top Padding for Radial Gauge
                float upperPading = progressUtils.getFactoredHeight(uPadding);

                // Xc & Yc are center of the Circle
                int Xc = info.Width / 2;
                float Yc = progressUtils.getFactoredHeight(side);

                // X1 Y1 are lefttop cordiates of rectange
                int X1 = (int)(Xc - Yc);
                int Y1 = (int)(Yc - Yc + upperPading);

                // X2 Y2 are rightbottom cordiates of rectange
                int X2 = (int)(Xc + Yc);
                int Y2 = (int)(Yc + Yc + upperPading);

                //Loggig Screen Specific Calculated Values
                Debug.WriteLine("INFO " + info.Width + " - " + info.Height);
                Debug.WriteLine(" C : " + upperPading + "  " + info.Height);
                Debug.WriteLine(" C : " + Xc + "  " + Yc);
                Debug.WriteLine("XY : " + X1 + "  " + Y1);
                Debug.WriteLine("XY : " + X2 + "  " + Y2);

                //  Empty Gauge Styling
                SKPaint paint1 = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    Color = EmptyFillColor.ToSKColor(),
                    StrokeWidth = progressUtils.getFactoredWidth(radialGaugeWidth),
                    StrokeCap = SKStrokeCap.Round
                };

                double pct = (double)CurrentValue / MaxValue;
                System.Drawing.Color interpolated = ColorInterpolator.InterpolateBetween(FromColor, ToColor, ViaColor, pct);

                // Filled Gauge Styling
                SKPaint paint2 = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    Color = new SKColor(interpolated.R, interpolated.G, interpolated.B),
                    StrokeWidth = progressUtils.getFactoredWidth(radialGaugeWidth),
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
                    skPaint.TextSize = progressUtils.getFactoredHeight(lineSize1);
                    skPaint.Typeface = SKTypeface.FromFamilyName(
                                        TextFont,
                                        SKFontStyleWeight.Bold,
                                        SKFontStyleWidth.Normal,
                                        SKFontStyleSlant.Upright);
                   canvas.DrawText(CurrentValue + "", Xc, Yc + progressUtils.getFactoredHeight(lineHeight1), skPaint);
                }

                // Achieved Minutes Text Styling
                using (SKPaint skPaint = new SKPaint())
                {
                    skPaint.Style = SKPaintStyle.Fill;
                    skPaint.IsAntialias = true;
                    skPaint.Color = TextColor.ToSKColor();
                    skPaint.TextAlign = SKTextAlign.Center;
                    skPaint.TextSize = progressUtils.getFactoredHeight(lineSize2);
                    canvas.DrawText(UnitOfMeasurement, Xc, Yc + progressUtils.getFactoredHeight(lineHeight2), skPaint);
                }

                // Goal Minutes Text Styling
                using (SKPaint skPaint = new SKPaint())
                {
                    skPaint.Style = SKPaintStyle.Fill;
                    skPaint.IsAntialias = true;
                    skPaint.Color = TextColor.ToSKColor();
                    skPaint.TextAlign = SKTextAlign.Center;
                    skPaint.TextSize = progressUtils.getFactoredHeight(lineSize3);
                    canvas.DrawText(BottomText, Xc, Yc + progressUtils.getFactoredHeight(lineHeight3), skPaint);
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }
        }
    }
}
