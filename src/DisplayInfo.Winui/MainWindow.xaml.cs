using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.Graphics;
using Windows.Graphics.Display;

namespace DisplayInfo.Winui;

public sealed partial class MainWindow : Window
{
	internal readonly SizeInt32 WindowSize = new SizeInt32(800, 600);

	public MainWindow()
	{
		this.InitializeComponent();

		AppWindow.Resize(WindowSize);
		AppWindow.Title = "DisplayInfo.WinUI";
	}

	private void Check_Click(object sender, RoutedEventArgs e)
	{
		Result.Text = null;

		var areas = DisplayArea.FindAll();
		if (areas is { Count: > 0 })
		{
			for (int i = 0; i < areas.Count; i++)
			{
				var monitorHandle = Win32Interop.GetMonitorFromDisplayId(areas[i].DisplayId);
				var displayInfo = DisplayInformationInterop.GetForMonitor(monitorHandle);
				var colorInfo = displayInfo.GetAdvancedColorInfo();

				if (Result.Text.Length > 0)
				{
					Result.Text += "\r\n";
				}
				Result.Text += $"SDR: {colorInfo.SdrWhiteLevelInNits} ColorKind: {colorInfo.CurrentAdvancedColorKind}";
			}
		}
	}
}
