using UnityEngine;
using UnityEditor;
using System.IO;

namespace PhotoMosaicEditor
{
    class AlbumAssetPreprocessor : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            TextureImporter importer = (TextureImporter)assetImporter;

            if (CheckIfPhoto())
            {
                importer.textureType = TextureImporterType.GUI;
                importer.textureFormat = TextureImporterFormat.AutomaticTruecolor;
                importer.maxTextureSize = 256;
                importer.isReadable = true;
            }
            else if (CheckIfLut())
            {
                importer.textureType = TextureImporterType.GUI;
                importer.textureFormat = TextureImporterFormat.AutomaticTruecolor;
                importer.maxTextureSize = 4096;
                importer.filterMode = FilterMode.Point;
            }
        }

        bool CheckIfPhoto()
        {
            var albumPath = Path.Combine("PhotoMosaic", "Album");
            var assetDir = Path.GetDirectoryName(assetPath);
            return assetDir.EndsWith(albumPath);
        }

        bool CheckIfLut()
        {
            return assetPath.EndsWith("AlbumLut.png");
        }
    }
}
