using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using Amoeba.Service;
using Omnius.Base;
using Omnius.Configuration;
using Omnius.Utilities;
using Omnius.Wpf;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Threading;
using System.Diagnostics;

namespace Amoeba.Interface
{
    class CloudControlViewModel : ManagerBase
    {
        private ServiceManager _serviceManager;
        private TaskManager _watchTaskManager;

        private Settings _settings;

        public ICollectionView ConnectionInfosView => CollectionViewSource.GetDefaultView(_connectionInfos);
        private ObservableDictionary<byte[], DynamicOptions> _connectionInfos = new ObservableDictionary<byte[], DynamicOptions>(new ByteArrayEqualityComparer());
        public ObservableCollection<object> ConnectionSelectedItems { get; } = new ObservableCollection<object>();
        private ListSortInfo _connectionSortInfo;
        public ReactiveCommand<string> ConnectionSortCommand { get; private set; }

        public ReactiveCommand ConnectionCopyCommand { get; private set; }
        public ReactiveCommand ConnectionPasteCommand { get; private set; }

        public ReactiveProperty<ManagerState> ServiceState { get; private set; }
        public ReactiveProperty<decimal> SendingSpeed { get; private set; }
        public ReactiveProperty<decimal> ReceivingSpeed { get; private set; }

        private TaskManager _statusBarTaskManager;
        private TaskManager _trafficMonitorTaskManager;

        public CloudStateInfo State { get; } = new CloudStateInfo();
        public ObservableDictionary<string, DynamicOptions> StateInfos { get; } = new ObservableDictionary<string, DynamicOptions>();
        public ObservableCollection<object> StateSelectedItems { get; } = new ObservableCollection<object>();

        public ReactiveCommand StateCopyCommand { get; private set; }

        public ObservableCollection<string> Logs { get; } = new ObservableCollection<string>();
        public ObservableCollection<object> LogSelectedItems { get; } = new ObservableCollection<object>();

        public ReactiveCommand LogCopyCommand { get; private set; }

        public DynamicOptions DynamicOptions { get; } = new DynamicOptions();

        private CompositeDisposable _disposable = new CompositeDisposable();
        private volatile bool _disposed;

        public CloudControlViewModel(ServiceManager serviceManager)
        {
            _serviceManager = serviceManager;

            this.Init();

            _watchTaskManager = new TaskManager(this.WatchThread);
            _watchTaskManager.Start();
        }

        private void Init()
        {
            {
                this.ConnectionSortCommand = new ReactiveCommand<string>().AddTo(_disposable);
                this.ConnectionSortCommand.Subscribe((propertyName) => this.ConnectionSort(propertyName)).AddTo(_disposable);

                this.ConnectionCopyCommand = this.ConnectionSelectedItems.ObserveProperty(n => n.Count).Select(n => n != 0).ToReactiveCommand().AddTo(_disposable);
                this.ConnectionCopyCommand.Subscribe(() => this.ConnectionCopy()).AddTo(_disposable);

                this.ConnectionPasteCommand = new ReactiveCommand().AddTo(_disposable);
                this.ConnectionPasteCommand.Subscribe(() => this.ConnectionPaste()).AddTo(_disposable);

                this.ServiceState = new ReactiveProperty<ManagerState>().AddTo(_disposable);
                this.SendingSpeed = new ReactiveProperty<decimal>().AddTo(_disposable);
                this.ReceivingSpeed = new ReactiveProperty<decimal>().AddTo(_disposable);

                this.StateCopyCommand = this.StateSelectedItems.ObserveProperty(n => n.Count).Select(n => n != 0).ToReactiveCommand().AddTo(_disposable);
                this.StateCopyCommand.Subscribe(() => this.StateCopy()).AddTo(_disposable);

                this.LogCopyCommand = this.LogSelectedItems.ObserveProperty(n => n.Count).Select(n => n != 0).ToReactiveCommand().AddTo(_disposable);
                this.LogCopyCommand.Subscribe(() => this.LogCopy()).AddTo(_disposable);
            }

            {
                string configPath = Path.Combine(AmoebaEnvironment.Paths.ConfigPath, "View", nameof(CloudControl));
                if (!Directory.Exists(configPath)) Directory.CreateDirectory(configPath);

                _settings = new Settings(configPath);
                int version = _settings.Load("Version", () => 0);

                _connectionSortInfo = _settings.Load("ConnectionSortInfo", () => new ListSortInfo());
                this.DynamicOptions.SetProperties(_settings.Load(nameof(DynamicOptions), () => Array.Empty<DynamicOptions.DynamicPropertyInfo>()));
            }

            {
                Backup.Instance.SaveEvent += () => this.Save();
            }

            {
                this.ConnectionSort(null);
            }

            {
                Log.LogEvent += (object sender, LogEventArgs e) =>
                {
                    try
                    {
                        App.Current.Dispatcher.InvokeAsync(() =>
                        {
                            try
                            {
                                if (this.Logs.Count > 100)
                                {
                                    this.Logs.RemoveAt(0);
                                }

                                this.Logs.Add(string.Format("{0} {1}:\t{2}", DateTime.Now.ToString(LanguagesManager.Instance.Global_DateTime_StringFormat, System.Globalization.DateTimeFormatInfo.InvariantInfo), e.MessageLevel, e.Message));
                            }
                            catch (Exception)
                            {

                            }
                        });
                    }
                    catch (Exception)
                    {

                    }
                };
            }
        }

