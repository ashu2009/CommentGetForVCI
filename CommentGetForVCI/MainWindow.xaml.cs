using System;
using System.Windows;
using System.IO;

namespace CommentGetForVCI
{
    public partial class MainWindow : Window
    {
        //ロード完了後か否か
        private bool loadEnd = false;
        //ConfigClass初期化
        private ConfigClass clsConfig = null;
        //ConnectClass初期化
        private ConnectClass clsConnect = null;

        public MainWindow()
        {
            //コンポーネント初期化
            InitializeComponent();
        }

        //コンポーネント読み込み時
        private void コメント取得forVCI_Load(object sender, EventArgs e)
        {
            //設定初期化
            clsConfig = new ConfigClass(this);
            //接続初期化
            clsConnect = new ConnectClass(this);
            //接続状態表示更新
            clsConnect.stateCheckUpdate();
            //コメントVCI更新
            clsConnect.commentVCIUpdate();
            //サイズ状態更新
            clsConfig.sizeCheckUpdate();
            //ロード完了
            loadEnd = true;
        }

        //コンポーネント削除時
        private void コメント取得forVCI_Close(object sender, EventArgs e)
        {
            //luaファイル位置
            string path = clsConfig.CLS_Any_Config.config[(int)AnyConfigClass.ConfigNum.VCI_ScriptPosition];
            //lua書き換え
            if (Directory.Exists(path.Substring(0, path.LastIndexOf("\\"))))
            {
                using (StreamWriter write = new StreamWriter(path, false))
                {
                    //画面サイズ
                    write.Write("if vci.assets.IsMine then end");
                }
            }
        }

        //接続
        private void conectBTN_Click(object sender, EventArgs e)
        {
            //接続挑戦
            clsConnect.TryConnect();
        }

        //切断
        private async void disconectBTN_Click(object sender, EventArgs e)
        {
            //切断挑戦
            clsConnect.TryDisonnect();
        }

        //一般--------------------------------------------------------------------------------------------
        //VCIフォルダ位置ダイアログ選択
        private void configVCIFilePos3_Click(object sender, EventArgs e)
        {
            //設定用データ
            string[] anyConfigData = clsConfig.CLS_Any_Config.config;
            //デフォルト値設定用データ
            string[] anyConfigDefault = clsConfig.CLS_Any_Config.defaultConfig;
            //設定用データ
            int choseConfigEnum = (int)AnyConfigClass.ConfigNum.VCI_ScriptPosition;
            //設定用データ
            int choseKindEnum = (int)ConfigClass.ConfigKind.Any;
            //ダイアログ新規作成
            var ofDialog = new Microsoft.Win32.OpenFileDialog() { FileName = "main.lua", Filter = "All Files|*.*", CheckFileExists = false };
            //開くフォルダを指定する(現在の設定場所かデフォルト)
            ofDialog.InitialDirectory = anyConfigData[choseConfigEnum];
            //ファイル存在しなければデフォルト使用
            if (!File.Exists(ofDialog.InitialDirectory)) ofDialog.InitialDirectory = anyConfigDefault[choseConfigEnum];
            //ダイアログのタイトルを指定する
            ofDialog.Title = "VCIスクリプトフォルダ選択";
            //ダイアログを表示し、okで決定と設定
            if ((bool)ofDialog.ShowDialog())
            {
                //ファイル名更新
                anyConfigData[choseConfigEnum] = ofDialog.FileName;
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", ofDialog.FileName, true);
            }
        }

