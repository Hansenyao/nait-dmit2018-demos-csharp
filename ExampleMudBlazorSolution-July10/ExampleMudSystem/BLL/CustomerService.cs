using ExampleMudSystem.Entities;
using ExampleMudSystem.ViewModels;
using ExampleMudSystem.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using BYSResults;

#nullable disable

namespace ExampleMudSystem.BLL
{
    public class CustomerService
    {
        // We must store the "context", meaning database connection, for 
        //		your methods in this class to use to perform transactional
        //		operations against your database.  The CRUD operations...
        private readonly HogWildContext _hogWildContext;

        // This constructor is required because we want to check for a 
        //		valid context as part of the successful creation of the 
        //		class instance.  The short-hand version used in the 
        //		CodeBehind class does not allow this check to be performed.
        internal CustomerService(HogWildContext context)
        {
            // If there is a valid context instance accepted, assign it to
            //		the ReadOnly class variable.
            _hogWildContext = context
                        // If the passed in context is null, throw an 
                        //		Exception.  You could also have accomplished
                        //		this task with an if statement checking for
                        //		a null context at the beginning of the 
                        //		constructor.
                        ?? throw new ArgumentException(nameof(context));
        }

        // This method will attempt to store the new or edited information
        //		for a customer passed in using the CustomerEditView view model
        //		instance accepted from the caller.
        public Result<CustomerEditView> AddEditCustomer(CustomerEditView editCustomer)
        {
            var result = new Result<CustomerEditView>();

            #region Data Validation

            // Check that a view model instance was supplied 
            if (editCustomer == null)
            {
                result.AddError(new BYSResults.Error("Missing Customer",
                                            "No customer object was supplied!"));

                // If we received no information, we may exit immediately
                return result;
            }

            // The following are all checks for missing information in the
            //		supplied view model.  Make sure to check all supplied
            //		pieces of data so you may return a full list of things
            //		that must be fixed to the user.
            // Note: You should make sure to match your checks to what is 
            //		 	required in the database fields. 
            if (string.IsNullOrWhiteSpace(editCustomer.FirstName))
            {
                result.AddError(new BYSResults.Error("Missing information",
                                                "First name required!"));
            }

            if (string.IsNullOrWhiteSpace(editCustomer.LastName))
            {
                result.AddError(new BYSResults.Error("Missing information",
                                                "Last name required!"));
            }

            if (string.IsNullOrWhiteSpace(editCustomer.Address1))
            {
                result.AddError(new BYSResults.Error("Missing information",
                                                "Address Line 1 required!"));
            }

            if (string.IsNullOrWhiteSpace(editCustomer.City))
            {
                result.AddError(new BYSResults.Error("Missing information",
                                                "City required!"));
            }

            if (editCustomer.ProvStateID <= 0)
            {
                result.AddError(new BYSResults.Error("Missing Information!",
                            "Province/State ID must be greater than zero!"));
            }

            if (editCustomer.CountryID <= 0)
            {
                result.AddError(new BYSResults.Error("Missing Information!",
                            "Country ID must be greater than zero!"));
            }

            if (string.IsNullOrWhiteSpace(editCustomer.PostalCode))
            {
                result.AddError(new BYSResults.Error("Missing information",
                                                "Postal code required!"));
            }

            if (string.IsNullOrWhiteSpace(editCustomer.Phone))
            {
                result.AddError(new BYSResults.Error("Missing information",
                                                "Phone number required!"));
            }

            if (string.IsNullOrWhiteSpace(editCustomer.Email))
            {
                result.AddError(new BYSResults.Error("Missing information",
                                                "Email is required!"));
            }

            if (editCustomer.StatusID <= 0)
            {
                result.AddError(new BYSResults.Error("Missing Information!",
                            "Status ID must be greater than zero!"));
            }

            // If after all of the above data checks have been completed, one
            //		or more errors have been encountered, then you may exit
            //		immediately.  There is no point to initiating database
            //		operations with invalid data.
            if (result.Errors.Count > 0)
            {
                return result;
            }
            #endregion

            #region Business Rules

            // After simple data validation, we must also enforce any business
            //		rules that further constrain the data.  

            // In this case, we wish to ensure that if a new customer is being 
            //		provided, that an existing customer with the same full name
            //		and phone number of an existing customer.
            if (editCustomer.CustomerID <= 0)
            {
                bool customerExist = _hogWildContext
                                     .Customers.Any(customer =>
                                     customer.FirstName.ToLower()
                                             == editCustomer.FirstName.ToLower()
                                    && customer.LastName.ToLower()
                                            == editCustomer.LastName.ToLower()
                                    && customer.Phone.ToLower()
                                            == editCustomer.Phone.ToLower());

                // Add an error message if a match is found.
                if (customerExist)
                {
                    result.AddError(new BYSResults.Error("Existing Customer Data",
                                "A customer with the same first name, last name," +
                                " and phone number already exists in the database!"));
                }
            }

            // A different way to check if any errors have been encountered
            //		is to check the IsFailure boolean in the result object.
            //		Effectively it checks to see if there are any Errors in the
            //		collection.
            if (result.IsFailure)
            {
                return result;
            }
            #endregion

            // If we get to this point, then we have valid data and no business
            //		rules have been violated.  Time to see if we have a customer
            //		matching the provided customer ID.
            Customer customer = _hogWildContext.Customers
                                .Where(x => x.CustomerID == editCustomer.CustomerID)
                                .FirstOrDefault();

            // If there is no matching customer, then we are dealing with a
            //		new customer and so we must create a new entity instance
            //		to populate.
            if (customer == null)
            {
                customer = new Customer();
            }

            // Transfer all customer information from the view model
            //		into the customer entity.
            customer.FirstName = editCustomer.FirstName;
            customer.LastName = editCustomer.LastName;
            customer.Address1 = editCustomer.Address1;
            customer.Address2 = editCustomer.Address2;
            customer.City = editCustomer.City;
            customer.ProvStateID = editCustomer.ProvStateID;
            customer.CountryID = editCustomer.CountryID;
            customer.PostalCode = editCustomer.PostalCode;
            customer.Email = editCustomer.Email;
            customer.Phone = editCustomer.Phone;
            customer.StatusID = editCustomer.StatusID;
            customer.RemoveFromViewFlag = editCustomer.RemoveFromViewFlag;

            // Save the customer information to the database
            if (customer.CustomerID <= 0)
            {
                // If the ID was 0, we are performing an insert
                _hogWildContext.Customers.Add(customer);
            }
            else
            {
                // If we found an existing customer, the ID will not be 0,
                //		so we perform an update.
                _hogWildContext.Customers.Update(customer);
            }

            // Now we try to submit the information to the database.
            try
            {
                // If all is successful, the following will be executed and
                //		the catch will be skipped.
                _hogWildContext.SaveChanges();
            }
            catch (Exception ex)
            {
                // If there was an error communicating with the database,
                //		or a data constraint that we did not account for
                //		throws an error on the database side during the
                //		SaveChanges() operation, we will end up in the catch.

                // Clear the ChangeTracker so that future operations are not
                //		interfered with.
                _hogWildContext.ChangeTracker.Clear();

                // Populate an Error with the most inner exception message.
                // Alternately, you may rethrow the exception and get the 
                //		inner exception message in the code behind.  To do
                //		a rethrow, just use "throw ex;".  If rethrowing,
                //		you may skip the following couple lines of code.
                result.AddError(new BYSResults.Error("Error saving changes",
                    HelperMethods.GetInnerMostException(ex).Message));

                return result;
            }

            // The only way we can get to this point is to have a successful
            //		operation.  So, retrieve the an updated CustomerEditView
            //		instance and return it through the Result instance.
            return GetCustomer(customer.CustomerID);
        }

