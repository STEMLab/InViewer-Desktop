using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InviewerDesktopGUI
{
    public partial class IndoorGMLViewerForm : Form
    {
        [DllImport("User32.dll")]
        static extern bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool redraw);

        internal delegate int WindowEnumProc(IntPtr hwnd, IntPtr lparam);
        [DllImport("user32.dll")]
        internal static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc func, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private delegate void UpdateLogCallback(string text);

        private Process process;
        private IntPtr unityHWND = IntPtr.Zero;
        private const int WM_ACTIVATE = 0x0006;
        private readonly IntPtr WA_ACTIVE = new IntPtr(1);
        private readonly IntPtr WA_INACTIVE = new IntPtr(0);

        private BackgroundWorker backgroundWorkerReader;
        private BackgroundWorker backgroundWorkerWriter;
        private StreamString serverStream;
        private int lastViewDirection = 1;

        private NamedPipeServerStream NPtoUnity;
        private NamedPipeServerStream NPtoGUI;

        public IndoorGMLViewerForm()
        {
            InitializeComponent();

            try
            {
                NPtoUnity = new NamedPipeServerStream("NPtoUnity", PipeDirection.InOut, 1, PipeTransmissionMode.Message);
                NPtoGUI = new NamedPipeServerStream("NPtoGUI", PipeDirection.InOut, 1, PipeTransmissionMode.Message);

                backgroundWorkerReader = new BackgroundWorker();
                backgroundWorkerReader.DoWork += new DoWorkEventHandler(backgroundWorker_GUI_Reader);
                backgroundWorkerReader.WorkerReportsProgress = true;
                backgroundWorkerReader.RunWorkerAsync();

                backgroundWorkerWriter = new BackgroundWorker();
                backgroundWorkerWriter.DoWork += new DoWorkEventHandler(backgroundWorker_GUI_Writer);
                backgroundWorkerWriter.WorkerReportsProgress = true;
                backgroundWorkerWriter.RunWorkerAsync();

                process = new Process();
                process.StartInfo.FileName = @"Inviewer-Desktop.exe";
                process.StartInfo.Arguments = "-parentHWND " + splitContainer_Right.Panel1.Handle.ToInt32() + " " + Environment.CommandLine;
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.CreateNoWindow = true;                        

                process.Start();
                process.WaitForInputIdle();

                EnumChildWindows(splitContainer_Right.Panel1.Handle, WindowEnum, IntPtr.Zero);

                NPtoUnity.WaitForConnection();
                NPtoGUI.WaitForConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw ex;
            }
        }

        private void ActivateUnityWindow()
        {
            SendMessage(unityHWND, WM_ACTIVATE, WA_ACTIVE, IntPtr.Zero);
        }

        private void DeactivateUnityWindow()
        {
            SendMessage(unityHWND, WM_ACTIVATE, WA_INACTIVE, IntPtr.Zero);
        }

        private int WindowEnum(IntPtr hwnd, IntPtr lparam)
        {
            unityHWND = hwnd;
            ActivateUnityWindow();
            return 0;
        }

        private void Panel_Unity3D_Resize(object sender, EventArgs e)
        {
            MoveWindow(unityHWND, 0, 0, splitContainer_Right.Panel1.Width, splitContainer_Right.Panel1.Height, true);
            ActivateUnityWindow();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                NPtoUnity.Close();

                process.CloseMainWindow();

                while (process.HasExited == false)
                {
                    process.Kill();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            ActivateUnityWindow();
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            DeactivateUnityWindow();
        }

        private void backgroundWorker_GUI_Writer(object sender, DoWorkEventArgs e)
        {
            //Init
            //UpdateLogCallback updateLog = new UpdateLogCallback(UpdateLog);

            try
            {
                while (NPtoUnity.IsConnected == false)
                {
                    Thread.Sleep(100);
                }

                serverStream = new StreamString(NPtoUnity);

            }
            catch (Exception ex)
            {
                //Invoke(updateLog, new object[] { ex.Message });
            }
        }

        private void SendToUnity(string uiBuffer)
        {
            serverStream.WriteString(uiBuffer);
        }

        private void backgroundWorker_GUI_Reader(object sender, DoWorkEventArgs e)
        {
            //Init
            UpdateLogCallback updateTree = new UpdateLogCallback(UpdateTree);
            string dataFromUnity = null;

            try
            {
                while (NPtoGUI.IsConnected == false)
                {
                    Thread.Sleep(100);
                }

                StreamString serverStream = new StreamString(NPtoGUI);

                while (true)
                {
                    int msgLength = Convert.ToInt32(serverStream.ReadString());
                    byte[] readBytes = new byte[msgLength];
                    NPtoGUI.Read(readBytes, 0, msgLength);
                    dataFromUnity = Encoding.ASCII.GetString(readBytes);

                    CommonTree loadedTree = new CommonTree();
                    Invoke(updateTree, new object[] { dataFromUnity });
                }
            }
            catch (Exception ex)
            {
                //Handle usual Communication Exceptions here - just logging here for demonstration and debugging Purposes
                //Invoke(updateLog, new object[] { ex.Message });
            }
        }

        private void UpdateTree(string jsonTree)
        {
            // GUI -> Unity3D 로 전송된 트리 재연
            // Root
            // ㄴCellSpace
            // ㄴGeneralSpace
            // ㄴCellSpaceBoundary
            // ㄴState
            // ㄴTransition

            CommonTree tree = JsonConvert.DeserializeObject<CommonTree>(jsonTree);

            treeView_IndoorGML.Nodes.Clear();

            TreeNode partSpace;

            partSpace = treeView_IndoorGML.Nodes.Add(CommonNames.ROOT_CELLSPACE);
            for(int idxOfSpace = 0; idxOfSpace < tree.ROOT_CELLSPACE.Length; idxOfSpace++)
            {
                var space = partSpace.Nodes.Add(tree.ROOT_CELLSPACE[idxOfSpace]);
                for (int idxOfFace = 0; idxOfFace < tree.ROOT_CELLSPACE_FACES_CNT[idxOfSpace]; idxOfFace++)
                {
                    space.Nodes.Add(string.Format("{0}_Face:{1}", space.Text, idxOfFace + 1));
                }
            }

            partSpace = treeView_IndoorGML.Nodes.Add(CommonNames.ROOT_GENERALSPACE);
            for (int idxOfSpace = 0; idxOfSpace < tree.ROOT_GENERALSPACE.Length; idxOfSpace++)
            {
                var space = partSpace.Nodes.Add(tree.ROOT_GENERALSPACE[idxOfSpace]);
                for (int idxOfFace = 0; idxOfFace < tree.ROOT_GENERALSPACE_FACES_CNT[idxOfSpace]; idxOfFace++)
                {
                    space.Nodes.Add(string.Format("{0}_Face:{1}", space.Text, idxOfFace + 1));
                }
            }

            partSpace = treeView_IndoorGML.Nodes.Add(CommonNames.ROOT_TRANSITIONSPACE);
            for (int idxOfSpace = 0; idxOfSpace < tree.ROOT_TRANSITIONSPACE.Length; idxOfSpace++)
            {
                var space = partSpace.Nodes.Add(tree.ROOT_TRANSITIONSPACE[idxOfSpace]);
                for (int idxOfFace = 0; idxOfFace < tree.ROOT_TRANSITIONSPACEFACES_CNT[idxOfSpace]; idxOfFace++)
                {
                    space.Nodes.Add(string.Format("{0}_Face:{1}", space.Text, idxOfFace + 1));
                }
            }

            partSpace = treeView_IndoorGML.Nodes.Add(CommonNames.ROOT_CELLSPACEBOUNDARY);
            for (int idxOfSpace = 0; idxOfSpace < tree.ROOT_CELLSPACEBOUNDARY.Length; idxOfSpace++)
            {
                var space = partSpace.Nodes.Add(tree.ROOT_CELLSPACEBOUNDARY[idxOfSpace]);
            }

            partSpace = treeView_IndoorGML.Nodes.Add(CommonNames.ROOT_STATE);
            for (int idxOfSpace = 0; idxOfSpace < tree.ROOT_STATE.Length; idxOfSpace++)
            {
                var space = partSpace.Nodes.Add(tree.ROOT_STATE[idxOfSpace]);
            }

            partSpace = treeView_IndoorGML.Nodes.Add(CommonNames.ROOT_TRANSITION);
            for (int idxOfSpace = 0; idxOfSpace < tree.ROOT_TRANSITION.Length; idxOfSpace++)
            {
                var space = partSpace.Nodes.Add(tree.ROOT_TRANSITION[idxOfSpace]);
            }

            // 모든 트리를 checked 처리.
            CheckAllNodes(treeView_IndoorGML.Nodes);
        }

        public void CheckAllNodes(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                node.Checked = true;
                CheckChildren(node, true);
            }
        }

        public void UncheckAllNodes(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                node.Checked = false;
                CheckChildren(node, false);
            }
        }

        private void CheckChildren(TreeNode rootNode, bool isChecked)
        {
            foreach (TreeNode node in rootNode.Nodes)
            {
                CheckChildren(node, isChecked);
                node.Checked = isChecked;
            }
        }

        //private void UpdateLogForDebug(string text)
        //{
        //    lock (textBox_Log)
        //    {
        //        textBox_Log.Text = text;
        //    }
        //}

        List<TreeNode> checkedNodes = new List<TreeNode>();

        void RemoveCheckedNodes(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node == null)
                {
                    continue;
                }

                if (node.Checked)
                {
                    checkedNodes.Add(node);
                }
                else
                {
                    RemoveCheckedNodes(node.Nodes);
                }
            }

            foreach (TreeNode checkedNode in checkedNodes)
            {
                nodes.Remove(checkedNode);
            }
        }

        private void CheckAllChildNodes(TreeNode treeNode, bool nodeChecked)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                node.Checked = nodeChecked;

                if (node.Nodes.Count > 0)
                {
                    this.CheckAllChildNodes(node, nodeChecked);
                }
            }
        }

        private void node_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                // 하위 노드들이 포함된 경우.                
                IndoorGMLTreeViewValidation(e);
            }
        }

        private void IndoorGMLTreeViewValidation(TreeViewEventArgs e)
        {
            if (e.Node.Nodes.Count > 0)
            {
                this.CheckAllChildNodes(e.Node, e.Node.Checked);

                StringBuilder sb = new StringBuilder();
                int cnt = 0;

                if (e.Node.Checked == true)
                {
                    sb.Append("SHOW|");
                }
                else
                {
                    sb.Append("HIDE|");
                }

                if (e.Node.Text == CommonNames.ROOT_CELLSPACE
                    || e.Node.Text == CommonNames.ROOT_CELLSPACEBOUNDARY
                    || e.Node.Text == CommonNames.ROOT_GENERALSPACE
                    || e.Node.Text == CommonNames.ROOT_STATE
                    || e.Node.Text == CommonNames.ROOT_TRANSITION
                    || e.Node.Text == CommonNames.ROOT_TRANSITIONSPACE
                    )
                {
                    sb.Append(e.Node.Text);
                }
                else
                {
                    foreach (TreeNode node in e.Node.Nodes)
                    {
                        if (node.Nodes.Count > 0)
                        {
                            foreach (TreeNode leaf in node.Nodes)
                            {
                                cnt++;
                                sb.Append(leaf.Text);
                                sb.Append(' ');
                            }
                        }
                        else
                        {
                            cnt++;
                            sb.Append(node.Text);
                            sb.Append(' ');
                        }
                    }
                }

                SendToUnity(sb.ToString().TrimEnd());
            }
            else
            {
                // 체크된 노드가 자식이 없는 경우.

                StringBuilder sb = new StringBuilder();

                if (e.Node.Checked == true)
                {
                    SendToUnity(string.Format("SHOW|{0}", e.Node.Text));
                }
                else
                {
                    SendToUnity(string.Format("HIDE|{0}", e.Node.Text));
                }
            }
        }

        private static bool IsSolidType(TreeViewEventArgs e)
        {
            return e.Node.Parent.Text == CommonNames.ROOT_CELLSPACE ||
                                    e.Node.Parent.Text == CommonNames.ROOT_GENERALSPACE ||
                                    e.Node.Parent.Text == CommonNames.ROOT_TRANSITIONSPACE;
        }

        private void openIndoorGMLOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CmdOpenIndoorGML();
        }

        private void CmdOpenIndoorGML()
        {
            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.Filter = "IndoorGML File(*.gml)|*.gml";
            openFileDlg.ShowDialog();
            if (openFileDlg.FileName.Length > 0)
            {
                SendToUnity("OPEN|" + openFileDlg.FileName);
                richTextBox_IndoorGML.Text = System.IO.File.ReadAllText(openFileDlg.FileName);
            }
        }

        private void treeView_IndoorGML_DoubleClick(object sender, EventArgs e)
        {
            GotoSelectedItem();
        }

        private void GotoSelectedItem()
        {
            if (treeView_IndoorGML.SelectedNode != null)
            {
                SendToUnity(string.Format("GOTO|{0}", treeView_IndoorGML.SelectedNode.Text));
            }
        }

        private void quitQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolStripButton_OpenFile_Click(object sender, EventArgs e)
        {
            CmdOpenIndoorGML();
        }

        private void toolStripButton_Fit_Click(object sender, EventArgs e)
        {
            CmdViewFit();
        }

        private void toolStripButton_SideView_Click(object sender, EventArgs e)
        {
            CmdViewSide();
        }

        private void toolStripButton_FrontView_Click(object sender, EventArgs e)
        {
            CmdViewFront();
        }

        private void toolStripButton_TopView_Click(object sender, EventArgs e)
        {
            CmdViewTop();
        }

        private void CmdViewFront()
        {
            lastViewDirection = 2;
            SendToUnity(string.Format("VIEW|{0}", lastViewDirection));
        }
        
        private void CmdViewFit()
        {
            lastViewDirection = 1;
            SendToUnity(string.Format("VIEW|{0}", lastViewDirection));
        }

        private void CmdViewSide()
        {
            lastViewDirection = 4;
            SendToUnity(string.Format("VIEW|{0}", lastViewDirection));
        }

        private void CmdViewTop()
        {
            lastViewDirection = 5;
            SendToUnity(string.Format("VIEW|{0}", lastViewDirection));
        }



        private void toolStripButton_Ortho_Click(object sender, EventArgs e)
        {
            if (((ToolStripButton)sender).CheckState == CheckState.Checked)
            {
                cmdViewOrthogonal();
            }
            else
            {
                cmdViewPerspective();
            }
        }

        private void cmdViewPerspective()
        {
            SendToUnity("VIEW|PERSPECTIVE");
        }

        private void cmdViewOrthogonal()
        {
            SendToUnity("VIEW|ORTHOGONAL");
        }

        private void cmdQueryOn()
        {
            SendToUnity("QUERY|ON");
        }

        private void cmdQueryOff()
        {
            SendToUnity("QUERY|OFF");
        }

        private void toolStripButton_RotateView_Click(object sender, EventArgs e)
        {
            if(lastViewDirection == 1)
            {
                lastViewDirection = 3;
            }
            else if (lastViewDirection == 3)
            {
                lastViewDirection = 9;
            }
            else if (lastViewDirection == 9)
            {
                lastViewDirection = 7;
            }
            else if (lastViewDirection == 7)
            {
                lastViewDirection = 1;
            }
            else if (lastViewDirection == 2)
            {
                lastViewDirection = 6;
            }
            else if (lastViewDirection == 6)
            {
                lastViewDirection = 8;
            }
            else if (lastViewDirection == 8)
            {
                lastViewDirection = 4;
            }
            else if (lastViewDirection == 4)
            {
                lastViewDirection = 2;
            }
            else if (lastViewDirection == 5)
            {
                lastViewDirection = 51;
            }
            else if (lastViewDirection == 51)
            {
                lastViewDirection = 52;
            }
            else if (lastViewDirection == 52)
            {
                lastViewDirection = 53;
            }
            else if (lastViewDirection == 53)
            {
                lastViewDirection = 5;
            }

            SendToUnity(string.Format("VIEW|{0}", lastViewDirection));
        }


        private void cellSpaceBoundaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (((ToolStripMenuItem)sender).CheckState == CheckState.Checked)
            {
                SendToUnity(string.Format("CULLON|{0}", CommonNames.ROOT_CELLSPACEBOUNDARY));
            }
            else
            {
                SendToUnity(string.Format("CULLOFF|{0}", CommonNames.ROOT_CELLSPACEBOUNDARY));
            }
        }

        private void cellSpaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(((ToolStripMenuItem)sender).CheckState == CheckState.Checked)
            {
                SendToUnity(string.Format("CULLON|{0}", CommonNames.ROOT_CELLSPACE));
            }
            else
            {
                SendToUnity(string.Format("CULLOFF|{0}", CommonNames.ROOT_CELLSPACE));
            }
        }

        private void generalSpaceToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (((ToolStripMenuItem)sender).CheckState == CheckState.Checked)
            {
                SendToUnity(string.Format("CULLON|{0}", CommonNames.ROOT_GENERALSPACE));
            }
            else
            {
                SendToUnity(string.Format("CULLOFF|{0}", CommonNames.ROOT_GENERALSPACE));
            }
        }

        private void transitionSpaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (((ToolStripMenuItem)sender).CheckState == CheckState.Checked)
            {
                SendToUnity(string.Format("CULLON|{0}", CommonNames.ROOT_TRANSITIONSPACE));
            }
            else
            {
                SendToUnity(string.Format("CULLOFF|{0}", CommonNames.ROOT_TRANSITIONSPACE));
            }
        }

        private void treeView_IndoorGML_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView_IndoorGML.SelectedNode != null)
            {
                SendToUnity(string.Format("SELECT|{0}", treeView_IndoorGML.SelectedNode.Text));
            }

            int startPos = richTextBox_IndoorGML.Find(treeView_IndoorGML.SelectedNode.Text);
            int length = treeView_IndoorGML.SelectedNode.Text.Length;
            if (startPos != -1)
            {
                richTextBox_IndoorGML.Focus();
                richTextBox_IndoorGML.Select(startPos, length);
            }
        }

        private void middleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendToUnity("STATE|1");
            MenuGroupCheck(sender);
        }

        private void largeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendToUnity("STATE|2");
            MenuGroupCheck(sender);
        }

        private void smallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendToUnity("STATE|0.5");
            MenuGroupCheck(sender);
        }

        private static void MenuGroupCheck(object sender)
        {
            foreach (ToolStripMenuItem item in ((ToolStripMenuItem)sender).GetCurrentParent().Items)
            {
                if (item == sender) item.Checked = true;
                if ((item != null) && (item != sender))
                {
                    item.Checked = false;
                }
            }
        }

        private void hideSelectedItemsHToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeView_IndoorGML.SelectedNode.Checked = false;
            IndoorGMLTreeViewValidation(new TreeViewEventArgs(treeView_IndoorGML.SelectedNode));
        }

        private void shotSelectedItemsSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeView_IndoorGML.SelectedNode.Checked = true;
            IndoorGMLTreeViewValidation(new TreeViewEventArgs(treeView_IndoorGML.SelectedNode));
        }

        private void toggleSelectedItemsAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeView_IndoorGML.SelectedNode.Checked = !treeView_IndoorGML.SelectedNode.Checked;
            IndoorGMLTreeViewValidation(new TreeViewEventArgs(treeView_IndoorGML.SelectedNode));
        }

        private void goToSelectedItemGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GotoSelectedItem();
        }

        private void perspectiveViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cmdViewPerspective();
            MenuGroupCheck(sender);
        }

        private void orthographicViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cmdViewOrthogonal();
            MenuGroupCheck(sender);
        }

        private void sideViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CmdViewSide();
        }

        private void frontViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CmdViewFront();
        }

        private void topViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CmdViewTop();
        }

        private void toolStripButton_Query_Click(object sender, EventArgs e)
        {
            if (((ToolStripButton)sender).CheckState == CheckState.Checked)
            {
                cmdQueryOn();
            }
            else
            {
                cmdQueryOff();
            }
        }

        private void richTextBox_IndoorGML_SelectionChanged(object sender, EventArgs e)
        {
            int index = richTextBox_IndoorGML.SelectionStart;
            
            int line = richTextBox_IndoorGML.GetLineFromCharIndex(index);
            toolStripStatusLabel1.Text = string.Format("Cursor at line {0:n0} / {1:n0}", line, richTextBox_IndoorGML.Lines.Length);
        }
    }
}
