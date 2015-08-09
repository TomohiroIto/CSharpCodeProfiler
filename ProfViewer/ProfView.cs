using CSharpCodeProfiler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace ProfViewer
{
    public partial class ProfView : Form
    {
        private List<ProfileResultItem> resultList = null;

        public ProfView()
        {
            InitializeComponent();
        }

        private void tsbRead_Click(object sender, EventArgs e)
        {
            if(ofdOpen.ShowDialog() != DialogResult.OK) return;

            openList(ofdOpen.FileName);
        }

        private void tvProfTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            makeListView();
        }

        private void openList(string filename)
        {
            if (!File.Exists(filename))
            {
                MessageBox.Show("Could not find the file.");
                return;
            }

            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(List<ProfileResultItem>));

                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    resultList = ser.Deserialize(fs) as List<ProfileResultItem>;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to read the file.");
                resultList = null;
                return;
            }

            lvProfSort.Items.Clear();
            tvProfTree.Nodes.Clear();
            tvProfTree.Nodes.Add(readList());
        }

        private TreeNode readList()
        {
            // get all items which have depth = 1.
            var firstNodes = resultList.Where((p) => p.Depth == 1);

            TreeNode tree = new TreeNode("<TOP>");
            tree.Tag = -1;

            foreach (ProfileResultItem first in firstNodes)
            {
                // add to the tree
                TreeNode fNode = tree.Nodes.Add(first.ProfileCount.ToString() + " " + first.FunctionName);
                fNode.Tag = first.ID;

                int id = first.ID;
                addChildren(id, fNode);
            }

            return tree;
        }

        private void addChildren(int id, TreeNode fNode)
        {
            // find children
            var children = resultList.Where(p => p.ParentID == id).OrderByDescending(p => p.ProfileCount);

            foreach (ProfileResultItem child in children)
            {
                // add children
                TreeNode cNode = fNode.Nodes.Add(child.ProfileCount.ToString() + " " + child.FunctionName);
                cNode.Tag = child.ID;

                int cid = child.ID;
                addChildren(cid, cNode);
            }
        }

        private void makeListView()
        {
            if (resultList == null) return;

            TreeNode curNode = tvProfTree.SelectedNode;
            int id = (int)curNode.Tag;
            if (id <= 0)
            {
                lvProfSort.Items.Clear();
                return;
            }

            var children = resultList.Where(p => p.ParentID == id).OrderByDescending(p => p.ProfileCount);

            lvProfSort.Items.Clear();
            foreach (ProfileResultItem child in children)
            {
                string[] list = new string[] { child.ProfileCount.ToString(), child.ClassName, child.FunctionString };
                lvProfSort.Items.Add(new ListViewItem(list));
            }
        }
    }
}
