using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Controller
{
    public partial class RemoveRobot : Form
    {
        private BotController rController;

        public RemoveRobot(BotController rController)
        {
            InitializeComponent();
            this.rController = rController;
            comboBox1.DataSource = rController.GetAllNames().GetAwaiter().GetResult();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)
                await rController.RemoveRobot(comboBox1.SelectedIndex);
            Close();
        }
    }
}
