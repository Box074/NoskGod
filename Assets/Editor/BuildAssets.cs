using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class CreateAssetBundles
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAssetBundles()
    {
        string assetBundleDirectory = "HKMod/AssetBundles/Windows";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }

        BuildPipeline.BuildAssetBundles(assetBundleDirectory,
                                        BuildAssetBundleOptions.ChunkBasedCompression,
                                        BuildTarget.StandaloneWindows);

    }
    [MenuItem("Assets/Build All AssetBundles")]
    static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = "HKMod/AssetBundles";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }

        var platforms = new List<(BuildTarget, string)>()
        {
            (BuildTarget.StandaloneLinux64, "Linux"),
            (BuildTarget.StandaloneOSX, "OSX"),
            (BuildTarget.StandaloneWindows, "Windows")
        };
        foreach (var v in platforms)
        {
            var dir = Path.Combine(assetBundleDirectory, v.Item2);
            Directory.CreateDirectory(dir);
            BuildPipeline.BuildAssetBundles(dir,
                                        BuildAssetBundleOptions.ChunkBasedCompression,
                                        v.Item1);
        }
    }
}