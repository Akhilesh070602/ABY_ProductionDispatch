//using System;
//using System.IO.Ports;
//using System.Threading;

//namespace PackingDisplay.Services
//{
//    public class WeighmentService
//    {
//        public string ReadWeight()
//        {
//            try
//            {
//                using (SerialPort port = new SerialPort("COM1", 9600))
//                {
//                    port.Parity = Parity.None;
//                    port.DataBits = 8;
//                    port.StopBits = StopBits.One;
//                    port.Handshake = Handshake.None;

//                    port.ReadTimeout = 2000;

//                    port.Open();

//                    Thread.Sleep(500);

//                    string data = port.ReadExisting(); // 🔥 FIX

//                    if (string.IsNullOrWhiteSpace(data))
//                        return "ERROR: NO DATA";

//                    return data.Trim();
//                }
//            }
//            catch (Exception ex)
//            {
//                return "ERROR: " + ex.Message;
//            }
//        }
//    }

//}
using System.IO.Ports;

namespace PackingDisplay.Services
{
    public class WeighmentService
    {
        private readonly LogService _logService;

        public WeighmentService(LogService logService)
        {
            _logService = logService;
        }

        public string ReadWeight()
        {
            try
            {
                var ports = SerialPort.GetPortNames();

                if (ports.Length == 0)
                    throw new Exception("No COM port found");

                string portName = ports.First();

                using (SerialPort port = new SerialPort(portName, 9600))
                {
                    port.Parity = Parity.None;
                    port.DataBits = 8;
                    port.StopBits = StopBits.One;
                    port.Handshake = Handshake.None;
                    port.ReadTimeout = 2000;

                    port.Open();

                    Thread.Sleep(500);

                    string data = port.ReadExisting();

                    if (string.IsNullOrWhiteSpace(data))
                        throw new Exception("No data from device");

                    // ✅ ONLY SUCCESS LOG
                    _logService.LogSuccess("", "ReadWeight", "WeighmentService");

                    return data.Trim();
                }
            }
            catch (Exception ex)
            {
                _logService.LogError(
                    ex,
                    "",
                    "ReadWeight",
                    "WeighmentService",
                    "Device read error"
                );

                throw;
            }
        }
    }
}