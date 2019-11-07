Shader "Ouke/SimpleShader"
{
	Properties{
		_MatColor("_MatColor", Color) = (0.2,0.2,0.2,0.5)
	}

	SubShader {
		Tags{"RenderType" = "Opaque"}

		Pass {

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#include "UnityCG.cginc" 
			half4 _MatColor;

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

				return o;
			}

			fixed4 frag (v2f i): SV_Target
			{
				return _MatColor;
			}
			ENDCG
		}	
	}
}