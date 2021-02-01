using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Runtime.CompilerServices;

// MEMO: やり方次第では resx ファイルの更新に追従もできるが、現段階ではそこまでしていない。

namespace FiledResx.Resources
{
    /// <summary>
    /// ファイルによる文字列リソースを提供します。
    /// </summary>
    public abstract class FileBasedStringResource : StringResourceBase
    {
        /// <summary>
        /// リソース ファイルへの既定の相対パスを表します。
        /// </summary>
        private const string DEFAULT_RELATIVE_PATH = "Resources";

        /// <summary>
        /// リソース ファイルのアクセサへの既定の相対パスを表します。
        /// </summary>
        private const string DEFAULT_ACCESSER_PATH = "ResourcesAccessor";

        /// <summary>
        /// オーバーライドする文字列リソースを保持します。
        /// </summary>
        private readonly Dictionary<string, Dictionary<string, string>> resource = new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// プロジェクトにおける、リソースファイルへの相対パスを保持します。
        /// </summary>
        private string relativePath;

        /// <summary>
        /// リソースファイルの名称を保持します。
        /// </summary>
        private string resxname;

        /// <summary>
        /// 初期化をします。
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

            // CheckResourceFile メソッドで利用するため、退避しておく。
            this.relativePath = GetPath(relativePath, callerFilePath);
            this.resxname = resxname;

            // 現在のカルチャについてはここで読み込んでおく。
            CheckResourceFile(CultureInfo.CurrentUICulture);
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

        /// <summary>
        /// カルチャに対応したリソース ファイルを登録します。
        /// </summary>
        /// <param name="culture">対象のカルチャ。</param>
        private void CheckResourceFile(CultureInfo culture)
        {
            // カルチャをさかのぼって辞書に登録されているか確認する。
            while (true)
            {
                // ファイルは読み込み済あるいはチェック済か
                if (resource.ContainsKey(culture.ToString()) == false)
                {
                    // 読み込み未

                    // 対象の resx ファイル名を組み立て
                    string cultureKey = string.Empty;
                    if (culture != CultureInfo.InvariantCulture)
                    {
                        cultureKey = $"{culture}.";
                    }
                    string fileName = relativePath + $"{resxname}.{cultureKey}resx";

                    // 2 回目以降のチェック省略のため、ファイルがなくてもディクショナリは作る。
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    resource.Add(culture.ToString(), dictionary);

                    // ファイルが存在すれば、resx ファイルからキーと値を登録する。
                    if (File.Exists(fileName) == true)
                    {
                        using (var reader = new ResXResourceReader(fileName))
                        {
                            foreach (DictionaryEntry entry in reader)
                            {
                                dictionary.Add(entry.Key.ToString(), entry.Value.ToString());
                            }
                        }
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
        }

        /// <summary>
        /// 現在の UI カルチャの指定した文字列リソースを返します。
        /// </summary>
        /// <param name="name">取得するリソースの名前。</param>
        /// <param name="culture">An object that represents the culture for which the resource is localized.</param>
        /// <returns>呼び出し元の現在の UI カルチャのためにローカライズされたリソースの値、または、リソース セットから値が見つからない場合は <c>null</c>。</returns>
        protected override string GetStringImpl(string name, CultureInfo culture)
        {
            // resx ファイル読み込みチェック。
            CheckResourceFile(culture);

            // リソース文字列を探す。
            string value = FindResource(name, culture, resource);

            // デザインモードでない場合は、リソースが見つからなかった場合にログを残す。
            if ((value == null) && (isInDesignMode == false))
            {
                Debug.WriteLine($"index not found: {name}");
            }

            return value;
        }
    }
}
