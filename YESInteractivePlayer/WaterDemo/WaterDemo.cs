using SlimDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YESInteractiveSDK;

namespace WaterDemo
{
    [Serializable]
    class WaterDemo : MarshalByRefObject,IYESInterfaceSDK
    {
        UserControl1 tmp;
        public void InitAddonFunc(Control Parent)
        {
            tmp = new UserControl1();
            tmp.Dock = DockStyle.Fill;
            Parent.Controls.Add(tmp);
        }

        public void FinalizeAddonFunc(Control Parent)
        {
            Parent.Controls.Remove(tmp);
            tmp.Dispose();
        }
        
        public void PointActive(ModuleStructure.PointInfo Point)
        {
            tmp.PointActive(Point);
        }

        public void PointActive(ModuleStructure.PointInfo[] Point)
        {
            Debug.WriteLine("");
        }
    }
}
