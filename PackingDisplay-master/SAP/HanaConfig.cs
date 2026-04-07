//using SAP.Middleware.Connector;

//namespace PackingDisplay.SAP
//{
//    public class HanaConfig : IDestinationConfiguration
//    {
//        public event RfcDestinationManager.ConfigurationChangeHandler ConfigurationChanged;

//        public bool ChangeEventsSupported() => false;

//        public RfcConfigParameters GetParameters(string destinationName)
//        {
//            // ✅ Must match exactly with GetDestination("DEP")
//            if (!string.Equals(destinationName, "DEP", StringComparison.OrdinalIgnoreCase))
//                return null;

//            return new RfcConfigParameters
//    {
//        { RfcConfigParameters.Name, "DEP" },
//        { RfcConfigParameters.AppServerHost, "10.17.114.122" },
//        { RfcConfigParameters.SystemNumber, "00" },
//        { RfcConfigParameters.Client, "100" },
//        { RfcConfigParameters.User, "SP7IT102" },
//        { RfcConfigParameters.Password, "Aby@032024" },
//        { RfcConfigParameters.Language, "EN" },

//        // 🔥 PERFORMANCE + STABILITY SETTINGS
//        { RfcConfigParameters.PoolSize, "5" },          // connection pool
//        { RfcConfigParameters.PeakConnectionsLimit, "10" },
//        { RfcConfigParameters.ConnectionIdleTimeout, "600" } // seconds
//    };
//        }


//    }
//}
using SAP.Middleware.Connector;
using PackingDisplay.Services;
using PackingDisplay.Models;
using System;

namespace PackingDisplay.SAP
{
    public class HanaConfig : IDestinationConfiguration
    {
        private readonly SapConnectionService _sapService;
        private SAPConnectionConfig _cachedConfig;

        public HanaConfig(SapConnectionService sapService)
        {
            _sapService = sapService;
        }

        public event RfcDestinationManager.ConfigurationChangeHandler ConfigurationChanged;

        public bool ChangeEventsSupported() => false;

        public RfcConfigParameters GetParameters(string destinationName)
        {
            if (!string.Equals(destinationName, "DEP", StringComparison.OrdinalIgnoreCase))
                return null;

            try
            {
                _cachedConfig ??= _sapService.GetActiveConfig();

                if (_cachedConfig == null)
                    throw new Exception("❌ No active SAP configuration found in DB");

                //Console.WriteLine("✅ SAP CONFIG LOADED FROM DB");

                var config = new RfcConfigParameters
                {
                    { RfcConfigParameters.Name, _cachedConfig.Name },
                    { RfcConfigParameters.AppServerHost, _cachedConfig.AppServerHost },
                    { RfcConfigParameters.SystemNumber, _cachedConfig.SystemNumber },
                    { RfcConfigParameters.Client, _cachedConfig.Client },
                    { RfcConfigParameters.User, _cachedConfig.User },
                    { RfcConfigParameters.Password, _cachedConfig.Password },
                    { RfcConfigParameters.Language, _cachedConfig.Language },

                    { RfcConfigParameters.PoolSize, _cachedConfig.PoolSize.ToString() },
                    { RfcConfigParameters.PeakConnectionsLimit, (_cachedConfig.PeakConnectionLimit ?? 10).ToString() },
                    { RfcConfigParameters.ConnectionIdleTimeout, (_cachedConfig.ConnectionIdleTimeout ?? 600).ToString() }
                };

                // ✅ SAP Router (optional)
                if (!string.IsNullOrEmpty(_cachedConfig.SAPRouter))
                {
                    config.Add(RfcConfigParameters.SAPRouter, _cachedConfig.SAPRouter);
                }

                return config;
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ SAP CONFIG ERROR: " + ex.Message);
                throw;
            }
        }
    }
}