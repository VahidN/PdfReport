using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using DemosBrowser.Core;
using DemosBrowser.Models;
using DemosBrowser.Toolkit.AcrobatReader;
using DemosBrowser.Toolkit.Helper;
using DemosBrowser.Toolkit.Mvvm;
using DemosBrowser.Toolkit.Threading;
using PdfRpt.Core.Contracts;

namespace DemosBrowser.ViewModels
{
    using System.Globalization;

    public class BrowserViewModel : ViewModelBase
    {
        #region Fields (3)

        AcroPdf _acroPdf;
        AcroPdf _acroSamplePdf;
        IList<Type> _samplesListInternal;

        #endregion Fields

        #region Constructors (1)

        public BrowserViewModel()
        {
            setupItemsData();
            setupAcrobatReader();
        }

        #endregion Constructors

        #region Properties (5)

        public BrowserGui BrowserGuiData { set; get; }

        public DelegateCommand<string> DoOpenInVs { set; get; }

        public DelegateCommand<string> DoStartTests { set; get; }

        public ICollectionView SamplesDataView { set; get; }

        private IList<Type> samplesListInternal
        {
            set
            {
                _samplesListInternal = value;
                if (SamplesDataView != null)
                {
                    SamplesDataView = CollectionViewSource.GetDefaultView(value);
                    RaisePropertyChanged("SamplesDataView");
                }
            }
            get { return _samplesListInternal; }
        }

        #endregion Properties

        #region Methods (19)

        // Private Methods (19) 

