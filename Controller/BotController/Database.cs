using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Controller {
    internal class Database {
        public object[][]        DB_Info;
        public MySqlConnection UploadConnection   { get; private set; }
        public MySqlConnection DownloadConnection { get; private set; }

        public bool IsConnect() {
            if (UploadConnection == null) {
                var connstring = $"Server=localhost; database=Dashboard; UID=root; password=";
                UploadConnection   = new MySqlConnection(connstring);
                DownloadConnection = new MySqlConnection(connstring);

                try {
                    UploadConnection.Open();
                }
                catch (Exception e) {
                    Logger.Instance.Log("DownloadConnection: " + e.Message);
                    UploadConnection = null;

                    return false;
                }
                Logger.Instance.Log("Upload Connection has been established");

                try {
                    DownloadConnection.Open();
                }
                catch (Exception e) {
                    Logger.Instance.Log("DownloadConnection: " + e.Message);
                    DownloadConnection = null;

                    return false;
                }
                Logger.Instance.Log("Download Connection has been established");
            }

            return true;
        }

        public void Close() {
            UploadConnection?.Close();
            DownloadConnection?.Close();
            Logger.Instance.Log("Database connection closed");
        }

        /// <summary>
        ///     Returns all the info from the database.
        /// </summary>
        /// <returns>id, direction, gameMode, powerUp</returns>
        public void Query() {
            if (IsConnect()) {
                var query = "SELECT * FROM control";
                var cmd   = new MySqlCommand(query, DownloadConnection);

                try {
                    var reader = cmd.ExecuteReader();
                    var result = new List<object[]>();

                    while (reader.Read()) {
                        var tmp    = new List<object>();
                        var id       = reader.GetInt32(0);
                        var dir      = reader.GetString(1);
                        var gameMode = reader.GetInt32(2);
                        var powerUp  = reader.GetString(3);
                        tmp.Add(id);
                        tmp.Add(dir);
                        tmp.Add(gameMode);
                        tmp.Add(powerUp);
                        result.Add(tmp.ToArray());
                    }

                    reader.Close();

                    DB_Info = result.ToArray();
                }
                catch (Exception e) {
                    Logger.Instance.Log("Download: " + e.Message);

                    return;
                }
            }

            return;
        }
        
        /// <summary>
        ///     Updates the database with the data from the robots.
        /// </summary>
        /// <param name="input">Controller.GetAllRobotInfo() goes here</param>
        public void Update(string[][] input) {
            for (var i = 0; i < input.Length - 1; i++) {
                var query = "UPDATE control SET powerUp=@power, isMoving=@move WHERE id=@id_num";

                if (IsConnect()) {
                    var cmd = new MySqlCommand(query, UploadConnection);
                    cmd.Parameters.AddWithValue("@power",  input[i][2]);
                    cmd.Parameters.AddWithValue("@move",   input[i][3]);
                    cmd.Parameters.AddWithValue("@id_num", i);

                    try {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception e) {
                        Logger.Instance.Log("Upload[" + i + "]: " + e.Message);
                    }
                }
            }
        }
    }
}