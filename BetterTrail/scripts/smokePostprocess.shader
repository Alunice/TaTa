Shader "SHAC/smokePostShader"
{
    Properties
    {  
       _MainTex ("Texture", any) = "" {}
    }
    SubShader
    {
        LOD 100


        Pass
        {
            Name "smokePass0"
            Tags {"RenderType" = "Opaque"  "RenderPipeline" = "UniversalPipeline" }
            ZTest Always
            ZWrite Off
            Cull Off

             HLSLPROGRAM
             #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
             #pragma vertex vert
             #pragma fragment frag
            
             struct a2v {
                 float4 positionOS   : POSITION;
                 float2 uv           : TEXCOORD0;
             };

             struct v2f {
                 float4 positionCS  : SV_POSITION;
                 float2 uv           : TEXCOORD0;
             };

             sampler2D _MainTex;
             float _FloatArray[6];//_Dissipation,_FlowThreshold,_FlowByTimeBase
             float4 _VectorArray[6];//_posNow,_posLast,_windDir
             int _TrailCount;


             // Encoding/decoding [0..1) floats into 8 bit/channel RG. Note that 1.0 will not be encoded properly.
            inline float2 EncodeFloatRG( float v )
            {
                float2 kEncodeMul = float2(1.0, 255.0);
                float kEncodeBit = 1.0/255.0;
                float2 enc = kEncodeMul * v;
                enc = frac (enc);
                enc.x -= enc.y * kEncodeBit;
                return enc;
            }
            inline float DecodeFloatRG( float2 enc )
            {
                float2 kDecodeDot = float2(1.0, 1/255.0);
                return dot( enc, kDecodeDot );
            }

            half2 SimulateSmokeCore(half2 tc,half2 bc,half2 cl,half2 cr,half2 cc,float2 fragPos,int offset ){
                 // float maxdepth = DecodeFloatRG(cc.gb);
                 float maxdepth = cc.g;

                float2 p1 = _VectorArray[3 * offset +1].xy;
                float2 p2 = _VectorArray[3 * offset +0].xy; 
                float2 p =  fragPos;

                float projectPos = (p2.x - p1.x) * (p.x - p1.x) + (p2.y - p1.y) * (p.y - p1.y); //|AB*AP|：矢量乘
                  
                float d2 = pow(length(p2 - p1),2);

                if(projectPos >= 0 && projectPos <= d2){
                    float r = projectPos / d2;
                    float2 pc = lerp(p1,p2,r);
                    float dist  = distance(pc, p);
                    float half_radius = lerp(_VectorArray[3 * offset +1].z,_VectorArray[3 * offset +0].z,r);
                    float radius = 16 * 5/half_radius;
                    if(dist < radius){
                        float atten = pow(dist / radius,4);
                        float antialtas =1 - pow(dist / radius,4);
                        float zz = (0.95 -0.94 * atten);
                        zz = zz * 0.5 + 0.5 * max(0.95 -0.8 * atten,cc.r);
                        cc.r = max(cc.r,zz);
                        cc.r = lerp(cc.r,(tc.r+bc.r+cl.r+cr.r+ cc.r) / 5,atten);
                        maxdepth = (20 -half_radius )/ 20 ;
                    }
                }
                if(projectPos < 0.01){
                    float dist = distance(p2,p);
                    float radius = 16 * 5/_VectorArray[3 * offset +1].z;
                     if(dist < radius){
                        float atten = pow(dist / radius,4);
                        float antialtas =1 - pow(dist / radius,4);
                        float zz = (0.9 -0.8 * atten);
                        zz = zz * 0.5 + 0.5 * max(0.95 -0.8 * atten,cc.r);
                        cc.r = max(cc.r,zz);
                        cc.r = lerp(cc.r,(tc.r+bc.r+cl.r+cr.r+ cc.r) / 5,atten);
                        maxdepth = (20 -_VectorArray[3 * offset +1].z )/ 20 ;
                     }
                }
                 // Diffusion step
                 //float flowValue = ((_VectorArray[2] * half4(tc,bc,cl,cr)) - 4*cc) * 64;//256 /4 =64
                 float4 windDirIntensity = lerp(_VectorArray[3 * offset +2],half4(1,1,1,1),saturate(cc.r * _FloatArray[3 * offset +1]));

                 float flowValue = ((dot(windDirIntensity ,half4(tc.r,bc.r,cl.r,cr.r))) - 4*cc.r) * 64;
                 half signValue = sign(flowValue);
                 float absValue = min(1,abs(flowValue));

                 float factor = _FloatArray[3 * offset +0] *(signValue * absValue) / 256;

                 // float factor =_FloatArray[0] *flowValue / 256;

                if(factor > 0){
                    half maxdepth1 = max(max(tc.r,bc.r),max(cl.r,cr.r));
                    maxdepth1 = max(cc.r,maxdepth1) ;
                    if(tc.r >= maxdepth1){
                        maxdepth = tc.g;
                    }
                    if(bc.r >= maxdepth1){
                        maxdepth = bc.g;
                    }
                    if(cl.r >= maxdepth1){
                        maxdepth = cl.g;
                    }
                    if(cr.r >= maxdepth1){
                        maxdepth = cr.g;
                    }
                }

                // if(factor > 0){
                //     half maxdepth1 = max(max(tc.r,bc.r),max(cl.r,cr.r));
                //     maxdepth1 = max(cc.r,maxdepth1) ;
                //     if(tc.r >= maxdepth1){
                //         maxdepth = DecodeFloatRG(tc.gb);
                //     }
                //     if(bc.r >= maxdepth1){
                //         maxdepth= DecodeFloatRG(bc.gb);
                //     }
                //     if(cl.r >= maxdepth1){
                //         maxdepth = DecodeFloatRG(cl.gb);
                //     }
                //     if(cr.r >= maxdepth1){
                //         maxdepth = DecodeFloatRG(cr.gb);
                //     }
                // }

                //  Minimum flow
                if (factor  >= -0.004 * _FloatArray[3 * offset +2] && factor <= 0.0)
                    factor = -0.004 * _FloatArray[3 * offset +2];
                cc.r += factor ;
               

                if(cc.r < 0.005){
                    maxdepth = 0;
                }

                // cc.gb = EncodeFloatRG(maxdepth);
                cc.g = maxdepth;
                return half2(cc.rg);

            }

             
             v2f vert(a2v v) {
                 v2f o;
                 o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                 o.uv = v.uv;
                 return o;
             }

             half4 frag(v2f i) : SV_Target {

                 half2 uv = i.uv;

                 // Neighbour cells
                 float w = 1 / _ScreenParams.x ;
                 float h = 1 / _ScreenParams.y ;

                 half4 tc = tex2D(_MainTex, uv + half2(-0, -h)); // Top Centre
                 half4 bc = tex2D(_MainTex, uv + half2(0, +h)); // Bottom Centre
                 half4 cl = tex2D(_MainTex, uv + half2(-w, 0)); // Centre Left
                 half4 cr = tex2D(_MainTex, uv + half2(+w, 0)); // Centre Right
                 half4 cc = tex2D(_MainTex, uv + half2(0, 0)); // Centre Centre

                 float2 fragPos = i.uv * _ScreenParams.xy;
                 half2 testzzr = SimulateSmokeCore(tc.rg,bc.rg,cl.rg,cr.rg,cc.rg,fragPos ,0 );
                 half2 testzzp =SimulateSmokeCore(tc.ba,bc.ba,cl.ba,cr.ba,cc.ba,fragPos ,1 );   
                 return float4(testzzr,testzzp); 
             }
             ENDHLSL
        }

         Pass
        {
            Name "smokePass1"
            Tags {"RenderType" = "Transparent"  "RenderPipeline" = "UniversalPipeline" }
            ZTest Always
            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

             HLSLPROGRAM
             #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
             #pragma vertex vert
             #pragma fragment frag

            
             struct a2v {
                 float4 positionOS   : POSITION;
                 float2 uv           : TEXCOORD0;
             };

             struct v2f {
                 float4 positionCS  : SV_POSITION;
                 float2 uv           : TEXCOORD0;
             };

             sampler2D _MainTex;
             sampler2D _SmokeFlowTex;
             TEXTURE2D(_CameraDepthTexture);
             SAMPLER(sampler_CameraDepthTexture);
             half4 _SmokeColor;


             // Encoding/decoding [0..1) floats into 8 bit/channel RG. Note that 1.0 will not be encoded properly.
            inline float2 EncodeFloatRG( float v )
            {
                float2 kEncodeMul = float2(1.0, 255.0);
                float kEncodeBit = 1.0/255.0;
                float2 enc = kEncodeMul * v;
                enc = frac (enc);
                enc.x -= enc.y * kEncodeBit;
                return enc;
            }
            inline float DecodeFloatRG( float2 enc )
            {
                float2 kDecodeDot = float2(1.0, 1/255.0);
                return dot( enc, kDecodeDot );
            }
             
             v2f vert(a2v v) {
                 v2f o;
                 o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                 o.uv = v.uv;
                 return o;
             }

             half4 frag(v2f i) : SV_Target { 

                half2 uv = i.uv;

                // half4 col = tex2D(_MainTex,uv);
                float4 flowtex = tex2D(_SmokeFlowTex,uv);
                float4 depColor = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.uv);

                float depth =LinearEyeDepth(depColor.r,_ZBufferParams);
                // float flowDepth = DecodeFloatRG(flowtex.gb);
                float flowDepth = flowtex.g;
                float flowDepth1 = flowtex.a;

                float smokeDepth =20* (1 - flowDepth);
                float smokeDepth1 =20* (1 - flowDepth1);

                float alpha = 0;
                float alpha1 = 0;

                if(smokeDepth < depth){
                    alpha = flowtex.r;
                }

                if(smokeDepth1 < depth){
                    alpha1 = flowtex.b;
                }

                float finalAlpha = 1 - (1 - alpha1) * (1 - alpha);


                half3 finalCol = 1;
                if(smokeDepth + 0.5 < smokeDepth1){//smokeDepth near the camera
                    finalCol = alpha1 * (1 - alpha) * half3(1,0,0) + alpha * half3(0,0,1);
                }else{
                    finalCol = alpha * (1 - alpha1) * half3(0,0,1) + alpha1 * half3(1,0,0);
                }
                // if(smokeDepth < depth){
                //     float fade = saturate (0.95 * (depth-smokeDepth));
                //     alpha = flowtex.r * fade;
                //     half3 changeColor = lerp(0.5,_SmokeColor.rgb,alpha);
                //     //col.rgb =col.rgb *(1 - alpha) + alpha * changeColor ;
                // }

                return half4(finalCol / finalAlpha,finalAlpha);
                 
             }
             ENDHLSL
        }
    }
}
