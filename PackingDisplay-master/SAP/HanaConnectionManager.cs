//using SAP.Middleware.Connector;
//using PackingDisplay.Services;

//namespace PackingDisplay.SAP
//{
//    public class HanaConnectionManager
//    {
//        private static bool _isRegistered = false;
//        private static readonly object _lock = new object();

//        public static RfcDestination GetDestination(SapConnectionService sapService)
//        {
//            if (!_isRegistered)
//            {
//                lock (_lock)
//                {
//                    if (!_isRegistered)
//                    {
//                        try
//                        {
//                            var config = new HanaConfig(sapService);
//                            RfcDestinationManager.RegisterDestinationConfiguration(config);
//                            _isRegistered = true;
//                        }
//                        catch (RfcInvalidStateException)
//                        {
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
using System;

namespace PackingDisplay.SAP
{
    public class HanaConnectionManager
    {
        private static bool _isRegistered = false;
        private static readonly object _lock = new object();

        public static RfcDestination GetDestination(
            SapConnectionService sapService,
            LogService logService) // 🔥 inject log service
        {
            try
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
                            catch (RfcInvalidStateException ex)
                            {
                                // Already registered
                                logService.LogError(
                                    ex,
                                    "",
                                    "RegisterDestination",
                                    "HanaConnectionManager",
                                    "Already registered"
                                );

                                _isRegistered = true;
                            }
                            catch (Exception ex)
                            {
                                logService.LogError(
                                    ex,
                                    "",
                                    "RegisterDestination",
                                    "HanaConnectionManager",
                                    "Registration failed"
                                );

                                throw;
                            }
                        }
                    }
                }

                var destination = RfcDestinationManager.GetDestination("DEP");

                try
                {
                    destination.Ping(); // 🔥 connection test
                }
                catch (Exception ex)
                {
                    logService.LogError(
                        ex,
                        "",
                        "SAP Ping",
                        "HanaConnectionManager",
                        "SAP connection failed"
                    );

                    throw;
                }

                return destination;
            }
            catch (Exception ex)
            {
                logService.LogError(
                    ex,
                    "",
                    "GetDestination",
                    "HanaConnectionManager",
                    "Failed to get SAP destination"
                );

                throw;
            }
        }
    }
}