using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDropsWater.CrossCutting.BarCode
{
    public static class BarCodeUtil
    {
        /// <summary>
        /// 条码生成
        /// </summary>
        /// </summary>
        /// <param name="InputData">Message to be encoded</param>
        /// <param name="BarWeight">Base thickness for bar width (1 or 2 works well)</param>
        /// <param name="AddQuietZone">Add required horiz margins (use if output is tight)</param>
        /// <returns>An Image of the Code128 barcode representing the message</returns>
        public static Image MakeBarcodeImage(string InputData, int BarWeight, bool AddQuietZone, bool isShowCode)
        {
            return Code128Rendering.MakeBarcodeImage(InputData, BarWeight, AddQuietZone, isShowCode);
        }
    }
}
