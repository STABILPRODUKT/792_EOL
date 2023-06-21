using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using EOL_tesztelo.Properties;


namespace EOL_tesztelo
{

    public partial class SaveForm : Form
    {
        public static bool visible = false;
        public static string SaveFolderBrowser;
        public static string SaveName;
        public static bool Save = false;
        public SaveForm()
        {
            InitializeComponent();
            if (Save)
            {
                btnSave.Enabled = false;
            }
        }
        public void Save_Finish()
        {
            btnSave.Enabled = true;
        }

        private void btnFolderBrowser_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.RootFolder = Environment.SpecialFolder.Desktop;
            fbd.Description = "+Mentés helye+";
            fbd.ShowNewFolderButton = false;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                tbFolderBrowser.Text = fbd.SelectedPath;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            btnSave.Enabled = false;
            if (tbFileName.TextLength < 1)
            {
                System.Windows.Forms.MessageBox.Show("File név nem megfelelő!");
            }
            else if (tbFolderBrowser.TextLength < 3)
            {
                System.Windows.Forms.MessageBox.Show("Elérési út nem megfelelő!");
            }
            else
            {
                SaveSettings("SaveFolderBrowser", tbFolderBrowser.Text);
                SaveSettings("SaveName", tbFileName.Text);
                Settings.Default.Save();
                SaveFolderBrowser = Settings.Default["SaveFolderBrowser"].ToString();
                SaveName = Settings.Default["SaveName"].ToString();
                Save = true;
            }
        }
        private void SaveSettings(string setting, object variant)
        {
            object obj = Settings.Default[setting];
            if (obj.ToString() != variant.ToString())
            {
          //      ClassMessage.SetMessage(ClassUser.LoginUser.LoginName, "Módosult a(z) " + setting + ": " + Settings.Default[setting].ToString() + "  ->  " + variant.ToString());
                Settings.Default[setting] = variant;
            }
        }

        private void SaveForm_Load(object sender, EventArgs e)
        {
            SaveFolderBrowser = Settings.Default["SaveFolderBrowser"].ToString();
            SaveName = Settings.Default["SaveName"].ToString();
            tbFolderBrowser.Text = SaveFolderBrowser;
            tbFileName.Text = SaveName;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SaveForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            visible = false;
        }
    }
}
