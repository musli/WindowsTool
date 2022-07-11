using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
using Win32;

namespace WindowTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        Model winModel = new Model();
        IntPtr windowHandle = IntPtr.Zero;
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = winModel;
        }
        bool isMouseUp;
        int count = 0;
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

            ////改变样式
            //User.ShowWindow(windowHandle, User.SW_SHOW);
            //User.EnableWindow(windowHandle, 1);
            //int style = User.GetWindowLong(windowHandle, User.GWL_STYLE);
            //style |= ((int)User.WS_CHILD);
            //style |= ((int)User.WS_CLIPCHILDREN);
            //User.SetWindowLong(windowHandle, User.GWL_STYLE, style);
            ////消除边框
            //style = User.GetWindowLong(windowHandle, User.GWL_STYLE);
            //style &= ~(int)(User.WS_EX_TOOLWINDOW | User.WS_CAPTION | User.WS_THICKFRAME |
            //    User.WS_MINIMIZEBOX | User.WS_MAXIMIZEBOX | User.WS_MAXIMIZE | User.WS_SYSMENU);
            //User.SetWindowLong(windowHandle, User.GWL_STYLE, User.GetWindowLong(windowHandle,User.GWL_STYLE)|User.WS_POPUP|User.WS_MAXIMIZEBOX);

            ////var rect = default(RECT);
            ////var ff=User.InvalidateRect(windowHandle, ref rect, 1);
            ////var dd1 = User.GetClassLong(windowHandle, User.GCL_STYLE);
            ////dd1 = dd1 & ~User.CS_OWNDC;
            ////var dd = User.SetClassLong(windowHandle, User.GCL_STYLE, dd1);
            ////ff = User.InvalidateRect(windowHandle, ref rect, 1);

            //var dd1 = User.GetClassLong(windowHandle, User.GCL_HCURSOR);
            //dd1 = dd1 & ~User.CS_OWNDC;
            //var dd = User.SetClassLong(windowHandle, User.GCL_HCURSOR, dd1);


            // var ddf=Kernel.GetLastError();
            // User.CS_VREDRAW | User.CS_HREDRAW |

            //设置透明
            int exStyle = User.GetWindowLong(windowHandle, User.GWL_EXSTYLE);
            exStyle |= (int)User.WS_EX_LAYERED;
            //exStyle |= (int)User.WS_EX_APPWINDOW;
            //exStyle = exStyle&~((int)User.WS_EX_WINDOWEDGE);
            User.SetWindowLong(windowHandle, User.GWL_EXSTYLE, exStyle);



            Win32.RECT lprect = default(Win32.RECT);
            User.GetWindowRect(windowHandle, ref lprect);
            winModel.Width = lprect.Right - lprect.Left;
            winModel.Height = lprect.Bottom - lprect.Top;
            winModel.Top = lprect.Top;
            winModel.Left = lprect.Left;
            widSli.Value = winModel.Width;
            heSli.Value = winModel.Height;

            User.SetWindowPos(windowHandle, (IntPtr)(0), winModel.Left, winModel.Top,400, 400, User.SWP_SHOWWINDOW | User.SWP_NOACTIVATE);

            MessageBox.Show($"顶{winModel.Top}左{winModel.Left}宽{winModel.Width}高{winModel.Height}");

        }

        StringBuilder sb = new StringBuilder();
        private void Button_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            count++;
            //当鼠标移动时发生
            if (isMouseUp)//左键是否被按下
            {
                try
                {
                    Win32.POINT pi;
                    User.GetCursorPos(out pi); //获取鼠标坐标值
                    var dd = User.GetDesktopWindow();
                    IntPtr intPtr = User.ChildWindowFromPointEx(dd, pi.x, pi.y, 0x0001 | 0x0002);
                    //Debug.WriteLine("FFF1   " + pi.x + "	" + pi.y + "    d" + dd + "    i" + intPtr);
                    if (intPtr != IntPtr.Zero)
                    {
                        try
                        {
                            sb.Clear();
                            User.GetWindowText((IntPtr)intPtr, sb, 256);
                            this.Title = sb.ToString();
                            windowHandle = (IntPtr)intPtr;


                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void opSli_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            User.SetLayeredWindowAttributes(windowHandle, 0xffffff, (byte)Math.Ceiling(255 * (Convert.ToDouble(opSli.Value))), 2);
        }

        private void widSli_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            User.SetWindowPos(windowHandle, (IntPtr)(0), winModel.Left, winModel.Top, winModel.Width, winModel.Height, User.SWP_SHOWWINDOW | User.SWP_NOACTIVATE);
        }

        private void heSli_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            User.SetWindowPos(windowHandle, (IntPtr)(0), winModel.Left, winModel.Top, winModel.Width, winModel.Height, User.SWP_SHOWWINDOW | User.SWP_NOACTIVATE);
        }

        private void widSli_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Win32.RECT lprect = default(Win32.RECT);
            User.GetWindowRect(windowHandle, ref lprect);
            winModel.Top = lprect.Top;
            winModel.Left = lprect.Left;
        }
    }
    public class Model
    {
        public int Opacity { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Top { get; set; }
        public int Left { get; set; }
    }
}
