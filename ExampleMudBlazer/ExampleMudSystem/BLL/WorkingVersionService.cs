using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExampleMudSystem.DAL;

using ExampleMudSystem.ViewModels;

namespace ExampleMudSystem.BLL
{
    public class WorkingVersionService
    {
        private readonly OLTPDMIT2018Context _context;
        internal WorkingVersionService(OLTPDMIT2018Context context) 
        { 
            _context = context;
        }

        // Retrieve Working Version view
        public WorkingVersionsView GetWorkingVersion()
        {
            return _context.WorkingVersions
                        .Select(x => new WorkingVersionsView
                        {
                            VersionID = x.VersionId,
                            Major = x.Major,
                            Minor = x.Minor,
                            Build = x.Build,
                            Revision = x.Revision,
                            AsOfDate = x.AsOfDate,
                            Comments = x.Comments

                        })
                        .FirstOrDefault();
        }
    }
}
