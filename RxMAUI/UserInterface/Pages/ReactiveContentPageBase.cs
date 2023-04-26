using System;
using System.Reactive.Disposables;
using ReactiveUI.Maui;

namespace RxMAUI.UserInterface.Pages
{
    public abstract class ReactiveContentPageBase<TViewModel> : ReactiveContentPage<TViewModel>
        where TViewModel : class
    {
        protected readonly CompositeDisposable ObservableDisposables = new CompositeDisposable();

        protected abstract void SetupUserInterface();

        protected abstract void SetupObservables();

        protected override void OnHandlerChanging(HandlerChangingEventArgs args)
        {
            base.OnHandlerChanging(args);

            if (args.OldHandler is not null)
            {
                ObservableDisposables.Clear();
            }

            if (args.NewHandler is not null)
            {
                SetupUserInterface();
                SetupObservables();
            }
        }
    }
}

