using System;
using ReactiveUI.Maui;

namespace RxMAUI.UserInterface.Pages.RxUI
{
	public class LoginPage : ReactiveContentPageBase<ViewModels.LoginViewModel>
	{
        private Entry emailEntry, passwordEntry;

        private Button login;

        private ActivityIndicator loading;

        public LoginPage()
		{
            ViewModel = new ViewModels.LoginViewModel();
		}

        protected override void SetupUserInterface()
        {
            Title = "RxUI - Login";

            Content =
                new StackLayout
                {
                    Spacing = 16,
                    Children =
                    {
                        new Entry()
                            .Placeholder("Email")
                            .Assign(out emailEntry),
                        new Entry
                        {
                            IsPassword = true,
                        }
                            .Placeholder("Password")
                            .Assign(out passwordEntry),
                        new Button()
                            .Text("Login")
                            .Assign(out login),
                        new ActivityIndicator()
                            .CenterHorizontal()
                            .Assign(out loading),
                    }
                }
                    .Padding(40d);
        }

        protected override void SetupObservables()
        {
            this.Bind(ViewModel, vm => vm.EmailAddress, c => c.emailEntry.Text)
                .DisposeWith(ObservableDisposables);

            this.Bind(ViewModel, vm => vm.Password, c => c.passwordEntry.Text)
                .DisposeWith(ObservableDisposables);

            this.OneWayBind(ViewModel, vm => vm.IsLoading, c => c.loading.IsRunning)
                .DisposeWith(ObservableDisposables);

            this.OneWayBind(ViewModel, vm => vm.IsLoading, c => c.loading.IsVisible)
                .DisposeWith(ObservableDisposables);

            this.BindCommand(ViewModel, vm => vm.PerformLogin, c => c.login)
                .DisposeWith(ObservableDisposables);

            this.WhenAnyObservable(x => x.ViewModel.PerformLogin)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(async _ => await DisplayAlert("Log In", "It's Log, It's Log", "It's Big, It's Heavy, It's Wood"))
                .Subscribe()
                .DisposeWith(ObservableDisposables);
        }
    }
}

