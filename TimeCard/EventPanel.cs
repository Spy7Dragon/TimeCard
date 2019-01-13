using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace TimeCard
{
    [Serializable]
    public partial class EventPanel : ISerializable
    {
        private FrmTimeCard m_Controller;
        private BindingSource m_Source;
        private int m_Index;
        private Point m_Location;
        private TableLayoutPanel m_Parent;

        public int Index
        {
            get { return m_Index; }
            set { m_Index = value; }
        }

        public string Activity
        {
            get { return cmBoxActivity.Text; }
            set { cmBoxActivity.Text = value; }
        }

        public string TimeCompleted
        {
            get { return txtNextTime.Text; }
            set { txtNextTime.Text = value; }
        }
 
        public EventPanel(FrmTimeCard controller, ref TableLayoutPanel parent, ref BindingSource source, int index)
        {
            m_Index = index;
            m_Controller = controller;
            m_Parent = parent;
            m_Source = source;
            InitializeComponent();
        }

        public void Delete()
        {
            m_Parent.Controls.Remove(cmBoxActivity);
            cmBoxActivity.Dispose();
            m_Parent.Controls.Remove(btnDelete);
            btnDelete.Dispose();
            m_Parent.Controls.Remove(txtNextTime);
            txtNextTime.Dispose();
            m_Parent.Controls.Remove(btnNextAdd);
            btnNextAdd.Dispose();

            int top_row_index = 2 * Index + 1;
            int bottom_row_index = 2 * Index + 2;
            if (m_Parent.RowStyles.Count > top_row_index
                && top_row_index >= 0)
            {
                m_Parent.RowStyles.RemoveAt(top_row_index);
            }
            if (m_Parent.RowStyles.Count > bottom_row_index
                && bottom_row_index >= 0)
            {
                m_Parent.RowStyles.RemoveAt(bottom_row_index);
            }
        }

        public void MoveDown()
        {
            m_Index++;
            MoveControls(); 
        }

        public void MoveUp()
        {
            m_Index--;
            MoveControls();
        }

        private void MoveControls()
        {
            m_Parent.SetRow(cmBoxActivity, 2 * m_Index + 1);
            m_Parent.SetColumn(cmBoxActivity, 1);
            m_Parent.SetColumnSpan(cmBoxActivity, 2);
            m_Parent.SetRow(btnDelete, 2 * m_Index + 1);
            m_Parent.SetColumn(btnDelete, 3);
            m_Parent.SetRow(txtNextTime, 2 * m_Index + 2);
            m_Parent.SetColumn(txtNextTime, 0);
            m_Parent.SetColumnSpan(txtNextTime, 2);
            m_Parent.SetRow(btnNextAdd, 2 * m_Index + 3);
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Index", Index);
            info.AddValue("Activity", Activity);
            info.AddValue("TimeCompleted", TimeCompleted);
        }

        public EventPanel(SerializationInfo info, StreamingContext context)
        {
            int index = (int) info.GetValue("Index", typeof(int));
            string activity = (string) info.GetValue("Activity", typeof(string));
            string time_completed = (string) info.GetValue("TimeCompleted", typeof(string));

            m_Index = index;
            m_Controller = FrmTimeCard.Instance;
            m_Parent = FrmTimeCard.Instance.panel1;
            m_Source = FrmTimeCard.Instance.DropDownSource;
            InitializeComponent();
            Activity = activity;
            TimeCompleted = time_completed;
        }
    }
}
