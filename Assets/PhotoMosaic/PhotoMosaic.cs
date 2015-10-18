//
// PhotoMosaic - Photo mosaic image effect
//
// Copyright (C) 2015 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class PhotoMosaic : MonoBehaviour
{
    #region Public Properties

    [SerializeField]
    float _blockSize = 32;

    public float blockSize
    {
        get { return _blockSize; }
        set { _blockSize = value; }
    }

    [SerializeField]
    Texture2D _albumTexture;

    [SerializeField]
    Texture2D _lutTexture;

    #endregion

    #region Private Properties

    [SerializeField] Shader _shader;

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

        var linear = QualitySettings.activeColorSpace == ColorSpace.Linear;
        var blockSize = Mathf.Max(_blockSize, 2.0f);

        if (linear)
            _material.EnableKeyword("COLORSPACE_LINEAR");
        else
            _material.DisableKeyword("COLORSPACE_LINEAR");

        _material.SetFloat("_BlockSize", blockSize);
        _material.SetTexture("_AlbumTex", _albumTexture);
        _material.SetTexture("_LutTex", _lutTexture);

        Graphics.Blit(source, destination, _material, 0);
    }

    #endregion
}
