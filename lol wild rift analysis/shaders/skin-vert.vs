#version 300 es

uniform 	vec3 _WorldSpaceCameraPos;
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_WorldToObject[4];
uniform 	vec4 unity_WorldTransformParams;
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
uniform 	vec4 hlslcc_mtx4x4u_UniqueShadowMatrix[4];
in highp vec4 in_POSITION0; // vertex pos
in mediump vec2 in_TEXCOORD0;// uv0 xy
in mediump vec2 in_TEXCOORD1;// uv1 xy
in mediump vec3 in_NORMAL0;//normal
in mediump vec4 in_TANGENT0;//tangent
out mediump vec4 vs_TEXCOORD0;
out mediump vec3 vs_TEXCOORD1;
out mediump vec4 vs_TEXCOORD2;//float4 tangentToWorldAndPackedData[3] : TEXCOORD2;     [3x3:tangentToWorld | 1x3:viewDirForParallax or worldPos]
out mediump vec4 vs_TEXCOORD3;
out mediump vec4 vs_TEXCOORD4;
out highp vec4 vs_TEXCOORD5;
out mediump vec4 vs_TEXCOORD6;
vec4 u_xlat0;
vec4 u_xlat1;
vec3 u_xlat2;
float u_xlat9;
void main()
{
    //obj position to world position
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];

    //MVP trans.
    u_xlat1 = u_xlat0.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
    gl_Position = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;

    //uv
    vs_TEXCOORD0.xy = in_TEXCOORD0.xy;
    vs_TEXCOORD0.zw = in_TEXCOORD1.xy;


    u_xlat0.xyz = in_POSITION0.yyy * hlslcc_mtx4x4unity_ObjectToWorld[1].xyz;
    u_xlat0.xyz = hlslcc_mtx4x4unity_ObjectToWorld[0].xyz * in_POSITION0.xxx + u_xlat0.xyz;
    u_xlat0.xyz = hlslcc_mtx4x4unity_ObjectToWorld[2].xyz * in_POSITION0.zzz + u_xlat0.xyz;
    u_xlat0.xyz = hlslcc_mtx4x4unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
    u_xlat1.xyz = (-u_xlat0.xyz) + _WorldSpaceCameraPos.xyz;

    // worldspace position
    vs_TEXCOORD1.xyz = u_xlat1.xyz;

    //calculate the tangent space normal 
    u_xlat1.xyz = in_TANGENT0.yyy * hlslcc_mtx4x4unity_ObjectToWorld[1].xyz;
    u_xlat1.xyz = hlslcc_mtx4x4unity_ObjectToWorld[0].xyz * in_TANGENT0.xxx + u_xlat1.xyz;
    u_xlat1.xyz = hlslcc_mtx4x4unity_ObjectToWorld[2].xyz * in_TANGENT0.zzz + u_xlat1.xyz;
    u_xlat9 = dot(u_xlat1.xyz, u_xlat1.xyz);
    u_xlat9 = inversesqrt(u_xlat9);
    u_xlat1.xyz = vec3(u_xlat9) * u_xlat1.xyz;
    u_xlat1.w = u_xlat0.x;

    //world pos tangent
    vs_TEXCOORD2 = u_xlat1;

    u_xlat1.xyz = in_NORMAL0.zxy * in_TANGENT0.yzx;
    u_xlat1.xyz = in_NORMAL0.yzx * in_TANGENT0.zxy + (-u_xlat1.xyz);
    u_xlat1.xyz = u_xlat1.xyz * in_TANGENT0.www;
    u_xlat2.xyz = u_xlat1.yyy * hlslcc_mtx4x4unity_ObjectToWorld[1].xyz;
    u_xlat1.xyw = hlslcc_mtx4x4unity_ObjectToWorld[0].xyz * u_xlat1.xxx + u_xlat2.xyz;
    u_xlat1.xyz = hlslcc_mtx4x4unity_ObjectToWorld[2].xyz * u_xlat1.zzz + u_xlat1.xyw;
    u_xlat9 = dot(u_xlat1.xyz, u_xlat1.xyz);
    u_xlat9 = inversesqrt(u_xlat9);
    u_xlat1.xyz = vec3(u_xlat9) * u_xlat1.xyz;
    u_xlat1.xyz = u_xlat1.xyz * unity_WorldTransformParams.www;
    u_xlat1.w = u_xlat0.y;

    //world pos bitangent
    vs_TEXCOORD3 = u_xlat1;

    u_xlat1.x = dot(in_NORMAL0.xyz, hlslcc_mtx4x4unity_WorldToObject[0].xyz);
    u_xlat1.y = dot(in_NORMAL0.xyz, hlslcc_mtx4x4unity_WorldToObject[1].xyz);
    u_xlat1.z = dot(in_NORMAL0.xyz, hlslcc_mtx4x4unity_WorldToObject[2].xyz);
    u_xlat9 = dot(u_xlat1.xyz, u_xlat1.xyz);
    u_xlat9 = inversesqrt(u_xlat9);
    u_xlat1.xyz = vec3(u_xlat9) * u_xlat1.xyz;
    u_xlat1.w = u_xlat0.z;

    //world pos normal
    vs_TEXCOORD4 = u_xlat1;
    //unuse
    vs_TEXCOORD5 = vec4(0.0, 0.0, 0.0, 0.0);
    u_xlat1 = u_xlat0.yyyy * hlslcc_mtx4x4u_UniqueShadowMatrix[1];
    u_xlat1 = hlslcc_mtx4x4u_UniqueShadowMatrix[0] * u_xlat0.xxxx + u_xlat1;
    u_xlat0 = hlslcc_mtx4x4u_UniqueShadowMatrix[2] * u_xlat0.zzzz + u_xlat1;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4u_UniqueShadowMatrix[3];
    //shadow coords
    vs_TEXCOORD6 = u_xlat0;
    return;
}