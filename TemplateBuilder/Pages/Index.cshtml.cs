using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO.Compression;
using System.Xml.Linq;

namespace TemplateBuilder.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        

        public void OnGet()
        {
            ViewData["DomainPath"] = AppDomain.CurrentDomain.BaseDirectory;
            ViewData["ContextPath"] = AppContext.BaseDirectory;



            
        }

        public async Task<IActionResult> OnGetTest()
        {
            //var dir =Directory.GetDirectories();
            // 通过递归获取下面的所有的文件路径
            var rootPath = "D:\\WorkFiles\\SourceCodes\\KiteTemplate\\src";
            var path = GetFiles(rootPath).Where(x => !(x.Contains("\\obj\\") || x.Contains("\\bin\\")));



             var zipStream = new MemoryStream();
             var zip = new ZipArchive(zipStream, ZipArchiveMode.Create);
            foreach (var item in path)
            {
                // 获取相对路径并对文件进行重命名
                var name = Path.GetRelativePath(rootPath, item).Replace("KiteTemplate", "Dest");

                ZipArchiveEntry entry = zip.CreateEntry(name);
                using (StreamWriter writer = new StreamWriter(entry.Open()))
                {
                    await writer.WriteAsync(System.IO.File.ReadAllText(item).Replace("KiteTemplate", "Dest"));
                }
            }

            // 替换内容写入压缩文件

            

            zipStream.Position = 0;


            // 在请求结束后释放对应的非托管资源
            HttpContext.Response.RegisterForDispose(zip);
            HttpContext.Response.RegisterForDispose(zipStream);

            return File(zipStream, "application/zip", "test.zip");
        }

        private List<string> GetFiles(string dir)
        {
            var files = new List<string>();
            files.AddRange(Directory.GetFiles(dir));

            var dirs = Directory.GetDirectories(dir);
            if (dirs.Length > 0)
            {
                foreach (var item in dirs)
                {
                    files.AddRange(GetFiles(item));
                }
            }

            return files;
        }
    }
}