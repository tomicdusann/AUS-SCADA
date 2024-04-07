using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read input registers functions/requests.
    /// </summary>
    public class ReadInputRegistersFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadInputRegistersFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public ReadInputRegistersFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            Console.WriteLine("Request started");
            // ModbusReadCommandParameters nam treba
            //byte[] recVal = new byte[12];

            ModbusReadCommandParameters parameters = CommandParameters as ModbusReadCommandParameters;

            //Head message

            // Data message
            byte[] req = new byte[12];

            req[0] = BitConverter.GetBytes(parameters.TransactionId)[1];
            req[1] = BitConverter.GetBytes(parameters.TransactionId)[0];
            req[2] = BitConverter.GetBytes(parameters.ProtocolId)[1];
            req[3] = BitConverter.GetBytes(parameters.ProtocolId)[0];
            req[4] = BitConverter.GetBytes(parameters.Length)[1];
            req[5] = BitConverter.GetBytes(parameters.Length)[0];
            req[6] = parameters.UnitId;
            req[7] = parameters.FunctionCode;
            req[8] = BitConverter.GetBytes(parameters.StartAddress)[1];
            req[9] = BitConverter.GetBytes(parameters.StartAddress)[0];
            req[10] = BitConverter.GetBytes(parameters.Quantity)[1];
            req[11] = BitConverter.GetBytes(parameters.Quantity)[0];

            return req;

            //Console.WriteLine("Request ended");
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            //TO DO: IMPLEMENT
            //throw new NotImplementedException();

            Dictionary<Tuple<PointType, ushort>, ushort> responseDict = new Dictionary<Tuple<PointType, ushort>, ushort>();

            int byteCounter = response[8];
            ushort startAddress = ((ModbusReadCommandParameters)CommandParameters).StartAddress;

            for (int i = 0; i < byteCounter; i += 2) { 
        
                ushort value = (ushort)IPAddress.NetworkToHostOrder((short)BitConverter.ToUInt16(response, 9 + i));
                Tuple<PointType, ushort> tuple = new Tuple<PointType, ushort>(PointType.ANALOG_INPUT, startAddress++);
                responseDict.Add(tuple, value);
            }

            return responseDict;
        }
    }
}