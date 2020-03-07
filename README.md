# ItamarD.Xamarin.Forms.RadialProgress

Radial Progress Indicator in Xamarin Forms based on @KPS250's component, NuGet package.

![Banner](https://github.com/KPS25/RadialProgress/blob/master/Screenshot_Banner.jpg)

[KPS'S Presentation Link](https://drive.google.com/open?id=1pAOznBc0N3W4dXJ_P3A9DckqLqq_UuRwKzAKHSYZ6fE)
[KPS'S Original Repo](https://github.com/KPS250/Xamarin-Forms-RadialProgress)

## Installation

Just add the NuGet package and you're good to go!

```bash
dotnet add package ItamarD.Xamarin.Forms.RadialProgress
```

## Usage

This is a Xamarin.Forms component, and is compatible with both iOS and Android.

**XAML**
Add the namespace

```csharp
 xmlns:radial="clr-namespace:RadialProgress;assembly=RadialProgressGauge"
```

then you can use the `radial:gauge` component!

### Bindable Properties

* `CurrentValue`

Value of the gauge

* `MaxValue`

Maximum value of the gauge

* `HasAnimation`

When `true`, displays an animation on `CurrentValue` change

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