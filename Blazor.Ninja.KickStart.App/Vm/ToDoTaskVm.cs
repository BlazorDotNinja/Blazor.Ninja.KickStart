using System;
using System.Linq;
using Blazor.Ninja.Common.Data.System;
using Blazor.Ninja.Common.Meta;
using Blazor.Ninja.Sdk.Vm;

namespace Blazor.Ninja.KickStart.App.Vm
{
	public abstract class ToDoTaskVm : BlazorNinjaComponentVm
	{
		public string GetDateLabel(DateTime? value)
		{
			if (!value.HasValue) return "";

			if (value.Value.Date == DateTime.Today.Date) return "today";

			if (value.Value.Date == DateTime.Today.Date.AddDays(1)) return "tomorrow";

			if (value.Value.Date.Year == DateTime.Today.Year) return value.Value.ToLocalTime().ToString("MMMM d");

			return value.Value.ToLocalTime().Date.ToString("MMMM d, yyyy");
		}

		public string GetInverseStatusId(string statusId)
		{
			if (string.IsNullOrWhiteSpace(statusId)) throw new ArgumentException(nameof(statusId));

			var feature = ProxyFactory.GetConfigurationProxy().GetFeature<TicketFeature>();

			var status = feature.StatusConfigurations.FirstOrDefault(it => it.Id == statusId);
			if (status == null) throw ExceptionBuilder.GetInstance(BlazorNinjaStatusCode.NotFound, "status");

			if (string.Equals(status.Label, "open", StringComparison.InvariantCultureIgnoreCase))
			{
				var inverseStatus = feature.StatusConfigurations.FirstOrDefault(
					it => it.Label.ToLowerInvariant() == "resolved");
				if (inverseStatus == null) throw ExceptionBuilder.GetInstance(BlazorNinjaStatusCode.NotFound, "status");

				return inverseStatus.Id;
			}

			if (string.Equals(status.Label, "resolved", StringComparison.InvariantCultureIgnoreCase))
			{
				var inverseStatus = feature.StatusConfigurations.FirstOrDefault(
					it => it.Label.ToLowerInvariant() == "open");
				if (inverseStatus == null) throw ExceptionBuilder.GetInstance(BlazorNinjaStatusCode.NotFound, "status");

				return inverseStatus.Id;
			}

			throw new NotImplementedException();
		}

		public bool IsOpen(string statusId)
		{
			var feature = ProxyFactory.GetConfigurationProxy().GetFeature<TicketFeature>();

			var status = feature.StatusConfigurations.FirstOrDefault(it => it.Id == statusId);
			if (status == null) throw ExceptionBuilder.GetInstance(BlazorNinjaStatusCode.NotFound, "status");

			return string.Equals(status.Label, "open", StringComparison.InvariantCultureIgnoreCase);
		}

		public bool IsResolved(string statusId)
		{
			var feature = ProxyFactory.GetConfigurationProxy().GetFeature<TicketFeature>();

			var status = feature.StatusConfigurations.FirstOrDefault(it => it.Id == statusId);
			if (status == null) throw ExceptionBuilder.GetInstance(BlazorNinjaStatusCode.NotFound, "status");

			return string.Equals(status.Label, "resolved", StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
