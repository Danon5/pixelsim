namespace PixelSim.Gameplay
{
    public struct Pixel
    {
        public Pixel(PixelMaterialType materialType)
        {
            this.materialType = materialType;
        }
        
        public PixelMaterialType materialType;
    }
}