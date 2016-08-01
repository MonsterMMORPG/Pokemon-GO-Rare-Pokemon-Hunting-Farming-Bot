﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoGo.NecroBot.CLI.WebSocketHandler.BasicGetCommands.Events
{
    class TrainerProfileResponce : IWebSocketResponce
    {
        public TrainerProfileResponce(dynamic data, string requestID)
        {
            Command = "TrainerProfile";
            Data = data;
            RequestID = requestID;
        }
        public string RequestID { get; private set; }
        public string Command { get; private set; }
        public dynamic Data { get; private set; }
    }
}
