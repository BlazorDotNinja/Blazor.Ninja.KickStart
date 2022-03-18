using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;

using Blazor.Ninja.Common.Data;
using Blazor.Ninja.Common.Meta;
using Blazor.Ninja.KickStart.Common;
using Blazor.Ninja.Sdk.AspNetCore;

namespace Blazor.Ninja.KickStart.App.Components
{
	public partial class ToDoTaskForm
	{
		private IUser _contextUser;
		private ToDoTask _item;

		[Parameter]
		public string ItemId { get; set; }

		protected override async Task OnInitializedAsync()
		{
			await base.OnInitializedAsync();

			try
			{
				State = BlazorNinjaComponentState.Loading;

				var token = await GetContextTokenAsync();

				var userProxy = ProxyFactory.GetUserProxy<GenericUser>(token);
				if (!await userProxy.HasUserContextAsync()) throw ExceptionBuilder.GetInstance(BlazorNinjaStatusCode.OperationIsNotValid, "user context is required");

				_contextUser = await userProxy.GetAsync();

				if (string.IsNullOrWhiteSpace(ItemId))
				{
					_item = new ToDoTask
					{
						OwnerId = _contextUser.Id,
						TypeId = Feature.GetDefaultTypeId(),
						PriorityId = Feature.GetDefaultPriorityId(),
						DueDate = DateTime.UtcNow.Date,
						StatusId = GetStatusId("open")
					};
				}
				else
				{
					var proxy = ProxyFactory.GetTicketProxy<ToDoTask>(token);
					_item = await proxy.GetAsync(ItemId);
					if (_item == null) throw ExceptionBuilder.GetInstance(BlazorNinjaStatusCode.NotFound, "item");
				}

				State = BlazorNinjaComponentState.WaitingForInput;
			}
			catch (Exception ex)
			{
				State = BlazorNinjaComponentState.Failed;

				RaiseFailed(ex);
			}
		}

		public async Task SaveAsync()
		{
			try
			{
				State = BlazorNinjaComponentState.Working;

				var token = await GetContextTokenAsync();

				var proxy = ProxyFactory.GetTicketProxy<ToDoTask>(token);

				if (_item.IsNew())
				{
					_item = await proxy.CreateAsync(_item);
					ItemId = _item.Id;
				}
				else _item = await proxy.UpdateAsync(_item);

				RaiseDone(true);
			}
			catch (Exception ex)
			{
				State = BlazorNinjaComponentState.WaitingForInput;

				RaiseFailed(ex);
			}
		}
	}
}
