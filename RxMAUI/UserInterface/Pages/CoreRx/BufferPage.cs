using System.Collections.ObjectModel;
using System.Reactive.Concurrency;
using ReactiveMarbles.ObservableEvents;

namespace RxMAUI.UserInterface.Pages.CoreRx;

public class BufferPage : ContentPageBase
{
    private readonly ObservableCollection<string> _values = new ObservableCollection<string>();

    private Entry textEntry;

    private CollectionView lastEntries;

    protected override void SetupUserInterface()
    {
        Title = "Rx - Buffer";

        Content =
            new VerticalStackLayout
            {
                Children =
                {
                    new Entry
                    {
                        VerticalOptions = LayoutOptions.Start,
                    }
                        .Assign(out textEntry),

                    new CollectionView()
                        .ItemsSource(_values)
                        .Assign(out lastEntries),
                },
            };
    }

    protected override void SetupObservables()
    {
        textEntry.Events()
            .TextChanged
            .Buffer(TimeSpan.FromSeconds(3), TaskPoolScheduler.Default)
            .Select(
                argsList =>
                    string.Join(
                        Environment.NewLine,
                        argsList.Select(args => args.NewTextValue).Reverse().ToList()))
            .Do(newValue => _values.Insert(0, newValue))
            .Subscribe()
            .DisposeWith(ObservableDisposables);
    }
}

