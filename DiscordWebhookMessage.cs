public class DiscordWebhookMessage
{
    public string username;
    public string avatar_url;
    public Embed[] embeds;
}

public class Embed
{
    public Author author;
    public string title;
    public string url;
    public string description;
    public int color;
    public Field[] fields;
    public Thumbnail thumbnail;
    public Image image;
    public Footer footer;
}

public class Field
{
    public Field(string name, string value, bool inline = true)
    {
        this.name = name;
        this.value = value.ToString();
        this.inline = inline;
    }

    public string name;
    public string value;
    public bool inline;
}

public class Author
{
    public Author()
    {

    }
    public Author(string name, string url, string icon_url)
    {
        this.name = name;
        this.url = url;
        this.icon_url = icon_url;
    }
    public string name;
    public string url;
    public string icon_url;
}

public class Thumbnail
{
    public Thumbnail()
    {

    }
    public Thumbnail(string url)
    {
        this.url = url;
    }

    public string url;
}

public class Image
{
    public Image()
    {

    }
    public Image(string url)
    {
        this.url = url;
    }

    public string url;
}

public class Footer
{
    public Footer()
    {

    }
    public Footer(string text, string iconUrl)
    {
        this.text = text;
        this.icon_url = iconUrl;
    }

    public string text;
    public string icon_url;
}