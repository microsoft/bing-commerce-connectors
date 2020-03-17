// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Bing.Commerce.Connectors.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using Microsoft.Bing.Commerce.Connectors.Core;
    using Microsoft.EntityFrameworkCore;

    class DBAccess : Core.IDataReader
    {
        private readonly DbConfig config;
        private readonly DbContext db;

        private static readonly Dictionary<DatabaseServer, Func<DbConfig, DbContextOptionsBuilder>> dbContextMap = new Dictionary<DatabaseServer, Func<DbConfig, DbContextOptionsBuilder>>()
        {
            { DatabaseServer.SqlServer, (c) => new DbContextOptionsBuilder().UseSqlServer(c.ConnectionString) },
            { DatabaseServer.MySql, (c) => new DbContextOptionsBuilder().UseMySql(c.ConnectionString) },
            { DatabaseServer.PostreSQL, (c) => new DbContextOptionsBuilder().UseNpgsql(c.ConnectionString) },
            { DatabaseServer.Sqlite, (c) => new DbContextOptionsBuilder().UseSqlite(c.ConnectionString) }
        };

        public DBAccess(DbConfig config)
        {
            this.config = config;
            db = new DbContext(dbContextMap[config.Server](config).Options);
        }

        public IEnumerable<DataPoint> ReadNext(IDataCheckpoint checkpoint)
        {
            using (var cmd = db.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = GetCommand(checkpoint);
                AddParameterIfNeeded(checkpoint, cmd);
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }

                string newCheckpoint = null;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Dictionary<string, object> record = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            record.Add(reader.GetName(i), reader[i]);
                        }
                        if (!record.ContainsKey(config.CheckpointColumn))
                        {
                            throw new Exception("Checkpoint column does not exist in the query, can't proceed");
                        }

                        newCheckpoint = record[config.CheckpointColumn].ToString();
                        record.Remove(config.CheckpointColumn);

                        yield return new DataPoint() { Record = record, OperationType = DataOperation.Update, Checkpoint = newCheckpoint };
                    }
                }
            }
        }

        private string GetCommand(IDataCheckpoint checkpoint)
        {
            var command = config.SqlStatement;
            if (checkpoint.IsValid())
            {
                return config.UpdateSqlStatement;
            }

            return command;
        }

        private void AddParameterIfNeeded(IDataCheckpoint checkpoint, DbCommand cmd)
        {
            if (checkpoint.IsValid())
            {
                var newParam = cmd.CreateParameter();
                newParam.ParameterName = "@CHECKPOINT";
                newParam.DbType = DbType.String;
                newParam.Direction = ParameterDirection.Input;
                newParam.Value = checkpoint.GetValue();

                cmd.Parameters.Add(newParam);
            }
        }
    }
}
