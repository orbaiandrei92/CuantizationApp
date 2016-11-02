using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Cuantizare
{
    public partial class Form1 : Form
    {
        private string[] folderFile = null;
        private int selected = 0;
        private int begin = 0;
        private int end = 0;

        Bitmap newBitmap;
        int [,] Y;
        int[,] YC;
        int[,] YC1;
        int W, H;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            /*chart1.ChartAreas["Hist"].AxisX.Minimum = 0;
            chart1.ChartAreas["Hist"].AxisX.Maximum = 255;
            chart1.ChartAreas["Hist"].AxisX.Interval = 1;
            chart1.ChartAreas["Hist"].AxisY.Minimum = 0;
            chart1.ChartAreas["Hist"].AxisY.Maximum = 1000;
            chart1.ChartAreas["Hist"].AxisY.Interval = 50;*/ 
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openfile = new OpenFileDialog();
            openfile.Filter = "jpg (*.jpg)|*.jpg|bmp (*.bmp)|*.bmp|png (*.png)|*.png";

            if (openfile.ShowDialog() == DialogResult.OK && openfile.FileName.Length > 0)
            {
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox1.Image = Image.FromFile(openfile.FileName);
                newBitmap = new Bitmap (openfile.FileName);
                //Matrice imagine
                W = newBitmap.Width;
                H = newBitmap.Height;
                Y = new int[W, H];
                //Gray image
                for (int x = 0; x < W; x++)
                {
                    for (int y = 0; y < H; y++)
                    {
                        Color originalColor = newBitmap.GetPixel(x, y);
                        int greyScale = (int)((originalColor.R * .3) + (originalColor.G * .59) + (originalColor.B * .11));
                        Y[x, y] = greyScale;
                        Color newColor = Color.FromArgb(greyScale, greyScale, greyScale);
                        newBitmap.SetPixel(x, y, newColor);
                    }
                }

                pictureBox1.Image = newBitmap;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.Filter = "jpg (*.jpg)|*.jpg|bmp (*.bmp)|*.bmp|png (*.png)|*.png";

            if (savefile.ShowDialog() == DialogResult.OK && savefile.FileName.Length > 0)
            {
                pictureBox1.Image.Save(savefile.FileName);
                pictureBox2.Image.Save(savefile.FileName);
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int lmax = 256;
            int[] histogram_r = new int[256];
            for (int i = 0; i < W; i++)
                for (int j = 0; j < H; j++)
                    histogram_r[Y[i, j]]++;

            chart1.Series.Add("Hist1");
            chart1.Series["Hist1"].Color = Color.Black;
             
            for (int i = 0; i < lmax; i++)
                chart1.Series["Hist1"].Points.AddXY(i, histogram_r[i]);
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Cuantizare uniforma
            int b = Convert.ToInt32(textBox1.Text);
            int l = (int) Math.Pow(2, b);// l=2^b
            int lmin = 0;
            int lmax = 256;
            int q;
           
            YC = new int[W, H];
            
            Bitmap imgUniform = new Bitmap(newBitmap.Width, newBitmap.Height);
            imgUniform.SetResolution(newBitmap.Width , newBitmap.Height);
                      
            q =(int) (lmax - lmin) / l;
            int[] t = new int[l+1];//intervale constructie
            int[] r = new int[l];//intervale reconstructie
            int [] uc = new int [lmax];

            t[0] = lmin;
            t[l] = lmax;
            //tj,rj
            for (int j = 0; j < l; j++)
                {
                    r[j] = t[j] + q / 2;                   
                    t[j+1] = t[j] + q;
                }
         
            for (int i = 0; i < l; i++)                            
                for (int j = t[i]; j<t[i+1]; j++)
                    uc[j] = r[i];
                                                                    
            for (int i = 0; i < W; i++)
            {                        
                for (int j = 0; j < H; j++)
                {
                    YC[i,j] = uc[Y[i,j]];
                    Color newColor = Color.FromArgb(YC[i,j], YC[i,j], YC[i,j]);
                    imgUniform.SetPixel(i, j, newColor);                              
                    }                         
                }
                                               
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;        
            pictureBox2.Image = imgUniform;

            //chart1.Series.Add("Nivel unif");
            for (int i = 0; i < lmax; i++)
                   chart1.Series["Nivel unif"].Points.AddXY(i, uc[i]);
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            int lmax = 256;
            int[] histogram_r = new int[256];
            for (int i = 0; i < W; i++)
                for (int j = 0; j < H; j++)
                    histogram_r[YC[i, j]]++;

            chart1.Series.Add("Hist2");
            chart1.Series["Hist1"].Color = Color.Black;

            for (int i = 0; i < lmax; i++)
                chart1.Series["Hist2"].Points.AddXY(i, histogram_r[i]);  
        }

        private void button7_Click(object sender, EventArgs e)
        {
           
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //Cuantizare optimala
            int b = Convert.ToInt32(textBox1.Text);
            int l = (int)Math.Pow(2, b);
            int lmin = 0;
            int lmax = 256;
            int q;
            int[] histogram_r = new int[256];
            YC1 = new int[W, H];
            int[] uc1 = new int[lmax];

            Bitmap imgUniform1 = new Bitmap(W, H);
            imgUniform1.SetResolution(W,H);

            q = (int)(lmax - lmin) / l;
            int[] t = new int[l + 1];//intervale constructie
            int[] r = new int[l];//intervale reconstructie
            
            int[] uc = new int[lmax];
            int[] tn = new int[l + 1];

            t[0] = lmin;
            t[l] = lmax;
            //tj,rj
            for (int j = 0; j < l; j++)
            {
                r[j] = t[j] + q / 2;
                t[j + 1] = t[j] + q;
            }
                        
            //Hist
            for (int i = 0; i < W; i++)           
                for (int j = 0; j < H; j++)
                    histogram_r[Y[i, j]]++;
                            
            bool cond = true;
            tn[0] = 0;
            tn[l] = 256;
            //int ru=0;
            //int rd = 0;
            do 
            {
                cond = false;
                for (int i = 0; i < l; i++)
                {
                    int ru = 0;
                    int rd = 0;
                    for (int j = t[i]; j < t[i + 1]; j++)
                    {
                        ru += j * histogram_r[j];
                        rd += histogram_r [j];
                        
                    }
                    r[i]=ru/rd;  //--                 
                }
                
               //  for rescri t [i] = (r[i]+

                for (int i = 1; i < l; i++)
                {
                    tn[i] = (int) ((r[i - 1] + r[i]) / 2);

                    if (Math.Abs(tn[i] - t[i])>5)
                        cond = true;
                }
                for (int i = 1; i < l; i++)
                    t[i] = tn[i];
                
                // verif tn != tv => cond = false;
                int kkkk = 0;
          }while(cond == true);

            for (int i = 0; i < l; i++)
                for (int j = t[i]; j < t[i + 1]; j++)
                    uc1[j] = r[i];

            for (int i = 0; i < W; i++)
            {
                for (int j = 0; j < H; j++)
                {
                    YC1[i, j] = uc1[Y[i, j]];
                    Color newColor = Color.FromArgb(YC1[i, j], YC1[i, j], YC1[i, j]);
                    imgUniform1.SetPixel(i, j, newColor);
                }
            }

            /*for (int j = 0; j < l; j++)
                uc1[j] = uc[j];*/

            //chart1.Series.Add("Nivel opt");
            for (int i = 0; i < lmax; i++)
                chart1.Series["Nivel opt"].Points.AddXY(i, uc1[i]);
            
                      
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.Image = imgUniform1;
        }

        private void chart1_Click(object sender, EventArgs e)
        {            
                          
        }

        private void button8_Click(object sender, EventArgs e)
        {
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int lmax = 256;
            int[] histogram_r = new int[256];
            for (int i = 0; i < W; i++)
                for (int j = 0; j < H; j++)
                    histogram_r[YC1[i, j]]++;

            chart1.Series.Add("Hist3");
            chart1.Series["Hist2"].Color = Color.Black;

            for (int i = 0; i < lmax; i++)
                chart1.Series["Hist3"].Points.AddXY(i, histogram_r[i]); 
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            //Change the path to location where your images are stored.
            DirectoryInfo directory = new DirectoryInfo(@"D:\Licenta\Img\");
            if (directory != null)
            {
                FileInfo[] files = directory.GetFiles();
                CombineImages(files);
            }
        }
        private void CombineImages(FileInfo[] files)
        {
            try
            {
                //change the location to store the final image.
                string finalImage = @"D:\Licenta\7.jpg";
                List<int> imageHeights = new List<int>();
                int nIndex = 0;
                int width = 0;
                foreach (FileInfo file in files)
                {
                    Image img = Image.FromFile(file.FullName);
                    imageHeights.Add(img.Height);
                    width += img.Width;
                    img.Dispose();
                }
                imageHeights.Sort();
                int height = imageHeights[imageHeights.Count - 1];
                Bitmap img3 = new Bitmap(width, height);
                Graphics g = Graphics.FromImage(img3);
                g.Clear(SystemColors.AppWorkspace);
                foreach (FileInfo file in files)
                {
                    Image img = Image.FromFile(file.FullName);
                    if (nIndex == 0)
                    {
                        g.DrawImage(img, new Point(0, 0));
                        nIndex++;
                        width = img.Width;
                    }
                    else
                    {
                        g.DrawImage(img, new Point(width, 0));
                        width += img.Width;
                    }
                    img.Dispose();
                }
                g.Dispose();
                img3.Save(finalImage, System.Drawing.Imaging.ImageFormat.Jpeg);
                img3.Dispose();
                pictureBox1.Image = Image.FromFile(finalImage);
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            }
            catch
            {

            }
        }
       

        
    }
    
}
