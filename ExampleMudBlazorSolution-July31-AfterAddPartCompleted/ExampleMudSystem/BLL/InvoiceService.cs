using BYSResults;
using ExampleMudSystem.DAL;
using ExampleMudSystem.Entities;
using ExampleMudSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleMudSystem.BLL
{
	public class InvoiceService
	{
		#region Connection Setup
		// We must store the "context", meaning database connection, for 
		//		your methods in this class to use to perform transactional
		//		operations against your database.  The CRUD operations...
		private readonly HogWildContext _hogWildContext;

		// This constructor is required because we want to check for a 
		//		valid context as part of the successful creation of the 
		//		class instance.  The short-hand version used in the 
		//		CodeBehind class does not allow this check to be performed.
		internal InvoiceService(HogWildContext context)
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

		#endregion

		// This method will be used to handle both existing invoices and new invoices.
		public Result<InvoiceView> GetInvoice(int invoiceID, int customerID, int employeeID)
		{
			var result = new Result<InvoiceView>();

			// Just a couple checks here so as in previous methods they can all be combined under 
			//		one section.
			#region Data Validation and Business Rules

			// Rule:  Both customer ID and employee ID must be provided

			if (customerID == 0)
			{
				result.AddError(new Error("Missing Information!",
								"A customer ID must be provided!"));
			}

			if (employeeID == 0)
			{
				result.AddError(new Error("Missing Information!",
								"An employee ID must be provided!"));
			}

			if (result.IsFailure)
			{
				return result;
			}
			#endregion

			InvoiceView invoice;

			// If creating a new invoice
			if (invoiceID <= 0)
			{
				// Use initializer syntax for creating a new class instance of InvoiceView
				invoice = new InvoiceView()
				{
					CustomerID = customerID,
					EmployeeID = employeeID,
					InvoiceDate = DateOnly.FromDateTime(DateTime.Now)
				};
			}
			else // If retrieving information for a current invoice
			{
				invoice = _hogWildContext.Invoices
							.Where(x => x.InvoiceID == invoiceID && !x.RemoveFromViewFlag)
							.Select(x => new InvoiceView
							{
								InvoiceID = x.InvoiceID,
								InvoiceDate = new DateOnly(x.InvoiceDate.Year, x.InvoiceDate.Month, x.InvoiceDate.Day),
								CustomerID = x.CustomerID,
								EmployeeID = x.EmployeeID,
								Subtotal = x.SubTotal,
								Tax = x.Tax,
								InvoiceLines = _hogWildContext.InvoiceLines
													.Where(line => line.InvoiceID == invoiceID)
													.Select(line => new InvoiceLineView
													{
														InvoiceLineID = line.InvoiceLineID,
														InvoiceID = line.InvoiceID,
														PartID = line.PartID,
														Quantity = line.Quantity,
														Description = line.Part.Description,
														Price = line.Price,
														Taxable = line.Part.Taxable,
														RemoveFromViewFlag = line.RemoveFromViewFlag
													})
													.ToList()
							})
							.FirstOrDefault();

				if (invoice != null)
					customerID = invoice.CustomerID;
			}

			// If there was no invoice for a provided invoice ID.  No point in continuing,
			//		so return an error message to the calling scope.
			if (invoice == null)
			{
				result.AddError(new Error("No Invoice Found!",
						"There was no invoice found for the provided invoice ID!"));

				return result;
			}

			// Whether a new or existing invoice, retrieve the full names for the customer and employee
			//		using their respective services.
			CustomerService cust = new CustomerService(_hogWildContext);
			invoice.CustomerName = cust.GetCustomerFullName(invoice.CustomerID);
			EmployeeService emp = new EmployeeService(_hogWildContext);
			invoice.EmployeeName = emp.GetEmployeeFullName(invoice.EmployeeID);

			// Getting to this point means we have a valid invoice
			return result.WithValue(invoice);
		}

		// This method will be used to retrieve a single existing invoice independent of the
		//		customer and employee IDs
		public Result<InvoiceView> GetInvoice(int invoiceID)
		{
			var result = new Result<InvoiceView>();

			// Just a couple checks here so as in previous methods they can all be combined under 
			//		one section.
			#region Data Validation and Business Rules

			// Rule:  Invoice ID must be provided

			if (invoiceID <= 0)
			{
				result.AddError(new Error("Missing Information!",
								"An invoice ID must be provided!"));

				return result;
			}


			#endregion


			var invoice = _hogWildContext.Invoices
							.Where(x => x.InvoiceID == invoiceID && !x.RemoveFromViewFlag)
							.Select(x => new InvoiceView
							{
								InvoiceID = x.InvoiceID,
								InvoiceDate = new DateOnly(x.InvoiceDate.Year, x.InvoiceDate.Month, x.InvoiceDate.Day),
								CustomerID = x.CustomerID,
								EmployeeID = x.EmployeeID,
								Subtotal = x.SubTotal,
								Tax = x.Tax,
								InvoiceLines = _hogWildContext.InvoiceLines
													.Where(line => line.InvoiceID == invoiceID)
													.Select(line => new InvoiceLineView
													{
														InvoiceLineID = line.InvoiceLineID,
														InvoiceID = line.InvoiceID,
														PartID = line.PartID,
														Quantity = line.Quantity,
														Description = line.Part.Description,
														Price = line.Price,
														Taxable = line.Part.Taxable,
														RemoveFromViewFlag = line.RemoveFromViewFlag
													})
													.ToList()
							})
							.FirstOrDefault();


			// If there was no invoice for a provided invoice ID.  No point in continuing,
			//		so return an error message to the calling scope.
			if (invoice == null)
			{
				result.AddError(new Error("No Invoice Found!",
						"There was no invoice found for the provided invoice ID!"));

				return result;
			}

			// Whether a new or existing invoice, retrieve the full names for the customer and employee
			//		using their respective services.
			CustomerService cust = new CustomerService(_hogWildContext);
			invoice.CustomerName = cust.GetCustomerFullName(invoice.CustomerID);
			EmployeeService emp = new EmployeeService(_hogWildContext);
			invoice.EmployeeName = emp.GetEmployeeFullName(invoice.EmployeeID);

			// Getting to this point means we have a valid invoice
			return result.WithValue(invoice);
		}

		// This method will retrieve all existing invoices fro a provided customer ID
		public Result<List<InvoiceView>> GetCustomerInvoices(int customerID)
		{
			var result = new Result<List<InvoiceView>>();

			// In this case only a single test is required for the incoming data, so the 
			//		data validation and business rules may be combined.
			#region Data Validation and Business Rules

			// If the provided customer ID is invalid,
			//		immediately return with the following error message for the user.
			if (customerID <= 0)
			{
				result.AddError(new Error("Missing Information!",
							"Please provide a valid customer ID!"));

				return result;
			}

			#endregion

			var customerInvoices = _hogWildContext.Invoices
									.Where(x => x.CustomerID == customerID && !x.RemoveFromViewFlag)
									.Select(x => new InvoiceView
									{
										InvoiceID = x.InvoiceID,
										InvoiceDate = new DateOnly(x.InvoiceDate.Year, x.InvoiceDate.Month, x.InvoiceDate.Day),
										CustomerID = x.CustomerID,
										Subtotal = x.SubTotal,
										Tax = x.Tax
									})
									.ToList();

			// If no invoices were found.  Send back an error message to the calling scope.
			if (customerInvoices == null || customerInvoices.Count() == 0)
			{
				result.AddError(new Error("No Invoice Found!",
									"No invoices found for the provided customer ID!"));

				return result;
			}

			// return the retrieved invoices
			return result.WithValue(customerInvoices);
		}

		// This method will save invoice details to the database for either new or existing
		//		invoices, including the associated invoice line details
		public Result<InvoiceView> AddEditInvoice(InvoiceView invoiceView)
		{
			var result = new Result<InvoiceView>();

			#region Data Validation

			if (invoiceView == null)
			{
				result.AddError(new Error("Missing Invoice!", "No invoice information was provided!"));

				return result;
			}

			if (invoiceView.CustomerID <= 0)
			{
				result.AddError(new Error("Missing information!", "Invalid customer ID provided!"));
			}

			if (invoiceView.EmployeeID <= 0)
			{
				result.AddError(new Error("Missing information!", "Invalid employee ID provided!"));
			}

			if (invoiceView.InvoiceLines.Count == 0)
			{
				result.AddError(new Error("Missing information!", "Invoice line details are required!"));
			}

			foreach (var line in invoiceView.InvoiceLines)
			{
				// If a part ID provided is missing or invalid
				if (line.PartID <= 0)
				{
					// Indicate which InvoiceLine the part ID is missing from
					result.AddError(new Error("Missing information!",
							$"No part ID provided for invoice line {line.InvoiceLineID}"));
				}

				// If a provided price in invalid
				if (line.Price < 0)
				{
					// If there is a valid part ID, extract the part name from the database
					string partName = null;
					if (line.PartID > 0)
					{
						partName = _hogWildContext.Parts
									.Where(x => x.PartID == line.PartID)
									.Select(x => x.Description)
									.FirstOrDefault();
					}

					// Add an error letting the user know which part has an invalid price.  If no valid part
					//		ID, then return the InvoiceLine number.
					result.AddError(new Error("Invalid Price!",
						$"Price provided for {(line.PartID > 0 ? partName
										: "Invoice Line " + line.InvoiceLineID)} is less than zero!"));
				}

				// If a provided quantity in invalid
				if (line.Quantity < 1)
				{
					// If there is a valid part ID, extract the part name from the database
					string partName = null;
					if (line.PartID > 0)
					{
						partName = _hogWildContext.Parts
									.Where(x => x.PartID == line.PartID)
									.Select(x => x.Description)
									.FirstOrDefault();
					}

					// Add an error letting the user know which part has an invalid price.  If no valid part
					//		ID, then return the InvoiceLine number.
					result.AddError(new Error("Invalid Quantity!",
							$"Quantity provided for {(line.PartID > 0 ? partName
										: "Invoice Line " + line.InvoiceLineID)} is less than one!"));
				}

				List<string> duplicatedParts = invoiceView.InvoiceLines
												.GroupBy(x => new { x.PartID })
												.Where(group => group.Count() > 1)
												.OrderBy(group => group.Key.PartID)
												.Select(group => _hogWildContext.Parts
																.Where(p => p.PartID == group.Key.PartID)
																.Select(p => p.Description)
																.FirstOrDefault()
														)
												.ToList();

				if (duplicatedParts.Count > 0)
				{
					foreach (var partName in duplicatedParts)
					{
						result.AddError(new Error("Duplicate Part!",
							$"{partName} can only be added to invoice lines once!"));
					}
				}
			}

			// If any errors, exit
			if (result.IsFailure)
			{
				return result;
			}

			#endregion

			Invoice invoice = _hogWildContext.Invoices
								.Where(x => x.InvoiceID == invoiceView.InvoiceID)
								.FirstOrDefault();

			if (invoice == null)
			{
				invoice = new Invoice();
				invoice.InvoiceDate = DateTime.Now;
			}

			invoice.CustomerID = invoiceView.CustomerID;
			invoice.EmployeeID = invoiceView.EmployeeID;
			invoice.RemoveFromViewFlag = invoiceView.RemoveFromViewFlag;
			invoice.SubTotal = 0;
			invoice.Tax = 0;

			foreach (var line in invoiceView.InvoiceLines)
			{
				InvoiceLine invoiceLine = _hogWildContext.InvoiceLines
											.Where(x => x.InvoiceLineID == line.InvoiceLineID
														&& !x.RemoveFromViewFlag)
											.FirstOrDefault();

				if (invoiceLine == null)
				{
					invoiceLine = new InvoiceLine();
					invoiceLine.PartID = line.PartID;
				}

				invoiceLine.Quantity = line.Quantity;
				invoiceLine.Price = line.Price;
				invoiceLine.RemoveFromViewFlag = line.RemoveFromViewFlag;

				if (invoiceLine.InvoiceLineID <= 0)
				{
					invoice.InvoiceLines.Add(invoiceLine);
				}
				else
				{
					_hogWildContext.InvoiceLines.Update(invoiceLine);
				}

				if (!invoiceLine.RemoveFromViewFlag)
				{
					invoice.SubTotal += invoiceLine.Quantity * invoiceLine.Price;
					bool isTaxable = _hogWildContext.Parts
										.Where(x => x.PartID == invoiceLine.PartID)
										.Select(x => x.Taxable)
										.FirstOrDefault();
					invoice.Tax += isTaxable ? invoiceLine.Quantity * invoiceLine.Price * 0.05m : 0;
				}
			}

			if (invoice.InvoiceID <= 0)
			{
				_hogWildContext.Invoices.Add(invoice);
			}
			else
			{
				_hogWildContext.Invoices.Update(invoice);
			}

			try
			{
				_hogWildContext.SaveChanges();
			}
			catch (Exception ex)
			{
				_hogWildContext.ChangeTracker.Clear();

				result.AddError(new Error("Error Saving Changes!", HelperMethods.GetInnerMostException(ex).Message));

				return result;
			}

			return GetInvoice(invoice.InvoiceID);
		}
	}
}
