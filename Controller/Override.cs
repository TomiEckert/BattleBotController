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
    public partial class Override : Form
    {
        private Robot robot;
        private bool down;

        public Override(Robot r)
        {
            robot = r;
            r._override = true;
            InitializeComponent();
        }

        private void Override_FormClosing(object sender, FormClosingEventArgs e)
        {
            robot._override = false;
        }

        private async void Override_KeyDown(object sender, KeyEventArgs e)
        {
            if(!down)
            switch (e.KeyCode)
            {
                case Keys.A:
                    down = true;
                    await robot.Send(new RobotCommand("L", 0, 5), true);
                    break;
                case Keys.D:
                    down = true;
                    await robot.Send(new RobotCommand("R", 0, 5), true);
                    break;
                case Keys.S:
                    down = true;
                    await robot.Send(new RobotCommand("B", 0, 5), true);
                    break;
                case Keys.W:
                    down = true;
                    await robot.Send(new RobotCommand("F", 0, 5), true);
                    break;
            }
        }

        private async void Override_KeyUp(object sender, KeyEventArgs e)
        {
            down = false;
            await robot.Send(new RobotCommand("N", 0, 5), true);
        }
    }
}
