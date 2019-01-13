using System;
using System.Windows.Forms;

namespace TimeCard
{
    public partial class EndOfDayForm : Form
    {
        private static EndOfDayForm s_Instance;

        public static EndOfDayForm Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new EndOfDayForm();
                }
                return s_Instance;
            }
        }

        private EndOfDayForm()
        {
            InitializeComponent();
        }

        private void btnStartNewDay_Click(object sender, EventArgs e)
        {

            FrmTimeCard.Instance.Restart();
            Close();
        }

        private void btnEditPreviousDay_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSubmitPreviousDay_Click(object sender, EventArgs e)
        {
            FrmTimeCard.Instance.Submit();
        }
    }
}
