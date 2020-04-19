Shader "Unlit/Wireframe"
{
    Properties
    {
        _lineColor("Line Color", Color) = (1,1,1,1)
        _surfaceColor("Surface Color", Color) = (1,1,1,1)
        _lineWidth("Line Width", Float) = 2
        _drawDiagonals("Draw Diagonals", Float) = 1
    }
        SubShader
    {
        Lighting Off
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
                float4 color : Color;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : POSITION;
                float4 color : Color;
            };

            fixed4 _lineColor;
            fixed4 _surfaceColor;
            fixed _lineWidth;
            fixed _drawDiagonals;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : Color
            {
                float2 d = fwidth(i.uv);
                float lineY1 = step(d.y * _lineWidth, i.uv.y);
                float lineY2 = step(d.y * _lineWidth, 1 - i.uv.y);
                float lineX1 = step(d.x * _lineWidth, i.uv.x);
                float lineX2 = step(d.x * _lineWidth, 1 - i.uv.x);

                float diagonal = 1;
                if (_drawDiagonals > 0) {
                    diagonal = step(fwidth(i.uv.x - i.uv.y) * _lineWidth, abs(i.uv.x - i.uv.y));
                }

                float4 color = lerp(_lineColor, _surfaceColor, lineY1 * lineY2 * lineX1 * lineX2 * diagonal);
                return color;
            }
            ENDCG
        }
    }
}
