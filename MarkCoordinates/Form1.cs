namespace MarkCoordinates
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Emgu.CV;
    using Emgu.CV.CvEnum;

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var openfile = new OpenFileDialog();
            if (openfile.ShowDialog() == DialogResult.OK)
            {
                const int size = 400;
                var bitmap1 = new Bitmap(openfile.FileName);
                var bitmap2 = new Bitmap(size, size);
                //Operations.CannyLines(openfile.FileName, out bitmap1, out bitmap2);

                var c = new Cluster(bitmap1, bitmap2);
                
                pictureBox1.Image = bitmap1;
                pictureBox2.Image = bitmap2;
            }

            
        }

        private void Test_Click(object sender, EventArgs e)
        {
            var a = new Matrix<Single>(2, 2);
            var b = new Matrix<Single>(2, 1);
            var x = new Matrix<Single>(2, 1);

            a[0,0] = 2;
            a[0,1] = 1;
            a[1,0] = 1;
            a[1,1] = 1;
            b[0,0] = 4;
            b[1,0] = 3;

            CvInvoke.cvSolve(a, b, x, SOLVE_METHOD.CV_LU);
        }

        
    }
}
