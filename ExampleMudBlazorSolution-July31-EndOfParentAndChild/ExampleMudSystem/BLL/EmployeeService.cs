using ExampleMudSystem.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleMudSystem.BLL
{
	public class EmployeeService
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
		internal EmployeeService(HogWildContext context)
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

		public string GetEmployeeFullName(int employeeID)
		{
			return _hogWildContext.Employees
								.Where(x => x.EmployeeID == employeeID
										&& !x.RemoveFromViewFlag)
								.Select(x => $"{x.FirstName} {x.LastName}")
								.FirstOrDefault()
										?? string.Empty;
		}
	}
}
