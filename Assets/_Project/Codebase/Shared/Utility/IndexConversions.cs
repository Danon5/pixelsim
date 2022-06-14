using UnityEngine;

namespace PixelSim.Shared.Utility
{
    public static class IndexConversions
    {
        public static int Index2DTo1D(in int x, in int y, in int size)
        {
            return x * size + y;
        }
        
        public static int Index2DTo1D(in Vector2Int index, in int size)
        {
            return index.x * size + index.y;
        }

        public static Vector2Int Index1DTo2D(in int index, in int size)
        {
            int xIndex = index / size;
            int yIndex = index % size;
            
            return new Vector2Int(xIndex, yIndex);
        }
    }
}