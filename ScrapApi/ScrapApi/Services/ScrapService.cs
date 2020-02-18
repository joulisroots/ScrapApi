using HtmlAgilityPack;
using ScrapApi.Models;
using ScrapApi.Utils;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ScrapApi.Services
{
    public class ScrapService
    {
        private readonly Uri _repositoryUri;

        /// <summary>
        /// Default constructor setting the repository Uri.
        /// </summary>
        /// <param name="repositoryUri">The repository Uri.</param>
        public ScrapService(Uri repositoryUri)
        {
            _repositoryUri = repositoryUri;
        }

        /// <summary>
        /// Collect the data from github repository.
        /// </summary>
        /// <returns>The datails of the repository content.</returns>
        public async Task<List<RepositoryDetailsModel>> CollectData()
        {
            var document = await ScrapeWebsite();

            var downloadUri = GetDownloadUrl(document);

            var zipStream = DownloadFile(downloadUri);

            return ExtractData(zipStream);
        }

        private List<RepositoryDetailsModel> ExtractData(Stream zipStream)
        {
            var guid = Guid.NewGuid();
            var pathToExtract = $"{Directory.GetCurrentDirectory()}/downloads/{guid}";

            using (var zip = new ZipArchive(zipStream, ZipArchiveMode.Read))
            {
                zip.ExtractToDirectory(pathToExtract);
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(pathToExtract);

            var groupedFiles = directoryInfo.GetFiles("*", SearchOption.AllDirectories).GroupBy(p => p.Extension);

            var detailsResult = new List<RepositoryDetailsModel>();

            foreach (var fileGroup in groupedFiles)
            {
                var detail = new RepositoryDetailsModel()
                {
                    FileExtension = fileGroup.First().Extension,
                    TotalBytes = fileGroup.Sum(p => p.Length),
                    TotalLines = fileGroup.Sum(p => File.ReadAllLines(p.FullName).Length),
                    Files = fileGroup.Select(p => p.Name).ToList()
                };

                detailsResult.Add(detail);
            }

            directoryInfo.Delete(true);

            return detailsResult;
        }

        /// <summary>
        /// Gets the website data and convert to HtmlDocument.
        /// </summary>
        /// <returns>Website HtmlDocument.</returns>
        private async Task<HtmlDocument> ScrapeWebsite()
        {
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(_repositoryUri);

            var pageContents = await response.Content.ReadAsStringAsync();
            HtmlDocument pageDocument = new HtmlDocument();

            pageDocument.LoadHtml(pageContents);

            return pageDocument;
        }

        /// <summary>
        /// Obtains the url from "Download Zip" button in WebSite's HtmlDocument.
        /// </summary>
        /// <param name="document">WebSite's HtmlDocument.</param>
        /// <returns>The uri for download the zip file.</returns>
        private Uri GetDownloadUrl(HtmlDocument document)
        {
            var downloadElementAttribute = document.DocumentNode.SelectNodes("//a[contains(@class,'js-anon-download-zip-link')]")[0].GetAttributeValue("href", null);

            if (string.IsNullOrWhiteSpace(downloadElementAttribute))
            {
                throw new ScrapException("MSG_0002");
            }

            Uri downloadUri;

            if (Uri.TryCreate($"https://github.com{downloadElementAttribute}", UriKind.Absolute, out downloadUri))
            {
                return downloadUri;
            }
            else
            {
                Log.Error($"Uri Value: https://github.com{downloadElementAttribute}");
                throw new ScrapException("MSG_0002");
            }
        }

        /// <summary>
        /// Download de the zip file.
        /// </summary>
        /// <param name="downloadFileuri">Uri for download the file.</param>
        /// <returns>The stream of the file.</returns>
        private Stream DownloadFile(Uri downloadFileuri)
        {
            WebClient webClient = new WebClient();

            webClient.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
            webClient.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");

            var fileBytes = webClient.DownloadData(downloadFileuri);

            webClient.Dispose();

            Stream zipStream = new MemoryStream(fileBytes);

            return zipStream;
        }
    }
}
