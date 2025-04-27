Shader "Custom/SpriteOutline"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineWidth ("Outline Width", float) = 0.01
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
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
            float4 _MainTex_ST;
            float4 _OutlineColor;
            float _OutlineWidth;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
{
    fixed4 col = tex2D(_MainTex, i.uv);
    fixed4 outline = _OutlineColor;

    // Sample more directions for a thicker outline
    float alpha = 0;
    alpha += tex2D(_MainTex, i.uv + float2(_OutlineWidth, 0)).a; // Right
    alpha += tex2D(_MainTex, i.uv + float2(-_OutlineWidth, 0)).a; // Left
    alpha += tex2D(_MainTex, i.uv + float2(0, _OutlineWidth)).a; // Up
    alpha += tex2D(_MainTex, i.uv + float2(0, -_OutlineWidth)).a; // Down
    alpha += tex2D(_MainTex, i.uv + float2(_OutlineWidth, _OutlineWidth)).a; // Up-Right
    alpha += tex2D(_MainTex, i.uv + float2(-_OutlineWidth, _OutlineWidth)).a; // Up-Left
    alpha += tex2D(_MainTex, i.uv + float2(_OutlineWidth, -_OutlineWidth)).a; // Down-Right
    alpha += tex2D(_MainTex, i.uv + float2(-_OutlineWidth, -_OutlineWidth)).a; // Down-Left

    // Draw outline if current pixel is transparent and any neighbor is opaque
    if (col.a == 0 && alpha > 0)
        return _OutlineColor;
    return col;
}
            ENDCG
        }
    }
}