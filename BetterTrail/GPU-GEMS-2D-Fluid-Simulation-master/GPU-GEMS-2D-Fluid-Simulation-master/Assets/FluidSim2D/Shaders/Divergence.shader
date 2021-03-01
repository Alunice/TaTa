
Shader "FluidSim/Divergence" 
{
	SubShader 
	{
    	Pass 
    	{
			ZTest Always

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			
			uniform sampler2D _Velocity;
			uniform sampler2D _Obstacles;
			uniform float _HalfInverseCellSize;
			uniform float2 _InverseSize;

			struct v2f 
			{
    			float4  pos : SV_POSITION;
    			float2  uv : TEXCOORD0;
			};

			v2f vert(appdata_base v)
			{
    			v2f OUT;
    			OUT.pos = mul(UNITY_MATRIX_MVP, v.vertex);
    			OUT.uv = v.texcoord.xy;
    			return OUT;
			}
			
			float4 frag(v2f IN) : COLOR
			{

			    // Find neighboring velocities:
			    float2 vN = tex2D(_Velocity, IN.uv + float2(0, _InverseSize.y)).xy;
			    float2 vS = tex2D(_Velocity, IN.uv + float2(0, -_InverseSize.y)).xy;
			    float2 vE = tex2D(_Velocity, IN.uv + float2(_InverseSize.x, 0)).xy;
			    float2 vW = tex2D(_Velocity, IN.uv + float2(-_InverseSize.x, 0)).xy;
			
			    // Find neighboring obstacles:
			    float bN = tex2D(_Obstacles, IN.uv + float2(0, _InverseSize.y)).x;
			    float bS = tex2D(_Obstacles, IN.uv + float2(0, -_InverseSize.y)).x;
			    float bE = tex2D(_Obstacles, IN.uv + float2(_InverseSize.x, 0)).x;
			    float bW = tex2D(_Obstacles, IN.uv + float2(-_InverseSize.x, 0)).x;
			
			    // Set velocities to 0 for solid cells:
			    if(bN > 0.0) vN = 0.0;
			    if(bS > 0.0) vS = 0.0;
			    if(bE > 0.0) vE = 0.0;
			    if(bW > 0.0) vW = 0.0;
			
			    float result = _HalfInverseCellSize * (vE.x - vW.x + vN.y - vS.y);
			    
			    return float4(result,0,0,1);
			}
			
			ENDCG

    	}
	}
}