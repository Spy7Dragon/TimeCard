using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace TimeCard
{
    partial class FrmTimeCard
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

        #region Windows Form Designer generated codeu

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblActivities = new System.Windows.Forms.Label();
            this.lblDateTime = new Label();
            this.lblSubmission = new Label();
            this.txtTime = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnSubmit = new Button();
            this.cmbBoxSplit = new ComboBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panel1 = new TableLayoutPanel();
            this.btnAddActivity = new System.Windows.Forms.Button();
            this.btnRemoveActivity = new System.Windows.Forms.Button();
            this.lblInstructions = new System.Windows.Forms.Label();
            this.lblTotalTime = new Label();
            this.lblStatus = new Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            this.Load += OnLoad;
            // 
            // menu_strip
            //
            MenuStrip menu_strip = new MenuStrip { ShowItemToolTips = true };
            ToolStripMenuItem file_item = new ToolStripMenuItem("File");
            menu_strip.Items.Add(file_item);
            ToolStripMenuItem settings_item = new ToolStripMenuItem("Settings");
            menu_strip.Items.Add(settings_item);
            Externals.BrowserMenu = new ToolStripMenuItem("Favorite Browser");
            settings_item.DropDownItems.Add(Externals.BrowserMenu);
            Externals.BrowserMenu.DropDownItems.Add("CHROME", null, Externals.SetBrowser);
            Externals.BrowserMenu.DropDownItems.Add("FIREFOX", null, Externals.SetBrowser);
            foreach (ToolStripMenuItem item in Externals.BrowserMenu.DropDownItems)
            {
                if (Externals.Browser == item.Text.ToLower())
                {
                    item.Checked = true;
                }
            }
            file_item.DropDownItems.Add("Start New Day", null, StartNewDay);
            file_item.DropDownItems.Add("Exit", null, Externals.ExitApplication);
            ToolStripMenuItem view_item = new ToolStripMenuItem("View");
            menu_strip.Items.Add(view_item);
            view_item.DropDownItems.Add("View Current Log", null, Logger.ViewCurrentLog);
            ToolStripMenuItem help_item = new ToolStripMenuItem("Help");
            menu_strip.Items.Add(help_item);
            help_item.DropDownItems.Add("Submit Request", null, Externals.SubmitRequest);
            ToolStripMenuItem calc_error_item =
                new ToolStripMenuItem("Submit Calculation Error", null, Externals.SubmitGuiError)
                {
                    ToolTipText = "Submits the values in the window as an automated Bug request.\n"
                                  + "For visible calculation errors."
                };
            help_item.DropDownItems.Add(calc_error_item);
            help_item.DropDownItems.Add("About", null, Externals.ShowAbout);
            MainMenuStrip = menu_strip;
            // 
            // lblActivities
            // 
            this.lblActivities.AutoSize = true;
            this.lblActivities.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblActivities.Location = new System.Drawing.Point(50, 25);
            this.lblActivities.Name = "lblActivities";
            this.lblActivities.Size = new System.Drawing.Size(77, 17);
            this.lblActivities.TabIndex = 2;
            this.lblActivities.Text = "Activities";
            //
            // lblDateTime
            //
            this.lblDateTime.AutoSize = true;
            this.lblDateTime.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDateTime.Location = new System.Drawing.Point(150, 25);
            this.lblDateTime.Text = DateTime.Today.ToLongDateString();
            // 
            // txtTime
            // 
            this.txtTime.Location = new System.Drawing.Point(0, 0);
            this.txtTime.Name = "txtTime";
            this.txtTime.Size = new System.Drawing.Size(75, 20);
            this.txtTime.TabIndex = 3;
            this.txtTime.DoubleClick += new System.EventHandler(this.txtTime_DoubleClick);
            this.txtTime.TextChanged += new System.EventHandler(this.txtTime_TextChanged);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(0, 25);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(20, 20);
            this.btnAdd.TabIndex = 4;
            this.btnAdd.Text = "+";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += delegate (object sender, System.EventArgs e)
            {
                btnAdd_Click(sender, e, 0);
            };
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(50, 50);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = false;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(400, 150);
            this.dataGridView1.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.HorizontalScroll.Maximum = 0;
            this.panel1.AutoScroll = false;
            this.panel1.VerticalScroll.Visible = false;
            this.panel1.AutoScroll = true;
            this.panel1.ColumnCount = 4;
            this.panel1.BorderStyle = BorderStyle.FixedSingle;
            this.panel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 25.0f));
            this.panel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 75.0f));
            this.panel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250.0f));
            this.panel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 25.0f));
            this.panel1.RowCount = 3;
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.txtTime, 0, 0);
            this.panel1.SetColumnSpan(this.txtTime, 2);
            this.panel1.Controls.Add(this.btnAdd, 0, 1);
            this.panel1.Location = new System.Drawing.Point(50, 250);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(400, 350);
            this.panel1.MinimumSize = new Size(panel1.Width, panel1.Height);
            this.panel1.MaximumSize = new Size(panel1.Width, panel1.Height);
            this.panel1.TabIndex = 6;
            // 
            // btnAddActivity
            // 
            this.btnAddActivity.Location = new System.Drawing.Point(50, 200);
            this.btnAddActivity.Name = "btnAddActivity";
            this.btnAddActivity.Size = new System.Drawing.Size(25, 25);
            this.btnAddActivity.TabIndex = 7;
            this.btnAddActivity.Text = "+";
            this.btnAddActivity.UseVisualStyleBackColor = true;
            this.btnAddActivity.Click += new System.EventHandler(this.btnAddActivity_Click);
            // 
            // btnRemoveActivity
            // 
            this.btnRemoveActivity.Location = new System.Drawing.Point(100, 200);
            this.btnRemoveActivity.Name = "btnRemoveActivity";
            this.btnRemoveActivity.Size = new System.Drawing.Size(25, 25);
            this.btnRemoveActivity.TabIndex = 8;
            this.btnRemoveActivity.Text = "-";
            this.btnRemoveActivity.UseVisualStyleBackColor = true;
            this.btnRemoveActivity.Click += new System.EventHandler(this.btnRemoveActivity_Click);
            //
            // btnSubmit
            //
            this.btnSubmit.Location = new System.Drawing.Point(50, 600);
            this.btnSubmit.Size = new System.Drawing.Size(100, 25);
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            //
            // lblStatus
            //
            this.lblStatus.Location = new System.Drawing.Point(150, 600);
            this.lblStatus.Size = new System.Drawing.Size(300, 25);
            this.lblStatus.Text = "Daily time has not been submitted.";
            this.lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            this.lblStatus.BackColor = Color.Red;
            // 
            // lblInstructions
            // 
            this.lblInstructions.AutoSize = true;
            this.lblInstructions.Location = new System.Drawing.Point(50, 230);
            this.lblInstructions.Name = "lblInstructions";
            this.lblInstructions.Size = new System.Drawing.Size(250, 15);
            this.lblInstructions.TabIndex = 9;
            this.lblInstructions.Text = "Double Click time box for current time.";
            // 
            // lblTotalTime
            //
            this.lblTotalTime.AutoSize = true;
            this.lblTotalTime.Location = new System.Drawing.Point(350, 230);
            this.lblTotalTime.Name = "lblTotalTime";
            this.lblTotalTime.Size = new System.Drawing.Size(100, 15);
            this.lblTotalTime.Text = "Total: 0.0 hrs";
            // 
            // FrmTimeCard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(480, 640);
            this.Controls.Add(menu_strip);
            this.Controls.Add(this.lblInstructions);
            this.Controls.Add(this.btnRemoveActivity);
            this.Controls.Add(this.btnAddActivity);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.cmbBoxSplit);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.lblActivities);
            this.Controls.Add(this.lblDateTime);
            this.Controls.Add(this.lblTotalTime);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblStatus);
            this.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FrmTimeCard";
            this.Text = "Time Card";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            string icon = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TimeCardIcon.ico");
            this.Icon = new Icon(icon);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblActivities;
        internal Label lblDateTime;
        private System.Windows.Forms.TextBox txtTime;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.DataGridView dataGridView1;
        internal TableLayoutPanel panel1;
        private Button btnAddActivity;
        private Button btnRemoveActivity;
        private Label lblInstructions;
        private Label lblTotalTime;
        private Label lblStatus;

        private Button btnSubmit;
        private Label lblSubmission;
        private ComboBox cmbBoxSplit;
    }
}

