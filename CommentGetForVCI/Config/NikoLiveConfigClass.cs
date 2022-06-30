using System;
using System.IO;

namespace CommentGetForVCI
{
    //ニコ生設定のイニシャライズ
    class NikoLiveConfigClass
    {
        //コンポーネント使用時に使用
        private MainWindow MAIN_COMPONENTS = null;
        //ニコ生関係設定値保存先
        private string CONFIG_DIRECTORY = null;
        private const string CONFIG_FILE_NAME = "NikoLive.config";


        //ニコ生関係設定値
        public string[] config = new string[] { };


        //設定番号
        public enum ConfigNum
        {
            //VCIにコメント送信するか否か
            VCI_ScriptSend_Comment
            //過去コメント何個取得するか
            , PreviousCommentMax
            //番組接続時全てのコメントを送るコメント送るか否か
            , GetAllCommentSendSend
            //新規コメント時スクロール必要か否か
            , Scroll_Comment
            //登録されたコテハンを使用する
            , RegisterFixHandleUse
            //@...でコテハン(...)を登録する
            , FixHandleRegister
            //通常コメント送るか否か
            , DefaultCommentSend
            //エモーションコメント送るか否か
            , CommentEmotionSend
            //放送リクエストコメント送るか否か
            , CommentRequestSend
            //延長のコメントを送るコメント送るか否か
            , CommentTimeExpectionSend
            //...が好きな人がきたコメント送るか否か
            , CommentLikeComeSend
            //広告関連のコメントを送るコメント送るか否か
            , CommentAdvertisementSend
            //無料ギフトコメント送るか否か
            , CommentFreeGiftSend
            //ギフトコメント送るか否か
            , CommentGiftSend
            //コメントのログとるかか否か
            , CommentLog
            //コメント生データ使うかか否か
            , CommentLaw
            //for等のカウント用の終了時使用
            , END
        };

        //デフォルト内容設定番号に対応した内容
        public readonly string[] defaultConfig = {
            //VCIにコメント送信するか否か
            "1"
            //過去コメント何個取得するか
            ,"0"
            //番組接続時全てのコメントを送るコメント送るか否か
            ,"0"
            //新規コメント時スクロール必要か否か
            ,"1"
            //登録されたコテハンを使用する
            ,"1"
            //@...でコテハン(...)を登録する
            ,"1"
            //通常コメント送るか否か
            ,"1"
            //エモーションコメント送るか否か
            ,"1"
            //放送リクエストコメント送るか否か
            ,"1"
            //...が好きな人がきたコメント送るか否か
            ,"1"
            //無料ギフトコメント送るか否か
            ,"1"
            //ギフトコメント送るか否か
            ,"1"
            //延長のコメントを送るコメント送るか否か
            ,"1"
            //広告関連のコメントを送るコメント送るか否か
            ,"1"
            //コメントのログとるかか否か
            ,"0"
            //コメント生データ使うかか否か
            , "0"
        };

