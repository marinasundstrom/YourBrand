namespace MudEmojiPicker.Data;


public class EmojiList
{
    public string unicode { get; set; }
    public string svg { get; set; }
    public List<string> tags { get; set; }
    public List<Skin> skins { get; set; }
    
}

public class EmojiGroup
{
    public int group { get; set; }
    public List<EmojiList> emojiList { get; set; }
}

public class Skin
{
    public string unicode { get; set; }
    public string svg { get; set; }
}