#nullable disable
using HogWildSystem.BLL;
using HogWildSystem.ViewModels;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HogWildWeb.Components.Pages.SamplePages
{
    public partial class InvoiceEdit
    {
        #region Fields
        private InvoiceView invoice = new();
        private int categoryID = 0;

        //Parts Search
        private string description = string.Empty;
        private List<LookupView> partCategories = [];
        private List<PartView> parts = [];
        private bool noParts;

        //Errors and Feedback
        private List<string> errorDetails = [];
        private string feedbackMessage = string.Empty;
        private string errorMessage = string.Empty;

        #endregion

        #region Properties
        [Inject]
        protected InvoiceService InvoiceService { get; set; } = default!;
        [Inject]
        protected PartService PartService { get; set; } = default!;
        [Inject]
        protected CategoryLookupService CategoryLookupService { get; set; } = default!;
        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected IDialogService DialogService { get; set; } = default!;

        //Page Parameters
        [Parameter] public int InvoiceID { get; set; }
        [Parameter] public int CustomerID { get; set; }
        [Parameter] public int EmployeeID { get; set; }
        #endregion

        #region Methods
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            // clear previous error details and messages
            errorDetails.Clear();
            errorMessage = string.Empty;
            feedbackMessage = String.Empty;

            // wrap the service call in a try/catch to handle unexpected exceptions
            try
            {
                var invoiveResult = InvoiceService.GetInvoice(InvoiceID, CustomerID, EmployeeID);
                if (invoiveResult.IsSuccess)
                {
                    invoice = invoiveResult.Value;
                }
                else
                {
                    errorDetails = BlazorHelperClass.GetErrorMessages(invoiveResult.Errors.ToList());
                }
                partCategories = CategoryLookupService.GetLookups("Part Categories");
            }
            catch (Exception ex)
            {
                // capture any exception message for display
                errorMessage = ex.Message;
            }
        }
        private void SearchParts()
        {
            //  reset the error detail list
            errorDetails.Clear();

            //  reset the error message to an empty string
            errorMessage = string.Empty;

            //  reset feedback message to an empty string
            feedbackMessage = String.Empty;

            //  clear the part list before we do our search
            parts.Clear();

            // reset no parts to false
            noParts = false;

            if (categoryID == 0 && string.IsNullOrWhiteSpace(description))
            {
                errorMessage = "Please provide either a category and/or description";
                return;
            }
            //  search for our parts
            List<int> existingPartIDs =
                invoice.InvoiceLines
                .Select(x => x.PartID)
                .ToList();
            try
            {
                var result = PartService.GetParts(categoryID, description, existingPartIDs);
                if (result.IsSuccess)
                {
                    parts = result.Value;
                    feedbackMessage = "Search for part(s) was successful";
                }
                else
                {
                    noParts = true;
                    errorDetails = BlazorHelperClass.GetErrorMessages(result.Errors.ToList());
                }
            }
            catch (Exception ex)
            {
                // capture any exception message for display
                errorMessage = ex.Message;
            }
        }

        // add part to invoice line and remove part from part list
        private async Task AddPart(int partID)
        {
            // clear previous error details and messages
            errorDetails.Clear();
            errorMessage = string.Empty;
            feedbackMessage = String.Empty;
            // wrap the service call in a try/catch to handle unexpected exceptions
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

                    // remove the current part from the part list
                    parts.Remove(parts.Where(p => p.PartID == partID)
                        .FirstOrDefault());
                    await InvokeAsync(StateHasChanged);
                }
                else
                {
                    errorDetails = BlazorHelperClass.GetErrorMessages(result.Errors.ToList());
                }
            }
            catch (Exception ex)
            {
                // capture any exception message for display
                errorMessage = ex.Message;
            }          
        }

        //  delete invoice line
        /// <summary>
        /// Deletes the invoice line.
        /// </summary>
        /// <param name="invoiceLine">The invoice line to remove.</param>
        private async Task DeleteInvoiceLine(InvoiceLineView invoiceLine)
        {
            bool? results = await DialogService.ShowMessageBox("Confirm Delete", 
                $"Are you sure that you wish to remove {invoiceLine.Description}?", 
                yesText: "Remove", cancelText: "Cancel");

            if (results == true)
            {
                invoice.InvoiceLines.Remove(invoiceLine);
                UpdateSubtotalAndTax();
            }
        }

        /// <summary>
        /// Synchronizes the price.
        /// </summary>
        /// <param name="line">The line.</param>
        private void SyncPrice(InvoiceLineView line)
        {
            //Find the original price of the Part from the database
            try
            {
                var result = PartService.GetPart(line.PartID);
                if (result.IsSuccess)
                {
                    PartView part = result.Value;
                    line.Price = part.Price;
                    UpdateSubtotalAndTax();
                }
                else
                {
                    errorDetails = BlazorHelperClass.GetErrorMessages(result.Errors.ToList());
                }
            }
            catch (Exception ex)
            {
                // capture any exception message for display
                errorMessage = ex.Message;
            }
        }

        /// <summary>
        /// Updates the subtotal and tax.
        /// </summary>
        private void UpdateSubtotalAndTax()
        {
            invoice.SubTotal = invoice.InvoiceLines
                .Where(x => !x.RemoveFromViewFlag)
                .Sum(x => x.Quantity * x.Price);
            invoice.Tax = invoice.InvoiceLines
                .Where(x => !x.RemoveFromViewFlag)
                .Sum(x => x.Taxable ? x.Quantity * x.Price * 0.05m : 0);
        }

        /// <summary>
        /// Quantities the edited.
        /// </summary>
        /// <param name="lineView">The line view.</param>
        /// <param name="newQuantity">The new quantity.</param>
        private void QuantityEdited(InvoiceLineView lineView, int newQuantity)
        {
            lineView.Quantity = newQuantity;
            UpdateSubtotalAndTax();
        }

        /// <summary>
        /// Prices the edited.
        /// </summary>
        /// <param name="lineView">The line view.</param>
        /// <param name="newPrice">The new price.</param>
        private void PriceEdited(InvoiceLineView lineView, decimal newPrice)
        {
            lineView.Price = newPrice;
            UpdateSubtotalAndTax();
        }

        /// <summary>
        /// Saves this invoice.
        /// </summary>
        private async Task Save()
        {
            // clear previous error details and messages
            errorDetails.Clear();
            errorMessage = string.Empty;
            feedbackMessage = String.Empty;

            //  use in the feedback message to indicate
            //   if this is a new invoice or an update
            bool isNewInvoice = false;
            
            // wrap the service call in a try/catch to handle unexpected exceptions
            try
            {

                var result = InvoiceService.AddEditInvoice(invoice);
                if (result.IsSuccess)
                {
                    isNewInvoice = invoice.InvoiceID == 0;
                    invoice = result.Value;
                    InvoiceID = invoice.InvoiceID;
                    feedbackMessage = isNewInvoice
                        ? $"New Invoice No {invoice.InvoiceID} was created"
                        : $"Invoice No {invoice.InvoiceID} was updated";
                    await InvokeAsync(StateHasChanged);
                }
                else
                {
                    errorDetails = BlazorHelperClass.GetErrorMessages(result.Errors.ToList());
                }
            }
            catch (Exception ex)
            {
                // capture any exception message for display
                errorMessage = ex.Message;
            }
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        private async Task Close()
        {
            bool? results = await DialogService.ShowMessageBox("Confirm Cancel", 
                    $"Do you wish to close the invoice editor? All unsaved changes will be lost.", 
                    yesText: "Yes", 
                    cancelText: "No");

            if (results == true)
            {
                NavigationManager.NavigateTo($"/SamplePages/CustomerEdit/{CustomerID}");
            }
        }
        #endregion
    }
}
