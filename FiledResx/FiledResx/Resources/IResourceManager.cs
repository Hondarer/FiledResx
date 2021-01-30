using System.ComponentModel;
using System.Globalization;

namespace FiledResx.Resources
{
    /// <summary>
    /// 文字列リソースの管理・参照インターフェースを提供します。
    /// </summary>
    public interface IStringResourceManager
    {
        /// <summary>
        /// 現在の UI カルチャの指定した文字列リソースを取得します。
        /// </summary>
        /// <param name="name">取得するリソースの名前。</param>
        string this[string index] { get; }

        /// <summary>
        /// 現在の UI カルチャの指定した文字列リソースを返します。
        /// </summary>
        /// <param name="name">取得するリソースの名前。</param>
        /// <returns>呼び出し元の現在の UI カルチャのためにローカライズされたリソースの値、または、リソース セットから値が見つからない場合は <c>null</c>。</returns>
        string GetString(string index);

        /// <summary>
        /// オーバーライドする <see cref="CultureInfo"/> を取得または設定します。
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        CultureInfo Culture { get; set; }
    }
}