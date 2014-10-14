using System.ComponentModel;

namespace Itera.MachineLearning.Fitness.Services
{
    public enum StateOfGround
    {
        [Description("Surface of ground dry (without cracks and no appreciable amount of dust or loose sand)")]
        Dry = 0,
        [Description("Surface of ground moist")]
        Moist = 1,
        [Description("Surface of ground wet (standing water in small or large pools on surface)")]
        Wet = 2,
        [Description("Flooded")]
        Flooded = 3,
        [Description("Surface of ground frozen")]
        SurfaceFrozen = 4,
        [Description("Glaze on ground")]
        Glaze = 5,
        [Description("Loose dry dust or sand not covering ground completely")]
        SomeDust = 6,
        [Description("Thin cover of loose dry dust or sand covering ground completely")]
        ThinDustCover = 7,
        [Description("Moderate or thick cover of loose dry dust or sand covering ground, completely")]
        ThickDustCover = 8,    
        [Description("Extremely dry with cracks")]
        ExtremelyDry = 9,
        [Description("Ground predominantly covered by ice")]
        CoveredWithIce = 10,
        [Description("Compact or wet snow (with or without ice) covering less than one half of the ground")]
        LittleWetSnowCover =11,
        [Description("Compact or wet snow (with or without ice) covering at least one half of the ground but ground not completely covered")]
        SomeWetSnowCover = 12,
        [Description("Even layer of compact or wet snow covering ground completely")]
        EvenWetSnowCover = 13,
        [Description("Uneven layer of compact or wet snow covering ground completely")]
        UnevenWetSnowCover = 14,
        [Description("Loose dry snow covering less than one half of the ground")]
        LittleDrySnowCover = 15,
        [Description("Loose dry snow covering at least one half of the ground but ground not completely covered")]
        SomeDrySnowCover = 16,
        [Description("Even layer of loose dry snow covering ground completely")]
        EvenDrySnowCover = 17,
        [Description("Uneven layer of loose dry snow covering ground completely")]
        UnevenDrySnowCover = 18,
        [Description("Snow covering ground completely; deep drifts")]
        DriftSnowCover  = 19,
        [Description("Missing value")]
        Missing = 31 
    }
}