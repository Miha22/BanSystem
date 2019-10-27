using System.Collections.Generic;

namespace BanSystem
{
    public class DiscordWebhookMessage
    {
        public string username { get; set; }
        public string avatar_url { get; set; }
        public string content { get; set; }
        public List<Embed> embeds = new List<Embed>();
    }

    public class Embed
    {
        public Author author { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string description { get; set; }
        public int color { get; set; }
        public Field[] fields { get; set; }
        public Thumbnail thumbnail { get; set; }
        public Image image { get; set; }
        public Footer footer { get; set; }
    }

    public class Author
    {
        public string name { get; set; }
        public string url { get; set; }
        public string icon_url { get; set; }
    }

    public class Thumbnail
    {
        public Thumbnail(string url)
        {
            this.url = url;
        }

        public string url { get; set; }
    }

    public class Image
    {
        public Image(string url)
        {
            this.url = url;
        }

        public string url { get; set; }
    }

    public class Footer
    {
        public Footer(string text, string iconUrl = null)
        {
            this.text = text;
            this.icon_url = iconUrl;
        }

        public string text { get; set; }
        public string icon_url { get; set; }
    }

    public class Field
    {
        public Field(string name, object value, bool inline)
        {
            this.name = name;
            this.value = value.ToString();
            this.inline = inline;
        }

        public string name { get; set; }
        public string value { get; set; }
        public bool inline { get; set; }
    }
}
