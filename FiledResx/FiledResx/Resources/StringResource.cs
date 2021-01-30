namespace FiledResx.Resources
{
    public class StringResource : StringResourceBase
    {
        private StringResource()
        {
        }

        /// <summary>
        /// このクラスで使用されているキャッシュされた <see cref="StringResource"/> インスタンスを返します。
        /// </summary>
        public static StringResource ResourceManager
        {
            get
            {
                if (resourceManager is null)
                {
                    resourceManager = new StringResource();
                }
                return (StringResource)resourceManager;
            }
        }
    }
}
