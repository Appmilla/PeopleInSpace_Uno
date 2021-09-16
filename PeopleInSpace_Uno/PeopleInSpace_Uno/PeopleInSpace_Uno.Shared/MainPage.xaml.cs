using PeopleInSpace_Uno.SharedFeatures.ViewModels;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PeopleInSpace_Uno
{
    public abstract partial class MainPageBase : ReactiveUI.Uno.ReactivePage<MainPageViewModel>
    {
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : MainPageBase
    {
        public MainPageViewModel ViewModel { get; set; }

        public MainPage()
        {
            DataContext = ViewModel = Locator.Current.GetService<MainPageViewModel>();

            this.InitializeComponent();

            //Observable.Return(Unit.Default).InvokeCommand(ViewModel.RefreshCommand);
            //ViewModel.RefreshCommand.Execute();
        }

        /*
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.RefreshCommand.Execute();

            base.OnNavigatedTo(e);
        }
        */
    }
}
