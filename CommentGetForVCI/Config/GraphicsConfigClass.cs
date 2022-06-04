using System;
using System.IO;
using System.Threading.Tasks;

namespace CommentGetForVCI
{
    class GraphicsConfigClass
    {
        //コンポーネント使用時に使用
        private MainWindow MAIN_COMPONENTS = null;
        //表示関係設定値保存先
        private string CONFIG_DIRECTORY = null;
        private const string CONFIG_FILE_NAME = "graphics.config";


        //ニコ生関係設定値
        public string[] config = new string[] { };
        //コメントコラム
        private enum CommentColumNumConfig { CommentNum, FixHandle, UserID, CommentTime, Comment };


        //設定番号
        public enum ConfigNum
        {
            //全体サイズ横幅
            WindowSize_Wide
            //全体サイズ縦幅
            , WindowSize_High
            //ニコ生コラム1サイズ
            , NikoLiveColumSize_CommentNum
            //ニコ生コラム2サイズ
            , NikoLiveColumSize_FixHandle
            //ニコ生コラム3サイズ
            , NikoLiveColumSize_UserID
            //ニコ生コラム4サイズ
            , NikoLiveColumSize_CommentTime
            //ニコ生コラム5サイズ
            , NikoLiveColumSize_Comment
            //SHOWROOMコラム1サイズ
            , SHOWROOMLiveColumSize_CommentNum
            //SHOWROOMコラム2サイズ
            , SHOWROOMLiveColumSize_FixHandle
            //SHOWROOMコラム3サイズ
            , SHOWROOMLiveColumSize_UserID
            //SHOWROOMコラム4サイズ
            , SHOWROOMLiveColumSize_CommentTime
            //SHOWROOMコラム5サイズ
            , SHOWROOMLiveColumSize_Comment
            //for等のカウント用の終了時使用
            , END
        };

        //デフォルト内容設定番号に対応した内容
        private readonly string[] defaultConfig = {
            //全体サイズ横幅
            "804"
            //全体サイズ縦幅
            ,"521"
            //ニコ生コラム1サイズ
            ,"70"
            //ニコ生コラム2サイズ
            ,"105"
            //ニコ生コラム3サイズ
            ,"190"
            //ニコ生コラム4サイズ
            ,"125"
            //ニコ生コラム5サイズ
            ,"280"
            //SHOWROOMコラム1サイズ
            ,"70"
            //SHOWROOMコラム2サイズ
            ,"105"
            //SHOWROOMコラム3サイズ
            ,"190"
            //SHOWROOMコラム4サイズ
            ,"125"
            //SHOWROOMコラム5サイズ
            ,"280"
        };

