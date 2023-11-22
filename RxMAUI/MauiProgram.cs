using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using RxMAUI.UserInterface.Pages.RxUI;

namespace RxMAUI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			// Initialize the .NET MAUI Community Toolkit by adding the below line of code
			.UseMauiCommunityToolkit()
			.ConfigureFonts(
				fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
					fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				})
			.RegisterUserInterfaces()
			.RegisterViewModels()
			.RegisterServices();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}

	private static MauiAppBuilder RegisterUserInterfaces(this MauiAppBuilder builder)
	{
		builder.Services.AddTransient<DynamicDataFilter>();

		return builder;
	}

	private static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
	{
		builder.Services.AddTransient<DynamicDataFilterViewModel>();

		return builder;
	}

	private static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
	{
		builder.Services.AddSingleton<RedditRssService>();

		return builder;
	}
}