namespace NETMP.Module10.Http.Data
{
    public class SiteNode
    {
        public string Uri { get; }

        public string Html { get; set; }

        public byte[] Media { get; set; }

        public SiteNode() { }

        public SiteNode(string uri)
        {
            Uri = uri;
        }
    }
}
