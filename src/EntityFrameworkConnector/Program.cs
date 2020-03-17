// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Bing.Commerce.Connectors.EntityFramework
{
    using System;
    using System.IO;
    using Microsoft.Bing.Commerce.Connectors.Core;
    using YamlDotNet.Serialization;

    class Program
    {
        public static int Main(string[] args)
        {
            var config = LoadConfig(args);
            if (config == null)
            {
                return 2;
            }

            var checkpoint = new Checkpoint(config.DatabaseConfig.CheckpointFile);

            using (var client = new SimpleBingCommercePusher(config.PushConfig, checkpoint))
            {
                var connector = new PollingConnector(config.ConnectorConfig,
                    new DBAccess(config.DatabaseConfig),
                    checkpoint,
                    client);

                connector.RunAsync().Wait();
            }
            return 0;
        }

        private static Config LoadConfig(string[] args)
        {
            string configFile = "connector.yml";
            if (args.Length > 0)
            {
                configFile = args[0];
            }
            if (!File.Exists(configFile))
            {
                Console.WriteLine($"Could not find file [{configFile}]. Please provide a valid configurations file.");
                return null;
            }

            Deserializer deserializer = new Deserializer();
            try
            {
                return deserializer.Deserialize<Config>(File.ReadAllText(configFile)).Check();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not load config from file [{configFile}]. Please provide a valid configurations file. Error: [{e.Message}]");
                return null;
            }
        }
    }
}
