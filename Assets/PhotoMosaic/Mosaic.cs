using UnityEngine;

namespace PhotoMosaic
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class Mosaic : MonoBehaviour
    {
        [SerializeField] float _blockSize = 32;

        #region Private Properties

        [SerializeField] Shader _shader;

        [SerializeField] Texture2D _albumTexture;
        [SerializeField] Texture2D _lutTexture;

        Material _material;

        #endregion

        #region MonoBehaviour Functions

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_material == null)
            {
                _material = new Material(_shader);
                _material.hideFlags = HideFlags.DontSave;
            }

            _material.SetFloat("_BlockSize", _blockSize);
            _material.SetTexture("_AlbumTex", _albumTexture);
            _material.SetTexture("_LutTex", _lutTexture);

            Graphics.Blit(source, destination, _material, 0);
        }

        #endregion
    }
}
