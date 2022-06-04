using System;
using System.IO;

namespace CommentGetForVCI
{
    class SHOWROOMConfigClass
    {
        //コンポーネント使用時に使用
        private MainWindow MAIN_COMPONENTS = null;
        //SHOWROOM関係設定値保存先
        private string CONFIG_DIRECTORY = null;
        private const string CONFIG_FILE_NAME = "SHOWROOM.config";


        //SHOWROOM関係設定値
        public string[] config = new string[] { };


        //設定番号
        public enum ConfigNum
        {
            //VCIにコメント送信するか否か
            VCI_ScriptSend_Comment
            //新規コメント時スクロール必要か否か
            , Scroll_Comment
            //標準コメント送るか否か
            , DefaultCommentSend
            //標準コメント表示するか否か
            , DefaultCommentShow
            //テロップコメント送るか否か
            , TelopCommentSend
            //テロップコメント表示するか否か
            , TelopCommentShow
            //無料ギフトコメント送るか否か
            , FreeGiftCommentSend
            //無料ギフトコメント表示するか否か
            , FreeGiftCommentShow
            //有料ギフトコメント送るか否か
            , GiftCommentSend
            //有料ギフトコメント表示するか否か
            , GiftCommentShow
            //訪問コメント送るか否か
            , VisitCommentSend
            //訪問コメント表示するか否か
            , VisitCommentShow
            //支援ポイントコメント送るか否か
            , PointCommentSend
            //支援ポイントコメント表示するか否か
            , PointCommentShow
            //カウントコメント送るか否か
            , CountCommentSend
            //カウントコメント表示するか否か
            , CountCommentShow
            //コメントのログとるかか否か
            , CommentLog
            //for等のカウント用の終了時使用
            , END
        };

        //デフォルト内容設定番号に対応した内容
        public readonly string[] defaultConfig = {
            //VCIにコメント送信するか否か
            "1"
            //新規コメント時スクロール必要か否か
            ,"1"
            //標準コメント送るか否か
            ,"1"
            //標準コメント表示するか否か
            ,"1"
            //テロップコメント送るか否か
            ,"0"
            //テロップコメント表示するか否か
            ,"0"
            //無料ギフトコメント送るか否か
            ,"0"
            //無料ギフトコメント表示するか否か
            ,"0"
            //有料ギフトコメント送るか否か
            ,"0"
            //有料ギフトコメント表示するか否か
            ,"0"
            //訪問コメント送るか否か
            ,"0"
            //訪問コメント表示するか否か
            ,"0"
            //支援ポイントコメント送るか否か
            ,"0"
            //支援ポイントコメント表示するか否か
            ,"0"
            //カウントコメント送るか否か
            ,"0"
            //カウントコメント表示するか否か
            ,"0"
            //コメントのログとるかか否か
            ,"0"
        };

