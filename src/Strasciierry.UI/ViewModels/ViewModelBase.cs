using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Dispatching;

namespace Strasciierry.UI.ViewModels;

public abstract class ViewModelBase : ObservableRecipient
{
    private protected readonly DispatcherQueue _dispatcherQueue;
    private protected readonly DispatcherQueueTimer _dispatcherQueueTimer;

    protected ViewModelBase()
    {
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        _dispatcherQueueTimer = _dispatcherQueue.CreateTimer();
    }
}