        // This method will retrieve all information for a single customer
        //		that has the matching customer ID
        public Result<CustomerEditView> GetCustomer(int customerID)
        {
            var result = new Result<CustomerEditView>();

            #region Data Validation and BusinessRules - This time combined

            // Rule: Provided customer ID must be valid.  Greater then zero.
            if (customerID <= 0)
            {
                result.AddError(new BYSResults.Error("Missing Information!",
                            "Customer ID must be greater than zero!"));

                // Only one check needed, so if this condition occurs, return
                //		immediately.  No point in carrying on.

                return result;
            }

            #endregion

            // In this case, we wish to retrieve all pieces of information
            //		for the customer matching the provided customer ID.
            var customer = _hogWildContext.Customers
                           .Where(c => c.CustomerID == customerID
                                       && !c.RemoveFromViewFlag)
                           .Select(c => new CustomerEditView
                           {
                               CustomerID = c.CustomerID,
                               FirstName = c.FirstName,
                               LastName = c.LastName,
                               Address1 = c.Address1,
                               Address2 = c.Address2,
                               City = c.City,
                               ProvStateID = c.ProvStateID,
                               CountryID = c.CountryID,
                               PostalCode = c.PostalCode,
                               Phone = c.Phone,
                               Email = c.Email,
                               StatusID = c.StatusID,
                               RemoveFromViewFlag = c.RemoveFromViewFlag
                           })
                           .FirstOrDefault();

            // If no matching customer is found, return an error message
            if (customer == null)
            {
                result.AddError(new BYSResults.Error("No Customer!",
                            "There is no customer with the provided customer ID!"));

                // No other errors possible at this point, so return immediately.
                return result;
            }

            // If we make it to here, we have customer information to return, so
            //		send back the populated CustomerEditView.
            return result.WithValue(customer);
        }



