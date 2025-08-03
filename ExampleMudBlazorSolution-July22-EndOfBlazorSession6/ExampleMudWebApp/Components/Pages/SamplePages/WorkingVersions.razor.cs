using ExampleMudSystem.BLL;
using ExampleMudSystem.ViewModels;
using Microsoft.AspNetCore.Components;

namespace ExampleMudWebApp.Components.Pages.SamplePages
{
    public partial class WorkingVersions
    {
        #region Properties

        [Inject]
        protected WorkingVersionService WorkingVersionService { get; set; }

        #endregion

        #region Fields or Data Members

        private WorkingVersionsView workingVersionsView = new WorkingVersionsView();

        private string feedback = string.Empty;

        #endregion

        #region Methods

        private void GetWorkingVersion()
        {
            try
            {
                workingVersionsView = WorkingVersionService.GetWorkingVersion();
            }
            catch(Exception ex)
            {
                feedback = ex.Message;
            }
        }

        #endregion
    }
}
