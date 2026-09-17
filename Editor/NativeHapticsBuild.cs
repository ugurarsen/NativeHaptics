#if UNITY_ANDROID
using System.IO;
using System.Text;
using System.Xml;
using UnityEditor.Android;
using UnityEngine;

namespace NativeHaptics.Editor
{
    public class NativeHapticsBuild : IPostGenerateGradleAndroidProject
    {
        private const string VIBRATE_PERMISSION = "android.permission.VIBRATE";
        private const string ANDROID_NS = "http://schemas.android.com/apk/res/android";

        public int callbackOrder { get { return 0; } }

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            string manifestPath = ResolveManifestPath(path);
            if (manifestPath == null)
            {
                Debug.LogWarning("[NativeHaptics] AndroidManifest.xml bulunamad\u0131. VIBRATE izni otomatik eklenemedi! " +
                                 "Cihazda haptik \u00e7a\u011fr\u0131lar\u0131 SecurityException ile \u00e7\u00f6kmesin diye " +
                                 "izin man\u00fcel olarak eklenmelidir.");
                return;
            }

            var document = new XmlDocument();
            document.Load(manifestPath);

            XmlNode manifest = document.SelectSingleNode("/*[local-name()='manifest']");
            if (manifest == null)
            {
                Debug.LogWarning("[NativeHaptics] AndroidManifest.xml i\u00e7inde <manifest> k\u00f6k d\u00fc\u011f\u00fcm\u00fc bulunamad\u0131. " +
                                 "VIBRATE izni eklenemedi.");
                return;
            }

            foreach (XmlNode node in manifest.SelectNodes("*[local-name()='uses-permission']"))
            {
                string name = XmlNameAttribute(node);
                if (name == VIBRATE_PERMISSION) return;
            }

            XmlElement permission = document.CreateElement("uses-permission");
            permission.SetAttribute("name", ANDROID_NS, VIBRATE_PERMISSION);
            manifest.AppendChild(permission);

            var settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = new UTF8Encoding(false)
            };
            using (var writer = XmlWriter.Create(manifestPath, settings))
            {
                document.Save(writer);
            }
        }

        private static string ResolveManifestPath(string path)
        {
            string[] candidates =
            {
                Path.Combine(path, "src" + Path.DirectorySeparatorChar + "main" + Path.DirectorySeparatorChar + "AndroidManifest.xml"),
                Path.Combine(path, "unityLibrary" + Path.DirectorySeparatorChar + "src" + Path.DirectorySeparatorChar + "main" + Path.DirectorySeparatorChar + "AndroidManifest.xml"),
                Path.Combine(path, "AndroidManifest.xml")
            };

            foreach (string candidate in candidates)
            {
                if (File.Exists(candidate)) return candidate;
            }
            return null;
        }

        private static string XmlNameAttribute(XmlNode node)
        {
            string value = node.Attributes?["name", ANDROID_NS]?.Value;
            if (!string.IsNullOrEmpty(value)) return value;
            return node.Attributes?["name"]?.Value;
        }
    }
}
#endif