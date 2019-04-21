using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Controller
{
    public partial class Form1 : Form
    {
        private BotController rController;
        private bool autoUpdate;
        private bool comms;
        private BackgroundWorker _bgWorker;
        private BackgroundWorker _bgQuery;
        private Database database;
        private List<RobotCommand> _lastCommand;

        public Form1()
        {
            rController = new BotController(null);
            database = new Database();
            _bgWorker = new BackgroundWorker();
            _bgQuery = new BackgroundWorker();
            _bgWorker.WorkerReportsProgress = true;
            _bgWorker.DoWork += _bgWorker_DoWork;
            _bgWorker.ProgressChanged += _bgWorker_ProgressChanged;
            Logger.Instance.Log("Initialized");
            var c = new RobotCommand("N", 0, 0);
            _lastCommand = new List<RobotCommand>();
            _lastCommand.Add(c);
            _lastCommand.Add(c);
            _lastCommand.Add(c);
            _lastCommand.Add(c);
            _lastCommand.Add(c);
            InitializeComponent();
            var s = Logger.Instance.speed;
            numericUpDown1.Value = s;
        }

        private async void _bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var si = listBox2.SelectedItem;
            listBox2.DataSource = await rController.GetAllNames();
            if (si != null && listBox2.Items.Contains(si))
                listBox2.SelectedItem = si;

            var array = Logger.Instance._buffer.ToArray();
            listBox3.DataSource = array;
            listBox3.SelectedIndex = array.Length - 1;
        }

        private void _bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Logger.Instance.Log("UI update on");
            while (autoUpdate)
            {
                Thread.Sleep(800);
                int p = 0;
                object param = "bigoof";
                _bgWorker.ReportProgress(p, param);
            }
            Logger.Instance.Log("UI update off");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddRobot ar = new AddRobot(rController);
            ar.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RemoveRobot rr = new RemoveRobot(rController);
            rr.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            autoUpdate = !autoUpdate;
            if (autoUpdate)
            {
                (sender as Button).Text = "Auto update is on";
                Logger.Instance.Log("Auto update is on");
                _bgWorker.RunWorkerAsync();
            }
            else
            {
                (sender as Button).Text = "Auto update is off";
                Logger.Instance.Log("Auto update is off");
            }
        }

        private async void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.DataSource = 
                (await rController.GetAllRobotInfo())
                [(sender as ListBox).SelectedIndex]
                .ToList();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            bool connected = database.IsConnect();
            label2.Text = connected ? "connected" : "disconnected";

        }

        private async void DBLoop()
        {
            Logger.Instance.Log("Communication loop has started");
            while (comms)
            {
                var dbTask = Task.Run(() => database.Query());
                if (!dbTask.Wait(TimeSpan.FromMilliseconds(500)))
                    Logger.Instance.Log("KILLED:::DB Query too slow");

                var robots = await rController.GetAllRobots();

                for (int i = 0; i < robots.Length; i++)
                {
                    var command = new RobotCommand(
                        database.DB_Info[i][1].ToString(),
                        GetPower(database.DB_Info[i][3].ToString()),
                        (int)database.DB_Info[i][2]
                    );

                    if (command != _lastCommand[i] && !robots[i]._override)
                    {
                        _lastCommand[i] = command;
                        await robots[i].Send(command);
                    }
                }

                var local = await rController.GetAllRobotInfo();

                dbTask = Task.Run(() => database.Update(local));

                if (!dbTask.Wait(TimeSpan.FromMilliseconds(500)))
                    Logger.Instance.Log("KILLED:::DB Upload too slow");

                await Task.Delay(100);
            }
            Logger.Instance.Log("Communication loop has stopped");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            comms = !comms;
            (sender as Button).Text = comms ? "Comms are on" : "Comms are off";
            if (comms)
                Task.Run(() => { DBLoop(); });
        }

        private int GetPower(string power)
        {
            switch (power)
            {
                case "speed":

                    return 1;
                case "turn":

                    return 2;
                case "reverse":

                    return 3;
                case "mirror":

                    return 4;
                case "slow":

                    return 5;
                default:

                    return 0;
            }
        }

        private async void button6_Click(object sender, EventArgs e)
        {
            await rController.PowerUp((await rController.GetAllNames()).Count);
        }

        private async void button7_Click(object sender, EventArgs e)
        {
            Logger.Instance.LogFile("dump");
            string file = "";
            foreach (var bot in await rController.GetAllRobots())
            {
                file += "robot" + bot.Name + Environment.NewLine;
                if (bot.ComPort.IsOpen)
                    file += bot.ComPort.ReadExisting() +
                        Environment.NewLine +
                        Environment.NewLine;
            }

            foreach (var line in Logger.Instance._file)
            {
                file += line + Environment.NewLine;
            }

            if (File.Exists("degub.txt"))
                File.Delete("degub.txt");
            StreamWriter sw = new StreamWriter("degub.txt");
            sw.Write(file);
            sw.Close();
        }

        private async void button8_Click(object sender, EventArgs e)
        {
            await rController.ConnectAll();
            comms = false;
            Thread.Sleep(1000);
            rController.DevilsMethod();
            comms = true;
            await rController.ConnectAll();
            Task.Run(() => DBLoop());
        }

        private async void button9_Click(object sender, EventArgs e)
        {
            var r = await rController.GetRobot(listBox2.SelectedItem.ToString());
            r._override = true;
            if (r.ComPort.IsOpen)
            {
                r.ComPort.DiscardInBuffer();
                r.ComPort.DiscardOutBuffer();
                r.ComPort.Close();
            }
            await r.Connect();
            await r.Send(new RobotCommand("N", 0, 5), true);
            r._override = false;
        }

        private async void button10_Click(object sender, EventArgs e)
        {
            var r = await rController.GetRobot(listBox2.SelectedItem.ToString());
            var o = new Override(r);
            o.ShowDialog();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            int s = (int)numericUpDown1.Value;
            Logger.Instance.speed = s;
            s = Logger.Instance.speed;
            numericUpDown1.Value = s;
        }
    }
}
