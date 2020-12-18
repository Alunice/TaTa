using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using Unity.Mathematics;

class OukeUtils 
{
    static OukeUtils utilsInstance;
    public static OukeUtils Get()
    {
        if (utilsInstance == null)
        {
            utilsInstance = new OukeUtils();
        }
        return utilsInstance;
    }


    #region Texture Load and Save
    public Texture2D duplicateTexture(Texture2D source)
    {
        return duplicateTextureCore(source);
    }

    private Texture2D duplicateTextureCore(Texture2D source)
    {
        RenderTexture tempRT = new RenderTexture(source.width, source.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        RenderTexture prert = RenderTexture.active;
        Graphics.Blit(source, tempRT);
        RenderTexture.active = tempRT;
        Texture2D readableTexture = new Texture2D(source.width, source.height);
        readableTexture.ReadPixels(new Rect(0, 0, tempRT.width, tempRT.height), 0, 0);
        readableTexture.Apply();
        RenderTexture.active = prert;
        //RenderTexture.ReleaseTemporary(tempRT);
        return readableTexture;
    }

    public void Texture2PNG(Texture2D src, string outpath)
    {
        Texture2PNGCore(src, outpath);
    }

    private void Texture2PNGCore(Texture2D src, string outpath)
    {
        byte[] bytes = src.EncodeToPNG();
        File.WriteAllBytes(outpath, bytes);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    #endregion


    #region float encode and decode to RGBA
    public float4 EncodeFloatRGBA(float v)
    {
        return EncodeFloatRGBACore(v);
    }

    // Encoding/decoding [0..1) floats into 8 bit/channel RGBA. Note that 1.0 will not be encoded properly.
    private float4 EncodeFloatRGBACore(float v)
    {
        float4 kEncodeMul = new float4(1.0f, 255.0f, 65025.0f, 16581375.0f);
        float kEncodeBit = 1.0f / 255.0f;
        float4 enc = kEncodeMul * v;
        enc = math.frac(enc);
        enc -= enc.yzww * kEncodeBit;
        return enc;
    }

    public float DecodeFloatRGBA(float4 enc)
    {
        return DecodeFloatRGBACore(enc);
    }
    private float DecodeFloatRGBACore(float4 enc)
    {
        float4 kDecodeDot =new float4(1.0f, 1 / 255.0f, 1 / 65025.0f, 1 / 16581375.0f); 
        return math.dot(enc, kDecodeDot);
    }


    public float2 EncodeFloatRG(float v)
    {
        return EncodeFloatRGCore(v);
    }

    // Encoding/decoding [0..1) floats into 8 bit/channel RG. Note that 1.0 will not be encoded properly.
    private float2 EncodeFloatRGCore(float v)
    {
        float2 kEncodeMul = new float2(1.0f, 255.0f);
        float kEncodeBit = 1.0f / 255.0f;
        float2 enc = kEncodeMul * v;
        enc = math.frac(enc);
        enc.x -= enc.y * kEncodeBit;
        return enc;
    }

    public float DecodeFloatRG(float2 enc)
    {
        return DecodeFloatRGCore(enc);
    }
    private float DecodeFloatRGCore(float2 enc)
    {
        float2 kDecodeDot = new float2(1.0f, 1 / 255.0f);
        return math.dot(enc, kDecodeDot);
    }
    #endregion

}
