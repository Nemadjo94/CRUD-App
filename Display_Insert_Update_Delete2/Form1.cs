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
using System.IO;
using System.Drawing.Imaging;

namespace Display_Insert_Update_Delete2
{
    public partial class Form1 : Form
    {
        DBConnect db = new DBConnect();

        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void BTN_CHOOSE_IMAGE_Click(object sender, EventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.Filter = "Choose Image File(*.jpg;*.png;*.gif)|*.jpg;*.png;*.gif";

            if (opf.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(opf.FileName);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        MySqlConnection connection = new MySqlConnection("datasource=localhost;port=3306;Initial Catalog='db_images';username=root;password=");

        private void Form1_Load(object sender, EventArgs e)
        {
            FilLDGV();
        }

        public void FilLDGV()
        {
            //Get data from mySql table and fill in a table
            MySqlCommand command = new MySqlCommand("SELECT ID,Name,Description, Image FROM myimages", connection);
            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            DataTable table = new DataTable();
            adapter.Fill(table);
            //Change the height of each row
            dataGridView1.RowTemplate.Height = 60;
            //Removes the empty row at the end 
            dataGridView1.AllowUserToAddRows = false;
            //Make the row data read only(disables editing)
            dataGridView1.ReadOnly = true;
            //Display table data
            dataGridView1.DataSource = table;
            //Make image column stretch over its field
            DataGridViewImageColumn imgCol = new DataGridViewImageColumn();
            imgCol = (DataGridViewImageColumn)dataGridView1.Columns[3];
            imgCol.ImageLayout = DataGridViewImageCellLayout.Stretch;
            //Maka table fill the data grid
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;


        }
        public void SearchFilLDGV(string valueToSearch)
        {
            //Get data from mySql table and fill in a table
            MySqlCommand command = new MySqlCommand("SELECT ID,Name,Description, Image FROM myimages WHERE CONCAT(ID, Name, Description) LIKE '%" + valueToSearch + "%'", connection);
            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            DataTable table = new DataTable();
            adapter.Fill(table);
            //Change the height of each row
            dataGridView1.RowTemplate.Height = 60;
            //Removes the empty row at the end 
            dataGridView1.AllowUserToAddRows = false;
            //Make the row data read only(disables editing)
            dataGridView1.ReadOnly = true;
            //Display table data
            dataGridView1.DataSource = table;
            //Make image column stretch over its field
            DataGridViewImageColumn imgCol = new DataGridViewImageColumn();
            imgCol = (DataGridViewImageColumn)dataGridView1.Columns[3];
            imgCol.ImageLayout = DataGridViewImageCellLayout.Stretch;
            //Maka table fill the data grid
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;


        }
        public void ClearFields()
        {
            textBoxID.Clear();
            textBoxName.Clear();
            textBoxDesc.Clear();

            pictureBox1.Image = null;
        }
        //Click the row in grid and display the image in the image box
        private void dataGridView1_Click(object sender, EventArgs e)
        {

            //Grab the picture from the 3rd row
            Byte[] img = (Byte[])dataGridView1.CurrentRow.Cells[3].Value;

            //Store the picture in stream
            MemoryStream ms = new MemoryStream(img);

            //Load the pic from stream
            pictureBox1.Image = Image.FromStream(ms);

            //Show text in text boxes
            textBoxID.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            textBoxName.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            textBoxDesc.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
        }
        //On click insert values into database
        private void BTN_INSERT_Click(object sender, EventArgs e)
        {

            if (textBoxID.Text.Equals(null) || textBoxName.Text.Equals(null) || textBoxDesc.Text.Equals(null) || pictureBox1.Image == null)
            {
                MessageBox.Show("Please fill out all the fields!");
            }
            else
            {
                //Using method from DBConnect class check if ID already exists in db
                if (db.checkID(textBoxID.Text))
                {
                    MessageBox.Show("Cannot insert new item because ID already exists in database!");                    
                }
                else
                {  
                    //Get the picture from picture box and store it as bytes into var because of the blob in db
                    MemoryStream ms = new MemoryStream();
                    pictureBox1.Image.Save(ms, pictureBox1.Image.RawFormat);
                    byte[] img = ms.ToArray();

                    //Insert values using method from DBConnect class
                    db.Insert(textBoxID.Text, textBoxName.Text, textBoxDesc.Text, img);

                    //Refreshes the grid after update
                    FilLDGV();
                    //Clear the fields after update
                    ClearFields();
                }
            }
        }
        //Message box
        public void ExecMyQuery(MySqlCommand mcomd, string myMsg)
        {
            connection.Open();
            if (mcomd.ExecuteNonQuery() == 1)
            {

                MessageBox.Show(myMsg);
            }
            else
            {
                MessageBox.Show("Query Not Executed");
            }

            connection.Close();
        }

        private void BTN_UPDATE_Click(object sender, EventArgs e)
        {
            if (textBoxID.Text.Equals(null) && textBoxName.Text.Equals(null) && textBoxDesc.Text.Equals(null) || pictureBox1.Image == null)
            {
                MessageBox.Show("Please select an item to update!");
            } 
            else if (textBoxID.Text.Equals(null) || textBoxName.Text.Equals(null) || textBoxDesc.Text.Equals(null) || pictureBox1.Image == null)
            { 
               MessageBox.Show("Please fill out all the fields!");
            }
            else
            {
                MemoryStream ms = new MemoryStream();
                pictureBox1.Image.Save(ms, pictureBox1.Image.RawFormat);
                byte[] img = ms.ToArray();

                MySqlCommand command = new MySqlCommand("UPDATE myimages SET Name=@name, Description=@desc, Image=@img WHERE ID = @id", connection);

                command.Parameters.Add("@id", MySqlDbType.VarChar).Value = textBoxID.Text;
                command.Parameters.Add("@name", MySqlDbType.VarChar).Value = textBoxName.Text;
                command.Parameters.Add("@desc", MySqlDbType.VarChar).Value = textBoxDesc.Text;
                command.Parameters.Add("@img", MySqlDbType.Blob).Value = img;

                ExecMyQuery(command, "Data Updated");
                //Refreshes the grid after update
                FilLDGV();
                ClearFields();
            }
        }

        private void BTN_DELETE_Click(object sender, EventArgs e)
        { 
            MySqlCommand command = new MySqlCommand("DELETE FROM myimages WHERE ID = @id", connection);

            command.Parameters.Add("@id", MySqlDbType.VarChar).Value = textBoxID.Text;

            ExecMyQuery(command, "Data Deleted");
            //Refreshes the grid after update
            FilLDGV();
            ClearFields();
        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            //Search db by whats in the txt box
            SearchFilLDGV(textBoxSearch.Text);
        }

        private void BTN_FIND_Click(object sender, EventArgs e)
        {
            MySqlCommand command = new MySqlCommand("SELECT ID,Name,Description, Image FROM myimages WHERE ID = @id", connection);
            command.Parameters.Add("@id", MySqlDbType.VarChar).Value = textBoxID.Text;
            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            DataTable table = new DataTable();
            adapter.Fill(table);

            if(table.Rows.Count <= 0)
            {
                MessageBox.Show("No Data Found!");
                ClearFields();
            }
            else
            {
                textBoxID.Text = table.Rows[0][0].ToString();
                textBoxName.Text = table.Rows[0][1].ToString();
                textBoxDesc.Text = table.Rows[0][2].ToString();

                byte[] img = (byte[])table.Rows[0][3];
                MemoryStream ms = new MemoryStream(img);
                pictureBox1.Image = Image.FromStream(ms);

            }
        }

        private void BTN_CLEAR_Click(object sender, EventArgs e)
        {
            ClearFields();
        }
    }
}
