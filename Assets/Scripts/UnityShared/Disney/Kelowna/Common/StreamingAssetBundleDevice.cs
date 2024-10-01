using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class StreamingAssetBundleDevice : Device
	{
		public const string DEVICE_TYPE = "sa-bundle";

		private string streamingAssetsPath;

		public override string DeviceType
		{
			get
			{
				return "sa-bundle";
			}
		}

		public StreamingAssetBundleDevice(DeviceManager deviceManager)
			: base(deviceManager)
		{
			streamingAssetsPath = "file://" + Application.streamingAssetsPath;
		}

		public override AssetRequest<TAsset> LoadAsync<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry, AssetLoadedHandler<TAsset> handler = null)
		{
			StreamingAssetBundleWrapper streamingAssetBundleWrapper = new StreamingAssetBundleWrapper();
			AsyncStreamingAssetBundleRequest<TAsset> result = new AsyncStreamingAssetBundleRequest<TAsset>(entry.Key, streamingAssetBundleWrapper);
			CoroutineRunner.StartPersistent(loadBundleFromStreamingAssets(entry, streamingAssetBundleWrapper, handler), this, "loadBundleFromStreamingAssets");
			return result;
		}

		private IEnumerator loadBundleFromStreamingAssets<TAsset>(ContentManifest.AssetEntry entry, StreamingAssetBundleWrapper wrapper, AssetLoadedHandler<TAsset> handler) where TAsset : class
		{
			string key = entry.Key;
			wrapper.LoadFromDownload(Path.Combine(streamingAssetsPath, entry.Key + ".txt"));
			yield return wrapper.WebRequest;
			AssetBundle assetBundle = wrapper.AssetBundle;
			if (handler != null)
			{
				handler(key, (TAsset)(object)assetBundle);
			}
			yield return null;
		}

		public override TAsset LoadImmediate<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry)
		{
			throw new InvalidOperationException("Streaming asset bundles must be loaded asynchronously.");
		}
	}
}
