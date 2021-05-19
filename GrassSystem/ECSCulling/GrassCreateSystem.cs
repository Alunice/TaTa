using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Collections;
using System.Linq;

public class GrassCreateSystem : JobComponentSystem
{
    private struct ArrayData
    {
        public NativeArray<float3> posList;
        public NativeArray<Quaternion> rotList;
        public int tile_id;
        public int writeOffset;
        
        public NativeArray<float3> FinalPosList;
        public NativeArray<Quaternion> FinalRotList;
        public NativeArray<int> FinalTileList;
    }

    private struct grassArrayDataJob : IJobParallelFor
    {
        public ArrayData aryData;
        public void Execute(int index)
        {
            aryData.FinalPosList[aryData.writeOffset + index] = aryData.posList[index];
            aryData.FinalRotList[aryData.writeOffset + index] = aryData.rotList[index];
            aryData.FinalTileList[aryData.writeOffset + index] = aryData.tile_id;
        }
    }

    private struct Data
    {
        public NativeArray<Entity> entityArray;
        public NativeArray<float3> posList;
        public NativeArray<quaternion> rotList;
        public float3 extend;
        public float3 center;
        public NativeArray<int> tileIDList;
    }
    private struct grassCreateJob : IJobParallelFor
    {
        [ReadOnly]
        public Data data;
        public void Execute(int index)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var randomRot = data.rotList[index];
            var randomPos = data.posList[index];

            Entity entityGroup = data.entityArray[index];
            entityManager.SetComponentData(entityGroup, new Translation { Value = randomPos });
            entityManager.SetComponentData(entityGroup, new Rotation { Value = randomRot });
            entityManager.SetComponentData(entityGroup, new RenderBounds { Value = new AABB() { Extents = data.extend, Center = data.center } });
            entityManager.SetComponentData(entityGroup, new EntityCreate.GeneraGrassTile() { tile_id = data.tileIDList[index] });

        }
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        int loopCount = CullingManager.CreateIndexList.Count;
        if(loopCount > 0)
        {
            
            var cullMgr = CullingManager.Get();
            var entityMgr = cullMgr.EntityMgr;
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityArchetype entityArchetype = entityManager.CreateArchetype(
               typeof(LocalToWorld),
               typeof(RenderMesh),
               typeof(RenderBounds),
               typeof(Translation),
               typeof(Rotation),
               typeof(EntityCreate.GeneraGrassTile),
               typeof(FrozenRenderSceneTag)
               );
            int InstanceCount = entityMgr.InstanceCount;
            int totalCount = loopCount * InstanceCount;
            RenderMesh RM = cullMgr.GetRenderMesh();
            AABB tmpAB = cullMgr.grassAABB;

            var grassEntity = entityManager.CreateEntity(entityArchetype);
            entityManager.SetSharedComponentData(grassEntity, RM);
            entityManager.SetSharedComponentData(grassEntity, new FrozenRenderSceneTag { HasStreamedLOD = 1, SectionIndex = 1 });
            NativeArray<Entity> entityGroupArray = new NativeArray<Entity>(totalCount, Allocator.Persistent);
            //entityManager.CreateEntity(entityArchetype, entityGroupArray);
            entityManager.Instantiate(grassEntity, entityGroupArray);
            entityManager.DestroyEntity(grassEntity);
            
            NativeArray<float3> totalPos = new NativeArray<float3>(totalCount, Allocator.Persistent);
            NativeArray<quaternion> totalRot = new NativeArray<quaternion>(totalCount, Allocator.Persistent);
            NativeArray<int> totalTileID = new NativeArray<int>(totalCount, Allocator.Persistent);
            
            for (int i = 0; i < loopCount; i++)
            {
                int startOff = i * InstanceCount;
                NativeArray<float3>.Copy(CullingManager.posList[CullingManager.CreateIndexList[i]].ToArray(),0, totalPos, startOff, InstanceCount);
                NativeArray<quaternion>.Copy(CullingManager.rotList[CullingManager.CreateIndexList[i]].ToArray(), 0, totalRot, startOff, InstanceCount);
                int[] tileIDtmp = Enumerable.Repeat(CullingManager.CreateIndexList[i], InstanceCount).ToArray();
                NativeArray<int>.Copy(tileIDtmp, 0, totalTileID, startOff, InstanceCount); 
                /*
                for (int j = 0; j < InstanceCount; j++)
                {
                    int nowIndex = i * InstanceCount + j;
                    totalPos[nowIndex] = CullingManager.posList[CullingManager.CreateIndexList[i]][j];
                    totalRot[nowIndex] = CullingManager.rotList[CullingManager.CreateIndexList[i]][j];
                    totalTileID[nowIndex] = CullingManager.CreateIndexList[i];
                }*/

                /*
               var tpPos = new NativeArray<float3>(CullingManager.posList[CullingManager.CreateIndexList[i]].ToArray(), Allocator.Persistent);
               var tpRot = new NativeArray<Quaternion>(CullingManager.rotList[CullingManager.CreateIndexList[i]].ToArray(), Allocator.Persistent);
               grassArrayDataJob datajob = new grassArrayDataJob()
               {
                   aryData = new ArrayData()
                   {
                       posList = tpPos,
                       rotList = tpRot,
                       tile_id = CullingManager.CreateIndexList[i],
                       writeOffset = i * InstanceCount,
                       FinalTileList = totalTileID,
                       FinalPosList = totalPos,
                       FinalRotList = totalRot

                   }
               };
               var data_handle = datajob.Schedule(InstanceCount, 1);
               data_handle.Complete();
               tpPos.Dispose();
               tpRot.Dispose(); */
            }

            grassCreateJob job = new grassCreateJob()
            {
                data = new Data()
                {
                    entityArray = entityGroupArray,
                    posList = totalPos,
                    rotList = totalRot,
                    extend = tmpAB.Extents,
                    center = tmpAB.Center,
                    tileIDList = totalTileID
                }
            };
            var t_handle = job.Schedule(totalCount, 1);
            CullingManager.CreateIndexList.Clear();
            t_handle.Complete();


            entityGroupArray.Dispose();
            totalPos.Dispose();
            totalRot.Dispose();
            totalTileID.Dispose();

            
        }


        return inputDeps;
    }
}