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
        int szamlalo;
        



        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofl = new OpenFileDialog();
            if(ofl.ShowDialog() == DialogResult.OK)
            {
                kep = new Image<Bgr, byte>(ofl.FileName);
                
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
            Image<Bgr, byte> szurkeSzinben = szurke.Convert<Bgr, byte>();
            Mat hier = new Mat();
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            VectorOfInt hull = new VectorOfInt();

            CvInvoke.FindContours(szurke, contours, hier, RetrType.Tree, ChainApproxMethod.ChainApproxNone);

            for (int i = 0; i < contours.Size; i++) 
            {
                CvInvoke.ConvexHull(contours[i], hull,true); //convex hull megalkotas

                for(int j=0;j<hull.Size;j++)
                {
                    if(j != hull.Size - 1)
                    {
                        int xtav = 15;
                        int ytav = 10;
                        int x = contours[i][hull[j]].X;
                        int y = contours[i][hull[j]].Y;
                        int xKov = contours[i][hull[j+1]].X;
                        int yKov = contours[i][hull[j+1]].Y;
                        if ((x + xtav)<xKov || (y + ytav)<yKov)
                        {
                            szamlalo++;
                            //textBox1.AppendText(j + ". X: " + contours[i][hull[j]].X.ToString() + " Y: " + contours[i][hull[j]].Y.ToString() + "\r\n");
                            CvInvoke.Circle(szurkeSzinben, contours[i][hull[j]], 5, new MCvScalar(200, 0, 50), 2); //pontok kirajzolása
                            CvInvoke.Line(szurkeSzinben, contours[i][hull[j]], contours[i][hull[j + 1]], new MCvScalar(0, 0, 155), 2); //vonalak meghuzasa

                        } else {
                            CvInvoke.Line(szurkeSzinben, contours[i][hull[j]], contours[i][hull[j + 1]], new MCvScalar(0, 0, 155), 2); //vonalak meghuzasa
                        }
                        textBox1.AppendText(j + ". X: " + x.ToString() + " Y: " + y.ToString() + "\r\n");

                        
                    }
                    
                }
            }
            textBox1.AppendText(szamlalo + "\r\n");
            pictureBox1.Image = szurkeSzinben.ToBitmap();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            label1.Text = "X: " + (((this.PointToClient(MousePosition).X - pictureBox1.Location.X))).ToString();
            label2.Text = "Y: " + (((this.PointToClient(MousePosition).Y - pictureBox1.Location.Y))).ToString();
        }
    }
}
