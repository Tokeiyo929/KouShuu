
//using ICSharpCode.SharpZipLib.Zip;

namespace QFramework
{
    public class ZipUtility
    {
        public static void ZipFolder(string folderPathToZip, string outputZipFileName)
        {
            // 注释掉以下代码，因为缺少 ICSharpCode.SharpZipLib 依赖库
            // var fastZip = new FastZip();
            // fastZip.CreateZip(outputZipFileName,
            //     folderPathToZip, true, string.Empty);
        }
    }
}
