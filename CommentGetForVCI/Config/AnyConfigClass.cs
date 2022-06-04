using System;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CommentGetForVCI
{
    class AnyConfigClass
    {
        //ユーザー名\\desktopまでのディレクトリ
        private static string DIRECTRY_USER_DESKTOP_PASS = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
        //ユーザー名までのディレクトリ
        private static string DIRECTRY_USER_PASS = DIRECTRY_USER_DESKTOP_PASS.Substring(0, DIRECTRY_USER_DESKTOP_PASS.LastIndexOf("\\"));
        //コンポーネント使用時に使用
        private MainWindow MAIN_COMPONENTS = null;
        //表示関係設定値保存先
        private string CONFIG_DIRECTORY = null;
        private const string CONFIG_FILE_NAME = "any.config";


        //ニコ生関係設定値
        public string[] config = new string[] { };


        //設定番号
        public enum ConfigNum
        {
            //VCIフォルダ保存場所
            VCI_ScriptPosition
            //コメント保存先ディレクトリ
            , CommentDirectory
            //コテハン保存先ディレクトリ
            , FixHandleDirectory
            //VCIにコメント送信するか否か
            , VCI_ScriptSend_Comment
            //VCIにコメント送信時、先頭に〔@放送先:〕を付けるか否か
            , VCI_ScriptSend_Comment_Broadcast
            //VCIにコメント送信時、先頭に〔@コテハン:〕を付けるか否か
            , VCI_ScriptSend_Comment_PutFixHandle
            //要素取得時コテハン必要か否か
            , Cpy_Comment_FixHandle
            //要素取得時ユーザーID必要か否か
            , Cpy_Comment_UserID
            //要素取得時コメント必要か否か
            , Cpy_Comment_Comment
            //VCIにコメント送信時タイミング制御
            , VCI_ScriptSend_Comment_Timming
            //VCIにコメント一斉に幾つ分送信するか
            , VCI_ScriptSend_Comment_Max
            //for等のカウント用の終了時使用
            , END
        };

        //デフォルト内容設定番号に対応した内容
        public readonly string[] defaultConfig = {
            //VCIフォルダ保存場所
             DIRECTRY_USER_PASS + "\\LocalLow\\infiniteloop Co,Ltd\\VirtualCast\\EmbeddedScriptWorkspace\\コメント取得\\main.lua"
            //コメント保存先ディレクトリ
            ,Directory.GetCurrentDirectory() + "\\GetCommentTxt"
            //コテハン保存先ディレクトリ
            ,Directory.GetCurrentDirectory() + "\\GetCommentFixHandle"
            //VCIにコメント送信するか否か
            ,"1"
            //VCIにコメント送信時、先頭に〔@放送先:〕を付けるか否か
            ,"1"
            //VCIにコメント送信時、先頭に〔@コテハン:〕を付けるか否か
            ,"1"
            //要素取得時コテハン必要か否か
            ,"1"
            //要素取得時ユーザーID必要か否か
            ,"1"
            //要素取得時コメント必要か否か
            ,"1"
            //VCIにコメント送信時タイミング制御
            ,"5"
        //VCIにコメント一斉に幾つ分送信するか
            ,"3"
        };

        //内部config情報反映
        private void configReflection()
        {
            //VCIフォルダ保存場所
            MAIN_COMPONENTS.configVCIFilePos2.Text = config[(int)ConfigNum.VCI_ScriptPosition];
            //コテハン保存先ディレクトリ
            MAIN_COMPONENTS.configCommentFilePos2.Text = config[(int)ConfigNum.CommentDirectory];
            //コテハン保存先ディレクトリ
            MAIN_COMPONENTS.configFixHandleFilePos2.Text = config[(int)ConfigNum.FixHandleDirectory];
            //VCIにコメント送信するか否か
            if (config[(int)ConfigNum.VCI_ScriptSend_Comment] == "0") MAIN_COMPONENTS.configVCICommentSend.IsChecked = false;
            else MAIN_COMPONENTS.configVCICommentSend.IsChecked = true;
            //VCIにコメント送信時、先頭に〔@放送先:〕を付けるか否か
            if (config[(int)ConfigNum.VCI_ScriptSend_Comment_Broadcast] == "0") MAIN_COMPONENTS.configVCICommentSendAddBroadcast.IsChecked = false;
            else MAIN_COMPONENTS.configVCICommentSendAddBroadcast.IsChecked = true;
            //VCIにコメント送信時、先頭に〔@コテハン:〕を付けるか否か
            if (config[(int)ConfigNum.VCI_ScriptSend_Comment_PutFixHandle] == "0") MAIN_COMPONENTS.configVCICommentSendAddFixHandle.IsChecked = false;
            else MAIN_COMPONENTS.configVCICommentSendAddFixHandle.IsChecked = true;
            //要素取得時コテハン必要か否か
            if (config[(int)ConfigNum.Cpy_Comment_FixHandle] == "0") MAIN_COMPONENTS.configDoubleClickGetFixHandle.IsChecked = false;
            else MAIN_COMPONENTS.configDoubleClickGetFixHandle.IsChecked = true;
            //要素取得時ユーザーID必要か否か
            if (config[(int)ConfigNum.Cpy_Comment_UserID] == "0") MAIN_COMPONENTS.configDoubleClickGetUserID.IsChecked = false;
            else MAIN_COMPONENTS.configDoubleClickGetUserID.IsChecked = true;
            //要素取得時コメント必要か否か
            if (config[(int)ConfigNum.Cpy_Comment_Comment] == "0") MAIN_COMPONENTS.configDoubleClickGetComment.IsChecked = false;
            else MAIN_COMPONENTS.configDoubleClickGetComment.IsChecked = true;
            //VCIにコメント送信時タイミング制御
            MAIN_COMPONENTS.configVCIUpdateTimming2.SelectedIndex = int.Parse(config[(int)ConfigNum.VCI_ScriptSend_Comment_Timming]);
        }

        //設定のイニシャライズ
        public AnyConfigClass(string directory, MainWindow mainComponents)
        {
            //config保存先ディレクトリ名
            CONFIG_DIRECTORY = directory;
            //コンポーネント保存
            MAIN_COMPONENTS = mainComponents;
            //設定値初期化
            Array.Resize(ref config, defaultConfig.Length);
            //デフォルト値代入
            configDefaultSet();
            //configファイル読み込み
            readConfigFile();
        }

        //デフォルト値代入
        private void configDefaultSet()
        {
            //設定あるぶん回してデフォルト値保持
            for (int count = 0; count < (int)ConfigNum.END; count++)
            {
                config[count] = defaultConfig[count];
            }
        }

        //configファイル読み込み
        private void readConfigFile()
        {
            //フォルダ存在確認(無ければデフォルト値作成済み)
            if (configFolderExistsCheck())
            {
                //ファイル存在しなければ作成
                if (!File.Exists(CONFIG_DIRECTORY + "\\" + CONFIG_FILE_NAME))
                {
                    //configファイル(デフォルト値)作成
                    defaultConfigFileCreate();
                }
                else
                {
                    //configファイル反映
                    configFileReflection();
                }
            }
        }

        //configフォルダ存在確認
        private bool configFolderExistsCheck()
        {
            //ディレクトリ存在しなければ作成
            if (!Directory.Exists(CONFIG_DIRECTORY))
            {
                //ディレクトリ作成
                Directory.CreateDirectory(CONFIG_DIRECTORY);
                //configファイル(デフォルト値)作成
                defaultConfigFileCreate();
                return false;
            }
            return true;
        }

        //configファイル(デフォルト値)作成
        public void defaultConfigFileCreate()
        {
            //ファイルデフォルト値設定
            using (StreamWriter write = new StreamWriter(CONFIG_DIRECTORY + "\\" + CONFIG_FILE_NAME, false))
            {
                //設定配列調整
                Array.Resize(ref config, (int)ConfigNum.END);
                //設定あるぶん回して保存
                for (int count = 0; count < (int)ConfigNum.END; count++) write.WriteLine(count.ToString() + ":" + defaultConfig[count]);
            }
            //configファイル反映
            configFileReflection();
        }

        //configファイル反映
        private void configFileReflection()
        {
            //ファイル読み込み
            using (StreamReader reader = new StreamReader(CONFIG_DIRECTORY + "\\" + CONFIG_FILE_NAME))
            {
                //1行ずつ確認更新
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    //設定あるぶん回して検索/保持
                    for (int count = 0; count < (int)ConfigNum.END; count++)
                    {
                        //指定番号に対応した内容を読み取る
                        if (line.StartsWith(count.ToString() + ":"))
                            config[count] = line.Substring((count.ToString() + ":").Length);
                    }
                }
            }
            //内部config情報反映
            configReflection();
        }

        //config更新
        public void configUpdate(string key, string value, bool reflection)
        {
            //ファイル存在しなければ作成
            if (!File.Exists(CONFIG_DIRECTORY + "\\" + CONFIG_FILE_NAME))
            {
                //configファイル(デフォルト値)作成
                defaultConfigFileCreate();
            }
            else
            {
                //configファイル更新書き換え
                configFileChangeWrite(key, value, reflection);
            }
        }

        //configファイル更新書き換え
        private void configFileChangeWrite(string key, string value, bool reflection)
        {
            //全体保存
            string all_line = "";
            //ファイル読み込み
            using (StreamReader reader = new StreamReader(CONFIG_DIRECTORY + "\\" + CONFIG_FILE_NAME))
            {
                //1行ずつ確認更新
                string line = null;
                int count = 0;
                int line_count = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    //更新
                    if (line.StartsWith(key)) { }
                    //そのまま
                    else
                    {
                        //改行処理
                        if ((line_count) != 0) all_line += "\n";
                        all_line += line;
                        ++line_count;
                    }
                    ++count;
                }
                if (count > line_count)
                {
                    if (count != 0) all_line += "\n";
                    all_line += key + value;
                }
            }
            //ファイル書き込み
            using (StreamWriter write = new StreamWriter(CONFIG_DIRECTORY + "\\" + CONFIG_FILE_NAME, false)) write.Write(all_line);

            //内部config情報反映するか否か
            if (reflection) configReflection();
        }
    }
}