        void browserGuiDataPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SearchText":
                    doSearch(BrowserGuiData.SearchText);
                    break;
                case "SelectedSample":
                    doDisplaySampleType(BrowserGuiData.SelectedSample);
                    break;
                case "SelectedFilePath":
                    doShowSourceCode();
                    break;
                case "SelectedSampleTestFile":
                    doShowSelectedSample();
                    break;
                case "ModifiedIPdfReportData":
                    doShowModifiedIPdfReportData();
                    break;
            }
        }

        bool canDoStartTests(string data)
        {
            return true;
        }

        private void doDisplayDynamicSampleInstance(IPdfReportData sampleInstance)
        {
            if (sampleInstance == null) return;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    BrowserGuiData.IsBusy = true;
                    BrowserGuiData.ControlVisibility = Visibility.Hidden;
                    IPdfReportData rpt = null;
                    resetInfo();
                    var performanceResult = Performance.RunActionMeasurePerformance(() => rpt = SamplesList.GeneratePdf(sampleInstance));
                    showResult(rpt, performanceResult);
                    BrowserGuiData.IsBusy = false;
                    BrowserGuiData.ControlVisibility = Visibility.Visible;
                }
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //}
                finally
                {
                    BrowserGuiData.IsBusy = false;
                }
            });
        }

        private void doDisplaySampleType(Type sampleType)
        {
            if (sampleType == null) return;
            Task.Factory.StartNew(() =>
                {
                    try
                    {
                        BrowserGuiData.IsBusy = true;
                        BrowserGuiData.ControlVisibility = Visibility.Hidden;
                        IPdfReportData rpt = null;
                        resetInfo();
                        var performanceResult = Performance.RunActionMeasurePerformance(() => rpt = SamplesList.GeneratePdf(sampleType));
                        showResult(rpt, performanceResult);
                        BrowserGuiData.IsBusy = false;
                        BrowserGuiData.ControlVisibility = Visibility.Visible;
                    }
                    //catch (Exception ex)
                    //{
                    //    MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    //}
                    finally
                    {
                        BrowserGuiData.IsBusy = false;
                    }
                });
        }

        void doOpenInVs(string path)
        {
            new OpenStackTraceFile { Column = 1, Line = 1, FullFilename = new FileInfo(path).FullName }.ShowToUser();
        }

        private void doSearch(string data)
        {
            SamplesDataView.Filter = obj =>
            {
                if (obj == null) return false;
                var item = obj as Type;
                return (item != null) && !string.IsNullOrWhiteSpace(item.FullName) && item.FullName.ToLower().Contains(data);
            };
        }

        private void doShowModifiedIPdfReportData()
        {
            if (BrowserGuiData.ModifiedIPdfReportData == null) return;
            doDisplayDynamicSampleInstance(BrowserGuiData.ModifiedIPdfReportData);
        }

        private void doShowSelectedSample()
        {
            if (string.IsNullOrWhiteSpace(BrowserGuiData.SelectedSampleTestFile.PdfFilePath)) return;
            _acroSamplePdf.ShowPdf(BrowserGuiData.SelectedSampleTestFile.PdfFilePath);
        }

        private void doShowSourceCode()
        {
            if (string.IsNullOrWhiteSpace(BrowserGuiData.SelectedFilePath) ||
                !File.Exists(BrowserGuiData.SelectedFilePath)) return;
            BrowserGuiData.SourceCode = File.ReadAllText(BrowserGuiData.SelectedFilePath);
        }

        void doStartTests(string data)
        {
            BrowserGuiData.IntegrationTests.Clear();
            Task.Factory.StartNew(runTests);
        }

        private void resetInfo()
        {
            BrowserGuiData.ElapsedTime = string.Empty;
            BrowserGuiData.FileSize = string.Empty;
            BrowserGuiData.MemoryUsage = string.Empty;
            BrowserGuiData.FilesList = new List<string>();
            BrowserGuiData.SourceCode = string.Empty;
        }

        private void runSample(Type sample)
        {
            IPdfReportData rpt = null;
            var performanceResult = Performance.RunActionMeasurePerformance(() => rpt = SamplesList.GeneratePdf(sample));
            if (rpt == null)
                return;

            DispatcherHelper.DispatchAction(() =>
                BrowserGuiData.IntegrationTests.Add(
                    new TestResultItem
                    {
                        TestName = sample.ToString().Split('.')[2],
                        PdfFilePath = rpt.FileName,
                        ElapsedTime = performanceResult.Item2.ToString(),
                        FileSize = string.IsNullOrWhiteSpace(rpt.FileName) ? string.Empty : new FileInfo(rpt.FileName).Length.FormatSize(),
                        MemoryUsage = performanceResult.Item1.FormatSize().ToString(CultureInfo.InvariantCulture)
                    })
             );
        }

        private void runTests()
        {
            try
            {
                BrowserGuiData.IsBusy = true;
                BrowserGuiData.SampleControlVisibility = Visibility.Hidden;
                using (var parallelTasksQueue = new ParallelTasksQueue(Environment.ProcessorCount))
                {
                    var waitingList = new List<Task>();
                    for (var i = 0; i < samplesListInternal.Count; i++)
                    {
                        var sample = samplesListInternal[i];
                        Action action = () => runSample(sample);
                        var task = Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, parallelTasksQueue);
                        waitingList.Add(task);
                    }
                    Task.WaitAll(waitingList.ToArray());
                }
            }
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
            finally
            {
                BrowserGuiData.IsBusy = false;
                BrowserGuiData.SampleControlVisibility = Visibility.Visible;
            }
        }

        private void setupAcrobatReader()
        {
            _acroPdf = new AcroPdf(string.Empty);
            BrowserGuiData.AcroPdf = _acroPdf;

            _acroSamplePdf = new AcroPdf(string.Empty);
            BrowserGuiData.AcroSamplePdf = _acroSamplePdf;
        }

        private void setupItemsData()
        {
            DoStartTests = new DelegateCommand<string>(doStartTests, canDoStartTests);
            DoOpenInVs = new DelegateCommand<string>(doOpenInVs);
            BrowserGuiData = new BrowserGui { IntegrationTests = new TestResultItems() };
            BrowserGuiData.PropertyChanged += this.browserGuiDataPropertyChanged;
            samplesListInternal = SamplesList.LoadSamplesList();
            BrowserGuiData.SamplesCount = samplesListInternal.Count;
            SamplesDataView = CollectionViewSource.GetDefaultView(samplesListInternal);
            SamplesList.WarmupTheSystem(samplesListInternal);
        }

        private void showFilesList()
        {
            BrowserGuiData.FilesList = SamplesList.SamplesPath(BrowserGuiData.SelectedSample);
        }

        private void showPdf(IPdfReportData rpt)
        {
            if (rpt == null) return;
            DispatcherHelper.DispatchAction(() => _acroPdf.ShowPdf(rpt.FileName));
            Task.Factory.StartNew(() => BrowserGuiData.PdfFilePath = rpt.FileName);
        }

        private void showPerformanceResult(IPdfReportData rpt, Tuple<long, TimeSpan> performanceResult)
        {
            BrowserGuiData.ElapsedTime = performanceResult.Item2.ToString();
            BrowserGuiData.MemoryUsage = performanceResult.Item1.FormatSize();
            if (string.IsNullOrWhiteSpace(rpt.FileName)) return;
            BrowserGuiData.FileSize = new FileInfo(rpt.FileName).Length.FormatSize();
        }

        private void showResult(IPdfReportData rpt, Tuple<long, TimeSpan> performanceResult)
        {
            if (rpt == null)
                return;

            showPerformanceResult(rpt, performanceResult);
            BrowserGuiData.SelectedIPdfReportData = rpt;
            showPdf(rpt);
            showFilesList();
        }

        #endregion Methods
    }
}