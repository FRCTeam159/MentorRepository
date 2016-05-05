using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GazeboExporter.Robot;
using System.Reflection;

namespace GazeboExporter.UI
{
    public delegate void SelectionChanged(ISelectable selection);

    public partial class RobotGraphPanel : UserControl
    {

        private const float blockSize = 20;
        private const float linkBoxWidth = blockSize * 3;
        private const float linkBoxHeight = blockSize * 3;
        private const float borderWidth = 2;
        private const float selectedBorderWidth = 4;
        private Pen selectedBorderColor = new Pen(Brushes.Blue, 4);
        private Pen attachmentBoxColor = new Pen(Brushes.Black, 2);
        private Pen linkNameBoxColor = new Pen(Brushes.DarkGreen, 2);
        private Pen jointTypeBoxColor = new Pen(Brushes.DarkRed, 2);
        private Pen connectingLineColor = new Pen(Brushes.Black, 1);
        private const float minSpacing = 10;

        private Dictionary<Rectangle, Link> dropZones;
        private Dictionary<Rectangle, ISelectable> selectionAreas;

        public SelectionChanged OnSelectionChanged;

        public ISelectable currentSelection;

        private Rectangle clientArea;
        private float totalHeight;
        private float totalWidth;

        private Dictionary<int,GraphNode> RobotNodes;
        private List<List<GraphNode>> RobotGraph;

        private bool connectingLinks = false;
        private Point dragStart;
        private Rectangle dragStartRect;



        public RobotGraphPanel()
        {
            InitializeComponent();
        }

        public void setRobot(RobotModel robot)
        {
            this.robot = robot;
            clientArea = new Rectangle(0, 0, this.Width, this.Height);
            currentSelection = robot;
            currentSelection.Selected = true;
            Refresh();
        }

        public override void Refresh()
        {
            //System.Diagnostics.Debug.WriteLine("Refreshing Graph Panel");
            //totalWidth = robot.BaseLink.getTotalWidth() + minSpacing * 4;
            //totalHeight = robot.BaseLink.getTotalHeight() + minSpacing * 3;
            base.Refresh();
            ToggleScrollBars();
            System.Diagnostics.Debug.WriteLine("Done: RobotGraphPanel refreshed.");
        }

        /// <summary>
        /// calculates node locations and draws nodes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            //RobotInfo.WriteToLogFile("Painting Panel");
            Graphics g = e.Graphics;
            //g.Clear(Color.White);
            selectionAreas = new Dictionary<Rectangle, ISelectable>();
            dropZones = new Dictionary<Rectangle, Link>();

            CalcNodeLocations();
            ToggleScrollBars();
            foreach (List<GraphNode> level in RobotGraph)
            {
                foreach (GraphNode node in level)
                {
                    node.DrawNode(g,clientArea.X,clientArea.Y,selectionAreas,dropZones);
                }
            }

            if (connectingLinks && !dragStartRect.Contains(PointToClient(Cursor.Position)))
            {
                Pen connectionPen = new Pen(Color.Gray,2);
                connectionPen.CustomEndCap =  new System.Drawing.Drawing2D.AdjustableArrowCap(6, 6);
                Point p = this.PointToClient(Cursor.Position);
                g.DrawLine(connectionPen,dragStart,p);
            }
            
            //g.Dispose();
            //RobotInfo.WriteToLogFile("Painting Panel Completed");
        }

        /// <summary>
        /// Toggles the scroll bar on or off as needed
        /// </summary>
        private void ToggleScrollBars()
        {
            if (totalWidth < clientArea.Width)
            {
                hScrollBar1.Enabled = false;
                clientArea.X = 0;
            }
            else
            {
                hScrollBar1.Enabled = true;
                hScrollBar1.Maximum = ((int)totalWidth - clientArea.Width) / 10 + hScrollBar1.LargeChange + vScrollBar1.Width / 10;
            }
            if (totalHeight < clientArea.Height)
            {
                vScrollBar1.Enabled = false;
                clientArea.Y = 0;
            }
            else
            {
                vScrollBar1.Enabled = true;
                vScrollBar1.Maximum = ((int)totalHeight - clientArea.Height) / 10 + vScrollBar1.LargeChange;
            }
        }

