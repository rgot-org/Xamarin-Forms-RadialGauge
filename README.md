# Xamarin-Forms-RadialGauge

Radial Gauge Indicator in Xamarin Forms based on [ItamarD.Xamarin.Forms.RadialProgress](https://github.com/doriitamar/Xamarin.Forms.RadialGauge)'s component, NuGet package.

![Banner](https://github.com/KPS25/RadialProgress/blob/master/Screenshot_Banner.jpg)

[KPS'S Presentation Link](https://drive.google.com/open?id=1pAOznBc0N3W4dXJ_P3A9DckqLqq_UuRwKzAKHSYZ6fE)
[KPS'S Original Repo](https://github.com/KPS250/Xamarin-Forms-RadialProgress)

## Installation

Just add the NuGet package and you're good to go!

```bash
dotnet add package org.rgot.RadialGauge
```
## Example
- Create  a Xamarin Forms solution. 
- Update Nuget Packages for entire solution. (Xamarin Forms > 5.0). 
- For Android project target framework 11. 
### MainPage.xaml
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:radial="clr-namespace:RadialGauge;assembly=RadialGauge"
             x:Class="test_radialGauge.MainPage">
    <StackLayout>
        <Frame BackgroundColor="#2196F3" Padding="24" CornerRadius="0">
            <Label Text="Test Gauge" HorizontalTextAlignment="Center" TextColor="White" FontSize="36"/>
        </Frame>
        <radial:Gauge x:Name="jauge" 
                      HorizontalOptions="CenterAndExpand"
                      WidthRequest="150"
                      HeightRequest="150"
                      MinValue="-20"
                      MaxValue="50"
                      CurrentValue="25"
                      UnitOfMeasurement="°C" 
                      BottomText="Sensor"
                      HasAnimation="False"
                      FromColor="#0000ff"
                      ToColor="Red"
                      ViaColor="Gold"
                      />
        <Slider Minimum="-20" Maximum="50" x:Name="slider" ValueChanged="slider_ValueChanged"/>
    </StackLayout>
</ContentPage>
```
### MainPage.xaml.cs
```csharp
using Xamarin.Forms;
namespace test_radialGauge
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        private void slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            jauge.CurrentValue = (int)slider.Value;
        }
    }
}
```

## Usage

This is a Xamarin.Forms component, and is compatible with both iOS and Android.

**XAML**
Add the namespace

```csharp
 xmlns:radial="clr-namespace:RadialGauge;assembly=RadialGauge"
```

then you can use the `radial:gauge` component!

### Bindable Properties

* `CurrentValue` : Value of the gauge

* `MaxValue` : Maximum value of the gauge
* `MinValue` : Minimum value of the gauge

* `HasAnimation` : When `true`, displays an animation on `CurrentValue` change

* `BackgroundColor`

* `FromColor`

Defaults to `Color.Red`
 
* `ToColor`

Defaults to `Color.Green`

* `ViaColor`

Defaults to `Color.Golden`

* `EmptyFillColor`

Color of the empty part of the gauge, Defaults to `Color.FromHex("#e0dfdf")`.

* `TextColor`

Defaults to `Color.FromHex("#676a69")`

* `UnitOfMeasurement`

Text to display below the number, defaults to `""`

* `BottomText`

Text to display below the unit, defaults to `""`

* `TextFont`
Defaults to `Arial`

### License 

Licensed under FreeBSD.

As with any open source project if you see improvements feel free to add your PR's