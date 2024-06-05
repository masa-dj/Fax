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
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MessageBox.Show("This project illustrates how Facsimile coding works. When clicked 'Open', user can choose a picture from device and display it. When clicked on 'Send', the document gets 'sent', and user can see how scaned and sent document looks on the receiving end. Enjoy :)");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                Bitmap bmp = (Bitmap)pictureBox1.Image;
                Facsimile f = new Facsimile();
                string code = f.scanDocument(bmp);
                Bitmap output = f.readFax(code, bmp.Height, bmp.Width);
                pictureBox2.Image = output;
                MessageBox.Show("You have successfully sent a document!");
            }
            else MessageBox.Show("Please choose a document to be sent!");
        }
    }
}