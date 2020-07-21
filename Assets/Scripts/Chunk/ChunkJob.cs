using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using VoxelEngine.Blocks;

namespace VoxelEngine.Chunk {

    public struct ChunkJob : IJob {

        public struct MeshData {

            public NativeList<int3> Vertices { get; set; }
            public NativeList<int> Triangles { get; set; }

        }
        
        public struct ChunkData {

            public NativeArray<Block> Blocks { get; set; }

        }
        
        public MeshData meshData { get; set; }
        public ChunkData chunkData { get; set; }
        
        public void Execute() {
            for (int x = 0; x < 16; x++) {
                for (int z = 0; z < 16; z++) {
                    for (int y = 0; y < 16; y++) {
                        if (chunkData.Blocks[BlockExtensions.GetBlockIndex(new int3(x,y,z))].IsEmpty()) continue;

                        for (int i = 0; i < 6; i++) {
                            var direction = (Direction) i;

                            if (Check(BlockExtensions.GetPositionInDirection(direction, x, y, z))) {
                                CreateFace(direction, new int3(x, y, z));
                            }
                        }
                    }
                }
            }
        }
        
        private void CreateFace(Direction direction, int3 pos) {
            var _vertices = BlockExtensions.GetFaceVertices(direction, 1, pos);
            
            meshData.Vertices.AddRange(_vertices);

            _vertices.Dispose();
            
            var vCount = meshData.Vertices.Length - 4;

            meshData.Triangles.Add(vCount);
            meshData.Triangles.Add(vCount + 1);
            meshData.Triangles.Add(vCount + 2);
            meshData.Triangles.Add(vCount);
            meshData.Triangles.Add(vCount + 2);
            meshData.Triangles.Add(vCount + 3);
        }

        private bool Check(int3 position) {
            if (position.x >= 16 || position.z >= 16 || position.x <= -1 || position.z <= -1 ||
                position.y <= -1) return true;
            if (position.y >= 16) return false;

            return chunkData.Blocks[BlockExtensions.GetBlockIndex(position)].IsEmpty();
        }

    }

}