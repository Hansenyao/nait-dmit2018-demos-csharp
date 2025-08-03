using ExampleMudSystem.BLL;
using ExampleMudSystem.ViewModels;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using static MudBlazor.Icons;

namespace ExampleMudWebApp.Components.Pages.SamplePages
{
	public partial class InvoiceEdit
	{
		#region Error Message and Feedback variables

		private string feedbackMessage = string.Empty;
		private string errorMessage = string.Empty;
		private bool hasFeedback => !string.IsNullOrWhiteSpace(feedbackMessage);
		private bool hasError => !string.IsNullOrWhiteSpace(errorMessage);
		private List<string> errorDetails = new List<string>();

		#endregion

		#region Fields

		private string description;

		private int categoryID;

		private List<LookupView> partCategories;

		private List<PartView> parts = new List<PartView>();

		private InvoiceView invoice;

		#endregion

		#region Properties

		[Inject]
		protected InvoiceService InvoiceService { get; set; }

		[Inject]
		protected CategoryLookupService CategoryLookupService { get; set; }

		[Inject]
		protected PartService PartService { get; set; }

		[Inject]
		protected NavigationManager NavigationManager { get; set; }

		[Inject]
		protected IDialogService DialogService { get; set; } = default!;

		[Parameter]
		public int CustomerID { get; set; }

		[Parameter]
		public int InvoiceID { get; set; }

		[Parameter]
		public int EmployeeID { get; set; }

		#endregion

		protected override async Task OnInitializedAsync()
		{
			await base.OnInitializedAsync();

			errorDetails.Clear();
			errorMessage = string.Empty;
			feedbackMessage = string.Empty;

			try
			{
				var partResults = CategoryLookupService.GetLookups("Part Categories");

				if (partResults.IsSuccess)
				{
					partCategories = partResults.Value;
				}
				else
				{
					errorDetails = HelperMethods.GetErrorMessages(partResults.Errors.ToList());
				}

				if (CustomerID > 0)
				{
					var invoiceResult = InvoiceService.GetInvoice(InvoiceID, CustomerID, EmployeeID);

					if (invoiceResult.IsSuccess)
					{
						invoice = invoiceResult.Value;
						//var invoiceResults = InvoiceService.GetCustomerInvoices(CustomerID);
						//if (invoiceResults.IsSuccess)
						//{
						//	invoices = invoiceResults.Value;
						//}
						//else
						//{
						//	errorDetails = HelperMethods.GetErrorMessages(invoiceResults.Errors.ToList());
						//}
					}
					else
					{
						errorDetails = HelperMethods.GetErrorMessages(invoiceResult.Errors.ToList());
					}
				}
			}
			catch (Exception ex)
			{
				errorMessage = HelperMethods.GetInnerMostException(ex).Message;
				//customer = new();
			}

			StateHasChanged();
		}

		private void SearchParts()
		{

		}
	}
}
