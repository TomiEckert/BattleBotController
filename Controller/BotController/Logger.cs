using System;
using System.Collections.Generic;
using System.Threading;

namespace Controller {
    internal class Logger {
        public static readonly Logger       Instance  = new Logger();
        public readonly       List<string> _buffer   = new List<string>();
        public readonly List<string> _file = new List<string>();
        public int speed = 150;

        public void Log(string text) {
            _buffer.Add(text);
            LogFile(text);
        }

        public string getRobotName(int num)
        {
            string result = "";
            switch (num)
            {
                case 1:
                    result = "IT1A";
                    break;
                case 2:
                    result = "IT1B";
                    break;
                case 3:
                    result = "IT1C";
                    break;
                case 4:
                    result = "IT1D";
                    break;
                case 5:
                    result = "IT1E";
                    break;
                default:
                    break;
            }
            return result;
        }

        public int getRobotID(string name)
        {
            int result = 0;
            switch (name)
            {
                case "IT1A":
                    result = 1;
                    break;
                case "IT1B":
                    result = 2;
                    break;
                case "IT1C":
                    result = 3;
                    break;
                case "IT1D":
                    result = 4;
                    break;
                case "IT1E":
                    result = 5;
                    break;
                default:
                    break;
            }
            return result;
        }

        public void LogFile(string text)
        {
            string entry = "[" + DateTime.Now.Hour + ":" +
                DateTime.Now.Minute + ":" +
                DateTime.Now.Second + ":" +
                DateTime.Now.Millisecond + "] - " + text;
            _file.Add(entry);
        }
    }
}