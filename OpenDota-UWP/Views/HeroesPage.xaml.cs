using OpenDota_UWP.Helpers;
using OpenDota_UWP.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace OpenDota_UWP.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class HeroesPage : Page
    {
        /// <summary>
        /// 三种属性的英雄根据情况放到这个集合里面
        /// </summary>
        public ObservableCollection<DotaHeroes> HeroesObservableCollection = new ObservableCollection<DotaHeroes>();

        public static DotaHeroes SelectedHero;

        /// <summary>
        /// 选择的英雄的主属性
        /// </summary>
        public static int selectedHeroPA = 0; // 1:str 2:agi 3:int

        public HeroesPage()
        {
            this.InitializeComponent();
        }
        /// <summary>
        /// 重写导航至此页面的代码,显示动画
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is NavigationTransitionInfo transition)
            {
                navigationTransition.DefaultNavigationTransitionInfo = transition;
            }
            if (e.Parameter.GetType().Equals(typeof(int)))
            {
                try
                {
                    selectedHeroPA = (int)e.Parameter;
                }
                catch
                {
                    selectedHeroPA = 1;
                }
            }

            LoadAllHeroesList();

            //判断是否需要下载新的数据，不用的话直接从DotaHeroHelper._data即可访问整个json，需要的话调用下载方法
            //await APIHelper.DownloadHeroAttributesDataAsync();
            MainPage.Current.ShowHero.Begin();

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (HeroesObservableCollection != null)
            {
                HeroesObservableCollection.Clear();
                HeroesObservableCollection = null;
            }

            base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// 添加所有的英雄到字典里，以及添加指定的英雄到列表中
        /// </summary>
        private async void LoadAllHeroesList()
        {
            WaitStackPanel.Visibility = Visibility.Visible;
            WaitProgressRing.IsActive = true;

            // 下载属性三围数据
            if (DotaHeroHelper._data.Length < 256)
            {
                await DotaHeroHelper.DownloadHeroAttributesDataAsync();
            }

            if (!ConstantsHelper.dotaHerosDictionary.ContainsKey("grimstroke"))
            {
                // 添加字典
                ConstantsHelper.dotaHerosDictionary.Clear();
                #region
                ConstantsHelper.dotaHerosDictionary.Add("axe", new DotaHeroes("斧王", "axe", "斧王二十连斩！"));
                //ConstantsHelper.dotaHerosDictionary["axe"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.axe.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("earthshaker", new DotaHeroes("撼地者", "earthshaker", "土地在我脚下移动。"));
                //ConstantsHelper.dotaHerosDictionary["earthshaker"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.earthshaker.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("pudge", new DotaHeroes("帕吉", "pudge", "嘴巴里再塞个苹果就能当火鸡了！"));
                //ConstantsHelper.dotaHerosDictionary["pudge"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.pudge.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("sand_king", new DotaHeroes("沙王", "sand_king", "国王一言，八马难追。"));
                //ConstantsHelper.dotaHerosDictionary["sand_king"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.sand_king.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("sven", new DotaHeroes("斯温", "sven", "流浪剑客又杀一个。"));
                //ConstantsHelper.dotaHerosDictionary["sven"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.sven.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("tiny", new DotaHeroes("小小", "tiny", "目标，采石场！"));
                //ConstantsHelper.dotaHerosDictionary["tiny"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.tiny.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("kunkka", new DotaHeroes("昆卡", "kunkka", "船员们，前方惊涛骇浪，等我发号施令！"));
                //ConstantsHelper.dotaHerosDictionary["kunkka"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.kunkka.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("slardar", new DotaHeroes("斯拉达", "slardar", "深埋于海底深渊里的宝藏由我来保护。"));
                //ConstantsHelper.dotaHerosDictionary["slardar"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.slardar.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("tidehunter", new DotaHeroes("潮汐猎人", "tidehunter", "鲜血染红了海水，到用餐的时间了！"));
                //ConstantsHelper.dotaHerosDictionary["tidehunter"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.tidehunter.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("beastmaster", new DotaHeroes("兽王", "beastmaster", "即使兵临城下，我们也不会灭绝。"));
                //ConstantsHelper.dotaHerosDictionary["beastmaster"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.beastmaster.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("wraith_king", new DotaHeroes("冥魂大帝", "wraith_king", "skeleton_king", "就算我是吓唬人又如何？"));
                //ConstantsHelper.dotaHerosDictionary["wraith_king"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.skeleton_king.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("dragon_knight", new DotaHeroes("龙骑士", "dragon_knight", "打仗靠武器，战争靠金钱。"));
                //ConstantsHelper.dotaHerosDictionary["dragon_knight"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.dragon_knight.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("clockwerk", new DotaHeroes("发条技师", "clockwerk", "rattletrap", "我运行起来就像时钟一样规律。"));
                //ConstantsHelper.dotaHerosDictionary["clockwerk"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.rattletrap.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("lifestealer", new DotaHeroes("噬魂鬼", "lifestealer", "life_stealer", "拿走你的钱，我要的是你的命。"));
                //ConstantsHelper.dotaHerosDictionary["lifestealer"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.life_stealer.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("omniknight", new DotaHeroes("全能骑士", "omniknight", "平静之人，方能行走自如。"));
                //ConstantsHelper.dotaHerosDictionary["omniknight"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.omniknight.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("huskar", new DotaHeroes("哈斯卡", "huskar", "你不适合这个版本。"));
                //ConstantsHelper.dotaHerosDictionary["huskar"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.huskar.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("night_stalker", new DotaHeroes("暗夜魔王", "night_stalker", "白昼行僵，暗夜魔王。"));
                //ConstantsHelper.dotaHerosDictionary["night_stalker"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.night_stalker.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("doom", new DotaHeroes("末日使者", "doom", "doom_bringer", "和我作对的结果就是死于我的双角之下。"));
                //ConstantsHelper.dotaHerosDictionary["doom"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.doom_bringer.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("spirit_breaker", new DotaHeroes("裂魂人", "spirit_breaker", "条条大路通战场。"));
                //ConstantsHelper.dotaHerosDictionary["spirit_breaker"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.spirit_breaker.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("alchemist", new DotaHeroes("炼金术士", "alchemist", "当初真应该去当个调酒师！"));
                //ConstantsHelper.dotaHerosDictionary["alchemist"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.alchemist.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("lycan", new DotaHeroes("狼人", "lycan", "防火防盗防狼人。"));
                //ConstantsHelper.dotaHerosDictionary["lycan"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.lycan.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("brewmaster", new DotaHeroes("酒仙", "brewmaster", "烈酒，是我前进的动力。"));
                //ConstantsHelper.dotaHerosDictionary["brewmaster"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.brewmaster.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("chaos_knight", new DotaHeroes("混沌骑士", "chaos_knight", "饥渴和宝剑，促使我前行。"));
                //ConstantsHelper.dotaHerosDictionary["chaos_knight"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.chaos_knight.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("treant_protector", new DotaHeroes("树精卫士", "treant_protector", "treant", "我最想找一个地方把自己种下，那里阳光明媚，旁边清泉流过。"));
                //ConstantsHelper.dotaHerosDictionary["treant_protector"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.treant.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("undying", new DotaHeroes("不朽尸王", "undying", "一次又一次，我在生死之间轮回。"));
                //ConstantsHelper.dotaHerosDictionary["undying"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.undying.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("Io", new DotaHeroes("艾欧", "Io", "wisp", "♫~"));
                //ConstantsHelper.dotaHerosDictionary["Io"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.wisp.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("centaur_warrunner", new DotaHeroes("半人马战行者", "centaur_warrunner", "centaur", "鲜血遍洒大地。"));
                //ConstantsHelper.dotaHerosDictionary["centaur_warrunner"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.centaur.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("magnus", new DotaHeroes("马格纳斯", "magnus", "magnataur", "兽蹄快如闪电。"));
                //ConstantsHelper.dotaHerosDictionary["magnus"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.magnataur.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("timbersaw", new DotaHeroes("伐木机", "timbersaw", "shredder", "那棵树看起来生气了。"));
                //ConstantsHelper.dotaHerosDictionary["timbersaw"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.shredder.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("bristleback", new DotaHeroes("钢背兽", "bristleback", "你被刺得眼睛都闭不上了，不是吗？"));
                //ConstantsHelper.dotaHerosDictionary["bristleback"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.bristleback.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("tusk", new DotaHeroes("巨牙海民", "tusk", "所到之处，皆是北方。"));
                //ConstantsHelper.dotaHerosDictionary["tusk"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.tusk.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("abaddon", new DotaHeroes("亚巴顿", "abaddon", "你的痛苦.. 将会载入史册。"));
                //ConstantsHelper.dotaHerosDictionary["abaddon"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.abaddon.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("elder_titan", new DotaHeroes("上古巨神", "elder_titan", "战斗如同破碎世界的碎片一般连结起来。"));
                //ConstantsHelper.dotaHerosDictionary["elder_titan"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.elder_titan.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("legion_commander", new DotaHeroes("军团指挥官", "legion_commander", "把敌人赶尽杀绝，不惜一切代价！"));
                //ConstantsHelper.dotaHerosDictionary["legion_commander"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.legion_commander.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("earth_spirit", new DotaHeroes("大地之灵", "earth_spirit", "眼睛会撒谎，但是灵气不虚。"));
                //ConstantsHelper.dotaHerosDictionary["earth_spirit"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.earth_spirit.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("abyssal_underlord", new DotaHeroes("孽主", "abyssal_underlord", "黑暗随我前进。"));
                //ConstantsHelper.dotaHerosDictionary["abyssal_underlord"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.abyssal_underlord.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("phoenix", new DotaHeroes("凤凰", "phoenix", "♫~"));
                //ConstantsHelper.dotaHerosDictionary["phoenix"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.phoenix.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("mars", new DotaHeroes("玛尔斯", "mars", "战矛所到之处，就是战场！"));
                //ConstantsHelper.dotaHerosDictionary["abyssal_underlord"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.abyssal_underlord.attribs;

                ConstantsHelper.dotaHerosDictionary.Add("anti_mage", new DotaHeroes("敌法师", "anti_mage", "antimage", "魔法之血债，汝之性命来偿还！"));
                //ConstantsHelper.dotaHerosDictionary["anti_mage"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.antimage.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("bloodseeker", new DotaHeroes("嗜血狂魔", "bloodseeker", "我闻到了血腥味。"));
                //ConstantsHelper.dotaHerosDictionary["bloodseeker"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.bloodseeker.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("drow_ranger", new DotaHeroes("卓尔游侠", "drow_ranger", "很多东西你都看不到了，比如说，明天。"));
                //ConstantsHelper.dotaHerosDictionary["drow_ranger"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.drow_ranger.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("juggernaut", new DotaHeroes("主宰", "juggernaut", "勤练，带来力量。"));
                //ConstantsHelper.dotaHerosDictionary["juggernaut"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.juggernaut.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("mirana", new DotaHeroes("米拉娜", "mirana", "我已经给他们机会逃跑了。"));
                //ConstantsHelper.dotaHerosDictionary["mirana"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.mirana.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("morphling", new DotaHeroes("变体精灵", "morphling", "开始涨潮了。"));
                //ConstantsHelper.dotaHerosDictionary["morphling"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.morphling.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("shadow_fiend", new DotaHeroes("影魔", "shadow_fiend", "nevermore", "你的灵魂是我的。"));
                //ConstantsHelper.dotaHerosDictionary["shadow_fiend"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.nevermore.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("phantom_lancer", new DotaHeroes("幻影长矛手", "phantom_lancer", "我们人数比你多。我们人数比你们全队都多！"));
                //ConstantsHelper.dotaHerosDictionary["phantom_lancer"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.phantom_lancer.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("razor", new DotaHeroes("剃刀", "razor", "金子，是优良的导体！"));
                //ConstantsHelper.dotaHerosDictionary["razor"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.razor.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("vengeful_spirit", new DotaHeroes("复仇之魂", "vengeful_spirit", "vengefulspirit", "复仇永在我左右。"));
                //ConstantsHelper.dotaHerosDictionary["vengeful_spirit"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.vengefulspirit.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("riki", new DotaHeroes("力丸", "riki", "我的招式并不复杂，但是足以致命。"));
                //ConstantsHelper.dotaHerosDictionary["riki"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.riki.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("sniper", new DotaHeroes("狙击手", "sniper", "锁定，死定"));
                //ConstantsHelper.dotaHerosDictionary["sniper"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.sniper.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("venomancer", new DotaHeroes("剧毒术士", "venomancer", "带着毒药，充满活力。"));
                //ConstantsHelper.dotaHerosDictionary["venomancer"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.venomancer.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("faceless_void", new DotaHeroes("虚空假面", "faceless_void", "我已经看到了未来，可是你不在其中。"));
                //ConstantsHelper.dotaHerosDictionary["faceless_void"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.faceless_void.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("phantom_assassin", new DotaHeroes("幻影刺客", "phantom_assassin", "我的暗杀名单上有五个名字。"));
                //ConstantsHelper.dotaHerosDictionary["phantom_assassin"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.phantom_assassin.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("templar_assassin", new DotaHeroes("圣堂刺客", "templar_assassin", "邪教异端，圣堂格杀勿论。"));
                //ConstantsHelper.dotaHerosDictionary["templar_assassin"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.templar_assassin.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("viper", new DotaHeroes("冥界亚龙", "viper", "没人会忘记蝮蛇之噬。"));
                //ConstantsHelper.dotaHerosDictionary["viper"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.viper.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("luna", new DotaHeroes("露娜", "luna", "月有盈亏，我的仁慈亦然。"));
                //ConstantsHelper.dotaHerosDictionary["luna"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.luna.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("clinkz", new DotaHeroes("克林克兹", "clinkz", "与其感慨路难行，不如马上出发。"));
                //ConstantsHelper.dotaHerosDictionary["clinkz"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.clinkz.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("broodmother", new DotaHeroes("育母蜘蛛", "broodmother", "在我的网中，你的眼睛背叛了你。"));
                //ConstantsHelper.dotaHerosDictionary["broodmother"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.broodmother.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("bounty_hunter", new DotaHeroes("赏金猎人", "bounty_hunter", "不嫌生意难做，不嫌赏金太多。"));
                //ConstantsHelper.dotaHerosDictionary["bounty_hunter"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.bounty_hunter.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("weaver", new DotaHeroes("编织者", "weaver", "命运的梭线，由我来编织。"));
                //ConstantsHelper.dotaHerosDictionary["weaver"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.weaver.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("spectre", new DotaHeroes("幽鬼", "spectre", "来自世外之地。来自世外之时。"));
                //ConstantsHelper.dotaHerosDictionary["spectre"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.spectre.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("ursa", new DotaHeroes("熊战士", "ursa", "金子，还有甜点。"));
                //ConstantsHelper.dotaHerosDictionary["ursa"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.ursa.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("gyrocopter", new DotaHeroes("矮人直升机", "gyrocopter", "我是属于天空的，而你只能呆在地面。"));
                //ConstantsHelper.dotaHerosDictionary["gyrocopter"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.gyrocopter.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("lone_druid", new DotaHeroes("德鲁伊", "lone_druid", "兵贵神速。"));
                //ConstantsHelper.dotaHerosDictionary["lone_druid"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.lone_druid.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("meepo", new DotaHeroes("米波", "meepo", "两只爪总有先后。"));
                //ConstantsHelper.dotaHerosDictionary["meepo"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.meepo.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("nyx_assassin", new DotaHeroes("司夜刺客", "nyx_assassin", "我的话语将穿透你的耳膜，直达你的思维。"));
                //ConstantsHelper.dotaHerosDictionary["nyx_assassin"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.nyx_assassin.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("naga_siren", new DotaHeroes("娜迦海妖", "naga_siren", "鱼人不能失败。"));
                //ConstantsHelper.dotaHerosDictionary["naga_siren"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.naga_siren.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("slark", new DotaHeroes("斯拉克", "slark", "你的身体是个牢笼，我只不过是解放了你。"));
                //ConstantsHelper.dotaHerosDictionary["slark"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.slark.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("medusa", new DotaHeroes("美杜莎", "medusa", "注视我的双眼。"));
                //ConstantsHelper.dotaHerosDictionary["medusa"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.medusa.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("troll_warlord", new DotaHeroes("巨魔战将", "troll_warlord", "我的大斧已经饥渴难耐了。"));
                //ConstantsHelper.dotaHerosDictionary["troll_warlord"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.troll_warlord.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("ember_spirit", new DotaHeroes("灰烬之灵", "ember_spirit", "学到了吗？"));
                //ConstantsHelper.dotaHerosDictionary["ember_spirit"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.ember_spirit.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("terrorblade", new DotaHeroes("恐怖利刃", "terrorblade", "聪明人见到我早逃走了。"));
                //ConstantsHelper.dotaHerosDictionary["terrorblade"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.terrorblade.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("arc_warden", new DotaHeroes("天穹守望者", "arc_warden", "本尊，终于驾临此地。"));
                //ConstantsHelper.dotaHerosDictionary["arc_warden"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.arc_warden.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("monkey_king", new DotaHeroes("齐天大圣", "monkey_king", "哪儿有架，哪儿就有俺！"));
                //ConstantsHelper.dotaHerosDictionary["monkey_king"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.monkey_king.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("pangolier", new DotaHeroes("石鳞剑士", "pangolier", "在你之前求战的可多了，都失败了。"));
                //ConstantsHelper.dotaHerosDictionary["pangolier"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.pangolier.attribs;

                ConstantsHelper.dotaHerosDictionary.Add("bane", new DotaHeroes("祸乱之源", "bane", "快闭上双眼梦见我。"));
                //ConstantsHelper.dotaHerosDictionary["bane"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.bane.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("crystal_maiden", new DotaHeroes("水晶室女", "crystal_maiden", "你可以燃烧我的躯体，但你永远无法解冻我的灵魂。"));
                //ConstantsHelper.dotaHerosDictionary["crystal_maiden"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.crystal_maiden.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("puck", new DotaHeroes("帕克", "puck", "莫名其妙的我就被牵扯进这一场奇怪的活动里来了。"));
                //ConstantsHelper.dotaHerosDictionary["puck"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.puck.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("storm_spirit", new DotaHeroes("风暴之灵", "storm_spirit", "都告诉你了，风暴要来。"));
                //ConstantsHelper.dotaHerosDictionary["storm_spirit"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.storm_spirit.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("windranger", new DotaHeroes("风行者", "windranger", "windrunner", "历经艰苦的失败，非我所爱。"));
                //ConstantsHelper.dotaHerosDictionary["windranger"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.windrunner.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("zeus", new DotaHeroes("宙斯", "zeus", "zuus", "你将希望我从没屈尊去关注你。"));
                //ConstantsHelper.dotaHerosDictionary["zeus"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.zuus.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("lina", new DotaHeroes("莉娜", "lina", "你脸发烫不是因为很热，是因为感到了耻辱。"));
                //ConstantsHelper.dotaHerosDictionary["lina"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.lina.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("lion", new DotaHeroes("莱恩", "lion", "我不怕地狱，地狱害怕我。"));
                //ConstantsHelper.dotaHerosDictionary["lion"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.lion.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("shadow_shaman", new DotaHeroes("暗影萨满", "shadow_shaman", "我刚得到暗影的指示，我们的敌人被判死刑了。"));
                //ConstantsHelper.dotaHerosDictionary["shadow_shaman"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.shadow_shaman.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("witch_doctor", new DotaHeroes("巫医", "witch_doctor", "我的骨骼精奇。"));
                //ConstantsHelper.dotaHerosDictionary["witch_doctor"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.witch_doctor.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("lich", new DotaHeroes("巫妖", "lich", "我朋友间的那一小团冰霜是什么？"));
                //ConstantsHelper.dotaHerosDictionary["lich"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.lich.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("enigma", new DotaHeroes("谜团", "enigma", "在我视野内，万物无所遁形。"));
                //ConstantsHelper.dotaHerosDictionary["enigma"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.enigma.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("tinker", new DotaHeroes("修补匠", "tinker", "思想的每一次碰撞，都会产生一个废物！"));
                //ConstantsHelper.dotaHerosDictionary["tinker"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.tinker.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("necrophos", new DotaHeroes("瘟疫法师", "necrophos", "necrolyte", "你的经脉已经枯萎。"));
                //ConstantsHelper.dotaHerosDictionary["necrolyte"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.necrolyte.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("warlock", new DotaHeroes("术士", "warlock", "死亡与毁灭与我同行！"));
                //ConstantsHelper.dotaHerosDictionary["warlock"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.warlock.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("queen_of_pain", new DotaHeroes("痛苦女王", "queen_of_pain", "queenofpain", "你的痛苦就是我的收获。"));
                //ConstantsHelper.dotaHerosDictionary["queen_of_pain"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.queenofpain.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("death_prophet", new DotaHeroes("死亡先知", "death_prophet", "活着的时候你没有仔细品尝生命，现在好好品尝死亡的滋味！"));
                //ConstantsHelper.dotaHerosDictionary["death_prophet"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.death_prophet.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("pugna", new DotaHeroes("帕格纳", "pugna", "我总是能想出办法让糟糕的事情变得更糟。"));
                //ConstantsHelper.dotaHerosDictionary["pugna"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.pugna.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("dazzle", new DotaHeroes("戴泽", "dazzle", "我会把你的灵魂带到虚无之境。"));
                //ConstantsHelper.dotaHerosDictionary["dazzle"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.dazzle.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("leshrac", new DotaHeroes("拉席克", "leshrac", "你和这世界一样，终会被毁灭。"));
                //ConstantsHelper.dotaHerosDictionary["leshrac"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.leshrac.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("natures_prophet", new DotaHeroes("先知", "natures_prophet", "furion", "拔起树根然后出发吧！"));
                //ConstantsHelper.dotaHerosDictionary["natures_prophet"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.furion.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("dark_seer", new DotaHeroes("黑暗贤者", "dark_seer", "我的敌人们已尽力，但也不过如此。"));
                //ConstantsHelper.dotaHerosDictionary["dark_seer"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.dark_seer.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("enchantress", new DotaHeroes("魅惑魔女", "enchantress", "噢，其实我还挺喜欢他们的。"));
                //ConstantsHelper.dotaHerosDictionary["enchantress"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.enchantress.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("jakiro", new DotaHeroes("杰奇洛", "jakiro", "举双头赞成。"));
                //ConstantsHelper.dotaHerosDictionary["jakiro"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.jakiro.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("batrider", new DotaHeroes("蝙蝠骑士", "batrider", "圣堂裙下死，做鬼也风流！"));
                //ConstantsHelper.dotaHerosDictionary["batrider"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.batrider.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("chen", new DotaHeroes("陈", "chen", "我将黑暗的过去抛弃，在奥比莱斯的光耀下前进。"));
                //ConstantsHelper.dotaHerosDictionary["chen"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.chen.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("ancient_apparition", new DotaHeroes("远古冰魄", "ancient_apparition", "总有一天，冰雪会覆盖这片土地，犹如战争从未发生。"));
                //ConstantsHelper.dotaHerosDictionary["ancient_apparition"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.ancient_apparition.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("invoker", new DotaHeroes("祈求者", "invoker", "吾已现世，普天同庆！"));
                //ConstantsHelper.dotaHerosDictionary["invoker"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.invoker.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("silencer", new DotaHeroes("沉默术士", "silencer", "轻轻的步伐..."));
                //ConstantsHelper.dotaHerosDictionary["silencer"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.silencer.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("Outworld_Devourer", new DotaHeroes("殁境神蚀者", "Outworld_Devourer", "obsidian_destroyer", "我将粉碎他们的意志，破坏他们的征服美梦。"));
                //ConstantsHelper.dotaHerosDictionary["Outworld_Devourer"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.obsidian_destroyer.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("shadow_demon", new DotaHeroes("暗影恶魔", "shadow_demon", "你的暗影落下，我的暗影升起。"));
                //ConstantsHelper.dotaHerosDictionary["shadow_demon"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.shadow_demon.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("ogre_magi", new DotaHeroes("食人魔魔法师", "ogre_magi", "— 好主意！ — 因为这是我想的！"));
                //ConstantsHelper.dotaHerosDictionary["ogre_magi"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.ogre_magi.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("rubick", new DotaHeroes("拉比克", "rubick", "读书人的事情，怎能算偷，这是借。"));
                //ConstantsHelper.dotaHerosDictionary["rubick"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.rubick.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("disruptor", new DotaHeroes("干扰者", "disruptor", "我奔腾而来。"));
                //ConstantsHelper.dotaHerosDictionary["disruptor"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.disruptor.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("keeper_of_the_light", new DotaHeroes("光之守卫", "keeper_of_the_light", "我离目标的距离没有更近，但我的决心更坚定了。"));
                //ConstantsHelper.dotaHerosDictionary["keeper_of_the_light"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.keeper_of_the_light.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("visage", new DotaHeroes("维萨吉", "visage", "凡人一旦穿过了幕帘，就永远也无法返回。"));
                //ConstantsHelper.dotaHerosDictionary["visage"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.visage.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("Skywrath_Mage", new DotaHeroes("天怒法师", "Skywrath_Mage", "skywrath_mage", "我鞠躬尽瘁，天地可鉴。"));
                //ConstantsHelper.dotaHerosDictionary["Skywrath_Mage"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.skywrath_mage.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("techies", new DotaHeroes("工程师", "techies", "Off we go!"));
                //ConstantsHelper.dotaHerosDictionary["techies"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.techies.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("oracle", new DotaHeroes("神谕者", "oracle", "就像承诺一样，你就是用来背弃的！"));
                //ConstantsHelper.dotaHerosDictionary["oracle"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.oracle.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("winter_wyvern", new DotaHeroes("寒冬飞龙", "winter_wyvern", "我不会把朋友丢在没有冬天的世界里。"));
                //ConstantsHelper.dotaHerosDictionary["winter_wyvern"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.winter_wyvern.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("dark_willow", new DotaHeroes("邪影芳灵", "dark_willow", "杰克斯，我们去找下手的人，再挖出他们的双眼，你觉得怎么样？"));
                //ConstantsHelper.dotaHerosDictionary["dark_willow"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.dark_willow.attribs;
                ConstantsHelper.dotaHerosDictionary.Add("grimstroke", new DotaHeroes("天涯墨客", "grimstroke", "你的命运尽在我的笔下！"));
                //ConstantsHelper.dotaHerosDictionary["grimstroke"].HeroInfo.attribs = AllHeroesAttibutesData.herodata.grimstroke.attribs;
                #endregion
            }

            // 根据选择的英雄类型加载指定的英雄列表
            switch (selectedHeroPA)
            {
                case 0:
                case 1:
                    ShowStrHero();
                    break;
                case 2:
                    ShowAgiHero();
                    break;
                case 3:
                    ShowIntHero();
                    break;
                default:
                    ShowStrHero();
                    break;
            }

            WaitStackPanel.Visibility = Visibility.Collapsed;
            WaitProgressRing.IsActive = false;

        }

        /// <summary>
        /// 弹出对话框
        /// </summary>
        /// <param name="content"></param>
        /// <param name="title"></param>
        public async void ShowDialog(string content)
        {
            var dialog = new ContentDialog()
            {
                Title = ":(",
                Content = content,
                PrimaryButtonText = "好的",
                FullSizeDesired = false,
            };

            dialog.PrimaryButtonClick += (_s, _e) => { };
            try
            {
                await dialog.ShowAsync();
            }
            catch { }
        }

        private void Rectangle_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
        }

        private void Rectangle_PointerExited(object sender, PointerRoutedEventArgs e)
        {
        }

        private void HeroesGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (HeroesGridView.ContainerFromItem(e.ClickedItem) is GridViewItem container)
            {
                SelectedHero = container.Content as DotaHeroes;
            }
            this.Frame.Navigate(typeof(HeroInfoPage));
        }

        /// <summary>
        /// 显示力量英雄
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowStrHero()
        {
            selectedHeroPA = 1;
            StrTextBlock.Opacity = 1;
            StrTextBlock.FontWeight = FontWeights.Bold;
            AgiTextBlock.Opacity = 0.7;
            AgiTextBlock.FontWeight = FontWeights.Medium;
            IntTextBlock.Opacity = 0.7;
            IntTextBlock.FontWeight = FontWeights.Medium;

            //AddStrHeroes();
        }

        /// <summary>
        /// 显示敏捷英雄
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowAgiHero()
        {
            selectedHeroPA = 2;
            StrTextBlock.Opacity = 0.7;
            StrTextBlock.FontWeight = FontWeights.Medium;
            AgiTextBlock.Opacity = 1;
            AgiTextBlock.FontWeight = FontWeights.Bold;
            IntTextBlock.Opacity = 0.7;
            IntTextBlock.FontWeight = FontWeights.Medium;

            //AddAgiHeroes();
        }

        /// <summary>
        /// 显示智力英雄
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowIntHero()
        {
            selectedHeroPA = 3;
            StrTextBlock.Opacity = 0.7;
            StrTextBlock.FontWeight = FontWeights.Medium;
            AgiTextBlock.Opacity = 0.7;
            AgiTextBlock.FontWeight = FontWeights.Medium;
            IntTextBlock.Opacity = 1;
            IntTextBlock.FontWeight = FontWeights.Bold;

            //AddIntHeroes();
        }

        ///// <summary>
        ///// 添加力量英雄到集合里面
        ///// </summary>
        //public void AddStrHeroes()
        //{
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["axe"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["earthshaker"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["pudge"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["sand_king"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["sven"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["tiny"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["kunkka"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["slardar"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["tidehunter"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["beastmaster"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["wraith_king"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["dragon_knight"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["clockwerk"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["lifestealer"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["omniknight"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["huskar"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["night_stalker"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["doom"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["spirit_breaker"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["alchemist"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["lycan"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["brewmaster"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["chaos_knight"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["treant_protector"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["undying"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["Io"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["centaur_warrunner"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["magnus"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["timbersaw"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["bristleback"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["tusk"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["abaddon"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["elder_titan"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["legion_commander"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["earth_spirit"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["abyssal_underlord"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["phoenix"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["mars"]);
        //}

        ///// <summary>
        ///// 添加敏捷英雄到集合里面
        ///// </summary>
        //public void AddAgiHeroes()
        //{
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["anti_mage"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["bloodseeker"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["drow_ranger"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["juggernaut"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["mirana"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["morphling"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["shadow_fiend"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["phantom_lancer"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["razor"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["vengeful_spirit"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["riki"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["sniper"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["venomancer"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["faceless_void"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["phantom_assassin"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["templar_assassin"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["viper"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["luna"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["clinkz"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["broodmother"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["bounty_hunter"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["weaver"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["spectre"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["ursa"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["gyrocopter"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["lone_druid"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["meepo"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["nyx_assassin"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["naga_siren"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["slark"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["medusa"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["troll_warlord"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["ember_spirit"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["terrorblade"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["arc_warden"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["monkey_king"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["pangolier"]);
        //}

        ///// <summary>
        ///// 添加智力英雄到集合里面
        ///// </summary>
        //public void AddIntHeroes()
        //{
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["bane"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["crystal_maiden"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["puck"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["storm_spirit"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["windranger"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["zeus"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["lina"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["lion"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["shadow_shaman"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["witch_doctor"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["lich"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["enigma"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["tinker"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["necrophos"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["warlock"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["queen_of_pain"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["death_prophet"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["pugna"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["dazzle"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["leshrac"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["natures_prophet"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["dark_seer"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["enchantress"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["jakiro"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["batrider"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["chen"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["ancient_apparition"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["invoker"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["silencer"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["Outworld_Devourer"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["shadow_demon"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["ogre_magi"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["rubick"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["disruptor"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["keeper_of_the_light"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["visage"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["Skywrath_Mage"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["techies"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["oracle"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["winter_wyvern"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["dark_willow"]);
        //    HeroesObservableCollection.Add(ConstantsHelper.dotaHerosDictionary["grimstroke"]);
        //}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(HeroesPage), 1);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(HeroesPage), 2);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

            this.Frame.Navigate(typeof(HeroesPage), 3);
        }
    }
}
