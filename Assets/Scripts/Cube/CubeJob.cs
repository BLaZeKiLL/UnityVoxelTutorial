using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using VoxelEngine.Blocks;

namespace VoxelEngine.Cube {

    public struct CubeJob : IJob {

        public struct MeshData {

            public NativeList<int3> Vertices { get; set; }
            public NativeList<int> Triangles { get; set; }

        }

        public int3 position { get; set; }
        
        public MeshData meshData { get; set; }

        public void Execute() {
            for (int i = 0; i < 6; i++) {
                CreateFace((Direction) i, position);
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
        
    }

}