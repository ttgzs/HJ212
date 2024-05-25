﻿using HJ212.Model;
using System.Text;
using TopPortLib.Interfaces;

namespace HJ212.Request
{
    internal class ResponseReq(RspInfo rspInfo) : IByteStream
    {
        public byte[] ToBytes()
        {
            var cmd = $"{rspInfo.QN};ST=91;CN={(int)CN_Client.请求应答};{rspInfo.PW};{rspInfo.MN};Flag={0 | (int)GB._version};CP=&&QnRtn=1&&";
            cmd = GB.GetGbCmd(cmd);
            return Encoding.ASCII.GetBytes(cmd);
        }
    }
}