        public Result<List<CustomerSearchView>>
                                GetCustomers(string lastName, string phone)
        {
            var result = new Result<List<CustomerSearchView>>();

            #region Data Validation and BusinessRules - This time combined

            // Rule: lastName and phone may not both be empty or white space
            if (string.IsNullOrWhiteSpace(lastName) &&
                                    string.IsNullOrWhiteSpace(phone))
            {
                result.AddError(new BYSResults.Error("Missing Information!",
                            "Must provide either last name or phone number!"));

                // Only one check needed, so if this condition occurs, return
                //		immediately.  No point in carrying on.
                return result;
            }

            #endregion

            // In this case, we wish to retrieve all customers where at
            //		there is a partial match to the lastName or phone
            //		provided.  RemoveFromViewFlag must be false.
            var customers = _hogWildContext.Customers
                            .Where(c => (string.IsNullOrWhiteSpace(lastName)
                                            || c.LastName.ToLower()
                                                .Contains(lastName.ToLower()))
                                     && (string.IsNullOrWhiteSpace(phone)
                                             || c.Phone.Contains(phone))
                                     && !c.RemoveFromViewFlag)
                            .Select(c => new CustomerSearchView
                            {
                                CustomerID = c.CustomerID,
                                FirstName = c.FirstName,
                                LastName = c.LastName,
                                City = c.City,
                                Phone = c.Phone,
                                Email = c.Email,
                                StatusID = c.StatusID,
                                TotalSales = c.Invoices.Sum(i =>
                                        ((decimal?)(i.SubTotal + i.Tax) ?? 0))
                            })
                            .OrderBy(anon => anon.LastName)
                            .ToList();

            // If no customers were found, return an error message
            if (customers == null || customers.Count == 0)
            {
                result.AddError(new BYSResults.Error("No Customers!",
                        "No customers were found matching the provided search values!"));

                // No other errors possible at this point, so return immediately.
                return result;
            }

            // If we make it to here, we have customer information to return, so
            //		send back the List<CustomerSearchView>.
            return result.WithValue(customers);
        }
    }
}
