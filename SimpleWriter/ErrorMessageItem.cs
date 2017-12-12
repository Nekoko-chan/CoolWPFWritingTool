using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ComplexWriter.global;

namespace ComplexWriter
{
    public class ErrorMessageItem :INotifyPropertyChanged
    {

        public const string DIVERSE = "Diverse";
        public const string SAVE = "Speicherung";
        public const string ERROR = "Fehler";
        public const string DICTIONARY = "Wörterbuch";
        public const string ZZZ = "ZZZ";

        private Exception _exception;
        private string _message;
        private string _stacktrace;
        private DateTime _occuranceTime;
        private bool _showsDetails;
        private bool _canShowDetails;
        private ErrorSeverity _severity;
        private bool _forUpdateTrigger;
        private string _groupName;
        private string _displayGroupName;

        public ErrorMessageItem(Exception exception, string header=null)
        {
            Exception = exception;
            Message = (header== null? string.Empty : header+"\n")+exception.Message;
            Stacktrace = exception.StackTrace;
            OccuranceTime = DateTime.Now;
            ShowsDetails = false;
            CanShowDetails = true;
            Severity = ErrorSeverity.Error;
            GroupName = ERROR;
            DisplayGroupName = ZZZ;
        }


        public ErrorMessageItem(string message, ErrorSeverity severity,string groupname)
        {
            Exception = null;
            Message = message;
            Stacktrace = string.Empty;
            OccuranceTime = DateTime.Now;
            ShowsDetails = false;
            Severity = severity;
            CanShowDetails = false;
            GroupName = groupname;
            DisplayGroupName = ZZZ;
        }


        public bool ForUpdateTrigger
        {
            get { return _forUpdateTrigger; }
            set { _forUpdateTrigger = value; OnPropertyChanged(); }
        }

        public ErrorSeverity Severity
        {
            get { return _severity; }
            set { _severity = value;OnPropertyChanged(); }
        }

        public bool CanShowDetails
        {
            get { return _canShowDetails; }
            set { _canShowDetails = value; OnPropertyChanged(); }
        }

        public bool ShowsDetails
        {
            get { return _showsDetails; }
            set { _showsDetails = value;OnPropertyChanged(); }
        }

        public DateTime OccuranceTime
        {
            get { return _occuranceTime; }
            private set { _occuranceTime = value; OnPropertyChanged(); }
        }

        public Exception Exception
        {
            get { return _exception; }
            private set
            {
                _exception = value;
                if (value == null)
                    return;
                 Message = value.Message;
                Stacktrace = value.StackTrace;
                GroupName = "Fehler";
            }
        }

        public string GroupName
        {
            get { return _groupName; }
            set { _groupName = value; OnPropertyChanged(); }
        }

        public string DisplayGroupName
        {
            get { return _displayGroupName; }
            set { _displayGroupName = value; OnPropertyChanged(); }
        }

        public string Message
        {
            get { return _message; }
            private set
            {
                _message = value;OnPropertyChanged();
            }
        }

        public string Stacktrace
        {
            get { return _stacktrace; }
            private set { _stacktrace = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
