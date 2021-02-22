using System.Collections.Generic;
using Orions.DotNinja.Common.Data;

namespace Orions.DotNinja.KickStart.Common
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
