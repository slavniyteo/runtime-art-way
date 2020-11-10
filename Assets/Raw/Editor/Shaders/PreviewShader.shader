Shader "ArtWindow/PreviewShader"
{
    Properties
    {
        //_MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 0, 0, 1)
    }
    SubShader
    {
    
        Tags {"Queue"="Transparent" "RenderType"="Opaque"}
		ZWrite On Lighting Off Cull Back  Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 _Color;
            sampler2D _MainTex;

            fixed4 frag (v2f i) : COLOR
            {
                fixed4 col = tex2D (_MainTex, i.uv);
                col = sign(col);
                return col * _Color;
            }
            ENDCG
        }
    }
}
