using System;
using System.Linq;
using System.Threading.Tasks;

using Blazor.Ninja.Common.Data.System;
using Blazor.Ninja.Common.Meta;
using Blazor.Ninja.Sdk.AspNetCore;

namespace Blazor.Ninja.KickStart.App.Components
{
	public abstract class ToDoTaskComponent : BlazorNinjaComponent
	{
		private TicketFeature _feature;
		protected TicketFeature Feature => _feature;

		protected override async Task OnInitializedAsync()
		{
			await base.OnInitializedAsync();

			_feature = await ProxyFactory.GetConfigurationProxy().GetFeatureAsync<TicketFeature>();
		}

		protected string GetDateLabel(DateTime? value)
		{
			if (!value.HasValue) return "";

			if (value.Value.Date == DateTime.Today.Date) return "today";

			if (value.Value.Date == DateTime.Today.Date.AddDays(1)) return "tomorrow";

			if (value.Value.Date.Year == DateTime.Today.Year) return value.Value.ToLocalTime().ToString("MMMM d");

			return value.Value.ToLocalTime().Date.ToString("MMMM d, yyyy");
		}

		protected string GetInverseStatusId(string statusId)
		{
			if (string.IsNullOrWhiteSpace(statusId)) throw new ArgumentException(nameof(statusId));

			var status = _feature.StatusConfigurations.FirstOrDefault(it => it.Id == statusId);
			if (status == null) throw ExceptionBuilder.GetInstance(BlazorNinjaStatusCode.NotFound, "status");

			if (string.Equals(status.Label, "open", StringComparison.InvariantCultureIgnoreCase))
			{
				var inverseStatus = _feature.StatusConfigurations.FirstOrDefault(
					it => it.Label.ToLowerInvariant() == "resolved");
				if (inverseStatus == null) throw ExceptionBuilder.GetInstance(BlazorNinjaStatusCode.NotFound, "status");

				return inverseStatus.Id;
			}

			if (string.Equals(status.Label, "resolved", StringComparison.InvariantCultureIgnoreCase))
			{
				var inverseStatus = _feature.StatusConfigurations.FirstOrDefault(
					it => it.Label.ToLowerInvariant() == "open");
				if (inverseStatus == null) throw ExceptionBuilder.GetInstance(BlazorNinjaStatusCode.NotFound, "status");

				return inverseStatus.Id;
			}

			throw new NotImplementedException();
		}

		protected string GetStatusId(string label)
		{
			if (string.IsNullOrWhiteSpace(label)) throw new ArgumentException(nameof(label));

			var status = _feature.StatusConfigurations.FirstOrDefault(it => it.Label.ToLowerInvariant() == label);
			if (status == null) throw ExceptionBuilder.GetInstance(BlazorNinjaStatusCode.NotFound, "status");

			return status.Id;
		}

		protected bool IsOpen(string statusId)
		{
			var status = _feature.StatusConfigurations.FirstOrDefault(it => it.Id == statusId);
			if (status == null) throw ExceptionBuilder.GetInstance(BlazorNinjaStatusCode.NotFound, "status");

			return string.Equals(status.Label, "open", StringComparison.InvariantCultureIgnoreCase);
		}

		protected bool IsResolved(string statusId)
		{
			var status = _feature.StatusConfigurations.FirstOrDefault(it => it.Id == statusId);
			if (status == null) throw ExceptionBuilder.GetInstance(BlazorNinjaStatusCode.NotFound, "status");

			return string.Equals(status.Label, "resolved", StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
