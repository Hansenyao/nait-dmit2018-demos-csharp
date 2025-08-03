using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ExampleMudWebApp.Components.Dialogues
{
	public partial class MessageDisplay
	{
		#region Fields

		private bool hasFeedback => !string.IsNullOrWhiteSpace(FeedbackMessage);
		private bool hasSingleError => !string.IsNullOrWhiteSpace(ErrorMessage);
		private bool hasMultipleErrors => ErrorMessages.Count > 0;

		#endregion


		#region Parameters

		[Parameter]
		public List<string> ErrorMessages { get; set; } = new List<string>();

		[Parameter]
		public string ErrorMessage { get; set; } = string.Empty;

		[Parameter]
		public string FeedbackMessage { get; set; } = string.Empty;

		#endregion
	}
}
