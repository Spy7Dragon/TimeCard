using System;
using System.Drawing;
using System.Windows.Forms;

namespace TimeCard
{
    partial class EventPanel
    {
        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.m_Location = new System.Drawing.Point(0, 25 + 50 * m_Index);

            m_Parent.RowCount = 2 * m_Index + 1;
            cmBoxActivity = new ComboBox
            {
                BindingContext = new BindingContext(),
                Size = new Size(325, 20),
                Location = new Point(m_Location.X + 25, m_Location.Y),
                DataSource = m_Source.DataSource
            };
            cmBoxActivity.TextChanged += new EventHandler(m_Controller.cmBoxActivities_TextChanged);
            m_Parent.Controls.Add(cmBoxActivity, 1, 2 * m_Index + 1);
            m_Parent.SetColumnSpan(cmBoxActivity, 2);

            btnDelete = new Button()
            {
                Size = new Size(20, 20),
                Location = new Point(m_Location.X + 355, m_Location.Y),
                Text = "-"
            };
            m_Parent.Controls.Add(btnDelete, 2, 2 * m_Index + 1);
            btnDelete.Click += delegate(object sender, System.EventArgs e)
            {
                m_Controller.btnDelete_Click(sender, e, m_Index);
            };
            
            // Time Box
            txtNextTime = new TextBox()
            {
                Size = new Size(75, 20),
                Location = new Point(m_Location.X, m_Location.Y + 25)
            };
            m_Parent.Controls.Add(txtNextTime, 0, 2 * m_Index + 2);
            m_Parent.SetColumnSpan(txtNextTime, 2);
            txtNextTime.DoubleClick += m_Controller.txtTime_DoubleClick;
            txtNextTime.TextChanged += m_Controller.txtTime_TextChanged;

            btnNextAdd = new Button()
            {
                Size = new Size(20, 20),
                Location = new Point(m_Location.X, m_Location.Y + 50),
                Text = "+"
            };
            m_Parent.Controls.Add(btnNextAdd, 0, 2 * m_Index + 3);
            btnNextAdd.Click += delegate (object sender, System.EventArgs e)
            {
                m_Controller.btnAdd_Click(sender, e, m_Index + 1);
            };
        }

        #endregion

        public ComboBox cmBoxActivity;
        public TextBox txtNextTime;
        public Button btnDelete;
        public Button btnNextAdd;
    }
}
