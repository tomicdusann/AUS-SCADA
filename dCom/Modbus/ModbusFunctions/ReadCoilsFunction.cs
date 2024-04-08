﻿using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read coil functions/requests.
    /// </summary>
    public class ReadCoilsFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadCoilsFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
		public ReadCoilsFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc/>
        public override byte[] PackRequest()
        {
            //TO DO: IMPLEMENT
            //throw new NotImplementedException();

            ModbusReadCommandParameters parameters = CommandParameters as ModbusReadCommandParameters;

            byte[] request = new byte[12];

            request[0] = BitConverter.GetBytes(parameters.TransactionId)[1];
            request[1] = BitConverter.GetBytes(parameters.TransactionId)[0];
            request[2] = BitConverter.GetBytes(parameters.ProtocolId)[1];
            request[3] = BitConverter.GetBytes(parameters.ProtocolId)[0];
            request[4] = BitConverter.GetBytes(parameters.Length)[1];
            request[5] = BitConverter.GetBytes(parameters.Length)[0];
            request[6] = parameters.UnitId;
            request[7] = parameters.FunctionCode;
            request[8] = BitConverter.GetBytes(parameters.StartAddress)[1];
            request[9] = BitConverter.GetBytes(parameters.StartAddress)[0];
            request[10] = BitConverter.GetBytes(parameters.Quantity)[1];
            request[11] = BitConverter.GetBytes(parameters.Quantity)[0];

            return request;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            //TO DO: IMPLEMENT
            //throw new NotImplementedException();

            Dictionary<Tuple<PointType, ushort>, ushort> responseDict = new Dictionary<Tuple<PointType, ushort>, ushort>();

            int byteCount = response[8];
            ushort startAddress = ((ModbusReadCommandParameters)CommandParameters).StartAddress;
            ushort counter = 0;

            for (int i = 0; i < byteCount; i++) { 
            
                byte temp = response[9 + i];
                byte mask = 1;

                ushort quantity = ((ModbusReadCommandParameters)CommandParameters).Quantity;

                for (int j = 0; j < 8; j++) { 
                
                    ushort value = (ushort)(temp & mask);
                    responseDict.Add(new Tuple<PointType, ushort>(PointType.DIGITAL_OUTPUT, startAddress++), value);

                    temp >>= 1;
                    counter++;

                    if (counter >= quantity) { 
                    
                        break;
                    }

                }
            }

            return responseDict;
        }
    }
}