using System.Windows;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace CommentGetForVCI
{
    class ConnectClass
    {
        //コンポーネント使用時に使用
        private MainWindow MAIN_COMPONENTS = null;
        //NikoLiveConnectClass初期化
        private NikoLiveConnectClass clsNikoLiveConnect = null;
        //SHOWROOMConnectClass初期化
        private SHOWROOMConnectClass clsSHOWROOMConnect = null;

        //接続待機中か否か(接続前準備)
        private bool anyConectWaitting = false;
        //接続開始中か否か(番組接続中)
        //private bool anyConectingProgram = false;
        //未送信コメント保持キュー
        private Queue<string> commentQue = new Queue<string>();
        //送信予定コメント
        private string unsentComment = "";
        //送信済みコメント
        private string sendcomment = "";
        //SupportClass初期化
        private SupportClass clsSupport = new SupportClass();


        //設定種類
        public enum ConfigKind { N_Live, SHOWROOM }

        //設定のイニシャライズ
        public ConnectClass(MainWindow mainComponents)
        {
            //コンポーネント保存
            MAIN_COMPONENTS = mainComponents;
            //ニコ生接続初期化
            clsNikoLiveConnect = new NikoLiveConnectClass(mainComponents, commentQue);
            //SHOWROOM接続初期化
            clsSHOWROOMConnect = new SHOWROOMConnectClass(mainComponents, commentQue);
        }

        //接続挑戦
        public async void TryConnect()
        {
            //何かしらの番組と接続待機中ではないならば
            if (!anyConectWaitting)
            {
                anyConectWaitting = true;
                await Task.Delay(500);

                //何かしらの番組と接続開始中ではないならば
                if (!(bool)MAIN_COMPONENTS.conectintgCheckBox.IsChecked)
                {
                    await Task.Delay(500);

                    switch (MAIN_COMPONENTS.comentTabControl.SelectedIndex)
                    {
                        case (int)ConfigKind.N_Live:
                            //ニコ生接続挑戦
                            clsNikoLiveConnect.TryConnect();
                            break;
                        case (int)ConfigKind.SHOWROOM:
                            //SHOWROOM接続挑戦
                            clsSHOWROOMConnect.TryConnect();
                            break;
                        default:
                            MessageBox.Show("繋ぎ先未確定です");
                            break;
                    }
                }
                else MessageBox.Show("番組接続中です");
                anyConectWaitting = false;
            }
            else MessageBox.Show("接続待機中です");
        }

        //切断挑戦
        public async void TryDisonnect()
        {
            //何かしらの番組と接続待機中ではないならば
            if (!anyConectWaitting)
            {
                //何かしらの番組と接続開始中ではないならば
                if (!(bool)MAIN_COMPONENTS.conectintgCheckBox.IsChecked)
                {
                    switch (MAIN_COMPONENTS.comentTabControl.SelectedIndex)
                    {
                        case (int)ConfigKind.N_Live:
                            //ニコ生切断挑戦
                            clsNikoLiveConnect.TryDisonnect();
                            break;
                        case (int)ConfigKind.SHOWROOM:
                            //SHOWROOM切断挑戦
                            clsSHOWROOMConnect.TryDisonnect();
                            break;
                        default:
                            break;
                    }
                }
                else MessageBox.Show("番組接続中です\n切断できません");
            }
            else MessageBox.Show("接続待機中です\n切断できません");
        }

        //接続状態更新処理
        public async Task stateCheckUpdate()
        {
            while (true)
            {
                //何かしらの番組と接続待機中
                if (anyConectWaitting) MAIN_COMPONENTS.conectWaittingCheckBox.IsChecked = true;
                else MAIN_COMPONENTS.conectWaittingCheckBox.IsChecked = false;
                //何かしらの番組と接続開始中
                if ( clsNikoLiveConnect.getConecttingWaitFlg() || clsNikoLiveConnect.getConecttingWaitFlg()) MAIN_COMPONENTS.conectintgCheckBox.IsChecked = true;
                else MAIN_COMPONENTS.conectintgCheckBox.IsChecked = false;
                //ニコ生接続中
                if (clsNikoLiveConnect.getConecttingFlg()) MAIN_COMPONENTS.nikoLiveGettingCheckBox.IsChecked = true;
                else MAIN_COMPONENTS.nikoLiveGettingCheckBox.IsChecked = false;
                //SHOWROOM接続中
                if (clsSHOWROOMConnect.getConecttingFlg()) MAIN_COMPONENTS.SHOWROOMLiveGettingCheckBox.IsChecked = true;
                else MAIN_COMPONENTS.SHOWROOMLiveGettingCheckBox.IsChecked = false;
                await Task.Delay(200);
            }
        }

        //コメントVCIループ
        public async Task commentVCIUpdate()
        {
            while (true)
            {
                //ディレクトリ確認
                if (Directory.Exists(MAIN_COMPONENTS.configVCIFilePos2.Text.Substring(0, MAIN_COMPONENTS.configVCIFilePos2.Text.LastIndexOf("\\"))))
                {
                    //ファイル確認
                    if (!File.Exists(MAIN_COMPONENTS.configVCIFilePos2.Text))
                    {
                        //無ければ作成
                        try
                        {
                            StreamWriter write = new StreamWriter(MAIN_COMPONENTS.configVCIFilePos2.Text, false);
                            write.Write("if vci.assets.IsMine then end");
                            write.Close();
                        }
                        catch (IOException exc)
                        {
                        }
                    }
                    else
                    {
                        //キューにコメントがあり、未送信コメントなければ
                        if ((unsentComment == "") && (commentQue.Count > 0))
                        {
                            string countMax = clsSupport.commentSplit(MAIN_COMPONENTS.configVCICommentSendMax2.SelectedItem.ToString(), "System.Windows.Controls.ComboBoxItem: ", "");
                            int max = int.Parse(countMax);
                            int count = 0;
                            while (0<commentQue.Count)
                            {
                                unsentComment += commentQue.Dequeue();
                                ++count;
                                if (count == max) break;
                            }
                        }

                        //未送信コメントあり
                        if (unsentComment != "")
                        {
                            try
                            {
                                //VCIメッセージ更新
                                StreamWriter write = new StreamWriter(MAIN_COMPONENTS.configVCIFilePos2.Text, false);
                                //前のコメントと同じ場合、VCI更新されないため空白付与
                                if (sendcomment == unsentComment)
                                {
                                    unsentComment += "\nif vci.assets.IsMine then  end";
                                }
                                write.Write(unsentComment);
                                write.Close();
                                MAIN_COMPONENTS.SendMessage.Text = "before\n\n" + sendcomment;
                                MAIN_COMPONENTS.SendMessage.Text += "\n\nafter\n\n" + unsentComment;
                                sendcomment = unsentComment;
                                unsentComment = "";
                            }
                            catch (IOException exc)
                            {
                            }
                        }
                    }
                }

                string Num = clsSupport.commentSplit(MAIN_COMPONENTS.configVCIUpdateTimming2.SelectedItem.ToString(), "System.Windows.Controls.ComboBoxItem: ", "");
                if (int.TryParse(Num, out int data))
                {
                    await Task.Delay(data * 1000);
                }
                else
                {
                    await Task.Delay(8000);
                }
            }
        }
    }
}