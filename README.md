# SDR Test

Follow up of [Brightness slider not working when Windows 11 HDR on](https://github.com/emoacht/Monitorian/issues/411) and SO question [Controlling SDR content brightness programmatically in Windows 11](https://stackoverflow.com/questions/74594751/controlling-sdr-content-brightness-programmatically-in-windows-11).

## IDisplayInformationStaticsInterop interface

[Windows.Graphics.Display.DisplayInformation](https://learn.microsoft.com/en-us/uwp/api/windows.graphics.display.displayinformation) of WinRT had been only usable within UWP until [Windows.Graphics.Display.IDisplayInformationStaticsInterop](https://learn.microsoft.com/en-us/windows/win32/api/windows.graphics.display.interop/nn-windows-graphics-display-interop-idisplayinformationstaticsinterop) interface introduced on Windows 11 10.0.22621.0. This interface has `GetForWindow` and `GetForMonitor` methods for C++/WinRT to obtain `DisplayInformation`.

This functionality is accessible from WPF (.NET 6.0) or WinUI as well through `Windows.Graphics.Display.DisplayInformationInterop` which has `GetForWindow` and `GetForMonitor` methods and was introduced on Windows 11 10.0.22621.0 and .NET 6.0, as indicated in [Available via Target Framework Moniker](https://learn.microsoft.com/en-us/windows/apps/desktop/modernize/winrt-com-interop-csharp#available-via-target-framework-moniker).

For WPF (.NET Framework), no direct way to get `DisplayInformation` is provided but it can be obtained from C++/WinRT Runtime Component library or using COM interop.

### C++/WinRT Runtime Component

To obtain `DisplayInformation` from C++/WinRT Runtime Component library, see [Enhancing Non-packaged Desktop Apps using Windows Runtime Components](https://blogs.windows.com/windowsdeveloper/2019/04/30/enhancing-non-packaged-desktop-apps-using-windows-runtime-components/) and [C++/WinRTのブリッジを作ってC#から呼び出す方法](https://qiita.com/everylittle/items/cb95b24eb9462ddb3cfc). Please note that AppContainerApplication in the library must be made false to allow the library to access APIs other than WinRT.

```xml
<AppContainerApplication>false</AppContainerApplication>
```

As for the ways for .NET Framework to use WinRT, the latest one is to install Microsoft.Windows.SDK.Contracts package. See [Earlier versions of .NET: Install the Microsoft.Windows.SDK.Contracts NuGet package](https://learn.microsoft.com/en-us/windows/apps/desktop/modernize/desktop-to-uwp-enhance#earlier-versions-of-net-install-the-microsoftwindowssdkcontracts-nuget-package).

### COM Interop

To obtain `DisplayInformation` using COM interop, the way to obtain RadialController for Surface Dial can be applied. See [Radial Controller sample](https://github.com/Microsoft/Windows-classic-samples/tree/main/Samples/RadialController) and [デスクトップ アプリを Surface Dial に対応させる](http://grabacr.net/archives/7141).

```csharp
var factory = (IDisplayInformationStaticsInterop)WindowsRuntimeMarshal.GetActivationFactory(typeof(DisplayInformation));
var iid = typeof(DisplayInformation).GetInterface("IDisplayInformation").GUID;
factory.GetForMonitor(monitorHandle, ref iid, out DisplayInformation displayInfo);
return displayInfo;
```

### Windows.System.DispatcherQueue

For `GetForWindow` method and `AdvancedColorInfoChanged` or other events, Windows.System.DispatcherQueue must be running. See [IDisplayInformationStaticsInterop::GetForWindow method](https://learn.microsoft.com/en-us/windows/win32/api/windows.graphics.display.interop/nf-windows-graphics-display-interop-idisplayinformationstaticsinterop-getforwindow) and [Windows App SDK 1.1 Preview 3 で追加された Desktop Acrylic と Mica のサポートを試した](https://blog.shibayan.jp/entry/20220510/1652191991).

## DWM

Result of Dumpbin dose not include the signature of the function in question.

```
> dumpbin /exports c:\Windows\System32\dwmapi.dll
Microsoft (R) COFF/PE Dumper Version 14.36.32534.0
Copyright (C) Microsoft Corporation.  All rights reserved.


Dump of file c:\Windows\System32\dwmapi.dll

File Type: DLL

  Section contains the following exports for dwmapi.dll

    00000000 characteristics
    7ECC0A11 time date stamp
        0.00 version
         100 ordinal base
          99 number of functions
          44 number of names

    ordinal hint RVA      name

        111    0 00017DF0 DllCanUnloadNow
        115    1 00017E20 DllGetClassObject
        116    2 00013160 DwmAttachMilContent
        117    3 00002110 DwmDefWindowProc
        118    4 00013160 DwmDetachMilContent
        119    5 00007480 DwmEnableBlurBehindWindow
        102    6 000128D0 DwmEnableComposition
        120    7 00002880 DwmEnableMMCSS
        121    8 00002CA0 DwmExtendFrameIntoClientArea
        122    9 00003860 DwmFlush
        123    A 00012440 DwmGetColorizationColor
        125    B 00002930 DwmGetCompositionTimingInfo
        126    C 00013160 DwmGetGraphicsStreamClient
        129    D 00013160 DwmGetGraphicsStreamTransformHint
        130    E 00013170 DwmGetTransportAttributes
        133    F 000111C0 DwmGetUnmetTabRequirements
        134   10 00004990 DwmGetWindowAttribute
        149   11 00001DC0 DwmInvalidateIconicBitmaps
        184   12 00004C90 DwmIsCompositionEnabled
        185   13 00012630 DwmModifyPreviousDxFrameDuration
        186   14 00014130 DwmQueryThumbnailSourceSize
        187   15 00014270 DwmRegisterThumbnail
        188   16 000135D0 DwmRenderGesture
        189   17 00012630 DwmSetDxFrameDuration
        190   18 00014340 DwmSetIconicLivePreviewBitmap
        191   19 00014630 DwmSetIconicThumbnail
        192   1A 00012630 DwmSetPresentParameters
        193   1B 00003540 DwmSetWindowAttribute
        194   1C 000137A0 DwmShowContact
        195   1D 00013880 DwmTetherContact
        156   1E 00013990 DwmTetherTextContact
        196   1F 00015DF0 DwmTransitionOwnedWindow
        197   20 00003910 DwmUnregisterThumbnail
        198   21 000149E0 DwmUpdateThumbnailProperties
        136   22 00006290 DwmpAllocateSecurityDescriptor
        100   23 00005F40 DwmpDxGetWindowSharedSurface
        101   24 00011FA0 DwmpDxUpdateWindowSharedSurface
        128   25 00001D10 DwmpDxgiIsThreadDesktopComposited
        143   26 00011FA0 DwmpEnableDDASupport
        137   27 00006440 DwmpFreeSecurityDescriptor
        127   28 000042B0 DwmpGetColorizationParameters
        135   29 00013B70 DwmpRenderFlick
        131   2A 00002750 DwmpSetColorizationParameters
        183   2B 00009FC0 DwmpUpdateProxyWindowForCapture
        103      00012D60 [NONAME]
        104      00012630 [NONAME]
        105      00011FA0 [NONAME]
        106      00012BD0 [NONAME]
        107      00012500 [NONAME]
        108      00012630 [NONAME]
        109      00013160 [NONAME]
        110      00013160 [NONAME]
        112      00013160 [NONAME]
        113      000121B0 [NONAME]
        114      00014D50 [NONAME]
        124      00014E30 [NONAME]
        132      00002970 [NONAME]
        138      00001FD0 [NONAME]
        139      00005A10 [NONAME]
        140      000020B0 [NONAME]
        141      00005A40 [NONAME]
        142      000157E0 [NONAME]
        144      00015EE0 [NONAME]
        145      00015ED0 [NONAME]
        146      00013190 [NONAME]
        147      00005020 [NONAME]
        148      00001FE0 [NONAME]
        150      00016600 [NONAME]
        151      00016510 [NONAME]
        152      000166E0 [NONAME]
        153      000167C0 [NONAME]
        154      00016AB0 [NONAME]
        155      000168E0 [NONAME]
        157      00012C80 [NONAME]
        158      00013020 [NONAME]
        159      00001D40 [NONAME]
        160      000151B0 [NONAME]
        161      00007AA0 [NONAME]
        162      00006200 [NONAME]
        163      00014B70 [NONAME]
        164      00015890 [NONAME]
        165      00013360 [NONAME]
        166      00014F80 [NONAME]
        167      00013280 [NONAME]
        168      00017620 [NONAME]
        169      00004320 [NONAME]
        170      00007970 [NONAME]
        171      00012DE0 [NONAME]
        172      00015B70 [NONAME]
        173      00012AF0 [NONAME]
        174      00012930 [NONAME]
        175      000130D0 [NONAME]
        176      00012F90 [NONAME]
        177      00012E70 [NONAME]
        178      000169F0 [NONAME]
        179      00015C30 [NONAME]
        180      00001ED0 [NONAME]
        181      00012A10 [NONAME]
        182      00012F00 [NONAME]

  Summary

        4000 .data
        1000 .didat
        2000 .pdata
        8000 .rdata
        1000 .reloc
        5000 .rsrc
       19000 .text
```