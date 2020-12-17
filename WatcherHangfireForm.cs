using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WatcherHangfireDemo
{
    public partial class frmWatcherHangfire : Form
    {
        private WatcherService _service;

        public frmWatcherHangfire()
        {
            InitializeComponent();
            _service = new WatcherService();
        }
        
        private void btnStart_Click(object sender, EventArgs e)
        {
            _service.StartService();
            _service.StartDashboard();
        }
    }
}
