using CommunityToolkit.Mvvm.ComponentModel;

namespace dndspellviewercrossplatform.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _greeting = "Welcome to Avalonia!";
}
