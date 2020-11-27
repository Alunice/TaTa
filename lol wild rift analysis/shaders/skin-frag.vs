#version 300 es
#ifdef GL_EXT_shader_texture_lod
#extension GL_EXT_shader_texture_lod : enable
#endif

precision highp float;
precision highp int;
uniform     vec3 _WorldSpaceCameraPos;
uniform     mediump vec4 _WorldSpaceLightPos0;
uniform     vec4 hlslcc_mtx4x4unity_MatrixV[4];
uniform     mediump vec4 _LightColor0;
uniform     mediump float _OcclusionStrength;
uniform     mediump float _BrightnessInOcclusion;
uniform     mediump float _SubSurface;
uniform     vec3 u_ShadowFadeCenter;
uniform     vec2 u_LightShadowData;
uniform     mediump float _ShadowStrength;
uniform     mediump vec3 _RakingLightColor;
uniform     mediump vec4 _AmbientCol;
uniform     mediump vec4 _ReflectionColor;
uniform     mediump float _BrightnessInShadow;
uniform     mediump float _RakingLightSoftness;
uniform     mediump vec3 _DirLight;
uniform     mediump float _BumpScale;
uniform     mediump vec4 _MainTex_ST;
uniform     mediump vec4 _Color;
uniform     mediump float _GlossMapScale;
uniform mediump sampler2D _BumpMap;
uniform mediump sampler2D _MainTex;
uniform mediump sampler2D _MetallicGlossMap;
uniform mediump sampler2D _OcclusionMap;
uniform mediump sampler2D _ReflectionMatCap;
uniform mediump sampler2D _SssLut;
uniform mediump sampler2DShadow hlslcc_zcmpu_UniqueShadowTexture;
uniform mediump sampler2D u_UniqueShadowTexture;
in mediump vec4 vs_TEXCOORD0;
in mediump vec3 vs_TEXCOORD1;
in mediump vec4 vs_TEXCOORD2;
in mediump vec4 vs_TEXCOORD3;
in mediump vec4 vs_TEXCOORD4;
in mediump vec4 vs_TEXCOORD6;
layout(location = 0) out mediump vec4 SV_Target0;
mediump vec3 u_xlat16_0;
mediump vec4 u_xlat16_1;
mediump vec4 u_xlat16_2;
mediump vec3 u_xlat16_3;
vec3 u_xlat4;
mediump vec3 u_xlat16_4;
bvec3 u_xlatb4;
mediump vec3 u_xlat16_5;
vec4 u_xlat6;
mediump vec4 u_xlat16_6;
bvec4 u_xlatb6;
mediump vec3 u_xlat16_7;
mediump vec3 u_xlat16_8;
mediump vec4 u_xlat16_9;
mediump vec3 u_xlat16_10;
vec3 u_xlat11;
mediump vec3 u_xlat16_11;
mediump vec3 u_xlat16_12;
mediump vec3 u_xlat16_13;
mediump vec3 u_xlat16_14;
mediump vec3 u_xlat16_15;
mediump float u_xlat16_18;
vec3 u_xlat19;
mediump vec3 u_xlat16_19;
mediump float u_xlat16_30;
float u_xlat34;
mediump float u_xlat16_36;
mediump vec2 u_xlat16_39;
mediump float u_xlat16_45;
mediump float u_xlat16_47;
mediump float u_xlat16_48;
mediump float u_xlat16_50;
mediump float u_xlat16_52;
mediump float u_xlat16_55;
void main()
{
    u_xlat16_0.x = dot(vs_TEXCOORD1.xyz, vs_TEXCOORD1.xyz);
    u_xlat16_0.x = inversesqrt(u_xlat16_0.x);
    u_xlat16_0.xyz = u_xlat16_0.xxx * vs_TEXCOORD1.xyz;
    u_xlat16_1.xyz = texture(_BumpMap, vs_TEXCOORD0.xy).xyz;
    u_xlat16_2.xyz = u_xlat16_1.xyz * vec3(2.0, 2.0, 2.0) + vec3(-1.0, -1.0, -1.0);
    u_xlat16_2.xy = u_xlat16_2.xy * vec2(vec2(_BumpScale, _BumpScale));
    u_xlat16_3.xyz = u_xlat16_2.yyy * vs_TEXCOORD3.xyz;
    u_xlat16_2.xyw = vs_TEXCOORD2.xyz * u_xlat16_2.xxx + u_xlat16_3.xyz;
    u_xlat16_2.xyz = vs_TEXCOORD4.xyz * u_xlat16_2.zzz + u_xlat16_2.xyw;
    u_xlat16_45 = dot(u_xlat16_2.xyz, u_xlat16_2.xyz);
    u_xlat16_45 = inversesqrt(u_xlat16_45);
    u_xlat16_2.xyz = vec3(u_xlat16_45) * u_xlat16_2.xyz;
    u_xlat16_3.xy = vs_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
    u_xlat16_1 = texture(_MainTex, u_xlat16_3.xy);
    u_xlat16_1 = u_xlat16_1 * _Color;
    u_xlat16_4.xyz = u_xlat16_1.xyz * vec3(0.305306017, 0.305306017, 0.305306017) + vec3(0.682171106, 0.682171106, 0.682171106);
    u_xlat16_4.xyz = u_xlat16_1.xyz * u_xlat16_4.xyz + vec3(0.0125228781, 0.0125228781, 0.0125228781);
    u_xlat16_5.xyz = u_xlat16_1.xyz * u_xlat16_4.xyz;
    u_xlat16_6.xyz = texture(_MetallicGlossMap, vs_TEXCOORD0.xy).xyz;
    u_xlat16_3.z = u_xlat16_6.z * _SubSurface;
    u_xlat16_45 = (-u_xlat16_6.y) * _GlossMapScale + 1.0;
    u_xlat16_47 = u_xlat16_45 * u_xlat16_45;
    u_xlat16_47 = max(u_xlat16_47, 0.00100000005);
    u_xlat16_18 = (-u_xlat16_6.x) + 1.0;
    u_xlat16_18 = u_xlat16_18 * 0.959999979;
    u_xlat16_7.xyz = vec3(u_xlat16_18) * u_xlat16_5.xyz;
    u_xlat16_8.xyz = u_xlat16_1.xyz * u_xlat16_4.xyz + vec3(-0.0399999991, -0.0399999991, -0.0399999991);
    u_xlat16_8.xyz = u_xlat16_6.xxx * u_xlat16_8.xyz + vec3(0.0399999991, 0.0399999991, 0.0399999991);
    u_xlat16_1.xy = texture(_OcclusionMap, vs_TEXCOORD0.xy).xy;
    u_xlat16_9.xy = (-vec2(_OcclusionStrength, _BrightnessInOcclusion)) + vec2(1.0, 1.0);
    u_xlat16_9.xy = u_xlat16_1.xy * vec2(_OcclusionStrength, _BrightnessInOcclusion) + u_xlat16_9.xy;
    vec3 txVec0 = vec3(vs_TEXCOORD6.xy,vs_TEXCOORD6.z);
    u_xlat16_48 = textureLod(hlslcc_zcmpu_UniqueShadowTexture, txVec0, 0.0);
    u_xlatb4.xyz = greaterThanEqual(vs_TEXCOORD6.xyzx, vec4(0.0, 0.0, 0.0, 0.0)).xyz;
    u_xlat4.xyz = mix(vec3(0.0, 0.0, 0.0), vec3(1.0, 1.0, 1.0), vec3(u_xlatb4.xyz));
    u_xlatb6.xzw = greaterThanEqual(vec4(1.0, 0.0, 1.0, 1.0), vs_TEXCOORD6.xxyz).xzw;
    u_xlat6.xzw = mix(vec3(0.0, 0.0, 0.0), vec3(1.0, 1.0, 1.0), vec3(u_xlatb6.xzw));
    u_xlat4.xyz = u_xlat4.xyz * u_xlat6.xzw;
    u_xlat16_52 = u_xlat4.y * u_xlat4.x;
    u_xlat16_52 = u_xlat4.z * u_xlat16_52;
    u_xlat16_52 = u_xlat16_52 * _ShadowStrength;
    u_xlat16_48 = u_xlat16_48 + -1.0;
    u_xlat16_48 = u_xlat16_48 * u_xlat16_52;
    u_xlat16_10.x = vs_TEXCOORD2.w;
    u_xlat16_10.y = vs_TEXCOORD3.w;
    u_xlat16_10.z = vs_TEXCOORD4.w;
    u_xlat4.xyz = u_xlat16_10.xyz + (-u_ShadowFadeCenter.xyz);
    u_xlat4.x = dot(u_xlat4.xyz, u_xlat4.xyz);
    u_xlat4.x = sqrt(u_xlat4.x);
    u_xlat4.x = u_xlat4.x * u_LightShadowData.x;
    u_xlat4.x = u_xlat4.x * u_xlat4.x;
    u_xlat19.x = u_xlat4.x * u_xlat4.x;
    u_xlat4.x = u_xlat19.x * u_xlat4.x;
    u_xlat4.x = min(u_xlat4.x, 1.0);
    u_xlat19.xyz = (-u_xlat16_10.xyz) + _WorldSpaceCameraPos.xyz;
    u_xlat11.x = hlslcc_mtx4x4unity_MatrixV[0].z;
    u_xlat11.y = hlslcc_mtx4x4unity_MatrixV[1].z;
    u_xlat11.z = hlslcc_mtx4x4unity_MatrixV[2].z;
    u_xlat19.x = dot(u_xlat19.xyz, u_xlat11.xyz);
    u_xlat34 = float(1.0) / u_LightShadowData.y;
    u_xlat19.x = u_xlat34 * u_xlat19.x;
#ifdef UNITY_ADRENO_ES3
    u_xlat19.x = min(max(u_xlat19.x, 0.0), 1.0);
#else
    u_xlat19.x = clamp(u_xlat19.x, 0.0, 1.0);
#endif
    u_xlat34 = u_xlat19.x * -2.0 + 3.0;
    u_xlat19.x = u_xlat19.x * u_xlat19.x;
    u_xlat19.x = u_xlat19.x * u_xlat34;
    u_xlat19.x = u_xlat19.x * u_xlat16_48 + 1.0;
    u_xlat16_48 = (-u_xlat19.x) + 1.0;
    u_xlat16_48 = u_xlat4.x * u_xlat16_48 + u_xlat19.x;
    u_xlat16_4.x = dot(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz);
    u_xlat16_4.x = inversesqrt(u_xlat16_4.x);
    u_xlat16_19.xyz = u_xlat16_4.xxx * _WorldSpaceLightPos0.xyz;
    u_xlat16_39.xy = (-vec2(u_xlat16_45)) * vec2(0.699999988, 0.0799999982) + vec2(1.70000005, 0.600000024);
    u_xlat16_52 = u_xlat16_45 * u_xlat16_39.x;
    u_xlat16_52 = u_xlat16_52 * 6.0;
    u_xlat16_10.xyz = u_xlat16_9.xxx * _AmbientCol.xyz;
    u_xlat6.xzw = u_xlat16_2.yyy * hlslcc_mtx4x4unity_MatrixV[1].xyz;
    u_xlat6.xzw = hlslcc_mtx4x4unity_MatrixV[0].xyz * u_xlat16_2.xxx + u_xlat6.xzw;
    u_xlat6.xzw = hlslcc_mtx4x4unity_MatrixV[2].xyz * u_xlat16_2.zzz + u_xlat6.xzw;
    u_xlat11.xyz = vs_TEXCOORD3.www * hlslcc_mtx4x4unity_MatrixV[1].xyz;
    u_xlat11.xyz = hlslcc_mtx4x4unity_MatrixV[0].xyz * vs_TEXCOORD2.www + u_xlat11.xyz;
    u_xlat11.xyz = hlslcc_mtx4x4unity_MatrixV[2].xyz * vs_TEXCOORD4.www + u_xlat11.xyz;
    u_xlat11.xyz = u_xlat11.xyz + hlslcc_mtx4x4unity_MatrixV[3].xyz;
    u_xlat16_39.x = dot(u_xlat11.xyz, u_xlat6.xzw);
    u_xlat16_39.x = u_xlat16_39.x + u_xlat16_39.x;
    u_xlat16_12.xyz = u_xlat6.xzw * (-u_xlat16_39.xxx) + u_xlat11.xyz;
    u_xlat16_39.x = dot(u_xlat16_12.xyz, u_xlat16_12.xyz);
    u_xlat16_39.x = inversesqrt(u_xlat16_39.x);
    u_xlat16_12.xy = u_xlat16_39.xx * u_xlat16_12.xy;
    u_xlat16_55 = dot(u_xlat16_12.xy, u_xlat16_12.xy);
    u_xlat16_39.x = u_xlat16_12.z * u_xlat16_39.x + 1.0;
    u_xlat16_39.x = u_xlat16_39.x * u_xlat16_39.x + u_xlat16_55;
    u_xlat16_39.x = sqrt(u_xlat16_39.x);
    u_xlat16_39.x = u_xlat16_39.x + u_xlat16_39.x;
    u_xlat16_12.xy = u_xlat16_12.xy / u_xlat16_39.xx;
    u_xlat16_12.xy = u_xlat16_12.xy + vec2(0.5, 0.5);
    u_xlat16_6.xzw = textureLod(_ReflectionMatCap, u_xlat16_12.xy, u_xlat16_52).xyz;
    u_xlat16_6.xzw = u_xlat16_6.xzw * _ReflectionColor.xyz;
    u_xlat16_11.xyz = u_xlat16_6.xzw * vec3(0.305306017, 0.305306017, 0.305306017) + vec3(0.682171106, 0.682171106, 0.682171106);
    u_xlat16_11.xyz = u_xlat16_6.xzw * u_xlat16_11.xyz + vec3(0.0125228781, 0.0125228781, 0.0125228781);
    u_xlat16_6.xzw = u_xlat16_6.xzw * u_xlat16_11.xyz;
    u_xlat16_12.xyz = u_xlat16_9.xxx * u_xlat16_6.xzw;
    u_xlat16_13.xyz = _WorldSpaceLightPos0.xyz * u_xlat16_4.xxx + u_xlat16_0.xyz;
    u_xlat16_52 = dot(u_xlat16_13.xyz, u_xlat16_13.xyz);
    u_xlat16_52 = inversesqrt(u_xlat16_52);
    u_xlat16_13.xyz = vec3(u_xlat16_52) * u_xlat16_13.xyz;
    u_xlat16_0.x = dot(u_xlat16_2.xyz, u_xlat16_0.xyz);
    u_xlat16_15.x = u_xlat16_0.x;
#ifdef UNITY_ADRENO_ES3
    u_xlat16_15.x = min(max(u_xlat16_15.x, 0.0), 1.0);
#else
    u_xlat16_15.x = clamp(u_xlat16_15.x, 0.0, 1.0);
#endif
    u_xlat16_30 = dot(u_xlat16_2.xyz, u_xlat16_19.xyz);
    u_xlat16_52 = dot(u_xlat16_2.xyz, u_xlat16_13.xyz);
#ifdef UNITY_ADRENO_ES3
    u_xlat16_52 = min(max(u_xlat16_52, 0.0), 1.0);
#else
    u_xlat16_52 = clamp(u_xlat16_52, 0.0, 1.0);
#endif
    u_xlat16_9.x = dot(u_xlat16_19.xyz, u_xlat16_13.xyz);
#ifdef UNITY_ADRENO_ES3
    u_xlat16_9.x = min(max(u_xlat16_9.x, 0.0), 1.0);
#else
    u_xlat16_9.x = clamp(u_xlat16_9.x, 0.0, 1.0);
#endif
#ifdef UNITY_ADRENO_ES3
    u_xlatb4.x = !!(0.0199999996<u_xlat16_3.z);
#else
    u_xlatb4.x = 0.0199999996<u_xlat16_3.z;
#endif
    if(u_xlatb4.x){
        u_xlat16_39.x = u_xlat16_48 * 0.5 + 0.5;
        u_xlat16_55 = u_xlat16_30 * 0.5 + 0.5;
        u_xlat16_3.x = u_xlat16_39.x * u_xlat16_55;
        u_xlat16_4.xyz = texture(_SssLut, u_xlat16_3.xz).xyz;
        u_xlat16_4.xyz = vec3(u_xlat16_48) * u_xlat16_4.xyz;
        u_xlat16_4.xyz = u_xlat16_4.xyz;
    } else {
        u_xlat16_30 = u_xlat16_30;
#ifdef UNITY_ADRENO_ES3
        u_xlat16_30 = min(max(u_xlat16_30, 0.0), 1.0);
#else
        u_xlat16_30 = clamp(u_xlat16_30, 0.0, 1.0);
#endif
        u_xlat16_4.xyz = vec3(u_xlat16_48) * vec3(u_xlat16_30);
    //ENDIF
    }
    u_xlat16_13.xyz = u_xlat16_4.xyz * _LightColor0.xyz;
    u_xlat16_30 = u_xlat16_47 * u_xlat16_47;
    u_xlat16_3.x = u_xlat16_52 * u_xlat16_52;
    u_xlat16_50 = u_xlat16_47 * u_xlat16_47 + -1.0;
    u_xlat16_50 = u_xlat16_3.x * u_xlat16_50 + 1.00001001;
    u_xlat16_3.x = u_xlat16_9.x * u_xlat16_9.x;
    u_xlat16_6.x = max(u_xlat16_3.x, 0.100000001);
    u_xlat16_36 = u_xlat16_47 + 0.5;
    u_xlat16_6.x = u_xlat16_36 * u_xlat16_6.x;
    u_xlat16_50 = u_xlat16_50 * u_xlat16_50;
    u_xlat16_50 = u_xlat16_50 * u_xlat16_6.x;
    u_xlat16_50 = u_xlat16_50 * 4.0;
    u_xlat16_50 = u_xlat16_30 / u_xlat16_50;
    u_xlat16_6.xzw = vec3(u_xlat16_50) * u_xlat16_8.xyz + vec3(-9.99999975e-05, -9.99999975e-05, -9.99999975e-05);
    u_xlat16_14.xyz = max(u_xlat16_6.xzw, vec3(0.0, 0.0, 0.0));
    u_xlat16_14.xyz = min(u_xlat16_14.xyz, vec3(100.0, 100.0, 100.0));
    u_xlat16_30 = u_xlat16_45 * u_xlat16_47;
    u_xlat16_30 = (-u_xlat16_30) * u_xlat16_39.y + 1.0;
    u_xlat16_45 = u_xlat16_6.y * _GlossMapScale + (-u_xlat16_18);
    u_xlat16_45 = u_xlat16_45 + 1.0;
#ifdef UNITY_ADRENO_ES3
    u_xlat16_45 = min(max(u_xlat16_45, 0.0), 1.0);
#else
    u_xlat16_45 = clamp(u_xlat16_45, 0.0, 1.0);
#endif
    u_xlat16_3.xyz = u_xlat16_5.xyz * vec3(u_xlat16_18) + u_xlat16_14.xyz;
    u_xlat16_7.xyz = u_xlat16_7.xyz * u_xlat16_10.xyz;
    u_xlat16_3.xyz = u_xlat16_3.xyz * u_xlat16_13.xyz + u_xlat16_7.xyz;
    u_xlat16_7.xyz = u_xlat16_12.xyz * vec3(u_xlat16_30);
    u_xlat16_15.x = (-u_xlat16_15.x) + 1.0;
    u_xlat16_15.x = u_xlat16_15.x * u_xlat16_15.x;
    u_xlat16_15.x = u_xlat16_15.x * u_xlat16_15.x;
    u_xlat16_9.xzw = (-u_xlat16_8.xyz) + vec3(u_xlat16_45);
    u_xlat16_15.xyz = u_xlat16_15.xxx * u_xlat16_9.xzw + u_xlat16_8.xyz;
    u_xlat16_15.xyz = u_xlat16_7.xyz * u_xlat16_15.xyz + u_xlat16_3.xyz;
    u_xlat16_15.xyz = max(u_xlat16_15.xyz, vec3(0.0, 0.0, 0.0));
    u_xlat16_5.xyz = log2(u_xlat16_15.xyz);
    u_xlat16_5.xyz = u_xlat16_5.xyz * vec3(0.416666657, 0.416666657, 0.416666657);
    u_xlat16_5.xyz = exp2(u_xlat16_5.xyz);
    u_xlat16_5.xyz = u_xlat16_5.xyz * vec3(1.05499995, 1.05499995, 1.05499995) + vec3(-0.0549999997, -0.0549999997, -0.0549999997);
    u_xlat16_5.xyz = max(u_xlat16_5.xyz, vec3(0.0, 0.0, 0.0));
    u_xlat16_0.x = (-u_xlat16_0.x) + 1.0;
#ifdef UNITY_ADRENO_ES3
    u_xlat16_0.x = min(max(u_xlat16_0.x, 0.0), 1.0);
#else
    u_xlat16_0.x = clamp(u_xlat16_0.x, 0.0, 1.0);
#endif
    u_xlat16_50 = max(_RakingLightSoftness, 1.0);
    u_xlat16_6.x = log2(u_xlat16_0.x);
    u_xlat16_50 = u_xlat16_50 * u_xlat16_6.x;
    u_xlat16_50 = exp2(u_xlat16_50);
    u_xlat16_0.x = u_xlat16_48 + -1.0;
    u_xlat16_0.x = _BrightnessInShadow * u_xlat16_0.x + 1.0;
    u_xlat16_0.x = u_xlat16_9.y * u_xlat16_0.x;
    u_xlat16_15.xyz = vec3(u_xlat16_50) * _RakingLightColor.xyz;
    u_xlat16_0.xyz = u_xlat16_0.xxx * u_xlat16_15.xyz;
    u_xlat16_45 = dot(_DirLight.xyz, _DirLight.xyz);
    u_xlat16_45 = inversesqrt(u_xlat16_45);
    u_xlat16_3.xyz = vec3(u_xlat16_45) * _DirLight.xyz;
    u_xlat16_45 = dot(u_xlat16_2.xyz, u_xlat16_3.xyz);
#ifdef UNITY_ADRENO_ES3
    u_xlat16_45 = min(max(u_xlat16_45, 0.0), 1.0);
#else
    u_xlat16_45 = clamp(u_xlat16_45, 0.0, 1.0);
#endif
    SV_Target0.xyz = u_xlat16_0.xyz * vec3(u_xlat16_45) + u_xlat16_5.xyz;
    SV_Target0.w = u_xlat16_1.w;
    return;
}