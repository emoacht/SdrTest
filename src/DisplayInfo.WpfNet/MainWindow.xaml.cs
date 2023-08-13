using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using Windows.Graphics.Display;

namespace DisplayInfo.WpfNet;

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

	private readonly WindowsSystemDispatcherQueueHelper _queueHelper = new();

	private DisplayInformation? _displayInfo;

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		_queueHelper.EnsureDispatcherQueueController();

		var windowHandle = new WindowInteropHelper(this).Handle;
		var monitorHandle = InteropHelper.MonitorFromWindow(windowHandle, InteropHelper.MONITOR_DEFAULTTO.MONITOR_DEFAULTTOPRIMARY);

		_displayInfo = DisplayInformationInterop.GetForMonitor(monitorHandle);
		_displayInfo.AdvancedColorInfoChanged += OnAdvancedColorInfoChanged;

		OnAdvancedColorInfoChanged(_displayInfo, EventArgs.Empty);

		var displayInfo = DisplayInformationInterop.GetForWindow(windowHandle);
		Trace.WriteLine($"Window SDR: {displayInfo.GetAdvancedColorInfo().SdrWhiteLevelInNits}");
	}

	private void OnAdvancedColorInfoChanged(DisplayInformation sender, object args)
	{
		SdrWhiteLevel = sender.GetAdvancedColorInfo().SdrWhiteLevelInNits;
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
