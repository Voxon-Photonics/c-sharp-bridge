using System;
using System.Collections.Generic;

namespace Voxon
{
    interface IRuntime
    {
        HashSet<string> GetFeatures();
        void Load();
        void Initialise();
        void Unload();

        bool isLoaded();
        bool isInitialised();

        void Shutdown();
        bool FrameStart();
        void FrameEnd();

        void SetAspectRatio(float aspx, float aspy, float aspz);
        float[] GetAspectRatio();

        void DrawGuidelines();
    }
}
