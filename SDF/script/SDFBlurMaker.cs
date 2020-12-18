using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class SDFBlurMaker : MonoBehaviour
{
    public Texture2D sdf_1;
    public Texture2D sdf_2;
    public int _SampleTimes = 200;
    public bool starmake = false;
    OukeUtils myutils = OukeUtils.Get();

    private void Update()
    {
        if(starmake && sdf_1 != null && sdf_2 != null)
        {
            starmake = false;
            var tex1 = myutils.duplicateTexture(sdf_1);
            var tex2 = myutils.duplicateTexture(sdf_2);
            SDFBlur(tex1, tex2, _SampleTimes);
        }
    }

    private void SDFBlur(Texture2D sdf1, Texture2D sdf2, int sampletimes)
    {
        string outputPath = Application.dataPath.Replace("Assets", "") + AssetDatabase.GetAssetPath(sdf_1).Replace(sdf_1.name, sdf_1.name + "_SDFBlur");
        int WIDTH = sdf1.width;
        int HEIGHT = sdf1.height;
        Color[] pixels = new Color[WIDTH * HEIGHT];
        for (int y = 0; y < HEIGHT; y++)
        {
            for(int x = 0; x < WIDTH; x++)
            {
                var dis1 = sdf1.GetPixel(x, y);
                var dis2 = sdf2.GetPixel(x, y);
                var c = SDFBlurCore(sampletimes,dis1.r,dis2.r);
                pixels[y * WIDTH + x] = new Color(c, c, c);
            }
        }
        Texture2D outTex = new Texture2D(WIDTH, HEIGHT, TextureFormat.RGB24, false);
        outTex.SetPixels(pixels);
        outTex.Apply();
        myutils.Texture2PNG(outTex, outputPath);
    }

    private float SDFBlurCore( int sampletimes, float dis1, float dis2)
    {
        float res = 0f;
       if (dis1 < 0.4999f && dis2 < 0.4999f)
            return 1.0f;
        if (dis1 > 0.5f && dis2 > 0.5f)
            return 0f;

        for(int i = 0; i < sampletimes; i++)
        {
            float lerpValue = (float)i / sampletimes;
            res += Mathf.Lerp(dis1, dis2, lerpValue) < 0.499f ? 1 : 0;
        }
        return res / sampletimes;
    }

}
