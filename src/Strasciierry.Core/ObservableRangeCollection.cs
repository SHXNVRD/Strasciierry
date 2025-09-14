using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing.Text;
using System.Reflection.Metadata.Ecma335;

namespace Strasciierry.Core;

public class ObservableRangeCollection<T> : ObservableCollection<T>
{
    public ObservableRangeCollection()
    { }

    public ObservableRangeCollection(IEnumerable<T> collection) 
        : base(collection) 
    { }

    public ObservableRangeCollection(List<T> list)
    : base(list)
    { }

    public void AddRange(IEnumerable<T> items)
        => InsertRange(Count, items);

    public void InsertRange(int index, IEnumerable<T> items)
    {
        if (items is null)
            throw new ArgumentNullException(nameof(items));
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index));
        if (index > Count)
            throw new ArgumentOutOfRangeException(nameof(index));
        if (!items.Any())
            return;

        CheckReentrancy();

        var target = (List<T>)Items;
        target.InsertRange(index, items);

        OnCountPropertyChanged();
        OnIndexerPropertyChanged();

        if (items is not IList<T> list)
            list = [.. items];

        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list, index));
    }

    public void RemoveRange(IEnumerable<T> items)
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items));
        if (!items.Any())
            return;
        if (Count == 0)
            return;

        CheckReentrancy();

        var clusters = new Dictionary<int, List<T>>();
        var lastIndex = -1;
        List<T>? lastCluster = null;
        foreach (T item in items)
        {
            var index = IndexOf(item);
            if (index < 0)
                continue;

            Items.RemoveAt(index);

            if (lastIndex == index && lastCluster != null)
                lastCluster.Add(item);
            else
                clusters[lastIndex = index] = lastCluster = new List<T> { item };
        }

        OnCountPropertyChanged();
        OnIndexerPropertyChanged();

        if (Count == 0)
            OnCollectionReset();
        else
        {
            foreach (var cluster in clusters)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, cluster.Value, cluster.Key));
            }
        }
    }

    private void OnIndexerPropertyChanged()
        => OnPropertyChanged(EventArgsCache.IndexerPropertyChanged);

    private void OnCountPropertyChanged()
        => OnPropertyChanged(EventArgsCache.CountPropertyChanged);

    private void OnCollectionReset()
        => OnCollectionChanged(EventArgsCache.ResetCollectionChanged);

    internal static class EventArgsCache
    {
        internal static readonly PropertyChangedEventArgs CountPropertyChanged = new("Count");
        internal static readonly PropertyChangedEventArgs IndexerPropertyChanged = new("Item[]");
        internal static readonly NotifyCollectionChangedEventArgs ResetCollectionChanged = new(NotifyCollectionChangedAction.Reset);
    }
}
