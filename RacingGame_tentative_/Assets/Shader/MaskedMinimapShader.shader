Shader "Unlit/MaskedMinimapShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MaskTex ("Mask Texture", 2D) = "white" {}
        _Color("Main Color", Color) = (1,1,1,0.5)
        _Width("Tex Width", Float) = 200.0
        _Height("Tex Height", Float) = 200.0
        _Thick("Line Thickness", Int) = 2
    }
    SubShader
    {
        // Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Tags{
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "False"
        }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog


            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uvA : TEXCOORD2;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _MaskTex;
            float4 _MaskTex_ST;

            fixed4 _Color;
            float _Width;
            float _Height;
            int _Thick;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uvA = TRANSFORM_TEX(v.uv, _MaskTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 mask = tex2D(_MaskTex, i.uvA);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                float rangeSq = _Thick*_Thick;

                // このピクセルの周囲の透明度の最大値を調べる
                float alphaMax = 0.0f;
                for (int x = -_Thick; x <= _Thick; ++x)
                    for (int y = -_Thick; y <= _Thick; ++y)
                    {
                        // float alpha = tex2D(_MainTex, i.uv + float2(x, y)).a;
                        float alpha = tex2D(_MainTex, i.uv + float2(x / _Width, y / _Height)).a;
                        if (alpha > 0.5 && x*x + y*y <= rangeSq)
                            alphaMax = 1;
                    }

                // このピクセルが透明なら、マスクの透明度をかけて塗る
                if (col.a < 0.5)
                    return float4(_Color.xyz, alphaMax * mask.a);
                // このピクセルが透明でないなら、透明で塗る
                else
                    return float4(0, 0, 0, 0);
            }
            ENDCG
        }
    }
}
