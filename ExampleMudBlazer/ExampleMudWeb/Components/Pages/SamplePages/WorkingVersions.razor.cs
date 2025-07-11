using ExampleMudSystem.BLL;
using ExampleMudSystem.ViewModels;
using Microsoft.AspNetCore.Components;

#nullable disable

namespace ExampleMudWeb.Components.Pages.SamplePages
{
    public partial class WorkingVersions
    {
        #region Properties
        [Inject]
        protected WorkingVersionService workingVersionService { get; set; }

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
                //workingVersionsView = workingVersionService.
            }
            catch (Exception ex) 
            { 
                feedback = ex.Message;
            }
        }
        #endregion

    }
}
