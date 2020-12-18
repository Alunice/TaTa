using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Mathematics;


[ExecuteInEditMode]
public class SDFMaker : MonoBehaviour
{
    public bool startGenerate = false;
    public Texture2D src;

    OukeUtils myutils = OukeUtils.Get();

    // Update is called once per frame
    void Update()
    {
        if (startGenerate)
        {
            if(src != null)
            {
                GenerateSDF8ssedt(src);
            }
            startGenerate = false;
        }
    }

    private class Point
    {
        public int dx, dy;

        public Point(int x,int y)
        {
            dx = x;
            dy = y;
        }

        public Point(Point p)
        {
            dx = p.dx;
            dy = p.dy;
        }

        public double DistSq
        {
            get { return dx * dx + dy * dy; }   
        }
    }

    private class Grid
    {
        public Point[,] outsideGrid;
        public Point[,] insideGrid;

        public Grid(int height, int width)
        {
            outsideGrid = new Point[height,width];
            insideGrid = new Point[height, width];
        }
    }

    private Point inside = new Point(0, 0);
    private Point empty = new Point(999, 999);
    private int WIDTH, HEIGHT;


    public void GenerateSDF8ssedt(Texture2D source)
    {
        string outputPath = Application.dataPath.Replace("Assets","") + AssetDatabase.GetAssetPath(source).Replace(source.name, source.name + "_SDF");
        WIDTH = source.width;
        HEIGHT = source.height;
        var srcTex = myutils.duplicateTexture(source);

        Grid sdf_grid = new Grid(WIDTH, HEIGHT);
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                Color pixel = srcTex.GetPixel(x, y);
                if(pixel.r > 0.1f)
                {
                    sdf_grid.insideGrid[x, y] = inside;
                    sdf_grid.outsideGrid[x, y] = empty;
                }
                else
                {
                    sdf_grid.insideGrid[x, y] = empty;
                    sdf_grid.outsideGrid[x, y] = inside;
                }
            }
        }

        var insideMax = SDF8ssedtCore(ref sdf_grid.insideGrid);
        var outsideMax = SDF8ssedtCore(ref sdf_grid.outsideGrid);

        Debug.Log("distance from inside is   " + insideMax.ToString());
        Debug.Log("distance from outside is   " + outsideMax.ToString());

        int maxBase = Mathf.Max(WIDTH, HEIGHT);

        Color[] pixels = new Color[WIDTH*HEIGHT];
        for (int y = 0; y < HEIGHT; y++)
        {
            for (int x = 0; x < WIDTH; x++)
            {
                double dist1 = math.sqrt(sdf_grid.insideGrid[x, y].DistSq);
                double dist2 = math.sqrt(sdf_grid.outsideGrid[x, y].DistSq);

                double dist = dist1 - dist2;
                double c = 0.5f;

                if(dist < 0)//pixel inside,color range 0-0.5
                {
                    c += dist / outsideMax * 0.5f;
                }
                else//pixel outside,color range 0.5-1
                {
                    c +=dist / insideMax * 0.5f;
                }
                pixels[y*WIDTH +x] = new Color((float)c, Mathf.Clamp01(insideMax / maxBase), Mathf.Clamp01(outsideMax / maxBase));
            }
        }

        Texture2D outTex = new Texture2D(WIDTH, HEIGHT, TextureFormat.RGB24,false);
        outTex.SetPixels(pixels);
        outTex.Apply();
        myutils.Texture2PNG(outTex, outputPath);

    }

    private Point Get( Point[,] grid,int x, int y)
    {
        if (x >= 0 && y >= 0 && x < WIDTH && y < HEIGHT)
            return new Point(grid[y,x]);
        return new Point(empty);
    }

    private void Compare(ref Point[,] grid,ref Point p,int x,int y,int offsetx,int offsety)
    {
        Point other = Get( grid, offsetx + x, offsety + y);
        other.dx += offsetx;
        other.dy += offsety;

        if (other.DistSq < p.DistSq)
            p = other;
    }

    private float SDF8ssedtCore(ref Point[,] g)
    {
        double maxValue = -1f;
        // Pass 0
        for (int y = 0; y < HEIGHT; y++)
        {
            for (int x = 0; x < WIDTH; x++)
            {
                Point p = Get(g, x, y);
                Compare(ref g, ref p, x, y, -1, 0);
                Compare(ref g, ref p, x, y, 0, -1);
                Compare(ref g, ref p, x, y, -1, -1);
                Compare(ref g, ref p, x, y, 1, -1);
                g[y, x] = p;
            }

            for (int x = WIDTH - 1; x >= 0; x--)
            {
                Point p = Get(g, x, y);
                Compare(ref g, ref p, x, y, 1, 0);
                g[y, x] = p;
            }
        }

        // Pass 1
        for (int y = HEIGHT - 1; y >= 0; y--)
        {
            for (int x = WIDTH - 1; x >= 0; x--)
            {
                Point p = Get(g, x, y);
                Compare(ref g, ref p, x, y, 1, 0);
                Compare(ref g, ref p, x, y, 0, 1);
                Compare(ref g, ref p, x, y, -1, 1);
                Compare(ref g, ref p, x, y, 1, 1);
                g[y, x] = p;
            }

            for (int x = 0; x < WIDTH; x++)
            {
                Point p = Get(g, x, y);
                Compare(ref g, ref p, x, y, -1, 0);
                g[y, x] = p;
                if(maxValue < p.DistSq)
                {
                    maxValue = p.DistSq;
                }
            }
        }
        return (float)math.sqrt(maxValue);
    }

    

    public void GenerateSDF(Texture2D source, Texture2D destination)
    {
        int sourceWidth = source.width;
        int sourceHeight = source.height;
        int targetWidth = destination.width;
        int targetHeight = destination.height;

        var pixels = new Pixel[sourceWidth, sourceHeight];
        var targetPixels = new Pixel[targetWidth, targetHeight];
        Debug.Log("sourceWidth" + sourceWidth);
        Debug.Log("sourceHeight" + sourceHeight);
        int x, y;
        Color targetColor = Color.white;
        for (y = 0; y < sourceWidth; y++)
        {
            for (x = 0; x < sourceHeight; x++)
            {
                pixels[x, y] = new Pixel();
                if (source.GetPixel(x, y) == Color.white)
                    pixels[x, y].isIn = true;
                else
                    pixels[x, y].isIn = false;
            }
        }


        int gapX = sourceWidth / targetWidth;
        int gapY = sourceHeight / targetHeight;
        int MAX_SEARCH_DIST = 512;
        int minx, maxx, miny, maxy;
        float max_distance = -MAX_SEARCH_DIST;
        float min_distance = MAX_SEARCH_DIST;

        for (x = 0; x < targetWidth; x++)
        {
            for (y = 0; y < targetHeight; y++)
            {
                targetPixels[x, y] = new Pixel();
                int sourceX = x * gapX;
                int sourceY = y * gapY;
                int min = MAX_SEARCH_DIST;
                minx = sourceX - MAX_SEARCH_DIST;
                if (minx < 0)
                {
                    minx = 0;
                }
                miny = sourceY - MAX_SEARCH_DIST;
                if (miny < 0)
                {
                    miny = 0;
                }
                maxx = sourceX + MAX_SEARCH_DIST;
                if (maxx > (int)sourceWidth)
                {
                    maxx = sourceWidth;
                }
                maxy = sourceY + MAX_SEARCH_DIST;
                if (maxy > (int)sourceHeight)
                {
                    maxy = sourceHeight;
                }
                int dx, dy, iy, ix, distance;
                bool sourceIsInside = pixels[sourceX, sourceY].isIn;
                if (sourceIsInside)
                {
                    for (iy = miny; iy < maxy; iy++)
                    {
                        dy = iy - sourceY;
                        dy *= dy;
                        for (ix = minx; ix < maxx; ix++)
                        {
                            bool targetIsInside = pixels[ix, iy].isIn;
                            if (targetIsInside)
                            {
                                continue;
                            }
                            dx = ix - sourceX;
                            distance = (int)Mathf.Sqrt(dx * dx + dy);
                            if (distance < min)
                            {
                                min = distance;
                            }
                        }
                    }

                    if (min > max_distance)
                    {
                        max_distance = min;
                    }
                    targetPixels[x, y].distance = min;
                }
                else
                {
                    for (iy = miny; iy < maxy; iy++)
                    {
                        dy = iy - sourceY;
                        dy *= dy;
                        for (ix = minx; ix < maxx; ix++)
                        {
                            bool targetIsInside = pixels[ix, iy].isIn;
                            if (!targetIsInside)
                            {
                                continue;
                            }
                            dx = ix - sourceX;
                            distance = (int)Mathf.Sqrt(dx * dx + dy);
                            if (distance < min)
                            {
                                min = distance;
                            }
                        }
                    }

                    if (-min < min_distance)
                    {
                        min_distance = -min;
                    }
                    targetPixels[x, y].distance = -min;
                }
            }
        }

        //EXPORT texture
        float clampDist = max_distance - min_distance;
        for (x = 0; x < targetWidth; x++)
        {
            for (y = 0; y < targetHeight; y++)
            {
                targetPixels[x, y].distance -= min_distance;
                float value = targetPixels[x, y].distance / clampDist;
                destination.SetPixel(x, y, new Color(1, 1, 1, value));
            }
        }
    }

    private class Pixel
    {
        public bool isIn;
        public float distance;
    }
}
