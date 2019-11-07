Shader "Ouke/AOShadow"{
	SubShader {
		Tags{"RenderType" = "Opaque" "LightMode" = "ForwardBase"}
		Pass {

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#include "UnityCG.cginc" 
			#include "Lighting.cginc"

			half4 _ShadowColor;

			struct appdata_in
			{
				float4 vertex : POSITION;
				float4 color : Color;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 vertexColor : TEXCOORD0;
			};

			v2f vert( appdata_in v){
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);

				float4 coeffs = v.color;
				coeffs = (coeffs - 0.5) * 10;

				float3 objDir = ObjSpaceLightDir(v.vertex);

				objDir = normalize(objDir);

				float PI = 3.141592653;
				float F0 = 0.5 * sqrt( 1 / PI );
				float FC = 0.5 * sqrt( 3 / PI );
				float finalResult =  coeffs.r * F0 + coeffs.g *FC * objDir.x + coeffs.b *FC * objDir.y + coeffs.a *FC * objDir.z;
				finalResult = clamp(finalResult,0,1);

				o.vertexColor = float4(finalResult,finalResult,finalResult,1);

				return o;
			}

			fixed4 frag (v2f i): SV_Target
			{
				half4 col = i.vertexColor;
				UNITY_OPAQUE_ALPHA(col.a);

				return col;
			}
			ENDCG
		}
	}
}