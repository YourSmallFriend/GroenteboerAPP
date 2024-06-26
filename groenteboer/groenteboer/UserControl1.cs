using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace groenteboer
{
    public partial class UserControl1 : UserControl
    {
        public event Action<decimal> OnQuantityEntered;
        public event Action OnProductSelected;

        public UserControl1()
        {
            InitializeComponent();
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.Click += pictureBox1_Click; // Ensure this is hooked up
        }

        public void SetContent(string groenten, string plaatjePath, string prijs)
        {
            label1.Text = groenten;
            label2.Text = "€" + prijs + " kg ";

            // Check if the image path exists before setting the image
            if (System.IO.File.Exists(plaatjePath))
            {
                pictureBox1.Image = Image.FromFile(plaatjePath);
            }
            else
            {
                MessageBox.Show($"Image not found: {plaatjePath}");
            }
        }

        public void SetQuantity(decimal quantity)
        {
            OnQuantityEntered?.Invoke(quantity);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            OnProductSelected?.Invoke();
        }
    }
}
