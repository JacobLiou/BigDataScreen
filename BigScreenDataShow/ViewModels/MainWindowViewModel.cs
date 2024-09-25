using BigScreenDataShow.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        private readonly DispatcherTimer _timer;

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

        private Random random = new Random();

        public MainWindowViewModel()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 1, 30);
            _timer.Tick += _timer_Tick;

            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(60);

            Task.Run(async () =>
            {
                await Task.Delay(1000);
                _timer_Tick(null, null);
            });
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            string startdate = DateTime.Today.ToString("yyyy-MM-dd hh:mm:ss");
            string enddate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

            Task.Run(() => RefreshVoltWithstandData(startdate, enddate));
            Task.Run(() => RefreshT1Data(startdate, enddate));
            Task.Run(() => RefreshT2Data(startdate, enddate));
            Task.Run(() => RefreshPackageData(startdate, enddate));
        }

        private void RefreshVoltWithstandData(string startdate, string enddate)
        {
            string testcategory = "耐压";
            string url = $@"{_rootUrl}/GetPassData?testcategory={testcategory}&startdate{startdate}=&enddate={enddate}";

            var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
            if (response != null && response.IsSuccessStatusCode)
            {
                var str = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var voltWithstandData = JsonConvert.DeserializeObject<DailyPassData>(str);
                Send(() => UpdateVoltWithstandData(voltWithstandData));
            }
            else
            {
                Send(() => UpdateVoltWithstandData(new DailyPassData()));
            }
        }

        private void RefreshT1Data(string startdate, string enddate)
        {
            string testcategory = "耐压";
            string url = $@"{_rootUrl}/GetPassData?testcategory={testcategory}&startdate{startdate}=&enddate={enddate}";

            var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
            if (response != null && response.IsSuccessStatusCode)
            {
                var str = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var t1Data = JsonConvert.DeserializeObject<DailyPassData>(str);
                Send(() => UpdateT1Data(t1Data));
            }
            else
            {
                Send(() => UpdateT1Data(new DailyPassData()));
            }
        }

        private void RefreshT2Data(string startdate, string enddate)
        {
            string testcategory = "T2";
            string url = $@"{_rootUrl}/GetPassData?testcategory={testcategory}&startdate{startdate}=&enddate={enddate}";

            var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
            if (response != null && response.IsSuccessStatusCode)
            {
                var str = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var t2Data = JsonConvert.DeserializeObject<DailyPassData>(str);
                Send(() => UpdateT2Data(t2Data));
            }
            else
            {
                Send(() => UpdateT2Data(new DailyPassData()));
            }
        }

        private void RefreshPackageData(string startdate, string enddate)
        {
            string testcategory = "包装";
            string url = $@"{_rootUrl}/GetPassData?testcategory={testcategory}&startdate{startdate}=&enddate={enddate}";

            var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
            if (response != null && response.IsSuccessStatusCode)
            {
                var str = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var packageData = JsonConvert.DeserializeObject<DailyPassData>(str);
                Send(() => UpdatePackageData(packageData));
            }
            else
            {
                Send(() => UpdatePackageData(new DailyPassData()));
            }
        }

        private void UpdateVoltWithstandData(DailyPassData dailyPassData)
        {
            VoltwithstandSeries = GaugeGenerator.BuildSolidGauge(new GaugeItem(double.Parse(dailyPassData.PassPercentage), series =>
            {
                series.MaxRadialColumnWidth = 50;
                series.DataLabelsSize = 50;
            }));
        }

        private void UpdateT1Data(DailyPassData dailyPassData)
        {
            T1Series = GaugeGenerator.BuildSolidGauge(new GaugeItem(double.Parse(dailyPassData.PassPercentage), series =>
            {
                series.MaxRadialColumnWidth = 50;
                series.DataLabelsSize = 50;
            }));
        }

        private void UpdateT2Data(DailyPassData dailyPassData)
        {
            T2Series = GaugeGenerator.BuildSolidGauge(new GaugeItem(double.Parse(dailyPassData.PassPercentage), series =>
            {
                series.MaxRadialColumnWidth = 50;
                series.DataLabelsSize = 50;
            }));
        }

        private void UpdatePackageData(DailyPassData dailyPassData)
        {
            PackageSeries = GaugeGenerator.BuildSolidGauge(new GaugeItem(double.Parse(dailyPassData.PassPercentage), series =>
            {
                series.MaxRadialColumnWidth = 50;
                series.DataLabelsSize = 50;
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