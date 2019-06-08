﻿using Common;
using System;
using System.Collections.Generic;

namespace Campus.GridTerrain
{
    /// <summary>
    /// Class used to make safe changes to terrain
    /// It checks on what moves are valid.
    /// It smooths out blocky changes.
    /// </summary>
    public class SafeTerrainEditor
    {
        // Shortcut for BFS
        private readonly int[] dx = new[] { -1, 0, 1, 0 };
        private readonly int[] dy = new[] { 0, -1, 0, 1 };

        private GridMesh _terrain;
        private bool[,] _vertexAnchored;

        /// <summary>
        /// Instantiates an instance of a SafeTerrainEditor.
        /// </summary>
        /// <param name="terrain">The terrain</param>
        public SafeTerrainEditor(GridMesh terrain)
        {
            _terrain = terrain;

            // Anchor all corners, otherwise start unanchored
            _vertexAnchored = new bool[_terrain.CountX + 1, _terrain.CountZ + 1];
            for (int i = 0; i <= _terrain.CountX; ++i)
                for (int j = 0; j <= _terrain.CountZ; ++j)
                    _vertexAnchored[i, j] = (i == 0 || j == 0 || i == _terrain.CountX - 1 || j == _terrain.CountZ - 1) ? true : false;
        }

        /// <summary>
        /// Set the grid square to be anchored by construction.
        /// </summary>
        /// <param name="x">The x grid coordinate.</param>
        /// <param name="z">The z grid coordinate.</param>
        public void SetAnchored(int x, int z)
        {
            if (x < 0 || x > _terrain.CountX || z < 0 || z > _terrain.CountZ)
                GameLogger.FatalError("Attempted to anchor grid outside of range! ({0},{1}) is outside of ({2},{3})", x, z, _terrain.CountX, _terrain.CountZ);

            _vertexAnchored[x, z] = _vertexAnchored[x, z + 1] = _vertexAnchored[x + 1, z] = _vertexAnchored[x + 1, z + 1] = true;
        }

        /// <summary>
        /// Set the grid square to be anchored by construction.
        /// </summary>
        /// <param name="x">The x grid coordinate.</param>
        /// <param name="z">The z grid coordinate.</param>
        public void RemoveAnchor(int x, int z)
        {
            if (x < 0 || x > _terrain.CountX || z < 0 || z > _terrain.CountZ)
                GameLogger.FatalError("Attempted to remove anchor from grid outside of range! ({0},{1}) is outside of ({2},{3})", x, z, _terrain.CountX, _terrain.CountZ);

            _vertexAnchored[x, z] = _vertexAnchored[x, z + 1] = _vertexAnchored[x + 1, z] = _vertexAnchored[x + 1, z + 1] = false;
        }

        /// <summary>
        /// Free anchored grid squares.
        /// </summary>
        /// <param name="xBase">The starting x coordinate.</param>
        /// <param name="zBase">The starting z coordinate.</param>
        /// <param name="freeGrids">Array of grid squares to set as free.</param>
        public void RemoveAnchorGrid(int xBase, int zBase, bool[,] freeGrids)
        {
            int xSize = freeGrids.GetLength(0);
            int zSize = freeGrids.GetLength(1);

            if (xBase < 0 || xBase + xSize > _terrain.CountX || zBase < 0 || zBase + zSize > _terrain.CountZ)
                GameLogger.FatalError("Attempted to remove anchor from outside of range! ({0},{1}) + ({2},{3}) is outside of ({4},{5})", xBase, zBase, xSize, zSize, _terrain.CountX, _terrain.CountZ);

            for (int x = 0; x < xSize; ++x)
            {
                for (int z = 0; z < zSize; ++z)
                {
                    int gridX = xBase + x;
                    int gridZ = zBase + z;

                    if (freeGrids[x, z])
                    {
                        RemoveAnchor(gridX, gridZ);
                    }
                }
            }
        }

        /// <summary>
        /// Attempt to set the grid height for a square.
        /// Will enforce smooth terrain.
        /// </summary>
        /// <param name="x">Grid x position.</param>
        /// <param name="z">Grid z position.</param>
        /// <param name="gridHeight">Grid height to set.</param>
        /// <returns>True if the set succeeded. False otherwise.</returns>
        public bool SafeSetHeight(int x, int z, int gridHeight)
        {
            var s = new Queue<Point2>();
            var visited = new HashSet<Point2>();
            var setHeights = new Dictionary<Point2, int>();

            var p1 = new Point2(x, z);
            var p2 = new Point2(x, z + 1);
            var p3 = new Point2(x + 1, z);
            var p4 = new Point2(x + 1, z + 1);

            if (_vertexAnchored[p1.x, p1.z] || _vertexAnchored[p2.x, p2.z] || _vertexAnchored[p3.x, p3.z] || _vertexAnchored[p4.x, p4.z])
                return false;

            // bounding box around the changes
            int minX = x;
            int maxX = x + 1;
            int minY = z;
            int maxY = z + 1;

            // initial state with the corners to traverse
            s.Enqueue(p1); s.Enqueue(p2); s.Enqueue(p3); s.Enqueue(p4);
            visited.Add(p1); visited.Add(p2); visited.Add(p3); visited.Add(p4);
            setHeights[p1] = setHeights[p2] = setHeights[p3] = setHeights[p4] = gridHeight;

            // breadth-first search to smooth the terrain around the change
            int visitedCount = 0;
            while (s.Count > 0)
            {
                if (visitedCount++ > 1023)
                    throw new InvalidOperationException(string.Format("Attempting to set height at ({0},{1}) resulted in too many operations!", x, z));

                var cur = s.Dequeue();

                minX = Math.Min(minX, cur.x);
                maxX = Math.Max(maxX, cur.x);
                minY = Math.Min(minY, cur.z);
                maxY = Math.Max(maxY, cur.z);

                for (int i = 0; i < dx.Length; ++i)
                {
                    var test = new Point2(cur.x + dx[i], cur.z + dy[i]);

                    if (visited.Contains(test))
                        continue;

                    // we are okay with grid difference up to 1 step
                    var heightDiff = _terrain.GetVertexHeight(test.x, test.z) - setHeights[cur];

                    if (heightDiff < -1)
                    {
                        if (_vertexAnchored[test.x, test.z])
                            return false;

                        setHeights[test] = setHeights[cur] - 1;
                        s.Enqueue(test);
                        visited.Add(test);
                    }

                    if (heightDiff > 1)
                    {
                        if (_vertexAnchored[test.x, test.z])
                            return false;

                        setHeights[test] = setHeights[cur] + 1;
                        s.Enqueue(test);
                        visited.Add(test);
                    }
                }
            }

            int sizeX = maxX - minX + 1;
            int sizeY = maxY - minY + 1;
            var newGridHeights = new int[sizeX, sizeY];
            for (int i = 0; i < sizeX; ++i)
            {
                for (int j = 0; j < sizeY; ++j)
                {
                    var p = new Point2(minX + i, minY + j);
                    newGridHeights[i, j] = setHeights.ContainsKey(p) ? setHeights[p] : _terrain.GetVertexHeight(minX + i, minY + j);
                }
            }

            _terrain.SetVertexHeights(minX, minY, newGridHeights);
            return true;
        }
    }
}