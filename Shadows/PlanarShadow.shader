Shader "Ouke/PlanarShadow"
{
	Properties{
		_MainTex("Tex", 2D) = "black" {}
		_LightPos("Light Position", Vector) = (0.38,1.19,0,0)
		_ShadowColor("ShdowCol",Color) = (0.125,0.125,0.125,0.5)
		_ShadowPlaneNormal("ShadowNormal",Vector) = (0,1,0,0)
		_ModelOffset("Offset",Vector) = (0,-20,0,0)
	}

	SubShader {
		Tags{"RenderType" = "Opaque"}

		Pass {

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#include "UnityCG.cginc" 

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			struct appdata_in
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0; 
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv :TEXCOORD0; 
			};

			v2f vert( appdata_in v){
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv.xy * _MainTex_ST.xy +_MainTex_ST.zw;

				return o;
			}

			fixed4 frag (v2f i): SV_Target
			{
				return tex2D(_MainTex,i.uv);
			}
			ENDCG
		}

		Pass{

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#include "UnityCG.cginc" 

			fixed4 _LightPos;
			fixed4 _ShadowColor;
			fixed4 _ShadowPlaneNormal;
			fixed4 _ModelOffset;

			struct appdata_in
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				fixed4 color : COLOR;
			};

			v2f vert( appdata_in v){
				v2f o;
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				float3 lightDir = _LightPos.xyz;
				float3 normal = normalize(_ShadowPlaneNormal.xyz);

				// float4 origPoint = (0,0,0,1);
				// origPoint.xyz = origPoint.xyz - _ModelOffset.xyz;
				// float3 p0 = mul(unity_ObjectToWorld,origPoint).xyz;
				float3 p0 = _ModelOffset.xyz;

				fixed3 dirShadow = worldPos + dot(p0 - worldPos, normal) / dot(normalize(lightDir),normal) * normalize(lightDir);
				//fixed3 dirShadow = worldPos + dot(p0 - worldPos, normal) * lightDir;

				o.pos = UnityWorldToClipPos(dirShadow);
				o.color = _ShadowColor;

				return o;
			}

			fixed4 frag (v2f i): SV_Target
			{
				return i.color;
			}
			ENDCG
		}
	}
}