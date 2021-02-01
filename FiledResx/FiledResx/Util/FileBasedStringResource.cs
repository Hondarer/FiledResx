using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Runtime.CompilerServices;

// MEMO: 外部からのリソースの個別書き換え、オーバーライドは現段階で未実装
// MEMO: カルチャの対応は現段階で未実装

namespace FiledResx.Resources
{
    /// <summary>
    /// ファイルによる文字列リソースを提供します。
    /// </summary>
    public abstract class FileBasedStringResource : StringResourceBase
    {
        /// <summary>
        /// リソース ファイルへの相対パスを表します。
        /// </summary>
        private const string DEFAULT_RELATIVE_PATH = "Resources";

        /// <summary>
        /// リソース ファイルのアクセサへの相対パスを表します。
        /// </summary>
        private const string DEFAULT_ACCESSER_PATH = "ResourcesAccessor";

        /// <summary>
        /// リソース キーとリソース文字列のディクショナリを保持します。
        /// </summary>
        protected readonly Dictionary<string,string> resources;

        /// <summary>
        /// <see cref="FileBasedStringResource"/> クラスの新しいインスタンスを生成します。
        /// </summary>
        public FileBasedStringResource()
        {
            resources = new Dictionary<string, string>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relativePath">プロジェクトにおける、リソースファイルへの相対パス。</param>
        /// <param name="resxname">リソースファイルの名称。</param>
        /// <param name="callerFilePath">呼び出し元のソースファイルのパス。デザインモードでの動作のために利用します。</param>
        protected void Init(string relativePath = null, string resxname = null, [CallerFilePath] string callerFilePath = null)
        {
            if (relativePath is null)
            {
                relativePath = DEFAULT_RELATIVE_PATH;
            }

            if (resxname is null)
            {
                resxname = GetType().FullName.Replace(DEFAULT_ACCESSER_PATH, DEFAULT_RELATIVE_PATH);
            }

            string fileName = GetPath(relativePath, callerFilePath) + $"{resxname}.resx";

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
        protected string GetPath(string relativePath, string callerFilePath)
        {
            if (isInDesignMode == true)
            {
                // Design mode
                string accessorPath = Path.GetDirectoryName(callerFilePath) + @"\";
                return accessorPath.Replace(DEFAULT_ACCESSER_PATH, DEFAULT_RELATIVE_PATH);
            }
            else
            {
                return @$"{relativePath}\";
            }
        }

        #region IStringResourceManager

        /// <summary>
        /// 現在の UI カルチャの指定した文字列リソースを返します。
        /// </summary>
        /// <param name="name">取得するリソースの名前。</param>
        /// <param name="culture">An object that represents the culture for which the resource is localized.</param>
        /// <returns>呼び出し元の現在の UI カルチャのためにローカライズされたリソースの値、または、リソース セットから値が見つからない場合は <c>null</c>。</returns>
        public override string GetString(string name, CultureInfo culture = null)
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

        #endregion
    }
}
