using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDropsWater.CrossCutting.GDI
{
    public class IconUtil
    {
        /// <summary>
        /// 把Bitmap转化为Icon
        /// </summary>
        /// <param name="bitmap">Bitmap图片对象</param>
        /// <returns>生成的Icon</returns>
        public static Icon ConvertToIcon(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new Exception("参数不能为空！");
            IntPtr intPtr = bitmap.GetHicon();
            return Icon.FromHandle(intPtr);
        }

        /// <summary>
        /// 叠加图标
        /// </summary>
        /// <param name="upIcon">上层图标</param>
        /// <param name="downIcon">下层图标</param>
        /// <returns>叠加后图标</returns>
        public static Bitmap SuperimposingIcon(Bitmap upIcon, Bitmap downIcon)
        {
            if (upIcon == null || downIcon == null)
                throw new Exception("叠加图片实例不能为空！");
            using (Graphics gp = Graphics.FromImage(downIcon))
            {
                gp.DrawImage(upIcon, new Rectangle(new Point(0, 0), new Size(16, 16)));
                gp.Save();
            }
            return downIcon;
        }
    }
}
