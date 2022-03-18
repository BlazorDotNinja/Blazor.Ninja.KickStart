using System.Threading.Tasks;

using Microsoft.JSInterop;
using Microsoft.AspNetCore.WebUtilities;

namespace Blazor.Ninja.KickStart.App.Pages
{
	public partial class ToDoTask
	{
		private string _itemId;
		private bool _canGoBack;

		protected override async Task OnInitializedAsync()
		{
			await base.OnInitializedAsync();

			var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
			QueryHelpers.ParseQuery(uri.Query).TryGetValue("id", out var id);
			_itemId = id;

			_canGoBack = await JSRuntime.InvokeAsync<bool>("canGoBack");
		}

		private async Task OnDoneAsync(bool wasUserInputNeeded)
		{
			await JSRuntime.InvokeVoidAsync("goBack");
		}

		private async Task OnBackClickedAsync()
		{
			await JSRuntime.InvokeVoidAsync("goBack");
		}

		private string GetHeader()
		{
			if (!string.IsNullOrWhiteSpace(_itemId)) return "Edit a Task";

			return "Add a Task";
		}
	}
}
