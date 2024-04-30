﻿using HJ212.Model;
using System.Text;
using TopPortLib.Interfaces;
using Utils;

namespace HJ212.Response
{
    internal class GetDayDataRsp : IAsyncResponse<(DateTime BeginTime, DateTime EndTime, RspInfo RspInfo)>
    {
        private DateTime _beginTime;
        private DateTime _endTime;
        private RspInfo _rspInfo = new();
        public async Task AnalyticalData(byte[] bytes)
        {
            var str = Encoding.ASCII.GetString(bytes.Skip(6).ToArray());
            var datalist = str.Split([";", ",", "&&"], StringSplitOptions.RemoveEmptyEntries).Where(item => item.Contains('=') && !item.Contains("CP"));
            _rspInfo.QN = datalist.FirstOrDefault(item => item.Contains("QN"));
            _rspInfo.ST = datalist.FirstOrDefault(item => item.Contains("ST"));
            _rspInfo.PW = datalist.FirstOrDefault(item => item.Contains("PW"));
            _rspInfo.MN = datalist.FirstOrDefault(item => item.Contains("MN"));
            if (!DateTime.TryParseExact(datalist.SingleOrDefault(item => item.Contains("BeginTime"))?.Split('=')[1], "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out _beginTime))
            {
                throw new ArgumentException($"{GB._name} HJ212 Get DayData BeginTime Error");
            }
            if (!DateTime.TryParseExact(datalist.SingleOrDefault(item => item.Contains("EndTime"))?.Split('=')[1], "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out _endTime))
            {
                throw new ArgumentException($"{GB._name} HJ212 Get DayData EndTime Error");
            }
            await Task.CompletedTask;
        }

        public (bool Type, byte[]? CheckBytes) Check(byte[] bytes)
        {
            var data = bytes.Skip(6).ToArray();
            var dstr = Encoding.ASCII.GetString(data);
            if (StringByteUtils.BytesToString(CRC.GBcrc16(data, data.Length - 4)).Replace(" ", "") != dstr[^4..])
            {
                throw new ArgumentException($"{GB._name} HJ212 CRC Error: {dstr}", nameof(bytes));
            }
            var rs = dstr.Split(';');
            return (rs.Where(item => item.Contains("CN=2031")).Any(), default);
        }

        public (DateTime BeginTime, DateTime EndTime, RspInfo RspInfo) GetResult()
        {
            return (_beginTime, _endTime, _rspInfo);
        }
    }
}