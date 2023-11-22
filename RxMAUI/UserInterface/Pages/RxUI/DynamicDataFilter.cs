namespace RxMAUI.UserInterface.Pages.RxUI;

public class DynamicDataFilter : ContentPage
{
	DynamicDataFilterViewModel ViewModel { get; }

	public DynamicDataFilter(DynamicDataFilterViewModel vm)
	{
		BindingContext = ViewModel = vm;

		Content = new Grid
		{
			Padding = 8,
			RowSpacing = 8,
			RowDefinitions = Rows.Define(Auto, Auto, Auto, Star),
			ColumnDefinitions = Columns.Define(Star),
			Children =
			{
				new Button()
					.Text("Load Data")
					.Row(0).Column(0)
					.BindCommand(static (DynamicDataFilterViewModel vm) => vm.LoadDataCommand),

				new Label()
					.Row(1).Column(0)
					.Text("Filter"),

				new Entry
				{
					Placeholder = "Filter",
					ClearButtonVisibility = ClearButtonVisibility.WhileEditing,
				}
					.Row(2).Column(0)
					.Bind(
						Entry.TextProperty,
						static (DynamicDataFilterViewModel vm) => vm.Filter,
						static (DynamicDataFilterViewModel vm, string filter) => vm.Filter = filter),

				new ListView
				{
					HasUnevenRows = true,
					SelectionMode = ListViewSelectionMode.None,
					ItemTemplate =
						new DataTemplate(() =>
							new ViewCell
							{
								View =
									new Grid
									{
										ColumnDefinitions = Columns.Define(Star, Star),
										RowDefinitions = Rows.Define(Auto, Auto),
										Padding = 8,
										RowSpacing = 4,
										Children =
										{
											new Label
											{
												FontAttributes = FontAttributes.Bold,
											}
												.Row(0).Column(0)
												.ColumnSpan(2)
												.Bind(Label.TextProperty, static (RssEntry rssEntry) => rssEntry.Title),
											new Label
											{
												FontAttributes = FontAttributes.Italic,
												FontSize = 10,
											}
												.Start()
												.Row(1).Column(0)
												.Bind(Label.TextProperty, static (RssEntry rssEntry) => rssEntry.Category),
											new Label
											{
												FontAttributes = FontAttributes.Italic,
												FontSize = 10,
											}
												.End()
												.Row(1).Column(1)
												.Bind(Label.TextProperty, static (RssEntry rssEntry) => rssEntry.Author),
										},
									},
							}),
				}
					.Row(3).Column(0)
					.Bind(ListView.ItemsSourceProperty, static (DynamicDataFilterViewModel vm) => vm.FilteredItems),
			},
		};
	}
}