using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using MySql.Data;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace groenteboer
{
    public partial class Form1 : Form
    {
        private const string ConnectionString = "Server=localhost;Database=groentefruit;user=root;";
        DatabaseHelper dbHelper = new DatabaseHelper(ConnectionString);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var groentenLijst = dbHelper.GetGroenten();

            foreach (var item in groentenLijst)
            {
                UserControl1 uc = new UserControl1();
                uc.SetContent(item.Item1, item.Item2);
                uc.Size = new Size(200, 200); // Set a size for the user control

                flowLayoutPanel1.Controls.Add(uc);
            }
        }
    }
}