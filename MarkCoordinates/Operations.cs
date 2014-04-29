namespace MarkCoordinates
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using Emgu.CV;
    using Emgu.CV.CvEnum;
    using Emgu.CV.Structure;

    static class Operations
    {
        internal static void CannyLines(String fileName, out Bitmap bitmap1, out Bitmap bitmap2)
        {
            //Load the image from file and resize it for display
            Image<Bgr, Byte> img = new Image<Bgr, byte>(fileName)
                                       .Resize(400, 400, INTER.CV_INTER_LINEAR, true);

            //Convert the image to grayscale
            Image<Gray, Byte> gray = img.Convert<Gray, Byte>();

            #region Canny and edge detection
            double cannyThreshold = 180.0;
            double cannyThresholdLinking = 120.0;
            Image<Gray, Byte> cannyEdges = gray.Canny(cannyThreshold, cannyThresholdLinking);
            LineSegment2D[] lines = cannyEdges.HoughLinesBinary(
                1, //Distance resolution in pixel-related units
                Math.PI / 45.0, //Angle resolution measured in radians.
                20, //threshold
                30, //min Line width
                10 //gap between lines
                )[0]; //Get the lines from the first channel
            #endregion

            #region draw lines
            Image<Bgr, Byte> lineImage = img.CopyBlank();
            foreach (LineSegment2D line in lines)
            {
                lineImage.Draw(line, new Bgr(Color.Green), 2);

                ////PointF center = CenterOf(line);
                ////lineImage.Draw(new CircleF(center, 4), new Bgr(Color.Coral), 2);
            }
            var centers = CentersOf(lines, 3);
            foreach (PointF center in centers)
            {
                lineImage.Draw(new CircleF(center, 4), new Bgr(Color.Coral), 2);
            }
            
            
            #endregion

            bitmap1 = gray.ToBitmap();
            bitmap2 = lineImage.ToBitmap();
        }

/*
        private static PointF CenterOf(LineSegment2D line)
        {
            Int32 maxX, minX, maxY, minY;
            Int32 centerX, centerY;
            if (line.P1.X > line.P2.X)
            {
                maxX = line.P1.X;
                minX = line.P2.X;
            }
            else
            {
                maxX = line.P2.X;
                minX = line.P1.X;
            }
            if (line.P1.Y > line.P2.Y)
            {
                maxY = line.P1.Y;
                minY = line.P2.Y;
            }
            else
            {
                maxY = line.P2.Y;
                minY = line.P1.Y;
            }
            centerX = (maxX - minX) / 2 + minX;
            centerY = (maxY - minY) / 2 + minY;
            return new PointF(centerX, centerY);
        }
*/


        /// <summary>
        /// Method using kmeans clustering
        /// </summary>
        /// <returns>Set of centers points</returns>
        private static IEnumerable<PointF> CentersOf(IList<LineSegment2D> lines, Int32 numberOfClusters)
        {
            Int32 numberOfPoints = lines.Count() * 2;
            Int32 clusterCount;
            if (numberOfClusters > numberOfPoints)
                clusterCount = numberOfPoints - 1 >= 0 ? numberOfPoints - 1 : 0;
            else
                clusterCount = numberOfClusters;
            var centers = new PointF[clusterCount];
            if (!lines.Any())
                return centers;      
  
            ////Код, отвечающий за использование метода Kmeans
            var samples = new Matrix<Single>(numberOfPoints, 2);
            var labels = new Matrix<Int32>(numberOfPoints, 1);
            var centersMatrix = new Matrix<Single>(clusterCount, 2);
            for (Int32 u = 0, k = 0; k < lines.Count(); u += 2, k++)
            {
                samples[u, 0] = lines[k].P1.X;
                samples[u, 1] = lines[k].P1.Y;
                samples[u + 1, 0] = lines[k].P2.X;
                samples[u + 1, 1] = lines[k].P2.Y;
            }
            CvInvoke.cvKMeans2(
                samples,
                clusterCount,
                labels,
                new MCvTermCriteria(),
                2,
                IntPtr.Zero,
                0,
                centersMatrix,
                IntPtr.Zero);

            var labels4Lines = new Matrix<Int32>(lines.Count, 1);
            for (Int32 k = 0; k < lines.Count(); k++)
            {
                labels4Lines[k, 0] = labels[k * 2, 0];
            }

            for (Int32 k = 0; k < clusterCount; k++)
            {
                centers[k].X = centersMatrix[k, 0];
                centers[k].Y = centersMatrix[k, 1];
                var linesgroup = new List<LineSegment2D>();
                for (Int32 i = 0; i < labels4Lines.Rows; i++)
                {
                    if (labels4Lines[i, 0] == k) linesgroup.Add(lines[i]);
                }
                if (linesgroup.Count > 1)
                {
                    //find a, b
                    //x1 * a + b = y1
                    //x2 * a + b = y2                   
                    var a1 = new Matrix<Single>(2, 2);
                    var b1 = new Matrix<Single>(2, 1);
                    a1[0, 0] = linesgroup[0].P1.X;
                    a1[0, 1] = 1;
                    a1[1, 0] = linesgroup[0].P2.X;
                    a1[1, 1] = 1;

                    b1[0, 0] = linesgroup[0].P1.Y;
                    b1[1, 0] = linesgroup[0].P2.Y;

                    var x1 = new Matrix<Single>(2, 1);
                    CvInvoke.cvSolve(a1, b1, x1, SOLVE_METHOD.CV_LU);

                    //find a, b
                    //x1 * a + b = y1
                    //x2 * a + b = y2                   
                    var a2 = new Matrix<Single>(2, 2);
                    var b2 = new Matrix<Single>(2, 1);
                    a2[0, 0] = linesgroup[1].P1.X;
                    a2[0, 1] = 1;
                    a2[1, 0] = linesgroup[1].P2.X;
                    a2[1, 1] = 1;

                    b2[0, 0] = linesgroup[1].P1.Y;
                    b2[1, 0] = linesgroup[1].P2.Y;

                    var x2 = new Matrix<Single>(2, 1);
                    CvInvoke.cvSolve(a2, b2, x2, SOLVE_METHOD.CV_LU);


                    //find point
                    //y - a1 * x = b1
                    //y - a2 * x = b2
                    var a3 = new Matrix<Single>(2, 2);
                    var b3 = new Matrix<Single>(2, 1);
                    a3[0, 0] = 1;
                    a3[0, 1] = -x1[0, 0]; //a1
                    a3[1, 0] = 1;
                    a3[1, 1] = -x2[0, 0]; //a2

                    b3[0, 0] = x1[1, 0]; //b1
                    b3[1, 0] = x2[1, 0]; //b2

                    var center = new Matrix<Single>(2, 1);
                    CvInvoke.cvSolve(a3, b3, center, SOLVE_METHOD.CV_LU);


                    centers[k].X = center[1, 0]; //x
                    centers[k].Y = center[0, 0]; //y
                }
            }
          
            return centers;
        }


    }
}
