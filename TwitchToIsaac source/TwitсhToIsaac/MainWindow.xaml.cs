﻿using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TwitchLib;
using TwitchLib.Models.Client;
using System.Timers;
using MaterialDesignThemes.Wpf;
using TwitсhToIsaac.Classes;
using TwitсhToIsaac.Classes.VotingOptions;
using System.Text.RegularExpressions;
using static TwitсhToIsaac.Classes.ScreenStatus;
using TwitchToIsaac;
using System.Collections.Specialized;
using System.Net;

namespace TwithToIsaac
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Card> UITabs = new List<Card>();
        List<Button> UIMenuButtons = new List<Button>();
        public bool isOk = false;
        Overlay overlay = null;
        public bool overlayShowed = false;

        public MainWindow()
        {
            InitializeComponent();

            UITabs.Add(Card_Main);
            UITabs.Add(Card_Channel);
            UITabs.Add(Card_Chances);
            UITabs.Add(Card_Render);
            UITabs.Add(Card_Mod);
            UITabs.Add(Card_About);

            UIMenuButtons.Add(B_main);
            UIMenuButtons.Add(B_channel);
            UIMenuButtons.Add(B_chances);
            UIMenuButtons.Add(B_render);
            UIMenuButtons.Add(B_mod);
            UIMenuButtons.Add(B_about);

            ScreenStatus.Lpaused = LMain_ispaused;
            ScreenStatus.Lchat = LMain_chatstat;
            ScreenStatus.Lviewers = LMain_viewers;
            ScreenStatus.Lsubs = LMain_newsubs;
            ScreenStatus.Lruns = LMain_runcount;
            ScreenStatus.log = LBMain_log;

            if (IOLink.IsCorrectPath())
            {
                IOLink.Start();
                overlay = new Overlay();
                Controller.Init(LMain_mainstatus, BMain_run, overlay);
                isOk = true;
            }
            else
            {
                LMain_mainstatus.Text = "Mod not found. Put program folder in the mod\nfolder and restart it";
                isOk = false;
                B_channel.IsEnabled = false;
                B_chances.IsEnabled = false;
                B_render.IsEnabled = false;
                B_about.IsEnabled = false;
            }

            string s = UpdateChecker.checkUpd();

            if (s != null)
                MessageBox.Show("New version available - " + s + "! Go to tab 'About' and download it!", "New version");


            if (File.Exists("../content/shaders.xml"))
                BRender_shaders.IsChecked = false;
            else
                BRender_shaders.IsChecked = true;

        }

        private void ChangeTabButton_click(object sender, RoutedEventArgs e)
        {
            for (var i = 0; i < UITabs.Count; i++)
            {
                UITabs[i].Visibility = Visibility.Hidden;
                UIMenuButtons[i].IsEnabled = true;
            }

            switch (((Button)sender).Name)
            {
                case "B_main":
                    Card_Main.Visibility = Visibility.Visible;
                    B_main.IsEnabled = false;
                    break;

                case "B_channel":
                    Card_Channel.Visibility = Visibility.Visible;
                    B_channel.IsEnabled = false;
                    break;

                case "B_chances":
                    Card_Chances.Visibility = Visibility.Visible;
                    B_chances.IsEnabled = false;
                    break;

                case "B_render":
                    Card_Render.Visibility = Visibility.Visible;
                    B_render.IsEnabled = false;
                    break;

                case "B_mod":
                    Card_Mod.Visibility = Visibility.Visible;
                    B_mod.IsEnabled = false;
                    break;

                case "B_about":
                        Card_About.Visibility = Visibility.Visible;
                        B_about.IsEnabled = false;
                    break;
            }
        }

        private void BChannel_save_Click(object sender, RoutedEventArgs e)
        {
            VoteTime.vote = int.Parse(TChannel_votetime.Text);
            VoteTime.delay = int.Parse(TChannel_delaytime.Text);
            SpecialAppear.subs = (bool)CChannel_subs.IsChecked;
            SpecialAppear.bits = (bool)CChannel_bits.IsChecked;
            SpecialAppear.followers = (bool)CChannel_followers.IsChecked;

            SettingsLoader.s.channel = TTwitchChannel_name.Text;
            SettingsLoader.s.timeforvote = TChannel_votetime.Text;
            SettingsLoader.s.delayvote = TChannel_delaytime.Text;
            SettingsLoader.s.subsap = CChannel_subs.IsChecked;
            SettingsLoader.s.followsap = CChannel_followers.IsChecked;
            SettingsLoader.s.bitsap = CChannel_bits.IsChecked;


            if (TTwitchChannel_name.Text != "")
                Controller.JoinOnChannel(TTwitchChannel_name.Text);

            if (TYoutubeChannel_name.Text != "")
                Controller.JoinOnYoutubeStream(TYoutubeChannel_name.Text);
        }

        private void SChances_events_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if ((bool)!CChances_events.IsChecked)
                CChances_events.IsChecked = true;

            VoteChances.Event = (byte)SChances_events.Value;

            if (LChances_events != null)
                LChances_events.Content = VoteChances.Event;
        }

        private void SChances_items_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if ((bool)!CChances_items.IsChecked)
                CChances_items.IsChecked = true;

            VoteChances.Item = (byte)SChances_items.Value;

            if (LChances_items != null)
                LChances_items.Content = VoteChances.Item;
        }

        private void SChances_trinkets_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if ((bool)!CChances_trinkets.IsChecked)
                CChances_trinkets.IsChecked = true;

            VoteChances.Trinket = (byte)SChances_trinkets.Value;

            if (LChances_trinkets != null)
                LChances_trinkets.Content = VoteChances.Trinket;
        }

        private void SChances_hearts_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if ((bool)!CChances_hearts.IsChecked)
                CChances_hearts.IsChecked = true;

            VoteChances.Heart = (byte)SChances_hearts.Value;

            if (LChances_hearts != null)
                LChances_hearts.Content = VoteChances.Heart;
        }

        private void SChances_pickups_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if ((bool)!CChances_pickups.IsChecked)
                CChances_pickups.IsChecked = true;

            VoteChances.Pickup = (byte)SChances_pickups.Value;

            if (LChances_pickups != null)
                LChances_pickups.Content = VoteChances.Pickup;
        }

        private void SChances_companions_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if ((bool)!CChances_companions.IsChecked)
                CChances_companions.IsChecked = true;

            VoteChances.Companion = (byte)SChances_companions.Value;

            if (LChances_companions != null)
                LChances_companions.Content = VoteChances.Companion;
        }

        private void SChances_pockets_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if ((bool)!CChances_pockets.IsChecked)
                CChances_pockets.IsChecked = true;

            VoteChances.Pocket = (byte)SChances_pockets.Value;

            if (LChances_pockets != null)
                LChances_pockets.Content = VoteChances.Pocket;
        }

        private void CChances_events_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)!CChances_events.IsChecked)
                VoteChances.Event = 0;
            else if (SChances_events != null)
                VoteChances.Event = (byte)SChances_events.Value;

            if (LChances_events != null && SChances_events != null)
                LChances_events.Content = VoteChances.Event;
        }

        private void CChances_items_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)!CChances_items.IsChecked)
                VoteChances.Item = 0;
            else if (SChances_items != null)
                VoteChances.Item = (byte)SChances_items.Value;

            if (LChances_items != null && SChances_items != null)
                LChances_items.Content = VoteChances.Item;
        }

        private void CChances_trinkets_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)!CChances_trinkets.IsChecked)
                VoteChances.Trinket = 0;
            else if (SChances_trinkets != null)
                VoteChances.Trinket = (byte)SChances_trinkets.Value;

            if (LChances_trinkets != null && SChances_trinkets != null)
                LChances_trinkets.Content = VoteChances.Trinket;
        }

        private void CChances_hearts_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)!CChances_hearts.IsChecked)
                VoteChances.Heart = 0;
            else if (SChances_hearts != null)
                VoteChances.Heart = (byte)SChances_hearts.Value;

            if (LChances_hearts != null && SChances_hearts != null)
                LChances_hearts.Content = VoteChances.Heart;
        }

        private void CChances_pickups_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)!CChances_pickups.IsChecked)
                VoteChances.Pickup = 0;
            else if (SChances_pickups != null)
                VoteChances.Pickup = (byte)SChances_pickups.Value;

            if (LChances_pickups != null && SChances_pickups != null)
                LChances_pickups.Content = VoteChances.Pickup;
        }

        private void CChances_companions_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)!CChances_companions.IsChecked)
                VoteChances.Companion = 0;
            else if (SChances_companions != null)
                VoteChances.Companion = (byte)SChances_companions.Value;

            if (LChances_companions != null && SChances_companions != null)
                LChances_companions.Content = VoteChances.Companion;
        }

        private void CChances_pockets_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)!CChances_pockets.IsChecked)
                VoteChances.Pocket = 0;
            else if (SChances_pockets != null)
                VoteChances.Pocket = (byte)SChances_pockets.Value;

            if (LChances_pockets != null && SChances_pockets != null)
                LChances_pockets.Content = VoteChances.Pocket;
        }

        private void BChances_save_Click(object sender, RoutedEventArgs e)
        {
            SettingsLoader.s.chEvents = (bool)CChances_events.IsChecked ? SChances_events.Value : 0;
            SettingsLoader.s.chItems = (bool)CChances_items.IsChecked ? SChances_items.Value : 0;
            SettingsLoader.s.chTrinkets = (bool)CChances_trinkets.IsChecked ? SChances_trinkets.Value : 0;
            SettingsLoader.s.chHearts = (bool)CChances_hearts.IsChecked ? SChances_hearts.Value : 0;
            SettingsLoader.s.chPickups = (bool)CChances_pickups.IsChecked ? SChances_pickups.Value : 0;
            SettingsLoader.s.chComps = (bool)CChances_companions.IsChecked ? SChances_companions.Value : 0;
            SettingsLoader.s.chPockets = (bool)CChances_pockets.IsChecked ? SChances_pockets.Value : 0;

            VoteChances.Save();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"\d{1,}");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void TRender_first_TextChanged(object sender, TextChangedEventArgs e)
        {
            Thickness t = new Thickness();
            if (TRender_firstX != null && TRender_firstY != null && TRender_firstX.Text != "" && TRender_firstY.Text != "")
                t = new Thickness(double.Parse(TRender_firstX.Text)/1.4, double.Parse(TRender_firstY.Text)/1.4, 0, 0);
            LRender_firstline.Margin = t;
        }

        private void BRender_resetFirstLine_Click(object sender, RoutedEventArgs e)
        {
            TRender_firstX.Text = "16";
            TRender_firstY.Text = "241";
        }

        private void TRender_second_TextChanged(object sender, TextChangedEventArgs e)
        {
            Thickness t = new Thickness();
            if (TRender_secondX != null && TRender_secondY != null && TRender_secondX.Text != "" && TRender_secondY.Text != "")
                t = new Thickness(double.Parse(TRender_secondX.Text) / 1.4, double.Parse(TRender_secondY.Text) / 1.4, 0, 0);
            LRender_secondline.Margin = t;
        }

        private void BRender_resetSecondLine_Click(object sender, RoutedEventArgs e)
        {
            TRender_secondX.Text = "16";
            TRender_secondY.Text = "256";
        }

        private void BRender_save_Click(object sender, RoutedEventArgs e)
        {

            if (BRender_shaders.IsChecked == true && File.Exists("../content/shaders.xml"))
            {
                File.Move("../content/shaders.xml", "../content/disabled.shaders.xml.disabled");
            }

            if (BRender_shaders.IsChecked == false && File.Exists("../content/disabled.shaders.xml.disabled"))
            {
                File.Move("../content/disabled.shaders.xml.disabled", "../content/shaders.xml");
            }

            SettingsLoader.s.firstline.x = TRender_firstX.Text;
            SettingsLoader.s.firstline.y = TRender_firstY.Text;
            SettingsLoader.s.secondline.x = TRender_secondX.Text;
            SettingsLoader.s.secondline.y = TRender_secondY.Text;

            RenderSettings.FirstLine.x = TRender_firstX.Text != "" ? int.Parse(TRender_firstX.Text) : 0;
            RenderSettings.FirstLine.y = TRender_firstY.Text != "" ? int.Parse(TRender_firstY.Text) : 0;

            RenderSettings.SecondLine.x = TRender_secondX.Text != "" ? int.Parse(TRender_secondX.Text) : 0;
            RenderSettings.SecondLine.y = TRender_secondY.Text != "" ? int.Parse(TRender_secondY.Text) : 0;

            IOLink.InputParam.textparam.firstline.x = RenderSettings.FirstLine.x;
            IOLink.InputParam.textparam.firstline.y = RenderSettings.FirstLine.y;
            IOLink.InputParam.textparam.secondline.x = RenderSettings.SecondLine.x;
            IOLink.InputParam.textparam.secondline.y = RenderSettings.SecondLine.y;
            IOLink.AcceptInputParam();
        }

        private void BMod_save_Click(object sender, RoutedEventArgs e)
        {
            SettingsLoader.s.subdel = TMod_subdeltime.Text;

            IOLink.InputParam.subdel = int.Parse(TMod_subdeltime.Text) * 60 * 30;
        }

        private void BMain_run_Click(object sender, RoutedEventArgs e)
        {
            Controller.Start();

            if ((string)BMain_run.Content == "Run!" && (bool)CChannel_announce.IsChecked == true)
            {
                string url = "https://discordapp.com/api/webhooks/344890534013173770/Aa6g-ki0tBO5OTHrrwe9QVLC9Xnd5YL19myRrlutBvKxMtAJa3Xf7LS9gD1s5Skc1inR";
                using (var webClient = new WebClient())
                {
                    var pars = new NameValueCollection();

                    string msg = "";

                    if (TTwitchChannel_name.Text != "")
                        msg += "\n :purple_heart: Watch on Twitch - " + "<https://twitch.tv/" + TTwitchChannel_name.Text + ">";

                    if (TYoutubeChannel_name.Text != "")
                        msg += "\n :heart: Watch on Youtube - <" + TYoutubeChannel_name.Text + ">";

                    pars.Add("content", "New stream available!" + msg);
                    webClient.UploadValues(url, pars);
                }
            }

            BMain_run.Content = "Reset";
            LMain_mainstatus.Text = "Let's fun!";
            overlay.wait_text.Text = "Please, wait...";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            overlay.Close();
            SettingsLoader.Save();

            Controller.Stop();
            System.Threading.Thread.Sleep(1200);
            IOLink.Stop();
        }

        private void BLinks_modpage_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://vfstudio.github.io/IsaacOnTwitch/");
        }

        private void BLink_feedback_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/VFStudio/IsaacOnTwitch/issues");
        }

        private void BLink_authorReddit_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.reddit.com/user/virtualZer0/");
        }

        private void BLink_authorVk_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://vk.com/yevstafyev");
        }

        private void BLink_authorFacebook_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.facebook.com/profile.php?id=100006251041621");
        }

        private void BLinks_modpage_Click_1(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://moddingofisaac.com/mod/2941/isaac-on-twitch");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SettingsLoader.Load();

            TTwitchChannel_name.Text = SettingsLoader.s.channel;
            TChannel_votetime.Text = SettingsLoader.s.timeforvote;
            TChannel_delaytime.Text = SettingsLoader.s.delayvote;
            CChannel_subs.IsChecked = SettingsLoader.s.subsap;
            CChannel_followers.IsChecked = SettingsLoader.s.followsap;
            CChannel_bits.IsChecked = SettingsLoader.s.bitsap;

            CChances_events.IsChecked = SettingsLoader.s.chEvents >= 1 ? true : false;
            SChances_events.Value = SettingsLoader.s.chEvents;
            CChances_items.IsChecked = SettingsLoader.s.chItems >= 1 ? true : false;
            SChances_items.Value = SettingsLoader.s.chItems;
            CChances_trinkets.IsChecked = SettingsLoader.s.chTrinkets >= 1 ? true : false;
            SChances_trinkets.Value = SettingsLoader.s.chTrinkets;
            CChances_hearts.IsChecked = SettingsLoader.s.chHearts >= 1 ? true : false;
            SChances_hearts.Value = SettingsLoader.s.chHearts;
            CChances_pickups.IsChecked = SettingsLoader.s.chPickups >= 1 ? true : false;
            SChances_pickups.Value = SettingsLoader.s.chPickups;
            CChances_companions.IsChecked = SettingsLoader.s.chComps >= 1 ? true : false;
            SChances_companions.Value = SettingsLoader.s.chComps;
            CChances_pockets.IsChecked = SettingsLoader.s.chPockets >= 1 ? true : false;
            SChances_pockets.Value = SettingsLoader.s.chPockets;

            TRender_firstX.Text = SettingsLoader.s.firstline.x;
            TRender_firstY.Text = SettingsLoader.s.firstline.y;
            TRender_secondX.Text = SettingsLoader.s.secondline.x;
            TRender_secondY.Text = SettingsLoader.s.secondline.y;

            TMod_subdeltime.Text = SettingsLoader.s.subdel;

            BChances_save_Click(null, null);
            BRender_save_Click(null, null);
            BMod_save_Click(null, null);

            ScanMods();
            CMod_integration_Click(null, null);
        }

        private void BLink_artistReddit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BLink_artistVk_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://vk.com/d0rot0s");
        }

        List<VoteItem> ModItems = new List<VoteItem>();
        List<VoteTrinket> ModTrinkets = new List<VoteTrinket>();
        int countItems = 0;
        int countMods = 0;

        public void ScanMods ()
        {
            DirectoryInfo[] Dirs = Directory.GetParent("../").Parent.GetDirectories();
            countItems = 0;
            countMods = 0;

            foreach (DirectoryInfo d in Dirs)
            {
                if (!File.Exists(d.FullName + "/disable.it") && File.Exists(d.FullName + "/content/items.xml") && d.FullName != Directory.GetParent("../").FullName)
                {
                    countMods++;
                    XDocument doc = XDocument.Load(d.FullName + "/content/items.xml");
                    foreach (XElement el in doc.Root.Elements())
                    {
                        string added = el.Attribute("isaacontwitch") == null ? "true" : el.Attribute("isaacontwitch").Value;

                        if (el.Name == "trinket" && added == "true")
                        {
                            ModTrinkets.Add(new VoteTrinket(el.Attribute("name").Value));
                            countItems++;
                        }

                        if (el.Name != "trinket" && added == "true")
                        {
                            ModItems.Add(new VoteItem(el.Attribute("name").Value));
                            countItems++;
                        }
                    }
                }
            }

            TMod_activemods.Text = "Found active mods: " + countMods + " (" + countItems + " items and trinkets)";
        }

        private void BMod_scan_Click(object sender, RoutedEventArgs e)
        {
            ScanMods();
        }

        private void CMod_integration_Click(object sender, RoutedEventArgs e)
        {
            if (CMod_integration.IsChecked == true)
            {
                foreach (VoteItem i in ModItems)
                    VotePool.Items.Add(i);

                foreach (VoteTrinket i in ModTrinkets)
                    VotePool.Trinkets.Add(i);
            }
            else
            {
                foreach (VoteItem i in ModItems)
                    VotePool.Items.Remove(i);

                foreach (VoteTrinket i in ModTrinkets)
                    VotePool.Trinkets.Remove(i);
            }
        }

        private void BMain_overlay_Click(object sender, RoutedEventArgs e)
        {
            if (overlayShowed)
            {
                overlay.Hide();
                IOLink.InputParam.textparam.enabled = true;
                overlayShowed = false;
                BMain_overlay.Content = "Show overlay";
                IOLink.AcceptInputParam();
            }
            else
            {
                overlay.Show();
                IOLink.InputParam.textparam.enabled = false;
                overlayShowed = true;
                BMain_overlay.Content = "Hide overlay";
                IOLink.AcceptInputParam();
            }
        }

        private void SMod_eventmode_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int res = Convert.ToInt32(SMod_eventmode.Value);

            try
            {
                switch (res)
                {
                    case 1:
                        TMod_eventmode.Text = "Easy";
                        Controller.esm = Controller.EventSelectMode.Easy;
                        break;
                    case 2:
                        TMod_eventmode.Text = "Normal";
                        Controller.esm = Controller.EventSelectMode.Normal;
                        break;
                    case 3:
                        TMod_eventmode.Text = "Crazy";
                        Controller.esm = Controller.EventSelectMode.Nightmare;
                        break;
                }
            }
            catch { }
        }
    }
}
