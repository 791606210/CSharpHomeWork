using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace ParallelCrawler
{
    class Crawler
    {
        public event Action<Crawler> CrawlerStopped;
        public event Action<Crawler, int, string, string> PageDownloaded;

        //所有已下载和待下载URL，key是URL，value表示是否下载成功
        private Dictionary<string, bool> urls = new Dictionary<string, bool>();

        //待下载队列
        private ConcurrentQueue<string> pending = new ConcurrentQueue<string>();


        //URL检测表达式，用于在HTML文本中查找URL
        private readonly string urlDetectRegex = @"(href|HREF)[]*=[]*[""'](?<url>[^""'#>]+)[""']";

        //URL解析表达式
        public static readonly string urlParseRegex = @"^(?<site>https?://(?<host>[\w.-]+)(:\d+)?($|/))(\w+/)*(?<file>[^#?]*)";
        public string HostFilter { get; set; } //主机过滤规则
        public string FileFilter { get; set; } //文件过滤规则
        public int MaxPage { get; set; } //最大下载数量
        public string StartURL { get; set; } //起始网址
        public Encoding HtmlEncoding { get; set; } //网页编码
        public Dictionary<string, bool> DownloadedPages { get => urls; } //已下载网页

        //构造方法，设置最大页面，编码方式和线程数
        public Crawler()
        {
            MaxPage = 100;
            HtmlEncoding = Encoding.UTF8;
            ThreadPool.SetMaxThreads(4, 2);

        }

        public void Start()
        {
            urls.Clear();
            pending = new ConcurrentQueue<string>();
            pending.Enqueue(StartURL);
            int count = 0;
            List<Task> tasks = new List<Task>();
            PageDownloaded += (crawler, index, url, info) => { count++; };
            while (tasks.Count < MaxPage)
            {
                if (!pending.TryDequeue(out string url))
                {
                    if (count < tasks.Count)
                    {
                        continue;
                    }
                    else { break; }
                }
                int index = tasks.Count;
                Task task = Task.Run(() => DownloadAndParse(url, index));
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());
            CrawlerStopped(this);
        }

        public void DownloadAndParse(string url, int index)
        {
            //StateObj state = obj as StateObj;

            try
            {
                string html = DownLoad(url, index);
               
                urls[url] = true;
                Parse(html, url);//解析,并加入新的链接
                PageDownloaded(this, index, url, "success");
                //ThreadPool.QueueUserWorkItem(new WaitCallback(()=>PageDownloaded()),count );

            }
            catch (Exception ex)
            {
                PageDownloaded(this, index, url, "  Error:" + ex.Message);

            }
        }


        private string DownLoad(string url, int index)
        {
            WebClient webClient = new WebClient();
           
            webClient.Encoding = HtmlEncoding;
            string html = webClient.DownloadString(url);
            string fileName = urls.Count.ToString();
            File.WriteAllText(index + ".html", html, Encoding.UTF8);
            return html;
        }

        private void Parse(string html, string pageUrl)
        {


            var matches = new Regex(urlDetectRegex).Matches(html);
            foreach (Match match in matches)
            {
                string linkUrl = match.Groups["url"].Value;
                if (linkUrl == null || linkUrl == "") continue;
                linkUrl = FixUrl(linkUrl, pageUrl);//转绝对路径
                                                   //解析出host和file两个部分，进行过滤
                Match linkUrlMatch = Regex.Match(linkUrl, urlParseRegex);
                string host = linkUrlMatch.Groups["host"].Value;
                string file = linkUrlMatch.Groups["file"].Value;
                if (file == "") file = "index.html";
                if (Regex.IsMatch(host, HostFilter) && Regex.IsMatch(file, FileFilter))
                {
                    if (!urls.ContainsKey(linkUrl))
                    {
                        pending.Enqueue(linkUrl);
                        urls.Add(linkUrl, false);
                    }
                }


            }
        }


        //将相对路径转为绝对路径
        static private string FixUrl(string url, string pageUrl)
        {
            if (url.Contains("://"))
            {
                return url;
            }
            if (url.StartsWith("//"))
            {
                return "http:" + url;
            }
            if (url.StartsWith("/"))
            {
                Match urlMatch = Regex.Match(pageUrl, urlParseRegex);
                String site = urlMatch.Groups["site"].Value;
                return site.EndsWith("/") ? site + url.Substring(1) : site + url;
            }

            if (url.StartsWith("../"))
            {
                url = url.Substring(3);
                int idx = pageUrl.LastIndexOf('/');
                return FixUrl(url, pageUrl.Substring(0, idx));
            }

            if (url.StartsWith("./"))
            {
                return FixUrl(url.Substring(2), pageUrl);
            }

            int end = pageUrl.LastIndexOf("/");
            return pageUrl.Substring(0, end) + "/" + url;
        }
    }

    class StateObj
    {
        public string html;
        public string url;
        public StateObj(string html, string url)
        {
            this.url = url;
            this.html = html;
        }
    }

}