        //内部config情報反映
        private void configReflection()
        {
            MAIN_COMPONENTS.Width = double.Parse(config[(int)ConfigNum.WindowSize_Wide]);
            MAIN_COMPONENTS.Height = double.Parse(config[(int)ConfigNum.WindowSize_High]);
            //タブサイズ更新
            MAIN_COMPONENTS.commentColumGrid.Columns[(int)CommentColumNumConfig.CommentNum].Width = int.Parse(config[(int)ConfigNum.NikoLiveColumSize_CommentNum]);
            MAIN_COMPONENTS.commentColumGrid.Columns[(int)CommentColumNumConfig.FixHandle].Width = int.Parse(config[(int)ConfigNum.NikoLiveColumSize_FixHandle]);
            MAIN_COMPONENTS.commentColumGrid.Columns[(int)CommentColumNumConfig.UserID].Width = int.Parse(config[(int)ConfigNum.NikoLiveColumSize_UserID]);
            MAIN_COMPONENTS.commentColumGrid.Columns[(int)CommentColumNumConfig.CommentTime].Width = int.Parse(config[(int)ConfigNum.NikoLiveColumSize_CommentTime]);
            MAIN_COMPONENTS.commentColumGrid.Columns[(int)CommentColumNumConfig.Comment].Width = int.Parse(config[(int)ConfigNum.NikoLiveColumSize_Comment]);
            //タブサイズ更新
            MAIN_COMPONENTS.commentSHOWROOMColumGrid.Columns[(int)CommentColumNumConfig.CommentNum].Width = int.Parse(config[(int)ConfigNum.SHOWROOMLiveColumSize_CommentNum]);
            MAIN_COMPONENTS.commentSHOWROOMColumGrid.Columns[(int)CommentColumNumConfig.FixHandle].Width = int.Parse(config[(int)ConfigNum.SHOWROOMLiveColumSize_FixHandle]);
            MAIN_COMPONENTS.commentSHOWROOMColumGrid.Columns[(int)CommentColumNumConfig.UserID].Width = int.Parse(config[(int)ConfigNum.SHOWROOMLiveColumSize_UserID]);
            MAIN_COMPONENTS.commentSHOWROOMColumGrid.Columns[(int)CommentColumNumConfig.CommentTime].Width = int.Parse(config[(int)ConfigNum.SHOWROOMLiveColumSize_CommentTime]);
            MAIN_COMPONENTS.commentSHOWROOMColumGrid.Columns[(int)CommentColumNumConfig.Comment].Width = int.Parse(config[(int)ConfigNum.SHOWROOMLiveColumSize_Comment]);
        }

        //設定のイニシャライズ
        public GraphicsConfigClass(string directory, MainWindow mainComponents)
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
        public bool configFolderExistsCheck()
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

        //内部config記録
        private void configGraphicWrite()
        {
            //ファイルデフォルト値設定
            using (StreamWriter write = new StreamWriter(CONFIG_DIRECTORY + "\\" + CONFIG_FILE_NAME, false))
            {
                //画面サイズ
                write.WriteLine(ConfigNum.WindowSize_Wide + ":" + MAIN_COMPONENTS.Width.ToString());
                write.WriteLine(ConfigNum.WindowSize_High + ":" + MAIN_COMPONENTS.Height.ToString());
                //コメ番タブ
                write.WriteLine(ConfigNum.NikoLiveColumSize_CommentNum + ":" + MAIN_COMPONENTS.commentColumGrid.Columns[(int)CommentColumNumConfig.CommentNum].Width.ToString());
                //コテハンタブ
                write.WriteLine(ConfigNum.NikoLiveColumSize_FixHandle + ":" + MAIN_COMPONENTS.commentColumGrid.Columns[(int)CommentColumNumConfig.FixHandle].Width.ToString());
                //ユーザーIDタブ
                write.WriteLine(ConfigNum.NikoLiveColumSize_UserID + ":" + MAIN_COMPONENTS.commentColumGrid.Columns[(int)CommentColumNumConfig.UserID].Width.ToString());
                //コメント時間タブ
                write.WriteLine(ConfigNum.NikoLiveColumSize_CommentTime + ":" + MAIN_COMPONENTS.commentColumGrid.Columns[(int)CommentColumNumConfig.CommentTime].Width.ToString());
                //コメントタブ
                write.WriteLine(ConfigNum.NikoLiveColumSize_Comment + ":" + MAIN_COMPONENTS.commentColumGrid.Columns[(int)CommentColumNumConfig.Comment].Width.ToString());
                //コメ番タブ
                write.WriteLine(ConfigNum.SHOWROOMLiveColumSize_CommentNum + ":" + MAIN_COMPONENTS.commentSHOWROOMColumGrid.Columns[(int)CommentColumNumConfig.CommentNum].Width.ToString());
                //コテハンタブ
                write.WriteLine(ConfigNum.SHOWROOMLiveColumSize_FixHandle + ":" + MAIN_COMPONENTS.commentSHOWROOMColumGrid.Columns[(int)CommentColumNumConfig.FixHandle].Width.ToString());
                //ユーザーIDタブ
                write.WriteLine(ConfigNum.SHOWROOMLiveColumSize_UserID + ":" + MAIN_COMPONENTS.commentSHOWROOMColumGrid.Columns[(int)CommentColumNumConfig.UserID].Width.ToString());
                //コメント時間タブ
                write.WriteLine(ConfigNum.SHOWROOMLiveColumSize_CommentTime + ":" + MAIN_COMPONENTS.commentSHOWROOMColumGrid.Columns[(int)CommentColumNumConfig.CommentTime].Width.ToString());
                //コメントタブ
                write.WriteLine(ConfigNum.SHOWROOMLiveColumSize_Comment + ":" + MAIN_COMPONENTS.commentSHOWROOMColumGrid.Columns[(int)CommentColumNumConfig.Comment].Width.ToString());
            }
        }

