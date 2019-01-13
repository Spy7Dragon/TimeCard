using System;
using System.Windows.Forms;

namespace TimeCard
{
    public partial class AddActivityForm : Form
    {
        private string m_Number;
        public string Description { get; set; }

        public string Number
        {
            get
            {
                return m_Number; 
            }
            set
            {
                if (m_Number != value)
                {
                    m_Number = value;
                }
            }
        }

        public AddActivityForm()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnOkay_Click(object sender, EventArgs e)
        {
            Description = txtBoxDescription.Text.Trim();
            if (Description == "")
            {
                MessageBox.Show("Invalid Description");
                return;
            }
            Number = txtBoxNumber.Text.Trim();
            if (Number == "")
            {
                MessageBox.Show("Invalid Number");
            }
            else
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
