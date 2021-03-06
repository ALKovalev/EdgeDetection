using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Filters
{
    public partial class Form1 : Form
    {
        private Bitmap image;
        private Stack<Bitmap> oldImagesStack = new Stack<Bitmap>();
        private Stack<Bitmap> newImagesStack = new Stack<Bitmap>();

        private int MMkWidth = 3;
        private int MMkHeight = 3;
        private int[,] MMkMatrix = { { 1, 1, 1 },
                                     { 1, 1, 1 },
                                     { 1, 1, 1 } };



        public Form1()
        {
            InitializeComponent();
        }


        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files | *.png; *.jpg; *.bmp | All files (*.*) | *.*";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                image = new Bitmap(dialog.FileName);
                pictureBox1.Image = image;
                pictureBox1.Refresh();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Bitmap newImage = ((Filters)e.Argument).processImage(image, backgroundWorker1);
            if (backgroundWorker1.CancellationPending != true)
            {
                image = newImage; 
            }
                
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                oldImagesStack.Push(new Bitmap(pictureBox1.Image));
                pictureBox1.Image = image;
                pictureBox1.Refresh();
            }

            progressBar1.Value = 0;
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog
            {
                Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif",
                Title = "Save an Image File"
            };
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                System.IO.FileStream fs =
                    (System.IO.FileStream)saveFileDialog1.OpenFile();

                switch (saveFileDialog1.FilterIndex)
                {
                    case 1:
                        image.Save(fs,
                           System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;

                    case 2:
                        image.Save(fs,
                           System.Drawing.Imaging.ImageFormat.Bmp);
                        break;

                    case 3:
                        image.Save(fs,
                           System.Drawing.Imaging.ImageFormat.Gif);
                        break;
                }

                fs.Close();
            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        private void StepBack_Click(object sender, EventArgs e)
        {
            if (oldImagesStack.Count > 0)
            {
                newImagesStack.Push(image);
                image = oldImagesStack.Pop();
                pictureBox1.Image = image;
                pictureBox1.Refresh();
            }
        }

        private void StepForward_Click(object sender, EventArgs e)
        {
            if (newImagesStack.Count > 0)
            {
                oldImagesStack.Push(image);
                image = newImagesStack.Pop();
                pictureBox1.Image = image;
                pictureBox1.Refresh();
            }
        }

        private void собеляToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new SobelFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void щарраToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new ScharraFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void прюиттаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new PrewittFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void растяжениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new DilationFilter(MMkWidth, MMkHeight, MMkMatrix);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void сужениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new ErosionFilter(MMkWidth, MMkHeight, MMkMatrix);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void закрытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new ClosingFilter(MMkWidth, MMkHeight, MMkMatrix);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void раскрытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new OpeningFilter(MMkWidth, MMkHeight, MMkMatrix);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void градиентToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new MMgradFilter(MMkWidth, MMkHeight, MMkMatrix);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        public void UpdateKernel(int width, int height, int[,] matrix)
        {
            MMkWidth = width;
            MMkHeight = height;
            MMkMatrix = matrix;
        }

        private void изменитьструктурныйэлементToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MathMorphKernelEditor form = new MathMorphKernelEditor(MMkWidth, MMkHeight, MMkMatrix, this);
            form.Show();
        }
    }
}
