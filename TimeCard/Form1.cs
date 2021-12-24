using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace TimeCard
{
    public partial class FrmTimeCard : Form
    {
        // Single Instance
        private static FrmTimeCard s_Instance;

        public List<EventPanel> m_EventList = new List<EventPanel>();

        public DataTable m_WorkTable = new DataTable();
        private BindingSource m_DropDownSource = new BindingSource();
        private Timers m_Timers = new Timers();
        /// <summary>
        /// Drop down list assigned to the drop down source.
        /// </summary>
        private BindingList<String> m_DropDownList = new BindingList<string>();

        /// <summary>
        /// Total for the current day.
        /// </summary>
        private double m_DailyTotal;

        /// <summary>
        /// Locks Ui thread to perform one action at a time.
        /// </summary>
        private static readonly object sr_UiLock = new object();

        private static BackgroundWorker s_SubmitWorker = new BackgroundWorker();

        private static string s_DataPath = "../Data";
        private static string s_TimeLogPath = "../Time_Logs";

        /// <summary>
        /// Source for the list of drop down menu.
        /// </summary>
        public BindingSource DropDownSource
        {
            get { return m_DropDownSource; }
        }

        public static BackgroundWorker SubmitWorker
        {
            get { return s_SubmitWorker; }
            set
            {
                if (s_SubmitWorker != value)
                {
                    s_SubmitWorker = value;
                }
            }
        }

        public static FrmTimeCard Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new FrmTimeCard();
                }
                return s_Instance;
            }
        }

        private FrmTimeCard()
        {
            InitializeComponent();

            LoadActivities();
            
            if (m_WorkTable.Columns.Count == 0)
            {
                // Create beginning work table.
                m_WorkTable.Columns.Add("Identification", typeof(string));
                m_WorkTable.Columns.Add("Description", typeof(string));
                m_WorkTable.Columns.Add("Hours", typeof(double));
                m_WorkTable.Columns.Add("Split", typeof(bool));
            }
            // Updates original version of the Work Table.
            else if (m_WorkTable.Columns.Count == 3)
            {
                m_WorkTable.Columns.Add("Split", typeof(bool));
            }
        }

        private void OnLoad(object o, EventArgs eventArgs)
        {
            dataGridView1.DataSource = m_WorkTable;

            dataGridView1.Columns[0].Width = 100;
            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[1].Width = 250;
            dataGridView1.Columns[1].ReadOnly = true;
            dataGridView1.Columns[2].Width = 50;
            dataGridView1.Columns[2].ReadOnly = true;
            dataGridView1.Columns[3].Width = 50;
            dataGridView1.Columns[3].ReadOnly = false;
            dataGridView1.Columns[3].ToolTipText = "Select work items to split Unknown time between\n"
                                                   + " Selecting none will result in split between all\n"
                                                   + " the items on the current day's work list.";

            DataGridViewColumn column = dataGridView1.Columns[2];
            const ListSortDirection direction = ListSortDirection.Descending;
            dataGridView1.Sort(column, direction);
            column.HeaderCell.SortGlyphDirection = SortOrder.Descending;

            // Update drop down list.
            m_DropDownList.Add("Break");
            m_DropDownList.Add("Unknown");
            foreach (string item in m_WorkTable.AsEnumerable().Select(r => r.Field<string>("Description")).ToList())
            {
                m_DropDownList.Add(item);
            }
            m_DropDownSource.DataSource = m_DropDownList;

            RecoverDay();
            try
            {
                UpdateDataGrid();
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message + "\n" + ex.StackTrace);
            }

            SubmitWorker.DoWork += SubmitWork;
            m_Timers.NewDay += OnNewDay;
            dataGridView1.CurrentCellDirtyStateChanged += DataGridView1OnCurrentCellDirtyStateChanged;
            dataGridView1.CellValueChanged += DataGridView1OnCellValueChanged;
        }

        private void DataGridView1OnCellValueChanged(object sender, DataGridViewCellEventArgs dataGridViewCellEventArgs)
        {
            // Remove focus
            dataGridView1.CurrentCell = null;
            if (dataGridViewCellEventArgs.ColumnIndex == 3)
            {
                UpdateGui();
            }
        }

        private void DataGridView1OnCurrentCellDirtyStateChanged(object sender, EventArgs eventArgs)
        {
            if (dataGridView1.IsCurrentCellDirty)
            {
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private static void OnNewDay(object sender, EventArgs e)
        {
            EndOfDayForm.Instance.ShowDialog();
        }

        private void StartNewDay(object sender, EventArgs e)
        {
            StartNewDay();
        }

        internal void StartNewDay()
        {
            try
            {
                lblDateTime.Text = DateTime.Today.ToLongDateString();
                txtTime.Text = "";
                lblStatus.Text = "Daily time has not been submitted.";
                lblStatus.BackColor = Color.Red;
                while (m_EventList.Count > 0)
                {
                    DeleteEvent(0);
                }
                ClearDataGrid();
                SaveDay();
                Logger.CreateNewLogger();
            }
            catch (Exception ex)
            {
                Logger.Log("Error while starting new day.\n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        public void ClearDataGrid()
        {
            foreach (string key in m_DropDownList)
            {
                string search = String.Format("Description = '{0}'", key);
                DataRow[] found_rows = m_WorkTable.Select(search);
                if (found_rows.Length > 0)
                {
                    string hours = "0.0";
                    found_rows[0]["Hours"] = hours;
                }
            }
        }

        public void UpdateDataGrid()
        {
            DateTime start = DateTime.ParseExact(txtTime.Text.Trim(), "hh:mm tt", CultureInfo.InvariantCulture);
            Dictionary<string, double> data = new Dictionary<string, double>();
            IOrderedEnumerable<EventPanel> ordered_events = m_EventList.OrderBy(i => i.Index);
            List<string> activity_list = new List<string>();
            double total = 0.0;
            foreach (EventPanel item in ordered_events)
            {
                string activity = item.cmBoxActivity.Text;
                DateTime end = DateTime.ParseExact(item.txtNextTime.Text.Trim(), "hh:mm tt", CultureInfo.InvariantCulture);
                double hours = (end - start).TotalHours >= 0.0? (end - start).TotalHours : 0.0;
                if (!data.ContainsKey(activity))
                {
                    data.Add(activity, hours);
                }
                else
                {
                    data[activity] += hours;
                }
                if (activity != "Break")
                {
                    total += hours;
                }
                start = end;

                // Add new activities to the activity list.
                if (!activity_list.Contains(activity)
                    && activity != "Unknown"
                    && activity != "Break")
                {
                    activity_list.Add(activity);
                }
            }
            // Assign total
            total = Math.Round(total, 1);
            m_DailyTotal = total;
            lblTotalTime.Text = String.Format("Total: {0:0.0} hrs", m_DailyTotal);

            double extra = 0;
            foreach (string key in m_DropDownList)
            {
                string search = String.Format("Description = '{0}'", key);
                DataRow[] found_rows = m_WorkTable.Select(search);
                if (found_rows.Length > 0)
                {
                    string hours = "0.0";
                    if (data.ContainsKey(key))
                    {
                        hours = data[key].ToString("0.0");
                    }
                    found_rows[0]["Hours"] = hours;
                }
                else
                {
                    if (key != "Break")
                    {
                        if (data.ContainsKey(key))
                        {
                            extra += data[key];
                        }
                    }
                }
            }

            // Spread Extas
            SpreadExtras(extra, activity_list);
            TrimExtras();
        }

        public void SpreadExtras(double amount, List<String> activities)
        {
            string search;
            List<String> split_activities = new List<string>();
            // check for activites that are split.
            foreach (string activity in activities)
            {
                search = String.Format("Description = '{0}'", activity);
                DataRow row = m_WorkTable.Select(search)[0];

                bool split = false;
                try
                {
                    split = (bool)row["Split"];
                }
                catch (Exception)
                {
                    // ignored
                }

                if (split)
                {
                    split_activities.Add(activity);
                }
            }

            // Split through all activities if the user has selected none.
            if (split_activities.Count == 0)
            {
                split_activities = activities;
            }
            double even_split = amount / split_activities.Count;

            foreach (string activity in split_activities)
            {
                search = String.Format("Description = '{0}'", activity);
                DataRow row = m_WorkTable.Select(search)[0];

                double current = double.Parse(row["Hours"].ToString());
                double final = current + even_split;
                row["Hours"] = final.ToString("0.0");
            }
        }

        private void TrimExtras()
        {
            double initial_hours = 0.0;
            foreach (DataRow row in m_WorkTable.Rows)
            {
                string hours_string = row["Hours"].ToString();
                double hours = Double.Parse(hours_string);
                initial_hours += hours;
            }

            // Don't do anything if no real activites have been populated.
            if (Math.Abs(initial_hours) < 0.01)
            {
                return;
            }

            double difference = initial_hours - m_DailyTotal;
            // Get rid of extras
            while (difference >= 0.05)
            {
                foreach (DataRow row in m_WorkTable.Rows)
                {
                    double hours = Double.Parse(row["Hours"].ToString());
                    if (hours > 0.0)
                    {
                        hours -= 0.1;
                        row["Hours"] = hours.ToString("0.0");
                        difference -= 0.1;
                    }

                    if (Math.Abs(difference) < 0.05)
                    {
                        break;
                    }
                }
            }
            // Spend extra hours to get to total.
            while (difference <= -0.05)
            {
                foreach (DataRow row in m_WorkTable.Rows)
                {
                    double hours = Double.Parse(row["Hours"].ToString());
                    if (hours > 0.0)
                    {
                        hours += 0.1;
                        row["Hours"] = hours.ToString("0.0");
                        difference += 0.1;
                    }

                    if (Math.Abs(difference) < 0.05)
                    {
                        break;
                    }
                }
            }
        }

        public void txtTime_DoubleClick(object sender, EventArgs e)
        {
            TextBox txt_box = sender as TextBox;
            if (txt_box == null) return;
            txt_box.Text = DateTime.Now.ToString("hh:mm tt");
        }

        public void txtTime_TextChanged(object sender, EventArgs e)
        {
            // Check for valid values.
            TextBox txt_box = (TextBox) sender;
            string time = txt_box.Text;
            try
            {
                DateTime date_time = DateTime.ParseExact(time.Trim(), "hh:mm tt", CultureInfo.InvariantCulture);
                txt_box.BackColor = DefaultBackColor;
            }
            catch (Exception ex)
            {
                txt_box.BackColor = Color.Red;
                Logger.Log("Turning back color red. " + ex.Message + "\n" + ex.StackTrace);
            }

            UpdateGui();
        }

        public void cmBoxActivities_TextChanged(object sender, EventArgs e)
        {
            UpdateGui();
        }

        public void UpdateGui()
        {
            try
            {
                UpdateDataGrid();
                SaveDay();
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message + "\n" + ex.StackTrace);
            }
        }

        public void btnSubmit_Click(object sender, EventArgs e)
        {
            Submit();
        }

        public void Submit()
        {
            if (!SubmitWorker.IsBusy)
            {
                SubmitWorker.RunWorkerAsync();
            }
        }

        private void SubmitWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            if (Externals.SubmitDay(m_WorkTable, m_DailyTotal))
            {
                lblStatus.Text = "Successfully submitted daily time!";
                lblStatus.BackColor = Color.Lime;
            }
        }

        public void SaveDay()
        {
            string file_name = "Log - " + DateTime.Today.ToLongDateString() + ".bin";
            string full_path = Path.Combine(Application.LocalUserAppDataPath, s_TimeLogPath);
            bool exists = Directory.Exists(full_path);
            if (!exists)
            {
                Directory.CreateDirectory(full_path);
            }

            string file = Path.Combine(full_path, file_name);
            FileStream fs = new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            BinaryFormatter bf = new BinaryFormatter();
            try
            {
                List<object> objects = new List<object> {txtTime.Text, m_EventList};
                bf.Serialize(fs, objects);
            }
            catch (Exception e)
            {
                Logger.Log("Failed to serialize day log. Reason: " + e.Message + "\n" + e.StackTrace);
            }
            finally
            {
                fs.Close();
            }
        }

        public string SaveGui()
        {
            string info = null;
            try
            {
                foreach (DataRow row in m_WorkTable.Rows)
                {
                    foreach (object item in row.ItemArray)
                    {
                        info += item + "    ";
                    }
                    info += "\n";
                }
                info += lblTotalTime.Text + "\n";
                info += txtTime.Text + "\n";
                foreach (EventPanel item in m_EventList)
                {
                    info += item.Activity + "\n";
                    info += item.TimeCompleted + "\n";
                }
            }
            catch (Exception e)
            {
                Logger.Log("Failed to stringify GUI. Reason: " + e.Message + "\n" + e.StackTrace);
            }
            return info;
        }

        public void RecoverDay()
        {
            string file_name = "Log - " + DateTime.Today.ToLongDateString() + ".bin";
            string full_path = Path.Combine(Application.LocalUserAppDataPath, s_TimeLogPath);
            bool exists = Directory.Exists(full_path);
            if (!exists)
            {
                Directory.CreateDirectory(full_path);
            }

            string file = Path.Combine(full_path, file_name);
            FileStream fs = new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            BinaryFormatter bf = new BinaryFormatter();
            try
            {
                List<object> objects = (List<object>) bf.Deserialize(fs);
                txtTime.Text = (string)objects[0];
                m_EventList = (List<EventPanel>)objects[1];

            }
            catch (Exception e)
            {
                Logger.Log("Failed to deserialize activities. Reason: " + e.Message + "\n" + e.StackTrace);
            }
            finally
            {
                fs.Close();
            }
        }

        public void AddEvent(int index)
        {
            // Dropdown Activity
            panel1.RowCount = (m_EventList.Count + 1) * 2 + 3;
            for (int i = 0; i < panel1.RowCount; i++)
            {
                if (i >= panel1.RowStyles.Count)
                {
                    panel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                }
            }
            panel1.SuspendLayout();
            EventPanel panel = new EventPanel(this, ref panel1, ref m_DropDownSource, index);
            // Move existing down.
            foreach (EventPanel item in m_EventList)
            {
                if (item.Index >= index)
                {
                    item.MoveDown();
                }
            }
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            // Insert current.
            m_EventList.Add(panel);
            // Adjust scroll
            panel1.SuspendLayout();
            panel1.ScrollControlIntoView(panel.btnNextAdd);
            // Remove scroll on less that six items.
            if (m_EventList.Count < 6)
            {
                panel1.AutoScroll = false;
            }
            else
            {
                panel1.AutoScroll = true;
            }
            // If last item on list, then scroll to bottom.
            if (index == m_EventList.Count - 1)
            {
                panel1.VerticalScroll.Value = panel1.VerticalScroll.Maximum;
            }
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
        }

        internal void btnAdd_Click(object sender, EventArgs e, int index)
        {
            lock (sr_UiLock)
            {
                AddEvent(index);
            }
        }

        public void DeleteEvent(int index)
        {
            EventPanel item_to_remove = null;

            panel1.SuspendLayout();
            foreach (EventPanel item in m_EventList)
            {
                if (item.Index == index)
                {
                    item_to_remove = item;
                }
                // Move existing up.X
                else if (item.Index > index)
                {
                    item.MoveUp();
                }
            }
            // Remove panel items.
            if (item_to_remove != null)
            {
                panel1.ScrollControlIntoView(item_to_remove.cmBoxActivity);
                item_to_remove.Delete();
                m_EventList.Remove(item_to_remove);
            }
            panel1.RowCount = m_EventList.Count * 2 + 3;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            // Adjust scroll.
            panel1.SuspendLayout();
            // Remove scroll on less that six items.
            if (m_EventList.Count < 6)
            {
                panel1.AutoScroll = false;
            }
            else
            {
                panel1.AutoScroll = true;
            }
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
        }

        internal void btnDelete_Click(object sender, EventArgs e, int index)
        {
            lock (sr_UiLock)
            {
                DeleteEvent(index);
            }
        }

        public void btnAddActivity_Click(object sender, EventArgs e)
        {
            AddActivityForm dialog = new AddActivityForm();
            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                if (!m_DropDownList.Contains(dialog.Description))
                {
                    m_WorkTable.Rows.Add(dialog.Number, dialog.Description, 0.0);
                    m_DropDownList.Add(dialog.Description);
                    // Scroll to bottom.
                    dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
                }
                else
                {
                    MessageBox.Show("Activity with the same Description already exists.");
                }
            }
            SaveActivities();
        }

        private void btnRemoveActivity_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            { 
                var row = dataGridView1.SelectedRows[0];
                DataRow data_row = ((DataRowView) row.DataBoundItem).Row;
                string description = data_row["Description"].ToString();
                dataGridView1.Rows.Remove(row);
                m_DropDownList.Remove(description);
                UpdateGui();
            }
            SaveActivities();
        }

        private void CmbBoxSplitOnSelectedIndexChanged(object o, EventArgs eventArgs)
        {
            try
            {
                UpdateDataGrid();
                SaveDay();
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message + "\n" + ex.StackTrace);
            }
        }

        public void SaveActivities()
        {
            const string file_name = "Activities.bin";
            string full_path = Path.Combine(Application.LocalUserAppDataPath, s_DataPath);
            bool exists = Directory.Exists((full_path));
            if (!exists)
            {
                Directory.CreateDirectory(full_path);
            }

            string file = Path.Combine(full_path, file_name);
            FileStream fs = new FileStream(file, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            BinaryFormatter bf = new BinaryFormatter();
            try
            {
                bf.Serialize(fs, m_WorkTable);
            }
            catch (Exception e)
            {
                Logger.Log("Failed to serialize activities. Reason: " + e.Message + "\n" + e.StackTrace);
            }
            finally
            {
                fs.Close();
            }
        }

        public void LoadActivities()
        {
            const string file_name = "Activities.bin";
            string full_path = Path.Combine(Application.LocalUserAppDataPath, s_DataPath);
            bool exists = Directory.Exists((full_path));
            if (!exists)
            {
                Directory.CreateDirectory(full_path);
            }

            string file = Path.Combine(full_path, file_name);
            FileStream fs = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None);
            BinaryFormatter bf = new BinaryFormatter();
            try
            {
                m_WorkTable = (DataTable) bf.Deserialize(fs);
            }
            catch (Exception e)
            {
                Logger.Log("Failed to deserialize activities. Reason: " + e.Message + "\n" + e.StackTrace);
            }
            finally
            {
                fs.Close();
            }
        }
    }
}
