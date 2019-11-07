Shader "Ouke/Shadow"
{
	Properties{
		_ShadowColor("_ShadowColor", Color) = (0.5,0.5,0.5,0.4)
	}

	SubShader {
		Tags{"RenderType" = "Opaque"}
		Pass {

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#include "UnityCG.cginc" 

			half4 _ShadowColor;

			struct appdata_in
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
			};

			v2f vert( appdata_in v){
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);

				return o;
			}

			fixed4 frag (v2f i): SV_Target
			{
				half4 col = _ShadowColor;

				return col;
			}
			ENDCG
		}
	}
}