using System;

using Unity.Collections;
using Unity.Mathematics;

using UnityEngine;

namespace VoxelEngine.Blocks {

    public enum Block : ushort {

        Null = 0x0000,
        Air = 0x0001,
        Stone = 0x0002

    }

    public enum Direction {

        Forward, //+z
        Right,   //+x
        Back,    //-z
        Left,    //-x
        Up,      //+y
        Down     //-y

    }

    public struct BlockData {

        public static readonly int3[] Vertices = {
            new int3(1, 1, 1),
            new int3(0, 1, 1),
            new int3(0, 0, 1),
            new int3(1, 0, 1),
            new int3(0, 1, 0),
            new int3(1, 1, 0),
            new int3(1, 0, 0),
            new int3(0, 0, 0)
        };
        
        public static readonly int[][] Triangles = {
            new[] { 0, 1, 2, 3 },
            new[] { 5, 0, 3, 6 },
            new[] { 4, 5, 6, 7 },
            new[] { 1, 4, 7, 2 },
            new[] { 5, 4, 1, 0 },
            new[] { 3, 2, 7, 6 }
        };

    }

    public static class BlockExtensions {

        public static NativeArray<int3> GetFaceVertices(Direction direction, int scale, int3 pos) {
            var faceVertices = new NativeArray<int3>(4, Allocator.Temp);

            for (int i = 0; i < 4; i++) {
                var index = BlockData.Triangles[(int) direction][i];
                faceVertices[i] = BlockData.Vertices[index] * scale + pos;
            }

            return faceVertices;
        }

        public static int GetBlockIndex(int3 position) => position.x + position.z * 16 + position.y * 16 * 16;

        public static bool IsEmpty(this Block block) => block == Block.Air;
        
        public static int3 GetPositionInDirection(Direction direction, int x, int y, int z) {
            switch (direction) {
                case Direction.Forward:
                    return new int3(x, y, z+1);
                case Direction.Right:
                    return new int3(x+1, y, z);
                case Direction.Back:
                    return new int3(x, y, z-1);
                case Direction.Left:
                    return new int3(x-1, y, z);
                case Direction.Up:
                    return new int3(x, y+1, z);
                case Direction.Down:
                    return new int3(x, y-1, z);
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

    }

}