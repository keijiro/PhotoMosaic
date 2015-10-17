using UnityEngine;
using UnityEditor;
using System.IO;

namespace PhotoMosaicEditor
{
    class AlbumBuilder
    {
        const int _albumWidth = 4096;
        const int _photoWidth = 256;
        const int _photosPerRow = _albumWidth / _photoWidth;
        const int _lutWidth = 16;

        static Texture2D[] LoadPhotos()
        {
            string[] folders = {"Assets/PhotoMosaic/Album"};
            var guids = AssetDatabase.FindAssets("t:Texture2D", folders);
            var textures = new Texture2D[guids.Length];
            for (var i = 0; i < guids.Length; i++) {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                textures[i] = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            }
            return textures;
        }

        static void ExportAndDestroy(Texture2D texture, string filename)
        {
            var path = Path.Combine("Assets/PhotoMosaic", filename);
            File.WriteAllBytes(path, texture.EncodeToPNG());
            Object.DestroyImmediate(texture);
        }

        static void Blit(Texture2D src, Texture2D dst, int ox, int oy)
        {
            dst.SetPixels32(ox, oy, src.width, src.height, src.GetPixels32());
        }

        static Vector3 GetAverageColor(Texture2D texture)
        {
            var sum = Vector4.zero;
            for (var y = 0; y < texture.height; y++)
                for (var x = 0; x < texture.width; x++)
                    sum += (Vector4)texture.GetPixel(x, y);
            return sum / (texture.width * texture.height);
        }

        static int SearchNearestColor(Vector3[] colors, Vector3 target)
        {
            var min_d = 1.0f;
            var min_i = -1;

            for (var i = 0; i < colors.Length; i++)
            {
                var d = (colors[i] - target).magnitude;
                if (d < min_d)
                {
                    min_d = d;
                    min_i = i;
                }
            }

            return min_i;
        }

        static Texture2D CreateLut(Vector3[] colors)
        {
            var texture = new Texture2D(_lutWidth * _lutWidth, _lutWidth);
            var rowCount = (colors.Length + _photosPerRow - 1) / _photosPerRow;

            for (var x = 0; x < _lutWidth; x++)
            {
                for (var y = 0; y < _lutWidth; y++)
                {
                    for (var z = 0; z < _lutWidth; z++)
                    {
                        var c = new Vector3(x, y, z) / _lutWidth;
                        var i = SearchNearestColor(colors, c);
                        var r = (float)(i % _photosPerRow) / _photosPerRow;
                        var g = (float)(i / _photosPerRow) / rowCount;
                        texture.SetPixel(x + z * _lutWidth, y, new Color(r, g, 0, 1));
                    }
                }
            }

            return texture;
        }

        [MenuItem("Assets/PhotoMosaic/Build Album")]
        static void BuildAlbum()
        {
            var photos = LoadPhotos();
            var rowCount = (photos.Length + _photosPerRow - 1) / _photosPerRow;

            var album = new Texture2D(_albumWidth, _photoWidth * rowCount);
            var photoColors = new Vector3[photos.Length];

            for (var i = 0; i < photos.Length; i++)
            {
                var column = i % _photosPerRow;
                var row = i / _photosPerRow;
                Blit(photos[i], album, column * _photoWidth, row * _photoWidth);
                photoColors[i] = GetAverageColor(photos[i]);
            }

            ExportAndDestroy(album, "AlbumTexture.png");
            ExportAndDestroy(CreateLut(photoColors), "AlbumLut.png");
        }
    }
}
