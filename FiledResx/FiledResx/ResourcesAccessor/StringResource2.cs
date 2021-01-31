namespace FiledResx.Resources
{
    public class StringResource2 : EmbedStringResource
    {
        /// <summary>
        /// <see cref="IStringResourceManager"/> のシングルトン インスタンスを保持します。
        /// </summary>
        private static IStringResourceManager stringResourceManager;

        /// <summary>
        /// <see cref="StringResource2"/> クラスの新しいインスタンスを生成します。
        /// </summary>
        /// <remarks>
        /// シングルトン デザイン パターンなので、private とする。
        /// </remarks>
        private StringResource2()
        {
        }

        /// <summary>
        /// <see cref="IStringResourceManager"/> のシングルトン インスタンスを取得します。
        /// </summary>
        public static IStringResourceManager ResourceManager
        {
            get
            {
                if (stringResourceManager is null)
                {
                    stringResourceManager = new StringResource2();
                }

                return stringResourceManager;
            }
        }
    }
}
