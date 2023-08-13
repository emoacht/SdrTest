using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Display;

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

	private const int S_OK = 0;

	// windows.graphics.display.interop.h
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
	[Guid("7449121C-382B-4705-8DA7-A795BA482013")]
	public interface IDisplayInformationStaticsInterop
	{
		[PreserveSig]
		uint GetForWindow(IntPtr window, [In] ref Guid riid, out DisplayInformation info);

		[PreserveSig]
		uint GetForMonitor(IntPtr monitor, [In] ref Guid riid, out DisplayInformation info);
	}

	public static (bool, DisplayInformation) GetForWindow(IntPtr windowHandle)
	{
		// This method only works when Windows.System.DispatcherQueue is running.

		var factory = (IDisplayInformationStaticsInterop)WindowsRuntimeMarshal.GetActivationFactory(typeof(DisplayInformation));
		var iid = typeof(DisplayInformation).GetInterface("IDisplayInformation").GUID;
		var result = factory.GetForWindow(windowHandle, ref iid, out DisplayInformation displayInfo);
		return (result == S_OK, displayInfo);
	}

	public static (bool, DisplayInformation) GetForMonitor(IntPtr monitorHandle)
	{
		var factory = (IDisplayInformationStaticsInterop)WindowsRuntimeMarshal.GetActivationFactory(typeof(DisplayInformation));
		var iid = typeof(DisplayInformation).GetInterface("IDisplayInformation").GUID;
		var result = factory.GetForMonitor(monitorHandle, ref iid, out DisplayInformation displayInfo);
		return (result == S_OK, displayInfo);
	}
}
