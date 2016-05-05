using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using GazeboExporter.Robot;

namespace GazeboExporter.UI
{
    /// <summary>
    /// represents a node in the graph. THis node can either be a link or a temp nod that a joint passes through
    /// </summary>
    public class GraphNode
    {
        #region Properties
        /// <summary>
        /// the level the node is in the graph
        /// </summary>
        public int GraphLevel { get; set; }

        /// <summary>
        /// the location of the node inside its level
        /// </summary>
        public int LevelIndex { get; set; }

        /// <summary>
        /// The X position of this node in pixels
        /// </summary>
        public int XPos { get; set; }

        /// <summary>
        /// The average of the node's parent's indexes
        /// </summary>
        public double ParentNodeAverage { get; set; }

        /// <summary>
        /// The average of the node's children's indexes
        /// </summary>
        public double ChildNodeAverage { get; set; }

        /// <summary>
        /// The link that this node represents. Null if this node is a temporary node
        /// </summary>
        public Link StoredLink { get; set; }

        /// <summary>
        /// True if the node is not for a link
        /// </summary>
        public bool IsFakeNode
        {
            get
            {
                return StoredLink == null;
            }
        }

        /// <summary>
        /// All nodes that are a parent to this node
        /// </summary>
        public HashSet<GraphNode> ParentNodes { get; private set; }

        /// <summary>
        /// All nodes that are a child to this one
        /// </summary>
        public HashSet<GraphNode> ChildNodes { get; private set; }

        /// <summary>
        /// List of all flipped nodes
        /// </summary>
        private List<GraphNode> FlippedNodes;

        /// <summary>
        /// Set of all nodes that their connection to this node is marked as a type 1 error. 
        /// Type 1 errors are connections that cross a connection that is between 2 temp nodes
        /// </summary>
        public HashSet<GraphNode> MarkedNodes;

        /// <summary>
        /// The RootNode for this link, if this is a root node it will contain itself.
        /// </summary>
        public GraphNode RootNode;

        /// <summary>
        /// Array containg the widths of each object in the block
        /// </summary>
        public GraphNode[] SubNodes;

