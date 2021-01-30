namespace FiledResx.Resources
{
    /// <summary>
    /// このアセンブリの文字列リソースを提供します。
    /// </summary>
    public class StringResource : FileBasedStringResourceBase
    {
        /// <summary>
        /// <see cref="IStringResourceManager"/> のシングルトン インスタンスを保持します。
        /// </summary>
        private static IStringResourceManager resourceManager;

        /// <summary>
        /// <see cref="StringResource"/> クラスの新しいインスタンスを生成します。
        /// </summary>
        private StringResource()
        {
            // シングルトン デザイン パターンなので、private とする。
        }

        /// <summary>
        /// <see cref="IStringResourceManager"/> のシングルトン インスタンスを取得します。
        /// </summary>
        public static IStringResourceManager ResourceManager
        {
            get
            {
                if (resourceManager is null)
                {
                    resourceManager = new StringResource();
                }

                return resourceManager;
            }
        }
    }
}