        //内部config情報反映
        private void configReflection()
        {
            //VCIにコメント送信するか否か
            if (config[(int)ConfigNum.VCI_ScriptSend_Comment] == "0") MAIN_COMPONENTS.configSHOWROOMCommentSend.IsChecked = false;
            else MAIN_COMPONENTS.configSHOWROOMCommentSend.IsChecked = true;
            //番組接続時全てのコメントを送るコメント送るか否か
            if (config[(int)ConfigNum.Scroll_Comment] == "0") MAIN_COMPONENTS.configSHOWROOMScrollnable.IsChecked = false;
            else MAIN_COMPONENTS.configSHOWROOMScrollnable.IsChecked = true;
            //標準コメント送るか否か
            if (config[(int)ConfigNum.DefaultCommentSend] == "0") MAIN_COMPONENTS.configSHOWROOMDefaultComment.IsChecked = false;
            else MAIN_COMPONENTS.configSHOWROOMDefaultComment.IsChecked = true;
            //標準コメント表示するか否か
            if (config[(int)ConfigNum.DefaultCommentShow] == "0") MAIN_COMPONENTS.configSHOWROOMDefaultComment2.IsChecked = false;
            else MAIN_COMPONENTS.configSHOWROOMDefaultComment2.IsChecked = true;
            //テロップコメント送るか否か
            if (config[(int)ConfigNum.TelopCommentSend] == "0") MAIN_COMPONENTS.configSHOWROOMTelopComment.IsChecked = false;
            else MAIN_COMPONENTS.configSHOWROOMTelopComment.IsChecked = true;
            //テロップコメント表示するか否か
            if (config[(int)ConfigNum.TelopCommentShow] == "0") MAIN_COMPONENTS.configSHOWROOMTelopComment2.IsChecked = false;
            else MAIN_COMPONENTS.configSHOWROOMTelopComment2.IsChecked = true;
            //無料ギフトコメント送るか否か
            if (config[(int)ConfigNum.FreeGiftCommentSend] == "0") MAIN_COMPONENTS.configSHOWROOMFreeGiftComment.IsChecked = false;
            else MAIN_COMPONENTS.configSHOWROOMFreeGiftComment.IsChecked = true;
            //無料ギフトコメント表示するか否か
            if (config[(int)ConfigNum.FreeGiftCommentShow] == "0") MAIN_COMPONENTS.configSHOWROOMFreeGiftComment2.IsChecked = false;
            else MAIN_COMPONENTS.configSHOWROOMFreeGiftComment2.IsChecked = true;
            //有料ギフトコメント送るか否か
            if (config[(int)ConfigNum.GiftCommentSend] == "0") MAIN_COMPONENTS.configSHOWROOMGiftComment.IsChecked = false;
            else MAIN_COMPONENTS.configSHOWROOMGiftComment.IsChecked = true;
            //有料ギフトコメント表示するか否か
            if (config[(int)ConfigNum.GiftCommentShow] == "0") MAIN_COMPONENTS.configSHOWROOMGiftComment2.IsChecked = false;
            else MAIN_COMPONENTS.configSHOWROOMGiftComment2.IsChecked = true;
            //訪問コメント送るか否か
            if (config[(int)ConfigNum.VisitCommentSend] == "0") MAIN_COMPONENTS.configSHOWROOMVisitComment.IsChecked = false;
            else MAIN_COMPONENTS.configSHOWROOMVisitComment.IsChecked = true;
            //訪問コメント表示するか否か
            if (config[(int)ConfigNum.VisitCommentShow] == "0") MAIN_COMPONENTS.configSHOWROOMVisitComment2.IsChecked = false;
            else MAIN_COMPONENTS.configSHOWROOMVisitComment2.IsChecked = true;
            //支援ポイントコメント送るか否か
            if (config[(int)ConfigNum.PointCommentSend] == "0") MAIN_COMPONENTS.configSHOWROOMPointComment.IsChecked = false;
            else MAIN_COMPONENTS.configSHOWROOMPointComment.IsChecked = true;
            //支援ポイントコメント表示するか否か
            if (config[(int)ConfigNum.PointCommentShow] == "0") MAIN_COMPONENTS.configSHOWROOMPointComment2.IsChecked = false;
            else MAIN_COMPONENTS.configSHOWROOMPointComment2.IsChecked = true;
            //カウントコメント送るか否か
            if (config[(int)ConfigNum.CountCommentSend] == "0") MAIN_COMPONENTS.configSHOWROOMCountComment.IsChecked = false;
            else MAIN_COMPONENTS.configSHOWROOMCountComment.IsChecked = true;
            //カウントコメント表示するか否か
            if (config[(int)ConfigNum.CountCommentSend] == "0") MAIN_COMPONENTS.configSHOWROOMCountComment2.IsChecked = false;
            else MAIN_COMPONENTS.configSHOWROOMCountComment2.IsChecked = true;
            //コメントのログとるかか否か
            if (config[(int)ConfigNum.CommentLog] == "0") MAIN_COMPONENTS.configSHOWROOMCommentLog.IsChecked = false;
            else MAIN_COMPONENTS.configSHOWROOMCommentLog.IsChecked = true;
        }

        //設定のイニシャライズ
        public SHOWROOMConfigClass(string directory, MainWindow mainComponents)
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