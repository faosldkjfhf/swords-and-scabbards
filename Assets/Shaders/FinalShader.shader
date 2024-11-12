Shader "Invert/FinalShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Noise", 2D) = "white" {}
        _Width ("Width", Int) = 1920
        _Height ("Height", Int) = 1080
        _VignetteRadius ("Vignette Radius", Range(-1, 1)) = 0.0
        _VignetteSoftness ("Vignette Softness", Range(0, 1)) = 0.5
        _VignetteDarkening("Vignette Darkening", Range(0, 1)) = 0.2
        _Tint("Tint", Color) = (1, 0, 0, 1)
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            int _Width;
            int _Height;
            float _VignetteRadius;
            float _VignetteSoftness;
            float _VignetteDarkening;
            float3 _Tint;

            fixed4 frag (v2f i) : SV_Target
            {
                float2 centeredUV = i.uv - 0.5;
                float2 dimensions = float2(_Width, _Height);
                float2 texCoords = centeredUV * dimensions;

                // Create a vignette mask
                float distFromCenter = distance(texCoords, 0.0);
                float minDimension = min(dimensions.x, dimensions.y);
                float smoothedDistFromCenter = smoothstep(_VignetteRadius, _VignetteRadius + _VignetteSoftness, distFromCenter / minDimension);
                float3 color = float3(smoothedDistFromCenter, smoothedDistFromCenter, smoothedDistFromCenter);
                float3 lerpedColor = lerp(1, _VignetteDarkening, color);

                // Get the texture color from the camera
                float3 texColor = tex2D(_MainTex, i.uv).rgb;

                // Interpolate between the tint and the texture based on the vignette mask
                return fixed4(lerp(_Tint, texColor, lerpedColor), 1);
            }
            ENDCG
        }
    }
}
