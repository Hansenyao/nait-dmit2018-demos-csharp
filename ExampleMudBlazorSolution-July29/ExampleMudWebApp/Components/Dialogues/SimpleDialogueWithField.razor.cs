using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ExampleMudWebApp.Components.Dialogues
{
	public partial class SimpleDialogueWithField
	{
		#region Fields

		private string feedbackMessage = string.Empty;

		#endregion

		#region Parameters

		[CascadingParameter]
		private IMudDialogInstance MudDialog { get; set; }

		[Parameter]
		public string ButtonText { get; set; }

		[Parameter]
		public Color Color { get; set; } = Color.Primary;

		#endregion

		#region Methods

		private void Submit() => MudDialog.Close(DialogResult.Ok(feedbackMessage));

		private void Cancel() => MudDialog.Cancel();

		#endregion


	}
}