        private void WatchThread(CancellationToken token)
        {
            var matchSizeTypeHashSet = new HashSet<string>()
            {
                "Cache_FreeSpace",
                "Cache_LockSpace",
                "Cache_UsingSpace",
                "Network_ReceivedByteCount",
                "Network_SentByteCount",
            };

            for (;;)
            {
                {
                    var dic = new Dictionary<byte[], Information>(new ByteArrayEqualityComparer());

                    foreach (var info in _serviceManager.GetConnectionInformations())
                    {
                        dic.Add(info.GetValue<byte[]>("Id"), info);
                    }

                    App.Current.Dispatcher.InvokeAsync(() =>
                    {
                        if (token.IsCancellationRequested) return;

                        foreach (var key in _connectionInfos.Keys.ToArray())
                        {
                            if (!dic.ContainsKey(key))
                            {
                                _connectionInfos.Remove(key);
                            }
                        }

                        foreach (var (key, info) in dic)
                        {
                            DynamicOptions viewModel;

                            if (!_connectionInfos.TryGetValue(key, out viewModel))
                            {
                                viewModel = new DynamicOptions();
                                _connectionInfos[key] = viewModel;
                            }

                            foreach (var (name, value) in info)
                            {
                                viewModel.SetValue(name, value);
                            }
                        }
                    });
                }

                {
                    App.Current.Dispatcher.InvokeAsync(() =>
                    {
                        if (token.IsCancellationRequested) return;

                        var location = _serviceManager.MyLocation;

                        if (location.Uris.Count() > 0)
                        {
                            this.State.Location = AmoebaConverter.ToLocationString(location);
                        }
                        else
                        {
                            this.State.Location = null;
                        }
                    });
                }

                {
                    var information = _serviceManager.Information;

                    App.Current.Dispatcher.InvokeAsync(() =>
                    {
                        if (token.IsCancellationRequested) return;

                        foreach (var (i, key, value) in information.Select((item, i) => (i, item.Key, item.Value)))
                        {
                            DynamicOptions viewModel;

                            if (!this.StateInfos.TryGetValue(key, out viewModel))
                            {
                                viewModel = new DynamicOptions();
                                this.StateInfos[key] = viewModel;
                            }

                            viewModel.SetValue("Index", i);

                            if (matchSizeTypeHashSet.Contains(key))
                            {
                                viewModel.SetValue("Content", NetworkConverter.ToSizeString((long)value));
                            }
                            else
                            {
                                viewModel.SetValue("Content", value.ToString());
                            }
                        }
                    });
                }

                if (token.WaitHandle.WaitOne(1000)) return;
            }
        }

        private void ConnectionSort(string propertyName)
        {
            if (propertyName == null)
            {
                this.ConnectionInfosView.SortDescriptions.Clear();

                if (!string.IsNullOrEmpty(_connectionSortInfo.PropertyName))
                {
                    this.ConnectionSort(_connectionSortInfo.PropertyName, _connectionSortInfo.Direction);
                }
            }
            else
            {
                var direction = ListSortDirection.Ascending;

                if (_connectionSortInfo.PropertyName == propertyName)
                {
                    if (_connectionSortInfo.Direction == ListSortDirection.Ascending)
                    {
                        direction = ListSortDirection.Descending;
                    }
                    else
                    {
                        direction = ListSortDirection.Ascending;
                    }
                }

                this.ConnectionInfosView.SortDescriptions.Clear();

                if (!string.IsNullOrEmpty(propertyName))
                {
                    this.ConnectionSort(propertyName, direction);
                }

                _connectionSortInfo.Direction = direction;
                _connectionSortInfo.PropertyName = propertyName;
            }
        }

        private void ConnectionSort(string propertyName, ListSortDirection direction)
        {
            switch (propertyName)
            {
                case "Type":
                    this.ConnectionInfosView.SortDescriptions.Add(new SortDescription("Value.Type", direction));
                    break;
                case "Id":
                    this.ConnectionInfosView.SortDescriptions.Add(new SortDescription("Value.Id", direction));
                    break;
                case "Priority":
                    this.ConnectionInfosView.SortDescriptions.Add(new SortDescription("Value.Priority", direction));
                    break;
                case "ReceivedByteCount":
                    this.ConnectionInfosView.SortDescriptions.Add(new SortDescription("Value.ReceivedByteCount", direction));
                    break;
                case "SentByteCount":
                    this.ConnectionInfosView.SortDescriptions.Add(new SortDescription("Value.SentByteCount", direction));
                    break;
            }
        }

