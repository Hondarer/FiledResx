using System.ComponentModel;
using System.Globalization;
using System.Windows;

namespace FiledResx.Resources
{
    /// <summary>
    /// 文字列リソースの基底機能を提供します。
    /// </summary>
    public abstract class StringResourceBase : IStringResourceManager
    {
        /// <summary>
        /// デザインモードかどうかを保持します。
        /// </summary>
        protected static readonly bool isInDesignMode = (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;

        /// <summary>
        /// オーバーライドする <see cref="CultureInfo"/> を保持します。
        /// </summary>
        protected CultureInfo resourceCulture;

        #region IStringResourceManager

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
        public abstract string GetString(string name, CultureInfo culture = null);

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
                // TODO: Binding.IndexerName を利用してインデクサーの更新を伝える
                resourceCulture = value;
            }
        }

        #endregion
    }
}
