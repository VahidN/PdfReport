using System.Windows.Forms;
using DemosBrowser.Toolkit.Mvvm;
using System.Windows;

namespace DemosBrowser.Models
{
    public class CodeGenGui : ViewModelBase
    {
        string _template;
        public string Template
        {
            get { return _template; }
            set
            {
                _template = value;
                RaisePropertyChanged("Template");
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
    }
}