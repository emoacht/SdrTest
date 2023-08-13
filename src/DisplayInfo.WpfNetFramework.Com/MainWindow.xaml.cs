using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Windows.Graphics.Display;

namespace DisplayInfo.WpfNetFramework.Com;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
	}

	public bool IsWindow
	{
		get { return (bool)GetValue(IsWindowProperty); }
		set { SetValue(IsWindowProperty, value); }
	}
	public static readonly DependencyProperty IsWindowProperty =
		DependencyProperty.Register("IsWindow", typeof(bool), typeof(MainWindow), new PropertyMetadata(true));

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

	private DisplayInformation _monitorDisplayInfo;
	private DisplayInformation _windowDisplayInfo;

	private void Start_Click(object sender, RoutedEventArgs e)
	{
		this.SelectPanel.IsEnabled = false;
		this.StartPanel.Visibility = Visibility.Collapsed;
		Start();
	}

	private void Start()
	{
		var controller = _queueHelper.EnsureDispatcherQueueController();

		var windowHandle = new WindowInteropHelper(this).Handle;
		var monitorHandle = InteropHelper.MonitorFromWindow(windowHandle, InteropHelper.MONITOR_DEFAULTTO.MONITOR_DEFAULTTOPRIMARY);

		Task.Run(() =>
		{
			controller.DispatcherQueue.TryEnqueue(() =>
			{
				var (success, displayInfo) = InteropHelper.GetForMonitor(monitorHandle);
				if (success)
				{
					var colorInfo = displayInfo.GetAdvancedColorInfo();
					Trace.WriteLine($"COM Interop Monitor SDR: {colorInfo.SdrWhiteLevelInNits} Min: {colorInfo.MinLuminanceInNits} Max: {colorInfo.MaxLuminanceInNits} ColorKind: {colorInfo.CurrentAdvancedColorKind}");
					Reflect(colorInfo, IsWindow is false);

					_monitorDisplayInfo = displayInfo;
					_monitorDisplayInfo.AdvancedColorInfoChanged += OnMonitorAdvancedColorInfoChanged;
				}
			});
		});

		var (success, displayInfo) = InteropHelper.GetForWindow(windowHandle);
		if (success)
		{
			var colorInfo = displayInfo.GetAdvancedColorInfo();
			Trace.WriteLine($"COM Interop Window SDR: {colorInfo.SdrWhiteLevelInNits} Min: {colorInfo.MinLuminanceInNits} Max: {colorInfo.MaxLuminanceInNits} ColorKind: {colorInfo.CurrentAdvancedColorKind}");
			Reflect(colorInfo, IsWindow is true);

			_windowDisplayInfo = displayInfo;
			_windowDisplayInfo.AdvancedColorInfoChanged += OnWindowAdvancedColorInfoChanged;
		}
	}

	private void OnMonitorAdvancedColorInfoChanged(DisplayInformation sender, object args)
	{
		var colorInfo = sender.GetAdvancedColorInfo();
		Trace.WriteLine($"Monitor SDR: {colorInfo.SdrWhiteLevelInNits} ColorKind: {colorInfo.CurrentAdvancedColorKind}");
		Reflect(colorInfo, IsWindow is false);
	}

	private void OnWindowAdvancedColorInfoChanged(DisplayInformation sender, object args)
	{
		var colorInfo = sender.GetAdvancedColorInfo();
		Trace.WriteLine($"Window SDR: {colorInfo.SdrWhiteLevelInNits} ColorKind: {colorInfo.CurrentAdvancedColorKind}");
		Reflect(colorInfo, IsWindow is true);
	}

	private void Reflect(AdvancedColorInfo colorInfo, bool isMatch)
	{
		if (!isMatch)
			return;

		SdrWhiteLevel = colorInfo.SdrWhiteLevelInNits;
		MinLuminance = colorInfo.MinLuminanceInNits;
		MaxLuminance = colorInfo.MaxLuminanceInNits;
		ColorKind = colorInfo.CurrentAdvancedColorKind.ToString();
	}

	protected override void OnClosed(EventArgs e)
	{
		if (_monitorDisplayInfo is not null)
		{
			_monitorDisplayInfo.AdvancedColorInfoChanged -= OnMonitorAdvancedColorInfoChanged;
			_monitorDisplayInfo = null;
		}
		if (_windowDisplayInfo is not null)
		{
			_windowDisplayInfo.AdvancedColorInfoChanged -= OnWindowAdvancedColorInfoChanged;
			_windowDisplayInfo = null;
		}
		base.OnClosed(e);
	}
}
