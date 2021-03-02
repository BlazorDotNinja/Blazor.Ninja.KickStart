using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using Blazor.Ninja.Common;
using Blazor.Ninja.Common.Data;
using Blazor.Ninja.Common.Factories;
using Blazor.Ninja.Common.Meta;
using Blazor.Ninja.Sdk.AspNetCore.Components;

using Blazor.Ninja.KickStart.Common;

namespace Blazor.Ninja.KickStart.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class GeoFeatureGroupController : ControllerBase
	{
		private readonly IProxyFactory _proxyFactory;

		public GeoFeatureGroupController(IProxyFactory proxyFactory)
		{
			if (proxyFactory == null) throw new ArgumentException(nameof(proxyFactory));

			_proxyFactory = proxyFactory;
		}

		[HttpGet]
		public async Task<Response<List<MapFeatureVm>>> Get(
			[FromQuery(Name = "polygon")] string json,
			CancellationToken cts = default)
		{
			try
			{
				var proxy = _proxyFactory.GetAppContextDataProxy<GeoFeatureGroup>();

				var polygon = JsonConvert.DeserializeObject<GeoPolygon>(json);

				var filter = Builders<GeoFeatureGroup>.Filter.GeoIntersects("QueryPolygon", polygon.Coordinates);

				var page = await proxy.GetPageAsync(filter, 0, int.MaxValue, cancellationToken: cts);

				var features = new List<MapFeatureVm>();
				foreach (var group in page.Items)
				{
					if (group.HasLand)
					{
						var vm = new MapFeatureVm
						{
							Id = group.LandFeature.Id,
							GroupId = group.Id,
							Type = FeatureTypes.Land,
							Polygon = group.LandFeature.Polygon
						};
						features.Add(vm);
					}

					foreach (var feature in group.Features)
					{
						var vm = new MapFeatureVm
						{
							Id = feature.Id,
							GroupId = group.Id,
							Type = FeatureTypes.Building,
							Polygon = feature.Polygon
						};
						features.Add(vm);
					}
				}

				return new Response<List<MapFeatureVm>>(features);
			}
			catch (Exception ex)
			{
				return new Response<List<MapFeatureVm>>
				{
					Outcome = Outcomes.Failure,
					Message = ex.Message
				};
			}
		}

		[HttpGet]
		[Route("/geofeaturegroup/{id}")]
		public async Task<Response<List<MapFeatureVm>>> GetById(
			string id,
			CancellationToken cts = default)
		{
			try
			{
				var proxy = _proxyFactory.GetAppContextDataProxy<GeoFeatureGroup>();

				var group = await proxy.GetAsync(id, cancellationToken: cts);
				if (group == null) throw ExceptionBuilder.GetInstance(DotNinjaStatusCode.NotFound, "item");

				var features = new List<MapFeatureVm>();

				if (group.HasLand)
				{
					var vm = new MapFeatureVm
					{
						Id = group.LandFeature.Id,
						GroupId = group.Id,
						Type = FeatureTypes.Land,
						Polygon = group.LandFeature.Polygon
					};
					features.Add(vm);
				}

				foreach (var feature in group.Features)
				{
					var vm = new MapFeatureVm
					{
						Id = feature.Id,
						GroupId = group.Id,
						Type = FeatureTypes.Building,
						Polygon = feature.Polygon
					};
					features.Add(vm);
				}


				return new Response<List<MapFeatureVm>>(features);
			}
			catch (Exception ex)
			{
				return new Response<List<MapFeatureVm>>
				{
					Outcome = Outcomes.Failure,
					Message = ex.Message
				};
			}
		}

		[HttpGet]
		[Route("/geofeaturegroup/azure")]
		public async Task<Response<List<MapFeatureVm>>> GetForAzure([FromQuery(Name = "polygon")] string json)
		{
			try
			{
				var proxy = _proxyFactory.GetAppContextDataProxy<GeoFeatureGroup>();

				var polygon = JsonConvert.DeserializeObject<GeoPolygon>(json);

				var filter = Builders<GeoFeatureGroup>.Filter.GeoIntersects("QueryPolygon", polygon.Coordinates);

				var page = await proxy.GetPageAsync(filter, 0, int.MaxValue);

				var features = new List<MapFeatureVm>();
				foreach (var group in page.Items)
				{
					if (group.HasLand)
					{
						var vm = new MapFeatureVm
						{
							Id = group.LandFeature.Id,
							GroupId = group.Id,
							Polygon = group.LandFeature.Polygon,
							Type = FeatureTypes.Land
						};
						features.Add(vm);
					}
				}

				return new Response<List<MapFeatureVm>>(features);
			}
			catch (Exception ex)
			{
				return new Response<List<MapFeatureVm>>
				{
					Outcome = Outcomes.Failure,
					Message = ex.Message
				};
			}
		}
	}
}
