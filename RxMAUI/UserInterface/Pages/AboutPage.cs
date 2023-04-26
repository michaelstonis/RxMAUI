using System;
namespace RxMAUI.UserInterface.Pages
{
	public class AboutPage : ContentPageBase
	{
        protected override void SetupUserInterface()
        {
            Content =
                new ScrollView
                {
                    Content =
                        new StackLayout
                        {
                            Children =
                            {
                                new Label()
                                    .Text("About"),
                            },
                        },
                };
        }

        protected override void SetupObservables()
        {
        }
    }
}

