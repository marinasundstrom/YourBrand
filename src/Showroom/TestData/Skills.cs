using System.Text.Json;

namespace YourBrand.Showroom.TestData
{
    public static class Skills2
    {
        public static IDictionary<string, Dictionary<string, SkillInfo>> FromJson(string json)  
        {
            return JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, SkillInfo>> >(json)!;
        }
    }

    public class SkillInfo 
    {
        public int Level { get; set; }

        public string? Comment { get; set; }

        public LinkInfo? Link { get; set; }
    }

    public  class LinkInfo 
    {
        public string Title { get; set; }

        public string Href { get; set; }
    }

    /*
    string SkillLevelToString(int skillLevel)
    {
        return skillLevel switch {
            1 => "Novice",
            2 => "Advanced Beginner",
            3 => "Competent",
            4 => "Proficient",
            5 => "Expert",
            _ => "Invalid"
        };
    }
    */
}