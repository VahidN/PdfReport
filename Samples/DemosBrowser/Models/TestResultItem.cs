using DemosBrowser.Toolkit.Mvvm;

namespace DemosBrowser.Models
{
    public class TestResultItem : ViewModelBase
    {
        string _testName;
        public string TestName
        {
            get { return _testName; }
            set
            {
                if (_testName == value) return;
                _testName = value;
                RaisePropertyChanged("TestName");
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
    }
}
