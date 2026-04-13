#if UNITY_ANDROID
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEditor.Android;

namespace GooglePlayGames.Editor
{
	internal class PlayGamesInitProviderRemovingProcessor : IPostGenerateGradleAndroidProject
	{
		private static readonly XNamespace AndroidNs = "http://schemas.android.com/apk/res/android";
		private static readonly XNamespace ToolsNs = "http://schemas.android.com/tools";

		private const string ProviderName = "com.google.android.gms.games.provider.PlayGamesInitProvider";

		public int callbackOrder => 999;

		public void OnPostGenerateGradleAndroidProject(string path)
		{
			PatchManifest(path);
		}

		private void PatchManifest(string path)
		{
			var manifestPath = Path.Combine(path, "..", "unityLibrary", "src", "main", "AndroidManifest.xml");
			var doc = XDocument.Load(manifestPath);
			var manifest = doc.Root;

			if (manifest.Attribute(XNamespace.Xmlns + "tools") == null)
				manifest.Add(new XAttribute(XNamespace.Xmlns + "tools", ToolsNs));

			var application = manifest.Element("application");
			if (application == null)
			{
				application = new XElement("application");
				manifest.Add(application);
			}

			bool alreadyExists = application.Elements("provider")
				.Any(e => (string)e.Attribute(AndroidNs + "name") == ProviderName);

			if (!alreadyExists)
			{
				application.Add(new XElement("provider",
					new XAttribute(ToolsNs + "node", "remove"),
					new XAttribute(AndroidNs + "name", ProviderName)));
			}

			doc.Save(manifestPath);
		}
	}
}
#endif