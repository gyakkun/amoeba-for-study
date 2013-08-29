﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Xml;
using Library;

namespace Amoeba.Properties
{
    public delegate void UsingLanguageChangedEventHandler(object sender);

    class LanguagesManager : IThisLock
    {
        private static LanguagesManager _defaultInstance = new LanguagesManager();
        private static Dictionary<string, Dictionary<string, string>> _dic = new Dictionary<string, Dictionary<string, string>>();
        private static string _usingLanguage = null;
        private static ObjectDataProvider provider;
        private object _thisLock = new object();

        public static UsingLanguageChangedEventHandler UsingLanguageChangedEvent;

        protected static void OnUsingLanguageChangedEvent()
        {
            if (LanguagesManager.UsingLanguageChangedEvent != null)
            {
                LanguagesManager.UsingLanguageChangedEvent(_defaultInstance);
            }
        }

        static LanguagesManager()
        {
#if DEBUG
            string path = @"C:\Core\Project\Amoeba\Amoeba\bin\Debug\Core\Languages";

            if (!Directory.Exists(path))
                path = Path.Combine(Directory.GetCurrentDirectory(), "Languages");
#else
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Languages");
#endif

            LanguagesManager.Load(path);
        }

        private static void Load(string directoryPath)
        {
            if (!Directory.Exists(directoryPath)) return;

            _dic.Clear();

            foreach (string path in Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories))
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();

                using (XmlTextReader xml = new XmlTextReader(path))
                {
                    try
                    {
                        while (xml.Read())
                        {
                            if (xml.NodeType == XmlNodeType.Element)
                            {
                                if (xml.LocalName == "Translate")
                                {
                                    dic.Add(xml.GetAttribute("Key"), xml.GetAttribute("Value"));
                                }
                            }
                        }
                    }
                    catch (XmlException)
                    {

                    }
                }

                _dic[Path.GetFileNameWithoutExtension(path)] = dic;
            }

