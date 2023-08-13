#include "pch.h"

#include "winrt/base.h"
#include "winrt/Windows.Graphics.Display.h"
#include <windows.graphics.display.interop.h>

using namespace winrt;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;

#include "Class.h"
#include "Class.g.cpp"

namespace winrt::DisplayInfo_CppWinrt_Runtime::implementation
{
	DisplayInformation Class::GetForWindow(const int64_t window)
	{
		// This method only works when Windows.System.DispatcherQueue is running.

		HWND hwnd = (HWND)LongToPtr((long)window);

		auto display_info_factory = get_activation_factory<DisplayInformation, IDisplayInformationStaticsInterop>();
		DisplayInformation display_info = nullptr;
		HRESULT result = display_info_factory->GetForWindow(hwnd, guid_of<DisplayInformation>(), put_abi(display_info));
		check_hresult(result);
		return display_info;
	}

	DisplayInformation Class::GetForMonitor(const int64_t monitor)
	{
		HMONITOR hmonitor = (HMONITOR)LongToPtr((long)monitor);

		auto display_info_factory = get_activation_factory<DisplayInformation, IDisplayInformationStaticsInterop>();
		DisplayInformation display_info = nullptr;
		HRESULT result = display_info_factory->GetForMonitor(hmonitor, guid_of<DisplayInformation>(), put_abi(display_info));
		check_hresult(result);
		return display_info;
	}
}
