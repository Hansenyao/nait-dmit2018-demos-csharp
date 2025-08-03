using Microsoft.AspNetCore.Components;

namespace ExampleMudWebApp.Components.Pages.SamplePages
{
	public partial class InvoiceEdit
	{
		[Parameter]
		public int CustomerID { get; set; }

		[Parameter]
		public int InvoiceID { get; set; }

		[Parameter]
		public int EmployeeID { get; set; }
	}
}
