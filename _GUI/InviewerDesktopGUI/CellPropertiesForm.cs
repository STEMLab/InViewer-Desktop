using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InviewerDesktopGUI
{
    public partial class CellPropertiesForm : Form
    {
        Dictionary<string, string> dictName;
        Dictionary<string, string> dictStorey;
        Dictionary<string, string> dictType;
        Dictionary<string, string> dictDesc;

        IndoorGMLViewerForm _mainForm;


        public CellPropertiesForm(IndoorGMLViewerForm mainForm)
        {
            _mainForm = mainForm;

            InitializeComponent();
        }

        public void SetData(string rawInfo)
        {
            dictName = new Dictionary<string, string>();
            dictStorey = new Dictionary<string, string>();
            dictType = new Dictionary<string, string>();
            dictDesc = new Dictionary<string, string>();
            
            listBox_hitList.Items.Clear();

            textBox_id.Text = string.Empty;
            textBox_name.Text = string.Empty;
            textBox_storey.Text = string.Empty;
            textBox_type.Text = string.Empty;
            textBox_description.Text = string.Empty;

            rawInfo = rawInfo.Replace("HIT|", "");

            string[] sepRecord = { "[#record#]" };
            string[] sepID = { "[#entry#]" };
            string[] sepValues = { "[#value#]" };

            // id=name,storey,type,desc id=name.....
            string[] tokens = rawInfo.Trim().Split(sepRecord, System.StringSplitOptions.RemoveEmptyEntries);

            foreach(string token in tokens)
            {
                if(token.Length < 3)
                {
                    continue;
                }

                string id = token.Split(sepID, System.StringSplitOptions.RemoveEmptyEntries)[0];

                string name = token.Split(sepID, System.StringSplitOptions.RemoveEmptyEntries)[1].Split(sepValues, System.StringSplitOptions.RemoveEmptyEntries)[0];
                string storey = token.Split(sepID, System.StringSplitOptions.RemoveEmptyEntries)[1].Split(sepValues, System.StringSplitOptions.RemoveEmptyEntries)[1];
                string type = token.Split(sepID, System.StringSplitOptions.RemoveEmptyEntries)[1].Split(sepValues, System.StringSplitOptions.RemoveEmptyEntries)[2];
                string desc = token.Split(sepID, System.StringSplitOptions.RemoveEmptyEntries)[1].Split(sepValues, System.StringSplitOptions.RemoveEmptyEntries)[3];

                dictName.Add(id, name);
                dictStorey.Add(id, storey);
                dictType.Add(id, type);
                dictDesc.Add(id, desc);

                listBox_hitList.Items.Add(id);
            }

            if (listBox_hitList.Items.Count > 0)
            {
                listBox_hitList.SelectedIndex = 0;
            }
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            Hide();
            //Close();
        }

        private void listBox_hitList_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox_id.Text = string.Empty;
            textBox_name.Text = string.Empty;
            textBox_storey.Text = string.Empty;
            textBox_type.Text = string.Empty;
            textBox_description.Text = string.Empty;

            if(listBox_hitList.SelectedIndex < 0)
            {
                return;
            }

            string selectedID = listBox_hitList.SelectedItem.ToString();
            textBox_id.Text = selectedID;

            string name = string.Empty;
            string storey = string.Empty;
            string type = string.Empty;
            string desc = string.Empty;

            dictName.TryGetValue(selectedID, out name);
            dictStorey.TryGetValue(selectedID, out storey);
            dictType.TryGetValue(selectedID, out type);
            dictDesc.TryGetValue(selectedID, out desc);

            textBox_id.Text = selectedID;
            textBox_name.Text = name;
            textBox_storey.Text = storey;
            textBox_type.Text = type;
            textBox_description.Text = desc;

            _mainForm.SendToUnity($"SELECT|{selectedID}");
        }

        private void button_submit_Click(object sender, EventArgs e)
        {
            if(File.Exists("history.csv") == false)
            {
                using (StreamWriter sw = new StreamWriter("history.csv", true))
                {
                    sw.WriteLine("CELL_ID,NAME,STOREY,TYPE,DESCRIPTION");
                }
            }
            try
            {
                using (StreamWriter sw = new StreamWriter("history.csv", true))
                {
                    sw.WriteLine($"{textBox_id.Text},{textBox_name.Text},{textBox_storey.Text},{textBox_type.Text},{textBox_description.Text}");
                }
                MessageBox.Show("저장 되었습니다", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("파일쓰기가 실패 했습니다", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_setFrom_Click(object sender, EventArgs e)
        {
            string selectedValue = listBox_hitList.SelectedItem.ToString().Split('-')[1];
            textBox_From.Text = selectedValue;
        }

        private void button_setTo_Click(object sender, EventArgs e)
        {
            string selectedValue = listBox_hitList.SelectedItem.ToString().Split('-')[1];
            textBox_To.Text = selectedValue;
        }

        private void button_goNormal_Click(object sender, EventArgs e)
        {
            _mainForm.SendToUnity($"PATHNORMAL|{textBox_From.Text},{textBox_To.Text}");
        }

        private void button_goFire_Click(object sender, EventArgs e)
        {
            _mainForm.SendToUnity($"PATHFIRE|{textBox_From.Text},{textBox_To.Text}");
        }

        private void button_resetPath_Click(object sender, EventArgs e)
        {
            _mainForm.SendToUnity($"PATHCLEAR|_");
            textBox_From.Text = "";
            textBox_To.Text = "";
        }
    }
}
