/// <summary>
/// 整理英雄信息中的技能信息
/// </summary>
/// <param name="info"></param>
async void OrganizeHeroAbilities(Models.Hero info)
{
    try
    {
        if (info?.abilities == null) return;
        foreach (var ability in info.abilities)
        {
            // 技能图片和描述
            ability.sAbilityImageUrl = "https://cdn.cloudflare.steamstatic.com/apps/dota2/images/dota_react/abilities/" + ability.name + ".png";
            ability.desc_loc = OrganizeLocString(ability.desc_loc, ability.special_values);
            ability.lore_loc = OrganizeLocString(ability.lore_loc, ability.special_values);
            // 备注
            try
            {
                if (ability.notes_loc != null && ability.notes_loc.Length > 0)
                {
                    StringBuilder notesLocSb = new StringBuilder();
                    for (int i = 0; i < ability.notes_loc.Length; i++)
                    {
                        notesLocSb.Append(ability.notes_loc[i]);
                        if (i < ability.notes_loc.Length - 1)
                        {
                            notesLocSb.Append("\n");
                        }
                    }
                    ability.notesStr = notesLocSb.ToString();
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            // 技能是否有神杖效果
            try
            {
                if (ability.ability_has_scepter)
                {
                    if (string.IsNullOrEmpty(ability.scepter_loc))
                    {
                        //ability.scepter_loc = "Upgraded by Scepter";
                        ability.ability_has_scepter = false;
                    }
                    else
                    {
                        ability.scepter_loc = OrganizeLocString(ability.scepter_loc, ability.special_values);
                    }
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            // 技能是否有魔晶效果
            try
            {
                if (ability.ability_has_shard)
                {
                    if (string.IsNullOrEmpty(ability.shard_loc))
                    {
                        //ability.shard_loc = "Upgraded by Shard";
                        ability.ability_has_shard = false;
                    }
                    else
                    {
                        ability.shard_loc = OrganizeLocString(ability.shard_loc, ability.special_values);
                    }
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            // 技能伤害类型
            try
            {
                switch (ability.damage)
                {
                    case 1:
                        ability.damageStr = "Physical";
                        ability.damageForeground = Models.Ability.AbilityDamageTypePhysicalColor;
                        break;
                    case 2:
                        ability.damageStr = "Magical";
                        ability.damageForeground = Models.Ability.AbilityDamageTypeMagicalColor;
                        break;
                    case 4:
                        ability.damageStr = "Pure";
                        ability.damageForeground = Models.Ability.AbilityDamageTypePureColor;
                        break;
                    case 8:
                        ability.damageStr = "HP Removal";
                        ability.damageForeground = Models.Ability.AbilityDamageTypeHPRemovalColor;
                        break;
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            // 技能是否可以驱散
            try
            {
                switch (ability.dispellable)
                {
                    case 1:
                        ability.dispellableStr = "Strong";
                        break;
                    case 2:
                        ability.dispellableStr = "Yes";
                        break;
                    case 3:
                        ability.dispellableStr = "No";
                        break;
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            // 技能是否无视魔免
            try
            {
                switch (ability.immunity)
                {
                    case 1:
                    case 3:
                        ability.immunityStr = "Yes";
                        break;
                    case 2:
                    case 4:
                        ability.immunityStr = "No";
                        break;
                    case 5:
                        ability.immunityStr = "Allies Yes Enemies No";
                        break;
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            // 技能作用目标
            try
            {
                switch (ability.target_team)
                {
                    case 1:
                        ability.targetStr = 7 == (7 & ability.target_type)
                                            ? "Allied Units And Buildings"
                                            : 3 == (3 & ability.target_type)
                                            ? "Allied Units"
                                            : 5 == (5 & ability.target_type)
                                            ? "Allied Heroes And Buildings"
                                            : 1 == (1 & ability.target_type)
                                            ? "Allied Heroes"
                                            : 2 == (2 & ability.target_type)
                                            ? "Allied Creeps" : "Allies";
                        break;
                    case 2:
                        ability.targetStr = 7 == (7 & ability.target_type)
                                            ? "Enemy Units And Buildings"
                                            : 3 == (3 & ability.target_type)
                                            ? "Enemy Units"
                                            : 5 == (5 & ability.target_type)
                                            ? "Enemy Heroes And Buildings"
                                            : 1 == (1 & ability.target_type)
                                            ? "Enemy Heroes"
                                            : 2 == (2 & ability.target_type)
                                            ? "Enemy Creeps" : "Enemies";
                        break;
                    case 3:
                        ability.targetStr = 1 == (1 & ability.target_type)
                                            ? "Heroes" : "Units";
                        break;
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            // 技能类型
            try
            {
                long behavior;
                bool parse = long.TryParse(ability.behavior, out behavior);
                if (parse)
                {
                    ability.behaviorStr = 0 != (65536 & behavior)
                                        ? "Aura"
                                        : 0 != (4 & behavior)
                                        ? "No Target"
                                        : 0 != (8 & behavior)
                                        ? "Unit Ttarget"
                                        : 0 != (16 & behavior)
                                        ? "Point Target"
                                        : 0 != (32 & behavior)
                                        ? "Point Aoe"
                                        : 0 != (128 & behavior)
                                        ? "Channeled"
                                        : 0 != (512 & behavior)
                                        ? "Toggle"
                                        : 0 != (4096 & behavior)
                                        ? "Autocast"
                                        : 0 != (2 & behavior)
                                        ? "Passive" : string.Empty;
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            // 技能其他的数值
            try
            {
                //Dictionary<string, string> SpecialValuesNames = new Dictionary<string, string>()
                //{
                //    {"#hero_ability", "Ability"}, {"#HeroAbility", "Ability"},
                //    {"#hero_affects", "Affects"}, {"#HeroAffects", "Affects"},
                //    {"#ability_damage", "Damage"}, {"#AbilityDamage", "Damage"},
                //    {"#hero_spell_immunity", "Immunity"}, {"#heroSpellImmunity", "Immunity"},
                //    {"#hero_dispellable", "Dispellable"}, {"#HeroDispellable", "Dispellable"},
                //    {"#hero_damage", "Damage"}, {"#HeroDamage", "Damage"}
                //};
                StringBuilder specialValuesSb = new StringBuilder();
                for (int j = 0; j < ability.special_values.Count; j++)
                {
                    var specialValue = ability.special_values[j];
                    if ((specialValue.name == "#AbilityDamage" || specialValue.name == "#HeroDamage") && string.IsNullOrEmpty(specialValue.heading_loc))
                    {
                        specialValue.heading_loc = "DAMAGE:";
                    }
                    if (!string.IsNullOrEmpty(specialValue.heading_loc) && specialValue.values_float != null && specialValue.values_float.Length > 0)
                    {
                        StringBuilder specialValueSb = new StringBuilder();
                        specialValueSb.Append(specialValue.heading_loc.Replace("\n", "").Trim());
                        specialValueSb.Append(" ");
                        for (int i = 0; i < specialValue.values_float.Length; i++)
                        {
                            if (i > 0)
                            {
                                specialValueSb.Append("/");
                            }
                            specialValueSb.Append(specialValue.values_float[i].ToString("f1").Replace("\n", ""));
                            if (specialValue.is_percentage)
                            {
                                specialValueSb.Append("%");
                            }
                        }
                        specialValuesSb.Append(specialValueSb);
                        specialValuesSb.Append("\n");
                    }
                }
                ability.specialValuesStr = specialValuesSb.ToString().TrimEnd('\n');
                ability.specialValuesStr = OrganizeLocString(ability.specialValuesStr, null);
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            //// 技能范围
            //try
            //{
            //    if (ability.cast_ranges != null && ability.cast_ranges.Length > 0)
            //    {
            //        StringBuilder castRangesSb = new StringBuilder();
            //        bool haveCastRanges = false;
            //        for (int i = 0; i < ability.cast_ranges.Length; i++)
            //        {
            //            double append = Math.Floor(ability.cast_ranges[i] * 10000) / 10000;
            //            castRangesSb.Append(append);
            //            if (i < ability.cast_ranges.Length - 1)
            //            {
            //                castRangesSb.Append("/");
            //            }
            //            if (append > 0)
            //            {
            //                haveCastRanges = true;
            //            }
            //        }
            //        if (haveCastRanges)
            //        {
            //            ability.castRangesStr = castRangesSb.ToString();
            //        }
            //    }
            //}
            //catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            //// 技能间隔
            //try
            //{
            //    if (ability.cast_points != null && ability.cast_points.Length > 0)
            //    {
            //        StringBuilder castPointsSb = new StringBuilder();
            //        bool haveCastPoints = false;
            //        for (int i = 0; i < ability.cast_points.Length; i++)
            //        {
            //            double append = Math.Floor(ability.cast_points[i] * 10000) / 10000;
            //            castPointsSb.Append(append);
            //            if (i < ability.cast_points.Length - 1)
            //            {
            //                castPointsSb.Append("/");
            //            }
            //            if (append > 0)
            //            {
            //                haveCastPoints = true;
            //            }
            //        }
            //        if (haveCastPoints)
            //        {
            //            ability.castPointsStr = castPointsSb.ToString();
            //        }
            //    }
            //}
            //catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }
        foreach (var ability in info.abilities)
        {
            await ability.LoadImageAsync(64);
        }
    }
    catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
}