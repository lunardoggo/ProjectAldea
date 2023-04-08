using UnityEngine;
using System.Text;
using System.IO;

namespace ProjectAldea
{
    public static class StreamingAssetHelper
    {
        public static string LoadTextFile(string relativePath)
        {
            string path = StreamingAssetHelper.GetAbsolutePath(relativePath);

            if (!File.Exists(path))
            {
                throw new IOException($"File \"{relativePath}\" does not exist");
            }
            return File.ReadAllText(path, Encoding.UTF8);
        }

        public static void SaveTextFile(string relativePath, string content)
        {
            string path = StreamingAssetHelper.GetAbsolutePath(relativePath);
            File.WriteAllText(path, content, Encoding.UTF8);
        }

        private static string GetAbsolutePath(string relativePath)
        {
            return Path.Combine(Application.streamingAssetsPath, relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        }
    }
}
