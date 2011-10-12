using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using System.Threading;
using System;
using System.Text;

namespace ValidatorWebRole
{
	public class WebRole : RoleEntryPoint
	{
		public override bool OnStart()
		{
			var CleanupThread = new Thread(new ThreadStart(Utils.CleanupThreadMethod));
			CleanupThread.Start();
			return base.OnStart();
		}
	}

	public class Utils
	{
		public static CloudStorageAccount acct = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("DataConnectionString"));
		public static CloudBlobClient blob_client = acct.CreateCloudBlobClient();

		public static string results_container_name = "results";
		public static string exceptions_container_name = "exceptions";

		public static CloudBlobContainer results_container = blob_client.GetContainerReference(results_container_name);
		public static CloudBlobContainer exceptions_container = blob_client.GetContainerReference(exceptions_container_name);

		public static string chunks_host = "http://icalvalid.blob.core.windows.net";

		public static Uri unparseable_message_uri = new Uri(chunks_host + "/chunks/unparseable.html");

		public static int cleanup_interval_minutes = 5;

		public static CloudBlob MakeResultsBlob(string ticks)
		{
			var blob_name = ticks + ".xml";
			var relative_uri = string.Format("{0}/{1}", Utils.results_container_name, blob_name);
			var blob = Utils.blob_client.GetBlobReference(relative_uri);
			blob.Properties.ContentType = "text/xml";
			return blob;
		}

		public static void StoreExceptionBlob(string text)
		{
			var blob_name = System.DateTime.UtcNow.Ticks + ".txt";
			var relative_uri = string.Format("{0}/{1}", Utils.exceptions_container_name, blob_name);
			var blob = Utils.blob_client.GetBlobReference(relative_uri);
			blob.Properties.ContentType = "text/plain";
			blob.UploadByteArray(Encoding.UTF8.GetBytes(text));
		}

		public static string MakeChunkUrl(string container, string blob)
		{
			return Utils.chunks_host + "/" + container + "/" + blob;
		}

		public static string MakePermalinkUrl(string id)
		{
			if (id.StartsWith("_"))
				return "";
			else
				return "/?id=" + id;
		}

		public static void CleanupThreadMethod()
		{
			while (true)
			{
				try
				{
					var blob_items = blob_client.ListBlobsWithPrefix(Utils.results_container_name + "/_");
					foreach (var blob_item in blob_items)
					{
						var blob = blob_client.GetBlobReference(blob_item.Uri.ToString());
						blob.FetchAttributes();
						var last_modified = blob.Attributes.Properties.LastModifiedUtc;
						var utc_now = System.DateTime.UtcNow;
						var age = utc_now - last_modified;
						if (age.Minutes > Utils.cleanup_interval_minutes)
							blob.Delete();
					}
					Thread.Sleep(TimeSpan.FromMinutes(Utils.cleanup_interval_minutes));
				}
				catch (Exception e)
				{
					StoreExceptionBlob(e.Message + e.StackTrace);
				}
			}
		}
	}

	public static class BlobExtensions
	{
		public static bool Exists(this CloudBlob blob)
		{
			try
			{
				blob.FetchAttributes();
				return true;
			}
			catch (StorageClientException e)
			{
				if (e.ErrorCode == StorageErrorCode.ResourceNotFound)
				{
					return false;
				}
				else
				{
					throw;
				}
			}
		}
	}
}

