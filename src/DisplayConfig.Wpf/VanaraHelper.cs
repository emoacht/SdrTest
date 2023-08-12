using System.Diagnostics;
using System.Linq;
using Vanara.PInvoke;
using static Vanara.PInvoke.Gdi32;
using static Vanara.PInvoke.User32;

namespace DisplayConfig.Wpf;

public static class VanaraHelper
{
	public static void Check()
	{
		var result = QueryDisplayConfig(QDC.QDC_ONLY_ACTIVE_PATHS, out var paths, out var modes, out _);
		if (result != Win32Error.ERROR_SUCCESS)
			return;

		foreach (var path in paths)
		{
			var w = DisplayConfigGetDeviceInfo<DISPLAYCONFIG_SDR_WHITE_LEVEL>(path.targetInfo.adapterId, path.targetInfo.id);
			var d = DisplayConfigGetDeviceInfo<DISPLAYCONFIG_TARGET_DEVICE_NAME>(path.targetInfo.adapterId, path.targetInfo.id);
			Debug.WriteLine($"Path: {DisplayHelper.ConvertToNits(w.SDRWhiteLevel)} | {d.monitorFriendlyDeviceName} | {d.monitorDevicePath}");
		}

		foreach (var mode in modes.Where(x => x.infoType == DISPLAYCONFIG_MODE_INFO_TYPE.DISPLAYCONFIG_MODE_INFO_TYPE_TARGET))
		{
			var w = DisplayConfigGetDeviceInfo<DISPLAYCONFIG_SDR_WHITE_LEVEL>(mode.adapterId, mode.id);
			var d = DisplayConfigGetDeviceInfo<DISPLAYCONFIG_TARGET_DEVICE_NAME>(mode.adapterId, mode.id);
			Debug.WriteLine($"Mode: {DisplayHelper.ConvertToNits(w.SDRWhiteLevel)} | {d.monitorFriendlyDeviceName} | {d.monitorDevicePath}");
		}
	}
}
