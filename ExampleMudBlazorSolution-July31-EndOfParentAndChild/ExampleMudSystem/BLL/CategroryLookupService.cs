using ExampleMudSystem.DAL;
using ExampleMudSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BYSResults;

namespace ExampleMudSystem.BLL
{
	public class CategoryLookupService
	{
		// We must store the "context", meaning database connection, for 
		//		your methods in this class to use to perform transactional
		//		operations against your database.  The CRUD operations...
		private readonly HogWildContext _hogWildContext;

		// This constructor is required because we want to check for a 
		//		valid context as part of the successful creation of the 
		//		class instance.  The short-hand version used in the 
		//		CodeBehind class does not allow this check to be performed.
		internal CategoryLookupService(HogWildContext context)
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

		public Result<List<LookupView>> GetLookups(string categoryName)
		{
			var result = new Result<List<LookupView>>();

			var lookups = _hogWildContext.Lookups
					.Where(x => x.Category.CategoryName == categoryName)
					.OrderBy(x => x.Name)
					.Select(x => new LookupView
					{
						LookupID = x.LookupID,
						CategoryID = x.CategoryID,
						Name = x.Name,
						RemoveFromViewFlag = x.RemoveFromViewFlag
					})
					.ToList();

			if (lookups == null)
			{
				result.AddError(new Error("There were no lookups for the category provided!"));

				return result;
			}

			return result.WithValue(lookups);
		}
	}
}
