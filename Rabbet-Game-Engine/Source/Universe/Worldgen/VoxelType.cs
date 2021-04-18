namespace RabbetGameEngine
{
    public class VoxelType
    {
        //static vars
        public static readonly int NUM_MAX_VOXEL_TYPES = 256;
        private static int NUM_VOXEL_TYPES = 0;
        private static byte voxelIdItter = 0;//for automatically assigning voxel ids.

        /// <summary>
        /// voxel at index 0 is null. represents air.
        /// </summary>
        private static VoxelType[] voxelTypes = new VoxelType[NUM_MAX_VOXEL_TYPES];

        //declaring all voxel types. Voxel ids are assigned based on the order they are declared here.
        public static readonly VoxelType stone = new VoxelType(true);
        public static readonly VoxelType dirt = new VoxelType(true);
        public static readonly VoxelType grass = new VoxelType(true);
        public static readonly VoxelType wood = new VoxelType(true);

        public static VoxelType getVoxelById(byte id)
        {
            return voxelTypes[id];
        }

        public static bool isVoxelOpaque(byte id)
        {
            return id == 0 ? false : voxelTypes[id].isOpaque;
        }

        public static int numVoxels
        {
            get => NUM_VOXEL_TYPES;
        }


        //Class starts here ###########################################################################################################################
        private byte voxelID = 0;
        private bool isOpaque = true;

        public VoxelType(bool isOpaque)
        {
            byte id = ++voxelIdItter;
            voxelID = id;
            this.isOpaque = isOpaque;
            voxelTypes[id] = this;
            NUM_VOXEL_TYPES++;
        }


        public byte id
        {
            get => voxelID;
        }

        public bool opaque
        {
            get => isOpaque;
        }

    }
}
