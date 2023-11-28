using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using RxMAUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using UIKit;

public class DynamicDataFilterViewModel : ReactiveObject
{
    private readonly RedditRssService _redditRssService;

    private string _filter;

    private SourceList<RssEntry> _items;

    private ReadOnlyObservableCollection<GroupedRssEntries> _filteredItems;

    public string Filter
    {
        get => _filter;
        set => this.RaiseAndSetIfChanged(ref _filter, value);
    }

    public ReadOnlyObservableCollection<GroupedRssEntries> FilteredItems => _filteredItems;

    public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }

    public DynamicDataFilterViewModel(RedditRssService redditRssService)
    {
        _redditRssService = redditRssService;

        _items = new SourceList<RssEntry>();

        _items
            .Connect()
            .Filter(
                this.WhenAnyValue(x => x.Filter)
                    .Throttle(TimeSpan.FromMilliseconds(250))
                    .Select(term => term?.Trim())
                    .DistinctUntilChanged()
                    .Select(
                        filter =>
                            new Func<RssEntry, bool>(
                                item =>
                                {
                                    if (string.IsNullOrWhiteSpace(filter))
                                    {
                                        return true;
                                    }

                                    return
                                        item.Author.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                                        item.Category.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                                        item.Title.Contains(filter, StringComparison.OrdinalIgnoreCase);
                                })))
            .GroupOn(x => x.Title.Substring(0, 1))
            .Transform(groupedItems => new GroupedRssEntries(groupedItems))
            .Sort(SortExpressionComparer<GroupedRssEntries>.Ascending(item => item.Key))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _filteredItems)
            .DisposeMany()
            .Subscribe();

        LoadDataCommand =
            ReactiveCommand
                .Create<Unit>(
                    async _ =>
                    {
                        var rssEntries = await _redditRssService.DownloadMultipleRssAsync();
                        _items.Edit(
                            list =>
                            {
                                list.Clear();
                                list.AddRange(rssEntries);
                            });
                    });
    }
}

public class GroupedRssEntries : ObservableCollectionExtended<RssEntry>, IDisposable
{
    private IDisposable _disposable;
    private bool disposedValue;

    public string Key { get; private set; }

    public GroupedRssEntries(IGroup<RssEntry, string> grouping)
    {
        Key = grouping.GroupKey;

        _disposable =
            grouping.List
                .Connect()
                .Sort(SortExpressionComparer<RssEntry>.Descending(x => x.Updated))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(this)
                .Subscribe();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _disposable?.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}