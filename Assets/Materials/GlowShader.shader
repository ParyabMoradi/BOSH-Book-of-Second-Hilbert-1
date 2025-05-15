Shader "Custom/GlowSurfaceShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _EmissionColor ("Emission Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float3 normal : NORMAL;
                float4 color : COLOR;
            };

            uniform float4 _EmissionColor;
            sampler2D _MainTex;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = v.normal;
                o.color = v.color;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.pos.xy);
                return col + _EmissionColor; // Add emission color
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}
