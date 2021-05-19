using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Rendering;
using System.Diagnostics;
using Unity.Collections;

public class CullingManager 
{
    private static CullingManager Instance;

    public static CullingManager Get()
    {
        if(Instance == null)
        {
            Instance = new CullingManager();
        }
        return Instance;
    }
    public static CullingGroup cullingGroup = null;

    public static BoundingSphere[] spheres;

    public static List<float3>[] posList;

    public static List<quaternion>[] rotList;

    public static int[] resultQuest;
    

    public Camera targetCam;

    public Mesh grassMesh;
    public Material grassMat;
    public BoxCollider grassBox;
    public AABB grassAABB;

    public EntityCreate EntityMgr;

    public RenderMesh RM;

    public static List<int> CreateIndexList = new List<int>();
    public static List<int> CullIndexList = new List<int>(); 


    public void InitCullingParams(int len,int instanceCount)
    {
        if(cullingGroup != null)
        {
            cullingGroup.Dispose();
            cullingGroup = null;
        }

        cullingGroup = new CullingGroup();
        cullingGroup.targetCamera = targetCam;
        
        posList = new  List<float3>[len];
        rotList = new  List<quaternion>[len];
        for (int i = 0; i < len; i++)
        {
            posList[i] = new List<float3>();
            rotList[i] = new List<quaternion>();
        }
        spheres = new BoundingSphere[len];

        resultQuest = new int[len];

        RM = new RenderMesh
        {
            mesh = grassMesh,
            material = grassMat,
            castShadows = UnityEngine.Rendering.ShadowCastingMode.Off,
            receiveShadows = false
        };

        grassAABB = new AABB { Extents = new float3(1.0f, 1.0f, 1.0f) };
        if (grassBox)
        {
            grassAABB.Center = new float3(grassBox.bounds.center);
            grassAABB.Extents = new float3(grassBox.bounds.size);
        }
    }

    public void ApplyCullingGroup()
    {
        cullingGroup.SetBoundingSpheres(spheres);
        cullingGroup.SetBoundingSphereCount(spheres.Length);
        cullingGroup.SetDistanceReferencePoint(targetCam.transform);
        cullingGroup.onStateChanged += OnStateChange;
        checkInitVisible();
    }

    public RenderMesh GetRenderMesh()
    {
        return RM;
    }

    void OnStateChange(CullingGroupEvent evt)
    {
        var cullsystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<CullingSystem>();
        if (cullsystem != null)
        {
            if (evt.hasBecomeInvisible)
            {
                if (!CullIndexList.Contains(evt.index))
                    CullIndexList.Add(evt.index);
                
            }
            else if (evt.hasBecomeVisible)
            {
                int index = evt.index;
               // Stopwatch sw = new Stopwatch();
               // sw.Start();
               // UnityEngine.Debug.Log("hasBecomeVisible " + index);
                //EntityMgr.CreateEntitiesByList(posList[index], rotList[index], index);
                CreateIndexList.Add(index);
               // sw.Stop();
               // UnityEngine.Debug.Log(string.Format("create total: {0} ms  {1}", sw.ElapsedMilliseconds, index));
            }
        }
           
    }

    void checkInitVisible()
    {
        int num = cullingGroup.QueryIndices(true, resultQuest, 0);
        for (int i = 0; i < num; i++)
        {
            int index = resultQuest[i];
           // EntityMgr.CreateEntitiesByList(posList[index], rotList[index], index);
            CreateIndexList.Add(index);
        }
    }


    public void Dispose()
    {
        if (cullingGroup != null)
        {
            cullingGroup.Dispose();
            cullingGroup = null;
        }
        posList = null;
        rotList = null;
    }



}
