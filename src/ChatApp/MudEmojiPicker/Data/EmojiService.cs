

using Newtonsoft.Json;

namespace MudEmojiPicker.Data
{
    public class EmojiService
    {

        private static List<EmojiGroup> emojiLists;
        private static Dictionary<string, string> _twitterEmojis;
        
        public string emptyEmoji = @"<svg xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 36 36""></svg>";
        private readonly HttpClient httpClient;

        public List<EmojiGroup> EmojiLists { get => emojiLists; set => emojiLists = value; }

        public EmojiService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task TaskInitialize() 
        {
            if(EmojiLists?.Any() ?? false)
                return;

            string EmojisString = await httpClient.GetStringAsync("emoji-all-groups.json");

            _twitterEmojis = typeof(Twemoji).GetFields()
                .ToDictionary(m =>
                {
                    var res = m.Name
                        .Replace("E", "").Replace("_", " ")
                        .ToUpper();
                    
                    return res;
                }, info => info.GetValue(info).ToString());

            EmojiLists = JsonConvert.DeserializeObject<List<EmojiGroup>>(EmojisString);

            EmojiLists = EmojiLists.Select(emojiGroup =>
            {
                emojiGroup.emojiList.Select(emojiList =>
                {
                    emojiList.svg = GetSvgValue(emojiList.unicode);
                    if (emojiList.skins != null)
                    {
                        emojiList.skins.Insert(0,new Skin(){svg = emojiList.svg, unicode = emojiList.unicode});
                        emojiList.skins = emojiList.skins.Select(skin =>
                        {
                            skin.svg = GetSvgValue(skin.unicode);
                            return skin;
                        }).Where(emoji => emoji.svg != emptyEmoji).ToList();
                    }
                    return emojiList;
                }).Where(emoji => emoji.svg != emptyEmoji).ToList();
                return emojiGroup;
            }).ToList();
        }
        
        
        /// <summary>
        ///     Get the Hex values from an emoji
        /// </summary>
        /// <returns> List of Hex Values </returns>
        public static IEnumerable<string> EmojiToHex(string emoji)
        {
            var enumerator = emoji.EnumerateRunes();
            while (enumerator.MoveNext())
                yield return Char.ConvertToUtf32(enumerator.Current.ToString(), 0)
                    .ToString("X");
        }
    
        /// <summary>
        ///     Get the Hex values from an emoji with ouf "FE0F"
        /// </summary>
        /// <returns> List of Hex Values </returns>
        public string EmojiToKey(string emoji)
        {
            var test = EmojiToHex(emoji)
                .Where(s => !s.Equals("FE0F"));
            return string.Join(" ", test);
        }
        
        /// <summary>
        ///     Get a SVG value from the given emoji, so it can be use in an MudIcone
        /// </summary>
        /// <returns> SVG as a string </returns>
        public string GetSvgValue(string emoji)
        {
            try
            {
                string key = EmojiToKey(emoji);
                
                if (_twitterEmojis.ContainsKey(key))
                {
                    return _twitterEmojis[key];
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return emptyEmoji;
        }
        
        
        
        
        
    }
}