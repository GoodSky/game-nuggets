﻿using Campus.GridTerrain;
using Common;
using UnityEngine;

namespace Campus
{
    /// <summary>
    /// A group of mesh cursors.
    /// Displays things like building footprints.
    /// </summary>
    public class FootprintCursor
    {
        public Point3 Location { get; private set; }

        private GridMesh _terrain;
        private Material _validMaterial;
        private Material _invalidMaterial;
        private GridCursor[,] _cursors;

        /// <summary>
        /// Instantiates a footprint cursor.
        /// </summary>
        /// <param name="terrain">The terrain to put cursors on.</param>
        /// <param name="validMaterial">The material to use when a footprint location is valid.</param>
        /// <param name="invalidMaterial">The material to use when a footprint location is invalid.</param>
        public FootprintCursor(GridMesh terrain, Material validMaterial, Material invalidMaterial)
        {
            _terrain = terrain;
            _validMaterial = validMaterial;
            _invalidMaterial = invalidMaterial;
        }

        /// <summary>
        /// Creates a new collection of cursors.
        /// </summary>
        /// <param name="footprint">The footprint of the building.</param>
        public void Create(bool[,] footprint)
        {
            if (_cursors != null)
                GameLogger.FatalError("Can't recreate a footprint cursor while one already exists!");

            int xSize = footprint.GetLength(0);
            int zSize = footprint.GetLength(1);

            _cursors = new GridCursor[xSize, zSize];
            for (int x = 0; x < xSize; ++x)
            {
                for (int z = 0; z < zSize; ++z)
                {
                    if (footprint[x, z])
                    {
                        _cursors[x, z] = GridCursor.Create(_terrain, _validMaterial);
                        _cursors[x, z].Activate();
                    }
                    else
                    {
                        _cursors[x, z] = null;
                    }
                }
            }
        }

        /// <summary>
        /// Place the cursors at a location.
        /// </summary>
        public void Place(Point3 location, BuildingRotation rotation)
        {
            Location = location;

            (Point3 cursorOrigin, GridCursor[,] cursors) = BuildingRotationUtils.RotateGridCursors(_cursors, location, rotation);
            int xSize = cursors.GetLength(0);
            int zSize = cursors.GetLength(1);

            for (int x = 0; x < xSize; ++x)
            {
                for (int z = 0; z < zSize; ++z)
                {
                    if (cursors[x, z] != null)
                    {
                        int cursorX = cursorOrigin.x + x;
                        int cursorZ = cursorOrigin.z + z;

                        if (_terrain.GridInBounds(cursorX, cursorZ))
                        {
                            cursors[x, z].Activate();
                            cursors[x, z].Place(new Point2(cursorX, cursorZ));
                        }
                        else
                        {
                            cursors[x, z].Deactivate();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Set the color of the cursors based on valid locations.
        /// </summary>
        /// <param name="validLocations">An array representing which locations are valid.</param>
        /// <param name="rotation">The current rotation of the footprint.</param>
        public void SetMaterials(bool[,] validLocations, BuildingRotation rotation)
        {
            (Point3 cursorOrigin, GridCursor[,] cursors) = BuildingRotationUtils.RotateGridCursors(_cursors, Location, rotation);
            int xSize = cursors.GetLength(0);
            int zSize = cursors.GetLength(1);

            for (int x = 0; x < xSize; ++x)
            {
                for (int z = 0; z < zSize; ++z)
                {
                    if (cursors[x, z] != null)
                    {
                        cursors[x, z].SetMaterial(validLocations[x, z] ? _validMaterial : _invalidMaterial);
                    }
                }
            }
        }

        /// <summary>
        /// Destroy the cursor game objects.
        /// </summary>
        public void Destroy()
        {
            if (_cursors != null)
            {
                foreach (var cursor in _cursors)
                {
                    if (cursor != null)
                    {
                        cursor.Deactivate();
                        UnityEngine.Object.Destroy(cursor);
                    }
                }

                _cursors = null;
            }
        }

        /// <summary>
        /// Activate all cursors.
        /// </summary>
        public void Activate()
        {
            if (_cursors != null)
            {
                foreach (var cursor in _cursors)
                {
                    if (cursor != null)
                    {
                        cursor.Activate();
                    }
                }
            }
        }

        /// <summary>
        /// Deactivate all cursors.
        /// </summary>
        public void Deactivate()
        {
            if (_cursors != null)
            {
                foreach (var cursor in _cursors)
                {
                    if (cursor != null)
                    {
                        cursor.Deactivate();
                    }
                }
            }
        }
    }
}
