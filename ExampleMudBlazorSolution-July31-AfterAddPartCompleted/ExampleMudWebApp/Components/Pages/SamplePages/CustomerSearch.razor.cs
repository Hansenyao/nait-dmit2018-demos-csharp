using ExampleMudSystem.BLL;
using ExampleMudSystem.ViewModels;
using Microsoft.AspNetCore.Components;
using static MudBlazor.Icons;
using ExampleMudWebApp;

namespace ExampleMudWebApp.Components.Pages.SamplePages
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

        private CustomerEditView customer = new CustomerEditView();
        #endregion

        #region Properties

        [Inject]
        protected CustomerService CustomerService { get; set; }
           
        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        protected List<CustomerSearchView> Customers { get; set; }
                                        = new List<CustomerSearchView>();

        #endregion



       


        private void Search()
        {
			// Clear all error messages in the UI.
			noRecords = false;
			errorDetails.Clear();
			errorMessage = string.Empty;
			feedbackMessage = string.Empty;

			// Standard try/catch block for dealing with unanticipated
			//		exceptions encountered by the CustomerService method.
			try
			{
				// Catch the result object returned from the CustomerService
				//		class method
				var result = CustomerService.GetCustomers(lastName, phoneNumber);

				// If the operation was successful, bind the returned information
				//		to the CodeBehind collection being read by the UI.
				if (result.IsSuccess)
				{
					Customers = result.Value;

				}
				else
				{
					errorMessage = "Please address the following errors:";
					noRecords = true;
					// Otherwise, bind the error messages returned to the 
					//		collection used by the UI.
					errorDetails = HelperMethods.GetErrorMessages(result.Errors.ToList());
				}
			}
			catch (Exception ex)
			{
				// If an exception occurs, show the user the base cause for
				//		the exception.
				errorMessage = HelperMethods.GetInnerMostException(ex).Message;
			}
          
		}

        private void New()
        {
			NavigationManager.NavigateTo("/SamplePages/CustomerEdit/0");
        }

		private void EditCustomer(int customerID)
		{
			NavigationManager.NavigateTo($"/SamplePages/CustomerEdit/{customerID}");
		}

		private void NewInvoice(int customerID)
		{

		}
    }
}
