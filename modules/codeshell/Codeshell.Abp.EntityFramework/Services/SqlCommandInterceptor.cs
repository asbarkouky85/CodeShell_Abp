using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using System.Threading;
using System.Text;
using System;

namespace Codeshell.Abp.Services
{
    public class SqlCommandInterceptor : DbCommandInterceptor
    {
        private readonly List<string> _capturedCommands;
        private bool _isActive = false;
        public SqlCommandInterceptor()
        {
            _capturedCommands = new List<string>();
        }

        public void StartCapturing()
        {
            _isActive = true;
            _capturedCommands.Clear();
        }

        public void StopCapturing() { _isActive = false; }

        public List<string> GetCommands() { return _capturedCommands; }

        private string GetFullCommandText(DbCommand command)
        {
            if (command == null) return string.Empty;

            var sb = new StringBuilder();


            if (command.Parameters.Count > 0)
            {
                sb.AppendLine("-- Parameters:");
                foreach (DbParameter param in command.Parameters)
                {
                    var paramValue = param.Value == DBNull.Value ? "NULL" :
                        GetSqlValue(param);

                    sb.AppendLine($"DECLARE {param.ParameterName} {GetSqlType(param)} = {paramValue};");
                }
            }
            sb.AppendLine(command.CommandText);

            return sb.ToString();
        }

        private string GetSqlType(DbParameter param)
        {
            // Map common .NET types to SQL types
            return param.DbType switch
            {
                System.Data.DbType.String => "NVARCHAR(MAX)",
                System.Data.DbType.Int32 => "INT",
                System.Data.DbType.Int64 => "BIGINT",
                System.Data.DbType.Decimal => "DECIMAL(18,2)",
                System.Data.DbType.DateTime => "DATETIME",
                System.Data.DbType.Boolean => "BIT",
                System.Data.DbType.Guid => "UNIQUEIDENTIFIER",
                _ => "NVARCHAR(MAX)" // Default fallback
            };
        }

        private string GetSqlValue(DbParameter param)
        {
            // Map common .NET types to SQL types
            return param.DbType switch
            {
                System.Data.DbType.String => $"N'{param.Value.ToString()}'",
                System.Data.DbType.Int32 => param.Value.ToString(),
                System.Data.DbType.Int64 => param.Value.ToString(),
                System.Data.DbType.Decimal => param.Value.ToString(),
                System.Data.DbType.DateTime => $"'{param.Value.ToString()}'",
                System.Data.DbType.Boolean => param.Value.ToString(),
                System.Data.DbType.Guid => $"'{param.Value.ToString()}'",
                _ => "''" // Default fallback
            };
        }

        public override InterceptionResult<DbDataReader> ReaderExecuting(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result)
        {
            if (_isActive && command != null)
            {
                _capturedCommands.Add(GetFullCommandText(command));
            }
            return result;
        }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = default)
        {
            if (_isActive && command != null)
            {
                _capturedCommands.Add(GetFullCommandText(command));
            }
            return new ValueTask<InterceptionResult<DbDataReader>>(result);
        }
    }
}
