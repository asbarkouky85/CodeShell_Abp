using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using Microsoft.Extensions.DependencyInjection;

namespace Codeshell.Abp.Services
{
    /// <summary>
    /// Generic class for capturing SQL commands from a DbContext without executing them
    /// </summary>
    /// <typeparam name="TDbContext">The type of DbContext to use</typeparam>
    public partial class SqlCommandCapturere<TDbContext> : ISqlCommandCapturere<TDbContext> where TDbContext : DbContext, ICaptureSqlDbContext
    {
        private TDbContext _dbContext;
        private readonly IAbpLazyServiceProvider lazy;
        private IDbContextTransaction _transaction;
        private IUnitOfWorkManager _unitOfWorkManager => lazy.LazyGetRequiredService<IUnitOfWorkManager>();

        public SqlCommandCapturere(IAbpLazyServiceProvider provider)
        {
            lazy = provider;
        }

        /// <summary>
        /// Starts capturing SQL commands without executing them
        /// </summary>
        private void _startCapturing()
        {
            // Start a transaction to prevent actual database changes
            _transaction = _dbContext.Database.BeginTransaction();

            _dbContext.StartCapturing();
        }

        /// <summary>
        /// Stops capturing SQL commands and returns the captured commands
        /// </summary>
        /// <returns>List of captured SQL commands</returns>
        private List<string> _stopCapturing()
        {
            var commands = _dbContext.GetCapturedCommands();

            // Rollback the transaction to prevent actual database changes
            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction.Dispose();
                _transaction = null;
            }
            _dbContext.StopCapturing();
            return commands;
        }


        /// <summary>
        /// Executes an async action and captures the SQL commands without executing them
        /// </summary>
        /// <param name="action">The async action to execute with the DbContext</param>
        /// <returns>List of captured SQL commands</returns>
        public async Task<List<string>> CaptureCommandsAsync(Func<TDbContext, Task> action)
        {
            List<string> commands = new List<string>();

            try
            {
                //using (var uow = _unitOfWorkManager.Begin(true, true))
                //{
                _dbContext = lazy.LazyGetRequiredService<TDbContext>();
                _startCapturing();
                await action(_dbContext);
                await _dbContext.SaveChangesAsync();
                //}
            }
            catch (Exception) { }
            finally
            {
                commands = _stopCapturing();
            }
            return commands;
        }

    }
}
