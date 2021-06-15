using System;
using System.Reactive.PlatformServices;
using Uno.UI;
using Windows.UI.Xaml;

namespace PeopleInSpace_Uno.Wasm
{
    public class Program
    {
        private static App _app;

        /*
        static int Main(string[] args)
        {
            Windows.UI.Xaml.Application.Start(_ => _app = new App());

            return 0;
        }
        */
        static int Main(string[] args)

        {

#pragma warning disable CS0618 // Type or member is obsolete

            PlatformEnlightenmentProvider.Current.EnableWasm();

#pragma warning restore CS0618 // Type or member is obsolete

            FeatureConfiguration.UIElement.AssignDOMXamlName = true;

            Windows.UI.Xaml.Application.Start(_ => _app = new App());

            return 0;

        }
    }
}
