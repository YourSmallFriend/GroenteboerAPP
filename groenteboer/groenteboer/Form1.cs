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
        private UserControl1 selectedProduct;
        private UserControl2 numPad;

        public Form1()
        {
            InitializeComponent();
            numPad = new UserControl2();
            numPad.OnNumberEntered += NumPad_OnNumberEntered;
            numPad.Visible = false; // Initially hide the numpad
            Controls.Add(numPad);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var groentenLijst = dbHelper.GetGroenten();

            foreach (var item in groentenLijst)
            {
                UserControl1 uc = new UserControl1();
                uc.SetContent(item.Item1, item.Item2, item.Item3);
                uc.Size = new Size(200, 150); // Set a size for the user control
                uc.OnProductSelected += () =>
                {
                    selectedProduct = uc;
                    numPad.Location = new Point(uc.Location.X + uc.Width, uc.Location.Y); // Position numpad next to the selected product
                    numPad.Visible = true; // Show the numpad
                };

                uc.OnQuantityEntered += (quantity) =>
                {
                    // Handle the quantity entered, e.g., update database or UI
                    Console.WriteLine($"Quantity for {item.Item1}: {quantity} gram");
                };

                // Add the user control to the FlowLayoutPanel
                flowLayoutPanel1.Controls.Add(uc);

                // Log to verify controls are added
                Console.WriteLine($"Added control for {item.Item1}");
            }

            // Force the form to refresh to ensure controls are displayed
            this.Refresh();
        }

        private void NumPad_OnNumberEntered(decimal quantity)
        {
            if (selectedProduct != null)
            {
                selectedProduct.SetQuantity(quantity);
                MessageBox.Show($"Aantal ingevoerd: {quantity} gram");
                numPad.Visible = false; // Hide the numpad after entering the quantity
                selectedProduct = null; // Reset selected product
            }
            else
            {
                MessageBox.Show("Selecteer eerst een product door op een afbeelding te klikken.");
            }
        }
    }
}