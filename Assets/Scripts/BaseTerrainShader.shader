Shader "Unlit/BaseTerrainShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _FresnelColor("Fresnel Color", Color) = (0,0,0,1)
        _FresnelPower("Fresnel Power", Float) = 1.0
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 100

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
                    float3 normal : NORMAL;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float3 worldNormal : TEXCOORD1;
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    float3 viewDir : TEXCOORD2;
                    UNITY_FOG_COORDS(2)
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float4 _FresnelColor;
                float _FresnelPower;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    o.worldNormal = UnityObjectToWorldNormal(v.normal);
                    o.viewDir = normalize(_WorldSpaceCameraPos - mul(unity_ObjectToWorld, v.vertex).xyz);
                    UNITY_TRANSFER_FOG(o,o.vertex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, i.uv);

                // Calculate Fresnel effect
                float fresnel = pow(abs(dot(normalize(i.worldNormal), float3(0, 1, 0))), _FresnelPower);

                // Blend Fresnel color with texture color
                col = lerp(col, _FresnelColor, fresnel);


                // Apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);

                return col;
                }

            
            ENDCG
        }
        }
}
