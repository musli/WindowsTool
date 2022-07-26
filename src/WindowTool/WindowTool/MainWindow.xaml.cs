using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Resources;
using System.Windows.Shapes;
using Win32;

namespace WindowTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        Model winModel = new Model();
        IntPtr windowHandle = IntPtr.Zero;
        IntPtr handle = IntPtr.Zero;
        public event PropertyChangedEventHandler PropertyChanged;
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = winModel;

            this.Width = 130;
            this.Height = 300;
            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(10);
                    if (isMouseUp || windowHandle == IntPtr.Zero)
                        continue;

                    Win32.RECT lprect = default(Win32.RECT);
                    User.GetWindowRect(windowHandle, ref lprect);

                    if (lprect.Top < 0 || lprect.Left < 0)
                        continue;

                    Dispatcher.Invoke(() =>
                    {
                        this.Top = lprect.Top - 25;
                        this.Left = lprect.Left - 20;
                    });
                }
            });
        }
        private bool isMouseUp;

        public bool IsMouseUp
        {
            get { return isMouseUp; }
            set
            {
                isMouseUp = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsMouseUp"));
            }
        }

        int count = 0;
        private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsMouseUp = true;//鼠标左右键被按下
                             //Cursor = Cursors.Hand; //改变鼠标样式为十字架
                             //var vb = new VisualBrush(dd);
            Debug.WriteLine(IsMouseUp);
            StreamResourceInfo sri = Application.GetResourceStream(new Uri("/Icon/target.cur", UriKind.Relative));
            Cursor = new Cursor(sri.Stream, true);
            Debug.WriteLine("down");
        }
        StringBuilder sb = new StringBuilder();
        private void Button_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            count++;
            //当鼠标移动时发生
            if (IsMouseUp)//左键是否被按下
            {
                //使用DragDrop的DoDragDrop方法开启拖动功能。拖动方式为拖动复制或移动
                //DragDrop.DoDragDrop(dd,"", DragDropEffects.All);
                try
                {
                    Win32.POINT pi;
                    User.GetCursorPos(out pi); //获取鼠标坐标值
                    var dd = User.GetDesktopWindow();
                    IntPtr intPtr = User.ChildWindowFromPointEx(dd, pi.x, pi.y, 0x0001 | 0x0002);
                    //Debug.WriteLine("FFF1   " + pi.x + "	" + pi.y + "    d" + dd + "    i" + intPtr);
                    if (intPtr != IntPtr.Zero && intPtr != handle)
                    {
                        try
                        {
                            sb.Clear();
                            User.GetWindowText(intPtr, sb, 256);

                            windowHandle = intPtr;


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
        private void Button_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("自身" + handle + "目标" + windowHandle);
            IsMouseUp = false;//鼠标左右键被弹起
            Debug.WriteLine(IsMouseUp);
            Cursor = Cursors.Arrow;//改变鼠标样式为默认
            Debug.WriteLine("up");


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

            //User.SetWindowPos(windowHandle, (IntPtr)(0), winModel.Left, winModel.Top, 400, 400, User.SWP_SHOWWINDOW | User.SWP_NOACTIVATE);

            //MessageBox.Show($"顶{winModel.Top}左{winModel.Left}宽{winModel.Width}高{winModel.Height}");
            btnLink_Click(null, null);
        }



        private void opSli_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            User.SetLayeredWindowAttributes(windowHandle, 0xffffff, (byte)Math.Ceiling(255 * (Convert.ToDouble(opSli.Value))), 2);
        }

        private void widSli_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            winModel.Height = winModel.Width / 16 * 9;
            User.SetWindowPos(windowHandle, (IntPtr)(0), winModel.Left, winModel.Top, winModel.Width, winModel.Height, User.SWP_SHOWWINDOW | User.SWP_NOACTIVATE);
        }

        private void heSli_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            winModel.Width = winModel.Height / 9 * 16;
            User.SetWindowPos(windowHandle, (IntPtr)(0), winModel.Left, winModel.Top, winModel.Width, winModel.Height, User.SWP_SHOWWINDOW | User.SWP_NOACTIVATE);
        }

        private void widSli_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Win32.RECT lprect = default(Win32.RECT);
            User.GetWindowRect(windowHandle, ref lprect);
            winModel.Top = lprect.Top;
            winModel.Left = lprect.Left;
        }
        List<int> pointList = new List<int>();
        int frequency = 10;


        private void btnLink_Click(object sender, RoutedEventArgs e)
        {
            //var dd = (IntPtr)0x001B0168;
            //var handle = (new WindowInteropHelper(this)).Handle;
            //User.SetParent(dd, windowHandle);
            //User.SetWindowPos(dd, (IntPtr)(0), 0, 0, 100, 100, User.SWP_SHOWWINDOW | User.SWP_NOACTIVATE);

        }

        private void Border_DragOver(object sender, DragEventArgs e)
        {
            Debug.WriteLine("拖放完毕");
        }

        private void Border_Drop(object sender, DragEventArgs e)
        {

        }

        private void dd_MouseMove(object sender, MouseEventArgs e)
        {
            // 判断左键是否按下
            //if (e.LeftButton == MouseButtonState.Pressed)
            //{
            //    ////声明DataObject,并打包圆控件的图像绘制方式(包含颜色)、高度及其副本。
            //    //DataObject data = new DataObject();
            //    //data.SetData(DataFormats.StringFormat, circleUI.Fill.ToString());
            //    //data.SetData("Double", circleUI.Height);
            //    //data.SetData("Object", this);


            //}
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            handle = new WindowInteropHelper(this).Handle;
        }

        private void dd_Click(object sender, RoutedEventArgs e)
        {
            User.SetWindowPos(windowHandle, (IntPtr)(0), winModel.Left, winModel.Top, 724, 440, User.SWP_SHOWWINDOW | User.SWP_NOACTIVATE);
        }


        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
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
