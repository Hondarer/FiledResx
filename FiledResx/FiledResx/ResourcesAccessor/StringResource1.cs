namespace FiledResx.Resources
{
    /// <summary>
    /// このアセンブリの文字列リソースを提供します。
    /// </summary>
    public class StringResource1 : FileBasedStringResource
    {
        /// <summary>
        /// <see cref="IStringResourceManager"/> のシングルトン インスタンスを保持します。
        /// </summary>
        private static IStringResourceManager stringResourceManager;

        /// <summary>
        /// <see cref="StringResource1"/> クラスの新しいインスタンスを生成します。
        /// </summary>
        private StringResource1()
        {
            // 初期化する。(CallerFilePath を利用するため、メソッドでの初期化としている。)
            Init();
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
                    stringResourceManager = new StringResource1();
                }

                return stringResourceManager;
            }
        }
    }
}
