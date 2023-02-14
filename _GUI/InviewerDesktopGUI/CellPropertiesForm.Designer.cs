
namespace InviewerDesktopGUI
{
    partial class CellPropertiesForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_description = new System.Windows.Forms.TextBox();
            this.textBox_id = new System.Windows.Forms.TextBox();
            this.textBox_name = new System.Windows.Forms.TextBox();
            this.textBox_storey = new System.Windows.Forms.TextBox();
            this.textBox_type = new System.Windows.Forms.TextBox();
            this.button_cancel = new System.Windows.Forms.Button();
            this.button_submit = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.listBox_hitList = new System.Windows.Forms.ListBox();
            this.textBox_From = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox_To = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.button_setFrom = new System.Windows.Forms.Button();
            this.button_setTo = new System.Windows.Forms.Button();
            this.button_goNormal = new System.Windows.Forms.Button();
            this.button_goFire = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button_resetPath = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(169, 92);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Cell ID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(169, 130);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(169, 170);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "Storey";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.Location = new System.Drawing.Point(169, 208);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 20);
            this.label4.TabIndex = 3;
            this.label4.Text = "Type";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.Location = new System.Drawing.Point(169, 255);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(103, 20);
            this.label5.TabIndex = 4;
            this.label5.Text = "Description";
            // 
            // textBox_description
            // 
            this.textBox_description.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_description.Location = new System.Drawing.Point(170, 285);
            this.textBox_description.Multiline = true;
            this.textBox_description.Name = "textBox_description";
            this.textBox_description.Size = new System.Drawing.Size(368, 188);
            this.textBox_description.TabIndex = 5;
            // 
            // textBox_id
            // 
            this.textBox_id.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_id.Location = new System.Drawing.Point(253, 88);
            this.textBox_id.Name = "textBox_id";
            this.textBox_id.Size = new System.Drawing.Size(285, 30);
            this.textBox_id.TabIndex = 6;
            // 
            // textBox_name
            // 
            this.textBox_name.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_name.Location = new System.Drawing.Point(253, 128);
            this.textBox_name.Name = "textBox_name";
            this.textBox_name.Size = new System.Drawing.Size(285, 30);
            this.textBox_name.TabIndex = 7;
            // 
            // textBox_storey
            // 
            this.textBox_storey.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_storey.Location = new System.Drawing.Point(253, 166);
            this.textBox_storey.Name = "textBox_storey";
            this.textBox_storey.Size = new System.Drawing.Size(285, 30);
            this.textBox_storey.TabIndex = 8;
            // 
            // textBox_type
            // 
            this.textBox_type.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_type.Location = new System.Drawing.Point(253, 206);
            this.textBox_type.Name = "textBox_type";
            this.textBox_type.Size = new System.Drawing.Size(285, 30);
            this.textBox_type.TabIndex = 9;
            // 
            // button_cancel
            // 
            this.button_cancel.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_cancel.Location = new System.Drawing.Point(170, 508);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(157, 48);
            this.button_cancel.TabIndex = 10;
            this.button_cancel.Text = "Cancel";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // button_submit
            // 
            this.button_submit.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_submit.Location = new System.Drawing.Point(382, 508);
            this.button_submit.Name = "button_submit";
            this.button_submit.Size = new System.Drawing.Size(157, 48);
            this.button_submit.TabIndex = 11;
            this.button_submit.Text = "Submit";
            this.button_submit.UseVisualStyleBackColor = true;
            this.button_submit.Click += new System.EventHandler(this.button_submit_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Blue;
            this.label6.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.label6.Location = new System.Drawing.Point(180, 22);
            this.label6.Name = "label6";
            this.label6.Padding = new System.Windows.Forms.Padding(100, 10, 100, 10);
            this.label6.Size = new System.Drawing.Size(359, 44);
            this.label6.TabIndex = 12;
            this.label6.Text = "Cell Properties";
            // 
            // listBox_hitList
            // 
            this.listBox_hitList.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.listBox_hitList.FormattingEnabled = true;
            this.listBox_hitList.ItemHeight = 24;
            this.listBox_hitList.Location = new System.Drawing.Point(11, 22);
            this.listBox_hitList.Name = "listBox_hitList";
            this.listBox_hitList.Size = new System.Drawing.Size(128, 460);
            this.listBox_hitList.TabIndex = 13;
            this.listBox_hitList.SelectedIndexChanged += new System.EventHandler(this.listBox_hitList_SelectedIndexChanged);
            // 
            // textBox_From
            // 
            this.textBox_From.Font = new System.Drawing.Font("굴림", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_From.Location = new System.Drawing.Point(87, 54);
            this.textBox_From.Name = "textBox_From";
            this.textBox_From.Size = new System.Drawing.Size(100, 26);
            this.textBox_From.TabIndex = 14;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.Location = new System.Drawing.Point(19, 60);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(50, 20);
            this.label7.TabIndex = 15;
            this.label7.Text = "From";
            // 
            // textBox_To
            // 
            this.textBox_To.Font = new System.Drawing.Font("굴림", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBox_To.Location = new System.Drawing.Point(87, 100);
            this.textBox_To.Name = "textBox_To";
            this.textBox_To.Size = new System.Drawing.Size(100, 26);
            this.textBox_To.TabIndex = 16;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label8.Location = new System.Drawing.Point(19, 101);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(32, 20);
            this.label8.TabIndex = 17;
            this.label8.Text = "To";
            // 
            // button_setFrom
            // 
            this.button_setFrom.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_setFrom.Location = new System.Drawing.Point(209, 54);
            this.button_setFrom.Name = "button_setFrom";
            this.button_setFrom.Size = new System.Drawing.Size(71, 28);
            this.button_setFrom.TabIndex = 18;
            this.button_setFrom.Text = "Set";
            this.button_setFrom.UseVisualStyleBackColor = true;
            this.button_setFrom.Click += new System.EventHandler(this.button_setFrom_Click);
            // 
            // button_setTo
            // 
            this.button_setTo.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_setTo.Location = new System.Drawing.Point(209, 100);
            this.button_setTo.Name = "button_setTo";
            this.button_setTo.Size = new System.Drawing.Size(71, 28);
            this.button_setTo.TabIndex = 19;
            this.button_setTo.Text = "Set";
            this.button_setTo.UseVisualStyleBackColor = true;
            this.button_setTo.Click += new System.EventHandler(this.button_setTo_Click);
            // 
            // button_goNormal
            // 
            this.button_goNormal.Font = new System.Drawing.Font("굴림", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_goNormal.Location = new System.Drawing.Point(63, 167);
            this.button_goNormal.Name = "button_goNormal";
            this.button_goNormal.Size = new System.Drawing.Size(157, 48);
            this.button_goNormal.TabIndex = 20;
            this.button_goNormal.Text = "Use Elevators";
            this.button_goNormal.UseVisualStyleBackColor = true;
            this.button_goNormal.Click += new System.EventHandler(this.button_goNormal_Click);
            // 
            // button_goFire
            // 
            this.button_goFire.Font = new System.Drawing.Font("굴림", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_goFire.Location = new System.Drawing.Point(63, 239);
            this.button_goFire.Name = "button_goFire";
            this.button_goFire.Size = new System.Drawing.Size(157, 48);
            this.button_goFire.TabIndex = 21;
            this.button_goFire.Text = "Stairs Only";
            this.button_goFire.UseVisualStyleBackColor = true;
            this.button_goFire.Click += new System.EventHandler(this.button_goFire_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button_resetPath);
            this.groupBox1.Controls.Add(this.button_goFire);
            this.groupBox1.Controls.Add(this.button_goNormal);
            this.groupBox1.Controls.Add(this.button_setTo);
            this.groupBox1.Controls.Add(this.button_setFrom);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.textBox_To);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.textBox_From);
            this.groupBox1.Location = new System.Drawing.Point(558, 88);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(293, 385);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Shortest Path";
            // 
            // button_resetPath
            // 
            this.button_resetPath.Font = new System.Drawing.Font("굴림", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_resetPath.Location = new System.Drawing.Point(63, 315);
            this.button_resetPath.Name = "button_resetPath";
            this.button_resetPath.Size = new System.Drawing.Size(157, 48);
            this.button_resetPath.TabIndex = 22;
            this.button_resetPath.Text = "Reset";
            this.button_resetPath.UseVisualStyleBackColor = true;
            this.button_resetPath.Click += new System.EventHandler(this.button_resetPath_Click);
            // 
            // CellPropertiesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(879, 596);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.listBox_hitList);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.button_submit);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.textBox_type);
            this.Controls.Add(this.textBox_storey);
            this.Controls.Add(this.textBox_name);
            this.Controls.Add(this.textBox_id);
            this.Controls.Add(this.textBox_description);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "CellPropertiesForm";
            this.Text = "CellPropertiesForm";
            this.TopMost = true;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_description;
        private System.Windows.Forms.TextBox textBox_id;
        private System.Windows.Forms.TextBox textBox_name;
        private System.Windows.Forms.TextBox textBox_storey;
        private System.Windows.Forms.TextBox textBox_type;
        private System.Windows.Forms.Button button_cancel;
        private System.Windows.Forms.Button button_submit;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListBox listBox_hitList;
        private System.Windows.Forms.TextBox textBox_From;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox_To;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button button_setFrom;
        private System.Windows.Forms.Button button_setTo;
        private System.Windows.Forms.Button button_goNormal;
        private System.Windows.Forms.Button button_goFire;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_resetPath;
    }
}