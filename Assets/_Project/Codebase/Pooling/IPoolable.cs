namespace VoxelSim.Pooling
{
    public interface IPoolable
    {
        void OnEnterPool();
        void OnExitPool();
    }
}