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
Shader "Hidden/PhotoMosaic"
{
    Properties
    {
        _MainTex ("-", 2D) = "" {}
        _AlbumTex ("-", 2D) = "" {}
        _LutTex ("-", 2D) = "" {}
    }

    CGINCLUDE

    #pragma multi_compile COLORSPACE_SRGB COLORSPACE_LINEAR

    #include "UnityCG.cginc"

    sampler2D _MainTex;
    float4 _MainTex_TexelSize;

    float _BlockSize;

    sampler2D _AlbumTex;
    float4 _AlbumTex_TexelSize;

    sampler2D _LutTex;

    half4 frag(v2f_img i) : SV_Target
    {
        // block size in the source texture space
        float2 block_size = _MainTex_TexelSize.xy * _BlockSize;

        // sample the source pixel with the downsampled uv coordinate
        float2 uv_down = trunc(i.uv / block_size) * block_size;
        half3 src = tex2D(_MainTex, uv_down).rgb;

#if COLORSPACE_LINEAR
        src = LinearToGammaSpace(src);
#endif

        // 4-bit quantized blue component level
        float src_b16 = floor(src.b * 16) / 16;

        // select a photo with using the lut
        float2 lut_uv = float2(src.r / 16 + src_b16, src.g);
        float2 lut = tex2D(_LutTex, lut_uv).rg;

        // repeat uv with the block size (including a small margin)
        float2 uv_block = frac(i.uv / block_size) * 0.94 + 0.03;

        // get uv in the album texture
        float album_aspect = _AlbumTex_TexelSize.y * _AlbumTex_TexelSize.z;
        float2 uv_photo = uv_block * float2(1.0 / 16, album_aspect / 16);

        half4 co = tex2D(_AlbumTex, lut + uv_photo);
#if COLORSPACE_LINEAR
        co.rgb = GammaToLinearSpace(co.rgb);
#endif
        return co;
    }

    ENDCG

    SubShader
    {
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            ENDCG
        }
    }
}
