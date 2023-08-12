using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace DisplayConfig.Wpf;

public static class DisplayHelper
{
	[DllImport("User32.dll")]
	private static extern int GetDisplayConfigBufferSizes(
		QUERY_DEVICE_CONFIG_FLAGS Flags,
		out uint numPathArrayElements,
		out uint numModeInfoArrayElements);

	[DllImport("User32.dll", CharSet = CharSet.Unicode)]
	private static extern int QueryDisplayConfig(
		QUERY_DEVICE_CONFIG_FLAGS Flags,
		ref uint numPathArrayElements,
		[Out] DISPLAYCONFIG_PATH_INFO[] pathArray,
		ref uint numModeInfoArrayElements,
		[Out] DISPLAYCONFIG_MODE_INFO[] modeInfoArray,
		IntPtr currentTopologyId);

	private enum QUERY_DEVICE_CONFIG_FLAGS : uint
	{
		QDC_ALL_PATHS = 0x00000001,
		QDC_ONLY_ACTIVE_PATHS = 0x00000002,
		QDC_DATABASE_CURRENT = 0x00000004
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct DISPLAYCONFIG_PATH_INFO
	{
		public DISPLAYCONFIG_PATH_SOURCE_INFO sourceInfo;
		public DISPLAYCONFIG_PATH_TARGET_INFO targetInfo;
		public uint flags;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct DISPLAYCONFIG_PATH_SOURCE_INFO
	{
		public LUID adapterId;
		public uint id;
		public uint modeInfoIdx; // Union
		public uint statusFlags;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct LUID
	{
		public uint LowPart;
		public int HighPart;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct DISPLAYCONFIG_PATH_TARGET_INFO
	{
		public LUID adapterId;
		public uint id;
		public uint modeInfoIdx; // Union
		public DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY outputTechnology;
		public DISPLAYCONFIG_ROTATION rotation;
		public DISPLAYCONFIG_SCALING scaling;
		public DISPLAYCONFIG_RATIONAL refreshRate;
		public DISPLAYCONFIG_SCANLINE_ORDERING scanLineOrdering;

		[MarshalAs(UnmanagedType.Bool)]
		public bool targetAvailable;

		public uint statusFlags;
	}

	public enum DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY : uint
	{
		DISPLAYCONFIG_OUTPUT_TECHNOLOGY_OTHER = 0xFFFFFFFF,
		DISPLAYCONFIG_OUTPUT_TECHNOLOGY_HD15 = 0,
		DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SVIDEO = 1,
		DISPLAYCONFIG_OUTPUT_TECHNOLOGY_COMPOSITE_VIDEO = 2,
		DISPLAYCONFIG_OUTPUT_TECHNOLOGY_COMPONENT_VIDEO = 3,
		DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DVI = 4,
		DISPLAYCONFIG_OUTPUT_TECHNOLOGY_HDMI = 5,
		DISPLAYCONFIG_OUTPUT_TECHNOLOGY_LVDS = 6,
		DISPLAYCONFIG_OUTPUT_TECHNOLOGY_D_JPN = 8,
		DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SDI = 9,
		DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EXTERNAL = 10,
		DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EMBEDDED = 11,
		DISPLAYCONFIG_OUTPUT_TECHNOLOGY_UDI_EXTERNAL = 12,
		DISPLAYCONFIG_OUTPUT_TECHNOLOGY_UDI_EMBEDDED = 13,
		DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SDTVDONGLE = 14,
		DISPLAYCONFIG_OUTPUT_TECHNOLOGY_MIRACAST = 15,
		DISPLAYCONFIG_OUTPUT_TECHNOLOGY_INTERNAL = 0x80000000,
		DISPLAYCONFIG_OUTPUT_TECHNOLOGY_FORCE_UINT32 = 0xFFFFFFFF
	}

	public enum DISPLAYCONFIG_ROTATION : uint
	{
		DISPLAYCONFIG_ROTATION_IDENTITY = 1,
		DISPLAYCONFIG_ROTATION_ROTATE90 = 2,
		DISPLAYCONFIG_ROTATION_ROTATE180 = 3,
		DISPLAYCONFIG_ROTATION_ROTATE270 = 4,
		DISPLAYCONFIG_ROTATION_FORCE_UINT32 = 0xFFFFFFFF
	}

	public enum DISPLAYCONFIG_SCALING : uint
	{
		DISPLAYCONFIG_SCALING_IDENTITY = 1,
		DISPLAYCONFIG_SCALING_CENTERED = 2,
		DISPLAYCONFIG_SCALING_STRETCHED = 3,
		DISPLAYCONFIG_SCALING_ASPECTRATIOCENTEREDMAX = 4,
		DISPLAYCONFIG_SCALING_CUSTOM = 5,
		DISPLAYCONFIG_SCALING_PREFERRED = 128,
		DISPLAYCONFIG_SCALING_FORCE_UINT32 = 0xFFFFFFFF
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct DISPLAYCONFIG_RATIONAL
	{
		public uint Numerator;
		public uint Denominator;
	}

	public enum DISPLAYCONFIG_SCANLINE_ORDERING : uint
	{
		DISPLAYCONFIG_SCANLINE_ORDERING_UNSPECIFIED = 0,
		DISPLAYCONFIG_SCANLINE_ORDERING_PROGRESSIVE = 1,
		DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED = 2,
		DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_UPPERFIELDFIRST = DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED,
		DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_LOWERFIELDFIRST = 3,
		DISPLAYCONFIG_SCANLINE_ORDERING_FORCE_UINT32 = 0xFFFFFFFF
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct DISPLAYCONFIG_MODE_INFO
	{
		public DISPLAYCONFIG_MODE_INFO_TYPE infoType;
		public uint id;
		public LUID adapterId;
		public DISPLAYCONFIG_MODE_INFO_UNION modeInfo;
	}

	public enum DISPLAYCONFIG_MODE_INFO_TYPE : uint
	{
		DISPLAYCONFIG_MODE_INFO_TYPE_SOURCE = 1,
		DISPLAYCONFIG_MODE_INFO_TYPE_TARGET = 2,
		DISPLAYCONFIG_MODE_INFO_TYPE_FORCE_UINT32 = 0xFFFFFFFF
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct DISPLAYCONFIG_MODE_INFO_UNION
	{
		[FieldOffset(0)]
		public DISPLAYCONFIG_TARGET_MODE targetMode;

		[FieldOffset(0)]
		public DISPLAYCONFIG_SOURCE_MODE sourceMode;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct DISPLAYCONFIG_TARGET_MODE
	{
		public DISPLAYCONFIG_VIDEO_SIGNAL_INFO targetVideoSignalInfo;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct DISPLAYCONFIG_VIDEO_SIGNAL_INFO
	{
		public ulong pixelRate;
		public DISPLAYCONFIG_RATIONAL hSyncFreq;
		public DISPLAYCONFIG_RATIONAL vSyncFreq;
		public DISPLAYCONFIG_2DREGION activeSize;
		public DISPLAYCONFIG_2DREGION totalSize;
		public uint videoStandard;
		public DISPLAYCONFIG_SCANLINE_ORDERING scanLineOrdering;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct DISPLAYCONFIG_2DREGION
	{
		public uint cx;
		public uint cy;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct DISPLAYCONFIG_SOURCE_MODE
	{
		public uint width;
		public uint height;
		public DISPLAYCONFIG_PIXELFORMAT pixelFormat;
		public POINTL position;
	}

	public enum DISPLAYCONFIG_PIXELFORMAT : uint
	{
		DISPLAYCONFIG_PIXELFORMAT_8BPP = 1,
		DISPLAYCONFIG_PIXELFORMAT_16BPP = 2,
		DISPLAYCONFIG_PIXELFORMAT_24BPP = 3,
		DISPLAYCONFIG_PIXELFORMAT_32BPP = 4,
		DISPLAYCONFIG_PIXELFORMAT_NONGDI = 5,
		DISPLAYCONFIG_PIXELFORMAT_FORCE_UINT32 = 0xffffffff
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct POINTL
	{
		public int x;
		public int y;
	}

	[DllImport("User32.dll")]
	private static extern int DisplayConfigGetDeviceInfo(ref DISPLAYCONFIG_SDR_WHITE_LEVEL whiteLevel);

	[DllImport("User32.dll")]
	private static extern int DisplayConfigGetDeviceInfo(ref DISPLAYCONFIG_TARGET_DEVICE_NAME deviceName);

	[StructLayout(LayoutKind.Sequential)]
	public struct DISPLAYCONFIG_DEVICE_INFO_HEADER
	{
		public DISPLAYCONFIG_DEVICE_INFO_TYPE type;
		public uint size;
		public LUID adapterId;
		public uint id;
	}

	public enum DISPLAYCONFIG_DEVICE_INFO_TYPE : uint
	{
		DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME = 2,
		DISPLAYCONFIG_DEVICE_INFO_GET_SDR_WHITE_LEVEL = 11,
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct DISPLAYCONFIG_SDR_WHITE_LEVEL
	{
		public DISPLAYCONFIG_DEVICE_INFO_HEADER header;

		/// <summary>
		/// The monitor's current SDR white level, specified as a multiplier of 80 nits, multiplied by 1000.
		/// E.g. a value of 1000 would indicate that the SDR white level is 80 nits, while a value of 2000
		/// would indicate an SDR white level of 160 nits.
		/// </summary>
		public uint SDRWhiteLevel;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct DISPLAYCONFIG_TARGET_DEVICE_NAME
	{
		public DISPLAYCONFIG_DEVICE_INFO_HEADER header;

		public uint flags; // Union
		public DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY outputTechnology;
		public ushort edidManufactureId;
		public ushort edidProductCodeId;
		public uint connectorInstance;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		public string monitorFriendlyDeviceName;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string monitorDevicePath;
	}

	private const int ERROR_SUCCESS = 0;

	private static DISPLAYCONFIG_SDR_WHITE_LEVEL GetWhiteLevel(LUID adapterId, uint id)
	{
		var buffer = new DISPLAYCONFIG_SDR_WHITE_LEVEL
		{
			header = new DISPLAYCONFIG_DEVICE_INFO_HEADER
			{
				type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_SDR_WHITE_LEVEL,
				size = (uint)Marshal.SizeOf<DISPLAYCONFIG_SDR_WHITE_LEVEL>(),
				adapterId = adapterId,
				id = id
			}
		};

		int error = DisplayConfigGetDeviceInfo(ref buffer);
		if (error is not ERROR_SUCCESS)
			throw new Win32Exception(error);

		return buffer;
	}

	private static DISPLAYCONFIG_TARGET_DEVICE_NAME GetDeviceName(LUID adapterId, uint id)
	{
		var buffer = new DISPLAYCONFIG_TARGET_DEVICE_NAME
		{
			header = new DISPLAYCONFIG_DEVICE_INFO_HEADER
			{
				type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME,
				size = (uint)Marshal.SizeOf<DISPLAYCONFIG_TARGET_DEVICE_NAME>(),
				adapterId = adapterId,
				id = id
			}
		};

		int error = DisplayConfigGetDeviceInfo(ref buffer);
		if (error is not ERROR_SUCCESS)
			throw new Win32Exception(error);

		return buffer;
	}

	public static (string friendlyName, string devicePath, int whiteLevel)[] Check()
	{
		int error = GetDisplayConfigBufferSizes(
			QUERY_DEVICE_CONFIG_FLAGS.QDC_ONLY_ACTIVE_PATHS,
			out uint numPaths,
			out uint numModes);
		if (error is not ERROR_SUCCESS)
			throw new Win32Exception(error);

		var paths = new DISPLAYCONFIG_PATH_INFO[numPaths];
		var modes = new DISPLAYCONFIG_MODE_INFO[numModes];

		error = QueryDisplayConfig(
			QUERY_DEVICE_CONFIG_FLAGS.QDC_ONLY_ACTIVE_PATHS,
			ref numPaths,
			paths,
			ref numModes,
			modes,
			IntPtr.Zero);
		if (error is not ERROR_SUCCESS)
			throw new Win32Exception(error);

		var buffer = new List<(string, string, int)>();

		foreach (var path in paths)
		{
			var w = GetWhiteLevel(path.targetInfo.adapterId, path.targetInfo.id);
			var d = GetDeviceName(path.targetInfo.adapterId, path.targetInfo.id);
			Debug.WriteLine($"Path: {ConvertToNits(w.SDRWhiteLevel)} | {d.monitorFriendlyDeviceName} | {d.monitorDevicePath}");

			buffer.Add((d.monitorFriendlyDeviceName, d.monitorDevicePath, ConvertToNits(w.SDRWhiteLevel)));
		}

		foreach (var mode in modes.Where(x => x.infoType == DISPLAYCONFIG_MODE_INFO_TYPE.DISPLAYCONFIG_MODE_INFO_TYPE_TARGET))
		{
			var w = GetWhiteLevel(mode.adapterId, mode.id);
			var d = GetDeviceName(mode.adapterId, mode.id);
			Debug.WriteLine($"Mode: {ConvertToNits(w.SDRWhiteLevel)} | {d.monitorFriendlyDeviceName} | {d.monitorDevicePath}");
		}

		return buffer.ToArray();
	}

	internal static int ConvertToNits(uint source) => (int)((float)source / 1000 * 80);
}
