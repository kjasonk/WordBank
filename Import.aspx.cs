﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.VisualBasic.FileIO;

namespace WordBank
{
    public partial class Import : System.Web.UI.Page
    {
        static SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["WordBank.Properties.Settings.ConnectionString"].ConnectionString);

        protected void Page_Load(object sender, EventArgs e)
        {

            if (Session["UsernameID"] == null)
            {
                Response.Redirect("Login.aspx");
            }
        }

        protected void UploadBtn_Click(object sender, EventArgs e)
        {
            connection.Open();
            DataTable DataTable = new DataTable();
            if (IsPostBack && Upload.HasFile)
            {
                var parser = new TextFieldParser(Upload.FileContent);
                parser.SetDelimiters(new string[] { "," });
                DataTable.Columns.Add("UserID");
                DataTable.Columns["UserID"].DefaultValue = Session["UserID"];
                DataTable.Columns.Add("Word");
                DataTable.Columns.Add("Definition");
                DataTable.Columns.Add("Sentence1");

                while (!parser.EndOfData)
                {
                    string[] fieldData = parser.ReadFields();
                    DataTable.Rows.Add(fieldData);
                }

                GridView.DataSource = DataTable;
                GridView.DataBind();

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = "WordBank";
                    try
                    {
                        SqlBulkCopyColumnMapping UserID = new SqlBulkCopyColumnMapping("UserID", "UserID");
                        bulkCopy.ColumnMappings.Add(UserID);

                        SqlBulkCopyColumnMapping Word = new SqlBulkCopyColumnMapping("Word", "Word");
                        bulkCopy.ColumnMappings.Add(Word);

                        SqlBulkCopyColumnMapping Definition = new SqlBulkCopyColumnMapping("Definition", "Definition");
                        bulkCopy.ColumnMappings.Add(Definition);

                        SqlBulkCopyColumnMapping Sentence1 = new SqlBulkCopyColumnMapping("Sentence1", "Sentence1");
                        bulkCopy.ColumnMappings.Add(Sentence1);

                        bulkCopy.WriteToServer(DataTable);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    
                }

            }
            else
            {
                Label1.Text = "YOU NEED TO SELECT A FILE";
            }
        }
        protected void Page_Unload(object sender, EventArgs e)
        {
            connection.Close();
        }
    }
}