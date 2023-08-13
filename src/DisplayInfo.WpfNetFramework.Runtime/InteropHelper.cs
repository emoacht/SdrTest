using System;
using System.Runtime.InteropServices;

namespace DisplayInfo;

internal static class InteropHelper
{
	[DllImport("User32.dll")]
	public static extern IntPtr MonitorFromWindow(
		IntPtr hwnd,
		MONITOR_DEFAULTTO dwFlags);

	public enum MONITOR_DEFAULTTO : uint
	{
		MONITOR_DEFAULTTONULL = 0x00000000,
		MONITOR_DEFAULTTOPRIMARY = 0x00000001,
		MONITOR_DEFAULTTONEAREST = 0x00000002,
	}
}
