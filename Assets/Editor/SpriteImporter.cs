using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpriteImporter : AssetPostprocessor
{
    private void OnPreprocessTexture()
    {
        var importer = (TextureImporter)assetImporter;
        importer.mipmapEnabled = false;
        importer.spritePixelsPerUnit = 64;
    }
}
