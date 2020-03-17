// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Bing.Commerce.Connectors.EntityFramework
{
    using Microsoft.Bing.Commerce.Connectors.Core.Config;
    using Microsoft.Bing.Commerce.Connectors.Core.Utilities;

    class Config
    {
        public PollingConnectorConfig ConnectorConfig { get; set; }
        public BingCommerceConfig PushConfig { get; set; }
        public DbConfig DatabaseConfig { get; set; }

        public Config Check()
        {
            ConnectorConfig.Check();
            PushConfig.Check();
            DatabaseConfig.Check();

            return this;
        }
    }

    class DbConfig
    {
        public string ConnectionString { get; set; }
        public DatabaseServer Server { get; set; }
        public string SqlStatement { get; set; }
        public string UpdateSqlStatement { get; set; }
        public string CheckpointColumn { get; set; }
        public string CheckpointFile { get; set; }

        public DbConfig Check()
        {
            Require.Instance.IsNotNull(this.ConnectionString, nameof(ConnectionString))
                .IsNotNull(this.Server, nameof(Server))
                .IsNotNull(this.SqlStatement, nameof(SqlStatement))
                .IsNotNull(this.UpdateSqlStatement, nameof(UpdateSqlStatement))
                .IsNotNull(this.CheckpointColumn, nameof(CheckpointColumn))
                .IsNotNull(this.CheckpointFile, nameof(CheckpointFile));

            return this;
        }
    }

    enum DatabaseServer
    {
        SqlServer,
        Sqlite,
        PostreSQL,
        MySql
    }
}
