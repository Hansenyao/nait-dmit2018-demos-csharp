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

		private List<PartView> partsToDisplay = new List<PartView>();

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

						if (invoice != null && invoice.InvoiceLines.Count > 0)
						{
							List<int> partIDs = invoice.InvoiceLines.Select(p => p.PartID).ToList();

							var partsResults = PartService.GetPartsForPartIDs(partIDs);

							if (partsResults.IsSuccess)
							{
								parts = partsResults.Value;
							}
							else
							{
								errorDetails = HelperMethods.GetErrorMessages(partsResults.Errors.ToList());
							}
						}
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
			}

			StateHasChanged();
		}

		private void SearchParts()
		{
			errorDetails.Clear();
			errorMessage = string.Empty;
			feedbackMessage = string.Empty;
			partsToDisplay.Clear();
			noParts = false;

			if (categoryID == 0 && string.IsNullOrWhiteSpace(description))
			{
				errorMessage = "Provide either a category and/or description!";
				return;
			}

			List<int> existingPartIDs = invoice.InvoiceLines
										.Where(x => !x.RemoveFromViewFlag)
										.Select(x => x.PartID).ToList();

			try
			{

				var result = PartService.GetParts(categoryID, description, existingPartIDs);

				if (result.IsSuccess)
				{
					partsToDisplay = result.Value;
					parts.RemoveAll(x => partsToDisplay.Any(y => y.PartID == x.PartID));
					parts.AddRange(partsToDisplay);
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
					int QOH = parts.Where(p => p.PartID == partID).Select(p => p.QOH).FirstOrDefault();

					partsToDisplay.Remove(partsToDisplay.Where(p => p.PartID == partID).FirstOrDefault());

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

		private async Task Save()
		{
			errorDetails.Clear();
			errorMessage = string.Empty;
			feedbackMessage = string.Empty;

			bool isNewInvoice = false;

			try
			{

				var result = InvoiceService.AddEditInvoice(invoice);

				if (result.IsSuccess)
				{
					invoice = result.Value;
					isNewInvoice = invoice.InvoiceID == 0;
					InvoiceID = invoice.InvoiceID;

					feedbackMessage = isNewInvoice
									? $"New Invoice No. {InvoiceID} was created"
									: $"Invoice No. {InvoiceID} was updated";

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

		private async Task Close()
		{
			bool? results = await DialogService.ShowMessageBox("Confirm Cancel",
				"Do you wish to close the customer editor?  All unsaved changes will be lost!",
				yesText: "Yes", cancelText: "No");

			if (results == true)
			{
				NavigationManager.NavigateTo("/SamplePages/CustomerSearch");
			}			
		}

		private async Task DeleteInvoiceLine(InvoiceLineView invoiceLine)
		{
			bool? results = await DialogService.ShowMessageBox("Confirm Delete",
				$"Are you sure you wish to remove {invoiceLine.Description}?",
				yesText: "Remove", cancelText: "Cancel");

			if (results == true)
			{ 
				invoiceLine.RemoveFromViewFlag = true;
				UpdateSubtotalAndTax();
			}
		}

		private void QuantityEdited(InvoiceLineView invoiceLine, int newQuantity)
		{
			invoiceLine.Quantity = newQuantity;
			UpdateSubtotalAndTax();
		}

		private void PriceEdited(InvoiceLineView invoiceLine, decimal newPrice)
		{
			invoiceLine.Price = newPrice;
			UpdateSubtotalAndTax();
		}

		private void SyncPrice(InvoiceLineView invoiceLine)
		{
			errorDetails.Clear();
			errorMessage = string.Empty;
			feedbackMessage = string.Empty;

			try
			{

				var result = PartService.GetPart(invoiceLine.PartID);

				if (result.IsSuccess)
				{
					PartView part = result.Value;
					invoiceLine.Price = part.Price;
					UpdateSubtotalAndTax();
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

		private void UpdateSubtotalAndTax()
		{
			invoice.Subtotal = invoice.InvoiceLines
								.Where(x => !x.RemoveFromViewFlag)
								.Sum(x => x.Quantity * x.Price);

			invoice.Tax = invoice.InvoiceLines
								.Where(x => !x.RemoveFromViewFlag)
								.Sum(x => x.Taxable? x.Quantity * x.Price * 0.05m : 0);
		}
	}
}
