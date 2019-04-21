using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Controller {
    public class BotController {
        private readonly Random      _random;
        private readonly List<Robot> _robots;
        private          bool        power;

        public BotController(Robot[] robots) {
            _robots = new List<Robot>();
            _random = new Random();
            power   = false;

            if (robots != null)
                foreach (var robot in robots)
                    _robots.Add(robot);
        }

        public async Task<Robot> GetRobot(int id) {
            if (_robots.Count - 1 < id)
                return null;

            return _robots[id];
        }

        public async Task<Robot> GetRobot(string name) {
            foreach (var robot in _robots)
                if (robot.Name.ToLower() == name.ToLower())
                    return robot;

            return null;
        }

        public async Task<Robot[]> GetAllRobots()
        {
            return _robots.ToArray();
        }

        public async Task AddRobot(Robot robot) {
            _robots.Add(robot);
        }

        public async Task AddRobot(string name, int port) {
            _robots.Add(new Robot(name, port));
        }

        public async Task RemoveRobot(int id) {
            var r = await GetRobot(id);

            if (r != null)
                _robots.Remove(r);

            await ButcherRobot(r);
        }

        public async Task ButcherRobot(Robot r)
        {
            if (r.ComPort.IsOpen)
            {
                r.ComPort.DiscardInBuffer();
                r.ComPort.DiscardOutBuffer();
                r.ComPort.Close();
            }
            r.Dispose();
            r = null;
        }

        public async Task RemoveRobot(string name) {
            var r = await GetRobot(name);

            if (r != null)
                _robots.Remove(r);

            await ButcherRobot(r);
        }

        public async Task<string[]> GetRobotInfo(int id) {
            var r = await GetRobot(id);

            return r != null ? await GetRobotInfo(r) : null;
        }

        public async Task<string[]> GetRobotInfo(string name) {
            var r = await GetRobot(name);

            return r != null ? await GetRobotInfo(r) : null;
        }

        private async Task<string[]> GetRobotInfo(Robot robot) {
            var name  = robot.Name;
            var port  = robot.Port;
            var power = await GetPowerName(robot.Power);
            var move  = robot.IsMoving ? "Moving" : "Stopped";

            return new[] {name, port, power, move};
        }

        public async Task<List<string>> GetAllNames()
        {
            List<string> temp_names = new List<string>();
            foreach (var robot in _robots)
            {
                temp_names.Add((await GetRobotInfo(robot))[0]);
            }
            return temp_names;
        }

        public async Task<string[][]> GetAllRobotInfo() {
            var tempList = new List<string[]>();

            foreach (var robot in _robots) {
                var name  = "name: " + robot.Name;
                var port  = "port: " + robot.Port;
                var power = "power: " + await GetPowerName(robot.Power);
                var move  = "status: " + (robot.IsMoving ? "Moving" : "Stopped");

                tempList.Add(new[] {name, port, power, move});
            }

            return tempList.ToArray();
        }

        public async Task Connect(int id) {
            var r = await GetRobot(id);

            if (r != null) await Connect(r);
        }

        public async Task Connect(string name) {
            var r = await GetRobot(name);

            if (r != null) await Connect(r);
        }

        public async Task Connect(Robot robot) {
            await robot.Connect();
        }

        public async Task ConnectAll() {
            foreach (var robot in _robots) await robot.Connect();
            List<string> offline = new List<string>();
            foreach (var robot in _robots) if (robot.ComPort.IsOpen) offline.Add(robot.Name);
            if (offline.Count > 0)
            {
                string log = "Robots: [" + string.Join(", ", offline) + "] couldn't connect.";
            }
            else
            {
                Logger.Instance.Log("All robots triumphantly connected");
            }
        }

        public async Task Disconnect(int id) {
            var r = await GetRobot(id);

            if (r != null) await Disconnect(r);
        }

        public async Task Disconnect(string name) {
            var r = await GetRobot(name);

            if (r != null) await Disconnect(r);
        }

        public async Task Disconnect(Robot robot) {
            if (robot.ComPort.IsOpen)
                robot.ComPort.Close();
        }

        public async Task DisconnectAll() {
            foreach (var robot in _robots)
                if (robot.ComPort.IsOpen)
                    robot.ComPort.Close();
        }

        public async Task PowerUp(int no = 2, int timeOut = 10000) {
            if (no > _robots.Count)
                return;
            var set    = 0;
            var powers = new List<int>() {1, 2, 3, 4, 5};

            while (set != no) {
                var r = _robots[_random.Next(_robots.Count)];

                if (r.Power == 0) {
                    var rand = _random.Next(powers.Count - 1);
                    r.Power = powers[rand];
                    powers.RemoveAt(rand);
                    set++;
                }
            }

            power = true;

            Logger.Instance.Log("Powers online");

            ExpirePower(timeOut);
        }

        private async Task ExpirePower(int timeOut) {
            await Task.Delay(timeOut);
            await RemoveAllPowers();
        }

        private async Task RemoveAllPowers() {
            foreach (var robot in _robots) robot.Power = 0;

            power = false;
        }

        private async Task<string> GetPowerName(int power) {
            switch (power) {
                case 1:

                    return "speed";
                case 2:

                    return "turn";
                case 3:

                    return "reverse";
                case 4:

                    return "mirror";
                case 5:

                    return "slow";
                default:

                    return "none";
            }
        }

        public void DevilsMethod()
        {
            foreach (var r in _robots)
            {
                try
                {
                    r.ComPort.Write(":::::::::");
                } catch (Exception e)
                {
                    Logger.Instance.Log(e.Message);
                }
            }
            Thread.Sleep(2000);
        }
    }
}