        //サイズ状態更新処理
        public async Task sizeCheckUpdate()
        {
            while (true)
            {
                bool[] flg = new bool[] { };
                //フォルダ存在確認(無ければデフォルト値作成済み)
                if (configFolderExistsCheck())
                {
                    //ファイル存在しなければ作成
                    if (!File.Exists(CONFIG_DIRECTORY + "\\" + CONFIG_FILE_NAME))
                    {
                        //グラフィックconfigファイル(デフォルト値)作成
                        defaultConfigFileCreate();
                    }
                    else
                    {
                        //ファイル読み込み
                        using (StreamReader reader = new StreamReader(CONFIG_DIRECTORY + "\\" + CONFIG_FILE_NAME))
                        {
                            //あるなし見て、フラグによりけり再書き込み必要
                            //1行ずつ確認更新
                            string line = null;
                            while ((line = reader.ReadLine()) != null)
                            {
                                //検索
                                //画面サイズ
                                if (line.StartsWith(ConfigNum.WindowSize_Wide + ":"))
                                    if (MAIN_COMPONENTS.Width.ToString() != line.Substring((ConfigNum.WindowSize_Wide + ":").Length))
                                    {
                                        Array.Resize(ref flg, flg.Length + 1);
                                        flg[flg.Length - 1] = true;
                                    }
                                if (line.StartsWith(ConfigNum.WindowSize_High + ":"))
                                    if (MAIN_COMPONENTS.Height.ToString() != line.Substring((ConfigNum.WindowSize_High + ":").Length))
                                    {
                                        Array.Resize(ref flg, flg.Length + 1);
                                        flg[flg.Length - 1] = true;
                                    }
                                //コメ番タブ
                                if (line.StartsWith(ConfigNum.NikoLiveColumSize_CommentNum + ":"))
                                    if (MAIN_COMPONENTS.commentColumGrid.Columns[(int)CommentColumNumConfig.CommentNum].Width.ToString() != line.Substring((ConfigNum.NikoLiveColumSize_CommentNum + ":").Length))
                                    {
                                        Array.Resize(ref flg, flg.Length + 1);
                                        flg[flg.Length - 1] = true;
                                    }
                                //コテハンタブ
                                if (line.StartsWith(ConfigNum.NikoLiveColumSize_FixHandle + ":"))
                                    if (MAIN_COMPONENTS.commentColumGrid.Columns[(int)CommentColumNumConfig.FixHandle].Width.ToString() != line.Substring((ConfigNum.NikoLiveColumSize_FixHandle + ":").Length))
                                    {
                                        Array.Resize(ref flg, flg.Length + 1);
                                        flg[flg.Length - 1] = true;
                                    }
                                //ユーザーIDタブ
                                if (line.StartsWith(ConfigNum.NikoLiveColumSize_UserID + ":"))
                                    if (MAIN_COMPONENTS.commentColumGrid.Columns[(int)CommentColumNumConfig.UserID].Width.ToString() != line.Substring((ConfigNum.NikoLiveColumSize_UserID + ":").Length))
                                    {
                                        Array.Resize(ref flg, flg.Length + 1);
                                        flg[flg.Length - 1] = true;
                                    }
                                //コメント時間タブ
                                if (line.StartsWith(ConfigNum.NikoLiveColumSize_CommentTime + ":"))
                                    if (MAIN_COMPONENTS.commentColumGrid.Columns[(int)CommentColumNumConfig.CommentTime].Width.ToString() != line.Substring((ConfigNum.NikoLiveColumSize_CommentTime + ":").Length))
                                    {
                                        Array.Resize(ref flg, flg.Length + 1);
                                        flg[flg.Length - 1] = true;
                                    }
                                //コメントタブ
                                if (line.StartsWith(ConfigNum.NikoLiveColumSize_Comment + ":"))
                                    if (MAIN_COMPONENTS.commentColumGrid.Columns[(int)CommentColumNumConfig.Comment].Width.ToString() != line.Substring((ConfigNum.NikoLiveColumSize_Comment + ":").Length))
                                    {
                                        Array.Resize(ref flg, flg.Length + 1);
                                        flg[flg.Length - 1] = true;
                                    }
                                //コメ番タブ
                                if (line.StartsWith(ConfigNum.SHOWROOMLiveColumSize_CommentNum + ":"))
                                    if (MAIN_COMPONENTS.commentSHOWROOMColumGrid.Columns[(int)CommentColumNumConfig.CommentNum].Width.ToString() != line.Substring((ConfigNum.SHOWROOMLiveColumSize_CommentNum + ":").Length))
                                    {
                                        Array.Resize(ref flg, flg.Length + 1);
                                        flg[flg.Length - 1] = true;
                                    }
                                //コテハンタブ
                                if (line.StartsWith(ConfigNum.SHOWROOMLiveColumSize_FixHandle + ":"))
                                    if (MAIN_COMPONENTS.commentSHOWROOMColumGrid.Columns[(int)CommentColumNumConfig.FixHandle].Width.ToString() != line.Substring((ConfigNum.SHOWROOMLiveColumSize_FixHandle + ":").Length))
                                    {
                                        Array.Resize(ref flg, flg.Length + 1);
                                        flg[flg.Length - 1] = true;
                                    }
                                //ユーザーIDタブ
                                if (line.StartsWith(ConfigNum.SHOWROOMLiveColumSize_UserID + ":"))
                                    if (MAIN_COMPONENTS.commentSHOWROOMColumGrid.Columns[(int)CommentColumNumConfig.UserID].Width.ToString() != line.Substring((ConfigNum.SHOWROOMLiveColumSize_UserID + ":").Length))
                                    {
                                        Array.Resize(ref flg, flg.Length + 1);
                                        flg[flg.Length - 1] = true;
                                    }
                                //コメント時間タブ
                                if (line.StartsWith(ConfigNum.SHOWROOMLiveColumSize_CommentTime + ":"))
                                    if (MAIN_COMPONENTS.commentSHOWROOMColumGrid.Columns[(int)CommentColumNumConfig.CommentTime].Width.ToString() != line.Substring((ConfigNum.SHOWROOMLiveColumSize_CommentTime + ":").Length))
                                    {
                                        Array.Resize(ref flg, flg.Length + 1);
                                        flg[flg.Length - 1] = true;
                                    }
                                //コメントタブ
                                if (line.StartsWith(ConfigNum.SHOWROOMLiveColumSize_Comment + ":"))
                                    if (MAIN_COMPONENTS.commentSHOWROOMColumGrid.Columns[(int)CommentColumNumConfig.Comment].Width.ToString() != line.Substring((ConfigNum.SHOWROOMLiveColumSize_Comment + ":").Length))
                                    {
                                        Array.Resize(ref flg, flg.Length + 1);
                                        flg[flg.Length - 1] = true;
                                    }
                            }
                        }
                    }
                }
                if (flg.Length < (int)ConfigNum.END) configGraphicWrite();
                await Task.Delay(500);
            }
        }
    }
}