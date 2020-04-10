using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Data;

namespace HierarchyManager
{
    public partial class MoveToDialog : Form
    {
        private readonly DynamicFetcher fetcher;

        public MoveToDialog(DynamicFetcher fetcher)
        {
            InitializeComponent();
            this.fetcher = fetcher;
            var items = fetcher.GetRoot();
            tree.Nodes.AddRange(items.Convert());
            tree.ContextMenuStrip = new ContextMenuStrip();
            tree.BeforeExpand += (s, e) =>
            {
                e.Node.Nodes.Clear();
                var children = fetcher.GetChildren(int.Parse(e.Node.Name));
                e.Node.Nodes.AddRange(children.Convert());
            };
        }
        public int? SelectedId { get
            {
                if (tree.SelectedNode == null) return null;
                return int.Parse(tree.SelectedNode.Name);
            } 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (tree.SelectedNode == null)
            {
                MessageBox.Show("Selecting a destination is required in order to move.");
                return;
            }
            if (MessageBox.Show("Are you sure?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
