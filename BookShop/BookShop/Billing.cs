﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BookShop
{
    public partial class Billing : Form
    {
        public Billing()
        {
            InitializeComponent();
            populate();
        }
        SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\USER\OneDrive\Dokumen\BookShopDb.mdf;Integrated Security=True;Connect Timeout=30");
        private void populate()
        {
            conn.Open();
            string query = "select * from BookTbl";
            SqlDataAdapter sda = new SqlDataAdapter(query, conn);
            SqlCommandBuilder builder = new SqlCommandBuilder(sda);
            var ds = new DataSet();
            sda.Fill(ds);
            BookDGV.DataSource = ds.Tables[0];
            conn.Close();
        }
        int n = 0, GrdTotal=0;
        private void SaveBtn_Click(object sender, EventArgs e)
        {
            
            if(QtyTb.Text == "" || Convert.ToInt32(QtyTb.Text) > stock)
            {
                MessageBox.Show("No Enough Stock");
            }
            else
            {
                int total = Convert.ToInt32(QtyTb.Text) * Convert.ToInt32(PriceTb.Text);
                DataGridViewRow newRow = new DataGridViewRow();
                newRow.CreateCells(BillDGV);
                newRow.Cells[0].Value = n + 1;
                newRow.Cells[1].Value = BTitleTb.Text;
                newRow.Cells[2].Value = QtyTb.Text;
                newRow.Cells[3].Value = PriceTb.Text;
                newRow.Cells[4].Value = total;
                BillDGV.Rows.Add(newRow);
                n++;
                UpdateBook();
                GrdTotal = GrdTotal + total;
                TotalLbl.Text = "Rp. " + GrdTotal; 
            }
        }
        private void UpdateBook()
        {
            int newQty = stock - Convert.ToInt32(QtyTb.Text);
            try
            {
                conn.Open();
                string query = "update BookTbl set BQty=" + newQty + "where BId=" + key + ";";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                //MessageBox.Show("Book Updated Successfully");
                conn.Close();
                populate();
                //Reset();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }
        int key = 0,stock = 0;
        private void BookDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            BTitleTb.Text = BookDGV.SelectedRows[0].Cells[1].Value.ToString();
            //QtyTb.Text = BookDGV.SelectedRows[0].Cells[4].Value.ToString();
            PriceTb.Text = BookDGV.SelectedRows[0].Cells[5].Value.ToString();

            if (BTitleTb.Text == "")
            {
                key = 0;
                stock = 0;
            }
            else
            {
                key = Convert.ToInt32(BookDGV.SelectedRows[0].Cells[0].Value.ToString());
                stock = Convert.ToInt32(BookDGV.SelectedRows[0].Cells[4].Value.ToString());
            }
        }
        int prodid, prodqty, prodprice, total, pos = 60;

        private void Billing_Load(object sender, EventArgs e)
        {
            UserNameLbl.Text = Login.UserName;
        }

        private void label10_Click(object sender, EventArgs e)
        {
            Login obj = new Login();
            obj.Show();
            this.Hide();
        }

        private void PrintBtn_Click(object sender, EventArgs e)
        {

            if (ClientNameTb.Text == "" || BTitleTb.Text == "")
            {
                MessageBox.Show("Select Client Name");
            }
            else
            {
                try
                {
                    conn.Open();
                    string query = "insert into BillTbl Values('" + UserNameLbl.Text + "','" + ClientNameTb.Text + "'," + GrdTotal + ")";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Bill Saved Successfully");
                    conn.Close();
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.Message);
                }

                printDocument1.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("pprnm", 285, 600);
                if (printPreviewDialog1.ShowDialog() == DialogResult.OK)
                {
                    printDocument1.Print();
                }
                
            }
        }

        private void EditBtn_Click(object sender, EventArgs e)
        {

        }

        string prodname;
        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.DrawString("BookShop", new Font("century gothic", 12, FontStyle.Bold), Brushes.Red, new Point(80));
            e.Graphics.DrawString("ID PRODUCT PRICE QUANTITY TOTAL", new Font("century gothic", 10, FontStyle.Bold), Brushes.Red, new Point(26, 40));
            foreach(DataGridViewRow row in BillDGV.Rows)
            {
                prodid = Convert.ToInt32(row.Cells["Column1"].Value);
                prodname = "" + row.Cells["Column2"].Value;
                prodprice = Convert.ToInt32(row.Cells["Column3"].Value);
                prodqty = Convert.ToInt32(row.Cells["Column4"].Value);
                total = Convert.ToInt32(row.Cells["Column5"].Value);
                e.Graphics.DrawString("" + prodid, new Font("Century Gothic", 8, FontStyle.Bold), Brushes.Blue, new Point(26, pos));
                e.Graphics.DrawString("" + prodname, new Font("Century Gothic", 8, FontStyle.Bold), Brushes.Blue, new Point(45, pos));
                e.Graphics.DrawString("" + prodprice, new Font("Century Gothic", 8, FontStyle.Bold), Brushes.Blue, new Point(120, pos));
                e.Graphics.DrawString("" + prodqty, new Font("Century Gothic", 8, FontStyle.Bold), Brushes.Blue, new Point(170, pos));
                e.Graphics.DrawString("" + total, new Font("Century Gothic", 8, FontStyle.Bold), Brushes.Blue, new Point(235, pos));
                pos = pos + 20;
            }
            e.Graphics.DrawString("Grand Total: Rp. " + GrdTotal, new Font("century gothic", 12, FontStyle.Bold), Brushes.Crimson, new Point(60, pos + 50));
            e.Graphics.DrawString("------------BookStore------------", new Font("century gothic", 10, FontStyle.Bold), Brushes.Crimson, new Point(40, pos + 80));
            BillDGV.Rows.Clear();
            BillDGV.Refresh();
            pos = 100;
            GrdTotal = 0;
        }

        private void Reset()
        {
            BTitleTb.Text = "";
            QtyTb.Text = "";
            PriceTb.Text = "";
            ClientNameTb.Text = "";
        }
        private void ResetBtn_Click(object sender, EventArgs e)
        {
            Reset();
        }
    }
}
