// This file has been autogenerated from a class added in the UI designer.

using System;

using Foundation;
using UIKit;
using System.Collections.Generic;
using System.Linq;

namespace TableSearch
{
	public partial class MainTableViewController : BaseTableViewController
	{
		ResultsTableController resultsTableController;
		UISearchController searchController;
		bool searchControllerWasActive;
		bool searchControllerSearchFieldWasFirstResponder;
		readonly Product[] products;

		public MainTableViewController (IntPtr handle) : base (handle)
		{
			products = new Product[] {
				new Product (Product.DeviceTypeTitle, "iPhone", 2007, 599),
				new Product (Product.DeviceTypeTitle, "iPod", 2001, 399),
				new Product (Product.DeviceTypeTitle, "iPod touch", 2007, 210),
				new Product (Product.DeviceTypeTitle, "iPad", 2010, 499),
				new Product (Product.DeviceTypeTitle, "iPad mini", 2012, 659),
				new Product (Product.DesktopTypeTitle, "iMac", 1997, 1299),
				new Product (Product.DesktopTypeTitle, "Mac Pro", 2006, 2499),
				new Product (Product.PortableTypeTitle, "MacBook Air", 2008, 1799),
				new Product (Product.PortableTypeTitle, "MacBook Pro", 2006, 1499),
			};
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			resultsTableController = new ResultsTableController {
				FilteredProducts = new List<Product> ()
			};

			searchController = new UISearchController (resultsTableController) {
				WeakDelegate = this,
				DimsBackgroundDuringPresentation = false,
				WeakSearchResultsUpdater = this
			};

			searchController.SearchBar.SizeToFit ();
			TableView.TableHeaderView = searchController.SearchBar;

			resultsTableController.TableView.WeakDelegate = this;
			searchController.SearchBar.WeakDelegate = this;

			DefinesPresentationContext = true;

			if (searchControllerWasActive) {
				searchController.Active = searchControllerWasActive;
				searchControllerWasActive = false;

				if (searchControllerSearchFieldWasFirstResponder) {
					searchController.SearchBar.BecomeFirstResponder ();
					searchControllerSearchFieldWasFirstResponder = false;
				}
			}
		}

		[Export ("searchBarSearchButtonClicked:")]
		public virtual void SearchButtonClicked (UISearchBar searchBar)
		{
			searchBar.ResignFirstResponder ();
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return products.Length;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			Product product = products [indexPath.Row];

			UITableViewCell cell = TableView.DequeueReusableCell ((NSString)cellIdentifier, indexPath);
			ConfigureCell (cell, product);
			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			Product selectedProduct = (tableView == TableView) ? products [indexPath.Row] : resultsTableController.FilteredProducts [indexPath.Row];

			var detailViewController = (DetailViewController)Storyboard.InstantiateViewController ("DetailViewController");
			detailViewController.Product = selectedProduct; // hand off the current product to the detail view controller

			NavigationController.PushViewController (detailViewController, true);
			tableView.DeselectRow (indexPath, true);
		}

		[Export ("updateSearchResultsForSearchController:")]
		public virtual void UpdateSearchResultsForSearchController (UISearchController searchController)
		{
			var tableController = (ResultsTableController)searchController.SearchResultsController;
			tableController.FilteredProducts = PerformSearch (searchController.SearchBar.Text);
			tableController.TableView.ReloadData ();
		}

		List<Product> PerformSearch(string searchString)
		{
			searchString = searchString.Trim ();
			string[] searchItems = string.IsNullOrEmpty (searchString)
				? new string[0]
				: searchString.Split (new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			var filteredProducts = new List<Product> ();

			foreach (var item in searchItems) {
				int year = Int32.MinValue;
				Int32.TryParse (item, out year);

				double price = Double.MinValue;
				Double.TryParse (item, out price);

				IEnumerable<Product> query = 
					from p in products
					where p.Title.IndexOf (item, StringComparison.OrdinalIgnoreCase) >= 0
					|| p.IntroPrice == price
					|| p.YearIntroduced == year
					orderby p.Title
					select p;

				filteredProducts.AddRange (query);
			}

			return filteredProducts.Distinct ().ToList ();
		}
	}
}
