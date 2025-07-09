#nullable disable
using BYSResults;
using HogWildSystem.DAL;
using HogWildSystem.Entities;
using HogWildSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HogWildSystem.BLL
{
    public class InvoiceService
    {
        #region Fields

        /// <summary>
        /// The hog wild context
        /// </summary>
        private readonly HogWildContext _hogWildContext;

        #endregion

        // Constructor for the InvoiceService class.
        internal InvoiceService(HogWildContext hogWildContext)
        {
            // Initialize the _hogWildContext field with the provided HogWildContext instance.
            _hogWildContext = hogWildContext;
        }

        //	Get the customer full name
        public string GetCustomerFullName(int customerID)
        {
            return _hogWildContext.Customers
                .Where(x => x.CustomerID == customerID && !x.RemoveFromViewFlag)
                .Select(x => $"{x.FirstName} {x.LastName}").FirstOrDefault() ?? string.Empty;
        }
        //	Get the employee full name
        public string GetEmployeeFullName(int employeeId)
        {
            {
                return _hogWildContext.Employees
                    .Where(x => x.EmployeeID == employeeId && !x.RemoveFromViewFlag)
                    .Select(x => $"{x.FirstName} {x.LastName}").FirstOrDefault() ?? string.Empty;
            }
        }
        //	Get the customer invoices
        public Result<List<InvoiceView>> GetCustomerInvoices(int customerId)
        {
            // Create a Result container that will hold either a
            //	PartView objects on success or any accumulated errors on failure
            var result = new Result<List<InvoiceView>>();
            #region Business Rules
            //	These are processing rules that need to be satisfied
            //		rule:	customerID must be valid
            //		rule: 	RemoveFromViewFlag must be false
            if (customerId == 0)
            {
                result.AddError(new Error("Missing Information",
                                "Please provide a valid customer id"));
                //  need to exit because we have no customer information
                return result;
            }
            #endregion

            var customerInvoices = _hogWildContext.Invoices
                    .Where(x => x.CustomerID == customerId
                                && !x.RemoveFromViewFlag)
                    .Select(x => new InvoiceView
                    {
                        InvoiceID = x.InvoiceID,
                        InvoiceDate = x.InvoiceDate,
                        CustomerID = x.CustomerID,
                        SubTotal = x.SubTotal,
                        Tax = x.Tax
                    }).ToList();

            //  if no invoices were found
            if (customerInvoices == null || customerInvoices.Count() == 0)
            {
                result.AddError(new Error("No customer invoices", "No invoices were found"));
                //  need to exit because we did not find any invoices
                return result;
            }
            //  return the result
            return result.WithValue(customerInvoices);
        }
        // Get invoice
        public Result<InvoiceView> GetInvoice(int invoiceID, int customerID, int employeeID)
	{
		// Create a Result container that will hold either a
		//	Invoice objects on success or any accumulated errors on failure
		var result = new Result<InvoiceView>();

		#region Business Rules
		//    These are processing rules that need to be satisfied
		//        for valid data
		//        rule:    both the customerID and empplyeeID must be provided 
		if (customerID == 0)
		{
			result.AddError(new Error("Missing Information",
				"Please provide a customer ID"));
		}
		if (employeeID == 0)
		{
			result.AddError(new Error("Missing Information",
				"Please provide a employee ID"));
		}
		if (result.IsFailure)
		{
			//  need to exit because we have no invoice record
			return result;
		}
		#endregion

		//	Handles both new and existing invoice
		//  For a new invoice the following information is needed
		//		Customer & Employee ID
		//  For a existing invoice the following information is needed
		//		Invoice & Employee ID (We maybe updating an invoice at a later date
		//			and we need the current employee who is handling the transaction.

		InvoiceView invoice;
		//  new invoice for customer
		if (invoiceID == 0)
		{
                invoice = new InvoiceView
                {
                    CustomerID = customerID,
                    EmployeeID = employeeID,
                    InvoiceDate = DateOnly.FromDateTime(DateTime.Now)
                };
		}
		else
		{
			invoice = _hogWildContext.Invoices
				.Where(x => x.InvoiceID == invoiceID
				 && !x.RemoveFromViewFlag
				)
				.Select(x => new InvoiceView
				{
					InvoiceID = invoiceID,
					InvoiceDate = x.InvoiceDate,
					CustomerID = x.CustomerID,
					EmployeeID = x.EmployeeID,
					SubTotal = x.SubTotal,
					Tax = x.Tax,
					InvoiceLines = _hogWildContext.InvoiceLines
						.Where(invoiceLine => invoiceLine.InvoiceID == invoiceID)
						.Select(invoiceLine => new InvoiceLineView
						{
							InvoiceLineID = invoiceLine.InvoiceLineID,
							InvoiceID = invoiceLine.InvoiceID,
							PartID = invoiceLine.PartID,
							Quantity = invoiceLine.Quantity,
							Description = invoiceLine.Part.Description,
							Price = invoiceLine.Price,
							Taxable = (bool)invoiceLine.Part.Taxable,
							RemoveFromViewFlag = invoiceLine.RemoveFromViewFlag
						}).ToList()
				}).FirstOrDefault() ?? new InvoiceView();
			customerID = invoice.CustomerID;
		}
		invoice.CustomerName = GetCustomerFullName(customerID);
		invoice.EmployeeName = GetEmployeeFullName(employeeID);

		//  only happens if the invoice was mark as remove
		if (invoice == null)
		{
			result.AddError(new Error("No Invoice", "No invoice were found"));
			//  need to exit because we did not find any invoice
			return result;
		}
		//  return the result
		return result.WithValue(invoice);
	}

        public Result<InvoiceView> AddEditInvoice(InvoiceView invoiceView)
        {
            // Create a Result container that will hold either a
            //	Invoice objects on success or any accumulated errors on failure
            var result = new Result<InvoiceView>();
            #region Business Logic and Parameter Exceptions
            //    These are processing rules that need to be satisfied
            //        for valid data
            // rule:    invoice cannot be null
            if (invoiceView == null)
            {
                result.AddError(new Error("Missing Invoice",
                        "No invoice was supply"));
                //  need to exit because we have no invoice record
                return result;
            }
            // rule:    customer id must be supply
            if (invoiceView.CustomerID == 0)
            {
                result.AddError(new Error("Missing Information",
                        "Please provide a valid customer ID"));
            }
            // rule:    employee id must be supply    
            if (invoiceView.EmployeeID == 0)
            {
                result.AddError(new Error("Missing Information",
                        "Please provide a valid employee ID"));
            }
            // rule:    there must be invoice lines provided
            if (invoiceView.InvoiceLines.Count == 0)
            {
                result.AddError(new Error("Missing Information",
                        "Invoice details are required"));
            }

            // rule:    for each invoice line, there must be a part
            // rule:    for each invoice line, the price cannot be less than zero
            // rule:    for each invoice line, the quantity cannot be less than 1
            foreach (var invoiceLine in invoiceView.InvoiceLines)
            {
                if (invoiceLine.PartID == 0)
                {
                    result.AddError(new Error("Missing Information",
                            "Missing part ID"));
                    //  need to exit because we have no part information to process further
                    return result;
                }
                if (invoiceLine.Price < 0)
                {
                    string partName = _hogWildContext.Parts
                                        .Where(x => x.PartID == invoiceLine.PartID)
                                        .Select(x => x.Description)
                                        .FirstOrDefault();
                    result.AddError(new Error("Invalid Price",
                            $"Part {partName} has a price that is less than zero"));
                }
                if (invoiceLine.Quantity < 1)
                {
                    string partName = _hogWildContext.Parts
                                        .Where(x => x.PartID == invoiceLine.PartID)
                                        .Select(x => x.Description)
                                        .FirstOrDefault();
                    result.AddError(new Error("Invalid Quantity",
                            $"Part {partName} has a quantity that is less than one"));
                }
            }

            // rule:    parts cannot be duplicated on more than one line.
            List<string> duplicatedParts = invoiceView.InvoiceLines
                                            .GroupBy(x => new { x.PartID })
                                            .Where(gb => gb.Count() > 1)
                                            .OrderBy(gb => gb.Key.PartID)
                                            .Select(gb => _hogWildContext.Parts
                                                            .Where(p => p.PartID == gb.Key.PartID)
                                                            .Select(p => p.Description)
                                                            .FirstOrDefault()
                                            ).ToList();
            if (duplicatedParts.Count > 0)
            {
                foreach (var partName in duplicatedParts)
                {
                    result.AddError(new Error("Duplicate Invoice Line Items",
                            $"Part {partName} can only be added to the invoice lines once."));
                }
            }

            //  exit if we have any outstanding errors
            if (result.IsFailure)
            {
                return result;
            }
            #endregion

            // Retrieve the invoice from the database or create a new one if it doesn't exist.
            Invoice invoice = _hogWildContext.Invoices
                                    .Where(x => x.InvoiceID == invoiceView.InvoiceID)
                                    .FirstOrDefault();
            // If the invoice doesn't exist, initialize it.
            if (invoice == null)
            {
                invoice = new Invoice();
                // Set the current date for new invoices.
                invoice.InvoiceDate = DateOnly.FromDateTime(DateTime.Now);
            }

            // Update invoice properties from the view model
            invoice.CustomerID = invoiceView.CustomerID;
            invoice.EmployeeID = invoiceView.EmployeeID;
            invoice.RemoveFromViewFlag = invoiceView.RemoveFromViewFlag;
            //  reset the subtotal & tax as this will be updated from the invoice lines.
            invoice.SubTotal = 0;
            invoice.Tax = 0;

            // Process each line item in the provided view model.
            foreach (var invoiceLineView in invoiceView.InvoiceLines)
            {
                InvoiceLine invoiceLine = _hogWildContext.InvoiceLines
                                                .Where(x => x.InvoiceLineID == invoiceLineView.InvoiceLineID
                                                        && !x.RemoveFromViewFlag)
                                                .FirstOrDefault();
                // If the line item doesn't exist, initialize it.
                if (invoiceLine == null)
                {
                    invoiceLine = new InvoiceLine();
                    invoiceLine.PartID = invoiceLineView.PartID;
                }
                // Update invoice line properties from the view model
                invoiceLine.Quantity = invoiceLineView.Quantity;
                invoiceLine.Price = invoiceLineView.Price;
                invoiceLine.RemoveFromViewFlag = invoiceLineView.RemoveFromViewFlag;

                // Handle new or existing line items.
                if (invoiceLine.InvoiceLineID == 0)
                {
                    // Add new line items to the invoice entity.
                    invoice.InvoiceLines.Add(invoiceLine);
                }
                else
                {
                    // Update the database record with the existing line items.
                    _hogWildContext.InvoiceLines.Update(invoiceLine);
                }

                //    need to update total and tax if the 
                //        invoice line item is not set to be removed from view.
                if (!invoiceLine.RemoveFromViewFlag)
                {
                    invoice.SubTotal += invoiceLine.Quantity * invoiceLine.Price;
                    bool isTaxable = _hogWildContext.Parts
                                        .Where(x => x.PartID == invoiceLine.PartID)
                                        .Select(x => x.Taxable)
                                        .FirstOrDefault();
                    invoice.Tax += isTaxable ? invoiceLine.Quantity * invoiceLine.Price * .05m : 0;
                }
            }
            // If it's a new invoice, add it to the collection.
            if (invoice.InvoiceID == 0)
            {
                //  add the invoice to the invoice table
                _hogWildContext.Invoices.Add(invoice);
            }
            else
            {
                //  update the invoice in the invoice table
                _hogWildContext.Invoices.Update(invoice);
            }

            try
            {
                // NOTE:  YOU CAN ONLY HAVE ONE SAVE CHANGES IN A METHOD  
                _hogWildContext.SaveChanges();
            }
            catch (Exception ex)
            {
                // Clear changes to maintain data integrity.
                _hogWildContext.ChangeTracker.Clear();
                // we do not have to throw an exception, just need to log the error message
                result.AddError(new Error(
                    "Error Saving Changes", ex.InnerException.Message));
                //  need to return the result
                return result;
            }
            //  need to refresh the customer information
            return GetInvoice(0, 0, 0);
        }
    }
}
