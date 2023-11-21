namespace RxMAUI.UserInterface.Pages.RxUI;

public class DynamicDataSearch : ContentPage
{
	DynamicDataSearchViewModel ViewModel { get; }

	public DynamicDataSearch()
	{
		BindingContext = ViewModel = new DynamicDataSearchViewModel();

		Content = new VerticalStackLayout
		{
			Padding = 8,
			Spacing = 8,
			Children =
			{
				new Grid
				{
					ColumnDefinitions =Columns.Define(Auto, Star, Auto),
					RowDefinitions = Rows.Define(Auto),
					ColumnSpacing = 8,
					Padding = 8,
					Children =
					{
						new Label()
							.Text("Entry")
							.CenterVertical()
							.Row(0).Column(0),

						new Entry()
							.Placeholder("Add an item")
							.CenterVertical()
							.Row(0).Column(1)
							.Bind(
								Entry.TextProperty,
								static (DynamicDataSearchViewModel vm) => vm.Item,
								static (DynamicDataSearchViewModel vm, string entry) => vm.Item = entry),

						new Button()
							.Text("Add")
							.CenterVertical()
							.Row(0).Column(2)
							.BindCommand(static (DynamicDataSearchViewModel vm) => vm.AddToSearchCommand)
							.Bind(Button.CommandParameterProperty, static (DynamicDataSearchViewModel vm) => vm.Item)
							.End(),
					},
				},

				new Label()
					.Top()
					.Text("Filter"),

				new Entry
				{
					Placeholder = "Filter",
				}
					.Top()
					.Bind(
						Entry.TextProperty,
						static (DynamicDataSearchViewModel vm) => vm.Filter,
						static (DynamicDataSearchViewModel vm, string filter) => vm.Filter = filter),

				new CollectionView
				{
					ItemTemplate =
						new DataTemplate(() =>
							new Label
							{
								MinimumHeightRequest = 44,
							}
								.Bind(Label.TextProperty, ".")),
				}
					.Bind(CollectionView.ItemsSourceProperty, static (DynamicDataSearchViewModel vm) => vm.FilteredItems),
			},
		};
	}
}