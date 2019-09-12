// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Dapper;
using DotNetCore.CAP.Persistence;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace DotNetCore.CAP.MySql
{
    public class MySqlStorageInitializer : IStorageInitializer
    {
        private readonly IOptions<MySqlOptions> _options;
        private readonly ILogger _logger;

        public MySqlStorageInitializer(
            ILogger<MySqlStorageInitializer> logger,
            IOptions<MySqlOptions> options, 
            IOptions<CapOptions> capOptions)
        {
            _options = options;
            _logger = logger;
        }

        public string GetPublishedTableName()
        {
            return $"{_options.Value.TableNamePrefix}.published";
        }

        public string GetReceivedTableName()
        {
            return $"{_options.Value.TableNamePrefix}.received";
        }

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var sql = CreateDbTablesScript(_options.Value.TableNamePrefix);
            using (var connection = new MySqlConnection(_options.Value.ConnectionString))
            {
                await connection.ExecuteAsync(sql);
            }

            _logger.LogDebug("Ensuring all create database tables script are applied.");
        }


        protected virtual string CreateDbTablesScript(string prefix)
        {
            var batchSql =
                $@"
CREATE TABLE IF NOT EXISTS `{prefix}.received` (
  `Id` bigint NOT NULL,
  `Version` varchar(20) DEFAULT NULL,
  `Name` varchar(400) NOT NULL,
  `Group` varchar(200) DEFAULT NULL,
  `Content` longtext,
  `Retries` int(11) DEFAULT NULL,
  `Added` datetime NOT NULL,
  `ExpiresAt` datetime DEFAULT NULL,
  `StatusName` varchar(50) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `{prefix}.published` (
  `Id` bigint NOT NULL,
  `Version` varchar(20) DEFAULT NULL,
  `Name` varchar(200) NOT NULL,
  `Content` longtext,
  `Retries` int(11) DEFAULT NULL,
  `Added` datetime NOT NULL,
  `ExpiresAt` datetime DEFAULT NULL,
  `StatusName` varchar(40) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
";
            return batchSql;
        }
    }
}