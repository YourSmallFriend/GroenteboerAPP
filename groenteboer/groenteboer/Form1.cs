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
using System.Diagnostics.PerformanceData;

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
        private ExtraForm extraForm;

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
                UpdateSecondFormDataGridView();
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
                UpdateSecondFormDataGridView();
            }
        }

        private void OpenFormOnSecondScreen()
        {
            // Get all screens
            Screen[] screens = Screen.AllScreens;

            // Check if there is more than one screen
            if (screens.Length > 1)
            {
                // Prompt the user to select the main screen for Form1
                DialogResult result = MessageBox.Show("Is dit je hoofdscherm?", "Hoofdscherm selectie", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                // Initialize Form1 on the selected main screen
                if (result == DialogResult.Yes)
                {
                    Screen mainScreenForm1 = screens[0];
                    this.StartPosition = FormStartPosition.Manual;
                    this.Location = mainScreenForm1.Bounds.Location;
                    this.WindowState = FormWindowState.Maximized;
                }
                else
                {
                    Screen mainScreenForm1 = screens[1];
                    this.StartPosition = FormStartPosition.Manual;
                    this.Location = mainScreenForm1.Bounds.Location;
                    this.WindowState = FormWindowState.Maximized;
                }

                // Initialize ExtraForm on the other screen
                Screen mainScreenExtraForm = (result == DialogResult.Yes) ? screens[1] : screens[0];
                extraForm = new ExtraForm();
                extraForm.StartPosition = FormStartPosition.Manual;
                extraForm.Location = mainScreenExtraForm.Bounds.Location;
                extraForm.WindowState = FormWindowState.Maximized;
                extraForm.Show();
            }
            else
            {
                // Only one screen detected, use default behavior
                MessageBox.Show("Slechts één scherm gedetecteerd. Verbind een tweede scherm om deze functie te gebruiken.", "Geen tweede scherm", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.WindowState = FormWindowState.Maximized;
            }
        }



        private void UpdateSecondFormDataGridView()
        {
            if (extraForm != null)
            {
                extraForm.SetData(dataGridView1);
            }
        }

        // Define the ExtraForm class within the same file
        public class ExtraForm : Form
        {
            public DataGridView dataGridView2 { get; private set; }

            public ExtraForm()
            {
                InitializeComponent();

                // Initialize DataGridView
                dataGridView2 = new DataGridView();
                dataGridView2.Dock = DockStyle.Fill;
                dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Columns fill the available width

                // Add DataGridView to the form's controls
                this.Controls.Add(dataGridView2);

                // Handle form load event to adjust column widths after data is set
                this.Load += ExtraForm_Load;
            }

            // Method to set DataGridView data
            public void SetData(DataGridView dataGridView1)
            {
                // Clear existing columns and rows
                dataGridView2.Columns.Clear();
                dataGridView2.Rows.Clear();

                // Copy columns
                foreach (DataGridViewColumn column in dataGridView1.Columns)
                {
                    dataGridView2.Columns.Add((DataGridViewColumn)column.Clone());
                }

                // Copy rows
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        DataGridViewRow newRow = new DataGridViewRow();
                        newRow.CreateCells(dataGridView2);

                        for (int i = 0; i < row.Cells.Count; i++)
                        {
                            newRow.Cells[i].Value = row.Cells[i].Value;
                        }

                        dataGridView2.Rows.Add(newRow);
                    }
                }

                // Auto resize columns after data is set
                dataGridView2.AutoResizeColumns();
            }

            // Adjust column widths on form load
            private void ExtraForm_Load(object sender, EventArgs e)
            {
                dataGridView2.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
            }

            // InitializeComponent method for completeness (you might want to use your designer-generated method)
            private void InitializeComponent()
            {
                this.SuspendLayout();
                // 
                // ExtraForm
                // 
                this.ClientSize = new System.Drawing.Size(800, 450);
                this.Name = "ExtraForm";
                this.ResumeLayout(false);
            }
        }
    }
}