        /// <summary>
        /// Set of all nodees that this connects to and have a level less than it
        /// </summary>
        public HashSet<GraphNode> LowerNodes;

        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new GraphNode object
        /// </summary>
        /// <param name="l">The link this node will represent. If null this node is a temporary node</param>
        public GraphNode(Link l)
        {
            StoredLink = l;
            ParentNodes = new HashSet<GraphNode>();
            ChildNodes = new HashSet<GraphNode>();
            GraphLevel = -1;
            LevelIndex = -1;
            FlippedNodes = new List<GraphNode>();
            MarkedNodes = new HashSet<GraphNode>();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Flips the joint that is connected to the specified node. This node must be the child of the joint
        /// </summary>
        /// <param name="g"> parent node that needs to be flipped</param>
        public void FlipJoint(GraphNode g)
        {
            if (ParentNodes.Contains(g) && !FlippedNodes.Contains(g))
            {

                ParentNodes.Remove(g);
                ChildNodes.Add(g);
                g.ChildNodes.Remove(this);
                g.ParentNodes.Add(this);
                FlippedNodes.Add(g);
            }
            else if (ChildNodes.Contains(g) && FlippedNodes.Contains(g))
            {
                ChildNodes.Remove(g);
                ParentNodes.Add(g);
                g.ParentNodes.Remove(this);
                g.ChildNodes.Add(this);
                FlippedNodes.Remove(g);
            }
            else
            {
                return;
            }

        }

        /// <summary>
        /// Restores all node joints to their correct orientations
        /// </summary>
        public void RestoreAllJoints()
        {
            while (FlippedNodes.Count > 0)
            {
                FlipJoint(FlippedNodes[0]);
            }
        }

        /// <summary>
        /// Creates a new node and inserts it inbetween this node and the given parent
        /// </summary>
        /// <param name="parent">The node that is a parent of this node and the new node will be inserted before</param>
        /// <returns>THe newly created node</returns>
        public GraphNode InsertNode(GraphNode parent)
        {
            GraphNode node = new GraphNode(null);
            
            
            node.ParentNodes.Add(parent);
            node.ChildNodes.Add(this);
            parent.ChildNodes.Remove(this);
            parent.ChildNodes.Add(node);
            this.ParentNodes.Remove(parent);
            this.ParentNodes.Add(node);
            if (parent.FlippedNodes.Contains(this))
            {
                parent.FlippedNodes.Remove(this);
                parent.FlippedNodes.Add(node);
                node.FlippedNodes.Add(this);
            }
            return node;
        }

        /// <summary>
        /// Traces the node joint and returns the node at the end of it.
        /// </summary>
        /// <param name="direction">True if trace up, false if trace down</param>
        /// <returns>The node that is at the end of the path. Will return the node if called on a non temporary node</returns>
        public GraphNode TraceNodes(bool direction)
        {
            if (StoredLink != null)
            {
                return this;
            }
            if (direction)
            {
                return ParentNodes.ElementAt(0).TraceNodes(true);
            }
            else
            {
                return ChildNodes.ElementAt(0).TraceNodes(false);
            }

        }

        /// <summary>
        /// Fills in the LowerNodes Property
        /// </summary>
        public void FillLowerNodes()
        {
            LowerNodes = new HashSet<GraphNode>();
            FindConnectedNodes(LowerNodes, this, this);
        }

        /// <summary>
        /// finds all nodes that are connected to the base node by joints that are of lower level than the base node
        /// </summary>
        /// <param name="set">The set that all connected nodes should be added to</param>
        /// <param name="baseNode">The base node for testing the tree</param>
        /// <param name="previousNode">The previous node to be tested</param>
        public void FindConnectedNodes(HashSet<GraphNode> set, GraphNode baseNode, GraphNode previousNode)
        {
            
            if (GraphLevel <= baseNode.GraphLevel && previousNode.GraphLevel > GraphLevel)
                return;
            if (!set.Contains(this))
                set.Add(this);
            else
                return;
            if (!previousNode.Equals(baseNode))
                foreach (GraphNode n in ParentNodes)
                {
                    if (!n.Equals(previousNode))
                        n.FindConnectedNodes(set,baseNode,this);
                }
            foreach (GraphNode n in ChildNodes)
            {
                if (!n.Equals(previousNode))
                    n.FindConnectedNodes(set,baseNode,this);
            }
            if(!set.Contains(this))
                set.Add(this);
        }


        public const int linkBoxHeight = 100;
        private const int linkNameHeight = 75;
        private const int jointNameHeight = 25;
        private const int MinLinkBoxWidth = 120;
        private const int MinJointBoxWidth = 60;
        public const int VerticalSpacing = 50;
        public const int attachmentBoxDim = 33;
        private const int numVerticalAttBoxes = linkBoxHeight / attachmentBoxDim;
        private const int minSpacing = 30;
        private static Pen linkOutline = new Pen(Brushes.DarkGreen, 4);
        private static Pen jointOutline = new Pen(Brushes.DarkRed, 4);
        private static Pen selectedOutline = new Pen(Brushes.Blue, 4); 
        private static Pen connectingLines = new Pen(Brushes.Black, 2);
        private static AdjustableArrowCap endCap = new AdjustableArrowCap(6, 6);

        /// <summary>
        /// gets the total width of this link and its attachments
        /// </summary>
        /// <returns>The total width of the link box and all of its attachments</returns>
        public float GetWidth()
        {
            if (StoredLink != null)
            {
                int jointWidth = ParentNodes.Count * MinJointBoxWidth;
                if (jointWidth > MinJointBoxWidth)
                    return jointWidth + ((StoredLink.GetNumAttachments() + (numVerticalAttBoxes - 1)) / numVerticalAttBoxes) * attachmentBoxDim;
                else
                    return MinLinkBoxWidth + ((StoredLink.GetNumAttachments() + (numVerticalAttBoxes - 1)) / numVerticalAttBoxes) * attachmentBoxDim;
            }
            else
                return 1;
            
        }

        public int FindCenter()
        {
            int largest = 0;
            foreach (GraphNode g in SubNodes)
            {
                if (g.GetWidth() > largest)
                    largest = (int)g.GetWidth();
            }
            return largest / 2;
        }

        /// <summary>
        /// returns the height of the node
        /// </summary>
        /// <returns></returns>
        public float GetHeight()
        {
            return linkBoxHeight;
        }

        /// <summary>
        /// draws the node on the graph panel
        /// </summary>
        /// <param name="g">The Graphics object that will be used for drawing</param>
        /// <param name="topCenter"></param>
        /// <param name="selectionAreas">Dictionary of all selection areas</param>
        /// <param name="dropZones">Dictionary of all attachment drop zones</param>
        public void DrawNode(System.Drawing.Graphics g, int xOffset,int yOffset, Dictionary<Rectangle, ISelectable> selectionAreas, Dictionary<Rectangle, Link> dropZones)
        {
            selectedOutline.DashStyle = DashStyle.Dash;
            Font font = new Font("Times New Roman", 12);
            StringFormat formater = new StringFormat();
            formater.Alignment = StringAlignment.Center;
            formater.LineAlignment = StringAlignment.Center;
            //Draw this link
            Point topCenter = new Point (XPos-xOffset,GraphLevel*linkBoxHeight + (GraphLevel+1)*VerticalSpacing-yOffset);
            Point topLeft = new Point((int)(topCenter.X-GetWidth()/2),topCenter.Y);
            Pen linkPen, jointPen;
            if (StoredLink == null)//draw temporary node
            {
                g.DrawLine(connectingLines,topCenter, new Point(topCenter.X,topCenter.Y+linkBoxHeight));
                Point lineStartPoint = topCenter;
                Point lineEndPoint;
                GraphNode parent = ParentNodes.ElementAt(0);
                if (parent.IsFakeNode)
                    lineEndPoint = new Point(parent.XPos - xOffset, topCenter.Y - VerticalSpacing);
                else
                    lineEndPoint = new Point(parent.XPos - xOffset, topCenter.Y - VerticalSpacing + 4);
                if (parent.FlippedNodes.Contains(this) && !parent.IsFakeNode)
                {
                    connectingLines.CustomEndCap = endCap;
                }
                
                g.DrawLine(connectingLines, lineStartPoint, lineEndPoint);
                connectingLines.EndCap = LineCap.NoAnchor;
                return;
            }

            Rectangle link;

            int linkWidth = MinLinkBoxWidth;
            if (MinJointBoxWidth * ParentNodes.Count > linkWidth)
            {
                linkWidth = MinJointBoxWidth * ParentNodes.Count;
            }

            link = new Rectangle((int)(topCenter.X - GetWidth() / 2), topCenter.Y, linkWidth, linkBoxHeight + 4);
            dropZones.Add(link, StoredLink);
            linkPen = new Pen(Color.FromArgb(StoredLink.ColorRed, StoredLink.ColorGreen, StoredLink.ColorBlue), 4); //linkOutline;
            if (StoredLink.Selected)
                linkPen.DashStyle = selectedOutline.DashStyle;   
            if (ParentNodes.Count > 0)
            {
                link = new Rectangle((int)(topCenter.X - GetWidth() / 2), (int)topCenter.Y + jointNameHeight + 4, linkWidth, linkNameHeight);
                int jointWidth = linkWidth / ParentNodes.Count;
                List<GraphNode> ordererdNodes = new List<GraphNode>();
                foreach (GraphNode node in ParentNodes)
                {
                    bool inserted = false;
                    for (int i = 0; i < ordererdNodes.Count; i++)
                    {
                        if (node.LevelIndex < ordererdNodes[i].LevelIndex)
                        {
                            ordererdNodes.Insert(i, node);
                            inserted = true;
                            break;
                        }
                    }
                    if (!inserted)
                    {
                        ordererdNodes.Add(node);
                    }
                }
                Joint[] joints = new Joint[ordererdNodes.Count];
                for (int i = 0; i < ordererdNodes.Count; i++)
                {
                    Link parent = ordererdNodes[i].TraceNodes(true).StoredLink;
                    joints[i] = StoredLink.GetJointFromLink(parent);
                }

                StoredLink.UpperJoints = joints;
                for (int i = 0; i < joints.Length; i++)
                {
                    Rectangle jointRect = new Rectangle(topLeft.X + jointWidth * i, topLeft.Y, jointWidth, jointNameHeight);

                    if (joints[i].Selected)
                        jointPen = selectedOutline;
                    else
                        jointPen = jointOutline;

                    selectionAreas.Add(jointRect, joints[i]);
                    g.DrawRectangle(jointPen, jointRect);
                    g.DrawString(joints[i].Type, font, Brushes.Black, jointRect, formater);

                    Point lineStartPoint = new Point((int)(topLeft.X + jointWidth * (i + .5)), topLeft.Y);
                    Point lineEndPoint;
                    if (ordererdNodes[i].IsFakeNode)
                        lineEndPoint = new Point(ordererdNodes[i].XPos-xOffset, topCenter.Y - VerticalSpacing);
                    else
                        lineEndPoint = new Point(ordererdNodes[i].XPos-xOffset, topCenter.Y - VerticalSpacing+4);


                    
                    if (!ordererdNodes[i].FlippedNodes.Contains(this))
                    {
                        connectingLines.CustomStartCap = endCap;
                    }
                    else if (!ordererdNodes[i].IsFakeNode)
                    {
                        connectingLines.CustomEndCap = endCap;
                    }
                    g.DrawLine(connectingLines, lineStartPoint, lineEndPoint);

                    connectingLines.StartCap = LineCap.NoAnchor;
                    connectingLines.EndCap = LineCap.NoAnchor;

                }
            }
            

            g.DrawRectangle(linkPen, link);
            g.DrawString(this.StoredLink.Name, font, Brushes.Black, link, formater);
            selectionAreas.Add(link, StoredLink);

            Rectangle att;
            for (int l = 0; l < StoredLink.GetNumAttachments(); l++)
            {
                att = new Rectangle((int)(2 + link.X + linkWidth + attachmentBoxDim * (int)(l / numVerticalAttBoxes)), (int)topCenter.Y + attachmentBoxDim * (int)(l % numVerticalAttBoxes), attachmentBoxDim, attachmentBoxDim);
                System.Diagnostics.Debug.WriteLine(StoredLink.GetAttachment(l));
                StoredLink.GetAttachment(l).DrawOnGraph(g, att, StoredLink.GetAttachment(l).Selected);
                selectionAreas.Add(att, StoredLink.GetAttachment(l));
            }
        }

        /// <summary>
        /// Writes the stored link and all of its parent and child nodes. Assumes this is not a temporary node
        /// </summary>
        /// <returns>String containg name of link and parent and child links</returns>
        public override string ToString()
        {
            if (StoredLink != null)
            {
                string str = StoredLink.Name + ": ";
                foreach (GraphNode g in ParentNodes)
                {
                    str += g.StoredLink + ", ";
                }
                str += "; ";
                foreach (GraphNode g in ChildNodes)
                {
                    str += g.StoredLink + ", ";
                }
                return str;
            }

            return TraceNodes(true).StoredLink.Name + " temp " + TraceNodes(false).StoredLink.Name;
        }
        #endregion
    }
}
