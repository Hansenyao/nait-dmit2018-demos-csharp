#nullable disable
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
    public class CategoryLookupService
    {
        private readonly OLTPDMIT2018Context _context;
        public CategoryLookupService(OLTPDMIT2018Context context)
        {
            _context = context;
        }

        public Result<List<LookupView>> GetLookups(string categoryName)
        {
            var result = new Result<List<LookupView>>();

            var lookups = _context.Lookups
                    .Where(x => x.Category.CategoryName == categoryName)
                    .OrderBy(x => x.Category.CategoryName)
                    .Select(x => new LookupView { 
                        LookupID = x.LookupID,
                        CategoryID = x.Category.CategoryID,
                        Name = x.Name,
                        RemoveFromViewFlag = x.RemoveFromViewFlag
                    })
                    .ToList();
            if (lookups == null)
            {
                result.AddError(new Error("Ther were "));
            }

            return result;
        }
        
    }
}
