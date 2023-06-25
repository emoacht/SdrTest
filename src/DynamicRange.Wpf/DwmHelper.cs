using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace DynamicRange.Wpf;

public static class DwmHelper
{
	[DllImport("User32.dll")]
	private static extern IntPtr MonitorFromWindow(
		IntPtr hwnd,
		MONITOR_DEFAULTTO dwFlags);

	private enum MONITOR_DEFAULTTO : uint
	{
		MONITOR_DEFAULTTONULL = 0x00000000,
		MONITOR_DEFAULTTOPRIMARY = 0x00000001,
		MONITOR_DEFAULTTONEAREST = 0x00000002,
	}

	[DllImport("Dwmapi.dll", EntryPoint = "#171")]
	private static extern int DwmNoName(
		IntPtr hwnd,
		double brightness);

	private const int S_OK = 0;

	public static bool Set(Window window, double brightness)
	{
		var windowHandle = new WindowInteropHelper(window).Handle;
		var monitorHandle = MonitorFromWindow(windowHandle, MONITOR_DEFAULTTO.MONITOR_DEFAULTTOPRIMARY);
		if (monitorHandle == IntPtr.Zero)
			return false;

		var result = DwmNoName(monitorHandle, brightness);
		return (result is S_OK);
	}
}
