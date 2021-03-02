using System.Collections.Generic;
using Blazor.Ninja.Common.Data;

namespace Blazor.Ninja.KickStart.Common
{
	public class GeoFeature : IdDataObject
	{
		public GeoPolygon Polygon { get; set; }

		public List<TagInstanceGroup> TagInstanceGroups { get; set; }

		public GeoFeature()
		{
			TagInstanceGroups = new List<TagInstanceGroup>();
		}
	}
}
