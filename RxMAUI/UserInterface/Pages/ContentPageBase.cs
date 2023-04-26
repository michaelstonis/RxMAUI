﻿using System;
using System.Reactive.Disposables;

namespace RxMAUI.UserInterface.Pages
{
    public abstract class ContentPageBase : ContentPage
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

