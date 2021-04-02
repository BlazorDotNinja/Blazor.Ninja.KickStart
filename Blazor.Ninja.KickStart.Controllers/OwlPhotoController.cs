using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Blazor.Ninja.Common;
using Blazor.Ninja.Common.Data;
using Blazor.Ninja.Common.Factories;

namespace Blazor.Ninja.KickStart.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class OwlPhotoController : ControllerBase
	{
		private readonly IProxyFactory _proxyFactory;

		public OwlPhotoController(IProxyFactory proxyFactory)
		{
			if (proxyFactory == null) throw new ArgumentException(nameof(proxyFactory));

			_proxyFactory = proxyFactory;
		}


		[HttpPost]
		[Route("/owlphoto/{id}")]
		public async Task<ActionResult<Response>> ProcessAsync(string id)
		{
			try
			{
				var proxy = _proxyFactory.GetContentProxy("OwlPhoto");

				var content = await proxy.GetAsync(id);
				var data = await proxy.DownloadDataAsync(id, 0, int.MaxValue);

				using var ms = new MemoryStream(data);
				var image = Image.FromStream(ms);

				// Set width and height
				content.MetaData.Add("Width", image.Width.ToString());
				content.MetaData.Add("Height", image.Height.ToString());
				await proxy.UpdateAsync(content);

				// Create preview
				var width = 200;
				var height = (image.Height * width) / image.Width;

				// crop: "attention"
				data = ResizeImage(image, width, height);
				content = new Content
				{
					Id = content.Id + "xw100",
					ContentType = "image/jpeg"
				};
				content.MetaData.Add("Width", width.ToString());
				content.MetaData.Add("Height", height.ToString());
				content = await proxy.CreateAsync(content);
				await proxy.AppendDataAsync(content.Id, data);

				return new Response { Outcome = Outcomes.Success };
			}
			catch (Exception ex)
			{
				return new Response
				{
					Outcome = Outcomes.Failure,
					Message = ex.Message
				};
			}
		}

		public static byte[] ResizeImage(Image image, int width, int height)
		{
			using var bitmap = new Bitmap(width, height);
			var rectangle = new Rectangle(0, 0, width, height);

			bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);

			using (var graphics = Graphics.FromImage(bitmap))
			{
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

				using var wrapMode = new ImageAttributes();
				wrapMode.SetWrapMode(WrapMode.TileFlipXY);
				graphics.DrawImage(image, rectangle, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
			}

			var encoderParameters = new EncoderParameters(1) { Param = { [0] = new EncoderParameter(Encoder.Quality, (long)100) } };

			using var stream = new MemoryStream();
			bitmap.Save(stream, GetEncoder(ImageFormat.Jpeg), encoderParameters);

			return stream.ToArray();
		}

		static ImageCodecInfo GetEncoder(ImageFormat format)
		{
			var codecs = ImageCodecInfo.GetImageDecoders();
			return codecs.Single(codec => codec.FormatID == format.Guid);
		}
	}
}
