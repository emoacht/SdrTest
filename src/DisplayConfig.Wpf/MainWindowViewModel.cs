using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DisplayConfig.Wpf;

public partial class MainWindowViewModel : ObservableObject
{
	public ObservableCollection<DisplayViewModel> Displays { get; } = new();

	public ICommand IncreaseCommand { get; }
	public ICommand RefreshCommand { get; }

	public MainWindowViewModel()
	{
		IncreaseCommand = new RelayCommand(Increase);
		RefreshCommand = new RelayCommand(Refresh);

		Refresh();
	}

	[ObservableProperty]
	private double _level = 0;

	public void Increase()
	{
		var buffer = Level switch
		{
			>= 10 => 1,
			_ => Level + 0.5
		};;

		var w = Application.Current.MainWindow;
		if (w is not null)
		{
			if (DwmHelper.Set(w, Level))
				Level = buffer;
		}
	}

	private void Refresh()
	{
		//VanaraHelper.Check();

		Displays.Clear();

		var displays = DisplayHelper.Check();
		foreach (var d in displays)
		{
			Displays.Add(new DisplayViewModel
			{
				FriendlyName = d.friendlyName,
				DevicePath = d.devicePath,
				WhiteLevel = d.whiteLevel
			});
		}
	}
}

public partial class DisplayViewModel : ObservableObject
{
	[ObservableProperty]
	private string? _friendlyName;

	[ObservableProperty]
	private string? _devicePath;

	[ObservableProperty]
	private int _whiteLevel;
}
