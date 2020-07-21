using System;
using System.Linq;

using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using UnityEngine;

using VoxelEngine.Blocks;

namespace VoxelEngine.Chunk {

    public class Chunk : MonoBehaviour {

        private MeshFilter _meshFilter;

        private void Awake() {
            _meshFilter = GetComponent<MeshFilter>();
        }

        private void Start() {
            // Height Map, populating blocks array

            var position = transform.position;
            
            var blocks = new NativeArray<Block>(4096, Allocator.TempJob);

            for (int x = 0; x < 16; x++) {
                for (int z = 0; z < 16; z++) {
                    var y = Mathf.FloorToInt(Mathf.PerlinNoise((position.x + x) * 0.15f, (position.z + z) * 0.15f) *
                        16);

                    for (int i = 0; i < y; i++) {
                        blocks[BlockExtensions.GetBlockIndex(new int3(x, i, z))] = Block.Stone;
                    }

                    for (int i = y; i < 16; i++) {
                        blocks[BlockExtensions.GetBlockIndex(new int3(x, i, z))] = Block.Air;
                    }
                }
            }

            // scheduling the job

            var meshData = new ChunkJob.MeshData {
                Vertices = new NativeList<int3>(Allocator.TempJob),
                Triangles = new NativeList<int>(Allocator.TempJob)
            };
            
            var jobHandle = new ChunkJob {
                meshData = meshData,
                chunkData = new ChunkJob.ChunkData {
                    Blocks = blocks
                }
            }.Schedule();
            
            jobHandle.Complete();
            
            // updating the mesh
            var mesh = new Mesh {
                vertices = meshData.Vertices.ToArray().Select(vertex => new Vector3(vertex.x, vertex.y, vertex.z)).ToArray(),
                triangles = meshData.Triangles.ToArray()
            };
            
            meshData.Vertices.Dispose();
            meshData.Triangles.Dispose();
            blocks.Dispose();
            
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();
            
            _meshFilter.mesh = mesh;
        }

    }

}