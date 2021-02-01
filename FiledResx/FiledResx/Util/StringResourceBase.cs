using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;

namespace FiledResx.Resources
{
    /// <summary>
    /// 文字列リソースの基底機能を提供します。
    /// </summary>
    public abstract class StringResourceBase : IStringResourceManager
    {
        /// <summary>
        /// デザイン モードかどうかを保持します。
        /// </summary>
        protected static readonly bool isInDesignMode = (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;

        /// <summary>
        /// オーバーライドする <see cref="CultureInfo"/> を保持します。
        /// </summary>
        private CultureInfo resourceCulture;

        /// <summary>
        /// オーバーライドする文字列リソースを保持します。
        /// </summary>
        private readonly Dictionary<string, Dictionary<string, string>> overrideResource = new Dictionary<string, Dictionary<string, string>>();

        #region IStringResourceManager

        #region INotifyPropertyChanged

        /// <summary>
        /// プロパティ変更イベントのイベント ハンドラーを保持します。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// プロパティが既に目的の値と一致しているかどうかを確認します。
        /// 必要な場合のみ、プロパティを設定し、リスナーに通知します。
        /// </summary>
        /// <typeparam name="T">プロパティの型。</typeparam>
        /// <param name="storage">get アクセス操作子と set アクセス操作子両方を使用したプロパティへの参照。</param>
        /// <param name="value">プロパティに必要な値。</param>
        /// <param name="propertyName">
        /// リスナーに通知するために使用するプロパティの名前。
        /// この値は省略可能で、<see cref="CallerMemberNameAttribute"/> をサポートするコンパイラから呼び出す場合に自動的に指定できます。
        /// </param>
        /// <returns>
        /// 値が変更された場合は <c>true</c>、既存の値が目的の値に一致した場合は <c>false</c> を返します。
        /// </returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// プロパティ値が変更されたことをリスナーに通知します。
        /// </summary>
        /// <param name="propertyName">リスナーに通知するために使用するプロパティの名前。
        /// この値は省略可能で、<see cref="CallerMemberNameAttribute"/> をサポートするコンパイラから呼び出す場合に自動的に指定できます。
        /// </param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        /// <summary>
        /// オーバーライドする文字列を登録または更新します。
        /// </summary>
        /// <param name="name">登録または更新するリソースの名前。</param>
        /// <param name="value">登録または更新するリソースの値。</param>
        /// <param name="culture">カルチャ。</param>
        public void RegistString(string name, string value, CultureInfo culture = null)
        {
            string cultureKey;
            if (culture != null)
            {
                // 外部からカルチャが与えられていればその値をカルチャキーとする。
                cultureKey = culture.ToString();
            }
            else
            {
                // カルチャは未指定。
                cultureKey = string.Empty;
            }

            // 指定されたカルチャはオーバーライド対象として登録済みか
            if (overrideResource.TryGetValue(cultureKey, out Dictionary<string, string> dictionary) == true)
            {
                // 指定されたキーは登録済みか
                if (dictionary.ContainsKey(name) == true)
                {
                    // 更新
                    dictionary[name] = value;
                }
                else
                {
                    // 登録
                    dictionary.Add(name, value);
                }
            }
            else
            {
                // カルチャを登録し、値も登録
                overrideResource.Add(cultureKey, new Dictionary<string, string> { { name, value } });
            }

            // インデクサーが更新されたことを通知する。
            OnPropertyChanged(Binding.IndexerName);
        }

        /// <summary>
        /// 現在の UI カルチャの指定した文字列リソースを取得します。
        /// </summary>
        /// <param name="name">取得するリソースの名前。</param>
        public virtual string this[string name]
        {
            get
            {
                return GetString(name);
            }
        }

        /// <summary>
        /// 現在の UI カルチャの指定した文字列リソースを返します。
        /// </summary>
        /// <param name="name">取得するリソースの名前。</param>
        /// <param name="culture">An object that represents the culture for which the resource is localized.</param>
        /// <returns>呼び出し元の現在の UI カルチャのためにローカライズされたリソースの値、または、リソース セットから値が見つからない場合は <c>null</c>。</returns>
        public string GetString(string name, CultureInfo culture = null)
        {
            // 外部からカルチャが指定されているかどうかを判定する。
            if (culture == null)
            {
                // 指定されてない。
                if (resourceCulture != null)
                {
                    // このクラスにカルチャが指定されている。
                    culture = resourceCulture;
                }
                else
                {
                    // UI カルチャを使用。
                    culture = CultureInfo.CurrentUICulture;
                }
            }

            // オーバーライド定義からリソース文字列を探す。
            string value = FindResource(name, culture, overrideResource);

            // オーバーライド定義からは必要なリソースが見つからなかった。
            // 継承されたクラスでリソース文字列を探す。
            if (value == null)
            {
                value = GetStringImpl(name, culture);
            }

            return value;
        }

        /// <summary>
        /// オーバーライドする <see cref="CultureInfo"/> を取得または設定します。
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
                SetProperty(ref resourceCulture, value);
            }
        }

        #endregion

        /// <summary>
        /// リソース ディクショナリから値を返します。
        /// </summary>
        /// <param name="name">取得するリソースの名前。</param>
        /// <param name="culture">An object that represents the culture for which the resource is localized.</param>
        /// <param name="resourceDictionary">検索対象のリソース ディクショナリ。</param>
        /// <returns>呼び出し元の現在の UI カルチャのためにローカライズされたリソースの値、または、リソース セットから値が見つからない場合は <c>null</c>。</returns>
        protected string FindResource(string name, CultureInfo culture, Dictionary<string, Dictionary<string, string>> resourceDictionary)
        {
            // カルチャをさかのぼって辞書に登録されているか確認する。
            while (true)
            {
                if (resourceDictionary.TryGetValue(culture.ToString(), out Dictionary<string, string> dictionary) == true)
                {
                    // キーに対応する値はあるか
                    if (dictionary.TryGetValue(name, out string value) == true)
                    {
                        // あれば、値を返す。
                        return value;
                    }
                }

                // CultureInfo.InvariantCulture のチェックが完了したらループ終了。
                if (culture == CultureInfo.InvariantCulture)
                {
                    break;
                }

                // ひとつ親のカルチャを参照する。
                // 最終的に CultureInfo.InvariantCulture が root。
                culture = culture.Parent;
            }

            // 見つからなかった。
            return null;
        }

        /// <summary>
        /// 現在の UI カルチャの指定した文字列リソースを返します。
        /// </summary>
        /// <param name="name">取得するリソースの名前。</param>
        /// <param name="culture">An object that represents the culture for which the resource is localized.</param>
        /// <returns>呼び出し元の現在の UI カルチャのためにローカライズされたリソースの値、または、リソース セットから値が見つからない場合は <c>null</c>。</returns>
        protected abstract string GetStringImpl(string name, CultureInfo culture);
    }
}
