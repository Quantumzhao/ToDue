using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ToDue2.Resources;

namespace ToDue2
{
    public class ApplicationResources
    {
        public static ApplicationResources Current { get; private set; }

        public ApplicationResources()
        {
            Current = this;
            Labels = new Labels();
        }

        public Labels Labels { get; set; }

        private string _language;

        /// <summary>
        /// 获取或设置 Language 的值
        /// </summary>
        public string Language
        {
            get { return _language; }
            set
            {
                if (_language == value)
                    return;

                _language = value;
                var cultureInfo = new CultureInfo(value);
                Thread.CurrentThread.CurrentUICulture = cultureInfo;
                Thread.CurrentThread.CurrentCulture = cultureInfo;
                Labels.Culture = cultureInfo;

                RaiseProoertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void ChangeCulture(System.Globalization.CultureInfo cultureInfo)
        {
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Labels.Culture = cultureInfo;

            if (Current != null)
                Current.RaiseProoertyChanged();
        }

        public void RaiseProoertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }
    }
}
