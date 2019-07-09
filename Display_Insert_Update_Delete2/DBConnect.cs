using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Display_Insert_Update_Delete2
{
    class DBConnect
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string table;
        private string uid;
        private string password;

        #region Constructor
        public DBConnect()
        {
            Initialize();
        }
        #endregion

        #region Initialize values
        private void Initialize()
        {
            server = "localhost";
            database = "db_images";
            table = "myimages";
            uid = "root";
            password = "";
            string connectionString;

            connectionString =
                "SERVER=" + server + ";"
                + "DATABASE=" + database + ";"
                + "UID=" + uid + ";"
                + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
        }

        #endregion

        #region Open connection to database
        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException exc)
            {
                //When handling errors, you can your application's response based 
                //on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (exc.Number)
                {
                    case 0:
                        MessageBox.Show("Cannot connect to server. Contact administrator.");
                        break;
                    case 1045:
                        MessageBox.Show("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }
        #endregion

        #region Close connection
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException exc)
            {
                MessageBox.Show(exc.Message);
                return false;
            }
        }
        #endregion
    
        #region Insert statement
        public void Insert(string ID, string Name, string Description, byte[] Image)
        { 
            //Create sql query
           string query = "INSERT INTO " + table + "(ID, Name, Description, Image) VALUES(@id,@name,@desc,@img)";

            //Open connection
            if (this.OpenConnection())
            {
                //Create command and asign the query and connection from the constructor
                MySqlCommand command = new MySqlCommand(query, connection);
                //Create parameters to insert into db
                command.Parameters.Add("@id", MySqlDbType.VarChar).Value = ID;
                command.Parameters.Add("@name", MySqlDbType.VarChar).Value = Name;
                command.Parameters.Add("@desc", MySqlDbType.VarChar).Value = Description;
                command.Parameters.Add("@img", MySqlDbType.Blob).Value = Image;
                //Execute command
                command.ExecuteNonQuery();
                //Close connection
                this.CloseConnection();

                
            }
            
        }
        #endregion

        #region Update statement
        public void Update()
        {

        }
        #endregion

        #region Delete statement
        public void Delete()
        {

        }
        #endregion

        #region Select statement 
        //public List<string>[] Select()
        //{

        //}
        #endregion

        #region Count statement
        //public int Count()
        //{

        //}
        #endregion

        #region Backup
        public void Backup()
        {

        }
        #endregion

        #region Restore 
        public void Restore()
        {

        }
        #endregion

        #region Tool methods
        public bool checkID(string ID)
        {
            //Make sql query
            string query = "SELECT * FROM " + table + " WHERE (ID = @id)";

            //Open connection
            if (this.OpenConnection())
            {
                //Create command and asign the query and connection from the constructor
                MySqlCommand command = new MySqlCommand(query, connection);
                //Create parameters
                command.Parameters.AddWithValue("@id", ID);
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    //Close connection
                    this.CloseConnection();
                    return true;
                }
                else
                {
                    //Close connection
                    this.CloseConnection();
                    return false;
                }


            }
            else
            {
                //Close connection
                this.CloseConnection();
                return false;
            }
            


        }
        #endregion
    }

}
