using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GazeboExporter.Robot;

namespace GazeboExporter.UI
{
    public delegate void LinkSelectionChanged(Link link);

    public partial class LinkSelector : UserControl
    {
        public Launcher OnEditLink;

        public LinkSelector(RobotModel robot)
        {
            this.robot = robot;
            InitializeComponent();
            this.linkSelectTree.Nodes.Add(robot.BaseLink);
            Refresh();
        }
        RobotModel robot;

        public void Refresh()
        {
            linkSelectTree.Invalidate();
        }

        public LinkSelectionChanged OnLinkSelectionChanged;

        public Link getSelectedLink()
        {
            return null;
        }

        public void close()
        {
            linkSelectTree.Nodes.Clear();
        }

        internal void SelectBase()
        {
            this.linkSelectTree.SelectedNode = linkSelectTree.Nodes[0];
            this.linkSelectTree.ExpandAll();
        }

        private void addLinkButton_Click(object sender, EventArgs e)
        {
            if (linkSelectTree.SelectedNode != null)
            {
                ((Link)linkSelectTree.SelectedNode).AddChild();
                linkSelectTree.SelectedNode.Expand();
                linkSelectTree.SelectedNode = linkSelectTree.SelectedNode.LastNode;
                OnLinkSelectionChanged((Link)linkSelectTree.SelectedNode);
            }
            Refresh();
        }

        private void removeLinkButton_Click(object sender, EventArgs e)
        {
            if (linkSelectTree.SelectedNode != null)
            {
                ((Link)linkSelectTree.SelectedNode.Parent).RemoveChild((Link)linkSelectTree.SelectedNode);
                SelectBase();
                Refresh();
            }      
        }

        private void editLinkButton_Click(object sender, EventArgs e)
        {
            if (linkSelectTree.SelectedNode != null)
            {
                if (OnEditLink != null)
                    OnEditLink((Link)linkSelectTree.SelectedNode);
            }
        }


        private void linkSelectTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (OnLinkSelectionChanged != null)
                OnLinkSelectionChanged((Link)linkSelectTree.SelectedNode);
            if (e.Node == robot.BaseLink)
                removeLinkButton.Enabled = false;
            else
                removeLinkButton.Enabled = true;
        }

        private void linkSelectTree_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void linkSelectTree_DragDrop(object sender, DragEventArgs e)
        {

        }

        private void linkSelectTree_DragEnter(object sender, DragEventArgs e)
        {

        }

        private void linkSelectTree_DragOver(object sender, DragEventArgs e)
        {

        }

        private void linkSelectTree_ItemDrag(object sender, ItemDragEventArgs e)
        {

        }

        private void linkSelectTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {

        }


        
    }
}
