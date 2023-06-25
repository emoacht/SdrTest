using System;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DynamicRange.Uwp;

public sealed partial class MainPage : Page
{
	public static readonly Size DefaultSize = new(360, 240);

	public float WhiteLevel
	{
		get { return (float)GetValue(WhiteLevelProperty); }
		set { SetValue(WhiteLevelProperty, value); }
	}
	public static readonly DependencyProperty WhiteLevelProperty =
		DependencyProperty.Register("WhiteLevel", typeof(float), typeof(MainPage), new PropertyMetadata(0F));

	private readonly DisplayInformation _displayInformation;

	public MainPage()
	{
		this.InitializeComponent();

		ApplicationView.PreferredLaunchViewSize = DefaultSize;
		ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

		_displayInformation = DisplayInformation.GetForCurrentView();
		_displayInformation.AdvancedColorInfoChanged += OnAdvancedColorInfoChanged;

		OnAdvancedColorInfoChanged(_displayInformation, null);
	}

	private void OnAdvancedColorInfoChanged(DisplayInformation sender, object args)
	{
		WhiteLevel = sender.GetAdvancedColorInfo().SdrWhiteLevelInNits;
	}
}
