using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace groenteboer
{
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        public void SetContent(string groenten, string plaatjePath, string prijs)
        {
            label1.Text = groenten;
            pictureBox1.Image = Image.FromFile(plaatjePath);
            label2.Text = "€" + prijs + " per kilo";
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }
    }
}