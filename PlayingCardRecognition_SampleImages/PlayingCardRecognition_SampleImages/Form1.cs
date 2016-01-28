using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

using AForge.Imaging.Filters;
using PlayingCardRecognition;
using System.IO;

namespace PlayingCardRecognition_SampleImages
{
    public partial class Form1 : Form
    {
        private Pen pen = new Pen(Brushes.Orange, 4); //is used for drawing rectangle around card
        private Font font = new Font("Tahoma", 15, FontStyle.Bold); //is used for writing string on card

        private CardRecognizer recognizer = new CardRecognizer();
        private Bitmap image;

        private string imagePath = @"Sample Images\";
        private List<string> images;
        private int i = 0;

        public Form1()
        {
            InitializeComponent();

            images = Directory.EnumerateFiles(imagePath, "*.png", SearchOption.TopDirectoryOnly).ToList();

            btn_Next_Click(this, null);
        }

        private void btn_Next_Click(object sender, EventArgs e)
        {
            if (i >= images.Count)
            {
                i = 0;
            }
            string imagePath = images[i];
            i++;

            this.LoadImage(imagePath);

            this.ProcessRecognition();
        }

        private void LoadImage(string imagePath)
        {
            this.image = Bitmap.FromFile(imagePath) as Bitmap;
            this.pb_loaded.Image = ResizeBitmap(this.image);
            lbl_FileName.Text = "File Name : " + imagePath;
        }

        private Bitmap ResizeBitmap(Bitmap bmp)
        {
            ResizeBilinear resizer = new ResizeBilinear(pb_loaded.Width, pb_loaded.Height);

            return resizer.Apply(bmp);
        }

        private void ProcessRecognition()
        {
            CardCollection cards = recognizer.Recognize(this.image);

            cardImagePanel.DrawImages(cards.ToImageList());

            txtCards.Clear();
            foreach (Card card in cards)
            {
                txtCards.AppendText(card.ToString() + Environment.NewLine);
            }

            //Draw Rectangle around cards and write card strings on card
            using (Graphics graph = Graphics.FromImage(image))
            {
                foreach (Card card in cards)
                {
                    graph.DrawPolygon(pen, card.Corners); //Draw a polygon around card
                    PointF point = CardRecognizer.GetStringPoint(card.Corners); //Find Top left corner
                    point.Y += 10;
                    graph.DrawString(card.ToString(), font, Brushes.White, point); //Write string on card
                }
            }
            this.pb_loaded.Image = ResizeBitmap(this.image);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
