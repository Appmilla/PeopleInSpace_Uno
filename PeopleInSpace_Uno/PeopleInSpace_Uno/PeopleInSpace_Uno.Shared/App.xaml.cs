﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using PeopleInSpace_Uno.SharedFeatures.Apis;
using PeopleInSpace_Uno.SharedFeatures.Queries;
using PeopleInSpace_Uno.SharedFeatures.Reactive;
using PeopleInSpace_Uno.SharedFeatures.ViewModels;
using ReactiveUI;
using Refit;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using Splat.Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace PeopleInSpace_Uno
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
#if NET5_0 && WINDOWS
        private Window _window;

#else
        private Windows.UI.Xaml.Window _window;
#endif

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Initialize();

            InitializeLogging();

            this.InitializeComponent();


#if HAS_UNO || NETFX_CORE
            this.Suspending += OnSuspending;
#endif                      
        }

        public IServiceProvider ServiceProvider { get; private set; }

        void Initialize()
        {
            var host = Host
              .CreateDefaultBuilder()
              .ConfigureAppConfiguration((hostingContext, config) =>
              {
                  config.Properties.Clear();
                  config.Sources.Clear();
                  hostingContext.Properties.Clear();

                  //foreach (var fileProvider in config.Properties.Where(p => p.Value is PhysicalFileProvider).ToList())
                  //  config.Properties.Remove(fileProvider);

                  //IHostEnvironment hostingEnvironment = hostingContext.HostingEnvironment;
                  //config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddJsonFile("appsettings." + hostingEnvironment.EnvironmentName + ".json", optional: true, reloadOnChange: true);


                  //if (hostingEnvironment.IsDevelopment() && !string.IsNullOrEmpty(hostingEnvironment.ApplicationName))
                  //{
                  //  Assembly assembly = Assembly.Load(new AssemblyName(hostingEnvironment.ApplicationName));
                  //  if (assembly != null)
                  //  {
                  //    config.AddUserSecrets(assembly, optional: true);
                  //  }
                  //}
                  //config.AddEnvironmentVariables();          
              })
              .ConfigureServices(ConfigureServices)
              .ConfigureLogging(loggingBuilder =>
              {
                  // remove loggers incompatible with UWP
                  {
                      var eventLoggers = loggingBuilder.Services
                      .Where(l => l.ImplementationType == typeof(EventLogLoggerProvider))
                      .ToList();

                      foreach (var el in eventLoggers)
                          loggingBuilder.Services.Remove(el);
                  }

                  //Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory.WithFilter(CreateFilterLoggerSettings());
                  loggingBuilder
                  .AddSplat()
#if !__WASM__
            .AddConsole()
#else
            .ClearProviders()            
#endif

#if DEBUG
            .SetMinimumLevel(LogLevel.Debug)
#else
            .SetMinimumLevel(LogLevel.Information)
#endif
            ;

              })
              .Build();

            ServiceProvider = host.Services;
            ServiceProvider.UseMicrosoftDependencyResolver();
        }

        void ConfigureServices(IServiceCollection services)
        {
            services.UseMicrosoftDependencyResolver();
            var resolver = Splat.Locator.CurrentMutable;
            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();

            var allTypes = Assembly.GetExecutingAssembly()
              .DefinedTypes
              .Where(t => !t.IsAbstract);
           
            // register services
            {
                services.AddSingleton<ISchedulerProvider, SchedulerProvider>();
                services.AddSingleton<IPeopleInSpaceQuery, PeopleInSpaceQuery>();

                //https://api.spacexdata.com/v4
                //ISpaceXApi              
#if __WASM__
                services.AddRefitClient<ISpaceXApi>()
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://api.spacexdata.com/v4"))
                    .ConfigurePrimaryHttpMessageHandler(h => new Uno.UI.Wasm.WasmHttpHandler());
                
    
#else
                //var settings = new RefitSettings();
                services.AddRefitClient<ISpaceXApi>(/*settings*/)
                        .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://api.spacexdata.com/v4"));
#endif

            }

            // register view models
            {
                services.AddSingleton<MainPageViewModel>();
                /*
                services.AddSingleton<IScreen>(sp => sp.GetRequiredService<NavigationViewModel>());

                var rvms = allTypes.Where(t => typeof(RoutableViewModel).IsAssignableFrom(t));
                foreach (var rvm in rvms)
                    services.AddTransient(rvm);
                */
            }

            // register views
            {
                var vf = typeof(IViewFor<>);
                bool isGenericIViewFor(Type ii) => ii.IsGenericType && ii.GetGenericTypeDefinition() == vf;
                var views = allTypes
                  .Where(t => t.ImplementedInterfaces.Any(isGenericIViewFor));

                foreach (var v in views)
                {
                    var ii = v.ImplementedInterfaces.Single(isGenericIViewFor);

                    services.AddTransient(ii, v);
                    //Locator.CurrentMutable.Register(() => Locator.Current.GetService(v), ii, "Landscape");
                }
            }

        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

#if NET5_0 && WINDOWS
            _window = new Window();
            _window.Activate();
#else
            _window = Windows.UI.Xaml.Window.Current;
#endif

            var rootFrame = _window.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                _window.Content = rootFrame;
            }

#if !(NET5_0 && WINDOWS)
            if (e.PrelaunchActivated == false)
#endif
            {                
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    var vm = ServiceProvider.GetService<MainPageViewModel>();
                    var view = ServiceProvider.GetRequiredService<IViewLocator>().ResolveView(vm);
                    rootFrame.Content = view;
                    rootFrame.DataContext = vm;
                }
                // Ensure the current window is active
                _window.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        /// <summary>
        /// Configures global Uno Platform logging
        /// </summary>
        private static void InitializeLogging()
        {
            var factory = LoggerFactory.Create(builder =>
            {
#if __WASM__
                builder.AddProvider(new global::Uno.Extensions.Logging.WebAssembly.WebAssemblyConsoleLoggerProvider());
#elif __IOS__
                builder.AddProvider(new global::Uno.Extensions.Logging.OSLogLoggerProvider());
#elif NETFX_CORE
                builder.AddDebug();
#else
                builder.AddConsole();
#endif

                // Exclude logs below this level
                builder.SetMinimumLevel(LogLevel.Information);

                // Default filters for Uno Platform namespaces
                builder.AddFilter("Uno", LogLevel.Warning);
                builder.AddFilter("Windows", LogLevel.Warning);
                builder.AddFilter("Microsoft", LogLevel.Warning);

                // Generic Xaml events
                // builder.AddFilter("Windows.UI.Xaml", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.VisualStateGroup", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.StateTriggerBase", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.UIElement", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.FrameworkElement", LogLevel.Trace );

                // Layouter specific messages
                // builder.AddFilter("Windows.UI.Xaml.Controls", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.Controls.Layouter", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.Controls.Panel", LogLevel.Debug );

                // builder.AddFilter("Windows.Storage", LogLevel.Debug );

                // Binding related messages
                // builder.AddFilter("Windows.UI.Xaml.Data", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.Data", LogLevel.Debug );

                // Binder memory references tracking
                // builder.AddFilter("Uno.UI.DataBinding.BinderReferenceHolder", LogLevel.Debug );

                // RemoteControl and HotReload related
                // builder.AddFilter("Uno.UI.RemoteControl", LogLevel.Information);

                // Debug JS interop
                // builder.AddFilter("Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug );
            });

            global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory = factory;
        }
    }
}
