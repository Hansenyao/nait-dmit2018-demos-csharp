using ExampleMudSystem.BLL;
using ExampleMudSystem.ViewModels;
using Microsoft.AspNetCore.Components;
using static MudBlazor.Icons;

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

        public void AddEditCustomer(CustomerEditView customer)
        {
            // Clear all error messages in the UI. 
            errorDetails.Clear();
            errorMessage = string.Empty;

            // Standard try/catch block for dealing with unanticipated
            //		exceptions encountered by the CustomerService method.
            try
            {
                // Catch the result object returned from the CustomerService
                //		class method
                var result = CustomerService.AddEditCustomer(customer);

                // If the operation was successful, bind the returned information
                //		to the CodeBehind variable being read by the UI.
                if (result.IsSuccess)
                {
                    customer = result.Value;
                }
                else
                {
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


        // This method is for retrieving a single customer by providing the 
        //		customer's ID.	
        public void GetCustomer(int customerID)
        {
            // Clear all error messages in the UI.
            errorDetails.Clear();
            errorMessage = string.Empty;

            // Standard try/catch block for dealing with unanticipated
            //		exceptions encountered by the CustomerService method.
            try
            {
                // Catch the result object returned from the CustomerService
                //		class method
                var result = CustomerService.GetCustomer(customerID);

                // If the operation was successful, bind the returned information
                //		to the CodeBehind collection being read by the UI.
                if (result.IsSuccess)
                {
                    customer = result.Value;
                }
                else
                {
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


        // This method is for retrieving customers that at least partially 
        //		match a supplied last name or phone number
        public void GetCustomers(string lastName, string phone)
        {
            // Clear all error messages in the UI.
            errorDetails.Clear();
            errorMessage = string.Empty;

            // Standard try/catch block for dealing with unanticipated
            //		exceptions encountered by the CustomerService method.
            try
            {
                // Catch the result object returned from the CustomerService
                //		class method
                var result = CustomerService.GetCustomers(lastName, phone);

                // If the operation was successful, bind the returned information
                //		to the CodeBehind collection being read by the UI.
                if (result.IsSuccess)
                {
                    Customers = result.Value;
                }
                else
                {
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

    }
}
