using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

using Blazor.Ninja.Common.Client;
using Blazor.Ninja.Sdk.AspNetCore;
using Blazor.Ninja.Common.Data;

namespace Blazor.Ninja.KickStart.App.Pages.ContentPages
{
    public partial class ContentListPage
    {
        private IContentProxy _proxy;

        private List<Content> _contents = new();

        private string _currentItemId;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            State = BlazorNinjaComponentState.Loading;

            var token = await GetContextTokenAsync();

            _proxy = ProxyFactory.GetContentProxy(token, "OwlPhoto");

            await LoadAsync();

            State = BlazorNinjaComponentState.WaitingForInput;
        }

        protected override async Task LoadAsync()
        {
	        var contentPage = await _proxy.GetPageAsync(0, int.MaxValue);

	        _contents = contentPage.Items;
        }

        private static string GetIconUrl(Content content)
        {
	        var imgUrl = content.ContentType switch
	        {
		        "application/pdf" => "_content/Blazor.Ninja.KickStart.App/images/pdf-image.png",
		        "application/octet-stream" => "_content/Blazor.Ninja.KickStart.App/images/export-csv.png",
		        _ => content.Url
	        };
	        return imgUrl;
        }

        private async Task OnDeleteAsync(string id)
        {
            State = BlazorNinjaComponentState.Working;

            await _proxy.DeleteAsync(id);

            await LoadAsync();

            State = BlazorNinjaComponentState.WaitingForInput;
        }

        private async Task OnDownloadAsync(string id, string contentType, string fileName)
        {
            State = BlazorNinjaComponentState.Working;

            var byteData = await _proxy.DownloadDataAsync(id, 0, int.MaxValue);

            await JSRuntime.InvokeVoidAsync(
                "downloadFile", fileName, contentType, byteData);

            State = BlazorNinjaComponentState.WaitingForInput;
        }

        public void OnUpdateEnterAsync(ChangeEventArgs e, string title, Content content)
        {
            if (e.Value != null) content.MetaData["Title"] = e.Value.ToString();
        }

        private async Task OnUpdateAsync(Content content)
        {
	        State = BlazorNinjaComponentState.Working;

            await _proxy.UpdateAsync(content);

            _currentItemId = default;

            State = BlazorNinjaComponentState.WaitingForInput;
        }

        private void OnEditAsync(string id)
        {
            _currentItemId = id;
        }

        private bool IsEditBtnVisible(string id)
        {
	        return _currentItemId != id;
        }

        private bool IsUpdateBtnVisible(string id)
        {
            return _currentItemId == id;
        }
    }
}
