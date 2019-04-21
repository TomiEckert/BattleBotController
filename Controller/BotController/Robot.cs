using System;
using System.IO.Ports;
using System.Threading.Tasks;

namespace Controller {
    public class Robot {
        public Robot(string name, int port) {
            Name = name;
            SetPort(port).GetAwaiter().GetResult();
            Power    = 0;
            GameMode = 0;
            IsMoving = false;
            
        }

        public string     Name     { get; }
        public string     Port     { get; private set; }
        public int        Power    { get; set; }
        public int        GameMode { get; }
        public bool       IsMoving { get; private set; }
        public SerialPort ComPort  { get; private set; }
        public bool _override;

        public async Task SetPort(int port) {
            if (ComPort != null && ComPort.IsOpen)
                ComPort.Close();

            ComPort?.Dispose();

            Port = "COM" + port;

            ComPort = new SerialPort(
                Port,
                38400,
                Parity.Odd,
                8,
                StopBits.One
            );
            ComPort.WriteTimeout = 1000;

            ComPort.DataReceived += ComPort_DataReceived;
        }

        private void ComPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var input = ComPort.ReadExisting();
            if(input.Contains("move"))
            {
                IsMoving = true;
            }
            else
            {
                IsMoving = false;
            }
        }

        public async Task Connect() {
            if (ComPort.IsOpen)
                return;

            try {
                ComPort.Open();
                Logger.Instance.Log(Name + " is connected");
            }
            catch (Exception e) {
                ComPort.Close();

                Logger.Instance.Log("ERROR:::" + Name + ":" + e.Message);
            }
        }

        private async Task<bool> IsConnected() {
            if (ComPort.IsOpen)
                await Connect();
            return ComPort.IsOpen;
        }

        public async Task Send(RobotCommand command)
        {
            if (await IsConnected())
            {
                var sendTask = Task.Run(() =>
                {
                    try
                    {
                        ComPort.Write(command.ToString());
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.Log("ERROR:::" + Name + ":" + e.Message);
                    }
                });
                if (!sendTask.Wait(TimeSpan.FromMilliseconds(800)))
                    Logger.Instance.Log("KILLED:::Robot comm to '" + Name + "' too slow");
            }
            else
            {
                Logger.Instance.Log(Name + " is not connected.");
            }
        }

        public async Task Send(RobotCommand command, bool asd)
        {
            if (await IsConnected())
            {
                var sendTask = Task.Run(() =>
                {
                    try
                    {
                        ComPort.Write(command.ToString(asd));
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.Log("ERROR:::" + Name + ":" + e.Message);
                    }
                });
                if (!sendTask.Wait(TimeSpan.FromMilliseconds(800)))
                    Logger.Instance.Log("KILLED:::Robot comm to '" + Name + "' too slow");
            }
            else
            {
                Logger.Instance.Log(Name + " is not connected.");
            }
        }

        public void Dispose()
        {
            ComPort.Dispose();
        }
    }
}