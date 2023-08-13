#pragma once

#include "Class.g.h"

namespace winrt::DisplayInfo_CppWinrt_Runtime::implementation
{
    struct Class : ClassT<Class>
    {
        Class() = default;

        DisplayInformation GetForWindow(int64_t window);
        DisplayInformation GetForMonitor(int64_t monitor);
    };
}

namespace winrt::DisplayInfo_CppWinrt_Runtime::factory_implementation
{
    struct Class : ClassT<Class, implementation::Class>
    {
    };
}
