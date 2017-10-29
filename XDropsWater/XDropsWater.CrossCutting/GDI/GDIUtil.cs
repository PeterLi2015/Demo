using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDropsWater.CrossCutting.GDI
{
    /// <summary>
    /// GDI+扩展
    /// </summary>
    public class GDIEx
    {
        /// <summary>
        /// 画一个圆角的矩形
        /// </summary>
        /// <param >绘图对象（Graphics)</param>
        /// <param >矩形起点的X坐标</param>
        /// <param >矩形起点的Y坐标</param>
        /// <param >矩形的宽度</param>
        /// <param >矩形的高度</param>
        /// <param >圆弧的半径</param>
        public static void DrawRoundRectange(Graphics g, Pen pen, int x, int y, int width, int height, int radius)
        {
            GDIRoundRect rr = new GDIRoundRect(g);
            rr.DrawRoundRectangle(pen, x, y, width, height, radius);
        }

        public static void FillRoundRectange(Graphics g, Brush brush, int x, int y, int width, int height, int radius)
        {
            GDIRoundRect rr = new GDIRoundRect(g);
            rr.FillRoundRectangle(brush, x, y, width, height, radius);
        }

        /// <summary>
        /// 把html的颜色码转化为颜色对象
        /// </summary>
        /// <param name="htmlColorCode"></param>
        /// <returns></returns>
        public static Color ConverToColor(string htmlColorCode)
        {
            return System.Drawing.ColorTranslator.FromHtml(htmlColorCode);
        }
    }
}