        //コメントフォルダ位置ダイアログ選択
        private void configCommentFilePos3_Click(object sender, EventArgs e)
        {
            //設定用データ
            string[] anyConfigData = clsConfig.CLS_Any_Config.config;
            //デフォルト値設定用データ
            string[] anyConfigDefault = clsConfig.CLS_Any_Config.defaultConfig;
            //設定用データ
            int choseConfigEnum = (int)AnyConfigClass.ConfigNum.CommentDirectory;
            //設定用データ
            int choseKindEnum = (int)ConfigClass.ConfigKind.Any;
            //ダイアログ新規作成
            var ofDialog = new Microsoft.Win32.OpenFileDialog() { FileName = "anyComentsFile", Filter = "Folder|.", CheckFileExists = false };
            //開くフォルダを指定する(現在の設定場所かデフォルト)
            ofDialog.InitialDirectory = anyConfigData[choseConfigEnum];
            //ファイル存在しなければデフォルト使用
            if (!File.Exists(ofDialog.InitialDirectory)) ofDialog.InitialDirectory = anyConfigDefault[choseConfigEnum];
            //ダイアログのタイトルを指定する
            ofDialog.Title = "コメント保存先フォルダ選択";
            //ダイアログを表示し、okで決定と設定
            if ((bool)ofDialog.ShowDialog())
            {
                //ファイル名更新
                anyConfigData[choseConfigEnum] = ofDialog.FileName.Substring(0, ofDialog.FileName.LastIndexOf("\\"));
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], true);
            }
        }

        //コテハンフォルダ位置ダイアログ選択
        private void configFixHandleFilePos3_Click(object sender, EventArgs e)
        {
            //設定用データ
            string[] anyConfigData = clsConfig.CLS_Any_Config.config;
            //デフォルト値設定用データ
            string[] anyConfigDefault = clsConfig.CLS_Any_Config.defaultConfig;
            //設定用データ
            int choseConfigEnum = (int)AnyConfigClass.ConfigNum.FixHandleDirectory;
            //設定用データ
            int choseKindEnum = (int)ConfigClass.ConfigKind.Any;
            //ダイアログ新規作成
            var ofDialog = new Microsoft.Win32.OpenFileDialog() { FileName = "anyFixHandleFile", Filter = "Folder|.", CheckFileExists = false };
            //開くフォルダを指定する(現在の設定場所かデフォルト)
            ofDialog.InitialDirectory = anyConfigData[choseConfigEnum];
            //ファイル存在しなければデフォルト使用
            if (!File.Exists(ofDialog.InitialDirectory)) ofDialog.InitialDirectory = anyConfigDefault[choseConfigEnum];
            //ダイアログのタイトルを指定する
            ofDialog.Title = "コテハン保存先フォルダ選択";
            //ダイアログを表示し、okで決定と設定
            if ((bool)ofDialog.ShowDialog())
            {
                //ファイル名更新
                anyConfigData[choseConfigEnum] = ofDialog.FileName.Substring(0, ofDialog.FileName.LastIndexOf("\\"));
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], true);
            }
        }

        //VCIにコメント送信許可するか否か
        private void configVCICommentSend_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_Any_Config.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_Any_Config.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)AnyConfigClass.ConfigNum.VCI_ScriptSend_Comment;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.Any;
                //チェックあったらば"1"
                if ((bool)configVCICommentSend.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }

        //VCIにコメント送信時、先頭に〔@コテハン:〕を付けるか否か
        private void configVCICommentSendAddBroadcast_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_Any_Config.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_Any_Config.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)AnyConfigClass.ConfigNum.VCI_ScriptSend_Comment_Broadcast;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.Any;
                //チェックあったらば"1"
                if ((bool)configVCICommentSendAddBroadcast.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }

        //VCIにコメント送信時、先頭に〔@コテハン:〕を付けるか否か
        private void configVCICommentSendAddFixHandle_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_Any_Config.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_Any_Config.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)AnyConfigClass.ConfigNum.VCI_ScriptSend_Comment_PutFixHandle;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.Any;
                //チェックあったらば"1"
                if ((bool)configVCICommentSendAddFixHandle.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }

        //要素取得時コテハン必要か否か
        private void configDoubleClickGetFixHandle_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_Any_Config.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_Any_Config.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)AnyConfigClass.ConfigNum.Cpy_Comment_FixHandle;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.Any;
                //チェックあったらば"1"
                if ((bool)configDoubleClickGetFixHandle.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }

        //要素取得時ユーザーID必要か否か
        private void configDoubleClickGetUserID_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_Any_Config.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_Any_Config.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)AnyConfigClass.ConfigNum.Cpy_Comment_UserID;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.Any;
                //チェックあったらば"1"
                if ((bool)configDoubleClickGetUserID.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }

        //要素取得時コメント必要か否か
        private void configDoubleClickGetComment_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_Any_Config.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_Any_Config.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)AnyConfigClass.ConfigNum.Cpy_Comment_Comment;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.Any;
                //チェックあったらば"1"
                if ((bool)configDoubleClickGetComment.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }

        //VCIにコメント送信時タイミング制御
        private void configVCIUpdateTimming2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_Any_Config.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_Any_Config.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)AnyConfigClass.ConfigNum.VCI_ScriptSend_Comment_Timming;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.Any;
                if (configVCIUpdateTimming2.SelectedIndex > -1)
                {
                    //ファイル名更新
                    anyConfigData[choseConfigEnum] = configVCIUpdateTimming2.SelectedIndex.ToString();
                    //configファイルの更新
                    clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
                }
            }
        }

        //VCIにコメント一斉に幾つ分送信するか
        private void configVCICommentSendMax2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_Any_Config.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_Any_Config.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)AnyConfigClass.ConfigNum.VCI_ScriptSend_Comment_Max;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.Any;
                if (configVCIUpdateTimming2.SelectedIndex > -1)
                {
                    //ファイル名更新
                    anyConfigData[choseConfigEnum] = configVCICommentSendMax2.SelectedIndex.ToString();
                    //configファイルの更新
                    clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
                }
            }
        }
        //一般--------------------------------------------------------------------------------------------

        //ニコ生--------------------------------------------------------------------------------------------
        //VCIにコメント送信許可するか否か
        private void configNikoLiveCommentSend_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_N_LiveConfig.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_N_LiveConfig.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)NikoLiveConfigClass.ConfigNum.VCI_ScriptSend_Comment;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.N_Live;
                //チェックあったらば"1"
                if ((bool)configVCICommentSend.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }

        //過去コメント取得数選択
        private void configPrevGetCount2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_N_LiveConfig.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_N_LiveConfig.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)NikoLiveConfigClass.ConfigNum.PreviousCommentMax;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.N_Live;
                if (configPrevGetCount2.SelectedIndex > -1)
                {
                    //ファイル名更新
                    anyConfigData[choseConfigEnum] = configPrevGetCount2.SelectedIndex.ToString();
                    //configファイルの更新
                    clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
                }
            }
        }

        //ニコ生番組接続時全てのコメントを送るか否か
        private void configNikoLiveGetAllCommentSend_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_N_LiveConfig.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_N_LiveConfig.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)NikoLiveConfigClass.ConfigNum.GetAllCommentSendSend;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.N_Live;
                //チェックあったらば"1"
                if ((bool)configNikoLiveGetAllCommentSend.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }

        //ニコ生新規コメント時スクロール必要か否か
        private void configNikoLiveScrollEnable_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_N_LiveConfig.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_N_LiveConfig.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)NikoLiveConfigClass.ConfigNum.Scroll_Comment;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.N_Live;
                //チェックあったらば"1"
                if ((bool)configNikoLiveScrollEnable.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }

        //登録されたコテハンを使用する
        private void configNikoLiveFixHandleUse_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_N_LiveConfig.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_N_LiveConfig.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)NikoLiveConfigClass.ConfigNum.RegisterFixHandleUse;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.N_Live;
                //チェックあったらば"1"
                if ((bool)configNikoLiveFixHandleUse.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }

        //@...でコテハン(...)を登録する
        private void configNikoLiveFixHandleAdd_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_N_LiveConfig.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_N_LiveConfig.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)NikoLiveConfigClass.ConfigNum.FixHandleRegister;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.N_Live;
                //チェックあったらば"1"
                if ((bool)configNikoLiveFixHandleAdd.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }

        //通常コメント送るか否か
        private void configNikoLiveDefaultCommentSend_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_N_LiveConfig.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_N_LiveConfig.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)NikoLiveConfigClass.ConfigNum.FixHandleRegister;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.N_Live;
                //チェックあったらば"1"
                if ((bool)configNikoLiveDefaultCommentSend.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }

        //エモーションコメント送るか否か
        private void configNikoLiveCommentEmotionSend_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_N_LiveConfig.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_N_LiveConfig.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)NikoLiveConfigClass.ConfigNum.CommentEmotionSend;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.N_Live;
                //チェックあったらば"1"
                if ((bool)configNikoLiveCommentEmotionSend.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }

        //放送リクエストコメント送るか否か
        private void configNikoLiveCommentRequestSend_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_N_LiveConfig.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_N_LiveConfig.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)NikoLiveConfigClass.ConfigNum.CommentRequestSend;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.N_Live;
                //チェックあったらば"1"
                if ((bool)configNikoLiveCommentRequestSend.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }

        //延長のコメントを送るか否か
        private void configNikoLiveCommentTimeExtensionSend_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_N_LiveConfig.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_N_LiveConfig.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)NikoLiveConfigClass.ConfigNum.CommentFreeGiftSend;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.N_Live;
                //チェックあったらば"1"
                if ((bool)configNikoLiveCommentTimeExtensionSend.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }

        //...が好きな人がきたコメント送るか否か
        private void configNikoLiveCommentLikeComeSend_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_N_LiveConfig.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_N_LiveConfig.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)NikoLiveConfigClass.ConfigNum.CommentLikeComeSend;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.N_Live;
                //チェックあったらば"1"
                if ((bool)configNikoLiveCommentLikeComeSend.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }

        //広告のコメントを送るか否か
        private void configNikoLiveCommentAdvertisementSend_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_N_LiveConfig.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_N_LiveConfig.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)NikoLiveConfigClass.ConfigNum.CommentAdvertisementSend;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.N_Live;
                //チェックあったらば"1"
                if ((bool)configNikoLiveCommentAdvertisementSend.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }

        //無料ギフトコメント送るか否か
        private void configNikoLiveCommentFreeGiftSend_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_N_LiveConfig.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_N_LiveConfig.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)NikoLiveConfigClass.ConfigNum.CommentFreeGiftSend;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.N_Live;
                //チェックあったらば"1"
                if ((bool)configNikoLiveCommentFreeGiftSend.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }

        //ギフトコメント送るか否か
        private void configNikoLiveCommentGiftSend_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_N_LiveConfig.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_N_LiveConfig.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)NikoLiveConfigClass.ConfigNum.CommentGiftSend;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.N_Live;
                //チェックあったらば"1"
                if ((bool)configNikoLiveCommentGiftSend.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }

        //ログをとるか否か
        private void configNikoLiveCommentLog_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_N_LiveConfig.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_N_LiveConfig.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)NikoLiveConfigClass.ConfigNum.CommentLog;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.N_Live;
                //チェックあったらば"1"
                if ((bool)configNikoCommentLog.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }

        //コメント生データ使うかか否か
        private void configNikoLiveCommentLaw_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_N_LiveConfig.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_N_LiveConfig.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)NikoLiveConfigClass.ConfigNum.CommentLog;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.N_Live;
                //チェックあったらば"1"
                if ((bool)configNikoCommentLaw.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }
        //ニコ生--------------------------------------------------------------------------------------------

        //SHOWROOM--------------------------------------------------------------------------------------------
        //VCIにコメント送信許可するか否か
        private void configSHOWROOMCommentSend_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_SHOWROOM_Config.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_SHOWROOM_Config.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)SHOWROOMConfigClass.ConfigNum.VCI_ScriptSend_Comment;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.SHOWROOM;
                //チェックあったらば"1"
                if ((bool)configVCICommentSend.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }

        //SHOWROOM新規コメント時スクロール必要か否か
        private void configSHOWROOMScrollnable_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_SHOWROOM_Config.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_SHOWROOM_Config.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)SHOWROOMConfigClass.ConfigNum.Scroll_Comment;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.SHOWROOM;
                //チェックあったらば"1"
                if ((bool)configSHOWROOMScrollnable.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }

        //通常コメントを送るか否か
        private void configSHOWROOMDefaultComment_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_SHOWROOM_Config.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_SHOWROOM_Config.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)SHOWROOMConfigClass.ConfigNum.DefaultCommentShow;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.SHOWROOM;
                //チェックあったらば"1"
                if ((bool)configSHOWROOMDefaultComment.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }

        //通常コメントを表示するか否か
        private void configSHOWROOMDefaultComment2_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_SHOWROOM_Config.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_SHOWROOM_Config.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)SHOWROOMConfigClass.ConfigNum.DefaultCommentShow;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.SHOWROOM;
                //チェックあったらば"1"
                if ((bool)configSHOWROOMDefaultComment2.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }

        //テロップコメントを送るか否か
        private void configSHOWROOMTelopComment_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_SHOWROOM_Config.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_SHOWROOM_Config.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)SHOWROOMConfigClass.ConfigNum.TelopCommentSend;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.SHOWROOM;
                //チェックあったらば"1"
                if ((bool)configSHOWROOMTelopComment.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }

        //テロップコメントを表示するか否か
        private void configSHOWROOMTelopComment2_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_SHOWROOM_Config.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_SHOWROOM_Config.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)SHOWROOMConfigClass.ConfigNum.TelopCommentShow;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.SHOWROOM;
                //チェックあったらば"1"
                if ((bool)configSHOWROOMTelopComment2.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }

        //無料ギフトコメント送るか否か
        private void configSHOWROOMFreeGiftComment_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_SHOWROOM_Config.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_SHOWROOM_Config.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)SHOWROOMConfigClass.ConfigNum.FreeGiftCommentSend;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.SHOWROOM;
                //チェックあったらば"1"
                if ((bool)configSHOWROOMFreeGiftComment.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }

        //無料ギフトコメント表示するか否か
        private void configSHOWROOMFreeGiftComment2_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_SHOWROOM_Config.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_SHOWROOM_Config.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)SHOWROOMConfigClass.ConfigNum.FreeGiftCommentShow;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.SHOWROOM;
                //チェックあったらば"1"
                if ((bool)configSHOWROOMFreeGiftComment2.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }

        //有料ギフトコメント送るか否か
        private void configSHOWROOMGiftComment_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_SHOWROOM_Config.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_SHOWROOM_Config.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)SHOWROOMConfigClass.ConfigNum.GiftCommentSend;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.SHOWROOM;
                //チェックあったらば"1"
                if ((bool)configSHOWROOMGiftComment.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }

        //有料ギフトコメント表示するか否か
        private void configSHOWROOMGiftComment2_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_SHOWROOM_Config.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_SHOWROOM_Config.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)SHOWROOMConfigClass.ConfigNum.GiftCommentShow;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.SHOWROOM;
                //チェックあったらば"1"
                if ((bool)configSHOWROOMGiftComment2.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }

        //訪問コメント送るか否か
        private void configSHOWROOMVisitComment_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_SHOWROOM_Config.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_SHOWROOM_Config.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)SHOWROOMConfigClass.ConfigNum.VisitCommentSend;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.SHOWROOM;
                //チェックあったらば"1"
                if ((bool)configSHOWROOMVisitComment.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }

        //訪問コメント表示するか否か
        private void configSHOWROOMVisitComment2_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_SHOWROOM_Config.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_SHOWROOM_Config.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)SHOWROOMConfigClass.ConfigNum.VisitCommentShow;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.SHOWROOM;
                //チェックあったらば"1"
                if ((bool)configSHOWROOMVisitComment2.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }

        //支援ポイントコメント送るか否か
        private void configSHOWROOMPointComment_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_SHOWROOM_Config.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_SHOWROOM_Config.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)SHOWROOMConfigClass.ConfigNum.PointCommentSend;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.SHOWROOM;
                //チェックあったらば"1"
                if ((bool)configSHOWROOMPointComment.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }

        //支援ポイントコメント表示するか否か
        private void configSHOWROOMPointComment2_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_SHOWROOM_Config.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_SHOWROOM_Config.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)SHOWROOMConfigClass.ConfigNum.PointCommentShow;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.SHOWROOM;
                //チェックあったらば"1"
                if ((bool)configSHOWROOMPointComment2.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }

        //カウントコメント送るか否か
        private void configSHOWROOMCountComment_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_SHOWROOM_Config.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_SHOWROOM_Config.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)SHOWROOMConfigClass.ConfigNum.CountCommentSend;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.SHOWROOM;
                //チェックあったらば"1"
                if ((bool)configSHOWROOMCountComment.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }

        //カウントコメント表示するか否か
        private void configSHOWROOMCountComment2_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_SHOWROOM_Config.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_SHOWROOM_Config.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)SHOWROOMConfigClass.ConfigNum.CountCommentShow;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.SHOWROOM;
                //チェックあったらば"1"
                if ((bool)configSHOWROOMCountComment2.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }

        //ログをとるか否か
        private void configSHOWROOMCommentLog_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (loadEnd)
            {
                //設定用データ
                string[] anyConfigData = clsConfig.CLS_SHOWROOM_Config.config;
                //デフォルト値設定用データ
                string[] anyConfigDefault = clsConfig.CLS_SHOWROOM_Config.defaultConfig;
                //設定用データ
                int choseConfigEnum = (int)SHOWROOMConfigClass.ConfigNum.CommentLog;
                //設定用データ
                int choseKindEnum = (int)ConfigClass.ConfigKind.SHOWROOM;
                //チェックあったらば"1"
                if ((bool)configSHOWROOMCommentLog.IsChecked) anyConfigData[choseConfigEnum] = "1";
                else anyConfigData[choseConfigEnum] = "0";
                //configファイルの更新
                clsConfig.changeConfigWrite(choseKindEnum, choseConfigEnum.ToString() + ":", anyConfigData[choseConfigEnum], false);
            }
        }
        //SHOWROOM--------------------------------------------------------------------------------------------
    }
}