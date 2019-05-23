using System;
using System.Collections.Generic;

namespace Voxon
{
    /*
     * This class forms the basis for the Voxon Photonics Runtime
     * no features listed here may be removed as they have been publically
     * shared with clients. If a future change to how the DLL operates
     * is required. A new interface should be developed as this functionality
     * should remain.
     */
    abstract public class RuntimePromise
    {
        // Used to provide programs loading runtime a current list of available features
        public HashSet<string> GetFeatures()
        {
            HashSet<string> results = new HashSet<string> {
                "GetFeatures",
                
                // DLL Control
                "Load",
                "Unload",
                "isLoaded",

                // Runtime Control
                "Initialise",
                "Shutdown",
                "isInitialised",

                // Engine Control
                "FrameStart",
                "FrameEnd",

                // Camera Control
                "SetAspectRatio",
                "GetAspectRatio",

                // Draw Calls
                "DrawGuidelines",
                "DrawLetters",
                "DrawBox",
                "DrawTexturedMesh",
                "DrawUntexturedMesh",
                "DrawSphere",
                "DrawVoxel",
                "DrawCube",
                "DrawLine",
                "DrawPolygon",
                "DrawHeightmap",

                // Input Calls
                // Keyboard
                "GetKeyState",
                "GetKey",
                "GetKeyUp",
                "GetKeyDown",

                // Mouse
                "GetMousePosition",
                "GetMouseButton",
                "GetMouseButtonDown",

                // Controller
                "GetButton",
                "GetButtonDown",
                "GetButtonUp",
                "GetAxis",

                // Audio
                "GetVolume",

                // Logging
                "LogToFile",
                "LogToScreen"
            };

            return results;
        }

        #region dll_control
        abstract public void Load();
        abstract public void Unload();

        abstract public bool isLoaded();
        #endregion

        #region runtime_control
        abstract public void Initialise();
        abstract public void Shutdown();

        abstract public bool isInitialised();
        #endregion

        #region engine_control
        abstract public bool FrameStart();
        abstract public void FrameEnd();
        #endregion

        #region camera_control
        abstract public void SetAspectRatio(float aspx, float aspy, float aspz);
        abstract public float[] GetAspectRatio();
        #endregion

        #region draw_calls
        abstract public void DrawGuidelines();
        abstract public void DrawLetters(ref point3d pp, ref point3d pr, ref point3d pd, Int32 col, byte[] text);
        abstract public void DrawBox(ref point3d min, ref point3d max, int fill, int colour);
        abstract public void DrawTexturedMesh(ref tiletype texture, poltex[] vertices, int vertice_count, int[] indices, int indice_count, int flags);
        abstract public void DrawUntexturedMesh(poltex[] vertices, int vertice_count, int[] indices, int indice_count, int flags, int colour);
        abstract public void DrawSphere(ref point3d position, float radius, int issol, int colour);
        abstract public void DrawVoxel(ref point3d position, int col);
        abstract public void DrawCube(ref point3d pp, ref point3d pr, ref point3d pd, ref point3d pf, int flags, Int32 col);
        abstract public void DrawLine(ref point3d min, ref point3d max, int col);
        abstract public void DrawPolygon(pol_t[] pt, int pt_count, Int32 col);
        abstract public void DrawHeightmap(ref tiletype texture, ref point3d pp, ref point3d pr, ref point3d pd, ref point3d pf, Int32 colorkey, int min_height, int flags);
        #endregion

        #region input_calls

        #region keyboard
        abstract public int GetKeyState(int keycode);
        abstract public bool GetKey(int keycode);
        abstract public bool GetKeyUp(int keycode);
        abstract public bool GetKeyDown(int keycode);
        #endregion
        
        #region mouse
        abstract public float[] GetMousePosition();
        abstract public bool GetMouseButton(int button);
        abstract public bool GetMouseButtonDown(int button);
        #endregion
        
        #region controller
        abstract public bool GetButton(int button, int player);
        abstract public bool GetButtonDown(int button, int player);
        abstract public bool GetButtonUp(int button, int player);
        abstract public float GetAxis(int axis, int player);
        #endregion
        #endregion

        #region audio
        abstract public float GetVolume();
        #endregion

        #region logging
        abstract public void LogToFile(string msg);
        abstract public void LogToScreen(int x, int y, string Text);
        #endregion
    }
}
