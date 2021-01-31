using System.Globalization;
using System.Resources;

namespace FiledResx.Resources
{
    /// <summary>
    /// 埋め込みリソースベースの文字列リソースを提供します。
    /// </summary>
    public abstract class EmbedStringResource : StringResourceBase
    {
        /// <summary>
        /// <see cref="ResourceManager"/> を保持します。
        /// </summary>
        private readonly ResourceManager resourceManager;

        /// <summary>
        /// <see cref="EmbedStringResource"/> クラスの新しいインスタンスを生成します。
        /// </summary>
        private EmbedStringResource()
        {
            // 引数付きコンストラクタを呼び出す想定なので、private とする。
        }

        /// <summary>
        /// <see cref="EmbedStringResource"/> クラスの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="baseName">The root name of the resource file without its extension but including any fullyqualified namespace name.</param>
        public EmbedStringResource(string baseName = null)
        {
            if (baseName == null)
            {
                baseName = GetType().FullName;
            }

            resourceManager = new ResourceManager(baseName, GetType().Assembly);
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
            return resourceManager.GetString(name, Culture);
        }

        #endregion
    }
}
