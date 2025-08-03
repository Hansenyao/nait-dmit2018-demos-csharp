using ExampleMudSystem.BLL;
using ExampleMudSystem.ViewModels;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;
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

		private bool noParts = false;

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
			errorDetails.Clear();
			errorMessage = string.Empty;
			feedbackMessage = string.Empty;
			parts.Clear();
			noParts = false;

			if (categoryID == 0 && string.IsNullOrWhiteSpace(description))
			{
				errorMessage = "Provide either a category and/or description!";
				return;
			}

			List<int> existingPartIDs = invoice.InvoiceLines.Select(x => x.PartID).ToList();

			try
			{

				var result = PartService.GetParts(categoryID, description, existingPartIDs);

				if (result.IsSuccess)
				{
					parts = result.Value;
					feedbackMessage = "Search for parts was successful!";
				}
				else
				{
					noParts = true;
					errorDetails = HelperMethods.GetErrorMessages(result.Errors.ToList());
				}

			}
			catch (Exception ex)
			{
				noParts = true;
				errorMessage = HelperMethods.GetInnerMostException(ex).Message;
			}
		}

		private async Task AddPart(int partID)
		{
			errorDetails.Clear();
			errorMessage = string.Empty;
			feedbackMessage = string.Empty;

			try
			{

				var result = PartService.GetPart(partID);

				if (result.IsSuccess)
				{
					PartView part = result.Value;
					InvoiceLineView invoiceLine = new InvoiceLineView();
					invoiceLine.PartID = partID;
					invoiceLine.Description = part.Description;
					invoiceLine.Price = part.Price;
					invoiceLine.Taxable = part.Taxable;
					invoiceLine.Quantity = 0;
					invoice.InvoiceLines.Add(invoiceLine);
					UpdateSubtotalAndTax();

					parts.Remove(parts.Where(p => p.PartID == partID).FirstOrDefault());

					await InvokeAsync(StateHasChanged);
				}
				else
				{
					errorDetails = HelperMethods.GetErrorMessages(result.Errors.ToList());
				}

			}
			catch (Exception ex)
			{
				errorMessage = HelperMethods.GetInnerMostException(ex).Message;
			}
		}

		private void Save()
		{

		}

		private void Close()
		{

		}

		private void DeleteInvoiceLine(InvoiceLineView invoiceLine)
		{

		}

		private void QuantityEdited(InvoiceLineView invoiceLine, int newQuantity)
		{

		}

		private void PriceEdited(InvoiceLineView invoiceLine, decimal newPrice)
		{

		}

		private void SyncPrice(InvoiceLineView invoiceLine)
		{

		}

		private void UpdateSubtotalAndTax()
		{

		}
	}
}
