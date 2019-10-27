//public class DiscordWebhookMessage
//{
//    public string username = "WebHook";
//    public string avatar_url = "https://i.imgur.com/4M34hi2.png";
//    public Embed[] embeds = new Embed[] { new Embed() };
//}

//public class Embed
//{
//    public Author author = new Author();
//    public string title = "Title";
//    public string url = "https://google.com/";
//    public string description = "Text message. You can use Markdown here. *Italic* **bold** __underline__ ~~strikeout~~ [hyperlink](https://google.com) `code`";
//    public int color = 15258703;
//    public Field[] fields = new Field[]
//    {
//            new Field("Text", "More text", true),
//            new Field("Even more text", "Yup", true),
//            new Field("Use `\"inline\": true` parameter, if you want to display fields in the same line.", "okay..."),
//            new Field("Thanks!", "You're welcome :wink:")
//    };
//    public Thumbnail thumbnail = new Thumbnail("https://upload.wikimedia.org/wikipedia/commons/3/38/4-Nature-Wallpapers-2014-1_ukaavUI.jpg");
//    public Image image = new Image("https://upload.wikimedia.org/wikipedia/commons/3/38/4-Nature-Wallpapers-2014-1_ukaavUI.jpg");
//    public Footer footer = new Footer("Woah! So cool! :smirk:", "https://i.imgur.com/fKL31aD.jpg");
//}

//public class Field
//{
//    public Field(string name, string value, bool inline = false)
//    {
//        this.name = name;
//        this.value = value.ToString();
//        this.inline = inline;
//    }

//    public string name = "";
//    public string value = "";
//    public bool inline = false;
//}

//public class Author
//{
//    public Author()
//    {

//    }
//    public Author(string name, string url, string icon_url)
//    {
//        this.name = name;
//        this.url = url;
//        this.icon_url = icon_url;
//    }
//    public string name = "Birdie";
//    public string url = "https://www.reddit.com/r/cats/";
//    public string icon_url = "https://i.imgur.com/R66g1Pe.jpg";
//}

//public class Thumbnail
//{
//    public Thumbnail()
//    {

//    }
//    public Thumbnail(string url)
//    {
//        this.url = url;
//    }

//    public string url = "";
//}

//public class Image
//{
//    public Image()
//    {

//    }
//    public Image(string url)
//    {
//        this.url = url;
//    }

//    public string url = "";
//}

//public class Footer
//{
//    public Footer()
//    {

//    }
//    public Footer(string text, string iconUrl)
//    {
//        this.text = text;
//        this.icon_url = iconUrl;
//    }

//    public string text = "";
//    public string icon_url = "";
//}