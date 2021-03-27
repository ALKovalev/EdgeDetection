using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;

namespace Filters
{
    class MMgradFilter : Filters
    {
        private Bitmap resE;
        private Bitmap resD;

        private int _kwidth;
        private int _kheight;
        private int[,] _kmatrix;

        public MMgradFilter(int w, int h, int[,] k)
        {
            _kwidth = w;
            _kheight = h;
            _kmatrix = k;
        }

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color color = resD.GetPixel(x, y);
            if (color.R >= 250 && color.G >= 250 && color.B >= 250)
            {
                return Color.Black;
            }

            return sourceImage.GetPixel(x, y);
        }


        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker bgWorker)
        {
            DilationFilter d = new DilationFilter(_kwidth, _kheight, _kmatrix);
            ErosionFilter e = new ErosionFilter(_kwidth, _kheight, _kmatrix);
            resE = d.processImage(sourceImage, bgWorker);
            resD = e.processImage(sourceImage, bgWorker);

            return base.processImage(resE, bgWorker);
        }
    }
}
