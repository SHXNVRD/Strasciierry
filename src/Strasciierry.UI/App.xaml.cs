using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Serilog;
using Strasciierry.UI.Services.Activation;
using Strasciierry.UI.Extensions;
using UnhandledExceptionEventArgs = System.UnhandledExceptionEventArgs;
using System.Runtime.ExceptionServices;
using Strasciierry.Core;

namespace Strasciierry.UI;

public partial class App : Application
{
    public static WindowEx MainWindow { get; } = new MainWindow();
    public static new App Current => (App)Application.Current;
    public static XamlRoot XamlRoot => MainWindow.Content.XamlRoot;
    public static UIElement? AppTitlebar { get; set; }

    public App()
    {
        InitializeComponent();

        ConfigureServices();

        UnhandledException += OnUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
        AppDomain.CurrentDomain.FirstChanceException += CurrentDomainOnFirstChanceException;
        TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
    }

    private static void ConfigureServices()
    {
        KeyedIoc.Instance.ConfigureServices(
            new ServiceCollection()
            .ConfigureServices()
            .BuildServiceProvider());
    }

    private void CurrentDomainOnFirstChanceException(object? sender, FirstChanceExceptionEventArgs e)
    {
        var ex = e.Exception;
        Log.Error("[CurrentDomainFirstChanceExcpetion] type: {type}, message: {description}, exception: {@exception}, inner exception: {@innerException}",
            ex.GetType().Name, ex.Message, ex, ex.InnerException);
    }

    private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var ex = (Exception)e.ExceptionObject;
        Log.Error("[CurrentDomainException] type: {type}, message: {description}, exception: {@exception}, inner exception: {@innerException}",
            ex.GetType().Name, ex.Message, ex, ex.InnerException);
    }

    private void TaskSchedulerOnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        var ex = e.Exception;
        Log.Error("[TaskSchedulerException] type: {type}, message: {description}, exception: {@exception}, inner exception: {@innerException}",
            ex.GetType().Name, ex.Message, ex, ex.InnerException);
    }

    private void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        var ex = e.Exception;
        Log.Error("[UnhandledException] type: {type}, message: {description}, exception: {@exception}, inner exception: {@innerException}",
            ex.GetType().Name, ex.Message, ex, ex.InnerException);

        e.Handled = true;
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        await KeyedIoc.Instance
            .GetRequiredService<IActivationService>()
            .ActivateAsync(args);
    }
}
