using System.Runtime.InteropServices;
using Windows.System;

namespace DisplayInfo;

internal class WindowsSystemDispatcherQueueHelper
{
	// DispatcherQueue.h
	[DllImport("CoreMessaging.dll")]
	private static extern int CreateDispatcherQueueController(
		[In] DispatcherQueueOptions options,
		ref DispatcherQueueController dispatcherQueueController);

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

	private DispatcherQueueController _dispatcherQueueController = null;

	public DispatcherQueueController EnsureDispatcherQueueController()
	{
		if (DispatcherQueue.GetForCurrentThread() is not null)
			return null;

		if (_dispatcherQueueController is null)
		{
			var options = new DispatcherQueueOptions
			{
				dwSize = (uint)Marshal.SizeOf<DispatcherQueueOptions>(),
				threadType = DISPATCHERQUEUE_THREAD_TYPE.DQTYPE_THREAD_CURRENT,
				apartmentType = DISPATCHERQUEUE_THREAD_APARTMENTTYPE.DQTAT_COM_STA
			};
			CreateDispatcherQueueController(options, ref _dispatcherQueueController);
		}
		return _dispatcherQueueController;
	}
}
