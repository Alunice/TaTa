using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Jobs;

public class EntityCreate : MonoBehaviour
{
    public bool _useIJobCreate = true;
    public Camera targetCam;

    public int TILE_X = 1;
    public int TILE_Y = 1;
    public int TileSize = 10;
    [Range(0,1024)]
    public int InstanceCount = 512;


    private EntityManager entityManager;
    private CullingManager cullMgr;
    

    public GameObject grassPrefab;

    bool hasInit = true;


    public struct GeneraGrassTile: IComponentData
    {
        public int tile_id;
    }

    // Start is called before the first frame update
    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        cullMgr = CullingManager.Get();
        cullMgr.targetCam = targetCam;
        cullMgr.EntityMgr = this;
        InitInstance();   
        if (hasInit)
        {
            cullMgr.InitCullingParams(TILE_Y * TILE_X,InstanceCount);
            CreateTiles();
        }
            
    }

    void InitInstance()
    {
        var msf = grassPrefab.GetComponent<MeshFilter>();
        if (msf)
        {
            cullMgr.grassMesh = msf.sharedMesh;
        }
        else
        {
            hasInit = false;
            Debug.Log("1111");
            return;
        }
        var msr = grassPrefab.GetComponent<MeshRenderer>();
        if (msr)
        {
            cullMgr.grassMat = msr.sharedMaterial;
        }
        else
        {
            hasInit = false;
            Debug.Log("2222");
            return;
        }
        cullMgr.grassBox = grassPrefab.GetComponent<BoxCollider>();
        if (cullMgr.grassMat == null)
            hasInit = false;
        return;

    }


    void CreateTiles()
    {
        
        float Xoff = TILE_X / 2.0f * TileSize - TileSize / 2;
        float Yoff = TILE_Y / 2.0f * TileSize - TileSize / 2;
        for(int i = 0;i < TILE_X; i++)
        {
            for(int j = 0; j < TILE_Y; j++)
            {
                int index = i + j * TILE_X;
                var subTile = new GameObject();
                subTile.transform.position = new Vector3(i * TileSize - Xoff, 0, j * TileSize - Yoff);
                subTile.transform.parent = transform;
                subTile.name = "subTile_" + i + "_" + j;
                CreateECSEntity(subTile.transform.position,index);
                if(!_useIJobCreate)
                    CreateEntitiesByList(CullingManager.posList[index], CullingManager.rotList[index], index);
            }
        }
        if (_useIJobCreate)
            cullMgr.ApplyCullingGroup();

    }

    void CreateECSEntity(float3 centerPos,int index)
    {
        var poslist = CullingManager.posList[index];
        var rotlist = CullingManager.rotList[index];
        for (int i = 0; i < InstanceCount; i++)
        {
            var randomRot = Quaternion.AngleAxis(UnityEngine.Random.Range(0f, 180), Vector3.up);
            var randomPos = new float3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-0.5f, 0.5f)) * TileSize + centerPos;
            poslist.Add(randomPos);
            rotlist.Add(randomRot);
        }
        CullingManager.spheres[index] = new BoundingSphere(centerPos, TileSize * 1.5f);

    }

    public void CreateEntitiesByList(List<float3> posList, List<quaternion> rotList,int index)
    {
        EntityArchetype entityArchetype = entityManager.CreateArchetype(
           typeof(LocalToWorld),
           typeof(RenderMesh),
           typeof(RenderBounds),
           typeof(Translation),
           typeof(Rotation),
           typeof(GeneraGrassTile)
           //  typeof(MeshLODComponent),
           // typeof(MeshLODGroupComponent),
           //  typeof(FrozenRenderSceneTag)
           );

        RenderMesh RM = cullMgr.GetRenderMesh();

        NativeArray<Entity> entityGroupArray = new NativeArray<Entity>(InstanceCount, Allocator.Temp);
        entityManager.CreateEntity(entityArchetype, entityGroupArray);


        AABB tmpAB = cullMgr.grassAABB;

        for (int i = 0; i < InstanceCount; i++)
        {
            var randomRot = rotList[i];
            var randomPos = posList[i];

            Entity entityGroup = entityGroupArray[i];
            entityManager.SetComponentData(entityGroup, new Translation { Value = randomPos });
            entityManager.SetComponentData(entityGroup, new Rotation { Value = randomRot });
            entityManager.SetSharedComponentData(entityGroup, RM);
            entityManager.SetComponentData(entityGroup, new RenderBounds { Value = tmpAB });
            entityManager.SetComponentData(entityGroup, new GeneraGrassTile() { tile_id = index });
        }

        entityGroupArray.Dispose();
        
    }



    private void OnDestroy()
    {
        cullMgr.Dispose();
    }


}

