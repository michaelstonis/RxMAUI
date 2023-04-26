using ReactiveMarbles.ObservableEvents;
using ReactiveUI;

namespace RxMAUI.UserInterface.Pages.CoreRx;

public class MergePage : ContentPageBase
{
    private Button _button1, _button2, _button3, _button4;

    private Span _output;

    protected override void SetupUserInterface()
    {
        Title = "Rx - Merge";

        Content =
            new StackLayout
            {
                Spacing = 16d,
                Children =
                {
                    new Button()
                        .Text("Peter Venkman")
                        .Assign(out _button1),

                    new Button()
                        .Text("Ray Stantz")
                        .Assign(out _button2),

                    new Button()
                        .Text("Egon Spengler")
                        .Assign(out _button3),

                    new Button()
                        .Text("Winston Zeddemore")
                        .Assign(out _button4),

                    new Label()
                        .FormattedText(
                            new Span
                            {
                                Text = $"Who Merged the streams?{Environment.NewLine}",
                                FontSize = 24,
                            },
                            new Span
                            {
                                FontAttributes = FontAttributes.Bold,
                                FontSize = 36,
                            }
                                .Assign(out _output))
                        .TextCenterHorizontal(),
                },
            }
                .Padding(8d);
    }

    protected override void SetupObservables()
    {
        Observable
            .Merge(
                _button1.Events()
                    .Clicked
                    .Select(static _ => "Peter Venkman"),
                _button2.Events()
                    .Clicked
                    .Select(static _ => "Ray Stantz"),
                _button3.Events()
                    .Clicked
                    .Select(static _ => "Egon Spengler"),
                _button4.Events()
                    .Clicked
                    .Select(static _ => "Winston Zeddemore"))
            .BindTo(this, x => x._output.Text)
            .DisposeWith(ObservableDisposables);
    }

}

