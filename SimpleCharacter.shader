Shader "Ouke/SimpleCharacter"
{
	Properties{
		_MainTex("Tex", 2D) = "white" {}
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
				//o.uv = TRANSFORM_TEX(v.uv,_MainTex)
				o.uv = v.uv.xy * _MainTex_ST.xy +_MainTex_ST.zw;

				return o;
			}

			fixed4 frag (v2f i): SV_Target
			{
				return tex2D(_MainTex,i.uv);
			}
			ENDCG
		}	
	}
}