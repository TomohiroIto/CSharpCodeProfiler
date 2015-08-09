using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace ProfViewer
{
    public partial class ProfView : Form
    {
        private DataTable resultTable = null;

        public ProfView()
        {
            InitializeComponent();
        }

        private void tsbRead_Click(object sender, EventArgs e)
        {
            if(ofdOpen.ShowDialog() != DialogResult.OK) return;

            openDataTable(ofdOpen.FileName);
        }

        private void openDataTable(string filename)
        {
            if (!File.Exists(filename))
            {
                MessageBox.Show("Could not find the file.");
                return;
            }

            resultTable = new DataTable();
            resultTable.Columns.Add("ID", typeof(int));
            resultTable.Columns.Add("FUNCTION_NAME", typeof(string));
            resultTable.Columns.Add("FUNCTION_STRING", typeof(string));
            resultTable.Columns.Add("CLASS_NAME", typeof(string));
            resultTable.Columns.Add("PARENT_NAME", typeof(string));
            resultTable.Columns.Add("PARENT_ID", typeof(int));
            resultTable.Columns.Add("DEPTH", typeof(int));
            resultTable.Columns.Add("PROFILE", typeof(int));

            resultTable.Namespace = "";
            resultTable.TableName = "PROFILE";

            try
            {
                resultTable.ReadXml(filename);
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to read the file.");
                resultTable = null;
                return;
            }

            tvProfTree.Nodes.Add(readTable(resultTable));
        }

        private TreeNode readTable(DataTable table)
        {
            // get all items which have depth = 1.
            DataRow[] firstNodes = table.Select("DEPTH = 1");

            TreeNode tree = new TreeNode("<TOP>");
            tree.Tag = -1;
            foreach (DataRow first in firstNodes)
            {
                // add to the tree
                TreeNode fNode = tree.Nodes.Add(first["PROFILE"].ToString() + " " + first["FUNCTION_NAME"].ToString());
                fNode.Tag = first["ID"];

                int id = (int)first["ID"];
                addChildren(table, id, fNode);
            }

            return tree;
        }

        private void addChildren(DataTable table, int id, TreeNode fNode)
        {
            // find children
            DataRow[] children = table.Select("PARENT_ID = " + id, "PROFILE DESC");

            foreach (DataRow child in children)
            {
                // add children
                TreeNode cNode = fNode.Nodes.Add(child["PROFILE"].ToString() + " " + child["FUNCTION_NAME"].ToString());
                cNode.Tag = child["ID"];

                int cid = (int)child["ID"];
                addChildren(table, cid, cNode);
            }
        }

        private void tvProfTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (resultTable == null) return;

            TreeNode curNode = tvProfTree.SelectedNode;
            int id = (int)curNode.Tag;
            if (id <= 0)
            {
                lvProfSort.Items.Clear();
                return;
            }

            DataRow[] children = resultTable.Select("PARENT_ID = " + id, "PROFILE DESC");

            lvProfSort.Items.Clear();
            foreach (DataRow child in children)
            {
                string[] list = new string[] { child["PROFILE"].ToString(), child["CLASS_NAME"].ToString(), child["FUNCTION_STRING"].ToString() };
                lvProfSort.Items.Add(new ListViewItem(list));
            }
        }
    }
}