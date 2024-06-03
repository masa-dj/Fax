using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Fax
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public int[,] bmpToMatrix(Bitmap bmp)
        {
            int[,] mat = new int[bmp.Height, bmp.Width]; // Note the dimensions
            Bitmap bw = Facsimile.toBW(bmp);
            for (int i = 0; i < bmp.Height; i++) // Iterate over rows
            {
                for (int j = 0; j < bmp.Width; j++) // Iterate over columns
                {
                    Color c = bw.GetPixel(j, i); // GetPixel(x, y)
                    if (c.R == 0) mat[i, j] = 1; // Black
                    else mat[i, j] = 0; // White
                }
            }
            return mat;
        }
        public Bitmap matrixToBMP(int[,] mat)
        {
            Bitmap b = new Bitmap(mat.GetLength(0), mat.GetLength(1));

            for (int i = 0; i < mat.GetLength(0); i++)
            {
                for (int j = 0; j < mat.GetLength(1); j++)
                {
                    if (mat[i, j] == 0) b.SetPixel(i, j, Color.FromArgb(255, 255, 255)); //bela
                    else b.SetPixel(i, j, Color.FromArgb(0, 0, 0)); //crna
                }
            }
            return b;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image |*.jpg;*.jpeg;*.png;*.bmp";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Bitmap bitmap = new Bitmap(openFileDialog.FileName);
                    int width = bitmap.Width;
                    int height = bitmap.Height;
                    pictureBox1.Image = bitmap;
                }
            }

            //Bitmap proba = new Bitmap(10, 10);

            //for (int i = 0; i < 10; i++)
            //{
            //    for (int j = 0; j < 10; j++)
            //    {
            //        int k = (i + j + 17 +i*j) % 2;
            //        if (k != 0) k = 255;
            //        proba.SetPixel(i, j, Color.FromArgb(k, k, k));
            //    }
            //}
            //pictureBox1.Image = proba;

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Bitmap bmp = (Bitmap)pictureBox1.Image;
            int[,] matrix = new int[bmp.Width, bmp.Height];
            matrix = bmpToMatrix(bmp);

            //StringBuilder sb = new StringBuilder();
            //for (int i = 0; i < matrix.GetLength(0); i++)
            //{
            //    for (int j = 0; j < matrix.GetLength(1); j++)
            //    {
            //        sb.Append(matrix[i, j].ToString());
            //        //if (j < matrix.GetLength(1) - 1)
            //        //{
            //        //    sb.Append(" "); // Add space between elements in the same row
            //        //}

            //    }
            //    sb.AppendLine(); // Add a new line after each row
            //}
            //textBox1.Text = sb.ToString();

            Facsimile f = new Facsimile();

            string code = f.scanDocument(matrix);
            ////kodira tacno

            int[,] mat = f.readFax(code, bmp.Height, bmp.Width);
            Bitmap output = matrixToBMP(mat);
            pictureBox2.Image = output;
            //int k = 1;
            //k++;
            //int w = bmp.Width;
            //int h = bmp.Height;
            //string enc = f.sendFax(bmp);
            //MessageBox.Show("Sent!");
            //bool[,] b = f.DecodeBitmap(enc, w, h);
            //pictureBox2.Image.Dispose();
        }
    }
}