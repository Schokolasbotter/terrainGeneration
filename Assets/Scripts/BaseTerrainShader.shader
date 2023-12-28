Shader "Unlit/BaseTerrainShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GrassColor("Grass Color", Color) = (0.0,0.5,0.0,1.0)
        _RockColor("Rock Color", Color) = (0.4,0.5,0.45,1.0)
        _Threshold("Normal Threshold", Range(0,1)) = 0.5
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
                    UNITY_FOG_COORDS(2)
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float4 _GrassColor;
                float4 _RockColor;
                float _Threshold;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    o.worldNormal = UnityObjectToWorldNormal(v.normal);
                    UNITY_TRANSFER_FOG(o,o.vertex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    // Determine if the normal is close to Vector3.up
                    float dotUp = dot(normalize(i.worldNormal), float3(0, 1, 0));
                    bool isGrassy = dotUp > _Threshold;

                    fixed4 col;
                    if (isGrassy)
                    {
                        col = _GrassColor; // Use grassy color
                    }
                    else
                    {
                        col = _RockColor; // Use rocky color
                    }

                    // apply fog
                    UNITY_APPLY_FOG(i.fogCoord, col);
                    return col;
                }
                ENDCG
            }
        }
}