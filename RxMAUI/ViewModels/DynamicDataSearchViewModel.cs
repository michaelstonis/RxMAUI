using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using UIKit;

public class DynamicDataSearchViewModel : ReactiveObject
{
    private string _filter;

    private string _item;

    private SourceList<string> _items;

    private ReadOnlyObservableCollection<string> _filteredItems;

    public string Item
    {
        get => _item;
        set => this.RaiseAndSetIfChanged(ref _item, value);
    }

    public string Filter
    {
        get => _filter;
        set => this.RaiseAndSetIfChanged(ref _filter, value);
    }

    public ReadOnlyObservableCollection<string> FilteredItems => _filteredItems;

    public ReactiveCommand<string, Unit> AddToSearchCommand { get; }

    public DynamicDataSearchViewModel()
    {
        _items = new SourceList<string>();

        _items
            .Connect()
            .Filter(
                this.WhenAnyValue(x => x.Filter)
                    .Throttle(TimeSpan.FromMilliseconds(200))
                    .Select(term => term?.Trim())
                    .DistinctUntilChanged()
                    .Select(
                        filter =>
                            new Func<string, bool>(
                                item =>
                                {
                                    return
                                        string.IsNullOrWhiteSpace(filter) ||
                                        item.Contains(filter, StringComparison.OrdinalIgnoreCase);
                                })))
            .Sort(SortExpressionComparer<string>.Ascending(item => item))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _filteredItems)
            .Subscribe();

        AddToSearchCommand =
            ReactiveCommand
                .Create<string>(
                    item =>
                    {
                        _items.Add(item);
                        Item = string.Empty;
                    });
    }
}