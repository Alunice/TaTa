Shader "Ouke/ShadowProjectorMultiply"
{
	Properties{
		_ShadowTex("Cookie", 2D) = "gray"
	}

	SubShader {
		Tags{"RenderType" = "Transparent" "Queue" = "Transparent 2"}
		Pass {
			ZWrite Off
			ColorMask RGB 
			// Blend SrcAlpha OneMinusSrcAlpha
			Blend DstColor Zero
			// Offset -1,-1
			// Fog{ Color(1,1,1) } 

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc" 

			struct appdata_in
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 uv_Main : TEXCOORD0; 
			};

			sampler2D  _ShadowTex;
			float4x4 _GlobalProjector;

			v2f vert( appdata_in v){
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv_Main = mul(_GlobalProjector, v.vertex);

				return o;
			}

			half4 frag(v2f i): COLOR
			{
				half4 tex = half4(1,1,1,1);
				half4 shadow_tex = tex2D(_ShadowTex,i.uv_Main.xy);
				//v_x and v_y value are 0 or 1, 1 means the uv is out of bounds
				//so we just need get the larger number(v_x or v_y),and compare weather its larger than 1
				half v_x = step(1.0f,i.uv_Main.x);
				half v_y = step(1.0f,i.uv_Main.y);
				half value = step(v_x,v_y);
				value = lerp(v_y,v_x,value);

				half4 col = lerp(shadow_tex,tex,value);
				// col.a =0.5;
				// half4 col = shadow_tex;
				col.a = 1;
				return col;

			}
			ENDCG
		}
	}
}