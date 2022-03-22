using System.Threading.Tasks;

using Blazor.Ninja.Sdk.AspNetCore;
using Blazor.Ninja.Sdk.AspNetCore.Components;

namespace Blazor.Ninja.KickStart.App.Pages
{
    public partial class DialogBoxPage
    {
	    private DialogBox _dialog;

	    private void OnButtonClicked()
	    {
		    _dialog.Show(OnConfirmedAsync, OnCanceledAsync);
	    }

		private async Task OnConfirmedAsync()
	    {
		    _dialog.Hide();

		    State = BlazorNinjaComponentState.Loading;

		    var token = await GetContextTokenAsync();

		    // TODO Get stuff done here

		    State = BlazorNinjaComponentState.WaitingForInput;
	    }

	    private async Task OnCanceledAsync()
	    {
		    await Task.Delay(0);
	    }
	}
}
