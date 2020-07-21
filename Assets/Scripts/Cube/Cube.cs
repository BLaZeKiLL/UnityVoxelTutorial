using System;
using System.Collections.Generic;
using System.Linq;

using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using UnityEngine;

using VoxelEngine.Blocks;

namespace VoxelEngine.Cube {

    public class Cube : MonoBehaviour {

        private MeshFilter _meshFilter;
        private JobHandle jobHandle;
        private CubeJob.MeshData meshData;

        private bool jobCompleted;

        private void Awake() {
            _meshFilter = GetComponent<MeshFilter>();
        }

        private void Start() {
            meshData = new CubeJob.MeshData {
                Vertices = new NativeList<int3>(Allocator.TempJob),
                Triangles = new NativeList<int>(Allocator.TempJob)
            };

            var cubeJob = new CubeJob {
                position = int3.zero,
                meshData = meshData
            };

            jobHandle = cubeJob.Schedule();
        }

        private void Update() {
            if (!jobCompleted && jobHandle.IsCompleted) {
                jobCompleted = true;
                OnComplete();
            }
        }

        private void OnComplete() {
            jobHandle.Complete();
            
            var mesh = new Mesh {
                vertices = meshData.Vertices.ToArray().Select(vertex => new Vector3(vertex.x, vertex.y, vertex.z)).ToArray(),
                triangles = meshData.Triangles.ToArray()
            };
            
            meshData.Vertices.Dispose();
            meshData.Triangles.Dispose();
            
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();
            
            _meshFilter.mesh = mesh;
        }

    }

}