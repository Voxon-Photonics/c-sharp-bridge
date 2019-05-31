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
     *
    public interface IRuntimePromise
    {
		// Used to provide programs loading runtime a current list of available features
		HashSet<string> GetFeatures();

        #region dll_control
        void Load();
        void Unload();

        bool isLoaded();
        #endregion

        #region runtime_control
        void Initialise();
        void Shutdown();

        bool isInitialised();
        #endregion

        #region engine_control
        bool FrameStart();
        void FrameEnd();
        #endregion

        #region camera_control
        void SetAspectRatio(float aspx, float aspy, float aspz);
        float[] GetAspectRatio();
        #endregion

        #region draw_calls
        void DrawGuidelines();
        void DrawLetters(ref point3d pp, ref point3d pr, ref point3d pd, Int32 col, byte[] text);
        void DrawBox(ref point3d min, ref point3d max, int fill, int colour);
        void DrawTexturedMesh(ref tiletype texture, poltex[] vertices, int vertice_count, int[] indices, int indice_count, int flags);
        void DrawUntexturedMesh(poltex[] vertices, int vertice_count, int[] indices, int indice_count, int flags, int colour);
        void DrawSphere(ref point3d position, float radius, int issol, int colour);
        void DrawVoxel(ref point3d position, int col);
        void DrawCube(ref point3d pp, ref point3d pr, ref point3d pd, ref point3d pf, int flags, Int32 col);
        void DrawLine(ref point3d min, ref point3d max, int col);
        void DrawPolygon(pol_t[] pt, int pt_count, Int32 col);
        void DrawHeightmap(ref tiletype texture, ref point3d pp, ref point3d pr, ref point3d pd, ref point3d pf, Int32 colorkey, int min_height, int flags);
        #endregion

        #region input_calls

        #region keyboard
        int GetKeyState(int keycode);
        bool GetKey(int keycode);
        bool GetKeyUp(int keycode);
        bool GetKeyDown(int keycode);
        #endregion
        
        #region mouse
        float[] GetMousePosition();
        bool GetMouseButton(int button);
        bool GetMouseButtonDown(int button);
        #endregion
        
        #region controller
        bool GetButton(int button, int player);
        bool GetButtonDown(int button, int player);
        bool GetButtonUp(int button, int player);
        float GetAxis(int axis, int player);
        #endregion
        #endregion

        #region audio
        float GetVolume();
        #endregion

        #region logging
        void LogToFile(string msg);
        void LogToScreen(int x, int y, string Text);
        #endregion
    }
	*/
}
