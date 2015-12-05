﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolskaBot.Core.Darkorbit.Commands.PostHandshake
{
    class Login : Command
    {
        public const ushort ID = 19667;

        public int userID { get; private set; }
        public string sessionID { get; private set; }
        public short factionID { get; private set; }
        public int instanceID { get; private set; }
        public string version { get; private set; }

        public Login(int userID, string sessionID, short factionID, int instanceID, string version = Config.VERSION)
        {
            this.userID = userID;
            this.sessionID = sessionID;
            this.factionID = factionID;
            this.instanceID = instanceID;
            this.version = version;
            Write();
        }

        public override void Write()
        {
            Console.WriteLine(packetWriter.Encoding.EncodingName);
            short totalLength = (short)(sessionID.Length + version.Length + 16);
            packetWriter.Write(totalLength);
            packetWriter.Write(ID);
            packetWriter.Write((int)(instanceID >> 11 | instanceID << 21));
            packetWriter.Write((short)factionID);
            packetWriter.Write((byte)0);
            packetWriter.Write(sessionID);
            packetWriter.Write((byte)0);
            packetWriter.Write(version);
            packetWriter.Write((int)(userID >> 9 | userID << 23));
            Console.WriteLine("Calculated: {0}, real: {1} || SID: {2}, VERSION: {3}", totalLength, ToArray().Length - 2, sessionID.Length, version.Length);
        }
    }
}