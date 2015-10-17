Shader "Hidden/PhotoMosaic/Mosaic"
{
    Properties
    {
        _MainTex ("-", 2D) = "" {}
        _AlbumTex ("-", 2D) = "" {}
        _LutTex ("-", 2D) = "" {}
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _MainTex;
    float4 _MainTex_TexelSize;
    float _BlockSize;

    sampler2D _AlbumTex;
    sampler2D _LutTex;

    half4 frag(v2f_img i) : SV_Target
    {
        float2 block = _MainTex_TexelSize.xy * _BlockSize;
        float2 uv1 = trunc(i.uv / block) * block;
        float2 uv2 = frac(i.uv / block);

        half4 src = tex2D(_MainTex, uv1);

        float b = floor(src.b * 16) / 16;
        float2 lut_uv = float2(src.r / 16 + b, src.g);
        half2 lut = tex2D(_LutTex, lut_uv).rg;

        half4 co = tex2D(_AlbumTex, lut + uv2 * float2(1.0/16, 1.0/3));

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
