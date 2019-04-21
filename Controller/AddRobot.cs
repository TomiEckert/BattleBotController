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
    public partial class AddRobot : Form
    {
        private BotController bController;
        public AddRobot(BotController b)
        {
            bController = b;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text;
            int port = (int)Math.Round(numericUpDown1.Value);
            (sender as Button).Text = "Loading";
            await bController.AddRobot(new Robot(name, port));
            await bController.Connect(name);
            Close();
        }
    }
}
