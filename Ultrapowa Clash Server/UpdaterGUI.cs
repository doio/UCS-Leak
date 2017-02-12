using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UCS.Core.Settings;
using UCS.Core.Web;

namespace UCS
{
    public partial class UpdaterGUI : MaterialForm
    {
        public UpdaterGUI()
        {
            InitializeComponent();
            var sm = MaterialSkinManager.Instance;
            sm.AddFormToManage(this);
            sm.Theme = MaterialSkinManager.Themes.DARK;
            sm.ColorScheme = new ColorScheme(Primary.Blue800, Primary.Blue900, Primary.Grey500, Accent.Blue200, TextShade.WHITE);
        }

        private void UpdaterGUI_Load(object sender, EventArgs e)
        {

        }

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            if (VersionChecker.GetVersionString() == Constants.Version)
            {
                MessageBox.Show("No Update is available.", "UCS is up to date", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (VersionChecker.GetVersionString() == "Error") 
            {
                MessageBox.Show("Please check your Notwork connection or contact the Support at https://ultrapowa.com/forum", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("An Update is available! New Version: " + VersionChecker.GetVersionString() + ", do you want to download it?", "Update available", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
            }
        }
    }
}
