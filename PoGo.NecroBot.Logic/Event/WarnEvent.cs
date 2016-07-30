﻿namespace PoGo.NecroBot.Logic.Event
{
    public class WarnEvent : IEvent
    {
        /// <summary>
        /// This event requires handler to perform input 
        /// </summary>
        public bool RequireInput;
        public string Message = "";

        public override string ToString()
        {
            return Message;
        }
    }
}