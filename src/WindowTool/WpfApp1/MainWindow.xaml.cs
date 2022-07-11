using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        bool isMouseUp;
        int count = 0;
        private void button1_Click(object sender, EventArgs e)
        {


        }


        private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isMouseUp = true;//鼠标左右键被按下
            Cursor = Cursors.Hand; //改变鼠标样式为十字架
            Debug.WriteLine("down");
        }

        private void Button_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isMouseUp = false;//鼠标左右键被弹起
            Cursor = Cursors.Arrow;//改变鼠标样式为默认
            Debug.WriteLine("up");
        }

        private void Button_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            count++;
            //当鼠标移动时发生
            if (isMouseUp)//左键是否被按下
            {
                Win32.LPPOINT pi;
                Win32.GetCursorPos(out pi); //获取鼠标坐标值
                var dd = Win32.GetDesktopWindow();
                IntPtr intPtr = Win32.ChildWindowFromPointEx(dd, pi.X, pi.Y, 0x0001 | 0x0002);
                Debug.WriteLine("FFF1   " + pi.X + "	" + pi.Y + "    d" + dd + "    i" + intPtr);
                if (intPtr != IntPtr.Zero)
                {
                    IntPtr intPtr2 = intPtr;
                    for (; ; )
                    {
                        Win32.ScreenToClient(intPtr2, ref pi);
                        intPtr2 = (IntPtr)Win32.ChildWindowFromPointEx(intPtr2, pi.X, pi.Y, 0x0000);
                        if (intPtr2 == IntPtr.Zero || intPtr2 == intPtr)
                        {
                            break;
                        }
                        intPtr = intPtr2;
                        Win32.GetCursorPos(out pi); //获取鼠标坐标值
                        Debug.WriteLine("FFF2   " + pi.X + "	" + pi.Y + "	" + intPtr);
                    }
                    var sb = new StringBuilder();
                    Win32.GetWindowText((IntPtr)intPtr, sb, 256);
                    Debug.WriteLine(sb.ToString());
                }
            }
        }
    }
    public class Win32
    {
        [DllImport("user32")] public static extern int GetCursorPos(out LPPOINT lpPoint);
        // Token: 0x060000BA RID: 186
        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);

        // Token: 0x060000BB RID: 187
        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        // Token: 0x060000BC RID: 188
        [DllImport("user32.dll")]
        public static extern IntPtr ChildWindowFromPointEx(IntPtr pHwnd, int xPoint, int yPoint, int uFlgs);

        // Token: 0x060000BD RID: 189
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        // Token: 0x060000BE RID: 190
        [DllImport("user32.dll")]
        public static extern bool ScreenToClient(IntPtr hWnd, ref Win32.LPPOINT lpPoint);

        // Token: 0x060000BF RID: 191
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out Win32.LPRECT lpRect);

        // Token: 0x060000C0 RID: 192
        [DllImport("user32.dll")]
        public static extern bool GetCursorInfo(out Win32.PCURSORINFO pci);

        [DllImport("user32")] public static extern int GetWindowText(IntPtr hwnd, StringBuilder lpString, int cch);

        // Token: 0x04000064 RID: 100
        public const int CWP_SKIPDISABLED = 2;

        // Token: 0x04000065 RID: 101
        public const int CWP_SKIPINVISIBL = 1;

        // Token: 0x04000066 RID: 102
        public const int CWP_All = 0;

        // Token: 0x04000067 RID: 103
        public const uint WM_LBUTTONUP = 514U;

        // Token: 0x02000014 RID: 20
        public struct LPPOINT
        {
            // Token: 0x04000068 RID: 104
            public int X;

            // Token: 0x04000069 RID: 105
            public int Y;
        }

        // Token: 0x02000015 RID: 21
        public struct LPRECT
        {
            // Token: 0x0400006A RID: 106
            public int Left;

            // Token: 0x0400006B RID: 107
            public int Top;

            // Token: 0x0400006C RID: 108
            public int Right;

            // Token: 0x0400006D RID: 109
            public int Bottom;
        }

        // Token: 0x02000016 RID: 22
        public struct PCURSORINFO
        {
            // Token: 0x0400006E RID: 110
            public int cbSize;

            // Token: 0x0400006F RID: 111
            public int flag;

            // Token: 0x04000070 RID: 112
            public IntPtr hCursor;

            // Token: 0x04000071 RID: 113
            public Win32.LPPOINT ptScreenPos;
        }
    }
}
