﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls;
using StikyNotes.Annotations;
using StikyNotes.Utils.HotKeyUtil;
using ComboBoxItem = System.Windows.Controls.ComboBoxItem;

namespace StikyNotes
{
    public class SettingViewModel
    {
        public ProgramData Datas { get; set; }

        public HotKeyModel ShowAllHotKey { get; set; }

        public SettingWindow SettingWin { get; set; }

        public RelayCommand<bool> IsTopMostChangedCommand { get; set; }

        public RelayCommand<bool> IsStartUpWithSystemChangedCommand { get; set; }

        public RelayCommand<SelectionChangedEventArgs> SelectionChangedCommand { get; set; }
        public RelayCommand<KeyEventArgs> ShowAllHotKeyChangedCommand { get; set; }

        /// <summary>
        /// 当输入Ctrl+A，X，C，Z，V等按键，调用此command
        /// </summary>
        public RelayCommand<string> ShowAllTextUsedCommand { get; set; }

        public SettingViewModel(SettingWindow win)
        {
            SettingWin = win;
            Datas = ProgramData.Instance;
            ShowAllHotKey = ProgramData.Instance.ShowAllHotKey;

            IsTopMostChangedCommand = new RelayCommand<bool>(IsTopMostChangedMethod);
            IsStartUpWithSystemChangedCommand = new RelayCommand<bool>(IsStartUpWithSystemChangedMethod);
            SelectionChangedCommand = new RelayCommand<SelectionChangedEventArgs>(SelectionChangedMethod);
            ShowAllHotKeyChangedCommand = new RelayCommand<KeyEventArgs>(ShowAllShortCutChangedMethod);
            ShowAllTextUsedCommand= new RelayCommand<string>(ShowAllTextUsedMethod);
        }

        private void ShowAllTextUsedMethod(string key)
        {
            //使用了ctrl
            bool useCtrl = true;
            bool useAlt = false;
            bool useShift = false;
            EKey? useKey = null;
            if (key == "X")
            {
                 useKey = EKey.X;
            }else if (key == "A")
            {
                useKey = EKey.A;
            }
            else if (key == "C")
            {
                useKey = EKey.C;
            }
            else if (key == "V")
            {
                useKey = EKey.V;
            }
            else if (key == "Z")
            {
                useKey = EKey.Z;
            }
            var oldHotKey = ShowAllHotKey;
            var newModel = ShowAllHotKey;
            newModel.IsSelectAlt = useAlt;
            newModel.IsSelectCtrl = useCtrl;
            newModel.IsSelectShift = useShift;
            if (useKey != null) newModel.SelectKey = (EKey) useKey;

            if (!useCtrl && !useAlt && !useShift)
                return;
            var hotKeyList = new ObservableCollection<HotKeyModel>
            {
                ShowAllHotKey
            };
            HotKeySettingsManager.Instance.IsShowAllWindowHotKeyNeedChanged = true;
            if (!HotKeySettingsManager.Instance.RegisterGlobalHotKey(hotKeyList))
            {
                //todo 
                MessageBox.Show("快捷键注册失败，可能系统或其它软件存在冲突");
                ShowAllHotKey = oldHotKey;
            }
            else
            {
                ShowAllHotKey = newModel;
                ProgramData.Instance.ShowAllHotKey = ShowAllHotKey;

                this.SettingWin.ShowAllTB.Text = ShowAllHotKey.ToString();
            }

            return;

        }
        private void ShowAllShortCutChangedMethod(KeyEventArgs e)
        {
            //            if (e.Key == Key.LeftAlt || e.Key == Key.RightAlt)
            //            {
            //                return;
            //            }
            //
            //            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            //            {
            //                return;
            //            }
            //
            //            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            //            {
            //                return;
            //            }
            bool useCtrl = false;//(e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
            bool useAlt = false;//(e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt;
            bool useShift = false;//(e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
//            EKey useKey = EKey.Q;
            foreach (int v in Enum.GetValues(typeof(EKey)))
            {
                string keyName = Enum.GetName(typeof(EKey), v);
                if (e.Key.ToString() == keyName)
                {
                    var useKey = (EKey)v;
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                        useCtrl = true;
                    if (Keyboard.IsKeyDown(Key.LeftShift)||Keyboard.IsKeyDown(Key.RightShift))
                        useShift = true;
                    if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
                        useAlt = true;
                    var oldHotKey = ShowAllHotKey;
                    var newModel = ShowAllHotKey;
                    newModel.IsSelectAlt = useAlt;
                    newModel.IsSelectCtrl = useCtrl;
                    newModel.IsSelectShift = useShift;
                    newModel.SelectKey = useKey;

                    if (!useCtrl && !useAlt && !useShift)
                        return;
                    var hotKeyList = new ObservableCollection<HotKeyModel>
                    {
                        ShowAllHotKey
                    };
                    HotKeySettingsManager.Instance.IsShowAllWindowHotKeyNeedChanged = true;
                    if (!HotKeySettingsManager.Instance.RegisterGlobalHotKey(hotKeyList))
                    {
                        //todo 
                        MessageBox.Show("快捷键注册失败，可能系统或其它软件存在冲突");
                        ShowAllHotKey = oldHotKey;
                    }
                    else
                    {
                        ShowAllHotKey = newModel;
                        ProgramData.Instance.ShowAllHotKey = ShowAllHotKey;

                        this.SettingWin.ShowAllTB.Text = ShowAllHotKey.ToString();
                    }

                    return;
                }
            }

            
//            HotKeySettingsManager.Instance.RegisterGlobalHotKey(hotKeyList);



           
            //清楚当前快捷键
            //todo
            //检测输入的快捷键是否可用
            //todo
            //将更新的快捷键输入文本框




        }



        /// <summary>
        /// 选择主题发生改变
        /// </summary>
        /// <param name="obj"></param>
        private void SelectionChangedMethod(SelectionChangedEventArgs e)
        {
            var content = e.AddedItems[0] as ComboBoxItem;

            switch (content.Content.ToString())
            {
                case "橘黄色":
                    ThemeAssist.ChangeTheme(Themes.Orange);
                    break;
                case "蓝色":
                    ThemeAssist.ChangeTheme(Themes.Blue);
                    break;
                case "灰色":
                    ThemeAssist.ChangeTheme(Themes.Gray);
                    break;
            }
        }

        /// <summary>
        /// 是否开机启动
        /// </summary>
        /// <param name="param">当前选项是否勾选</param>
        private void IsStartUpWithSystemChangedMethod(bool param)
        {
            if (param == false)
            {
                Microsoft.Win32.RegistryKey key =
                    Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run",
                        true);

                Assembly curAssembly = Assembly.GetExecutingAssembly();
                key.SetValue(curAssembly.GetName().Name, curAssembly.Location);
            }
            else
            {
                Microsoft.Win32.RegistryKey key =
                    Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run",
                        true);
                Assembly curAssembly = Assembly.GetExecutingAssembly();
                if (key.GetValue(curAssembly.GetName().Name) != null)
                {
                    key.DeleteValue(curAssembly.GetName().Name);
                }
            }

            Datas.IsStartUpWithSystem = !param;
        }

        /// <summary>
        /// 窗体是否置顶
        /// </summary>
        /// <param name="param"></param>
        private void IsTopMostChangedMethod(bool param)
        {
            if (param == false)
            {
                foreach (var win in WindowsManager.Instance.Windows)
                {
                    win.Topmost = true;
                }
            }
            else
            {
                foreach (var win in WindowsManager.Instance.Windows)
                {
                    win.Topmost = false;
                }
            }

            Datas.IsWindowTopMost = !param;
        }



    }
}