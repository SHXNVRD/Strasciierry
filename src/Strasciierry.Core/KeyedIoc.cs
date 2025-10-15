using Microsoft.Extensions.DependencyInjection;
using Windows.Devices.AllJoyn;

namespace Strasciierry.Core;

// https://github.com/CommunityToolkit/dotnet/issues/803
public class KeyedIoc : IKeyedServiceProvider
{
    public static KeyedIoc Instance { get; } = new();

    private volatile IKeyedServiceProvider? _serviceProvider;

    public void ConfigureServices(IKeyedServiceProvider serviceProvider)
    {
        IKeyedServiceProvider? oldServices = Interlocked.CompareExchange(ref _serviceProvider, serviceProvider, null);

        if (oldServices is not null)
        {
            ThrowInvalidOperationExceptionForRepeatedConfiguration();
        }
    }

    public object? GetService(Type serviceType)
    {
        IKeyedServiceProvider? provider = _serviceProvider;

        if (provider is null)
        {
            ThrowInvalidOperationExceptionForMissingInitialization();
        }

        return provider!.GetService(serviceType);
    }

    public object? GetKeyedService(Type serviceType, object serviceKey)
    {
        if (_serviceProvider is null)
        {
            ThrowInvalidOperationExceptionForMissingInitialization();
        }

        return _serviceProvider.GetKeyedService(serviceType, serviceKey);
    }

    public object GetRequiredKeyedService(Type serviceType, object serviceKey)
    {
        if (_serviceProvider is null)
        {
            ThrowInvalidOperationExceptionForMissingInitialization();
        }

        var service = _serviceProvider.GetKeyedService(serviceType, serviceKey);

        if (service is null)
        {
            ThrowInvalidOperationExceptionForUnregisteredType();
        }

        return service;
    }

    private static void ThrowInvalidOperationExceptionForMissingInitialization()
    {
        throw new InvalidOperationException("The service provider has not been configured yet");
    }

    private static void ThrowInvalidOperationExceptionForUnregisteredType()
    {
        throw new InvalidOperationException("The requested service type was not registered");
    }

    private static void ThrowInvalidOperationExceptionForRepeatedConfiguration()
    {
        throw new InvalidOperationException("The default service provider has already been configured");
    }
}
