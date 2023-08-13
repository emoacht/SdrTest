using System.Runtime.InteropServices;
using Windows.System;

namespace DisplayInfo;

internal class WindowsSystemDispatcherQueueHelper
{
	// DispatcherQueue.h
	[DllImport("CoreMessaging.dll")]
	private static extern int CreateDispatcherQueueController(
		[In] DispatcherQueueOptions options,
		[In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object dispatcherQueueController);

	[StructLayout(LayoutKind.Sequential)]
	private struct DispatcherQueueOptions
	{
		public uint dwSize;
		public DISPATCHERQUEUE_THREAD_TYPE threadType;
		public DISPATCHERQUEUE_THREAD_APARTMENTTYPE apartmentType;
	}

	private enum DISPATCHERQUEUE_THREAD_TYPE
	{
		DQTYPE_THREAD_DEDICATED = 1,
		DQTYPE_THREAD_CURRENT = 2,
	};

	private enum DISPATCHERQUEUE_THREAD_APARTMENTTYPE
	{
		DQTAT_COM_NONE = 0,
		DQTAT_COM_ASTA = 1,
		DQTAT_COM_STA = 2
	};

	private object? _dispatcherQueueController = null;

	public void EnsureDispatcherQueueController()
	{
		if (DispatcherQueue.GetForCurrentThread() is not null)
			return;

		if (_dispatcherQueueController is null)
		{
			DispatcherQueueOptions options;
			options.dwSize = (uint)Marshal.SizeOf<DispatcherQueueOptions>();
			options.threadType = DISPATCHERQUEUE_THREAD_TYPE.DQTYPE_THREAD_CURRENT;
			options.apartmentType = DISPATCHERQUEUE_THREAD_APARTMENTTYPE.DQTAT_COM_STA;

			CreateDispatcherQueueController(options, ref _dispatcherQueueController!);
		}
	}
}
