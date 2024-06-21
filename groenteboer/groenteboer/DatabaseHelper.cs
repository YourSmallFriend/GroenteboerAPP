using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace groenteboer
{
    public class DatabaseHelper
    {
        private string ConnectionString { get; }

        public DatabaseHelper(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public List<Tuple<string, string>> GetGroenten()
        {
            var groentenLijst = new List<Tuple<string, string>>();

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                string query = "SELECT * FROM `fruitgroente`";
                MySqlCommand command = new MySqlCommand(query, connection);

                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var imgPath = reader["plaatje"].ToString();
                        var groenten = reader["groenten/fruit"].ToString();
                        groentenLijst.Add(new Tuple<string, string>(groenten, imgPath));
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

            return groentenLijst;
        }
    }
}