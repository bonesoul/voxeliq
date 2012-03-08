using VolumetricStudios.VoxeliqGame.Blocks;
using VolumetricStudios.VoxeliqGame.Chunks;
using VolumetricStudios.VoxeliqGame.Generators.Biomes;
using VolumetricStudios.VoxeliqGame.Utils.Algorithms;

namespace VolumetricStudios.VoxeliqGame.Generators.Terrain
{
    /// <summary>
    /// Biomed terrain generators.
    /// </summary>
    public class BasicTerrain : TerrainGenerator
    {
        protected BiomeGenerator BiomeGenerator { get; private set; }
        public const int WaterLevel = 99;

        protected float RockHeight;
        protected int DirtHeight;

        public BasicTerrain(BiomeGenerator biomeGenerator)
        {
            this.BiomeGenerator = biomeGenerator;
        }

        protected override void GenerateChunk(Chunk chunk)
        {
            for (byte x = 0; x < Chunk.WidthInBlocks; x++)
            {
                int worldPositionX = chunk.WorldPosition.X + x;
                int seededWorldPositionX = worldPositionX + this.Seed;

                for (byte z = 0; z < Chunk.LenghtInBlocks; z++)
                {
                    int worldPositionZ = chunk.WorldPosition.Z + z;
                    this.GenerateTerrain(chunk, x, z, worldPositionX, worldPositionZ, seededWorldPositionX);
                }
            }

            // this.GenerateWater(chunk);

            if (this.BiomeGenerator != null)
                this.BiomeGenerator.ApplyBiome(chunk);
        }

        protected virtual void GenerateTerrain(Chunk chunk, byte x, byte z, int worldPositionX, int worldPositionZ, int seededWorldPositionX)
        {
            this.RockHeight = this.GetRockHeight(seededWorldPositionX, worldPositionZ);
            this.DirtHeight = this.GetDirtHeight(seededWorldPositionX, worldPositionZ, RockHeight);

            //int offset = x * Chunk.FlattenOffset + z * Chunk.HeightInBlocks;
            var offset = BlockCache.BlockIndexByWorldPosition(worldPositionX, worldPositionZ);

            for (int y = Chunk.MaxHeightInBlocks; y >= 0; y--) 
            {
                BlockType blockType;

                if (y > DirtHeight) // air
                    blockType = BlockType.None;
                else if (y > RockHeight) // dirt
                    blockType = BlockType.Dirt;
                else // rock level
                    blockType = BlockType.Rock;

                switch (blockType)
                {
                    case BlockType.None: 
                        if (chunk.LowestEmptyBlockOffset > y) chunk.LowestEmptyBlockOffset = (byte)y; 
                        break;
                    default: 
                        if (y > chunk.HighestSolidBlockOffset) chunk.HighestSolidBlockOffset = (byte)y; 
                        break;
                }

                //chunk.Blocks[offset + y] = new Block(blockType);
                BlockCache.Blocks[offset + y] = new Block(blockType);
            }
        }

        //protected  virtual void GenerateWater(Chunk chunk) - TODO: not working yet.
        //{
        //    for (int x = 0; x < Chunk.WidthInBlocks; x++)
        //    {
        //        for (int z = 0; z < Chunk.LenghtInBlocks; z++)
        //        {
        //            int offset = x * Chunk.FlattenOffset + z * Chunk.HeightInBlocks;
        //            for (int y=WaterLevel + 3; y>= RockHeight; y--)
        //            {
        //                if (chunk.Blocks[offset + y].Type == BlockType.None)
        //                    chunk.Blocks[offset + y] = new Block(BlockType.Water);
        //            }
        //        }
        //    }
        //}

        protected virtual int GetDirtHeight(int blockX, int blockZ, float rockHeight)
        {
            float octave1 = PerlinSimplexNoise.noise((blockX + 100) * 0.001f, blockZ * 0.001f) * 0.5f;
            float octave2 = PerlinSimplexNoise.noise((blockX + 100) * 0.002f, blockZ * 0.002f) * 0.25f;
            float octave3 = PerlinSimplexNoise.noise((blockX + 100) * 0.01f, blockZ * 0.01f) * 0.25f;
            float octaveSum = octave1 + octave2 + octave3;

            return (int)(octaveSum * (Chunk.HeightInBlocks / 8)) + (int)(rockHeight);
        }

        protected virtual float GetRockHeight(int blockX, int blockZ)
        {
            int minimumGroundheight = Chunk.HeightInBlocks / 2;
            int minimumGroundDepth = (int)(Chunk.HeightInBlocks * 0.4f);

            float octave1 = PerlinSimplexNoise.noise(blockX * 0.0001f, blockZ * 0.0001f) * 0.5f;
            float octave2 = PerlinSimplexNoise.noise(blockX * 0.0005f, blockZ * 0.0005f) * 0.35f;
            float octave3 = PerlinSimplexNoise.noise(blockX * 0.02f, blockZ * 0.02f) * 0.15f;
            float lowerGroundHeight = octave1 + octave2 + octave3;

            lowerGroundHeight = lowerGroundHeight * minimumGroundDepth + minimumGroundheight;

            return lowerGroundHeight;
        }
    }
}
