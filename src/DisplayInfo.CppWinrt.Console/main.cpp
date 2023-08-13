#include "pch.h"

#include "winrt/base.h"
#include "winrt/Windows.Graphics.Display.h"
#include <windows.graphics.display.interop.h>
#include <iostream>

using namespace winrt;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;

int main()
{
	init_apartment();

	HMONITOR hmonitor = MonitorFromWindow(nullptr, MONITOR_DEFAULTTOPRIMARY);

	auto display_info_factory = get_activation_factory<DisplayInformation, IDisplayInformationStaticsInterop>();
	DisplayInformation display_info = nullptr;
	display_info_factory->GetForMonitor(hmonitor, guid_of<DisplayInformation>(), put_abi(display_info));

	AdvancedColorInfo advanced_color_info = display_info.GetAdvancedColorInfo();

	std::cout << "SDR While Level: " << advanced_color_info.SdrWhiteLevelInNits() << " nits" << std::endl;
	std::cout << "Min Luminance: " << advanced_color_info.MinLuminanceInNits() << " nits" << std::endl;
	std::cout << "Max Luminance: " << advanced_color_info.MaxLuminanceInNits() << " nits" << std::endl;
}
