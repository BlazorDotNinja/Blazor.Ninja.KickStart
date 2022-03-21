using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;

using Syncfusion.Blazor.Grids;

using Blazor.Ninja.Common.Data;
using Blazor.Ninja.Common.Meta;
using Blazor.Ninja.KickStart.Common;
using Blazor.Ninja.Sdk.AspNetCore;

namespace Blazor.Ninja.KickStart.App.Components
{
    public partial class ToDoTaskList
    {
	    private GenericUser _contextUser;
	    private SfGrid<ToDoTask> _grid;

	    private List<ToDoTask> _items = new();

	    private bool _hasMore;

	    [Parameter]
	    public int Limit { get; set; }

	    protected override async Task OnInitializedAsync()
	    {
		    await base.OnInitializedAsync();

		    await LoadAsync();
	    }

	    protected override async Task LoadAsync()
	    {
			try
			{
				State = BlazorNinjaComponentState.Loading;

				var token = await GetContextTokenAsync();

				var userProxy = ProxyFactory.GetUserProxy<GenericUser>(token);

				if (!await userProxy.HasUserContextAsync()) throw ExceptionBuilder.GetInstance(BlazorNinjaStatusCode.OperationIsNotValid, "user context is required");

				// Get context user
				_contextUser = await userProxy.GetAsync();

				var openStatusId = GetStatusId("open");

				// Get tickets
				var filter = Builders<ToDoTask>.Filter.Eq(it => it.OwnerId, _contextUser.Id)
				             & Builders<ToDoTask>.Filter.Eq(it => it.StatusId, openStatusId);
				var sort = Builders<ToDoTask>.Sort.Ascending(it => it.DueDate);

				var proxy = ProxyFactory.GetTicketProxy<ToDoTask>(token);
				var frameResult = await proxy.GetFrameAsync(filter, 0, Math.Max(Limit, _items.Count), sort);
				_items = frameResult.Items;
				_hasMore = frameResult.HasMore;

				State = BlazorNinjaComponentState.WaitingForInput;
			}
			catch (Exception ex)
			{
				State = BlazorNinjaComponentState.Failed;

				RaiseFailed(ex);
			}
		}

	    private async void OnMoreClickedAsync()
	    {
			try
			{
				State = BlazorNinjaComponentState.Working;

				var token = await GetContextTokenAsync();

				// Get tickets
				var filter = Builders<ToDoTask>.Filter.Empty;
				var sort = Builders<ToDoTask>.Sort.Descending(it => it.Created);
				var proxy = ProxyFactory.GetTicketProxy<ToDoTask>(token);
				var frameResult = await proxy.GetFrameAsync(filter, _items.Count, Limit, sort);

				_items = frameResult.Items;
				_hasMore = frameResult.HasMore;

				State = BlazorNinjaComponentState.WaitingForInput;
			}
			catch (Exception ex)
			{
				State = BlazorNinjaComponentState.Failed;

				RaiseFailed(ex);
			}
		}

	    private string GetDueDateCssClass(DateTime? dueDate)
	    {
		    if (!dueDate.HasValue) return default;

		    if (dueDate.Value.Date >= DateTime.Now.Date) return default;

		    return "past-due";
	    }

	    private async Task ChangedStatusAsync(string taskId, string statusId)
	    {
			try
			{
				State = BlazorNinjaComponentState.Working;

				var token = await GetContextTokenAsync();

				var item = _items.FirstOrDefault(it => it.Id == taskId);
				if (item == null) throw ExceptionBuilder.GetInstance(BlazorNinjaStatusCode.NotFound, "item");

				var openStatusId = GetStatusId("open");

				var ticketProxy = ProxyFactory.GetTicketProxy<ToDoTask>(token);
				await ticketProxy.UpdateStatusAsync(taskId, statusId);

				item.StatusId = statusId;

				if (item.StatusId != openStatusId)
				{
					(_items).Remove(item);

					if (_items.Count == Limit - 1)
					{
						var filter = Builders<ToDoTask>.Filter.Eq(it => it.OwnerId, _contextUser.Id)
						             & Builders<ToDoTask>.Filter.Eq(it => it.StatusId, openStatusId);
						var sort = Builders<ToDoTask>.Sort.Descending(it => it.Created);
						var frameResult = await ticketProxy.GetFrameAsync(filter, _items.Count, 1, sort);

						_items = frameResult.Items;
						_hasMore = frameResult.HasMore;
					}
				}

				State = BlazorNinjaComponentState.WaitingForInput;
			}
			catch (Exception ex)
			{
				State = BlazorNinjaComponentState.WaitingForInput;

				RaiseFailed(ex);
			}

			_grid.Refresh();
	    }
	}
}
