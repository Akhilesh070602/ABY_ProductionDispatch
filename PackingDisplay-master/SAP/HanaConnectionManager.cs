//using SAP.Middleware.Connector;

//namespace PackingDisplay.SAP
//{
//    public class HanaConnectionManager
//    {
//        private static bool _isRegistered = false;
//        private static readonly object _lock = new object();

//        public static RfcDestination GetDestination()
//        {
//            if (!_isRegistered)
//            {
//                lock (_lock)
//                {
//                    if (!_isRegistered)
//                    {
//                        try
//                        {
//                            RfcDestinationManager.RegisterDestinationConfiguration(new HanaConfig());
//                            _isRegistered = true;
//                        }
//                        catch (RfcInvalidStateException)
//                        {
//                            // ✅ Already registered → ignore
//                            _isRegistered = true;
//                        }
//                    }
//                }
//            }

//            var destination = RfcDestinationManager.GetDestination("DEP");

//            destination.Ping();

//            return destination;
//        }
//    }
//}

using SAP.Middleware.Connector;
using PackingDisplay.Services;

namespace PackingDisplay.SAP
{
    public class HanaConnectionManager
    {
        private static bool _isRegistered = false;
        private static readonly object _lock = new object();

        public static RfcDestination GetDestination(SapConnectionService sapService)
        {
            if (!_isRegistered)
            {
                lock (_lock)
                {
                    if (!_isRegistered)
                    {
                        try
                        {
                            var config = new HanaConfig(sapService);
                            RfcDestinationManager.RegisterDestinationConfiguration(config);
                            _isRegistered = true;
                        }
                        catch (RfcInvalidStateException)
                        {
                            _isRegistered = true;
                        }
                    }
                }
            }

            var destination = RfcDestinationManager.GetDestination("DEP");
            destination.Ping();

            return destination;
        }
    }
}