        //内部config情報反映
        private void configReflection()
        {
            //VCIにコメント送信するか否か
            if (config[(int)ConfigNum.VCI_ScriptSend_Comment] == "0") MAIN_COMPONENTS.configNikoLiveCommentSend.IsChecked = false;
            else MAIN_COMPONENTS.configNikoLiveCommentSend.IsChecked = true;
            //VCIにコメント送信時タイミング制御
            MAIN_COMPONENTS.configPrevGetCount2.SelectedIndex = int.Parse(config[(int)ConfigNum.PreviousCommentMax]);
            //番組接続時全てのコメントを送るコメント送るか否か
            if (config[(int)ConfigNum.GetAllCommentSendSend] == "0") MAIN_COMPONENTS.configNikoLiveGetAllCommentSend.IsChecked = false;
            else MAIN_COMPONENTS.configNikoLiveGetAllCommentSend.IsChecked = true;
            //新規コメント時スクロール必要か否か
            if (config[(int)ConfigNum.Scroll_Comment] == "0") MAIN_COMPONENTS.configNikoLiveScrollEnable.IsChecked = false;
            else MAIN_COMPONENTS.configNikoLiveScrollEnable.IsChecked = true;
            //登録されたコテハンを使用する
            if (config[(int)ConfigNum.RegisterFixHandleUse] == "0") MAIN_COMPONENTS.configNikoLiveFixHandleUse.IsChecked = false;
            else MAIN_COMPONENTS.configNikoLiveFixHandleUse.IsChecked = true;
            //@...でコテハン(...)を登録する
            if (config[(int)ConfigNum.FixHandleRegister] == "0") MAIN_COMPONENTS.configNikoLiveFixHandleAdd.IsChecked = false;
            else MAIN_COMPONENTS.configNikoLiveFixHandleAdd.IsChecked = true;
            //通常コメント送るか否か
            if (config[(int)ConfigNum.DefaultCommentSend] == "0") MAIN_COMPONENTS.configNikoLiveDefaultCommentSend.IsChecked = false;
            else MAIN_COMPONENTS.configNikoLiveDefaultCommentSend.IsChecked = true;
            //エモーションコメント送るか否か
            if (config[(int)ConfigNum.CommentEmotionSend] == "0") MAIN_COMPONENTS.configNikoLiveCommentEmotionSend.IsChecked = false;
            else MAIN_COMPONENTS.configNikoLiveCommentEmotionSend.IsChecked = true;
            //放送リクエストコメント送るか否か
            if (config[(int)ConfigNum.CommentRequestSend] == "0") MAIN_COMPONENTS.configNikoLiveCommentRequestSend.IsChecked = false;
            else MAIN_COMPONENTS.configNikoLiveCommentRequestSend.IsChecked = true;
            //延長のコメントを送るコメント送るか否か
            if (config[(int)ConfigNum.CommentTimeExpectionSend] == "0") MAIN_COMPONENTS.configNikoLiveCommentTimeExtensionSend.IsChecked = false;
            else MAIN_COMPONENTS.configNikoLiveCommentTimeExtensionSend.IsChecked = true;
            //...が好きな人がきたコメント送るか否か
            if (config[(int)ConfigNum.CommentLikeComeSend] == "0") MAIN_COMPONENTS.configNikoLiveCommentLikeComeSend.IsChecked = false;
            else MAIN_COMPONENTS.configNikoLiveCommentLikeComeSend.IsChecked = true;
            //広告関連のコメントを送るコメント送るか否か
            if (config[(int)ConfigNum.CommentAdvertisementSend] == "0") MAIN_COMPONENTS.configNikoLiveCommentAdvertisementSend.IsChecked = false;
            else MAIN_COMPONENTS.configNikoLiveCommentAdvertisementSend.IsChecked = true;
            //無料ギフトコメント送るか否か
            if (config[(int)ConfigNum.CommentFreeGiftSend] == "0") MAIN_COMPONENTS.configNikoLiveCommentFreeGiftSend.IsChecked = false;
            else MAIN_COMPONENTS.configNikoLiveCommentFreeGiftSend.IsChecked = true;
            //ギフトコメント送るか否か
            if (config[(int)ConfigNum.CommentGiftSend] == "0") MAIN_COMPONENTS.configNikoLiveCommentGiftSend.IsChecked = false;
            else MAIN_COMPONENTS.configNikoLiveCommentGiftSend.IsChecked = true;
            //コメントのログとるかか否か
            if (config[(int)ConfigNum.CommentLog] == "0") MAIN_COMPONENTS.configNikoCommentLog.IsChecked = false;
            else MAIN_COMPONENTS.configNikoCommentLog.IsChecked = true;
            //コメント生データ使うかか否か
            if (config[(int)ConfigNum.CommentLaw] == "0") MAIN_COMPONENTS.configNikoCommentLaw.IsChecked = false;
            else MAIN_COMPONENTS.configNikoCommentLaw.IsChecked = true;
        }

        //設定のイニシャライズ
        public NikoLiveConfigClass(string directory, MainWindow mainComponents)
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