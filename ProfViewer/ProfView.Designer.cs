namespace ProfViewer
{
    partial class ProfView
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProfView));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbRead = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tvProfTree = new System.Windows.Forms.TreeView();
            this.lvProfSort = new System.Windows.Forms.ListView();
            this.colCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colClass = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFunc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ofdOpen = new System.Windows.Forms.OpenFileDialog();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbRead});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1245, 30);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbRead
            // 
            this.tsbRead.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbRead.Image = ((System.Drawing.Image)(resources.GetObject("tsbRead.Image")));
            this.tsbRead.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRead.Name = "tsbRead";
            this.tsbRead.Size = new System.Drawing.Size(49, 27);
            this.tsbRead.Text = "Load";
            this.tsbRead.Click += new System.EventHandler(this.tsbRead_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 30);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tvProfTree);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lvProfSort);
            this.splitContainer1.Size = new System.Drawing.Size(1245, 608);
            this.splitContainer1.SplitterDistance = 414;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 3;
            // 
            // tvProfTree
            // 
            this.tvProfTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvProfTree.Location = new System.Drawing.Point(0, 0);
            this.tvProfTree.Margin = new System.Windows.Forms.Padding(4);
            this.tvProfTree.Name = "tvProfTree";
            this.tvProfTree.Size = new System.Drawing.Size(414, 608);
            this.tvProfTree.TabIndex = 5;
            this.tvProfTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvProfTree_AfterSelect);
            // 
            // lvProfSort
            // 
            this.lvProfSort.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colCount,
            this.colClass,
            this.colFunc});
            this.lvProfSort.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvProfSort.Location = new System.Drawing.Point(0, 0);
            this.lvProfSort.Margin = new System.Windows.Forms.Padding(4);
            this.lvProfSort.Name = "lvProfSort";
            this.lvProfSort.ShowGroups = false;
            this.lvProfSort.Size = new System.Drawing.Size(826, 608);
            this.lvProfSort.TabIndex = 6;
            this.lvProfSort.UseCompatibleStateImageBehavior = false;
            this.lvProfSort.View = System.Windows.Forms.View.Details;
            // 
            // colCount
            // 
            this.colCount.Text = "Count";
            this.colCount.Width = 68;
            // 
            // colClass
            // 
            this.colClass.Text = "Class";
            this.colClass.Width = 164;
            // 
            // colFunc
            // 
            this.colFunc.Text = "Function";
            this.colFunc.Width = 381;
            // 
            // ProfView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1245, 638);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ProfView";
            this.Text = "Profile Result Viewer";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbRead;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView tvProfTree;
        private System.Windows.Forms.ListView lvProfSort;
        private System.Windows.Forms.ColumnHeader colCount;
        private System.Windows.Forms.ColumnHeader colClass;
        private System.Windows.Forms.ColumnHeader colFunc;
        private System.Windows.Forms.OpenFileDialog ofdOpen;
    }
}