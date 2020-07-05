using System;
using System.Runtime.InteropServices;

namespace Voxon
{
	class Alert
	{
		private static long MB_OK = 0x00000000L;
		private static long MB_YESNO = 0x00000004L;
		private static long MB_DEFBUTTON2 = 0x00000100L;
		private static int IDOK = 0;
		private static int IDYES = 6;
		private static int IDNO = 7;
		[DllImport("user32.dll")]
		public static extern int MessageBox(IntPtr hWnd, String text, String caption, int options);

		public static bool Show(string Message)
		{
			int result = MessageBox(IntPtr.Zero, Message, "Alert", (int)(MB_OK));
			return result == IDOK;
		}
	}
}