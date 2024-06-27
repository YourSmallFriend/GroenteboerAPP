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
        private decimal totalPrice = 0m;
        private int totalRowIndex;
        private int selectedRowIndex = -1;

        public Form1()
        {
            InitializeComponent();
            userControl21.OnNumberEntered += NumPad_OnNumberEntered;
            button1.Click += Button1_Click;
            button1.Visible = false; // Hide the delete button initially
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            OpenFormOnSecondScreen();

            var groentenLijst = dbHelper.GetGroenten();

            foreach (var item in groentenLijst)
            {
                UserControl1 uc = new UserControl1();
                uc.SetContent(item.Item1, item.Item2, item.Item3);
                uc.OnProductSelected += () =>
                {
                    selectedProduct = uc;
                    userControl21.Visible = true; // Show the numpad
                };

                // Add the user control to the FlowLayoutPanel
                flowLayoutPanel1.Controls.Add(uc);
            }

            // Initialize DataGridView columns
            dataGridView1.Columns.Add("Product", "Product");
            dataGridView1.Columns.Add("Aantal", "Aantal (gram)");
            dataGridView1.Columns.Add("Prijs", "Prijs (€)");

            // Add the initial total price row
            totalRowIndex = dataGridView1.Rows.Add("Totaal", "", totalPrice.ToString("0.00"));

            // Force the form to refresh to ensure controls are displayed
            this.Refresh();

            // Handle row click event
            dataGridView1.CellClick += DataGridView1_CellClick;
        }

        private void UpdateTotalPrice(decimal price)
        {
            totalPrice += price;
            dataGridView1.Rows[totalRowIndex].Cells[2].Value = totalPrice.ToString("0.00");
        }

        private void NumPad_OnNumberEntered(decimal quantity)
        {
            if (selectedProduct != null)
            {
                selectedProduct.SetQuantity(quantity);
                userControl21.Visible = false; // Hide the numpad after entering the quantity

                // Calculate the price based on the quantity and unit price
                string productName = selectedProduct.Controls["label1"].Text;
                decimal unitPrice = decimal.Parse(selectedProduct.Controls["label2"].Text);
                decimal totalPriceForProduct = (quantity / 1000) * unitPrice;

                bool productExists = false;

                // Check if the product already exists in the DataGridView
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells[0].Value != null && row.Cells[0].Value.ToString() == productName)
                    {
                        // Product exists, update quantity and price
                        decimal existingQuantity = decimal.Parse(row.Cells[1].Value.ToString());
                        decimal newQuantity = existingQuantity + quantity;
                        decimal newTotalPriceForProduct = (newQuantity / 1000) * unitPrice;

                        row.Cells[1].Value = newQuantity;
                        row.Cells[2].Value = newTotalPriceForProduct.ToString("0.00");

                        UpdateTotalPrice(totalPriceForProduct);
                        productExists = true;
                        break;
                    }
                }

                if (!productExists)
                {
                    // Insert the product details to the DataGridView above the total row
                    dataGridView1.Rows.Insert(totalRowIndex, productName, quantity, totalPriceForProduct.ToString("0.00"));

                    // Update the total row index
                    totalRowIndex++;

                    UpdateTotalPrice(totalPriceForProduct);
                }

                selectedProduct = null; // Reset selected product
            }
            else
            {
                MessageBox.Show("Selecteer eerst een product door op een afbeelding te klikken.");
            }
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex != totalRowIndex)
            {
                selectedRowIndex = e.RowIndex;
                DataGridViewRow row = dataGridView1.Rows[selectedRowIndex];
                button1.Visible = true;
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (selectedRowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRowIndex];
                decimal rowPrice = decimal.Parse(row.Cells[2].Value.ToString());

                // Update the total price by subtracting the price of the removed product
                UpdateTotalPrice(-rowPrice);

                // Remove the selected row
                dataGridView1.Rows.RemoveAt(selectedRowIndex);

                // Update the total row index
                totalRowIndex--;

                // Hide the delete button
                button1.Visible = false;
                selectedRowIndex = -1;
            }
        }

        private void OpenFormOnSecondScreen()
        {
            // Get all screens
            Screen[] screens = Screen.AllScreens;

            // Check if there is more than one screen
            if (screens.Length > 1)
            {
                // Get the second screen (assuming second screen is the second in the array)
                Screen secondScreen = screens[1];

                // Create an instance of ExtraForm
                Form extraForm = new Form();

                // Set the location of the extra form to the second screen
                extraForm.StartPosition = FormStartPosition.Manual;
                extraForm.Location = new Point(secondScreen.Bounds.Left, secondScreen.Bounds.Top);

                // Optionally maximize the form to fit the screen
                extraForm.WindowState = FormWindowState.Maximized;

                // Show the extra form
                extraForm.Show();
            }
            else
            {
                MessageBox.Show("Only one screen detected. Please connect a second screen to use this feature.");
            }
        }
    }
}