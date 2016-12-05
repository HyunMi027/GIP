using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using MahApps.Metro.Controls;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SerialRecorder
{
    public partial class MainWindow : MetroWindow, INotifyPropertyChanged
    {
        public List<int> BaudRates => new List<int>() { 110, 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 28800, 38400, 56000, 57600, 115200 };

        public List<string> SerialPortNames => SerialPort.GetPortNames().ToList();


        private SerialPort _serialPort;

        public bool CanConnect => !string.IsNullOrWhiteSpace(this.SelectedSerialPort) && this._serialPort == null && !this.IsConnected;

        public bool IsConnected => this._serialPort != null && this._serialPort.IsOpen;

        private string _selectedSerialPort = SerialPort.GetPortNames().FirstOrDefault();

        public ConcurrentStack<string> Log = new ConcurrentStack<string>();
        
        public Stopwatch Timer = new Stopwatch();

        public string DisplayTime => this.Timer.Elapsed.ToString("mm\\:ss\\.fff");

        private Task LogTask;

        private int _drops = 0;

        public int Drops
        {
            get { return this._drops; }
            set { this._drops = value; }
        }

        public string DisplayLogText
        {
            get
            {
                if (this.Log.IsEmpty)
                {
                    return string.Empty;
                }
                StringBuilder builder = new StringBuilder();
                foreach (string line in this.Log.Reverse().Skip(Math.Max(0, this.Log.Count - 20)))
                {
                    builder.AppendLine(line);
                }
                return builder.ToString();
            }
        }

        public string SelectedSerialPort
        {
            get { return this._selectedSerialPort; }
            set
            {
                this._selectedSerialPort = value;
                this.OnPropertyChanged(nameof(this.SelectedSerialPort));
            }
        }

        private int _selectedBaudrate;

        public int SelectedBaudrate
        {
            get { return this._selectedBaudrate; }
            set
            {
                this._selectedBaudrate = value;
                this.OnPropertyChanged(nameof(this.SelectedBaudrate));
            }
        }

        private void Connect_OnClick(object sender, RoutedEventArgs e)
        {
            this.Timer.Start();
            this._serialPort = new SerialPort(this.SelectedSerialPort, this._selectedBaudrate);
            this._serialPort.Open();
            this.LogTask = Task.Factory.StartNew(delegate
            {
                StreamWriter writer = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DateTime.Now.ToFileTime() + ".txt"));
                while (this.IsConnected)
                {
                    TimeSpan time = this.Timer.Elapsed;
                    string line = string.Empty;
                    try
                    {
                        line = this._serialPort.ReadLine();
                    }
                    catch (Exception)
                    {
                        break;
                    }
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        break;
                    }
                    string text = "Time: " + time.TotalSeconds.ToString("##.###").PadRight(26) + "Drops: " + this.Drops.ToString().PadRight(26) + "Value: " + line.Remove(line.Length - 1, 1);
                    Debug.WriteLine(text);
                    writer.WriteLine(text);
                    writer.Flush();
                    this.Log.Push(text);
                    this.OnPropertyChanged(nameof(this.Log));
                    this.OnPropertyChanged(nameof(this.DisplayLogText));
                }
                writer.Close();
                writer.Dispose();
            });
            this.OnPropertyChanged(nameof(this.IsConnected));
            this.OnPropertyChanged(nameof(this.CanConnect));
        }

        private void Disconnect_OnClick(object sender, RoutedEventArgs e)
        {
            this.Timer.Stop();
            this.Timer.Reset();
            this._serialPort.Close();
            this._serialPort.Dispose();
            this._serialPort = null;
            this.OnPropertyChanged(nameof(this.IsConnected));
            this.OnPropertyChanged(nameof(this.CanConnect));
            this.Log = new ConcurrentStack<string>();
        }

        private void IncrementDrops_OnClick(object sender, RoutedEventArgs e)
        {
            this.Drops++;
            this.OnPropertyChanged(nameof(this.Drops));
        }

        public MainWindow()
        {
            this.InitializeComponent();

            this.Loaded += delegate(object sender, RoutedEventArgs args)
            {
                Task.Factory.StartNew(delegate
                {
                    while (true)
                    {
                        this.OnPropertyChanged(nameof(this.DisplayTime));
                        Thread.Sleep(100);
                    }
                });
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}