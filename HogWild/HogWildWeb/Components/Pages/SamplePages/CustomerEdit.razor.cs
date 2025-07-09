#nullable disable
using HogWildSystem.BLL;
using HogWildSystem.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Rendering;
using MudBlazor;
using System.Globalization;
using static MudBlazor.Icons;


namespace HogWildWeb.Components.Pages.SamplePages
{
    public partial class CustomerEdit
    {
        #region Fields
        // The customer
        private CustomerEditView customer = new();
        //  The provinces
        private List<LookupView> provinces = new();
        //  The countries
        private List<LookupView> countries = new();
        //  The status lookup
        private List<LookupView> statusLookup = new();
        //  The invoice list
        private List<InvoiceView> invoices = new();
        //  mudform control
        private MudForm customerForm = new();
        #endregion

        #region Feedback & Error Messages
        // The feedback message
        private string feedbackMessage = string.Empty;

        // The error message
        private string? errorMessage = string.Empty;

        // has feedback
        private bool hasFeedback => !string.IsNullOrWhiteSpace(feedbackMessage);

        // has error
        private bool hasError => !string.IsNullOrWhiteSpace(errorMessage) || errorDetails.Any();

        // error details
        private List<string> errorDetails = new();
        #endregion

        #region Properties
        //  The customer service
        [Inject] protected CustomerService CustomerService { get; set; } = default!;

        //  The category lookup service
        [Inject] protected CategoryLookupService CategoryLookupService { get; set; } = default!;

        [Inject] protected InvoiceService InvoiceService { get; set; } = default!;
        //   Injects the NavigationManager dependency
        [Inject] protected NavigationManager NavigationManager { get; set; } = default!;
        //   Injects the DialogService dependency

        [Inject]
        protected IDialogService DialogService { get; set; } = default!;
        //  Customer ID used to create or edit a customer
        [Parameter] public int CustomerID { get; set; } = 0;
        #endregion

        #region Validation
        // flag to if the form is valid.
        private bool isFormValid;
        //  flag if data has change
        private bool hasDataChanged = false;
        //  set text for cancel/close button
        private string closeButtonText => hasDataChanged ? "Cancel" : "Close";
        #endregion


        #region Methods
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            try
            {
                // clear previous error details and messages                
                errorDetails.Clear();
                errorMessage = string.Empty;
                feedbackMessage = String.Empty;


                //  check to see if we are navigating using a valid customer CustomerID.
                //      or are we going to create a new customer.
                if (CustomerID > 0)
                {
                    var result = CustomerService.GetCustomer(CustomerID);
                    if (result.IsSuccess)
                    {
                        customer = result.Value;
                        //  get customer invoices
                        var invoiceResults = InvoiceService.GetCustomerInvoices(CustomerID);
                        if (invoiceResults.IsSuccess)
                        {
                            invoices = invoiceResults.Value;                            
                        }
                        else
                        {
                            errorDetails = BlazorHelperClass.GetErrorMessages(invoiceResults.Errors.ToList());
                        }
                    }
                    else
                    {
                        errorDetails = BlazorHelperClass.GetErrorMessages(result.Errors.ToList());
                    }
                }
                else
                {
                    customer = new();
                }
                //  lookups
                provinces = CategoryLookupService.GetLookups("Province");
                countries = CategoryLookupService.GetLookups("Country");
                statusLookup = CategoryLookupService.GetLookups("Customer Status");
                
                //  update that data has changed
                StateHasChanged();
            }
            catch (Exception ex)
            {
                // capture any exception message for display
                errorMessage = ex.Message;
            }
        }

        // save the customer
        private void Save()
        {
            // clear previous error details and messages
            errorDetails.Clear();
            errorMessage = string.Empty;
            feedbackMessage = String.Empty;

            // wrap the service call in a try/catch to handle unexpected exceptions
            try
            {
                var result = CustomerService.AddEditCustomer(customer);
                if (result.IsSuccess)
                {
                    customer = result.Value;
                    feedbackMessage = "Data was successfully saved!";

                    // Reset change tracking
                    hasDataChanged = false;
                    isFormValid = false;
                    customerForm.ResetTouched(); // reset the touched
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

        ////  Cancels/closes this instance.
        private async Task Cancel()
        {
            if (hasDataChanged)
            {
                bool? results = await DialogService.ShowMessageBox("Confirm Cancel",
                                $"Do you wish to close the customer editor? All unsaved changes will be lost.",
                                yesText: "Yes", cancelText: "No");

                //  true means affirmative action (e.g., "Yes").
                //  null means the user dismissed the dialog(e.g., clicking "No" or closing the dialog).
                if (results == null)
                {
                    return;
                }
            }
            NavigationManager.NavigateTo("/SamplePages/CustomerList");
        }


        /// <summary>
        /// Creates new invoice.
        /// </summary>
        private void NewInvoice()
        {
            //  NOTE:   we will hard code employee ID (1)            
            NavigationManager.NavigateTo($"/SamplePages/InvoiceEdit/0/{CustomerID}/1");
        }

        /// <summary>
        /// Edit the invoice.
        /// </summary>
        /// <param name="invoiceID">The invoice identifier.</param>
        private void EditInvoice(int invoiceID)
        {
            //  NOTE:   we will hard code employee ID (1)            
            NavigationManager.NavigateTo($"/SamplePages/InvoiceEdit/{invoiceID}/{CustomerID}/1");
        }
        #endregion
    }
}
