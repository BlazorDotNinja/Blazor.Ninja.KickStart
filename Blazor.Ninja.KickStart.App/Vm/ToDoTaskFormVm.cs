using System;
using System.Threading.Tasks;

using Blazor.Ninja.Common.Data;
using Blazor.Ninja.Common.Meta;
using Blazor.Ninja.KickStart.Common;
using Blazor.Ninja.Sdk.Vm;

namespace Blazor.Ninja.KickStart.App.Vm
{
	public class ToDoTaskFormVm : ToDoTaskVm
	{
		private readonly string _authentication;
		private string _itemId;
		private GenericUser _contextUser;
		public ToDoTask Item { get; private set; }

		public event EventHandler<Tuple<bool, ToDoTask>> Done;

		public ToDoTaskFormVm(string authentication, string itemId = default)
		{
			if (string.IsNullOrWhiteSpace(authentication)) throw new ArgumentException(nameof(authentication));

			_authentication = authentication;
			_itemId = itemId;
		}

		public override async Task LoadAsync()
		{
			try
			{
				await base.LoadAsync();

				State = BlazorNinjaComponentState.Loading;

				var userProxy = ProxyFactory.GetUserProxy<GenericUser>(_authentication);
				if (!await userProxy.HasUserContextAsync()) throw ExceptionBuilder.GetInstance(BlazorNinjaStatusCode.OperationIsNotValid, "user context is required");

				_contextUser = await userProxy.GetAsync();

				if (string.IsNullOrWhiteSpace(_itemId))
				{
					Item = new ToDoTask
					{
						OwnerId = _contextUser.Id,
						TypeId = _feature.GetDefaultTypeId(),
						PriorityId = _feature.GetDefaultPriorityId(),
						DueDate = DateTime.UtcNow.Date,
						StatusId = GetStatusId("open")
					};
				}
				else
				{
					var proxy = ProxyFactory.GetTicketProxy<ToDoTask>(_authentication);
					Item = await proxy.GetAsync(_itemId);
					if (Item == null) throw ExceptionBuilder.GetInstance(BlazorNinjaStatusCode.NotFound, "item");
				}

				State = BlazorNinjaComponentState.WaitingForInput;
			}
			catch (Exception ex)
			{
				State = BlazorNinjaComponentState.LoadFailed;

				RaiseFailed(ex);
			}
		}

		public async Task SaveAsync()
		{
			try
			{
				State = BlazorNinjaComponentState.Working;

				var proxy = ProxyFactory.GetTicketProxy<ToDoTask>(_authentication);

				if (string.IsNullOrWhiteSpace(Item.Id))
				{
					Item = await proxy.CreateAsync(Item);
					_itemId = Item.Id;
				}
				else Item = await proxy.UpdateAsync(Item);

				Raise(Done, new Tuple<bool, ToDoTask>(true, Item));
			}
			catch (Exception ex)
			{
				State = BlazorNinjaComponentState.WaitingForInput;

				RaiseFailed(ex);
			}
		}
	}
}
