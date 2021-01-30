using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Windows;

// MEMO: 外部からのリソースの個別書き換え、オーバーライドは現段階で未実装
// MEMO: カルチャの対応は現段階で未実装

namespace FiledResx.Resources
{
    /// <summary>
    /// ファイルベースの文字列リソースの基底機能を提供します。
    /// </summary>
    public abstract class FileBasedStringResource : INotifyPropertyChanged, IStringResourceManager
    {
        /// <summary>
        /// リソース ファイルへの相対パスを表します。
        /// </summary>
        private const string DEFAULT_RELATIVE_PATH = "Resources";

        /// <summary>
        /// プロパティ変更イベントのイベント  ハンドラーを保持します。
        /// </summary>
        /// <remarks>
        /// WPF のバインディング機構に対するメモリ リーク対策として定義されています。
        /// </remarks>
#pragma warning disable 0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067

        /// <summary>
        /// オーバーライドする <see cref="CultureInfo"/> を保持します。
        /// </summary>
        protected CultureInfo resourceCulture;

        /// <summary>
        /// リソース キーとリソース文字列のディクショナリを保持します。
        /// </summary>
        protected readonly Dictionary<string,string> resources;

        /// <summary>
        /// デザインモードかどうかを保持します。
        /// </summary>
        protected static readonly bool isInDesignMode = (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;

        /// <summary>
        /// <see cref="FileBasedStringResource"/> クラスの新しいインスタンスを生成します。
        /// </summary>
        private FileBasedStringResource()
        {
            // 引数付きコンストラクタを呼び出す想定なので、private とする。
        }

        /// <summary>
        /// <see cref="FileBasedStringResource"/> クラスの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="relativePath">プロジェクトにおける、リソースファイルへの相対パス。</param>
        /// <param name="resxname">リソースファイルの名称。</param>
        public FileBasedStringResource(string relativePath = null, string resxname = null)
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

            string fileName = GetPath(relativePath) + $"{resxname}.resx";

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

        /// <summary>
        /// 読み出すリソース ファイルへのパスを返します。
        /// </summary>
        /// <param name="relativePath">プロジェクトにおける、リソースファイルへの相対パス。</param>
        /// <param name="callerFilePath">呼び出し元のソースファイルのパス。デザインモードでの動作のために利用します。</param>
        /// <returns>読み出すリソース ファイルへのパス。</returns>
        protected string GetPath(string relativePath, [CallerFilePath] string callerFilePath = null)
        {
            if (isInDesignMode == true)
            {
                // Design mode
                return Path.GetDirectoryName(callerFilePath) + @"\";
            }
            else
            {
                return @$"{relativePath}\";
            }
        }

        #region IStringResourceManager

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
        /// <returns>呼び出し元の現在の UI カルチャのためにローカライズされたリソースの値、または、リソース セットから値が見つからない場合は <c>null</c>。</returns>
        public virtual string GetString(string name)
        {
            if (resources.TryGetValue(name, out string value) == true)
            {
                return value;
            }

            if (isInDesignMode == false)
            {
                Debug.WriteLine($"index not found: {name}");
            }

            return null;
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
                resourceCulture = value;
            }
        }

        #endregion
    }
}
