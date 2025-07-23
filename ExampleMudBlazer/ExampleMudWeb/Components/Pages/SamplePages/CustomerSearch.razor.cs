using ExampleMudSystem.BLL;
using ExampleMudSystem.ViewModels;
using Microsoft.AspNetCore.Components;

namespace ExampleMudWeb.Components.Pages.SamplePages
{
    public partial class CustomerSearch
    {
        #region Fields
        private string lastName = string.Empty;
        private string phoneNumber = string.Empty;
        private bool noRecords = false;
        private string feedbackMessage = string.Empty;
        private string errorMessage = string.Empty;
        private bool hasFeedback => !string.IsNullOrWhiteSpace(feedbackMessage);
        private bool hasError => !string.IsNullOrWhiteSpace(errorMessage);
        private List<string> errorDetails = new List<string>();
        #endregion

        #region Properties
        [Inject]
        protected CustomerService customerService { get; set; } = default!;

        [Inject]
        protected NavigationManager navigationManager { get; set; } = default!;

        protected List<CustomerSearchView> Customers { get; set; }


        #endregion

        private void New()
        {
            navigationManager.NavigateTo("/SampelPages/CustomerEdit/0");

        }

        private void EditCustomer(int customerID)
        {
            navigationManager.NavigateTo($"/SampelPages/CustomerEdit/{customerID}");
        }

        private void NewInvoice(int customerID)
        {

        }
    }
}
