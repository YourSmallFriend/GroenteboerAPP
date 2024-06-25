using System;
using System.Windows.Forms;

namespace groenteboer
{
    public partial class UserControl2 : UserControl
    {
        public event Action<decimal> OnNumberEntered;
        private string currentInput = string.Empty;
        private bool isKilo = true; // Default to kilo

        public UserControl2()
        {
            InitializeComponent();
        }

        private void AppendNumber(string number)
        {
            currentInput += number;
            textBox1.Text = currentInput;
        }

        private void btn1_Click(object sender, EventArgs e) => AppendNumber("1");
        private void btn2_Click(object sender, EventArgs e) => AppendNumber("2");
        private void btn3_Click(object sender, EventArgs e) => AppendNumber("3");
        private void btn4_Click(object sender, EventArgs e) => AppendNumber("4");
        private void btn5_Click(object sender, EventArgs e) => AppendNumber("5");
        private void btn6_Click(object sender, EventArgs e) => AppendNumber("6");
        private void btn7_Click(object sender, EventArgs e) => AppendNumber("7");
        private void btn8_Click(object sender, EventArgs e) => AppendNumber("8");
        private void btn9_Click(object sender, EventArgs e) => AppendNumber("9");
        private void btn0_Click(object sender, EventArgs e) => AppendNumber("0");
        private void btn00_Click(object sender, EventArgs e) => AppendNumber("00");

        private void btncom_Click(object sender, EventArgs e)
        {
            if (!currentInput.Contains(","))
            {
                if (string.IsNullOrEmpty(currentInput))
                {
                    currentInput = "0,";
                }
                else
                {
                    currentInput += ",";
                }
                textBox1.Text = currentInput;
            }
        }

        private void btndel_Click(object sender, EventArgs e)
        {
            if (currentInput.Length > 0)
            {
                currentInput = currentInput.Substring(0, currentInput.Length - 1);
                textBox1.Text = currentInput;
            }
        }

        private void btnGram_Click(object sender, EventArgs e)
        {
            isKilo = false;
            textBox1.Text = $"{currentInput} g";
        }

        private void btnKilo_Click(object sender, EventArgs e)
        {
            isKilo = true;
            textBox1.Text = $"{currentInput} kg";
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            if (decimal.TryParse(currentInput, out decimal result))
            {
                decimal finalAmount = isKilo ? result * 1000 : result; // Convert to grams if it's in kilos
                OnNumberEntered?.Invoke(finalAmount);
            }
            currentInput = string.Empty;
            textBox1.Text = currentInput;
        }
    }
}
