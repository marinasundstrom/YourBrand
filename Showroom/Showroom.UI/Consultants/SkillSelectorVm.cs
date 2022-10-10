using System.ComponentModel.DataAnnotations;

using YourBrand.Showroom.Client;

namespace YourBrand.Showroom.Consultants;


    public class SkillSelectorVm 
    {
        [Required]
        public SkillDto? Skill { get; set; }

        [Required]
        public SkillLevel SkillLevel { get; set; } = Showroom.Client.SkillLevel.Competent;

        public List<SkillVM> Skills { get; set; } = new List<SkillVM>();

        public void AddSkill()
        {
            Skills.Add(new SkillVM() {
                Skill = Skill!,
                Level = SkillLevel,
                Selected = true
            });

            Skill = null;
            SkillLevel = Showroom.Client.SkillLevel.Competent;
        }

        public void RemoveSkill(SkillVM skill) 
        {
            Skills.Remove(skill);
        }
    }

    public class SkillVM
    {
        public SkillDto Skill { get; set; }

        public SkillLevel Level { get; set; } = Showroom.Client.SkillLevel.Competent;

        public bool Selected { get; set; }
    }