using ExampleMudSystem.BLL;
using ExampleMudSystem.ViewModels;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ExampleMudWeb.Components.Pages.SamplePages
{
    public partial class CustomerEdit
    {
        #region Error Message and Feedback variables
        private string feedbackMessage = string.Empty;
        private string errorMessage = string.Empty;
        private bool hasFeedback => !string.IsNullOrWhiteSpace(feedbackMessage);
        private bool hasError => !string.IsNullOrWhiteSpace(errorMessage);
        private List<string> errorDetails = new List<string>();
        #endregion

        #region Validation
        private bool isFormValid;
        private bool hasDataChanged = false;
        private string closeButtonText => hasDataChanged ? "Cancel" : "Close";
        #endregion

        private CustomerEditView customer = new CustomerEditView();
        private List<LookupView> provinces = new List<LookupView>();
        private List<LookupView> countries = new List<LookupView>();
        private List<LookupView> statusLookup = new List<LookupView>();


        private MudForm customerForm = new MudForm();

        [Inject]
        protected CustomerService CustomerService { get; set; }
        [Inject]
        protected CategoryLookupService CategoryLookupService { get; set; }
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        [Inject]
        protected IDialogService DialogService { get; set; }

        [Parameter]
        public int CustomerID { get; set; } = 0;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            errorDetails.Clear();
            errorMessage = string.Empty;
            feedbackMessage = string.Empty;

            try
            {
                var result = CategoryLookupService.GetLookups("Province");
                if (result.IsSuccess)
                {
                    provinces = result.Value;
                }
                else
                {
                    errorDetails = HelperMethods.GetErrorMessages(result.Errors.ToList());
                }
                result = CategoryLookupService.GetLookups("Country");
                if (result.IsSuccess)
                {
                    countries = result.Value;
                }
                else
                {
                    errorDetails = HelperMethods.GetErrorMessages(result.Errors.ToList());
                }
                result = CategoryLookupService.GetLookups("Customer Status");
                if (result.IsSuccess)
                {
                    statusLookup = result.Value;
                }
                else
                {
                    errorDetails = HelperMethods.GetErrorMessages(result.Errors.ToList());
                }

                if (CustomerID > 0)
                {
                    var customerResult = CustomerService.GetCustomer(CustomerID);
                    if (result.IsSuccess)
                    {
                        customer = customerResult.Value;
                    }
                    else
                    {
                        errorDetails = HelperMethods.GetErrorMessages(result.Errors.ToList());
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = HelperMethods.GetInnerException(ex).Message;
                customer = new();
            }

            StateHasChanged();
        }

        private void Save()
        {
            errorDetails.Clear();
            errorMessage = string.Empty;
            feedbackMessage = string.Empty;

        }

        private async Task Cancel()
        {
            bool? results = await DialogService.ShowMessageBox("Confirm Cancel",
                "Do you wish to close the customer editor? All unsaved changes will be lost!",
                yesText: "Yes", cancelText: "No");
            if (results == null)
            {
                return;
            }

            NavigationManager.NavigateTo("/SamplePages/CustomerList");
        }
    }
}
