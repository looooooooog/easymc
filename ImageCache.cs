using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;

namespace EasyMC
{
	internal class ImageCache
	{
		private static Dictionary<string, Bitmap> heads;

		private static Dictionary<string, Bitmap> renders;

		public static string HEAD_URL;

		public static string RENDER_URL;

		static ImageCache()
		{
			ImageCache.heads = new Dictionary<string, Bitmap>();
			ImageCache.renders = new Dictionary<string, Bitmap>();
			ImageCache.HEAD_URL = "";
			ImageCache.RENDER_URL = "";
		}

		public ImageCache()
		{
		}

		private static Bitmap byteToImage(byte[] blob)
		{
			MemoryStream memoryStream = new MemoryStream();
			byte[] numArray = blob;
			memoryStream.Write(numArray, 0, Convert.ToInt32((int)numArray.Length));
			Bitmap bitmap = new Bitmap(memoryStream, false);
			memoryStream.Dispose();
			return bitmap;
		}

		public static Bitmap getHead(string uuid)
		{
			Bitmap item;
			try
			{
				if (!ImageCache.heads.ContainsKey(uuid))
				{
					Bitmap bitmap = ImageCache.loadImage(string.Format(ImageCache.HEAD_URL, uuid));
					ImageCache.heads[uuid] = bitmap;
					item = ImageCache.heads[uuid];
				}
				else
				{
					item = ImageCache.heads[uuid];
				}
			}
			catch (Exception exception)
			{
				item = null;
			}
			return item;
		}

		public static Bitmap getRender(string uuid)
		{
			Bitmap item;
			try
			{
				if (!ImageCache.renders.ContainsKey(uuid))
				{
					Bitmap bitmap = ImageCache.loadImage(string.Format(ImageCache.RENDER_URL, uuid));
					ImageCache.renders[uuid] = bitmap;
					item = ImageCache.renders[uuid];
				}
				else
				{
					item = ImageCache.renders[uuid];
				}
			}
			catch (Exception exception)
			{
				item = null;
			}
			return item;
		}

		private static Bitmap loadImage(string url)
		{
			return ImageCache.byteToImage((new WebClient()).DownloadData(url));
		}
	}
}