using System.IO;
using System.Threading.Tasks;

namespace CommentGetForVCI
{
    //設定ファイルクラス
    class ConfigClass
    {
        //各種設定ファイル保存先ディレクトリ
        private string CONFIG_DIRECTORY = Directory.GetCurrentDirectory() + "\\Config";
        //ニコ生コンフィグ関係
        public NikoLiveConfigClass CLS_N_LiveConfig = null;
        //SHOWROOMコンフィグ関係
        public SHOWROOMConfigClass CLS_SHOWROOM_Config = null;
        //表示関係コンフィグ関係
        public GraphicsConfigClass CLS_Graphics_Config = null;
        //コンフィグ関係
        public AnyConfigClass CLS_Any_Config = null;

        //設定種類
        public enum ConfigKind { N_Live, SHOWROOM, Graphics, Any }

        //設定のイニシャライズ
        public ConfigClass(MainWindow mainComponents)
        {
            //各種設定ファイル
            CLS_N_LiveConfig = new NikoLiveConfigClass(CONFIG_DIRECTORY, mainComponents);
            CLS_SHOWROOM_Config = new SHOWROOMConfigClass(CONFIG_DIRECTORY, mainComponents);
            CLS_Graphics_Config = new GraphicsConfigClass(CONFIG_DIRECTORY, mainComponents);
            CLS_Any_Config = new AnyConfigClass(CONFIG_DIRECTORY, mainComponents);
        }

        //configファイルの更新
        public void changeConfigWrite(int kind, string key, string value, bool reflection)
        {
            //フォルダ存在確認(無ければデフォルト値作成済み)
            if (configFolderExistsCheck())
            {
                //各configで更新作業
                switch (kind)
                {
                    case (int)ConfigKind.N_Live:
                        CLS_N_LiveConfig.configUpdate(key, value, reflection);
                        break;
                    case (int)ConfigKind.SHOWROOM:
                        CLS_SHOWROOM_Config.configUpdate(key, value, reflection);
                        break;
                    case (int)ConfigKind.Graphics:
                        CLS_Graphics_Config.configUpdate(key, value, reflection);
                        break;
                    case (int)ConfigKind.Any:
                        CLS_Any_Config.configUpdate(key, value, reflection);
                        break;
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
                CLS_N_LiveConfig.defaultConfigFileCreate();
                CLS_SHOWROOM_Config.defaultConfigFileCreate();
                CLS_Graphics_Config.defaultConfigFileCreate();
                CLS_Any_Config.defaultConfigFileCreate();
                return false;
            }
            return true;
        }

        //サイズ状態更新処理
        public async Task sizeCheckUpdate()
        {
            CLS_Graphics_Config.sizeCheckUpdate();
        }
    }
}