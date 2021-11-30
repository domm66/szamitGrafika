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
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace szamitGrafika
{
    public partial class Form1 : Form
    {

        Image<Bgr, byte> kep;
        Image<Bgr, byte> eredeti;
        int szamlalo;
        int papir;
        int ko;
        int ollo;
        Image<Hsv, byte> imgInput;
        string path;
        string[] fileok;
        int i = 0;
        int atlag;
        int xtav; //= 20;
        int ytav; //= 11;

        //Image<Bgr, byte> kepekFile = new Image<Bgr, byte>(@"C:\Users\This dude\source\repos\szamitGrafika\szamitGrafika\paper");

        public Form1()
        {
            InitializeComponent();
            //textBox3.Text = "15";
            //textBox4.Text = "13";
        }

        public void fileAdd()
        {
            pictureBox1.Image = null;

            OpenFileDialog ofl = new OpenFileDialog();
            if (ofl.ShowDialog() == DialogResult.OK)
            {
                imgInput = new Image<Hsv, byte>(ofl.FileName).Erode(2).Dilate(2);
                kep = new Image<Bgr, byte>(ofl.FileName);
                eredeti = kep.Clone();
                pictureBox1.Image = kep.ToBitmap();

                //kep = kep.Resize(0.3, Inter.Linear).SmoothMedian(5);
                //kep = kep.Resize(3.3, Inter.Linear);

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            fileAdd();
        }

        public void convexHull() 
        {
            
            pictureBox1.Image = null;
            eredeti = kep.Clone();
            szamlalo = 0;
            Image<Gray, byte> szurke = kep.Convert<Gray, byte>(); //eredeti kep szurkeve alakitasa


            Mat hsv = new Mat();
            CvInvoke.InRange(imgInput, new ScalarArray(new MCvScalar(0, 48, 80)), new ScalarArray(new MCvScalar(20, 255, 255)), hsv);


            CvInvoke.Threshold(szurke, szurke, 0, 255, ThresholdType.Otsu);  //Otsu thresholding, talan kicsit jobban mukodik, de a masikkar is lehet probalkozni, csak ki kell kommentelni

            Image<Bgr, byte> szurkeSzinben = szurke.Convert<Bgr, byte>();
            Mat hier = new Mat();
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            VectorOfInt hull = new VectorOfInt();

            CvInvoke.FindContours(hsv, contours, hier, RetrType.Tree, ChainApproxMethod.ChainApproxNone);

            for (int i = 0; i < contours.Size; i++)
            {
                CvInvoke.ConvexHull(contours[i], hull, true); //convex hull megalkotas
                //textBox2.AppendText(hsv.Size.ToString());

                for (int j = 0; j < hull.Size; j++)
                {
                    if (j != hull.Size - 1)
                    {
                        
                        int x = contours[i][hull[j]].X;
                        int y = contours[i][hull[j]].Y;
                        int xKov = contours[i][hull[j + 1]].X;
                        int yKov = contours[i][hull[j + 1]].Y;
                        if ((x + xtav) < xKov || (y + ytav) < yKov)
                        {
                            if (x > 5 && x < 180)
                            {
                                szamlalo++;
                                CvInvoke.Circle(szurkeSzinben, contours[i][hull[j]], 7, new MCvScalar(200, 0, 50), 2); //pontok kirajzolása
                            }
                            //textBox1.AppendText(j + ". X: " + contours[i][hull[j]].X.ToString() + " Y: " + contours[i][hull[j]].Y.ToString() + "\r\n");
                            CvInvoke.Line(szurkeSzinben, contours[i][hull[j]], contours[i][hull[j + 1]], new MCvScalar(0, 0, 155), 2); //vonalak meghuzasa
                        }
                        else
                        {
                            //CvInvoke.Circle(eredeti, contours[i][hull[j]], 5, new MCvScalar(200, 0, 50), 2); //pontok kirajzolása
                            CvInvoke.Line(szurkeSzinben, contours[i][hull[j]], contours[i][hull[j + 1]], new MCvScalar(0, 0, 155), 2); //vonalak meghuzasa
                        }
                        //textBox1.AppendText(j + ". X: " + x.ToString() + " Y: " + y.ToString() + "\r\n");
                    }
                }
            }
            if (szamlalo == 2)
            {
                //textBox1.AppendText("Olló" + "\r\n");
                ollo++;
            }
            else if (szamlalo == 5 ||szamlalo==4|| szamlalo == 6)
            {
                //textBox1.AppendText("Papír" + "\r\n");
                papir++;
            }
            else
            {
                //textBox1.AppendText("Kő" + "\r\n");
                ko++;
            }
            //textBox1.AppendText(szamlalo + "\r\n");
            pictureBox1.Image = szurkeSzinben.ToBitmap();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            reset();
            convexHull();
            if(ko==1)
            {
                label6.Text = "Kő";
            } else if(papir == 1)
            {
                label6.Text = "Papír";
            } else if(ollo == 1)
            {
                label6.Text = "Olló";
            }
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
           
        }

        public void reset()
        {
            szamlalo = 0;
            papir = 0;
            ko = 0;
            ollo = 0;
            atlag = 0;
            label6.Text = "";
            fileok = null;
            checkBox1.Checked = false;
            checkBox2.Checked = false;
            checkBox3.Checked = false;
            pictureBox1.Image = null;
            button6.Enabled = false;
            checkBox1.Enabled = false;
            checkBox2.Enabled = false;
            checkBox3.Enabled = false;
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            reset();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
            imgInput = new Image<Hsv, byte>(fileok[i]).Erode(2).Dilate(2);
            kep = new Image<Bgr, byte>(fileok[i]);
            eredeti = kep.Clone();
            pictureBox1.Image = kep.ToBitmap();
            i++;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            
            reset();
            checkBox1.Enabled = true;
            checkBox2.Enabled = true;
            checkBox3.Enabled = true;
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            
            if (dialog.ShowDialog() == DialogResult.OK)
            {
               
                //path = dialog.SelectedPath;
                fileok = Directory.GetFiles(dialog.SelectedPath);
                label4.Text = dialog.SelectedPath;
                //textBox1.Text = dialog.SelectedPath;
                //textBox1.Text = fileok.Length.ToString();

            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //reset();
            
            for(int j=0;j<fileok.Length;j++)
            {
                pictureBox1.Image = null;
                imgInput = new Image<Hsv, byte>(fileok[j]).Erode(2).Dilate(2);
                kep = new Image<Bgr, byte>(fileok[j]);
                eredeti = kep.Clone();
                pictureBox1.Image = kep.ToBitmap();
                convexHull();
            }
            
            //textBox1.Text = papir.ToString();
            if (checkBox1.Checked)
            {
                atlag = papir * 100 / fileok.Length;
            }
            else if (checkBox2.Checked)
            {
                atlag = ollo * 100 / fileok.Length;
            }
            else if (checkBox3.Checked) 
            {
                atlag = ko * 100 / fileok.Length;
            }
            
            //textBox1.Text = papir.ToString() + " " + fileok.Length.ToString() + "=" + atlag.ToString();
            label6.Text = atlag.ToString() + "%";
            button6.Enabled = false;
            
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            xtav = 20;
            ytav = 11;
            if (checkBox1.Checked)
            {
                button6.Enabled = true;
            }
            if (checkBox1.Checked)
            {
                atlag = papir * 100 / fileok.Length;
                label6.Text = atlag.ToString() + "%";
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                atlag = papir * 100 / fileok.Length;
            }
            else if (checkBox2.Checked)
            {
                atlag = ollo * 100 / fileok.Length;
            }
            else if (checkBox3.Checked)
            {
                atlag = ko * 100 / fileok.Length;
            }

            //textBox1.Text = papir.ToString() + " " + fileok.Length.ToString() + "=" + atlag.ToString();
            label6.Text = atlag.ToString() + "%";
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            xtav = 45;
            ytav = 27;
            if (checkBox2.Checked)
            {
                button6.Enabled = true;
            }
            if (checkBox2.Checked)
            {
                atlag = ollo * 100 / fileok.Length;
                label6.Text = atlag.ToString() + "%";
            }
           
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            xtav = 6;
            ytav = 2;
            if (checkBox3.Checked)
            {
                button6.Enabled = true;
            }
            if (checkBox3.Checked)
            {
                atlag = ko * 100 / fileok.Length;
                label6.Text = atlag.ToString() + "%";
            }
        }

        private void checkBox3_Click(object sender, EventArgs e)
        {
            
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
