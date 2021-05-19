using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;


[UpdateBefore(typeof(SimulationSystemGroup))]
public class CullingSystem : JobComponentSystem
{

    /*

    protected override void OnUpdate()
    {
        if(CullingManager.CullIndexList.Count > 0)
        {
            Entities.WithAll<EntityCreate.GeneraGrassTile>().ForEach((Entity en, ref EntityCreate.GeneraGrassTile GrassTile) => {

                if (CullingManager.CullIndexList.Contains(GrassTile.tile_id))
                {
                    PostUpdateCommands.DestroyEntity(en);
                }
            });
            CullingManager.CullIndexList.Clear();
        }
        return;
    }*/
    EntityCommandBufferSystem EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        EntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if(CullingManager.CullIndexList.Count > 0)
        {
            int totalCount = CullingManager.CullIndexList.Count;
            var EntityCommandBuffer = EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            NativeArray<int> totalTileID = new NativeArray<int>(totalCount, Allocator.Persistent);
            for(int i = 0; i < totalCount; i++)
            {
                totalTileID[i] = CullingManager.CullIndexList[i];
            }
            var jobhandle = Entities.ForEach((Entity en, int nativeThreadIndex, ref EntityCreate.GeneraGrassTile GrassTile) => {

                for(int i = 0; i < totalCount; i++)
                {
                    if(totalTileID[i] == GrassTile.tile_id)
                    {
                        EntityCommandBuffer.DestroyEntity(nativeThreadIndex, en);
                        return;
                    }
                }
            }).Schedule(inputDeps);
            CullingManager.CullIndexList.Clear();
            EntityCommandBufferSystem.AddJobHandleForProducer(jobhandle);
            jobhandle.Complete();
            totalTileID.Dispose();
            return jobhandle;
        }

        return inputDeps;
    }
}
