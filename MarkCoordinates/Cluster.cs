namespace MarkCoordinates
{
    using System.Drawing;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Emgu.CV;
    using Emgu.Util;
    using Emgu.CV.Structure;

    class Cluster
    {
        private List<PointF> points;
        internal List<PointF> Points
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                points = value;
            }
        }

        public Cluster(Bitmap image, Bitmap outBitmap)
        {
            //var matrix = new Matrix<int>(image.Height, image.Width);
            int threshold_value = 50;
            Image<Gray, byte> img = new Image<Gray, byte>(image);
            Image<Gray,Byte> binary_Image = img.ThresholdBinary(new Gray(threshold_value), new Gray(255));
            outBitmap = binary_Image.ToBitmap();
        }
    }
}
