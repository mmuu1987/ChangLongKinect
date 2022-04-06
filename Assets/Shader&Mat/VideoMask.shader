Shader "Unlit/VideoMask"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Mask ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100

        Pass
        {

             // 关闭深度写入
             ZWrite Off
            // 开启混合模式，并设置混合因子为SrcAlpha和OneMinusSrcAlpha
             Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
           

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
             
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _Mask;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
            
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 colMask = tex2D(_Mask, i.uv);
               
                float sum = colMask.r+colMask.g+colMask.b;
                col.a = lerp(0,1,sum/3);
                return col;
            }
            ENDCG
        }
    }
}
