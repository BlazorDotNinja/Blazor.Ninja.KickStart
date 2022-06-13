using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Blazor.Ninja.Common.Data;
using Blazor.Ninja.Sdk.AspNetCore.Components;

namespace Blazor.Ninja.KickStart.App.Pages.DataPages
{
	public partial class DataForm2Page
	{
        private bool _isInitialized;

        private DataForm<GenericIdDataObject> _dataForm;

        private string _projectId;

        private List<GenericIdDataObject> _categories;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            var token = await GetContextTokenAsync();

            var categoryProxy = ProxyFactory.GetDataProxyForNamespace<GenericIdDataObject>(token, "ProjectCategory");

            var categoryPage = await categoryProxy.GetPageAsync(
                Filter<GenericIdDataObject>.Empty,
                0,
                int.MaxValue,
                SortOrder<GenericIdDataObject>.Empty);

            _categories = categoryPage.Items;

            var projectProxy = ProxyFactory.GetDataProxyForNamespace<GenericIdDataObject>(token, "Project");

            var project = await projectProxy.GetOneAsync(Filter<GenericIdDataObject>.Empty);

            _projectId = project?.Id;

            _isInitialized = true;
        }

        private async Task<List<GenericIdDataObject>> OnReferenceValueClickedAsync(
            string fieldLabel)
        {
            await Task.Delay(0);

            if (fieldLabel == "ProjectCategoryId") return _categories;

            throw new NotImplementedException();
        }

        private async Task<List<GenericIdDataObject>> OnReferenceSearchValueChangedAsync(
            string fieldLabel,
            string value)
        {
            await Task.Delay(0);

            if (fieldLabel == "ProjectCategoryId") return _categories?.Where(it => it.Get<string>("Label").StartsWith(value, StringComparison.InvariantCultureIgnoreCase)).ToList();

            throw new NotImplementedException();
        }

        private async Task<GenericIdDataObject> GetReferenceValueAsync(
            string fieldLabel,
            string id)
        {
            await Task.Delay(0);

            if (fieldLabel == "ProjectCategoryId") return _categories?.FirstOrDefault(it => it.Id == id);

            throw new NotImplementedException();
        }

        private void OnValueChanged(
            object sender,
            string fieldLabel)
        {
            if (fieldLabel == "ProjectCategoryId") _dataForm.SetValue("Text", default);
        }
    }
}
