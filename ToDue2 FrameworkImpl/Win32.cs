using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace ToDue2
{
	public class Win32
	{
		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

		[Flags]
		public enum ExtendedWindowStyles
		{
			// ...
			WS_EX_TOOLWINDOW = 0x00000080,
			// ...
		}

		public enum GetWindowLongFields
		{
			// ...
			GWL_EXSTYLE = -20,
			// ...
		}

		public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
		{
			int error = 0;
			IntPtr result = IntPtr.Zero;
			// Win32 SetWindowLong doesn't clear error on success
			SetLastError(0);

			if (IntPtr.Size == 4)
			{
				// use SetWindowLong
				Int32 tempResult = IntSetWindowLong(hWnd, nIndex, IntPtrToInt32(dwNewLong));
				error = Marshal.GetLastWin32Error();
				result = new IntPtr(tempResult);
			}
			else
			{
				// use SetWindowLongPtr
				result = IntSetWindowLongPtr(hWnd, nIndex, dwNewLong);
				error = Marshal.GetLastWin32Error();
			}

			if ((result == IntPtr.Zero) && (error != 0))
			{
				throw new System.ComponentModel.Win32Exception(error);
			}

			return result;
		}

		[DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
		private static extern IntPtr IntSetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		[DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
		private static extern Int32 IntSetWindowLong(IntPtr hWnd, int nIndex, Int32 dwNewLong);

		private static int IntPtrToInt32(IntPtr intPtr)
		{
			return unchecked((int)intPtr.ToInt64());
		}

		[DllImport("kernel32.dll", EntryPoint = "SetLastError")]
		public static extern void SetLastError(int dwErrorCode);

		[DllImport("user32.dll")]
		static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
		[DllImport("user32.dll")]

		static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X,
			int Y, int cx, int cy, uint uFlags);

		const uint SWP_NOSIZE = 0x0001;
		const uint SWP_NOMOVE = 0x0002;
		const uint SWP_NOACTIVATE = 0x0010;

		static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

		public static void SetBottom(Window window)
		{
			IntPtr hWnd = new WindowInteropHelper(window).Handle;
			SetWindowPos(hWnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);
		}
	}
}
