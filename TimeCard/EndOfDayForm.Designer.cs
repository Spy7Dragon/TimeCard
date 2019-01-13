namespace TimeCard
{
    partial class EndOfDayForm
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
            this.lblMessage = new System.Windows.Forms.Label();
            this.btnStartNewDay = new System.Windows.Forms.Button();
            this.btnSubmitPreviousDay = new System.Windows.Forms.Button();
            this.btnEditPreviousDay = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.Location = new System.Drawing.Point(154, 9);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(109, 18);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "End of Day!";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnStartNewDay
            // 
            this.btnStartNewDay.AutoSize = true;
            this.btnStartNewDay.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnStartNewDay.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartNewDay.Location = new System.Drawing.Point(12, 38);
            this.btnStartNewDay.Name = "btnStartNewDay";
            this.btnStartNewDay.Size = new System.Drawing.Size(109, 23);
            this.btnStartNewDay.TabIndex = 1;
            this.btnStartNewDay.Text = "Start New Day";
            this.btnStartNewDay.UseVisualStyleBackColor = true;
            this.btnStartNewDay.Click += new System.EventHandler(this.btnStartNewDay_Click);
            // 
            // btnSubmitPreviousDay
            // 
            this.btnSubmitPreviousDay.AutoSize = true;
            this.btnSubmitPreviousDay.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSubmitPreviousDay.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSubmitPreviousDay.Location = new System.Drawing.Point(127, 38);
            this.btnSubmitPreviousDay.Name = "btnSubmitPreviousDay";
            this.btnSubmitPreviousDay.Size = new System.Drawing.Size(152, 23);
            this.btnSubmitPreviousDay.TabIndex = 2;
            this.btnSubmitPreviousDay.Text = "Submit Previous Day";
            this.btnSubmitPreviousDay.UseVisualStyleBackColor = true;
            this.btnSubmitPreviousDay.Click += new System.EventHandler(this.btnSubmitPreviousDay_Click);
            // 
            // btnEditPreviousDay
            // 
            this.btnEditPreviousDay.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnEditPreviousDay.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEditPreviousDay.Location = new System.Drawing.Point(285, 36);
            this.btnEditPreviousDay.Name = "btnEditPreviousDay";
            this.btnEditPreviousDay.Size = new System.Drawing.Size(132, 25);
            this.btnEditPreviousDay.TabIndex = 3;
            this.btnEditPreviousDay.Text = "Edit Previous Day";
            this.btnEditPreviousDay.UseVisualStyleBackColor = true;
            this.btnEditPreviousDay.Click += new System.EventHandler(this.btnEditPreviousDay_Click);
            // 
            // EndOfDayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 81);
            this.Controls.Add(this.btnEditPreviousDay);
            this.Controls.Add(this.btnSubmitPreviousDay);
            this.Controls.Add(this.btnStartNewDay);
            this.Controls.Add(this.lblMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "EndOfDayForm";
            this.Text = "EndOfDayForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Button btnStartNewDay;
        private System.Windows.Forms.Button btnSubmitPreviousDay;
        private System.Windows.Forms.Button btnEditPreviousDay;
    }
}