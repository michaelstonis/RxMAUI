using System;
using System.Text.RegularExpressions;

namespace RxMAUI.ViewModels
{
	public class LoginViewModel : ReactiveObject
	{
		[Reactive]
        public string EmailAddress { get; set; }

        [Reactive]
        public string Password { get; set; }

        [Reactive]
        public bool IsValid { get; private set; }

        [Reactive]
		public bool IsLoading { get; private set; }

        [Reactive]
        public ReactiveCommand<Unit, Unit> PerformLogin { get; private set; }

        public LoginViewModel()
		{
            this.WhenAnyValue(
                e => e.EmailAddress,
                p => p.Password,
                (emailAddress, password) =>
                    /* Validate our email address */
                    (
                        !string.IsNullOrEmpty(emailAddress)
                            &&
                        Regex.Matches(emailAddress, "^\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$").Count == 1
                    )
                    &&
                    /* Validate our password */
                    (
                        !string.IsNullOrEmpty(password)
                            &&
                        password.Length > 5
                    ))
                .BindTo(this, x => x.IsValid);

            PerformLogin =
                ReactiveCommand
                    .CreateFromTask(
                        async _ =>
                        {
                            var random = new Random(Guid.NewGuid().GetHashCode());
                            await Task.Delay(random.Next(250, 1000)) /* Fake Web Service Call */;

                            return Unit.Default;
                        },
                        this.WhenAnyValue(
                            x => x.IsLoading,
                            x => x.IsValid,
                            (isLoading, IsValid) => !isLoading && IsValid)
                            .Do(x => System.Diagnostics.Debug.WriteLine($"Can Login: {x}")));

            this.WhenAnyObservable(x => x.PerformLogin.IsExecuting)
                .BindTo(this, x => x.IsLoading);
        }
	}
}

