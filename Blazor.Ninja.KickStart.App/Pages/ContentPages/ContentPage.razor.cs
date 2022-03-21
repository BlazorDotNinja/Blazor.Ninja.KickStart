using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blazor.Ninja.Common.Client;
using Blazor.Ninja.Common.Data;
using Blazor.Ninja.Sdk.AspNetCore;
using Microsoft.AspNetCore.Components.Forms;

namespace Blazor.Ninja.KickStart.App.Pages.ContentPages
{
    public partial class ContentPage
    {
        private byte[] _data;
        private string _label;
        private string _contentType;
        private string _filename;

        private IContentProxy _proxy;

        private const long MaxFileSize = 16777216;
        private const long ChunkSize = 100000;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            State = BlazorNinjaComponentState.Loading;

            var token = await GetContextTokenAsync();

            _proxy = ProxyFactory.GetContentProxy(token, "OwlPhoto");

            State = BlazorNinjaComponentState.WaitingForInput;
        }

        public async Task OnCreateClickedAsync()
        {
            try
            {
                State = BlazorNinjaComponentState.Working;

                var content = new Content
                {
                    ContentType = _contentType
                };
                content.MetaData.Add("Title", _label);
                content.MetaData.Add("FileName", _filename);
                content = await _proxy.CreateAsync(content);

                var index = 0;
                while (true)
                {
                    var length = Math.Min(ChunkSize, _data.Length - index * ChunkSize);
                    if (length <= 0) break;

                    var chunk = new byte[length];

                    Array.Copy(_data, index * ChunkSize, chunk, 0, length);

                    await _proxy.AppendDataAsync(content.Id, chunk);

                    index++;
                }

                State = BlazorNinjaComponentState.WaitingForInput;

                NavigationManager.NavigateTo("/content");
            }
            catch (Exception ex)
            {
                State = BlazorNinjaComponentState.Failed;

                await HandleExceptionAsync(ex);
            }
        }

        private async Task OnFileSelectedAsync(
            InputFileChangeEventArgs args)
        {
            try
            {
                State = BlazorNinjaComponentState.Working;

                await using (var ms = new MemoryStream())
                {
                    await args.File.OpenReadStream(MaxFileSize).CopyToAsync(ms);

                    _data = ms.ToArray();
                }

                _contentType = args.File.ContentType;

                _filename = args.File.Name;

                State = BlazorNinjaComponentState.WaitingForInput;
            }
            catch (Exception ex)
            {
                State = BlazorNinjaComponentState.Failed;

                await HandleExceptionAsync(ex);
            }
        }
    }
}
