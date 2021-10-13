using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blazor.Ninja.Common.Data;
using Blazor.Ninja.Common.Data.System;
using Blazor.Ninja.Common.Meta;
using Blazor.Ninja.KickStart.Common;
using Blazor.Ninja.Sdk.Vm;

namespace Blazor.Ninja.KickStart.App.Vm
{
	public class ToDoTaskListVm : ToDoTaskVm
	{
		private readonly string _authentication;

		public int Limit { get; private set; }
		public int RefreshInterval { get; private set; }
		public GenericUser ContextUser { get; private set; }
		public List<ToDoTask> Items { get; private set; }
		public bool HasMore { get; private set; }


		public ToDoTaskListVm(
			string authentication,
			int limit,
			int refreshInterval = 0)
		{
			if (string.IsNullOrWhiteSpace(authentication)) throw new ArgumentException(nameof(authentication));
			if (limit <= 0) throw new ArgumentException(nameof(limit));
			if (refreshInterval < 0) throw new ArgumentException(nameof(refreshInterval));

			_authentication = authentication;

			Limit = limit;
			RefreshInterval = refreshInterval;

			Items = new List<ToDoTask>();
		}

		public override async Task LoadAsync()
		{
			try
			{
				await base.LoadAsync();

				State = BlazorNinjaComponentState.Loading;

				var userProxy = ProxyFactory.GetUserProxy<GenericUser>(_authentication);

				if (!await userProxy.HasUserContextAsync()) throw ExceptionBuilder.GetInstance(BlazorNinjaStatusCode.OperationIsNotValid, "user context is required");

				// Get context user
				ContextUser = await userProxy.GetAsync();

				var openStatusId = GetStatusId("open");

				// Get tickets
				var filter = Builders<ToDoTask>.Filter.Eq(it => it.OwnerId, ContextUser.Id)
					& Builders<ToDoTask>.Filter.Eq(it => it.StatusId, openStatusId);
				var sort = Builders<ToDoTask>.Sort.Ascending(it => it.DueDate);

				var proxy = ProxyFactory.GetTicketProxy<ToDoTask>(_authentication);
				var frameResult = await proxy.GetFrameAsync(filter, 0, Math.Max(Limit, Items.Count), sort);

				HasMore = frameResult.HasMore;
				Items = frameResult.Items;

				if (RefreshInterval > 0)
				{
					var _ = Task.Run(async delegate
					{
						await Task.Delay(TimeSpan.FromSeconds(RefreshInterval));
						await LoadAsync();
					});
				}

				State = BlazorNinjaComponentState.WaitingForInput;
			}
			catch (Exception ex)
			{
				State = BlazorNinjaComponentState.LoadFailed;

				RaiseFailed(ex);
			}
		}

		public async Task UpdateStatusAsync(
			string ticketId, 
			string statusId)
		{
			if (string.IsNullOrWhiteSpace(ticketId)) throw new ArgumentException(nameof(ticketId));
			if (string.IsNullOrWhiteSpace(statusId)) throw new ArgumentException(nameof(statusId));

			try
			{
				State = BlazorNinjaComponentState.Working;

				var item = Items.FirstOrDefault(it => it.Id == ticketId);
				if (item == null) throw ExceptionBuilder.GetInstance(BlazorNinjaStatusCode.NotFound, "item");

				var openStatusId = GetStatusId("open");

				var ticketProxy = ProxyFactory.GetTicketProxy<ToDoTask>(_authentication);
				await ticketProxy.UpdateStatusAsync(ticketId, statusId);

				item.StatusId = statusId;

				if (item.StatusId != openStatusId)
				{
					Items.Remove(item);

					if (Items.Count == Limit - 1)
					{
						var filter = Builders<ToDoTask>.Filter.Eq(it => it.OwnerId, ContextUser.Id)
						             & Builders<ToDoTask>.Filter.Eq(it => it.StatusId, openStatusId);
						var sort = Builders<ToDoTask>.Sort.Descending(it => it.Created);
						var frameResult = await ticketProxy.GetFrameAsync(filter, Items.Count, 1, sort);

						HasMore = frameResult.HasMore;
						Items.AddRange(frameResult.Items);
					}
				}

				State = BlazorNinjaComponentState.WaitingForInput;
			}
			catch (Exception ex)
			{
				State = BlazorNinjaComponentState.WaitingForInput;

				RaiseFailed(ex);
			}
		}

		public async Task LoadMoreAsync()
		{
			try
			{
				State = BlazorNinjaComponentState.Working;

				// Get tickets
				var filter = Builders<ToDoTask>.Filter.Empty;
				var sort = Builders<ToDoTask>.Sort.Descending(it => it.Created);
				var proxy = ProxyFactory.GetTicketProxy<ToDoTask>(_authentication);
				var frameResult = await proxy.GetFrameAsync(filter, Items.Count, Limit, sort);

				HasMore = frameResult.HasMore;
				Items.AddRange(frameResult.Items);

				State = BlazorNinjaComponentState.WaitingForInput;
			}
			catch (Exception ex)
			{
				State = BlazorNinjaComponentState.LoadFailed;

				RaiseFailed(ex);
			}
		}

		public async Task DeleteAsync(
			string ticketId)
		{
			if (string.IsNullOrWhiteSpace(ticketId)) throw new ArgumentException(nameof(ticketId));

			try
			{
				State = BlazorNinjaComponentState.Working;

				Items.RemoveAll(it => it.Id == ticketId);

				var proxy = ProxyFactory.GetTicketProxy<ToDoTask>(_authentication);

				await proxy.DeleteAsync(ticketId);

				var filter = Builders<ToDoTask>.Filter.Empty;
				var sort = Builders<ToDoTask>.Sort.Descending(it => it.Created);
				var frameResult = await proxy.GetFrameAsync(filter, Items.Count, 1, sort);

				HasMore = frameResult.HasMore;
				Items.AddRange(frameResult.Items);

				State = BlazorNinjaComponentState.WaitingForInput;
			}
			catch (Exception ex)
			{
				State = BlazorNinjaComponentState.WaitingForInput;

				RaiseFailed(ex);
			}
		}
	}
}