        private void ConnectionCopy()
        {
            var list = new List<Location>();

            foreach (var (key, value) in this.ConnectionSelectedItems.Cast<KeyValuePair<byte[], DynamicOptions>>())
            {
                list.Add(value.GetValue<Location>("Location"));
            }

            Clipboard.SetLocations(list);
        }

        private void ConnectionPaste()
        {
            _serviceManager.SetCloudLocations(Clipboard.GetLocations());
        }

        private void Setting_StatusBar()
        {
            _trafficMonitorTaskManager = new TaskManager(this.TrafficMonitorThread);
            _trafficMonitorTaskManager.Start();

            _statusBarTaskManager = new TaskManager(this.StatusBarThread);
            _statusBarTaskManager.Start();
        }

        private volatile TrafficInformation _sentInfomation = new TrafficInformation();
        private volatile TrafficInformation _receivedInfomation = new TrafficInformation();

        private class TrafficInformation : ISynchronized
        {
            public long PreviousTraffic { get; set; }
            public int Round { get; set; }
            public decimal[] AverageTraffics { get; } = new decimal[3];
            public object LockObject { get; } = new object();
        }

        private void StatusBarThread(CancellationToken token)
        {
            try
            {
                for (;;)
                {
                    Thread.Sleep(1000);
                    if (token.IsCancellationRequested) return;

                    var state = _serviceManager.State;

                    App.Current.Dispatcher.Invoke(DispatcherPriority.Send, new TimeSpan(0, 0, 1), new Action(() =>
                    {
                        try
                        {
                            decimal sentAverageTraffic;

                            lock (_sentInfomation.LockObject)
                            {
                                sentAverageTraffic = _sentInfomation.AverageTraffics.Sum() / _sentInfomation.AverageTraffics.Length;
                            }

                            decimal receivedAverageTraffic;

                            lock (_receivedInfomation.LockObject)
                            {
                                receivedAverageTraffic = _receivedInfomation.AverageTraffics.Sum() / _receivedInfomation.AverageTraffics.Length;
                            }

                            this.SendingSpeed.Value = sentAverageTraffic;
                            this.ReceivingSpeed.Value = receivedAverageTraffic;

                            this.ServiceState.Value = state;
                        }
                        catch (Exception)
                        {

                        }
                    }));
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private void TrafficMonitorThread(CancellationToken token)
        {
            try
            {
                var sw = new Stopwatch();
                sw.Start();

                while (!token.IsCancellationRequested)
                {
                    Thread.Sleep(((int)Math.Max(2, 1000 - sw.ElapsedMilliseconds)) / 2);
                    if (sw.ElapsedMilliseconds < 1000) continue;

                    long receivedByteCount = _serviceManager.Information.GetValue<long>("Network_ReceivedByteCount");
                    long sentByteCount = _serviceManager.Information.GetValue<long>("Network_SentByteCount");

                    lock (_sentInfomation.LockObject)
                    {
                        _sentInfomation.AverageTraffics[_sentInfomation.Round++]
                            = ((decimal)(sentByteCount - _sentInfomation.PreviousTraffic)) * 1000 / sw.ElapsedMilliseconds;
                        _sentInfomation.PreviousTraffic = sentByteCount;

                        if (_sentInfomation.Round >= _sentInfomation.AverageTraffics.Length)
                        {
                            _sentInfomation.Round = 0;
                        }
                    }

                    lock (_receivedInfomation.LockObject)
                    {
                        _receivedInfomation.AverageTraffics[_receivedInfomation.Round++]
                            = ((decimal)(receivedByteCount - _receivedInfomation.PreviousTraffic)) * 1000 / sw.ElapsedMilliseconds;
                        _receivedInfomation.PreviousTraffic = receivedByteCount;

                        if (_receivedInfomation.Round >= _receivedInfomation.AverageTraffics.Length)
                        {
                            _receivedInfomation.Round = 0;
                        }
                    }

                    sw.Restart();
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private void StateCopy()
        {
            var sb = new StringBuilder();

            foreach (var (key, value) in this.StateSelectedItems.Cast<KeyValuePair<string, DynamicOptions>>())
            {
                sb.AppendLine($"{key}: {value.GetValue<string>("Content")}");
            }

            Clipboard.SetText(sb.ToString());
        }

        private void LogCopy()
        {
            var sb = new StringBuilder();

            foreach (string line in this.LogSelectedItems.Cast<string>())
            {
                sb.AppendLine(line);
            }

            Clipboard.SetText(sb.ToString());
        }

        private void Save()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                _settings.Save("Version", 0);
                _settings.Save("ConnectionSortInfo", _connectionSortInfo);
                _settings.Save(nameof(DynamicOptions), this.DynamicOptions.GetProperties(), true);
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;

            if (disposing)
            {
                _watchTaskManager.Stop();
                _watchTaskManager.Dispose();

                _statusBarTaskManager.Stop();
                _statusBarTaskManager.Dispose();

                _trafficMonitorTaskManager.Stop();
                _trafficMonitorTaskManager.Dispose();

                this.Save();

                _disposable.Dispose();
            }
        }
    }
}