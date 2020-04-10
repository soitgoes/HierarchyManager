using Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HierarchyManager
{
    public partial class Form1 : Form
    {
        private readonly List<int> selectedIds =  new List<int>();
        private readonly DynamicFetcher fetcher;

        public Form1()
        {
            InitializeComponent();
            string idField = ConfigurationManager.AppSettings["id"];
            string parentIdField = ConfigurationManager.AppSettings["parentId"];
            string tableName = ConfigurationManager.AppSettings["table"];
            string nameField = ConfigurationManager.AppSettings["name"];
            string connString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;


            var conn = new SqlConnection(connString);
            fetcher = new DynamicFetcher(conn, tableName, idField, parentIdField, nameField);
            ReInitTree(); 
            tree.ContextMenuStrip = new ContextMenuStrip();
            tree.CheckBoxes = true;
            tree.AfterCheck += (s, e) =>
            {
                try
                {
                    if (e.Node.Checked)
                        selectedIds.Add(int.Parse(e.Node.Name));
                    else
                        selectedIds.Remove(int.Parse(e.Node.Name));
                }
                catch (Exception ex)
                {

                }
            };
            tree.BeforeExpand += (s, e) =>
            {
                e.Node.Nodes.Clear();
                var children = fetcher.GetChildren(int.Parse(e.Node.Name));
                e.Node.Nodes.AddRange(children.Convert());
            };


        }

        private void ReInitTree()
        {
            tree.Nodes.Clear();
            selectedIds.Clear();
            var items = fetcher.GetRoot();
            tree.Nodes.AddRange(items.Convert());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var moveToForm = new MoveToDialog(fetcher);
            if (moveToForm.ShowDialog() == DialogResult.OK && moveToForm.SelectedId.HasValue)
            {
               foreach (var id in selectedIds)
                {
                    fetcher.MoveItem(id, moveToForm.SelectedId.Value);
                }
            }
            ReInitTree();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ReInitTree();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearch.Text.Trim())) return;
            tree.Nodes.Clear();
            var items = fetcher.Search(txtSearch.Text.Trim());
            tree.Nodes.AddRange(items.Convert());
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            txtSearch.Text = "";
        }
    }
}
