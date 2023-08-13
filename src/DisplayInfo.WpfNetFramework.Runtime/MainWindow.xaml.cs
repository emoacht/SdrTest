using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using Windows.Graphics.Display;

namespace DisplayInfo.WpfNetFramework.Runtime;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
		this.Loaded += OnLoaded;
	}

	public float SdrWhiteLevel
	{
		get { return (float)GetValue(SdrWhiteLevelProperty); }
		set { SetValue(SdrWhiteLevelProperty, value); }
	}
	public static readonly DependencyProperty SdrWhiteLevelProperty =
		DependencyProperty.Register("SdrWhiteLevel", typeof(float), typeof(MainWindow), new PropertyMetadata(0F));

	public float MinLuminance
	{
		get { return (float)GetValue(MinLuminanceProperty); }
		set { SetValue(MinLuminanceProperty, value); }
	}
	public static readonly DependencyProperty MinLuminanceProperty =
		DependencyProperty.Register("MinLuminance", typeof(float), typeof(MainWindow), new PropertyMetadata(0F));

	public float MaxLuminance
	{
		get { return (float)GetValue(MaxLuminanceProperty); }
		set { SetValue(MaxLuminanceProperty, value); }
	}
	public static readonly DependencyProperty MaxLuminanceProperty =
		DependencyProperty.Register("MaxLuminance", typeof(float), typeof(MainWindow), new PropertyMetadata(0F));

	public string ColorKind
	{
		get { return (string)GetValue(ColorKindProperty); }
		set { SetValue(ColorKindProperty, value); }
	}
	public static readonly DependencyProperty ColorKindProperty =
		DependencyProperty.Register("ColorKind", typeof(string), typeof(MainWindow), new PropertyMetadata(default(string)));

	private readonly WindowsSystemDispatcherQueueHelper _queueHelper = new();

	private DisplayInformation _displayInfo;

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		_queueHelper.EnsureDispatcherQueueController();

		var windowHandle = new WindowInteropHelper(this).Handle;
		var monitorHandle = InteropHelper.MonitorFromWindow(windowHandle, InteropHelper.MONITOR_DEFAULTTO.MONITOR_DEFAULTTOPRIMARY);

		var cls = new DisplayInfo_CppWinrt_Runtime.Class();

		_displayInfo = cls.GetForMonitor(monitorHandle.ToInt64());
		var colorInfo = _displayInfo.GetAdvancedColorInfo();
		Trace.WriteLine($"CppWinrt Lib Monitor SDR: {colorInfo.SdrWhiteLevelInNits} Min: {colorInfo.MinLuminanceInNits} Max: {colorInfo.MaxLuminanceInNits} ColorKind: {colorInfo.CurrentAdvancedColorKind}");

		_displayInfo = cls.GetForWindow(windowHandle.ToInt64());
		colorInfo = _displayInfo.GetAdvancedColorInfo();
		Trace.WriteLine($"CppWinrt Lib Window SDR: {colorInfo.SdrWhiteLevelInNits} Min: {colorInfo.MinLuminanceInNits} Max: {colorInfo.MaxLuminanceInNits} ColorKind: {colorInfo.CurrentAdvancedColorKind}");

		_displayInfo.AdvancedColorInfoChanged += OnAdvancedColorInfoChanged;

		OnAdvancedColorInfoChanged(_displayInfo, EventArgs.Empty);
	}

	private void OnAdvancedColorInfoChanged(DisplayInformation sender, object args)
	{
		var colorInfo = sender.GetAdvancedColorInfo();
		SdrWhiteLevel = colorInfo.SdrWhiteLevelInNits;
		MinLuminance = colorInfo.MinLuminanceInNits;
		MaxLuminance = colorInfo.MaxLuminanceInNits;
		ColorKind = colorInfo.CurrentAdvancedColorKind.ToString();
	}

	protected override void OnClosed(EventArgs e)
	{
		if (_displayInfo is not null)
		{
			_displayInfo.AdvancedColorInfoChanged -= OnAdvancedColorInfoChanged;
			_displayInfo = null;
		}
		base.OnClosed(e);
	}
}
