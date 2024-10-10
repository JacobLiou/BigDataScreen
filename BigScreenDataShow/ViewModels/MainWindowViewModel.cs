using BigScreenDataShow.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.Painting;
using Newtonsoft.Json;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace BigScreenDataShow.ViewModels
{
    public partial class MainWindowViewModel : ObservableRecipient
    {
        private readonly SynchronizationContext _synContext = new DispatcherSynchronizationContext(App.Current.Dispatcher);

        private readonly object _objLock = new();

        private readonly DispatcherTimer _updaterealtimetimer;

        private readonly DispatcherTimer _timer;

        private readonly DispatcherTimer _agingtimer;

        private readonly DispatcherTimer _aginghourtimer;

        private readonly DispatcherTimer _agingdaytimer;

        private readonly HttpClient _httpClient;

        private readonly string _rootUrl = "https://localhost:7143";

        [ObservableProperty]
        private IEnumerable<ISeries> _voltwithstandSeries;

        [ObservableProperty]
        private IEnumerable<ISeries> _t1Series;

        [ObservableProperty]
        private IEnumerable<ISeries> _t2Series;

        [ObservableProperty]
        private IEnumerable<ISeries> _packageSeries;

        [ObservableProperty]
        private DailyPassData _voltwithstandDaily;

        [ObservableProperty]
        private DailyPassData _t1Daily;

        [ObservableProperty]
        private DailyPassData _t2Daily;

        [ObservableProperty]
        private DailyPassData _packageDaily;

        [ObservableProperty]
        private AgeDailyData _ageDailyData;

        [ObservableProperty]
        private RealTimeData _realTimeData;

        #region 老化分时数据图表元素
        /// <summary>
        /// 老化分时数据集合
        /// </summary>
        public ObservableCollection<ObservableValue> aging_hoursharing_data { get; set; }

        [ObservableProperty]
        private ISeries[] _aging_hoursharing_yieldSeries;

        [ObservableProperty]
        public Axis[] _aging_hoursharing_yieldXAxes;
        #endregion

        #region 老化日产量趋势图表元素
        /// <summary>
        /// 老化日产量趋势数据集合
        /// </summary>
        public ObservableCollection<ObservableValue> aging_daysharing_data { get; set; }
        /// <summary>
        /// 老化日产量趋势
        /// </summary>
        [ObservableProperty]
        private ISeries[] _aging_daysharing_yieldSeries;

        [ObservableProperty]
        public Axis[] _aging_daysharing_yieldXAxes;
        #endregion

        private Random random = new Random();

        public MainWindowViewModel()
        {
            InitAging_hoursharing_yield();
            InitAging_daysharing_yield();
            //四个站工位
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 10, 0);
            _timer.Tick += _timer_Tick;
            _timer.Start();

            //老化产量
            _agingtimer = new DispatcherTimer();
            _agingtimer.Interval = new TimeSpan(1, 0, 0);
            _agingtimer.Tick += _agingtimer_Tick;
            _agingtimer.Start();

            //老化分时产量
            _aginghourtimer = new DispatcherTimer();
            _aginghourtimer.Interval = new TimeSpan(0, 0, 2);
            _aginghourtimer.Tick += _aginghourtimer_Tick;
            _aginghourtimer.Start();

            //老化日产量趋势
            _agingdaytimer = new DispatcherTimer();
            _agingdaytimer.Interval = new TimeSpan(4, 0, 0);
            _agingdaytimer.Tick += _agingdaytimer_Tick;
            _agingdaytimer.Start();

            _updaterealtimetimer = new DispatcherTimer();
            _updaterealtimetimer.Interval = new TimeSpan(0, 0, 1);
            _updaterealtimetimer.Tick += _updaterealtimetimer_Tick;
            _updaterealtimetimer.Start();

            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(60);

            Task.Run(async () =>
            {
                await Task.Delay(1000);
                _updaterealtimetimer_Tick(null, null);
                _timer_Tick(null, null);
                _agingtimer_Tick(null, null);
                _aginghourtimer_Tick(null, null);
                _agingdaytimer_Tick(null, null);
            });
        }

        /// <summary>
        /// 初始化老化分时产量柱形图
        /// </summary>
        private void InitAging_hoursharing_yield()
        {
            aging_hoursharing_data = new ObservableCollection<ObservableValue>();
            for(int i = 0;i < 24;i++)
            {
                aging_hoursharing_data.Add(new ObservableValue(0));
            }
            
            Aging_hoursharing_yieldSeries = new ColumnSeries<ObservableValue>[]
           {
               new ColumnSeries<ObservableValue>
               {
                    Values = aging_hoursharing_data,
                    // Defines the distance between every bars in the series
                    Padding = 0,
                    // Defines the max width a bar can have
                    MaxBarWidth = double.PositiveInfinity,
                    //显示数据标签
                    DataLabelsPaint = new SolidColorPaint(SKColors.White),
                    DataLabelsSize = 20,
                    DataLabelsPosition = DataLabelsPosition.End
               }

           };

            Aging_hoursharing_yieldXAxes = new Axis[]
            {
                new Axis
                {
                    // Use the labels property to define named labels.
                    Labels = new string[] {
                        "0h", "1h", "2h",
                        "3h", "4h", "5h",
                        "6h", "7h", "8h",
                        "9h", "10h", "11h",
                        "12h", "13h", "14h",
                        "15h", "16h", "17h",
                        "18h", "19h", "20h",
                        "21h","22h", "23h"},
                    //LabelsRotation = -45,
                    LabelsPaint = new SolidColorPaint(SKColors.Yellow)
                },
            };
        }

        /// <summary>
        /// 初始化老化日产量趋势折线图
        /// </summary>
        private void InitAging_daysharing_yield()
        {
            aging_daysharing_data = new ObservableCollection<ObservableValue>();
            for (int i = 0; i < 31; i++)
            {
                aging_daysharing_data.Add(new ObservableValue(0));
            }
            Aging_daysharing_yieldSeries = new LineSeries<ObservableValue>[]
            {
               new LineSeries<ObservableValue>
               {
                    Values = aging_daysharing_data,
                    //显示数据标签
                    DataLabelsPaint = new SolidColorPaint(SKColors.White),
                    DataLabelsSize = 20,
                    DataLabelsPosition = DataLabelsPosition.End
               }

            };

            Aging_daysharing_yieldXAxes = new Axis[]
            {
                new Axis
                {
                    // Use the labels property to define named labels.
                    Labels = new string[] {
                        "1d", "2d", "3d",
                        "4d", "5d", "6d",
                        "7d", "8d", "9d",
                        "10d", "11d", "12d",
                        "13d", "14d", "15d",
                        "16d", "17d", "18d",
                        "19d", "20d", "21d",
                        "22d","23d", "24d",
                        "25d", "26d", "27d",
                        "28d", "29d", "30d",
                        "31d"},
                    //LabelsRotation = -45,
                    LabelsPaint = new SolidColorPaint(SKColors.Yellow)
                },
            };
        }

        private void _updaterealtimetimer_Tick(object sender, EventArgs e)
        {
            Task.Run(() => RefreshRealTimeData());
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            string startdate = DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss");
            string enddate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            Task.Run(() => RefreshVoltWithstandData(startdate, enddate));
            Task.Run(() => RefreshT1Data(startdate, enddate));
            Task.Run(() => RefreshT2Data(startdate, enddate));
            Task.Run(() => RefreshPackageData(startdate, enddate));
        }

        private void _agingtimer_Tick(object sender, EventArgs e)
        {
            string startdate = DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss");
            string enddate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            Task.Run(() => RefreshAgingData(startdate, enddate));
        }

        /// <summary>
        /// 老化分时数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _aginghourtimer_Tick(object sender, EventArgs e)
        {
            string startdate = DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss");
            string enddate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            Task.Run(() => RefreshAginghourData());
        }

        /// <summary>
        /// 老化日产量趋势数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _agingdaytimer_Tick(object sender, EventArgs e)
        {
            string startdate = DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss");
            string enddate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            Task.Run(() => RefreshAgingdayData());
        }

        private void RefreshAgingData(string startdate, string enddate)
        {
            AgeDailyData = new AgeDailyData();
            #region 模拟数据
            //AgeDailyData.G3 = random.Next(1, 100).ToString();
            //AgeDailyData.G4 = random.Next(1, 100).ToString();
            //AgeDailyData.EBI = random.Next(1, 100).ToString();

            #endregion

            #region 正式逻辑
            string agingtablecategory1 = "G3";
            string agingtablecategory2 = "G4";
            string agingtablecategory3 = "EBI";
            string url1 = $@"{_rootUrl}/GetAgingPassData?agingtablecategory={agingtablecategory1}&startdate={startdate}&enddate={enddate}";
            string url2 = $@"{_rootUrl}/GetAgingPassData?agingtablecategory={agingtablecategory2}&startdate={startdate}&enddate={enddate}";
            string url3 = $@"{_rootUrl}/GetAgingPassData?agingtablecategory={agingtablecategory3}&startdate={startdate}&enddate={enddate}";
            try
            {
                var response = _httpClient.GetAsync(url1).GetAwaiter().GetResult();
                if (response != null && response.IsSuccessStatusCode)
                {
                    AgeDailyData.G3 = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                }

                response = _httpClient.GetAsync(url2).GetAwaiter().GetResult();
                if (response != null && response.IsSuccessStatusCode)
                {
                    AgeDailyData.G4 = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                }

                response = _httpClient.GetAsync(url3).GetAwaiter().GetResult();
                if (response != null && response.IsSuccessStatusCode)
                {
                    AgeDailyData.EBI = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                }
            }
            catch
            {
            }
            #endregion
        }

        private void RefreshAginghourData()
        {
            #region 模拟数据
            //for (int i = 0; i < aging_hoursharing_data.Count();i++)
            //{
            //    var randomValue = random.Next(10, 50);
            //    var lastInstance = aging_hoursharing_data[i];
            //    lastInstance.Value = randomValue;
            //}
            #endregion
        }

        private void RefreshAgingdayData()
        {
            #region 模拟数据
            //for (int i = 0; i < aging_daysharing_data.Count(); i++)
            //{
            //    var randomValue = random.Next(10, 50);
            //    var lastInstance = aging_daysharing_data[i];
            //    lastInstance.Value = randomValue;
            //}
            #endregion
        }

        private void RefreshRealTimeData()
        {
            RealTimeData = new RealTimeData();
            var currentTime = DateTime.Now;
            RealTimeData.RealTime = currentTime.ToString("yyyy-MM-dd HH:mm:ss") + $"  {System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(currentTime.DayOfWeek)}";
     
        }

        private void RefreshVoltWithstandData(string startdate, string enddate)
        {
            string testcategory = "耐压";
            string url = $@"{_rootUrl}/GetPassData?testcategory={testcategory}&startdate={startdate}&enddate={enddate}";

            try
            {
                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
                if (response != null && response.IsSuccessStatusCode)
                {
                    var str = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    VoltwithstandDaily = JsonConvert.DeserializeObject<DailyPassData>(str);
                    Send(() => UpdateVoltWithstandData(VoltwithstandDaily));
                }
                else
                {
                    VoltwithstandDaily = new DailyPassData();
                    Send(() => UpdateVoltWithstandData(VoltwithstandDaily));
                }
            }
            catch
            {
                VoltwithstandDaily = new DailyPassData();
                Send(() => UpdateVoltWithstandData(VoltwithstandDaily));
            }
        }

        private void RefreshT1Data(string startdate, string enddate)
        {
            string testcategory = "T1";
            string url = $@"{_rootUrl}/GetPassData?testcategory={testcategory}&startdate={startdate}&enddate={enddate}";

            try
            {
                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
                if (response != null && response.IsSuccessStatusCode)
                {
                    var str = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    T1Daily = JsonConvert.DeserializeObject<DailyPassData>(str);
                    Send(() => UpdateT1Data(T1Daily));
                }
                else
                {
                    T1Daily = new DailyPassData();
                    Send(() => UpdateT1Data(T1Daily));
                }
            }
            catch
            {
                T1Daily = new DailyPassData();
                Send(() => UpdateT1Data(T1Daily));
            }
        }

        private void RefreshT2Data(string startdate, string enddate)
        {
            string testcategory = "T2";
            string url = $@"{_rootUrl}/GetPassData?testcategory={testcategory}&startdate={startdate}&enddate={enddate}";

            try
            {
                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
                if (response != null && response.IsSuccessStatusCode)
                {
                    var str = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    T2Daily = JsonConvert.DeserializeObject<DailyPassData>(str);
                    Send(() => UpdateT2Data(T2Daily));
                }
                else
                {
                    T2Daily = new DailyPassData();
                    Send(() => UpdateT2Data(T2Daily));
                }
            }
            catch
            {
                T2Daily = new DailyPassData();
                Send(() => UpdateT2Data(T2Daily));
            }
        }

        private void RefreshPackageData(string startdate, string enddate)
        {
            string testcategory = "包装";
            string url = $@"{_rootUrl}/GetPassData?testcategory={testcategory}&startdate={startdate}&enddate={enddate}";

            try
            {
                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
                if (response != null && response.IsSuccessStatusCode)
                {
                    var str = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    PackageDaily = JsonConvert.DeserializeObject<DailyPassData>(str);
                    Send(() => UpdatePackageData(PackageDaily));
                }
                else
                {
                    PackageDaily = new DailyPassData();
                    Send(() => UpdatePackageData(PackageDaily));
                }
            }
            catch
            {
                PackageDaily = new DailyPassData();
                Send(() => UpdatePackageData(PackageDaily));
            }
        }

        private void UpdateVoltWithstandData(DailyPassData dailyPassData)
        {
            VoltwithstandSeries = GaugeGenerator.BuildSolidGauge(new GaugeItem(double.Parse(dailyPassData.PassPercentage), series =>
            {
                series.MaxRadialColumnWidth = 50;
                series.DataLabelsSize = 50;
                series.Fill = new SolidColorPaint(SKColors.DodgerBlue);
            }));
        }

        private void UpdateT1Data(DailyPassData dailyPassData)
        {
            T1Series = GaugeGenerator.BuildSolidGauge(new GaugeItem(double.Parse(dailyPassData.PassPercentage), series =>
            {
                series.MaxRadialColumnWidth = 50;
                series.DataLabelsSize = 50;
                series.Fill = new SolidColorPaint(SKColors.DodgerBlue);
            }));
        }

        private void UpdateT2Data(DailyPassData dailyPassData)
        {
            T2Series = GaugeGenerator.BuildSolidGauge(new GaugeItem(double.Parse(dailyPassData.PassPercentage), series =>
            {
                series.MaxRadialColumnWidth = 50;
                series.DataLabelsSize = 50;
                series.Fill = new SolidColorPaint(SKColors.DodgerBlue);
            }));
        }

        private void UpdatePackageData(DailyPassData dailyPassData)
        {
            PackageSeries = GaugeGenerator.BuildSolidGauge(new GaugeItem(double.Parse(dailyPassData.PassPercentage), series =>
            {
                series.MaxRadialColumnWidth = 50;
                series.DataLabelsSize = 50;
                series.Fill = new SolidColorPaint(SKColors.DodgerBlue);
            }));
        }

        private void Send(Action action)
        {
            _synContext.Send(P =>
            {
                lock (_objLock)
                {
                    action();
                }
            }, null);
        }
    }
}