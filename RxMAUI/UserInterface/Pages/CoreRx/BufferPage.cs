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
            new Grid
            {
                RowDefinitions =
                    Rows.Define(
                        Auto,
                        Star),

                Children =
                {
                    new Entry
                    {
                        VerticalOptions = LayoutOptions.Start,
                    }
                        .Row(0)
                        .Assign(out textEntry),

                    new CollectionView()
                        .ItemsSource(_values)
                        .ItemTemplate(
                            new DataTemplate(
                                () =>
                                new VerticalStackLayout
                                {
                                    Spacing = 4,
                                    Children =
                                    {
                                        new Label
                                        {
                                            FontSize = 18,
                                        }
                                            .Bind(Label.TextProperty, Binding.SelfPath, BindingMode.OneTime),
                                        new BoxView
                                        {
                                            Color = Colors.Gray,
                                            HeightRequest = 1,
                                        },
                                    },
                                }))
                        .Row(1)
                        .Assign(out lastEntries),
                },
            };
    }

    protected override void SetupObservables()
    {
        textEntry.Events()
            .TextChanged
            .Buffer(TimeSpan.FromSeconds(3), Scheduler.Default)
            .Select(
                argsList =>
                    argsList.Count > 0
                        ? string.Join(
                            Environment.NewLine,
                            argsList.Select(args => args.NewTextValue).Reverse().ToList())
                        : "~No Changes~")
            .Do(newValue => _values.Insert(0, newValue))
            .Subscribe()
            .DisposeWith(ObservableDisposables);
    }
}

