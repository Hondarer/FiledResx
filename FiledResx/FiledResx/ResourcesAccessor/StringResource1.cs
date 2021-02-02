namespace FiledResx.Resources
{
    /// <summary>
    /// このアセンブリのファイルによる文字列リソースを提供します。
    /// </summary>
    public class StringResource1 : FiledStringResource<StringResource1>
    {
        /// <summary>
        /// <see cref="StringResource1"/> クラスの新しいインスタンスを生成します。
        /// </summary>
        /// <remarks>
        /// シングルトン デザイン パターンなので、protected とする。
        /// CallerFilePath を利用するため、メソッドでの初期化が必要。
        /// </remarks>
        protected StringResource1()
        {
            Init();
        }
    }
}
