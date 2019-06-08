﻿using System;
using System.Collections.Generic;

namespace Common
{
    public enum AxisAlignment
    {
        None,
        XAxis,
        ZAxis,
    }

    /// <summary>
    /// A line that is parallel with either the x or z axis.
    /// Used for constructing things in straight lines.
    /// </summary>
    public struct AxisAlignedLine
    {
        public Point2 Start { get; private set; }
        public Point2 End { get; private set; }
        public int Length { get; private set; }
        public AxisAlignment Alignment { get; private set; }

        public AxisAlignedLine(Point2 start)
        {
            Start = End = start;
            Length = 1;
            Alignment = AxisAlignment.None;
        }

        public AxisAlignedLine(Point3 start)
            : this(new Point2(start.x, start.z))
        {
        }

        public void UpdateEndPointAlongAxis(Point2 newEnd)
        {
            int lengthX = Math.Abs(Start.x - newEnd.x);
            int lengthZ = Math.Abs(Start.z - newEnd.z);

            if (lengthX == 0 && lengthZ == 0)
            {
                End = newEnd;
                Length = 1;
                Alignment = AxisAlignment.None;
            }
            else if (lengthX > lengthZ)
            {
                End = new Point2(newEnd.x, Start.z);
                Length = lengthX + 1;
                Alignment = AxisAlignment.XAxis;
            }
            else
            {
                End = new Point2(Start.x, newEnd.z);
                Length = lengthZ + 1;
                Alignment = AxisAlignment.ZAxis;
            }
        }

        public void UpdateEndPointAlongAxis(Point3 newEnd)
        {
            UpdateEndPointAlongAxis(new Point2(newEnd.x, newEnd.z));
        }

        public IEnumerable<(int index, Point2 point)> PointsAlongLine()
        {
            int dx = 0;
            int dz = 0;

            switch (Alignment)
            {
                case AxisAlignment.None:
                    // Case: Line is a single square
                    break;

                case AxisAlignment.XAxis:
                    // Case: Line is along the x-axis
                    dx = Start.x < End.x ? 1 : -1;
                    break;

                case AxisAlignment.ZAxis:
                    // Case: Line is along the z-axis
                    dz = Start.z < End.z ? 1 : -1;
                    break;
            }

            for (int i = 0; i < Length; ++i)
            {
                yield return (i, new Point2(Start.x + i * dx, Start.z + i * dz));
            }
        }
    }
}
