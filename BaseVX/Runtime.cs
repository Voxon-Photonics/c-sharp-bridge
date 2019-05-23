using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Voxon
{
    public class Runtime : RuntimePromise
    {
        #region magic_numbers
        const int MAXCONTROLLERS = 4;
        const int TEXTURE_BACK_COLOUR = 0x3F3F3F;
        private System.Text.Encoding enc = System.Text.Encoding.ASCII;
        #endregion

        #region private_structures
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct voxie_xbox_t
        {
            public short but;       //XBox controller buttons (same layout as XInput)
            public short lt, rt;    //XBox controller left&right triggers (0..255)
            public short tx0, ty0;  //XBox controller left  joypad (-32768..32767)
            public short tx1, ty1;  //XBox controller right joypad (-32768..32767)
            public short hat;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct point2d
        {
            public float x, y;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct voxie_disp_t
        {
            //settings for quadrilateral (keystone) compensation (use voxiedemo mode 2 to calibrate)
            public point2d keyst0, keyst1, keyst2, keyst3, keyst4, keyst5, keyst6, keyst7;
            public int colo_r, colo_g, colo_b; //initial values at startup for rgb color mode
            public int mono_r, mono_g, mono_b; //initial values at startup for mono mode
            public int mirrorx, mirrory;       //projector hardware flipping (I suggest avoiding this)
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct voxie_wind_t
        {
            //Emulation
            public int useemu;             //0=Voxiebox, 1=emulation in 2D window (representative of actual view on HW), 2=emulation in 2D window (made for 2D display)
            public float emuhang;          //emulator horizontal angle (radians)
            public float emuvang;          //emulator vertical   angle (radians)
            public float emudist;          //emulator distance; minimum is 2000.0.

            //Display
            public int xdim, ydim;         //projector dimensions (912,1140)
            public int projrate;           //projector rate in Hz {60..107}
            public int framepervol;        //# projector frames per volume {1..16}. Ex:framepervol=3 at projrate=60
                                           //gives 60/3 = 20Hz volume rate
            public int usecol;             //0=mono white, 1=full color time multiplexed,
                                           //-1=red, -2=green, -3=yellow, -4=blue, -5=magenta, -6=cyan
            public int dispnum;            //number of displays to search for and use (typically 1)
            public int HighLumenDangerProtect; //if enabled (nonzero), protects ledr/g/b from going too high
            public voxie_disp_t disp0, disp1, disp2; //see voxie_disp_t

            //Actuator
            public int hwsync_frame0;      //first frame offset (-1 to disable sync hw)
            public int hwsync_phase;       //high precision phase offset
            public int hwsync_amp0, hwsync_amp1, hwsync_amp2, hwsync_amp3;      //amplitude {0..65536} for each channel
            public int hwsync_pha0, hwsync_pha1, hwsync_pha2, hwsync_pha3;      //phase {0..65536} for each channel
            public int hwsync_levthresh;   //threshold ADC value for peak detection {0..1024, but typically in range: 256..512}
            public int voxie_vol;          //amplitude scale when using sine wave audio output {0..100}

            //Render
            public int ilacemode;
            public int drawstroke;         //1=draw on up stroke, 2=draw on down stroke, 3=draw on both up&down
            public int dither;             //0=dither off, 1=dither mono only, 2=dither RGB only, 3=dither both
            public int smear;              //1+=increase brightness in x-y at post processing (slower render), 0=not
            public int usekeystone;        //0=no keystone compensation (for testing only), 1=keystone quadrilateral compensation (default) - see proj[]
            public int flip;               //flip coordinate system in voxiebox.dll
            public int menu_on_voxie;      //1=display menu on voxiebox view

            public float aspx, aspy, aspz; //aspect ratio, loaded from voxiebox.ini, used by application. Values typically around 1.f
            public float gamma;            //gamma value for interpolating in voxie_drawspr()/voxie_drawheimap().
            public float density;          //scale factor controlling dot density of STL model rendering (default:1.f)

            //Audio
            public int sndfx_vol;          //amplitude scale of sound effects {0..100}
            public int voxie_aud;          //audio channel index of motor. (Playback devices..Speakers..Configure..
                                           //   Audio Channels)
            public int excl_audio;         //1=exclusive audio mode (much faster & more stable sync - recommended!
                                           //0=shared audio mode (if audio access to other programs required)
            public int sndfx_aud0, sndfx_aud1;       //audio channel indices of sound effects, [0]=left, [1]=right
            public int playsamprate;       //sample rate used by audio driver (written by voxie_init()). 0 is written
                                           //   if no audio channels are enabled in voxiebox.ini.
            public int playnchans;         //number of audio channels expected to be rendered by user audio mixer
            public int recsamprate;        //recording sample rate - to use, must write before voxie_init()
            public int recnchans;          //number of audio channels in recording callback

            //Misc.
            public int isrecording;        //0=normally, 1 when .REC file recorder is in progress (written by DLL)
            public int excl_mouse;         //1=exclusive mouse, 0=not
            public int dispcur;            //current display selected in menus {0..dispnum-1}

            //Obsolete
            public double freq;            //starting value in Hz (must be set before first call to voxie_init()); obsolete - not used by current hardware
            public double phase;           //phase lock {0.0..1.0} (can be updated on later calls to voxie_init()); obsolete - not used by current hardware

			//Helix
			public int reserved2;
			public int motortyp; //0=Old DC brush motor, 1=ClearPath using Frequency Input, 2=AU brushless airplane motor
			public int clipshape; //0=rectangle (vw.aspx,vw.aspy), 1=circle (vw.aspr)
			public int goalrpm, cpmaxrpm, ianghak0, ianghak1, ianghak2;
			public int upndow; //0=sawtooth, 1=triangle
			public int nblades; //0=VX1 (not spinner), 1=/|, 2=/|/|, ..
			public int reserved3_0, reserved3_1, reserved3_2, reserved3_3;
			public float sync_usb_offset;
			public int reserved4_0, reserved4_1, reserved4_2, reserved4_3, reserved4_4, reserved4_5;
			public float aspr, sawtoothrat;
		}

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct voxie_frame_t
        {
            public IntPtr f;              //Pointer to top-left-up of current frame to draw
            public IntPtr p;              //Number of bytes per horizontal line (x)
            public IntPtr fp;             //Number of bytes per 24-plane frame (1/3 of screen)
            public int x, y;               //Width and height of viewport
            public int usecol;             //Tells whether color mode is selected
            public int drawplanes;         //Tells how many planes to draw
            public int x0, y0, x1, y1;     //Viewport extents
            public float xmul, ymul, zmul; //Transform for medium and high level graphics functions..
            public float xadd, yadd, zadd; //Transform is: actual_x = passed_x * xmul + xadd
            public tiletype f2d;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct voxie_inputs_t
        {
            public int bstat, obstat, dmousx, dmousy, dmousz;
        }

        struct tri_t
        {
            public float x0, y0, z0;
            public int n0;
            public float x1, y1, z1;
            public int n1;
            public float x2, y2, z2;
            public int n2;
        }

        // Controls
        private struct xbox_input
        {
            public voxie_xbox_t input;
            public voxie_xbox_t offset;
            public voxie_xbox_t last_frame;
        }

        #endregion

        #region delegate_functions
        delegate void voxie_load_d(ref voxie_wind_t vw);

        delegate void voxie_loadini_int_d(ref voxie_wind_t vw);

        delegate void voxie_getvw_d(ref voxie_wind_t vw);

        delegate int voxie_init_d(ref voxie_wind_t vw);

        delegate void voxie_uninit_int_d(int id);

        delegate int voxie_breath_d(ref voxie_inputs_t ins);

        delegate void voxie_quitloop_d();

        delegate double voxie_klock_d();

        delegate int voxie_keystat_d(int i);

        delegate int voxie_keyread_d();

        delegate void voxie_doscreencap_d();

        delegate void voxie_setview_d(ref voxie_frame_t vf, float x0, float y0, float z0, float x1, float y1, float z1);

        delegate int voxie_frame_start_d(ref voxie_frame_t vf);

        delegate void voxie_frame_end_d();

        delegate void voxie_setleds_d(int r, int g, int b);

        delegate void voxie_drawvox_d(ref voxie_frame_t vf, float fx, float fy, float fz, int col);

        delegate void voxie_drawbox_d(ref voxie_frame_t vf, float x0, float y0, float z0, float x1, float y1,
            float z1, int fillmode, int col);

        delegate void voxie_drawlin_d(ref voxie_frame_t vf, float x0, float y0, float z0, float x1, float y1, float z1, int col);

        delegate void voxie_drawpol_d(ref voxie_frame_t vf, pol_t[] pt, int n, int col);

        delegate void voxie_drawmesh_d(ref voxie_frame_t vf, poltex[] vt, int vtn, int[] mesh, int meshn, int fillmode, int col);

        delegate void voxie_drawmeshtex_d(ref voxie_frame_t vf, ref tiletype texture, poltex[] vt, int vtn, int[] mesh, int meshn,
                int flags, int col);

        delegate void voxie_drawmeshtex_null_d(ref voxie_frame_t vf, int nullptr, poltex[] vt, int vtn, int[] mesh, int meshn,
                int flags, int col);

        delegate void voxie_drawsph_d(ref voxie_frame_t vf, float fx, float fy, float fz, float rad,
                int issol, int col);

        delegate void voxie_drawcone_d(ref voxie_frame_t vf, float x0, float y0, float z0, float r0, float x1,
            float y1, float z1, float r1, int issol, int col);

        delegate int voxie_drawspr_d(ref voxie_frame_t vf, [MarshalAs(UnmanagedType.LPStr)] string st,
            ref point3d p, ref point3d r, ref point3d d, ref point3d f, int col);

        delegate void voxie_printalph_d(ref voxie_frame_t vf, ref point3d p, ref point3d r, ref point3d d,
            int col, byte[] st);

        delegate void voxie_drawcube_d(ref voxie_frame_t vf, ref point3d p, ref point3d r, ref point3d d,
            ref point3d f, int fillmode, int col);

        delegate float voxie_drawheimap_d(ref voxie_frame_t vf, ref tiletype texture,
            ref point3d p, ref point3d r, ref point3d d, ref point3d f, int colorkey, int heimin, int flags);

        delegate void voxie_playsound_d([MarshalAs(UnmanagedType.LPStr)] string st, int chan, int volperc0,
            int volperc1, float frqmul);

        delegate int voxie_xbox_read_d(int id, ref voxie_xbox_t vx);

        delegate void voxie_xbox_write_d(int id, float lmot, float rmot);

        delegate void voxie_debug_print6x8_d     (int x, int y, int fcol, int bcol, byte[] fmt);
        delegate void voxie_debug_drawpix_d      (int x, int y, int col);
        delegate void voxie_debug_drawhlin_d     (int x0, int x1, int y, int col);
        delegate void voxie_debug_drawline_d     (float x0, float y0, float x1, float y1, int col);
        delegate void voxie_debug_drawcirc_d     (int xc, int yc, int r, int col);
        delegate void voxie_debug_drawrectfill_d (int x0, int y0, int x1, int y1, int col);
        delegate void voxie_debug_drawcircfill_d (int x, int y, int r, int col);

        #endregion

        #region DLL_imports
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr LoadLibrary(string libname);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        static private extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);
        #endregion

        #region delegate_instances
        // Operation
        voxie_loadini_int_d voxie_loadini_int;
        voxie_init_d voxie_init;
        voxie_uninit_int_d voxie_uninit_int;
        voxie_breath_d voxie_breath;
        voxie_getvw_d voxie_getvw;
        voxie_quitloop_d voxie_quitloop;
        voxie_klock_d voxie_klock;
        voxie_keystat_d voxie_keystat;
        voxie_keyread_d voxie_keyread;
        voxie_doscreencap_d voxie_doscreencap;
        voxie_setview_d voxie_setview;
        voxie_frame_start_d voxie_frame_start;
        voxie_frame_end_d voxie_frame_end;
        voxie_setleds_d voxie_setleds;
        voxie_drawvox_d voxie_drawvox;
        voxie_drawbox_d voxie_drawbox;
        voxie_drawlin_d voxie_drawlin;
        voxie_drawpol_d voxie_drawpol;
        voxie_drawmeshtex_d voxie_drawmeshtex;
        voxie_drawmeshtex_null_d voxie_drawmeshtex_null;
        voxie_drawsph_d voxie_drawsph;
        voxie_drawcone_d voxie_drawcone;
        voxie_drawspr_d voxie_drawspr;
        voxie_printalph_d voxie_printalph;
        voxie_drawcube_d voxie_drawcube;
        voxie_drawheimap_d voxie_drawheimap;
        voxie_playsound_d voxie_playsound;
        voxie_xbox_read_d voxie_xbox_read;
        voxie_xbox_write_d voxie_xbox_write;

        // Debug Functions
        voxie_debug_print6x8_d voxie_debug_print6x8;
        voxie_debug_drawpix_d voxie_debug_drawpix;
        voxie_debug_drawhlin_d voxie_debug_drawhlin;
        voxie_debug_drawline_d voxie_debug_drawline;
        voxie_debug_drawcirc_d voxie_debug_drawcirc;
        voxie_debug_drawrectfill_d voxie_debug_drawrectfill;
        voxie_debug_drawcircfill_d voxie_debug_drawcircfill;

        #endregion

        #region runtime_dll
        string dll = "";
        IntPtr Handle = IntPtr.Zero;
        #endregion

        #region runtime_values
        private voxie_wind_t vw;
        private voxie_frame_t vf;
        private voxie_inputs_t ins;
        private xbox_input[] controllers = new xbox_input[MAXCONTROLLERS];
        private bool b_initialised = false;
        #endregion

        #region runtime_functions
        void setup_delegates(IntPtr Handle)
        {
            if (!isLoaded()) return;

            IntPtr funcaddr = GetProcAddress(Handle, "voxie_loadini_int");
            voxie_loadini_int = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(voxie_loadini_int_d)) as voxie_loadini_int_d;

            funcaddr = GetProcAddress(Handle, "voxie_init");
            voxie_init = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(voxie_init_d)) as voxie_init_d;

            funcaddr = GetProcAddress(Handle, "voxie_uninit_int");
            voxie_uninit_int = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(voxie_uninit_int_d)) as voxie_uninit_int_d;

            funcaddr = GetProcAddress(Handle, "voxie_breath");
            voxie_breath = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(voxie_breath_d)) as voxie_breath_d;

            funcaddr = GetProcAddress(Handle, "voxie_getvw");
            voxie_getvw = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(voxie_getvw_d)) as voxie_getvw_d;

            funcaddr = GetProcAddress(Handle, "voxie_quitloop");
            voxie_quitloop = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(voxie_quitloop_d)) as voxie_quitloop_d;

            funcaddr = GetProcAddress(Handle, "voxie_klock");
            voxie_klock = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(voxie_klock_d)) as voxie_klock_d;

            funcaddr = GetProcAddress(Handle, "voxie_keystat");
            voxie_keystat = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(voxie_keystat_d)) as voxie_keystat_d;

            funcaddr = GetProcAddress(Handle, "voxie_keyread");
            voxie_keyread = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(voxie_keyread_d)) as voxie_keyread_d;

            funcaddr = GetProcAddress(Handle, "voxie_doscreencap");
            voxie_doscreencap = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(voxie_doscreencap_d)) as voxie_doscreencap_d;

            funcaddr = GetProcAddress(Handle, "voxie_setview");
            voxie_setview = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(voxie_setview_d)) as voxie_setview_d;

            funcaddr = GetProcAddress(Handle, "voxie_frame_start");
            voxie_frame_start = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(voxie_frame_start_d)) as voxie_frame_start_d;

            funcaddr = GetProcAddress(Handle, "voxie_frame_end");
            voxie_frame_end = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(voxie_frame_end_d)) as voxie_frame_end_d;

            funcaddr = GetProcAddress(Handle, "voxie_setleds");
            voxie_setleds = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(voxie_setleds_d)) as voxie_setleds_d;

            funcaddr = GetProcAddress(Handle, "voxie_drawvox");
            voxie_drawvox = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(voxie_drawvox_d)) as voxie_drawvox_d;

            funcaddr = GetProcAddress(Handle, "voxie_xbox_read");
            voxie_xbox_read = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(voxie_xbox_read_d)) as voxie_xbox_read_d;

            funcaddr = GetProcAddress(Handle, "voxie_xbox_write");
            voxie_xbox_write = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(voxie_xbox_write_d)) as voxie_xbox_write_d;

            funcaddr = GetProcAddress(Handle, "voxie_drawbox");
            voxie_drawbox = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(voxie_drawbox_d)) as voxie_drawbox_d;

            funcaddr = GetProcAddress(Handle, "voxie_drawlin");
            voxie_drawlin = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(voxie_drawlin_d)) as voxie_drawlin_d;

            funcaddr = GetProcAddress(Handle, "voxie_drawpol");
            voxie_drawpol = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(voxie_drawpol_d)) as voxie_drawpol_d;

            funcaddr = GetProcAddress(Handle, "voxie_drawmeshtex");
            voxie_drawmeshtex = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(voxie_drawmeshtex_d)) as voxie_drawmeshtex_d;
            voxie_drawmeshtex_null = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(voxie_drawmeshtex_null_d)) as voxie_drawmeshtex_null_d;

            funcaddr = GetProcAddress(Handle, "voxie_drawsph");
            voxie_drawsph = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(voxie_drawsph_d)) as voxie_drawsph_d;

            funcaddr = GetProcAddress(Handle, "voxie_drawcone");
            voxie_drawcone = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(voxie_drawcone_d)) as voxie_drawcone_d;

            funcaddr = GetProcAddress(Handle, "voxie_drawspr");
            voxie_drawspr = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(voxie_drawspr_d)) as voxie_drawspr_d;

            funcaddr = GetProcAddress(Handle, "voxie_printalph");
            voxie_printalph = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(voxie_printalph_d)) as voxie_printalph_d;

            funcaddr = GetProcAddress(Handle, "voxie_drawcube");
            voxie_drawcube = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(voxie_drawcube_d)) as voxie_drawcube_d;

            funcaddr = GetProcAddress(Handle, "voxie_drawheimap");
            voxie_drawheimap = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(voxie_drawheimap_d)) as voxie_drawheimap_d;

            funcaddr = GetProcAddress(Handle, "voxie_playsound");
            voxie_playsound = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(voxie_playsound_d)) as voxie_playsound_d;

            funcaddr = GetProcAddress(Handle, "voxie_debug_print6x8");
            voxie_debug_print6x8 = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(voxie_debug_print6x8_d)) as voxie_debug_print6x8_d;

            funcaddr = GetProcAddress(Handle, "voxie_debug_drawpix");
            voxie_debug_drawpix = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(voxie_debug_drawpix_d)) as voxie_debug_drawpix_d;

            funcaddr = GetProcAddress(Handle, "voxie_debug_drawhlin");
            voxie_debug_drawhlin = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(voxie_debug_drawhlin_d)) as voxie_debug_drawhlin_d;

            funcaddr = GetProcAddress(Handle, "voxie_debug_drawline");
            voxie_debug_drawline = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(voxie_debug_drawline_d)) as voxie_debug_drawline_d;

            funcaddr = GetProcAddress(Handle, "voxie_debug_drawcirc");
            voxie_debug_drawcirc = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(voxie_debug_drawcirc_d)) as voxie_debug_drawcirc_d;

            funcaddr = GetProcAddress(Handle, "voxie_debug_drawrectfill");
            voxie_debug_drawrectfill = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(voxie_debug_drawrectfill_d)) as voxie_debug_drawrectfill_d;

            funcaddr = GetProcAddress(Handle, "voxie_debug_drawcircfill");
            voxie_debug_drawcircfill = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(voxie_debug_drawcircfill_d)) as voxie_debug_drawcircfill_d;
        }
        #endregion

        #region public_functions
        #region dll_control

        public override void Load()
        {
            try
            {
                if (!isLoaded())
                {
                    if (dll == "")
                    {
                        var _dll = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Voxon\\Voxon");
                        if (_dll != null)
                        {
                            dll = (string)Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Voxon\\Voxon").GetValue("Path") + "voxiebox.dll";
                        }


                        if (dll == "")
                        {
                            string[] paths = Environment.GetEnvironmentVariable("Path").Split(';');

                            foreach (var path in paths)
                            {
                                if (File.Exists(path + "\\voxiebox.dll"))
                                {
                                    dll = path + "\\voxiebox.dll";
                                }
                            }
                        }

                        if (dll == "")
                        {
                            if (File.Exists("voxiebox.dll"))
                            {
                                dll = "voxiebox.dll";
                            }
                            else
                            {
                                LogToFile("DLL not found");
                            }
                        }
                    }
                    //Load DLL
                    Handle = LoadLibrary(dll);
                    if (Handle == IntPtr.Zero)
                    {
                        int errorCode = Marshal.GetLastWin32Error();
                        throw new Exception(string.Format("Failed to load library (ErrorCode: {0}). Path = {1}", errorCode, dll));
                    }
                }
                else
                {
                    LogToFile("VX.dll already loaded");
                }
            }
            catch (Exception e)
            {
                LogToFile(e.Message);
            }


            // Set up function delegates
            setup_delegates(Handle);
        }

        public override void Initialise()
        {
            if (!isActive())
            {
                // Refresh components
                vw = new voxie_wind_t();
                vf = new voxie_frame_t();
                ins = new voxie_inputs_t();

                // Initialise Box
                voxie_loadini_int(ref vw);


                voxie_init(ref vw);

                // Set Up Controllers (now the box is running)
                load_xbox_controllers();

                b_initialised = true;
            }
        }

        public override void Unload()
        {
            if (isLoaded())
            {
                try
                {
                    if (isInitialised())
                    {
                        Shutdown();
                    }

                    while (FreeLibrary(Handle))
                    { }

                    Handle = IntPtr.Zero;
                }
                catch (Exception e)
                {
                    LogToFile(e.Message);
                }
            }
        }

        #endregion

        #region dll_status
        public override bool isLoaded()
        {
            return (Handle != IntPtr.Zero);
        }

        public override bool isInitialised()
        {
            return b_initialised;
        }

        bool isActive()
        {
            return (Handle != IntPtr.Zero) && b_initialised;
        }
        #endregion

        #region device_calls
        public override void Shutdown()
        {
            if (!isActive()) return;

            try
            {
                voxie_quitloop();
                voxie_frame_end();
                voxie_getvw(ref vw);
                voxie_breath(ref ins);
                voxie_uninit_int(0);
                b_initialised = false;

                // Clear old references
                vw = new voxie_wind_t();
                vf = new voxie_frame_t();
                ins = new voxie_inputs_t();
            }
            catch (Exception e)
            {
                LogToFile(e.Message);
            }
        }

        public override bool FrameStart()
        {
            if (!isActive()) return false;

            bool has_breath = voxie_breath(ref ins) == 0;

            voxie_frame_start(ref vf);

            voxie_setview(ref vf, -vw.aspx, -vw.aspy, -vw.aspz, vw.aspx, vw.aspy, vw.aspz);

            update_xbox_controllers();

            return has_breath;
        }

        public override void FrameEnd()
        {
            if (!isActive()) return;

            voxie_frame_end();

            voxie_getvw(ref vw);
        }
        #endregion

        #region camera_controls
        // TODO: Spinning Not Currently Used
        private void set_is_circular(bool is_circular)
        {
            if (!isActive()) return;

            if(is_circular)
            {
                vw.clipshape = 1;
                voxie_init(ref vw);
            } else
            {
                vw.clipshape = 0;
                voxie_init(ref vw);
            }
            
        }

        public override void SetAspectRatio(float aspx, float aspy, float aspz)
        {
            if (aspx <= 0 || aspy <= 0 || aspz <= 0)
                return;

            vw.aspx = aspx;
            vw.aspy = aspy;
            vw.aspz = aspz;
        }

        public override float[] GetAspectRatio()
        {
            float[] asp = new float[3];
            // Uninitialised
            if (vw.aspx <= 0 || vw.aspy <= 0 || vw.aspz <= 0)
            {
                asp[0] = 1.0f;
                asp[0] = 0.444f;
                asp[0] = 1.0f;
            }
            else
            {
                asp[0] = vw.aspx;
                asp[1] = vw.aspy;
                asp[2] = vw.aspz;
            }

            return asp;
        }
        #endregion

        #region drawables
        public override void DrawGuidelines()
        {
            if (!isActive()) return;
            voxie_drawbox(ref vf, -vw.aspx + 1e-3f, -vw.aspy + 1e-3f, -vw.aspz, +vw.aspx - 1e-3f, +vw.aspy - 1e-3f, +vw.aspz, 1, 0xffffff);
        }

        public override void DrawLetters(ref point3d pp, ref point3d pr, ref point3d pd, Int32 col, byte[] text)
        {
            if (!isActive()) return;
            voxie_printalph(ref vf, ref pp, ref pr, ref pd, col, text);
        }

        public override void DrawBox(ref point3d min, ref point3d max, int fill, int colour)
        {
            if (!isActive()) return;
            voxie_drawbox(ref vf, min.x, min.y, min.z, max.x, max.y, max.z, fill, colour);
        }

        public override void DrawTexturedMesh(ref tiletype texture, poltex[] vertices, int vertice_count, int[] indices, int indice_count, int flags)
        {
            if (!isActive()) return;
            voxie_drawmeshtex(ref vf, ref texture, vertices, vertice_count, indices, indice_count, flags, TEXTURE_BACK_COLOUR);
        }

        public override void DrawUntexturedMesh(poltex[] vertices, int vertice_count, int[] indices, int indice_count, int flags, int colour)
        {
            if (!isActive()) return;
            voxie_drawmeshtex_null(ref vf, 0, vertices, vertice_count, indices, indice_count, flags, colour);
        }

        public override void DrawSphere(ref point3d position, float radius, int issol, int colour)
        {
            if (!isActive()) return;
            voxie_drawsph(ref vf, position.x, position.y, position.z, radius, issol, colour);
        }

        public override void DrawVoxel(ref point3d position, int col)
        {
            if (!isActive()) return;
            voxie_drawvox(ref vf, position.x, position.y, position.z, col);
        }

        public override void DrawCube(ref point3d pp, ref point3d pr, ref point3d pd, ref point3d pf, int flags, Int32 col)
        {
            if (!isActive()) return;
            voxie_drawcube(ref vf, ref pp, ref pr, ref pd, ref pf, flags, col);
        }

        public override void DrawLine(ref point3d min, ref point3d max, int col)
        {
            if (!isActive()) return;
            voxie_drawlin(ref vf, min.x, min.y, min.z, max.x, max.y, max.z, col);
        }

        public override void DrawPolygon(pol_t[] pt, int pt_count, Int32 col)
        {
            if (!isActive()) return;
            voxie_drawpol(ref vf, pt, pt_count, col);
        }

        public override void DrawHeightmap(ref tiletype texture, ref point3d pp, ref point3d pr, ref point3d pd, ref point3d pf, Int32 colorkey, int min_height, int flags)
        {
            if (!isActive()) return;
            voxie_drawheimap(ref vf, ref texture , ref pp, ref pr, ref pd, ref pf, colorkey, min_height, flags);
        }
        #endregion

        #region input

        public override int GetKeyState(int i)
        {
            if (!isActive()) return 0;

            return voxie_keystat(i);
        }

        public override bool GetKey(int keycode)
        {
            if (!isActive()) return false;
            int ks = voxie_keystat(keycode);
            return ks != 0;
        }

        public override bool GetKeyUp(int keycode)
        {
            if (!isActive()) return false;
            return voxie_keystat(keycode) == 0;
        }

        public override bool GetKeyDown(int keycode)
        {
            if (!isActive()) return false;
            return voxie_keystat(keycode) == 1;
        }

        public override float[] GetMousePosition()
        {
            if (!isActive()) return null;
            float[] pos = new float[3];
            pos[0] = ins.dmousx; pos[1] = ins.dmousy; pos[2] = ins.dmousz;

            return pos;
        }

        public override bool GetMouseButton(int button)
        {
            if (!isActive()) return false;
            return (ins.bstat & button) != 0;
        }

        public override bool GetMouseButtonDown(int button)
        {
            if (!isActive()) return false;
            int button_state = ins.bstat & button;
            int old_button_state = ins.obstat & button;

            // TODO: Need to implement seen-ness if we can't fix disparity between input call and ins update
            // Possible solution would be to to old_button_state |= button to make it seen
            bool mouse_down = (old_button_state == 0 & button_state != 0);

            return mouse_down;

        }

        void update_xbox_controllers()
        {
            if (!isActive()) return;
            for (int idx = 0; idx < MAXCONTROLLERS; ++idx)
            {
                controllers[idx].last_frame = controllers[idx].input;
                voxie_xbox_read(idx, ref controllers[idx].input);
            }
        }

        void load_xbox_controllers()
        {
            for (int idx = 0; idx < MAXCONTROLLERS; ++idx)
            {
                controllers[idx].input = new voxie_xbox_t();
                controllers[idx].offset = new voxie_xbox_t();
                voxie_xbox_read(idx, ref controllers[idx].offset);
            }
        }

        public override bool GetButton(int button, int player)
        {
            if (!isActive()) return false;
            return (controllers[player].input.but & button) > 0;
        }

        public override bool GetButtonDown(int button, int player)
        {
            if (!isActive()) return false;
            return ((controllers[player].input.but & button) > 0 && (controllers[player].last_frame.but & button) == 0);
        }

        public override bool GetButtonUp(int button, int player)
        {
            if (!isActive()) return false;
            return ((controllers[player].input.but & button) == 0 && (controllers[player].last_frame.but & button) > 0);
        }

        public override float GetAxis(int axis, int player)
        {
            if (!isActive()) return 0;
            if (axis == 1)
            {
                return Convert.ToSingle(controllers[player].input.lt) / 32768.0f;
            }
            else if (axis == 2)
            {
                return Convert.ToSingle(controllers[player].input.rt) / 32768.0f;
            }
            else
            {
                switch (axis)
                {
                    case 3: // LeftStickX
                        return (Convert.ToSingle(controllers[player].input.tx0) / 32768.0f);
                    case 4: // LeftStickY
                        return Convert.ToSingle(controllers[player].input.ty0) / 32768.0f;
                    case 5: // RightStickX
                        return (Convert.ToSingle(controllers[player].input.tx1) / 32768.0f);
                    case 6: // RightStickY
                        return Convert.ToSingle(controllers[player].input.ty1) / 32768.0f;
                    default:
                        return 0;
                }
            }
        }
        #endregion

        #region sound
        public override float GetVolume()
        {
            return vw.sndfx_vol / 65535.0f;
        }
        #endregion

        #region logging
        // LOGGING
        public override void LogToFile(string msg)
        {
            var FS = File.AppendText("exception.log");
            
            FS.WriteLine(msg);
            FS.Close();
        }

        public override void LogToScreen(int x, int y, string Text)
        {
            if (!isActive()) return;
            // Get Char Values for String
            byte[] tmp = enc.GetBytes(Text);
            byte[] ts = new byte[tmp.Length + 1];
            tmp.CopyTo(ts, 0);
            // Append 0 to end string
            ts[tmp.Length] = 0;

            voxie_debug_print6x8(x, y, 0xFFFFFF, 0x0, ts); // array);
        }
        #endregion
        #endregion

        #region class_functions
        ~Runtime()
        {
            Shutdown();
        }
        #endregion
    }
}
