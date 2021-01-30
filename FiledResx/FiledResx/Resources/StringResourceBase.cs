using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Windows;

namespace FiledResx.Resources
{
    public abstract class StringResourceBase : INotifyPropertyChanged
    {
        private const string DEFAULT_RELATIVE_PATH = "Resources";

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// WPF のバインディング機構に対するメモリ リーク対策として定義されています。
        /// </remarks>
#pragma warning disable 0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067

        protected static StringResourceBase resourceManager;

        protected CultureInfo resourceCulture;

        protected readonly Dictionary<string,string> resources;

        protected readonly bool isInDesignMode = (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;

        protected string GetMyPath(string relativePath, [CallerFilePath] string from = null)
        {
            if (isInDesignMode == true)
            {
                // Design mode
                return Path.GetDirectoryName(from) + @"\";
            }
            else
            {
                return @$"{relativePath}\";
            }
        }

        private StringResourceBase()
        {
        }

        public StringResourceBase(string relativePath = null, string resxname = null)
        {
            resources = new Dictionary<string, string>();

            if (relativePath is null)
            {
                relativePath = DEFAULT_RELATIVE_PATH;
            }

            if (resxname is null)
            {
                resxname = GetType().FullName;
            }

            string fileName = GetMyPath(relativePath) + @$"{resxname}.resx";

            if (File.Exists(fileName) == true)
            {
                using (var reader = new ResXResourceReader(fileName))
                {
                    foreach (DictionaryEntry entry in reader)
                    {
                        resources.Add(entry.Key.ToString(), entry.Value.ToString());
                    }
                }
            }
        }

        public virtual string this[string index]
        {
            get
            {
                return GetString(index);
            }
        }

        public virtual string GetString(string index)
        {
            if (resources.TryGetValue(index, out string value) == true)
            {
                return value;
            }

            if (isInDesignMode == false)
            {
                Debug.WriteLine($"index not found: {index}");
            }

            return $"!{index}";
        }

        /// <summary>
        /// 現在のスレッドの CurrentUICulture プロパティをオーバーライドします。
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }
    }
}
