using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Voxon
{
	[TestClass]
	public class DLL_Load_Tests
	{
		public Runtime runtime;

		[TestInitialize]
		public void Initialize()
		{
			runtime = new Runtime();
		}

		[TestMethod]
		public void DefaultLoadedState_Test()
		{
			Assert.IsFalse(runtime.isLoaded(), "\nIsLoaded");
		}

		[TestMethod]
		public void RetrieveDLLString_Test()
		{
			Assert.AreNotEqual("", runtime.GetDLLPath());
		}

		[TestMethod]
		public void LoadLibraryExtraSlash_Test()
		{
			string testStr = runtime.GetDLLPath();
			testStr.Replace("\\", "\\\\");
			runtime.LoadDLL(runtime.GetDLLPath());
		}

		[TestMethod]
		public void LoadLibrary_Test()
		{
			runtime.LoadDLL(runtime.GetDLLPath());
		}

		[TestMethod]
		public void HandleGenerated_Test()
		{
			runtime.Load();
			Assert.AreNotEqual(IntPtr.Zero, runtime.Handle);
			Assert.IsTrue(runtime.isLoaded(), "\nIsUnloaded");
		}

		[TestMethod]
		public void GetDLLVersion_Test()
		{
			runtime.Load();
			Int64 version = runtime.GetDLLVersion();
			Assert.IsTrue(version > 0);
		}

		[TestMethod]
		public void DelegateInstancesBound_Test()
		{
			runtime.Load();

			Assert.IsNotNull(runtime.voxie_loadini_int, "voxie_loadinit_int");
			Assert.IsNotNull(runtime.voxie_init, "voxie_init");
			Assert.IsNotNull(runtime.voxie_uninit_int, "voxie_uninit_int");
			Assert.IsNotNull(runtime.voxie_breath, "voxie_breath");
			Assert.IsNotNull(runtime.voxie_getvw, "voxie_getvw");
			Assert.IsNotNull(runtime.voxie_quitloop, "voxie_quitloop");
			Assert.IsNotNull(runtime.voxie_klock, "voxie_klock");
			Assert.IsNotNull(runtime.voxie_keystat, "voxie_keystat");
			Assert.IsNotNull(runtime.voxie_keyread, "voxie_keyread");
			Assert.IsNotNull(runtime.voxie_volcap, "voxie_volcap");
			Assert.IsNotNull(runtime.voxie_setview, "voxie_setview");
			Assert.IsNotNull(runtime.voxie_frame_start, "voxie_frame_start");
			Assert.IsNotNull(runtime.voxie_frame_end, "voxie_frame_end");
			Assert.IsNotNull(runtime.voxie_setleds, "voxie_setleds");
			Assert.IsNotNull(runtime.voxie_drawvox, "voxie_drawvox");
			Assert.IsNotNull(runtime.voxie_drawbox, "voxie_drawbox");
			Assert.IsNotNull(runtime.voxie_drawlin, "voxie_drawlin");
			Assert.IsNotNull(runtime.voxie_drawpol, "voxie_drawpol");
			Assert.IsNotNull(runtime.voxie_drawmeshtex, "voxie_drawmeshtex");
			Assert.IsNotNull(runtime.voxie_drawmeshtex_null, "voxie_drawmeshtex_null");
			Assert.IsNotNull(runtime.voxie_drawsph, "voxie_drawsph");
			Assert.IsNotNull(runtime.voxie_drawcone, "voxie_drawcone");
			Assert.IsNotNull(runtime.voxie_drawspr, "voxie_drawspr");
			Assert.IsNotNull(runtime.voxie_printalph, "voxie_printalph");
			Assert.IsNotNull(runtime.voxie_drawcube, "voxie_drawcube");
			Assert.IsNotNull(runtime.voxie_drawheimap, "voxie_drawheimap");
			Assert.IsNotNull(runtime.voxie_playsound, "voxie_playsound");
			Assert.IsNotNull(runtime.voxie_xbox_read, "voxie_xbox_read");
			Assert.IsNotNull(runtime.voxie_xbox_write, "voxie_xbox_write");
            Assert.IsNotNull(runtime.voxie_nav_read, "voxie_nav_read");
            // Debug Functions
            Assert.IsNotNull(runtime.voxie_debug_print6x8, "voxie_debug_print6x8");
			Assert.IsNotNull(runtime.voxie_debug_drawpix, "voxie_debug_drawpix");
			Assert.IsNotNull(runtime.voxie_debug_drawhlin, "voxie_debug_drawhlin");
			Assert.IsNotNull(runtime.voxie_debug_drawline, "voxie_debug_drawline");
			Assert.IsNotNull(runtime.voxie_debug_drawcirc, "voxie_debug_drawcirc");
			Assert.IsNotNull(runtime.voxie_debug_drawrectfill, "voxie_debug_drawrectfill");
			Assert.IsNotNull(runtime.voxie_debug_drawcircfill, "voxie_debug_drawcircfill");
			Assert.IsNotNull(runtime.voxie_getversion, "voxie_getversion");
		}

		[TestMethod]
		public void SetEmulatorVAng_Test()
		{
			// Start Up Simulator
			runtime.Load();
			runtime.Initialise();
			while (!runtime.isInitialised()) ;

			float expected, result, input;

			// Normal Usage
			input = -1; expected = -1;
			result = runtime.SetEmulatorVerticalAngle(input);
			Assert.AreEqual(expected, result, "BASECASE: Normal Usage");

			// Edge Case Negative Valid
			input = Runtime.MIN_EMU_VANG; expected = Runtime.MIN_EMU_VANG;
			result = runtime.SetEmulatorVerticalAngle(input);
			Assert.AreEqual(expected, result, "EDGECASE: Negative Valid");

			// Edge Case Negative Invalid
			input = Runtime.MIN_EMU_VANG - 1; expected = Runtime.MIN_EMU_VANG;
			result = runtime.SetEmulatorVerticalAngle(input);
			Assert.AreEqual(expected, result, "EDGECASE: Negative Invalid");

			// Edge Case Positive Valid
			input = Runtime.MAX_EMU_VANG; expected = Runtime.MAX_EMU_VANG;
			result = runtime.SetEmulatorVerticalAngle(input);
			Assert.AreEqual(expected, result, "EDGECASE: Positive Valid");

			// Edge Case Positive Invalid
			input = Runtime.MAX_EMU_VANG + 1; expected = Runtime.MAX_EMU_VANG;
			result = runtime.SetEmulatorVerticalAngle(input);
			Assert.AreEqual(expected, result, "EDGECASE: Positive Invalid");
		}

		[TestMethod]
		public void SetEmulatorHAng_Test()
		{
			// Start Up Simulator
			runtime.Load();
			runtime.Initialise();
			while (!runtime.isInitialised()) ;

			float expected, result, input;

			// Normal Usage
			input = 1; expected = 1;
			result = runtime.SetEmulatorHorizontalAngle(input);
			Assert.AreEqual(expected, result,"BASECASE: Normal Usage");

			// Edge Case Negative Valid
			input = Runtime.MIN_EMU_HANG; expected = Runtime.MIN_EMU_HANG;
			result = runtime.SetEmulatorHorizontalAngle(input);
			Assert.AreEqual(expected, result, "EDGECASE: Negative Valid");

			// Edge Case Negative Invalid
			input = Runtime.MIN_EMU_HANG - 1; expected = Runtime.MIN_EMU_HANG;
			result = runtime.SetEmulatorHorizontalAngle(input);
			Assert.AreEqual(expected, result, "EDGECASE: Negative Invalid");

			// Edge Case Positive Valid
			input = Runtime.MAX_EMU_HANG; expected = Runtime.MAX_EMU_HANG;
			result = runtime.SetEmulatorHorizontalAngle(input);
			Assert.AreEqual(expected, result, "EDGECASE: Positive Valid");

			// Edge Case Positive Invalid
			input = Runtime.MAX_EMU_HANG + 1; expected = Runtime.MAX_EMU_HANG;
			result = runtime.SetEmulatorHorizontalAngle(input);
			Assert.AreEqual(expected, result, "EDGECASE: Positive Invalid");
		}

		[TestCleanup]
		public void Cleanup()
		{
			runtime.Unload();
			runtime = null;
		}
	}
}
