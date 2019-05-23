using System;
using System.Runtime.InteropServices;

namespace Voxon
{
    #region public_structures
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct point3d
    {
        public float x, y, z;

        public float[] GetPosition()
        {
            return new float[] { x, y, z };
        }

        public point3d(float[] pos)
        {
            x = pos[0];
            y = pos[1];
            z = pos[2];
        }

        public point3d(poltex pos)
        {
            x = pos.x;
            y = pos.y;
            z = pos.z;
        }

        public override string ToString()
        {
            return "(" + x + "," + y + "," + z + ")";
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct pol_t
    {
        public float x, y, z;
        public int p2;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct poltex
    {
        public float x, y, z, u, v;
        public int col;

        override public string ToString()
        {
            return "(" + x + ", " + y + ", " + z + ")-(" + u + ", " + v + ") : " + col;
        }

        public float[] GetPosition()
        {
            return new float[] { x, y, z };
        }

        public poltex(point3d pos)
        {
            x = pos.x;
            y = pos.y;
            z = pos.z;
            u = 0;
            v = 0;
            col = 0;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct tiletype
    {
        public IntPtr first_pixel;          // pointer to first pixel of the texture (usually the top-left corner)
        public IntPtr pitch;                   // pitch, or number of bytes per horizontal line (usually x*4)
        public IntPtr height, width;          // width & height of texture
    }
    #endregion
}