            if (CultureInfo.CurrentUICulture.Name == "ja-JP" && _dic.Keys.Any(n => n == "Japanese"))
            {
                _usingLanguage = "Japanese";
            }
            else if (_dic.Keys.Any(n => n == "English"))
            {
                _usingLanguage = "English";
            }
        }

        public static LanguagesManager GetInstance()
        {
            return _defaultInstance;
        }

        public static LanguagesManager Instance
        {
            get
            {
                return _defaultInstance;
            }
        }

        /// <summary>
        /// 言語の切り替えメソッド
        /// </summary>
        /// <param name="language">使用言語を指定する</param>
        public static void ChangeLanguage(string language)
        {
            if (!_dic.ContainsKey(language)) throw new ArgumentException();

            _usingLanguage = language;
            LanguagesManager.ResourceProvider.Refresh();

            LanguagesManager.OnUsingLanguageChangedEvent();
        }

        /// <summary>
        /// 使用できる言語リスト
        /// </summary>
        public IEnumerable<string> Languages
        {
            get
            {
                var list = _dic.Keys.ToList();

                list.Sort((x, y) =>
                {
                    return System.IO.Path.GetFileNameWithoutExtension(x).CompareTo(System.IO.Path.GetFileNameWithoutExtension(y));
                });

                return list.ToArray();
            }
        }

        public static ObjectDataProvider ResourceProvider
        {
            get
            {
                if (System.Windows.Application.Current != null)
                {
                    provider = (ObjectDataProvider)System.Windows.Application.Current.FindResource("ResourcesInstance");
                }

                return provider;
            }
        }

        public string Translate(string value)
        {
            if (_usingLanguage != null && _dic[_usingLanguage].ContainsKey(value))
            {
                return _dic[_usingLanguage][value];
            }
            else
            {
                return null;
            }
        }

        #region Property

        public string FontFamily
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("FontFamily");
                }
            }
        }

        public string FontSize
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("FontSize");
                }
            }
        }

        public string DateTime_StringFormat
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("DateTime_StringFormat");
                }
            }
        }


        public string Languages_English
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("Languages_English");
                }
            }
        }

        public string Languages_Japanese
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("Languages_Japanese");
                }
            }
        }


        public string Seed_Name
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("Seed_Name");
                }
            }
        }

        public string Seed_Signature
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("Seed_Signature");
                }
            }
        }

        public string Seed_Length
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("Seed_Length");
                }
            }
        }

        public string Seed_Keywords
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("Seed_Keywords");
                }
            }
        }

        public string Seed_CreationTime
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("Seed_CreationTime");
                }
            }
        }

        public string Seed_Comment
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("Seed_Comment");
                }
            }
        }


        public string Box_Name
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("Box_Name");
                }
            }
        }

        public string Box_Signature
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("Box_Signature");
                }
            }
        }

        public string Box_CreationTime
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("Box_CreationTime");
                }
            }
        }

        public string Box_Comment
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("Box_Comment");
                }
            }
        }


        public string MainWindow_Connection
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_Connection");
                }
            }
        }

        public string MainWindow_Search
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_Search");
                }
            }
        }

        public string MainWindow_Download
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_Download");
                }
            }
        }

        public string MainWindow_Upload
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_Upload");
                }
            }
        }

        public string MainWindow_Share
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_Share");
                }
            }
        }

        public string MainWindow_Store
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_Store");
                }
            }
        }

        public string MainWindow_Log
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_Log");
                }
            }
        }

        public string MainWindow_Connections
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_Connections");
                }
            }
        }

        public string MainWindow_Start
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_Start");
                }
            }
        }

        public string MainWindow_Stop
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_Stop");
                }
            }
        }

        public string MainWindow_UpdateBaseNode
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_UpdateBaseNode");
                }
            }
        }

        public string MainWindow_Link
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_Link");
                }
            }
        }

        public string MainWindow_LinkSettings
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_LinkSettings");
                }
            }
        }

        public string MainWindow_Cache
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_Cache");
                }
            }
        }

        public string MainWindow_CheckSeeds
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_CheckSeeds");
                }
            }
        }

        public string MainWindow_CheckInternalBlocks
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_CheckInternalBlocks");
                }
            }
        }

        public string MainWindow_CheckExternalBlocks
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_CheckExternalBlocks");
                }
            }
        }

        public string MainWindow_ConnectionsSettings
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_ConnectionsSettings");
                }
            }
        }

        public string MainWindow_EncodeAndDecodeStart
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_EncodeAndDecodeStart");
                }
            }
        }

        public string MainWindow_EncodeAndDecodeStop
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_EncodeAndDecodeStop");
                }
            }
        }

        public string MainWindow_View
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_View");
                }
            }
        }

        public string MainWindow_ViewSettings
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_ViewSettings");
                }
            }
        }

        public string MainWindow_Languages
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_Languages");
                }
            }
        }

        public string MainWindow_Help
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_Help");
                }
            }
        }

        public string MainWindow_ManualSite
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_ManualSite");
                }
            }
        }

        public string MainWindow_DeveloperSite
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_DeveloperSite");
                }
            }
        }

        public string MainWindow_CheckUpdate
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_CheckUpdate");
                }
            }
        }

        public string MainWindow_VersionInformation
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_VersionInformation");
                }
            }
        }

        public string MainWindow_SendSpeed
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_SendSpeed");
                }
            }
        }

        public string MainWindow_ReceiveSpeed
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_ReceiveSpeed");
                }
            }
        }

        public string MainWindow_DiskSpaceNotFound_Message
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_DiskSpaceNotFound_Message");
                }
            }
        }

        public string MainWindow_SearchSpaceNotFound_Message
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_SearchSpaceNotFound_Message");
                }
            }
        }

        public string MainWindow_CheckInternalBlocks_Message
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_CheckInternalBlocks_Message");
                }
            }
        }

        public string MainWindow_CheckExternalBlocks_Message
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_CheckExternalBlocks_Message");
                }
            }
        }

        public string MainWindow_CheckBlocks_State
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_CheckBlocks_State");
                }
            }
        }

        public string MainWindow_CheckUpdate_Message
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_CheckUpdate_Message");
                }
            }
        }

        public string MainWindow_LatestVersion_Message
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_LatestVersion_Message");
                }
            }
        }

        public string MainWindow_Close_Message
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_Close_Message");
                }
            }
        }

        public string MainWindow_Delete_Message
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_Delete_Message");
                }
            }
        }

        public string MainWindow_TransferLimit_Message
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_TransferLimit_Message");
                }
            }
        }

        public string MainWindow_Upload_Message
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("MainWindow_Upload_Message");
                }
            }
        }


        public string ConnectionsSettingsWindow_Title
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_Title");
                }
            }
        }

        public string ConnectionsSettingsWindow_BaseNode
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_BaseNode");
                }
            }
        }

        public string ConnectionsSettingsWindow_Node
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_Node");
                }
            }
        }

        public string ConnectionsSettingsWindow_Uris
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_Uris");
                }
            }
        }

        public string ConnectionsSettingsWindow_Uri
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_Uri");
                }
            }
        }

        public string ConnectionsSettingsWindow_OtherNodes
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_OtherNodes");
                }
            }
        }

        public string ConnectionsSettingsWindow_Nodes
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_Nodes");
                }
            }
        }

        public string ConnectionsSettingsWindow_Client
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_Client");
                }
            }
        }

        public string ConnectionsSettingsWindow_Filters
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_Filters");
                }
            }
        }

        public string ConnectionsSettingsWindow_ConnectionType
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_ConnectionType");
                }
            }
        }

        public string ConnectionsSettingsWindow_ProxyUri
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_ProxyUri");
                }
            }
        }

        public string ConnectionsSettingsWindow_UriCondition
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_UriCondition");
                }
            }
        }

        public string ConnectionsSettingsWindow_Option
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_Option");
                }
            }
        }

        public string ConnectionsSettingsWindow_Type
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_Type");
                }
            }
        }

        public string ConnectionsSettingsWindow_Host
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_Host");
                }
            }
        }

        public string ConnectionsSettingsWindow_Server
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_Server");
                }
            }
        }

        public string ConnectionsSettingsWindow_ListenUris
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_ListenUris");
                }
            }
        }

        public string ConnectionsSettingsWindow_Events
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_Events");
                }
            }
        }

        public string ConnectionsSettingsWindow_Data
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_Data");
                }
            }
        }

        public string ConnectionsSettingsWindow_Bandwidth
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_Bandwidth");
                }
            }
        }

        public string ConnectionsSettingsWindow_BandwidthLimit
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_BandwidthLimit");
                }
            }
        }

        public string ConnectionsSettingsWindow_Transfer
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_Transfer");
                }
            }
        }

        public string ConnectionsSettingsWindow_TransferLimitType
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_TransferLimitType");
                }
            }
        }

        public string ConnectionsSettingsWindow_TransferLimitSpan
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_TransferLimitSpan");
                }
            }
        }

        public string ConnectionsSettingsWindow_TransferLimitSize
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_TransferLimitSize");
                }
            }
        }

        public string ConnectionsSettingsWindow_TransferInformation
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_TransferInformation");
                }
            }
        }

        public string ConnectionsSettingsWindow_Downloaded
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_Downloaded");
                }
            }
        }

        public string ConnectionsSettingsWindow_Uploaded
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_Uploaded");
                }
            }
        }

        public string ConnectionsSettingsWindow_Total
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_Total");
                }
            }
        }

        public string ConnectionsSettingsWindow_Reset
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_Reset");
                }
            }
        }

        public string ConnectionsSettingsWindow_ConnectionCount
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_ConnectionCount");
                }
            }
        }

        public string ConnectionsSettingsWindow_DownloadDirectory
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_DownloadDirectory");
                }
            }
        }

        public string ConnectionsSettingsWindow_CacheSize
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_CacheSize");
                }
            }
        }

        public string ConnectionsSettingsWindow_AutoBaseNodeSetting
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_AutoBaseNodeSetting");
                }
            }
        }

        public string ConnectionsSettingsWindow_Tor
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_Tor");
                }
            }
        }

        public string ConnectionsSettingsWindow_Ipv4
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_Ipv4");
                }
            }
        }

        public string ConnectionsSettingsWindow_Ipv6
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_Ipv6");
                }
            }
        }

        public string ConnectionsSettingsWindow_Up
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_Up");
                }
            }
        }

        public string ConnectionsSettingsWindow_Down
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_Down");
                }
            }
        }

        public string ConnectionsSettingsWindow_Add
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_Add");
                }
            }
        }

        public string ConnectionsSettingsWindow_Edit
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_Edit");
                }
            }
        }

        public string ConnectionsSettingsWindow_Delete
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_Delete");
                }
            }
        }

        public string ConnectionsSettingsWindow_Ok
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_Ok");
                }
            }
        }

        public string ConnectionsSettingsWindow_Cancel
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_Cancel");
                }
            }
        }

        public string ConnectionsSettingsWindow_Cut
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_Cut");
                }
            }
        }

        public string ConnectionsSettingsWindow_Copy
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_Copy");
                }
            }
        }

        public string ConnectionsSettingsWindow_Paste
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_Paste");
                }
            }
        }

        public string ConnectionsSettingsWindow_CacheResize_Message
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionsSettingsWindow_CacheResize_Message");
                }
            }
        }


        public string ConnectionType_None
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionType_None");
                }
            }
        }

        public string ConnectionType_Tcp
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionType_Tcp");
                }
            }
        }

        public string ConnectionType_Socks4Proxy
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionType_Socks4Proxy");
                }
            }
        }

        public string ConnectionType_Socks4aProxy
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionType_Socks4aProxy");
                }
            }
        }

        public string ConnectionType_Socks5Proxy
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionType_Socks5Proxy");
                }
            }
        }

        public string ConnectionType_HttpProxy
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionType_HttpProxy");
                }
            }
        }


        public string TransferLimitType_None
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("TransferLimitType_None");
                }
            }
        }

        public string TransferLimitType_Downloads
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("TransferLimitType_Downloads");
                }
            }
        }

        public string TransferLimitType_Uploads
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("TransferLimitType_Uploads");
                }
            }
        }

        public string TransferLimitType_Total
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("TransferLimitType_Total");
                }
            }
        }


        public string ProgressWindow_Title
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ProgressWindow_Title");
                }
            }
        }

        public string ProgressWindow_Cancel
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ProgressWindow_Cancel");
                }
            }
        }

        public string ProgressWindow_Ok
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ProgressWindow_Ok");
                }
            }
        }


        public string ViewSettingsWindow_Title
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ViewSettingsWindow_Title");
                }
            }
        }

        public string ViewSettingsWindow_Update
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ViewSettingsWindow_Update");
                }
            }
        }

        public string ViewSettingsWindow_UpdateUrl
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ViewSettingsWindow_UpdateUrl");
                }
            }
        }

        public string ViewSettingsWindow_ProxyUri
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ViewSettingsWindow_ProxyUri");
                }
            }
        }

        public string ViewSettingsWindow_Signature
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ViewSettingsWindow_Signature");
                }
            }
        }

        public string ViewSettingsWindow_Signatures
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ViewSettingsWindow_Signatures");
                }
            }
        }

        public string ViewSettingsWindow_Keywords
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ViewSettingsWindow_Keywords");
                }
            }
        }

        public string ViewSettingsWindow_Miscellaneous
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ViewSettingsWindow_Miscellaneous");
                }
            }
        }

        public string ViewSettingsWindow_Box
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ViewSettingsWindow_Box");
                }
            }
        }

        public string ViewSettingsWindow_RelateBoxFile
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ViewSettingsWindow_RelateBoxFile");
                }
            }
        }

        public string ViewSettingsWindow_Events
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ViewSettingsWindow_Events");
                }
            }
        }

        public string ViewSettingsWindow_OpenBox
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ViewSettingsWindow_OpenBox");
                }
            }
        }

        public string ViewSettingsWindow_ExtractTo
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ViewSettingsWindow_ExtractTo");
                }
            }
        }

        public string ViewSettingsWindow_UpdateOption
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ViewSettingsWindow_UpdateOption");
                }
            }
        }

        public string ViewSettingsWindow_None
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ViewSettingsWindow_None");
                }
            }
        }

        public string ViewSettingsWindow_AutoCheck
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ViewSettingsWindow_AutoCheck");
                }
            }
        }

        public string ViewSettingsWindow_AutoUpdate
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ViewSettingsWindow_AutoUpdate");
                }
            }
        }

        public string ViewSettingsWindow_Value
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ViewSettingsWindow_Value");
                }
            }
        }

        public string ViewSettingsWindow_Import
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ViewSettingsWindow_Import");
                }
            }
        }

        public string ViewSettingsWindow_Export
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ViewSettingsWindow_Export");
                }
            }
        }

        public string ViewSettingsWindow_Up
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ViewSettingsWindow_Up");
                }
            }
        }

        public string ViewSettingsWindow_Down
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ViewSettingsWindow_Down");
                }
            }
        }

        public string ViewSettingsWindow_Add
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ViewSettingsWindow_Add");
                }
            }
        }

        public string ViewSettingsWindow_Edit
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ViewSettingsWindow_Edit");
                }
            }
        }

        public string ViewSettingsWindow_Delete
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ViewSettingsWindow_Delete");
                }
            }
        }

        public string ViewSettingsWindow_Ok
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ViewSettingsWindow_Ok");
                }
            }
        }

        public string ViewSettingsWindow_Cancel
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ViewSettingsWindow_Cancel");
                }
            }
        }

        public string ViewSettingsWindow_Cut
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ViewSettingsWindow_Cut");
                }
            }
        }

        public string ViewSettingsWindow_Copy
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ViewSettingsWindow_Copy");
                }
            }
        }

        public string ViewSettingsWindow_Paste
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ViewSettingsWindow_Paste");
                }
            }
        }


        public string LinkWindow_Title
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("LinkWindow_Title");
                }
            }
        }

        public string LinkWindow_Download
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("LinkWindow_Download");
                }
            }
        }

        public string LinkWindow_Upload
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("LinkWindow_Upload");
                }
            }
        }

        public string LinkWindow_LinkerSignature
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("LinkWindow_LinkerSignature");
                }
            }
        }

        public string LinkWindow_TrustSignature
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("LinkWindow_TrustSignature");
                }
            }
        }

        public string LinkWindow_New
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("LinkWindow_New");
                }
            }
        }

        public string LinkWindow_Edit
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("LinkWindow_Edit");
                }
            }
        }

        public string LinkWindow_Delete
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("LinkWindow_Delete");
                }
            }
        }

        public string LinkWindow_Cut
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("LinkWindow_Cut");
                }
            }
        }

        public string LinkWindow_Copy
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("LinkWindow_Copy");
                }
            }
        }

        public string LinkWindow_Paste
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("LinkWindow_Paste");
                }
            }
        }

        public string LinkWindow_Ok
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("LinkWindow_Ok");
                }
            }
        }

        public string LinkWindow_Cancel
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("LinkWindow_Cancel");
                }
            }
        }


        public string VersionInformationWindow_Title
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("VersionInformationWindow_Title");
                }
            }
        }

        public string VersionInformationWindow_FileName
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("VersionInformationWindow_FileName");
                }
            }
        }

        public string VersionInformationWindow_Version
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("VersionInformationWindow_Version");
                }
            }
        }

        public string VersionInformationWindow_License
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("VersionInformationWindow_License");
                }
            }
        }

        public string VersionInformationWindow_Close
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("VersionInformationWindow_Close");
                }
            }
        }


        public string SignatureWindow_Title
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SignatureWindow_Title");
                }
            }
        }

        public string SignatureWindow_Signature
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SignatureWindow_Signature");
                }
            }
        }

        public string SignatureWindow_Ok
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SignatureWindow_Ok");
                }
            }
        }

        public string SignatureWindow_Cancel
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SignatureWindow_Cancel");
                }
            }
        }


        public string ConnectionControl_Uri
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionControl_Uri");
                }
            }
        }

        public string ConnectionControl_Priority
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionControl_Priority");
                }
            }
        }

        public string ConnectionControl_SentByteCount
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionControl_SentByteCount");
                }
            }
        }

        public string ConnectionControl_ReceivedByteCount
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionControl_ReceivedByteCount");
                }
            }
        }

        public string ConnectionControl_Name
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionControl_Name");
                }
            }
        }

        public string ConnectionControl_Value
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionControl_Value");
                }
            }
        }

        public string ConnectionControl_CreateConnectionCount
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionControl_CreateConnectionCount");
                }
            }
        }

        public string ConnectionControl_AcceptConnectionCount
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionControl_AcceptConnectionCount");
                }
            }
        }

        public string ConnectionControl_SurroundingNodeCount
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionControl_SurroundingNodeCount");
                }
            }
        }

        public string ConnectionControl_RelayBlockCount
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionControl_RelayBlockCount");
                }
            }
        }

        public string ConnectionControl_FreeSpace
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionControl_FreeSpace");
                }
            }
        }

        public string ConnectionControl_LockSpace
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionControl_LockSpace");
                }
            }
        }

        public string ConnectionControl_UsingSpace
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionControl_UsingSpace");
                }
            }
        }

        public string ConnectionControl_NodeCount
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionControl_NodeCount");
                }
            }
        }

        public string ConnectionControl_SeedCount
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionControl_SeedCount");
                }
            }
        }

        public string ConnectionControl_BlockCount
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionControl_BlockCount");
                }
            }
        }

        public string ConnectionControl_DownloadCount
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionControl_DownloadCount");
                }
            }
        }

        public string ConnectionControl_UploadCount
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionControl_UploadCount");
                }
            }
        }

        public string ConnectionControl_ShareCount
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionControl_ShareCount");
                }
            }
        }

        public string ConnectionControl_PushNodeCount
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionControl_PushNodeCount");
                }
            }
        }

        public string ConnectionControl_PushBlockLinkCount
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionControl_PushBlockLinkCount");
                }
            }
        }

        public string ConnectionControl_PushBlockRequestCount
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionControl_PushBlockRequestCount");
                }
            }
        }

        public string ConnectionControl_PushBlockCount
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionControl_PushBlockCount");
                }
            }
        }

        public string ConnectionControl_PushSeedRequestCount
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionControl_PushSeedRequestCount");
                }
            }
        }

        public string ConnectionControl_PushSeedCount
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionControl_PushSeedCount");
                }
            }
        }

        public string ConnectionControl_PullNodeCount
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionControl_PullNodeCount");
                }
            }
        }

        public string ConnectionControl_PullBlockLinkCount
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionControl_PullBlockLinkCount");
                }
            }
        }

        public string ConnectionControl_PullBlockRequestCount
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionControl_PullBlockRequestCount");
                }
            }
        }

        public string ConnectionControl_PullBlockCount
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionControl_PullBlockCount");
                }
            }
        }

        public string ConnectionControl_PullSeedRequestCount
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionControl_PullSeedRequestCount");
                }
            }
        }

        public string ConnectionControl_PullSeedCount
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionControl_PullSeedCount");
                }
            }
        }

        public string ConnectionControl_Copy
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionControl_Copy");
                }
            }
        }

        public string ConnectionControl_Paste
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ConnectionControl_Paste");
                }
            }
        }


        public string StoreControl_Download
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreControl_Download");
                }
            }
        }

        public string StoreControl_Upload
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreControl_Upload");
                }
            }
        }

        public string StoreControl_Library
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreControl_Library");
                }
            }
        }


        public string StoreDownloadControl_Name
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreDownloadControl_Name");
                }
            }
        }

        public string StoreDownloadControl_Signature
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreDownloadControl_Signature");
                }
            }
        }

        public string StoreDownloadControl_Length
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreDownloadControl_Length");
                }
            }
        }

        public string StoreDownloadControl_Keywords
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreDownloadControl_Keywords");
                }
            }
        }

        public string StoreDownloadControl_CreationTime
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreDownloadControl_CreationTime");
                }
            }
        }

        public string StoreDownloadControl_Comment
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreDownloadControl_Comment");
                }
            }
        }

        public string StoreDownloadControl_State
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreDownloadControl_State");
                }
            }
        }

        public string StoreDownloadControl_Id
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreDownloadControl_Id");
                }
            }
        }

        public string StoreDownloadControl_Upload
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreDownloadControl_Upload");
                }
            }
        }

        public string StoreDownloadControl_NewStore
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreDownloadControl_NewStore");
                }
            }
        }

        public string StoreDownloadControl_NewCategory
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreDownloadControl_NewCategory");
                }
            }
        }

        public string StoreDownloadControl_NewBox
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreDownloadControl_NewBox");
                }
            }
        }

        public string StoreDownloadControl_Edit
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreDownloadControl_Edit");
                }
            }
        }

        public string StoreDownloadControl_Delete
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreDownloadControl_Delete");
                }
            }
        }

        public string StoreDownloadControl_Cut
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreDownloadControl_Cut");
                }
            }
        }

        public string StoreDownloadControl_Copy
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreDownloadControl_Copy");
                }
            }
        }

        public string StoreDownloadControl_Paste
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreDownloadControl_Paste");
                }
            }
        }

        public string StoreDownloadControl_Import
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreDownloadControl_Import");
                }
            }
        }

        public string StoreDownloadControl_Export
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreDownloadControl_Export");
                }
            }
        }

        public string StoreDownloadControl_CopyInfo
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreDownloadControl_CopyInfo");
                }
            }
        }

        public string StoreDownloadControl_Download
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreDownloadControl_Download");
                }
            }
        }

        public string StoreDownloadControl_DigitalSignatureAnnulled_Message
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreDownloadControl_DigitalSignatureAnnulled_Message");
                }
            }
        }

        public string StoreDownloadControl_DigitalSignatureError_Message
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreDownloadControl_DigitalSignatureError_Message");
                }
            }
        }


        public string StoreUploadControl_Name
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreUploadControl_Name");
                }
            }
        }

        public string StoreUploadControl_Signature
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreUploadControl_Signature");
                }
            }
        }

        public string StoreUploadControl_Length
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreUploadControl_Length");
                }
            }
        }

        public string StoreUploadControl_Keywords
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreUploadControl_Keywords");
                }
            }
        }

        public string StoreUploadControl_CreationTime
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreUploadControl_CreationTime");
                }
            }
        }

        public string StoreUploadControl_Comment
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreUploadControl_Comment");
                }
            }
        }

        public string StoreUploadControl_State
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreUploadControl_State");
                }
            }
        }

        public string StoreUploadControl_Id
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreUploadControl_Id");
                }
            }
        }

        public string StoreUploadControl_Upload
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreUploadControl_Upload");
                }
            }
        }

        public string StoreUploadControl_NewStore
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreUploadControl_NewStore");
                }
            }
        }

        public string StoreUploadControl_NewCategory
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreUploadControl_NewCategory");
                }
            }
        }

        public string StoreUploadControl_NewBox
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreUploadControl_NewBox");
                }
            }
        }

        public string StoreUploadControl_Edit
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreUploadControl_Edit");
                }
            }
        }

        public string StoreUploadControl_Delete
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreUploadControl_Delete");
                }
            }
        }

        public string StoreUploadControl_Cut
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreUploadControl_Cut");
                }
            }
        }

        public string StoreUploadControl_Copy
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreUploadControl_Copy");
                }
            }
        }

        public string StoreUploadControl_Paste
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreUploadControl_Paste");
                }
            }
        }

        public string StoreUploadControl_Import
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreUploadControl_Import");
                }
            }
        }

        public string StoreUploadControl_Export
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreUploadControl_Export");
                }
            }
        }

        public string StoreUploadControl_CopyInfo
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreUploadControl_CopyInfo");
                }
            }
        }

        public string StoreUploadControl_Download
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreUploadControl_Download");
                }
            }
        }

        public string StoreUploadControl_DigitalSignatureAnnulled_Message
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreUploadControl_DigitalSignatureAnnulled_Message");
                }
            }
        }

        public string StoreUploadControl_DigitalSignatureError_Message
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreUploadControl_DigitalSignatureError_Message");
                }
            }
        }


        public string StoreCategorizeTreeItemEditWindow_Title
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreCategorizeTreeItemEditWindow_Title");
                }
            }
        }

        public string StoreCategorizeTreeItemEditWindow_Name
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreCategorizeTreeItemEditWindow_Name");
                }
            }
        }

        public string StoreCategorizeTreeItemEditWindow_Ok
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreCategorizeTreeItemEditWindow_Ok");
                }
            }
        }

        public string StoreCategorizeTreeItemEditWindow_Cancel
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("StoreCategorizeTreeItemEditWindow_Cancel");
                }
            }
        }


        public string LibraryControl_Name
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("LibraryControl_Name");
                }
            }
        }

        public string LibraryControl_Signature
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("LibraryControl_Signature");
                }
            }
        }

        public string LibraryControl_Length
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("LibraryControl_Length");
                }
            }
        }

        public string LibraryControl_Keywords
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("LibraryControl_Keywords");
                }
            }
        }

        public string LibraryControl_CreationTime
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("LibraryControl_CreationTime");
                }
            }
        }

        public string LibraryControl_Comment
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("LibraryControl_Comment");
                }
            }
        }

        public string LibraryControl_State
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("LibraryControl_State");
                }
            }
        }

        public string LibraryControl_Id
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("LibraryControl_Id");
                }
            }
        }

        public string LibraryControl_NewBox
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("LibraryControl_NewBox");
                }
            }
        }

        public string LibraryControl_Edit
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("LibraryControl_Edit");
                }
            }
        }

        public string LibraryControl_Delete
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("LibraryControl_Delete");
                }
            }
        }

        public string LibraryControl_Cut
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("LibraryControl_Cut");
                }
            }
        }

        public string LibraryControl_Copy
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("LibraryControl_Copy");
                }
            }
        }

        public string LibraryControl_Paste
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("LibraryControl_Paste");
                }
            }
        }

        public string LibraryControl_Import
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("LibraryControl_Import");
                }
            }
        }

        public string LibraryControl_Export
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("LibraryControl_Export");
                }
            }
        }

        public string LibraryControl_CopyInfo
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("LibraryControl_CopyInfo");
                }
            }
        }

        public string LibraryControl_Download
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("LibraryControl_Download");
                }
            }
        }

        public string LibraryControl_DigitalSignatureAnnulled_Message
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("LibraryControl_DigitalSignatureAnnulled_Message");
                }
            }
        }

        public string LibraryControl_DigitalSignatureError_Message
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("LibraryControl_DigitalSignatureError_Message");
                }
            }
        }


        public string SearchControl_Name
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchControl_Name");
                }
            }
        }

        public string SearchControl_Signature
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchControl_Signature");
                }
            }
        }

        public string SearchControl_Length
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchControl_Length");
                }
            }
        }

        public string SearchControl_Keywords
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchControl_Keywords");
                }
            }
        }

        public string SearchControl_CreationTime
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchControl_CreationTime");
                }
            }
        }

        public string SearchControl_Comment
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchControl_Comment");
                }
            }
        }

        public string SearchControl_State
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchControl_State");
                }
            }
        }

        public string SearchControl_Id
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchControl_Id");
                }
            }
        }

        public string SearchControl_New
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchControl_New");
                }
            }
        }

        public string SearchControl_Edit
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchControl_Edit");
                }
            }
        }

        public string SearchControl_Delete
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchControl_Delete");
                }
            }
        }

        public string SearchControl_Cut
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchControl_Cut");
                }
            }
        }

        public string SearchControl_Copy
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchControl_Copy");
                }
            }
        }

        public string SearchControl_CopyInfo
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchControl_CopyInfo");
                }
            }
        }

        public string SearchControl_Paste
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchControl_Paste");
                }
            }
        }

        public string SearchControl_Export
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchControl_Export");
                }
            }
        }

        public string SearchControl_DeleteCache
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchControl_DeleteCache");
                }
            }
        }

        public string SearchControl_DeleteShare
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchControl_DeleteShare");
                }
            }
        }

        public string SearchControl_DeleteDownload
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchControl_DeleteDownload");
                }
            }
        }

        public string SearchControl_DeleteUpload
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchControl_DeleteUpload");
                }
            }
        }

        public string SearchControl_DeleteDownloadHistory
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchControl_DeleteDownloadHistory");
                }
            }
        }

        public string SearchControl_DeleteUploadHistory
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchControl_DeleteUploadHistory");
                }
            }
        }

        public string SearchControl_Download
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchControl_Download");
                }
            }
        }

        public string SearchControl_Search
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchControl_Search");
                }
            }
        }

        public string SearchControl_SearchSignature
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchControl_SearchSignature");
                }
            }
        }

        public string SearchControl_SearchKeyword
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchControl_SearchKeyword");
                }
            }
        }

        public string SearchControl_SearchCreationTimeRange
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchControl_SearchCreationTimeRange");
                }
            }
        }

        public string SearchControl_SearchState
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchControl_SearchState");
                }
            }
        }

        public string SearchControl_Filter
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchControl_Filter");
                }
            }
        }

        public string SearchControl_FilterName
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchControl_FilterName");
                }
            }
        }

        public string SearchControl_FilterSignature
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchControl_FilterSignature");
                }
            }
        }

        public string SearchControl_FilterKeyword
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchControl_FilterKeyword");
                }
            }
        }

        public string SearchControl_FilterCreationTimeRange
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchControl_FilterCreationTimeRange");
                }
            }
        }

        public string SearchControl_FilterSeed
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchControl_FilterSeed");
                }
            }
        }


        public string DownloadControl_Name
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("DownloadControl_Name");
                }
            }
        }

        public string DownloadControl_Length
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("DownloadControl_Length");
                }
            }
        }

        public string DownloadControl_Priority
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("DownloadControl_Priority");
                }
            }
        }

        public string DownloadControl_Rank
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("DownloadControl_Rank");
                }
            }
        }

        public string DownloadControl_Rate
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("DownloadControl_Rate");
                }
            }
        }

        public string DownloadControl_Path
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("DownloadControl_Path");
                }
            }
        }

        public string DownloadControl_State
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("DownloadControl_State");
                }
            }
        }

        public string DownloadControl_Delete
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("DownloadControl_Delete");
                }
            }
        }

        public string DownloadControl_Copy
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("DownloadControl_Copy");
                }
            }
        }

        public string DownloadControl_CopyInfo
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("DownloadControl_CopyInfo");
                }
            }
        }

        public string DownloadControl_Paste
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("DownloadControl_Paste");
                }
            }
        }

        public string DownloadControl_Priority0
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("DownloadControl_Priority0");
                }
            }
        }

        public string DownloadControl_Priority1
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("DownloadControl_Priority1");
                }
            }
        }

        public string DownloadControl_Priority2
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("DownloadControl_Priority2");
                }
            }
        }

        public string DownloadControl_Priority3
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("DownloadControl_Priority3");
                }
            }
        }

        public string DownloadControl_Priority4
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("DownloadControl_Priority4");
                }
            }
        }

        public string DownloadControl_Priority5
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("DownloadControl_Priority5");
                }
            }
        }

        public string DownloadControl_Priority6
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("DownloadControl_Priority6");
                }
            }
        }

        public string DownloadControl_Reset
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("DownloadControl_Reset");
                }
            }
        }

        public string DownloadControl_DeleteComplete
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("DownloadControl_DeleteComplete");
                }
            }
        }


        public string DownloadState_Downloading
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("DownloadState_Downloading");
                }
            }
        }

        public string DownloadState_Decoding
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("DownloadState_Decoding");
                }
            }
        }

        public string DownloadState_ParityDecoding
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("DownloadState_ParityDecoding");
                }
            }
        }

        public string DownloadState_Completed
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("DownloadState_Completed");
                }
            }
        }

        public string DownloadState_Error
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("DownloadState_Error");
                }
            }
        }


        public string UploadControl_Name
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("UploadControl_Name");
                }
            }
        }

        public string UploadControl_Length
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("UploadControl_Length");
                }
            }
        }

        public string UploadControl_Priority
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("UploadControl_Priority");
                }
            }
        }

        public string UploadControl_Rank
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("UploadControl_Rank");
                }
            }
        }

        public string UploadControl_Rate
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("UploadControl_Rate");
                }
            }
        }

        public string UploadControl_Path
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("UploadControl_Path");
                }
            }
        }

        public string UploadControl_State
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("UploadControl_State");
                }
            }
        }

        public string UploadControl_Add
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("UploadControl_Add");
                }
            }
        }

        public string UploadControl_Delete
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("UploadControl_Delete");
                }
            }
        }

        public string UploadControl_Copy
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("UploadControl_Copy");
                }
            }
        }

        public string UploadControl_CopyInfo
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("UploadControl_CopyInfo");
                }
            }
        }

        public string UploadControl_Priority0
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("UploadControl_Priority0");
                }
            }
        }

        public string UploadControl_Priority1
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("UploadControl_Priority1");
                }
            }
        }

        public string UploadControl_Priority2
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("UploadControl_Priority2");
                }
            }
        }

        public string UploadControl_Priority3
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("UploadControl_Priority3");
                }
            }
        }

        public string UploadControl_Priority4
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("UploadControl_Priority4");
                }
            }
        }

        public string UploadControl_Priority5
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("UploadControl_Priority5");
                }
            }
        }

        public string UploadControl_Priority6
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("UploadControl_Priority6");
                }
            }
        }

        public string UploadControl_Reset
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("UploadControl_Reset");
                }
            }
        }

        public string UploadControl_DeleteComplete
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("UploadControl_DeleteComplete");
                }
            }
        }


        public string UploadState_ComputeHash
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("UploadState_ComputeHash");
                }
            }
        }

        public string UploadState_Encoding
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("UploadState_Encoding");
                }
            }
        }

        public string UploadState_ParityEncoding
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("UploadState_ParityEncoding");
                }
            }
        }

        public string UploadState_Uploading
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("UploadState_Uploading");
                }
            }
        }

        public string UploadState_Completed
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("UploadState_Completed");
                }
            }
        }

        public string UploadState_Error
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("UploadState_Error");
                }
            }
        }


        public string ShareControl_Name
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ShareControl_Name");
                }
            }
        }

        public string ShareControl_Path
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ShareControl_Path");
                }
            }
        }

        public string ShareControl_BlockCount
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ShareControl_BlockCount");
                }
            }
        }

        public string ShareControl_Add
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ShareControl_Add");
                }
            }
        }

        public string ShareControl_Delete
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ShareControl_Delete");
                }
            }
        }

        public string ShareControl_CheckExist
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("ShareControl_CheckExist");
                }
            }
        }


        public string UploadWindow_Title
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("UploadWindow_Title");
                }
            }
        }

        public string UploadWindow_Name
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("UploadWindow_Name");
                }
            }
        }

        public string UploadWindow_Keywords
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("UploadWindow_Keywords");
                }
            }
        }

        public string UploadWindow_Signature
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("UploadWindow_Signature");
                }
            }
        }

        public string UploadWindow_Comment
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("UploadWindow_Comment");
                }
            }
        }

        public string UploadWindow_List
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("UploadWindow_List");
                }
            }
        }

        public string UploadWindow_Path
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("UploadWindow_Path");
                }
            }
        }

        public string UploadWindow_Ok
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("UploadWindow_Ok");
                }
            }
        }

        public string UploadWindow_Cancel
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("UploadWindow_Cancel");
                }
            }
        }


        public string SearchState_Cache
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchState_Cache");
                }
            }
        }

        public string SearchState_Share
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchState_Share");
                }
            }
        }

        public string SearchState_Downloading
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchState_Downloading");
                }
            }
        }

        public string SearchState_Uploading
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchState_Uploading");
                }
            }
        }

        public string SearchState_Downloaded
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchState_Downloaded");
                }
            }
        }

        public string SearchState_Uploaded
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchState_Uploaded");
                }
            }
        }

        public string SearchState_Box
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchState_Box");
                }
            }
        }

        public string SearchState_Link
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchState_Link");
                }
            }
        }


        public string SearchItemEditWindow_Title
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchItemEditWindow_Title");
                }
            }
        }

        public string SearchItemEditWindow_Name
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchItemEditWindow_Name");
                }
            }
        }

        public string SearchItemEditWindow_NameRegex
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchItemEditWindow_NameRegex");
                }
            }
        }

        public string SearchItemEditWindow_Signature
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchItemEditWindow_Signature");
                }
            }
        }

        public string SearchItemEditWindow_Keyword
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchItemEditWindow_Keyword");
                }
            }
        }

        public string SearchItemEditWindow_CreationTimeRange
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchItemEditWindow_CreationTimeRange");
                }
            }
        }

        public string SearchItemEditWindow_LengthRange
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchItemEditWindow_LengthRange");
                }
            }
        }

        public string SearchItemEditWindow_Seed
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchItemEditWindow_Seed");
                }
            }
        }

        public string SearchItemEditWindow_SearchState
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchItemEditWindow_SearchState");
                }
            }
        }

        public string SearchItemEditWindow_Contains
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchItemEditWindow_Contains");
                }
            }
        }

        public string SearchItemEditWindow_IsIgnoreCase
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchItemEditWindow_IsIgnoreCase");
                }
            }
        }

        public string SearchItemEditWindow_Value
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchItemEditWindow_Value");
                }
            }
        }

        public string SearchItemEditWindow_Min
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchItemEditWindow_Min");
                }
            }
        }

        public string SearchItemEditWindow_Max
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchItemEditWindow_Max");
                }
            }
        }

        public string SearchItemEditWindow_Up
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchItemEditWindow_Up");
                }
            }
        }

        public string SearchItemEditWindow_Down
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchItemEditWindow_Down");
                }
            }
        }

        public string SearchItemEditWindow_Add
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchItemEditWindow_Add");
                }
            }
        }

        public string SearchItemEditWindow_Edit
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchItemEditWindow_Edit");
                }
            }
        }

        public string SearchItemEditWindow_Delete
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchItemEditWindow_Delete");
                }
            }
        }

        public string SearchItemEditWindow_Ok
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchItemEditWindow_Ok");
                }
            }
        }

        public string SearchItemEditWindow_Cancel
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchItemEditWindow_Cancel");
                }
            }
        }

        public string SearchItemEditWindow_Cut
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchItemEditWindow_Cut");
                }
            }
        }

        public string SearchItemEditWindow_Copy
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchItemEditWindow_Copy");
                }
            }
        }

        public string SearchItemEditWindow_Paste
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchItemEditWindow_Paste");
                }
            }
        }

        public string SearchItemEditWindow_SearchState_Cache
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchItemEditWindow_SearchState_Cache");
                }
            }
        }

        public string SearchItemEditWindow_SearchState_Share
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchItemEditWindow_SearchState_Share");
                }
            }
        }

        public string SearchItemEditWindow_SearchState_Downloading
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchItemEditWindow_SearchState_Downloading");
                }
            }
        }

        public string SearchItemEditWindow_SearchState_Uploading
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchItemEditWindow_SearchState_Uploading");
                }
            }
        }

        public string SearchItemEditWindow_SearchState_Downloaded
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchItemEditWindow_SearchState_Downloaded");
                }
            }
        }

        public string SearchItemEditWindow_SearchState_Uploaded
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchItemEditWindow_SearchState_Uploaded");
                }
            }
        }

        public string SearchItemEditWindow_SearchState_Box
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchItemEditWindow_SearchState_Box");
                }
            }
        }

        public string SearchItemEditWindow_SearchState_Link
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SearchItemEditWindow_SearchState_Link");
                }
            }
        }


        public string SeedEditWindow_Title
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SeedEditWindow_Title");
                }
            }
        }

        public string SeedEditWindow_Name
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SeedEditWindow_Name");
                }
            }
        }

        public string SeedEditWindow_Keywords
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SeedEditWindow_Keywords");
                }
            }
        }

        public string SeedEditWindow_Signature
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SeedEditWindow_Signature");
                }
            }
        }

        public string SeedEditWindow_Comment
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SeedEditWindow_Comment");
                }
            }
        }

        public string SeedEditWindow_Ok
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SeedEditWindow_Ok");
                }
            }
        }

        public string SeedEditWindow_Cancel
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("SeedEditWindow_Cancel");
                }
            }
        }


        public string BoxEditWindow_Title
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("BoxEditWindow_Title");
                }
            }
        }

        public string BoxEditWindow_Name
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("BoxEditWindow_Name");
                }
            }
        }

        public string BoxEditWindow_Signature
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("BoxEditWindow_Signature");
                }
            }
        }

        public string BoxEditWindow_Comment
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("BoxEditWindow_Comment");
                }
            }
        }

        public string BoxEditWindow_Ok
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("BoxEditWindow_Ok");
                }
            }
        }

        public string BoxEditWindow_Cancel
        {
            get
            {
                lock (this.ThisLock)
                {
                    return this.Translate("BoxEditWindow_Cancel");
                }
            }
        }

        #endregion

        #region IThisLock

        public object ThisLock
        {
            get
            {
                return _thisLock;
            }
        }

        #endregion
    }
}
