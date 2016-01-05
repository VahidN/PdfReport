using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using DemosBrowser.Toolkit.Mvvm;
using PdfRpt.Core.Contracts;

namespace DemosBrowser.Models
{
    public class BrowserGui : ViewModelBase
    {
        IList<Type> _samplesList;
        public IList<Type> SamplesList
        {
            get { return _samplesList; }
            set
            {
                _samplesList = value;
                RaisePropertyChanged("SamplesList");
            }
        }

        TestResultItems _integrationTests;
        public TestResultItems IntegrationTests
        {
            get { return _integrationTests; }
            set
            {
                _integrationTests = value;
                RaisePropertyChanged("IntegrationTests");
            }
        }

        TestResultItem _selectedSampleTestFile;
        public TestResultItem SelectedSampleTestFile
        {
            get { return _selectedSampleTestFile; }
            set
            {
                _selectedSampleTestFile = value;
                RaisePropertyChanged("SelectedSampleTestFile");
            }
        }

        Type _selectedSample;
        public Type SelectedSample
        {
            get { return _selectedSample; }
            set
            {
                if (_selectedSample == value) return;
                _selectedSample = value;
                RaisePropertyChanged("SelectedSample");
            }
        }

        string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (_searchText == value) return;
                _searchText = value;
                RaisePropertyChanged("SearchText");
            }
        }

        bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (_isBusy == value) return;
                _isBusy = value;
                RaisePropertyChanged("IsBusy");
            }
        }

        Control _acroPdf;
        public Control AcroPdf
        {
            get { return _acroPdf; }
            set
            {
                _acroPdf = value;
                RaisePropertyChanged("AcroPdf");
            }
        }


        Control _acroSamplePdf;
        public Control AcroSamplePdf
        {
            get { return _acroSamplePdf; }
            set
            {
                _acroSamplePdf = value;
                RaisePropertyChanged("AcroSamplePdf");
            }
        }

        Visibility _controlVisibility = Visibility.Hidden;
        public Visibility ControlVisibility
        {
            get { return _controlVisibility; }
            set
            {
                _controlVisibility = value;
                RaisePropertyChanged("ControlVisibility");
            }
        }

        Visibility _sampleControlVisibility = Visibility.Hidden;
        public Visibility SampleControlVisibility
        {
            get { return _sampleControlVisibility; }
            set
            {
                _sampleControlVisibility = value;
                RaisePropertyChanged("SampleControlVisibility");
            }
        }

        int _samplesCount;
        public int SamplesCount
        {
            get { return _samplesCount; }
            set
            {
                if (_samplesCount == value) return;
                _samplesCount = value;
                RaisePropertyChanged("SamplesCount");
            }
        }

        string _elapsedTime;
        public string ElapsedTime
        {
            get { return _elapsedTime; }
            set
            {
                if (_elapsedTime == value) return;
                _elapsedTime = value;
                RaisePropertyChanged("ElapsedTime");
            }
        }

        string _fileSize;
        public string FileSize
        {
            get { return _fileSize; }
            set
            {
                if (_fileSize == value) return;
                _fileSize = value;
                RaisePropertyChanged("FileSize");
            }
        }

        string _memoryUsage;
        public string MemoryUsage
        {
            get { return _memoryUsage; }
            set
            {
                if (_memoryUsage == value) return;
                _memoryUsage = value;
                RaisePropertyChanged("MemoryUsage");
            }
        }

        IList<string> _filesList = new List<string>();
        public IList<string> FilesList
        {
            get { return _filesList; }
            set
            {
                if (_filesList == value) return;
                _filesList = value;
                RaisePropertyChanged("FilesList");
            }
        }

        string _selectedFilePath;
        public string SelectedFilePath
        {
            get { return _selectedFilePath; }
            set
            {
                if (_selectedFilePath == value) return;
                _selectedFilePath = value;
                RaisePropertyChanged("SelectedFilePath");
            }
        }


        string _pdfFilePath;
        public string PdfFilePath
        {
            get { return _pdfFilePath; }
            set
            {
                if (_pdfFilePath == value) return;
                _pdfFilePath = value;
                RaisePropertyChanged("PdfFilePath");
            }
        }

        string _sourceCode;
        public string SourceCode
        {
            get { return _sourceCode; }
            set
            {
                if (_sourceCode == value) return;
                _sourceCode = value;
                RaisePropertyChanged("SourceCode");
            }
        }

        IPdfReportData _selectedIPdfReportData;
        public IPdfReportData SelectedIPdfReportData
        {
            get { return _selectedIPdfReportData; }
            set
            {
                _selectedIPdfReportData = value;
                RaisePropertyChanged("SelectedIPdfReportData");
            }
        }

        IPdfReportData _modifiedIPdfReportData;
        public IPdfReportData ModifiedIPdfReportData
        {
            get { return _modifiedIPdfReportData; }
            set
            {
                _modifiedIPdfReportData = value;
                RaisePropertyChanged("ModifiedIPdfReportData");
            }
        }
    }
}
