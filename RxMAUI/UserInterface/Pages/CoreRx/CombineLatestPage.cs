using CommunityToolkit.Maui.Markup;
using ReactiveMarbles.ObservableEvents;

namespace RxMAUI.UserInterface.Pages.CoreRx;

public class CombineLatestPage : ContentPageBase
{
    private Frame _colorDisplay;

    private Label _colorHex;

    private Slider _red, _green, _blue;

    protected override void SetupUserInterface()
    {
        Title = "Rx - Combine Latest";

        Content =
            new StackLayout
            {
                Padding = new Thickness(40d),
                Children =
                {
                    new Frame
                    {
                        CornerRadius = 12f,
                        Content =
                            new Label()
                                .Font(size:48, bold: true)
                                .TextCenterHorizontal()
                                .TextCenterVertical()
                                .Assign(out _colorHex),
                    }
                        .Height(250)
                        .Assign(out _colorDisplay),

                    new Label()
                        .Text("Red"),

                    new Slider(0, 255, 0)
                        .Assign(out _red),

                    new Label()
                        .Text("Green"),

                    new Slider(0, 255, 0)
                        .Assign(out _green),

                    new Label()
                        .Text("Blue"),

                    new Slider(0, 255, 0)
                        .Assign(out _blue),
                },
            }
                .Padding(40);
    }

    protected override void SetupObservables()
    {
        Observable
            .CombineLatest(
                _red.Events()
                    .ValueChanged
                    .Select(x => (int)x.NewValue)
                    .StartWith(0),
                _green.Events()
                    .ValueChanged
                    .Select(x => (int)x.NewValue)
                    .StartWith(0),
                _blue.Events()
                    .ValueChanged
                    .Select(x => (int)x.NewValue)
                    .StartWith(0),
                (r, g, b) => Color.FromRgb(r, g, b))
            .Do(
                color =>
                {
                    _colorDisplay.BackgroundColor = color;

                    _colorHex.Text = color.ToHex();
                })
            .Subscribe()
            .DisposeWith(ObservableDisposables);
    }
}

