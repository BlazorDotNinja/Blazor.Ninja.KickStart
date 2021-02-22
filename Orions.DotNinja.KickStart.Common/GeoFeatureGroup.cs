using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Orions.DotNinja.Common.Data;
using Orions.DotNinja.Common.Meta;

namespace Orions.DotNinja.KickStart.Common
{
	public class GeoFeatureGroup : IdDataObject
	{
		public GeoFeature LandFeature { get; set; }

		public List<GeoFeature> Features { get; set; }

		public string Label { get; set; }

		public string Description { get; set; }

		public List<string> Emails { get; set; }

		public List<string> Phones { get; set; }

		public List<string> Urls { get; set; }

		public Address Address { get; set; }

		public List<TagInstanceGroup> TagInstanceGroups { get; set; }

		public GeoPolygon QueryPolygon
		{
			get
			{
				if (LandFeature != null) return LandFeature.Polygon;

				if (Features != null && Features.Count > 0) return Features[0].Polygon;

				return null;
			}
		}

		[JsonIgnore]
		public bool HasLabel => !string.IsNullOrWhiteSpace(Label);

		[JsonIgnore]
		public bool HasDescription => !string.IsNullOrWhiteSpace(Description);

		[JsonIgnore]
		public bool HasEmails => Emails != null && Emails.Any();

		[JsonIgnore]
		public bool HasPhones => Phones != null && Phones.Any();

		[JsonIgnore]
		public bool HasUrls => Urls != null && Urls.Any();

		[JsonIgnore]
		public bool HasLand => LandFeature != null;

		[JsonIgnore]
		public bool HasFeatures => Features != null && Features.Any();

		[JsonIgnore]
		public double[] Center
		{
			get
			{
				if (QueryPolygon == null) return default;

				var x1 = 180.0;
				var x2 = -180.0;
				var y1 = 90.0;
				var y2 = -90.0;

				foreach (var point in QueryPolygon.Coordinates[0])
				{
					var x = point[0];
					if (x < x1) x1 = x;
					if (x > x2) x2 = x;

					var y = point[1];
					if (y < y1) y1 = y;
					if (y > y2) y2 = y;
				}

				var centerX = x1 + ((x2 - x1) / 2);
				var centerY = y1 + ((y2 - y1) / 2);

				return new[] { centerX, centerY };
			}
		}

		public GeoFeatureGroup()
		{
			Features = new List<GeoFeature>();

			Emails = new List<string>();

			Phones = new List<string>();

			Urls = new List<string>();

			TagInstanceGroups = new List<TagInstanceGroup>();
		}

		public void AddLabel(
			string value)
		{
			if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException(nameof(value));

			if (HasLabel) Label += $" {value}";
			else Label = value;
		}

		public void AddDescription(
			string value)
		{
			if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException(nameof(value));

			if (HasDescription) Description += $" {value}";
			else Description = value;
		}

		public void AddEmails(
			List<string> values)
		{
			if (values == null) throw new ArgumentException(nameof(values));

			foreach (var value in values)
			{
				if (Emails.Exists(it => string.Equals(it, value, StringComparison.InvariantCultureIgnoreCase))) return;

				Emails.Add(value.ToLower().Trim());
			}
		}

		public void AddPhones(
			List<string> values)
		{
			if (values == null) throw new ArgumentException(nameof(values));

			foreach (var value in values)
			{
				if (Phones.Exists(it => string.Equals(it, value, StringComparison.InvariantCultureIgnoreCase))) return;

				Phones.Add(value.ToLower().Trim());
			}
		}

		public void AddUrls(
			List<string> values)
		{
			if (values == null) throw new ArgumentException(nameof(values));

			foreach (var value in values)
			{
				if (Urls.Exists(it => string.Equals(it, value, StringComparison.InvariantCultureIgnoreCase))) return;

				Urls.Add(value.ToLower().Trim());
			}
		}

		public void AddTagInstanceGroups(
			List<TagInstanceGroup> values)
		{
			if (values == null) throw new ArgumentException(nameof(values));

			foreach (var value in values)
			{
				var tagInstanceGroup = TagInstanceGroups.FirstOrDefault(it => it.GroupTagId == value.GroupTagId);
				if (tagInstanceGroup == null)
				{
					tagInstanceGroup = new TagInstanceGroup
					{
						GroupTagId = value.GroupTagId
					};
					TagInstanceGroups.Add(tagInstanceGroup);
				}

				foreach (var instance in value.Instances)
				{
					if (tagInstanceGroup.Instances.Exists(it => it.TagId == instance.TagId)) continue;

					tagInstanceGroup.Instances.Add(instance);
				}
			}
		}

		public GeoFeature GetFeature(string featureId)
		{
			if (string.IsNullOrWhiteSpace(featureId)) throw new ArgumentException(nameof(featureId));

			GeoFeature feature = null;

			if (LandFeature?.Id == featureId) feature = LandFeature;

			if (feature == null) feature = Features.FirstOrDefault(it => it.Id == featureId);

			if (feature == null) throw ExceptionBuilder.GetInstance(DotNinjaStatusCode.NotFound, "feature");

			return feature;
		}

		public void RemoveFeature(string featureId)
		{
			if (string.IsNullOrWhiteSpace(featureId)) throw new ArgumentException(nameof(featureId));

			bool found;

			if (LandFeature?.Id == featureId)
			{
				found = true;
				LandFeature = null;
			}
			else
			{
				var count = Features.RemoveAll(it => it.Id == featureId);
				found = count > 0;
			}

			if (!found) throw ExceptionBuilder.GetInstance(DotNinjaStatusCode.NotFound, "feature");
		}
	}
}
