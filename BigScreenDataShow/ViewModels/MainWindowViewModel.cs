using BigScreenDataShow.Common;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BigScreenDataShow.Models;
using LiveChartsCore.SkiaSharpView.Extensions;

namespace BigScreenDataShow.ViewModels
{
    public class MainWindowViewModel : PropertyChangedBase
    {
        public IEnumerable<ISeries> VoltwithstandSeries { get; set; }
        public IEnumerable<ISeries> T1Series { get; set; }
        public IEnumerable<ISeries> T2Series { get; set; }
        public IEnumerable<ISeries> PackageSeries { get; set; }

        Random random = new Random();

        public MainWindowViewModel()
        {
            Init();
        }
        /// <summary>
        /// 初始化数据
        /// </summary>
        private void Init()
        {
            InitVoltWithstandData();
            InitT1Data();
            InitT2Data();
            InitPackageData();
        }

        private void InitVoltWithstandData()
        {
            VoltwithstandSeries = GaugeGenerator.BuildSolidGauge(new GaugeItem(100, series =>
            {
                series.MaxRadialColumnWidth = 50;
                series.DataLabelsSize = 50;
            }));

        }

        private void InitT1Data()
        {
            T1Series = GaugeGenerator.BuildSolidGauge(new GaugeItem(90, series =>
            {
                series.MaxRadialColumnWidth = 50;
                series.DataLabelsSize = 50;
            }));

        }

        private void InitT2Data()
        {
            T2Series = GaugeGenerator.BuildSolidGauge(new GaugeItem(95, series =>
            {
                series.MaxRadialColumnWidth = 50;
                series.DataLabelsSize = 50;
            }));

        }

        private void InitPackageData()
        {
            PackageSeries = GaugeGenerator.BuildSolidGauge(new GaugeItem(100, series =>
            {
                series.MaxRadialColumnWidth = 50;
                series.DataLabelsSize = 50;
            }));

        }
    }
}
