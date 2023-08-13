using System;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DisplayInfo.Uwp;

public sealed partial class MainPage : Page
{
	public static readonly Size DefaultSize = new(360, 240);

	public float SdrWhiteLevel
	{
		get { return (float)GetValue(SdrWhiteLevelProperty); }
		set { SetValue(SdrWhiteLevelProperty, value); }
	}
	public static readonly DependencyProperty SdrWhiteLevelProperty =
		DependencyProperty.Register("SdrWhiteLevel", typeof(float), typeof(MainPage), new PropertyMetadata(0F));

	public float MinLuminance
	{
		get { return (float)GetValue(MinLuminanceProperty); }
		set { SetValue(MinLuminanceProperty, value); }
	}
	public static readonly DependencyProperty MinLuminanceProperty =
		DependencyProperty.Register("MinLuminance", typeof(float), typeof(MainPage), new PropertyMetadata(0F));

	public float MaxLuminance
	{
		get { return (float)GetValue(MaxLuminanceProperty); }
		set { SetValue(MaxLuminanceProperty, value); }
	}
	public static readonly DependencyProperty MaxLuminanceProperty =
		DependencyProperty.Register("MaxLuminance", typeof(float), typeof(MainPage), new PropertyMetadata(0F));

	public string ColorKind
	{
		get { return (string)GetValue(ColorKindProperty); }
		set { SetValue(ColorKindProperty, value); }
	}
	public static readonly DependencyProperty ColorKindProperty =
		DependencyProperty.Register("ColorKind", typeof(string), typeof(MainPage), new PropertyMetadata(default(string)));

	private readonly DisplayInformation _displayInfo;

	public MainPage()
	{
		this.InitializeComponent();

		ApplicationView.PreferredLaunchViewSize = DefaultSize;
		ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

		_displayInfo = DisplayInformation.GetForCurrentView();
		_displayInfo.AdvancedColorInfoChanged += OnAdvancedColorInfoChanged;

		OnAdvancedColorInfoChanged(_displayInfo, null);
	}

	private void OnAdvancedColorInfoChanged(DisplayInformation sender, object args)
	{
		var colorInfo = sender.GetAdvancedColorInfo();
		SdrWhiteLevel = colorInfo.SdrWhiteLevelInNits;
		MinLuminance = colorInfo.MinLuminanceInNits;
		MaxLuminance = colorInfo.MaxLuminanceInNits;
		ColorKind = colorInfo.CurrentAdvancedColorKind.ToString();
	}
}
