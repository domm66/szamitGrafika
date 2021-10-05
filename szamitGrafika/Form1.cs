using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu;
using Emgu.CV.Util;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV;
using Emgu.CV.CvEnum;

namespace szamitGrafika
{
    public partial class Form1 : Form
    {

        Image<Bgr, byte> kep;
        Image<Bgr, byte> eredeti;
        



        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofl = new OpenFileDialog();
            if(ofl.ShowDialog() == DialogResult.OK)
            {
                eredeti = new Image<Bgr, byte>(ofl.FileName);
                kep = eredeti.Clone();
                pictureBox1.Image = kep.ToBitmap();

                //kep = kep.Resize(0.2, Inter.Linear).SmoothMedian(5).SmoothGaussian(1);
                //kep = kep.Resize(5, Inter.Linear);

                //kep = kep.Resize(0.25, Inter.Linear).SmoothMedian(5);
                //kep = kep.Resize(4, Inter.Linear);

                kep = kep.Resize(0.3, Inter.Linear).SmoothMedian(5);
                kep = kep.Resize(3.3, Inter.Linear);

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Image<Gray, byte> szurke = kep.Convert<Gray, byte>(); //eredeti kep szurkeve alakitasa
            //CvInvoke.Threshold(szurke, szurke, 115, 255, ThresholdType.Binary);
            CvInvoke.Threshold(szurke, szurke, 0, 255, ThresholdType.Otsu);  //Otsu thresholding, talan kicsit jobban mukodik, de a masikkar is lehet probalkozni, csak ki kell kommentelni
            Mat hier = new Mat();
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            VectorOfInt hull = new VectorOfInt();

            CvInvoke.FindContours(szurke, contours, hier, RetrType.Tree, ChainApproxMethod.ChainApproxNone);

            for (int i = 0; i < contours.Size; i++) 
            {
                CvInvoke.ConvexHull(contours[i], hull); //convex hull megalkotas

                for(int j=0;j<hull.Size;j++)
                {
                    if(j != hull.Size - 1)
                    {
                        CvInvoke.Circle(eredeti, contours[i][hull[j]], 2, new MCvScalar(200, 0, 50), 2); //pontok kirajzolása
                        CvInvoke.Line(eredeti, contours[i][hull[j]], contours[i][hull[j + 1]], new MCvScalar(0, 0, 155), 2); //vonalak meghuzasa
                    }
                    
                }
            }

            pictureBox1.Image = eredeti.ToBitmap();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }
    }
}
