Shader "Custom/GravityPadArrow"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1) // Unused
        _PadHalfSize ("Pad Half Size", Vector) = (0.5, 0.5, 0.5, 0) // x,y,z half extents
    }
    SubShader
    {
        Tags { "Queue" = "Geometry" }
        Pass
        {
            Stencil
            {
                Ref 1
                Comp Equal
                Pass Keep
            }
            ZTest LEqual
            ZWrite On
            ColorMask RGBA

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile _ UNITY_SINGLE_PASS_STEREO
            #include "UnityCG.cginc"

            fixed4 _Color;
            float3 _PadHalfSize;

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4x4, _GravityPadWorldToLocal)
            UNITY_INSTANCING_BUFFER_END(Props)

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed3 HueToRGB(float h)
            {
                float3 rgb = saturate(abs(frac(h + float3(0, 2/3.0, 1/3.0)) * 6.0 - 3.0) - 1.0);
                return rgb;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 localPos = mul(UNITY_ACCESS_INSTANCED_PROP(Props, _GravityPadWorldToLocal), float4(i.worldPos, 1.0)).xyz;

                // Scale to account for real pad size
                float normalizedY = saturate((localPos.y * _PadHalfSize.y) + (_PadHalfSize.y * 0.5));

                // Use normalizedY as hue
                fixed3 rainbowColor = HueToRGB(normalizedY);

                // Lambert lighting
                float3 lightDir = normalize(float3(0, 1, 0)); 
                float NdotL = saturate(dot(normalize(i.worldNormal), lightDir));

                return fixed4(rainbowColor * (0.4 + 0.6 * NdotL), 1.0);
            }
            ENDCG
        }
    }
}
