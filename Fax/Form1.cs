using System.Windows.Forms;

namespace Fax
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }



        private void button1_Click(object sender, EventArgs e)
        {
            //using (OpenFileDialog openFileDialog = new OpenFileDialog())
            //{
            //    openFileDialog.Filter = "Image |*.jpg;*.jpeg;*.png;*.bmp";
            //    if (openFileDialog.ShowDialog() == DialogResult.OK)
            //    {
            //        Bitmap bitmap = new Bitmap(openFileDialog.FileName);
            //        //pictureBox2.Image = RGBToYUV(bitmap);

            //        int width = bitmap.Width;
            //        int height = bitmap.Height;
            //        //int size = width * height * 24 / 8 / 1024;
            //        //MessageBox.Show("not bigger than " + size.ToString());
            //    }
            //}
            Bitmap proba = new Bitmap(10, 10);

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    int k = (i + j + 17 +i*j) % 2;
                    if (k != 0) k = 255;
                    proba.SetPixel(i, j, Color.FromArgb(k, k, k));
                }
            }
            pictureBox1.Image = proba;

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Bitmap bmp = (Bitmap)pictureBox1.Image;
            int[,] matrix = new int[3, 10]
           {
                { 0, 0, 0, 0, 1, 1, 0, 0, 0, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 0, 0, 1 },
                { 0, 1, 1, 1, 1, 1, 0, 1, 1, 0 }
           };

            Facsimile f = new Facsimile();
            string code = f.scanDocument(matrix);
            //kodira tacno

            int[,] mat = f.readFax(code, 3, 10);

            int k=1;
            k++;
            //int w = bmp.Width;
            //int h = bmp.Height;
            //string enc = f.sendFax(bmp);
            //MessageBox.Show("Sent!");
            //bool[,] b = f.DecodeBitmap(enc, w, h);
            //pictureBox2.Image.Dispose();
        }
    }
}