        private RobotModel robot;

        public void select(ISelectable select)
        {
            currentSelection.Selected = false;
            currentSelection = select;
            currentSelection.Selected = true;
            panel1.Invalidate();
        }

        /// <summary>
        /// if mouse down on a link, call dragdrop to connect links
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (dropZones == null || selectionAreas == null)
            {
                return;
            }

            Point downAt = new Point(e.X, e.Y);
            if (currentSelection != null)
                currentSelection.Selected = false;


            //show property page of selected link/joint/attachment
            currentSelection = robot;
            foreach (Rectangle r in selectionAreas.Keys)
            {
                if (r.Contains(downAt))
                {
                    currentSelection = selectionAreas[r];
                    break;
                }
            }
            if (currentSelection == robot || currentSelection is Attachment)
            {
                currentSelection.Selected = true;
                if (OnSelectionChanged != null)
                    OnSelectionChanged(currentSelection);
                panel1.Invalidate();
                return;
            }

            //If select a link to connect to other link
            foreach (Rectangle r in dropZones.Keys)
            { 
                if (r.Contains(downAt))
                {
                    connectingLinks = true;
                    dragStart = new Point(e.X, e.Y);
                    dragStartRect = r;
                    DoDragDrop(dropZones[r], DragDropEffects.Copy | DragDropEffects.Move); 
                    connectingLinks = false;

                    break;
                }
            }


        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            clientArea.Width = this.Width;
            clientArea.Height = this.Height;
            Refresh();
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            clientArea.Y = e.NewValue * 10;
            panel1.Invalidate();
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            clientArea.X = e.NewValue * 10;
            panel1.Invalidate();
        }



        private void RobotGraphPanel_DragDrop(object sender, DragEventArgs e)
        {

        }

        private void panel1_DragDrop(object sender, DragEventArgs e)
        {
            if (dropZones == null)
                return;

            ManageRobot manager = (ManageRobot)this.FindForm();
            int whichTab = manager.getAttachmentTabIndex();
            //System.Diagnostics.Debug.WriteLine(whichTab);
            Point p = panel1.PointToClient(new Point(e.X, e.Y));

            // make new joint to connect links 
            if (connectingLinks == true)
            {
                foreach (Rectangle r in dropZones.Keys)
                {
                    if (r.Contains(p))
                    {
                        Link firstLink = (Link)e.Data.GetData(typeof(Link));
                        Link secondLink = dropZones[r];
                        if (!(firstLink.Equals(secondLink)) && 
                            Array.IndexOf(firstLink.GetParentLinks(), secondLink) == -1 &&
                            Array.IndexOf(secondLink.GetParentLinks(), firstLink) == -1)
                        {
                            RobotInfo.WriteToLogFile("\nDragging custom joint (RobotGraphPanel)");
                            Joint newJoint = secondLink.connectLinks(firstLink, secondLink);
                            RobotInfo.WriteToLogFile("Successfully created new joint: " + newJoint.Name + " (RobotGraphPanel)");
                            select(newJoint);
                        }
                        break;
                    }
                }
            }
            // make new attachment
            else if (e.Data.GetData(typeof(AttachmentDescriptor)) != null)
            {
                AttachmentDescriptor desc = (AttachmentDescriptor)e.Data.GetData(typeof(AttachmentDescriptor));
                foreach (Rectangle r in dropZones.Keys)
                {
                    if (r.Contains(p))
                    {
                        RobotInfo.WriteToLogFile("\nCreating new attachment (RobotGraphPanel)");
                        Attachment att = dropZones[r].AddAttachment(desc.id);
                        if (att != null)
                        {
                            RobotInfo.WriteToLogFile("Successfully added new attachment: " + att.Name + " (RobotGraphPanel)");
                            select(att);
                        }
                        break;
                    }
                }
            }
            // make new child Link with connecting joint
            else if (e.Data.GetData(typeof(Link)) != null)
            {
                foreach (Rectangle r in dropZones.Keys)
                {
                    if (r.Contains(p))
                    {
                        RobotInfo.WriteToLogFile("\nDragging custom link (RobotGraphPanel)");
                        Link l = (Link)(dropZones[r].AddChild());
                        RobotInfo.WriteToLogFile("Successfully created new link " + l.Name + " on link " +
                                dropZones[r].Name + " (RobotGraphPanel)");
                        select(l);
                        break;
                    }
                }
            }

            

            currentSelection.Selected = true;
            if (OnSelectionChanged != null)
                OnSelectionChanged(currentSelection);
            panel1.Invalidate();
            return;
        }

        private void panel1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        #region Graph Calculations
        /// <summary>
        /// creates the graph of the robot
        /// </summary>
        private void CreateGraphNodes()
        {
            Link[] links = robot.GetLinksAsArray();
            //System.Diagnostics.Debug.WriteLine(String.Join(",", (object[])links));
            RobotNodes = new Dictionary<int, GraphNode>();
            foreach (Link l in links)
            {
                RobotNodes.Add(l.Id, new GraphNode(l));
            }

            foreach (GraphNode g in RobotNodes.Values.ToArray())
            {
                foreach (Link l in g.StoredLink.GetParentLinks())
                {
                    g.ParentNodes.Add(RobotNodes[l.Id]);
                    RobotNodes[l.Id].ChildNodes.Add(g);
                }
            }
            
        }

        /// <summary>
        /// temporarily flips joints to remove cycles
        /// </summary>
        private void RemoveGraphCycles()
        {
            HashSet<GraphNode> tempGraph = new HashSet<GraphNode>(RobotNodes.Values.ToArray());
            while (tempGraph.Count > 0)
            {
                bool hasFinished = false;
                while (!hasFinished)//remove all sources from tempGraph. A source is a node with no parents
                {
                    hasFinished = true;
                    GraphNode tempNode;

                    for (int i = 0; i < tempGraph.Count;i++ )
                    {
                        tempNode = tempGraph.ElementAt(i);
                        if(!tempGraph.Overlaps(tempNode.ParentNodes))
                        {
                            tempGraph.Remove(tempNode);
                            hasFinished = false;
                            i--;
                        }
                    }
                }
                hasFinished = false;
                while (!hasFinished)//remove all sinks from the graph. A sink is a node with no children
                {
                    hasFinished = true;
                    GraphNode tempNode;
                    for (int i = 0; i < tempGraph.Count; i++)
                    {
                        tempNode = tempGraph.ElementAt(i);
                        if(!tempGraph.Overlaps(tempNode.ChildNodes))
                        {
                            tempGraph.Remove(tempNode);
                            hasFinished = false;
                            i--;
                        }
                    }
                }

                if (tempGraph.Count > 0) //If nodes are left, they must be in a cycle, so flip joints to fix
                {
                    foreach (GraphNode tempNode in tempGraph)
                    {
                        if (tempNode.Equals(RobotNodes[0]) || tempNode.ParentNodes.Count > 1)
                        {
                            List<GraphNode> tempList = tempNode.ParentNodes.ToList();
                            foreach (GraphNode g in tempList)
                            {
                                if (tempGraph.Contains(g))
                                {
                                    tempNode.FlipJoint(g);
                                }

                            }
                            break;
                        }

                    }
                }

            }
            
        }

        /// <summary>
        /// sets each node to the correct level and creates virtual nodes for joints that span levels
        /// </summary>
        private void SetNodeLevels()
        {
            List<GraphNode> remainingNodes = RobotNodes.Values.ToList<GraphNode>();
            /*foreach (GraphNode g in remainingNodes)
            {
                System.Diagnostics.Debug.WriteLine(g.ToString());
            }*/
            RobotGraph = new List<List<GraphNode>>();
            RobotGraph.Add(new List<GraphNode>());
            RobotGraph[0].Add(remainingNodes[0]);
            remainingNodes[0].GraphLevel = 0;
            remainingNodes.RemoveAt(0);
            while (remainingNodes.Count > 0)
            {
                GraphNode currentNode;
                for (int i = 0; i < remainingNodes.Count; i++)
                {
                    currentNode = remainingNodes[i];
                    int largestDist = -1;
                    foreach (GraphNode g in currentNode.ParentNodes)
                    {
                        if (g.GraphLevel == -1)
                        {
                            largestDist = -1;
                            break;
                        }
                        else if (g.GraphLevel > largestDist)
                        {
                            largestDist = g.GraphLevel;
                        }
                    }
                    if (largestDist != -1)
                    {
                        largestDist++;
                        if (RobotGraph.Count <= largestDist)
                        {
                            RobotGraph.Add(new List<GraphNode>());
                        }
                        RobotGraph[largestDist].Add(currentNode);
                        currentNode.GraphLevel = largestDist;
                        List<GraphNode> tempList = currentNode.ParentNodes.ToList();
                        foreach (GraphNode g in tempList)
                        {
                            GraphNode parentNode = g;
                            GraphNode tempNode = currentNode;
                            for (int j = currentNode.GraphLevel - 1; j > parentNode.GraphLevel; j--)
                            {
                                GraphNode n = tempNode.InsertNode(parentNode);
                                RobotGraph[j].Add(n);
                                n.GraphLevel = j;
                                tempNode = n;
                            }
                        }

                        remainingNodes.Remove(currentNode);
                        i--;
                    }
                }
            }
        }

        /// <summary>
        /// Orders all level in the graph to minimize crossings
        /// </summary>
        private void OrderNodeLevels()
        {
            for (int i = 0; i < RobotGraph.Count; i++)//set initial level indexs
                for (int j = 0; j < RobotGraph[i].Count; j++)
                    RobotGraph[i][j].LevelIndex = j;
            for (int count = 0; count < 3;count++ )
                for (int i = 1; i < RobotGraph.Count; i++)
                {
                    List<GraphNode> tempList = new List<GraphNode>();
                    foreach (GraphNode g in RobotGraph[i])
                    {
                        double parentAverage = 0;
                        foreach (GraphNode g2 in g.ParentNodes)
                        {
                            parentAverage += g2.LevelIndex;
                        }
                        parentAverage /= g.ParentNodes.Count;
                        g.ParentNodeAverage = parentAverage;
                        double childAverage = 0;
                        foreach (GraphNode g2 in g.ChildNodes)
                        {
                            if (g2.IsFakeNode)
                                childAverage += g2.TraceNodes(false).LevelIndex;
                            else
                                childAverage += g2.LevelIndex;
                        }
                        g.FillLowerNodes();

                        if (g.ChildNodes.Count == 0)
                            g.ChildNodeAverage = double.MaxValue;
                        else
                            g.ChildNodeAverage = childAverage;
                        bool wasInserted = false;
                        for (int j = 0; j < tempList.Count; j++)
                        {
                            if (tempList[j].ParentNodeAverage - g.ParentNodeAverage > .000001)
                            {
                                tempList.Insert(j, g);
                                g.LevelIndex = j;
                                wasInserted = true;
                                for (int k = j + 1; k < tempList.Count; k++)
                                {
                                    tempList[k].LevelIndex++;
                                }
                                break;
                            }
                            else if (Math.Abs(tempList[j].ParentNodeAverage - g.ParentNodeAverage) < .0000001)
                            {
                                if (j < tempList.Count - 1 && tempList[j].LowerNodes.Overlaps(g.LowerNodes) && !tempList[j + 1].LowerNodes.Overlaps(g.LowerNodes))
                                {
                                    tempList.Insert(j, g);
                                    g.LevelIndex = j;
                                    wasInserted = true;
                                    for (int k = j + 1; k < tempList.Count; k++)
                                    {
                                        tempList[k].LevelIndex++;
                                    }
                                    break;
                                }
                                else
                                    if (tempList[j].ChildNodeAverage > g.ChildNodeAverage)
                                    {
                                        tempList.Insert(j, g);
                                        g.LevelIndex = j;
                                        wasInserted = true;
                                        for (int k = j + 1; k < tempList.Count; k++)
                                        {
                                            tempList[k].LevelIndex++;
                                        }
                                        break;
                                    }
                            }
                        }
                        if (!wasInserted)
                        {
                            g.LevelIndex = tempList.Count;
                            tempList.Add(g);
                        }


                    }
                    RobotGraph[i] = tempList;
                }
            
        }

        /// <summary>
        /// Sets the position of each node
        /// </summary>
        private void SetNodeXLocs()
        {
            //Brandes-K¨opf algorithm
            int[][][] layouts = new int[4][][];//create an array that will contain the 4 layouts in the order TL,TR,BL,BR
            for(int i = 0; i<layouts.Length;i++)
            {
                layouts[i] = new int[RobotGraph.Count][];
                for (int j = 0; j < layouts[i].Length; j++)
                {
                    layouts[i][j] = new int[RobotGraph[j].Count];
                }
            }
            //iterates through each layer, aligning them to different corners
            for (int i = 0; i < layouts.Length;i++ )
            {
                
                foreach (List<GraphNode> layer in RobotGraph)
                {
                    if (i % 2 == 0)
                    {
                        int index = -1;
                        foreach (GraphNode g in layer)
                        {
                            FindRootNode(g, true, i <= 1, ref index);
                        }
                    }
                    else
                    {
                        int index = int.MaxValue;
                        for (int j = layer.Count - 1; j >= 0; j--)
                        {
                            FindRootNode(layer[j], false, i <= 1, ref index);
                        }
                    }
                    
                }
                //Gets a list of all root nodes
                List<GraphNode> roots = new List<GraphNode>();
                foreach(List<GraphNode> layer in RobotGraph)
                {
                    foreach (GraphNode g in layer)
                    {
                        if (g.RootNode == g)
                        {
                            bool wasInserted = false;
                            for (int j = 0; j < roots.Count; j++)
                            {
                                //System.Diagnostics.Debug.WriteLine("Temp:" + roots[j].SubNodes[g.GraphLevel].LevelIndex + " / " + g.LevelIndex);
                                if (roots[j].SubNodes[g.GraphLevel] != null && roots[j].SubNodes[g.GraphLevel].LevelIndex > g.LevelIndex)
                                {
                                    roots.Insert(j, g);
                                    wasInserted = true;
                                    break;
                                }
                            }
                            if (!wasInserted)
                            {
                                roots.Add(g);
                            }
                        }
                            

                    }
                }
                int[] bound = new int[RobotGraph.Count];
                //places each root as close as possible to eachother
                //shifts all to left or right depending on bias of current layout
                if (i % 2 == 0)
                    foreach (GraphNode g in roots)
                    {
                        int largestDist = 0;
                        for (int l = 0; l < bound.Length; l++)
                        {
                            float levelWidth = 0;
                            if (g.SubNodes[l] != null)
                            {
                                levelWidth = g.SubNodes[l].GetWidth();
                                if (bound[l] + levelWidth / 2 + minSpacing > largestDist)
                                {
                                    largestDist = (int)(bound[l] + levelWidth / 2 + minSpacing);
                                }
                            }

                        }
                        g.XPos = largestDist;

                        for (int l = 0; l < bound.Length; l++)
                        {
                            float disp = 0;
                            if (g.SubNodes[l] != null)
                            {
                                disp = g.XPos + g.SubNodes[l].GetWidth() / 2;
                            }
                            if (bound[l] < disp)
                            {
                                bound[l] = (int)(disp);
                            }
                        }
                        //System.Diagnostics.Debug.WriteLine(i + " " + g.ToString() + " " + g.XPos);
                    }
                else
                {
                    for (int index = roots.Count - 1; index >= 0; index--)
                    {
                        int largestDist = 0;
                        for (int l = 0; l < bound.Length; l++)
                        {
                            float levelWidth = 0;
                            if (roots[index].SubNodes[l] != null)
                            {
                                levelWidth = roots[index].SubNodes[l].GetWidth();
                                if (bound[l] + levelWidth / 2 + minSpacing > largestDist)
                                {
                                    largestDist = (int)(bound[l] + levelWidth / 2 + minSpacing);
                                }
                            }

                        }
                        roots[index].XPos = largestDist;

                        for (int l = 0; l < bound.Length; l++)
                        {
                            float disp = 0;
                            if (roots[index].SubNodes[l] != null)
                            {
                                disp = roots[index].XPos + roots[index].SubNodes[l].GetWidth() / 2;
                            }
                            if (bound[l] < disp)
                            {
                                bound[l] = (int)(disp);
                            }
                        }

                    }
                    float tempWidth= bound.Max();
                    
                    foreach (GraphNode g in roots)
                    {
                        g.XPos = (int)(tempWidth - g.XPos);
                        //System.Diagnostics.Debug.WriteLine(i+ " " + g.ToString() + " " + g.XPos);
                    }
                        

                }
                    
                //assigns the x coord to all nodes in this layout
                for (int level = 0; level < RobotGraph.Count; level++)
                {
                    for (int index = 0; index < RobotGraph[level].Count; index++)
                    {
                        layouts[i][level][index] = RobotGraph[level][index].RootNode.XPos;
                    }
                }


                
                //resets root nodes
                foreach (List<GraphNode> layer in RobotGraph)
                {
                    foreach (GraphNode g in layer)
                    {
                        g.RootNode=null;
                    }
                }

            }
            //median average the 4 layouts
            for (int level = 0; level < RobotGraph.Count; level++)
            {
                for (int index = 0; index < RobotGraph[level].Count; index++)
                {
                    int[] values = new int[4];
                    for (int i = 0; i < layouts.Length; i++)
                    {
                        values[i] = layouts[i][level][index];
                            
                    }
                    Array.Sort(values);
                    RobotGraph[level][index].XPos = (values[1]+values[2])/2;
                }
            }

        }

        /// <summary>
        /// Finds the root node of the selected node and sets it
        /// </summary>
        /// <param name="node">Node to have the root of found</param>
        /// <param name="leftBias">True if the scan should go left-right, false if right-left</param>
        /// <param name="lookUpwards">True if the scan should look at the parents, false if it will look at children</param>
        /// <param name="cutoffIndex">The index that is the cutoff for a valid root</param>
        private void FindRootNode(GraphNode node, bool leftBias, bool lookUpwards, ref int cutoffIndex)
        {

            //System.Diagnostics.Debug.WriteLine(node.ToString());
            List<GraphNode> canidates = new List<GraphNode>();
            if (lookUpwards)
            {
                if (node.ParentNodes.Count > 0)
                {
                    int upperBound = node.ParentNodes.Count / 2;
                    int lowerBound = (int)(node.ParentNodes.Count / 2.0 - .5);
                    int count = 0;
                    foreach (GraphNode g in RobotGraph[node.GraphLevel - 1])
                    {
                        if (node.ParentNodes.Contains(g))
                        {
                            if (count <= upperBound && count >= lowerBound)
                            {
                                canidates.Add(g);
                            }
                            count++;
                        }
                    }

                    if (canidates.Count > 0)
                    {
                        if (leftBias)
                        {
                            foreach (GraphNode g in canidates)
                            {
                                if (g.LevelIndex > cutoffIndex)
                                {
                                    node.RootNode = g.RootNode;
                                    node.RootNode.SubNodes[node.GraphLevel] = node;
                                    cutoffIndex=g.LevelIndex;
                                    return;
                                }
                            }
                        }
                        else
                        {
                            for (int i = canidates.Count - 1; i >= 0; i--)
                            {
                                if (canidates[i].LevelIndex < cutoffIndex)
                                {
                                    node.RootNode = canidates[i].RootNode;
                                    node.RootNode.SubNodes[node.GraphLevel] = node;
                                    cutoffIndex = canidates[i].LevelIndex;
                                    return;
                                }
                            }
                        }
                    }
                }
                node.RootNode = node;
                node.SubNodes = new GraphNode[RobotGraph.Count];
                node.SubNodes[node.GraphLevel] = node;
                return;

            }
            else
            {
                if (node.RootNode == null)
                {
                    node.RootNode = node;
                    node.SubNodes = new GraphNode[RobotGraph.Count];
                    node.SubNodes[node.GraphLevel] = node;
                }
                if (node.ChildNodes.Count > 0)
                {
                    int upperBound = node.ChildNodes.Count / 2;
                    int lowerBound = (int)(node.ChildNodes.Count / 2.0 - .5);
                    int count = 0;
                    foreach (GraphNode g in RobotGraph[node.GraphLevel + 1])
                    {
                        if (node.ChildNodes.Contains(g))
                        {
                            if (count <= upperBound && count >= lowerBound)
                            {
                                canidates.Add(g);
                            }
                            count++;
                        }
                    }
                    if (canidates.Count > 0)
                    {
                        if (leftBias)
                        {
                            foreach (GraphNode g in canidates)
                            {
                                if (g.LevelIndex > cutoffIndex)
                                {
                                    g.RootNode = node.RootNode;
                                    g.RootNode.SubNodes[g.GraphLevel] = g;
                                    cutoffIndex = g.LevelIndex;
                                    return;
                                }
                            }
                        }
                        else
                        {
                            for (int i = canidates.Count - 1; i >= 0; i--)
                            {
                                if (canidates[i].LevelIndex < cutoffIndex)
                                {
                                    canidates[i].RootNode = node.RootNode;
                                    canidates[i].RootNode.SubNodes[canidates[i].GraphLevel] = canidates[i];
                                    cutoffIndex = canidates[i].LevelIndex;
                                    return;
                                }
                            }
                        }
                    }
                }
            } 
                

        }

        /// <summary>
        /// Calculates the relative positions of the nodes to each other in the graph to reduce cross overs
        /// </summary>
        private void CalcNodeLocations()
        {
            System.Diagnostics.Debug.WriteLine("Recalculating graph");
            CreateGraphNodes();
            RemoveGraphCycles();
            SetNodeLevels();
            OrderNodeLevels();
            SetNodeXLocs();
            totalWidth = 0;
            foreach (List<GraphNode> layer in RobotGraph)
            {
                GraphNode lastNode = layer[layer.Count-1];
                if (totalWidth < lastNode.XPos + lastNode.GetWidth() / 2 + minSpacing)
                    totalWidth = lastNode.XPos + lastNode.GetWidth() / 2 + minSpacing;
            }
            totalHeight = (RobotGraph.Count + 1) * GraphNode.VerticalSpacing + RobotGraph.Count * GraphNode.linkBoxHeight;
            System.Diagnostics.Debug.WriteLine("Finished recalculating graph");
        }
        #endregion

        private void panel1_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            panel1.Invalidate();
            //panel1.CreateGraphics()
            //e.UseDefaultCursors = false;
            //SetCursor();
        }


        private void SetCursor()
        {
            Bitmap bmp = new Bitmap(CommandManager.NewJointPic);
            bmp.MakeTransparent(Color.White);
            Cursor.Current = new Cursor(bmp.GetHicon());
        }
    }
}
