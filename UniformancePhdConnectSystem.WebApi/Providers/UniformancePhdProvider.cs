using Serilog;
using System;
using System.Threading;
using Uniformance.PHD;

namespace UniformancePhdConnectSystem.WebApi.Providers
{
    public sealed class UniformancePhdProvider : IDisposable
    {
        private readonly ILogger _logger = Log.ForContext<UniformancePhdProvider>();

        private static readonly Lazy<UniformancePhdProvider> _instance 
            = new Lazy<UniformancePhdProvider>(() => new UniformancePhdProvider());
        public static UniformancePhdProvider Instance => _instance.Value;

        private readonly object _syncLock = new object();
        public object SyncLock => _syncLock;

        private int _cleanupStarted = 0;

        public PHDHistorian GlobalHistorian { get; private set; }
        private Timer _keepAliveTimer;

        private UniformancePhdProvider() { }

        ~UniformancePhdProvider() => CleanupResources();

        public void Dispose()
        {
            CleanupResources();
            GC.SuppressFinalize(this);
        }

        public void Initialize(string host, int port)
        {
            lock (_syncLock)
            {
                if (GlobalHistorian == null)
                {
                    _logger.Information("Initializing UniformancePHD connection to: PHD-L35-1A[10.94.0.241]...");

                    GlobalHistorian = new PHDHistorian();
                    GlobalHistorian.DefaultServer = new PHDServer(host, SERVERVERSION.API200) { Port = port };

                    try
                    {
                        GlobalHistorian.DefaultServer.ConnectToServer();
                        _logger.Information("UniformancePHD connection established successfully! API version: {Version}", GlobalHistorian.DefaultServer.APIVersion);

                        _keepAliveTimer = new Timer(KeepAlive, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, $"UniformancePHD Initialization failed for: PHD-L35-1A[10.94.0.241]");
                        CleanupResources();
                        throw;
                    }
                }
            }
        }

        public void Shutdown()
        {
            lock (_syncLock)
            {
                _logger.Information("Shutting down UniformancePHD Provider...");
                CleanupResources();
            }
        }

        private void KeepAlive(object state)
        {
            lock (_syncLock)
            {
                if (GlobalHistorian == null)
                    return;

                try
                {
                    var version = GlobalHistorian.DefaultServer.APIVersion;
                    this._logger.Information("UniformancePHD connection Keep-Alive successfully!");
                }
                catch(Exception ex)
                {
                    this._logger.Error(ex, $"UniformancePHD connection Keep-Alive failed: {ex.Message}!");
                }
            }
        }

        private void CleanupResources()
        {
            if (Interlocked.CompareExchange(ref _cleanupStarted, 1, 0) != 0)
                return;

            try
            {
                if (_keepAliveTimer != null)
                {
                    _keepAliveTimer.Dispose();
                    _logger.Debug("UniformancePHD Keep-alive timer disposed.");
                }

                if (GlobalHistorian != null)
                {
                    GlobalHistorian.Dispose();
                    _logger.Information("UniformancePHD Historian session closed via Dispose.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during UniformancePHD resource cleanup.");
            }
            finally
            {
                GlobalHistorian = null;
                _keepAliveTimer = null;
                Interlocked.Exchange(ref _cleanupStarted, 0);
                _logger.Debug("UniformancePHD Provider singleton references reset to null.");
            }
        }
    }
}