using System;
using System.IO;

namespace Controller {
    public class RobotCommand {
        private readonly int gameMode;
        private readonly int lback;
        private readonly int left;
        private readonly int rback;
        private readonly int right;
        private readonly string dir;

        public RobotCommand(string dir, int power, int game) {
            gameMode = game;
            this.dir = dir;

            switch (dir) {
                case "F":
                    left  = Logger.Instance.speed;
                    right = Logger.Instance.speed;
                    lback = 0;
                    rback = 0;

                    break;
                case "L":
                    left  = 255 - Logger.Instance.speed / 3 * 2;
                    right = Logger.Instance.speed / 3 * 2;
                    lback = 1;
                    rback = 0;

                    break;
                case "R":
                    left  = Logger.Instance.speed / 3 * 2;
                    right = 255 - Logger.Instance.speed / 3 * 2;
                    lback = 0;
                    rback = 1;

                    break;
                case "B":
                    left  = 255 - Logger.Instance.speed;
                    right = 255 - Logger.Instance.speed;
                    lback = 1;
                    rback = 1;

                    break;
                case "N":
                    left  = 0;
                    right = 0;
                    lback = 0;
                    rback = 0;

                    break;
            }

            switch (power) {
                case 1:

                    if (dir == "B") {
                        left  -= 50;
                        right -= 50;
                    }
                    else {
                        left  += 50;
                        right += 50;
                    }

                    break;
                case 2:

                    if (dir == "L") {
                        left  -= 50;
                        right += 50;
                    }

                    if (dir == "R") {
                        left  += 50;
                        right -= 50;
                    }

                    break;
                case 3:

                    if (dir == "F") {
                        left  = 255 - left;
                        right = 255 - right;
                        lback = 1;
                        rback = 1;
                    }

                    if (dir == "B") {
                        left  = 255 - left;
                        right = 255 - right;
                        lback = 0;
                        rback = 0;
                    }

                    break;
                case 4:

                    if (dir == "L" || dir == "R") {
                        var tmp = left;
                        left  = right;
                        right = tmp;
                    }

                    break;
                case 5:

                    if (dir == "B") {
                        left  += 50;
                        right += 50;
                    }
                    else {
                        left  -= 50;
                        right -= 50;
                    }

                    break;
                default:

                    break;
            }
        }

        public override string ToString()
        {
            string leftS = 
                left.ToString().Length < 3 ? 0 + left.ToString() : left.ToString();
            string rightS = 
                right.ToString().Length < 3 ? 0 + right.ToString() : right.ToString();

            if (rightS.Length == 2)
                rightS = "0" + rightS;
            if (leftS.Length == 2)
                leftS = "0" + leftS;

            var serverMessage = leftS + rightS;    // 6 chars
            serverMessage += lback.ToString() + rback;      // 2 chars
            serverMessage += gameMode.ToString();           // 1 char

            if (!File.Exists("commands.txt"))
                File.Create("commands.txt");
            StreamWriter sw = new StreamWriter("commands.txt", true);
            sw.WriteLine("[" + DateTime.Now.Hour + ":" +
                DateTime.Now.Minute + ":" +
                DateTime.Now.Second + ":" +
                DateTime.Now.Millisecond + "] - " + serverMessage);
            sw.Close();

            if (serverMessage.Length != 9)
                throw new Exception("Motherfucker fix this piece of item");

            return serverMessage;
        }

        public string ToString(bool asd)
        {
            string leftS =
                left.ToString().Length < 3 ? 0 + left.ToString() : left.ToString();
            string rightS =
                right.ToString().Length < 3 ? 0 + right.ToString() : right.ToString();

            if (rightS.Length == 2)
                rightS = "0" + rightS;
            if (leftS.Length == 2)
                leftS = "0" + leftS;

            var serverMessage = leftS + rightS;    // 6 chars
            serverMessage += lback.ToString() + rback;      // 2 chars
            serverMessage += gameMode.ToString();           // 1 char

            if (serverMessage.Length != 9)
                throw new Exception("Motherfucker fix this piece of item");

            return serverMessage;
        }
    }
}