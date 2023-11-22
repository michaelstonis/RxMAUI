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

    private ReadOnlyObservableCollection<RssEntry> _filteredItems;

    public string Filter
    {
        get => _filter;
        set => this.RaiseAndSetIfChanged(ref _filter, value);
    }

    public ReadOnlyObservableCollection<RssEntry> FilteredItems => _filteredItems;

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
            .Sort(SortExpressionComparer<RssEntry>.Ascending(item => item.Updated))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _filteredItems)
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