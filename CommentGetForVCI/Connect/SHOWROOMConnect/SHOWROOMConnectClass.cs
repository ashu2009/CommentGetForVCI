using System.Collections.Generic;
using System.Windows;
using System.Net;
using System.Net.WebSockets;
using System;
using System.Threading;
using System.IO;
using System.Text;
using System.Threading.Tasks;


namespace CommentGetForVCI
{
    class SHOWROOMConnectClass
    {
        //SHOWROOMbroadcast_key検索
        private readonly string SHOWROOMLIVE_BROADCAST_KEY = "\"online.showroom-live.com\",\"";
        //SHOWROOM固定wss
        private readonly string SHOWROOMLIVE_WSS = "wss://online.showroom-live.com";
        //コンポーネント使用時に使用
        private MainWindow MAIN_COMPONENTS = null;
        //未送信コメント保持キュー
        private Queue<string> commentQue = null;
        //SupportClass初期化
        private SupportClass clsSupport = new SupportClass();

        //接続中か否か(接続前準備)
        private bool conecttingwait = false;
        //接続中か否か(接続前準備)
        private bool conecttingLive = false;
        //現在接続中のURL
        private string conecttingProgramURL = "";
        //現在接続中のウェブソケットでコメント取得時必要
        private string conecttingLiveProgramWebsoketForComment = "";
        //現在接続中のウェブソケット
        private ClientWebSocket conecttingLiveProgramWebsoket = null;

        //設定のイニシャライズ
        public SHOWROOMConnectClass(MainWindow mainComponents, Queue<string> Que)
        {
            //コンポーネント保存
            MAIN_COMPONENTS = mainComponents;
            //未送信コメント保持キュー保存
            commentQue = Que;
        }

        //接続挑戦
        public async void TryConnect()
        {
            //接続待ち
            conecttingwait = true;
            //SHOWROOM未接続状態
            if (!conecttingLive)
            {
                //番組情報入力済み(正否は問わず)
                string URL = correctSHOWROOMURL(MAIN_COMPONENTS.programINF_TXTBox.Text);
                //今見ているものは再読み込みしない
                if (conecttingProgramURL != URL)
                {
                    if ((MAIN_COMPONENTS.programINF_TXTBox.Text != null) && (URL != null))
                    {
                        //HTML情報取得
                        string html_INF = getSHOWROOMHTML_INF(URL);
                        if ((html_INF != null) && (html_INF != ""))
                        {
                            //HTML情報からbroadcast_key取得
                            string broadcast_key = getSHOWROOMHTML_PartINF(html_INF);
                            if ((broadcast_key != null) && (broadcast_key != ""))
                            {
                                //SHOWROOMコメント接続
                                conectWebSocketSHOWROOM(URL, broadcast_key);
                            }
                            else if (broadcast_key != null) MessageBox.Show("broadcast_keyを正常に取得できませんでした");
                        }
                        else MessageBox.Show("html情報を正常に取得できませんでした");
                    }
                    else MessageBox.Show("URLがSHOWROOMでない、もしくは間違っています\nSHOWROOMのURLを入力願います");
                }
                else MessageBox.Show("現在コメント取得先です");
            }
            else MessageBox.Show("新たに接続する前に切断してください");
        }

        //ウェブソケットと接続しコメント関係処理
        async void conectWebSocketSHOWROOM(string URL, string message_send)
        {
            //クライアント側のWebSocketを定義
            conecttingLiveProgramWebsoket = new ClientWebSocket();
            //接続先エンドポイントを指定
            var uri = new Uri(SHOWROOMLIVE_WSS);
            //User-AgentないとCONECTION_ERROR返ってくるため適当なの
            conecttingLiveProgramWebsoket.Options.SetRequestHeader(clsSupport.USER_AGENT, clsSupport.USER_AGENT_BROWSER);
            try
            {
                //サーバに対し、接続を開始
                await conecttingLiveProgramWebsoket.ConnectAsync(uri, CancellationToken.None);
                var buffer = new byte[1024];
                //ディレクトリ存在しなければ作成
                if (!Directory.Exists(MAIN_COMPONENTS.configCommentFilePos2.Text)) Directory.CreateDirectory(MAIN_COMPONENTS.configCommentFilePos2.Text);
                //ファイル初期化/作成
                using (StreamWriter writer = new StreamWriter(MAIN_COMPONENTS.configCommentFilePos2.Text + "\\" + clsSupport.COMMENT_SHOWROOM_FILENAME, false)) writer.Write("");

                //所得情報確保用の配列を準備
                var segment = new ArraySegment<byte>(buffer);
                //送るメッセージ(中にいくつかあるのでjson見送り)
                string return_message = message_send;

                //メッセージを変換
                ArraySegment<byte> return_buffer
                      = new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(return_message));
                //メッセージ送る    
                await conecttingLiveProgramWebsoket.SendAsync(return_buffer, WebSocketMessageType.Text, true, System.Threading.CancellationToken.None);

                //SHOWROOM接続状態のフラグ制御(接続等)
                SHOWROOMLiveConectedflgChange(URL, return_message);
                await Task.Delay(500);


                int Count = 1;
                //情報取得待ちループ
                while (true)
                {
                    //サーバからのレスポンス情報を取得
                    var result = await conecttingLiveProgramWebsoket.ReceiveAsync(segment, CancellationToken.None);
                    //エンドポイントCloseの場合、処理を中断
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        //ニコ生接続状態のフラグ制御(切断等)
                        await conecttingLiveProgramWebsoket.CloseAsync(WebSocketCloseStatus.NormalClosure, "end", CancellationToken.None);
                        SHOWROOMLiveDisconectedflgChange();
                        return;
                    }

                    //バイナリの場合は、当処理では扱えないため、処理を中断
                    if (result.MessageType == WebSocketMessageType.Binary)
                    {
                        //ニコ生接続状態のフラグ制御(切断等)
                        await conecttingLiveProgramWebsoket.CloseAsync(WebSocketCloseStatus.NormalClosure, "end", CancellationToken.None);
                        SHOWROOMLiveDisconectedflgChange();
                        return;
                    }

                    //メッセージの最後まで取得
                    int count = result.Count;
                    while (!result.EndOfMessage)
                    {
                        if (count >= buffer.Length)
                        {
                            //ニコ生接続状態のフラグ制御(切断等)
                            await conecttingLiveProgramWebsoket.CloseAsync(WebSocketCloseStatus.NormalClosure, "end", CancellationToken.None);
                            SHOWROOMLiveDisconectedflgChange();
                            return;
                        }
                        segment = new ArraySegment<byte>(buffer, count, buffer.Length - count);
                        result = await conecttingLiveProgramWebsoket.ReceiveAsync(segment, CancellationToken.None);
                        count += result.Count;
                    }

                    //メッセージを取得
                    var message = Encoding.UTF8.GetString(buffer, 0, count);

                    // ファイルの内容を1行ずつ読み込み比較
                    string all_line = null;
                    using (StreamReader reader = new StreamReader(MAIN_COMPONENTS.configCommentFilePos2.Text + "\\" + clsSupport.COMMENT_SHOWROOM_FILENAME)) all_line = reader.ReadToEnd();
                    if (!all_line.Contains(message))
                    {
                        //System.Diagnostics.Trace.WriteLine("> " + Count + "|||" + message);
                        if ((bool)MAIN_COMPONENTS.configSHOWROOMCommentLog.IsChecked)
                            using (StreamWriter writer = new StreamWriter(MAIN_COMPONENTS.configCommentFilePos2.Text + "\\" + clsSupport.COMMENT_SHOWROOM_FILENAME, true)) writer.WriteLine(message);
                        //リストに追加
                        if (SHOWROOMLiveCommentAddList(message_send, Count, message))
                        {
                            Count++;
                        }
                    }

                    //接続切る
                    if (!conecttingLive)
                    {
                        //切断
                        conecttingLiveProgramWebsoket.Dispose();
                        //切断
                        SHOWROOMLiveDisconectedflgChange();
                        return;
                    }
                }
            }
            catch (WebSocketException exc)
            {
            }
            catch (NullReferenceException exc)
            {
            }
        }

        //SHOWROOM接続状態のフラグ制御(接続等)
        private void SHOWROOMLiveConectedflgChange(string threadID, string return_message)
        {
            //接続待ち
            conecttingwait = false;
            //接続時に前のデータ削除
            MAIN_COMPONENTS.SHOWROOMLiveCommentListView.Items.Clear();
            //SHOWROOM接続
            conecttingLive = true;
            //SHOWROOM接続番組URL初期化
            conecttingProgramURL = threadID;
            //現在接続中のウェブソケットでコメント取得時必要
            conecttingLiveProgramWebsoketForComment = return_message;
        }

        //SHOWROOM接続状態のフラグ制御(切断等)
        private void SHOWROOMLiveDisconectedflgChange()
        {
            //接続待ち
            conecttingwait = false;
            //SHOWROOM接続解除
            conecttingLive = false;
            //SHOWROOM接続番組URL初期化
            conecttingProgramURL = "";
            //現在接続中のウェブソケットでコメント取得時必要
            conecttingLiveProgramWebsoketForComment = "";
        }

        //SHOWROOMのコメント反映
        private bool SHOWROOMLiveCommentAddList(string message_send, int num, string message)
        {
            string MSG = "MSG\t" + message_send.Substring("SUB\n".Length);
            //コメント番号設定
            string comment_num = num.ToString();
            //コメント内容取得
            string comment = "";
            //userID取得
            string userID = "";
            //コメント時間取得(変換)
            string time_data = "";
            //コテハン
            string fix_handle = "";
            //送信用フラグ
            bool send_flg = true;
            //情報コメント
            if (message.StartsWith(MSG + "\t{\"created_at\":"))
            {
                if (!(bool)MAIN_COMPONENTS.configSHOWROOMVisitComment2.IsChecked) return false;
                if (!(bool)MAIN_COMPONENTS.configSHOWROOMVisitComment.IsChecked) send_flg = false;
                //文字列処理中保存
                string data = clsSupport.commentSplit(message, MSG + "\t{", "");
                //時間
                string timeUNIX = clsSupport.commentSplit(data, "\"created_at\":", ",");

                //UNIX→日付
                if (Int64.TryParse(timeUNIX, out Int64 local_time))
                    time_data = DateTimeOffset.FromUnixTimeSeconds(local_time).ToLocalTime().ToString();

                data = clsSupport.commentSplit(data, ",", "");
                //合計ポイント
                if (data.StartsWith("\"c\":0") || data.StartsWith("\"c\":1"))
                {
                    if (!(bool)MAIN_COMPONENTS.configSHOWROOMPointComment2.IsChecked) return false;
                    if (!(bool)MAIN_COMPONENTS.configSHOWROOMPointComment.IsChecked) send_flg = false;
                    string cc = clsSupport.commentSplit(data, "\"c\":0", ",");
                    data = clsSupport.commentSplit(data, ",", "");
                    string p = clsSupport.commentSplit(data, "\"p\":", ",");
                    comment = "現在の支援ゲージは" + p + "ptです";
                }
                else
                {
                    if (data.StartsWith("\"n\":"))
                    {
                        return false;
                    }
                    if (data.StartsWith("\"t\":"))
                    {
                        return false;
                    }
                    if (data.StartsWith("\"u\":"))
                    {
                        return false;
                    }
                    //わからん
                    string c = clsSupport.commentSplit(data, "\"c\":\"", "\",");
                    data = clsSupport.commentSplit(data, ",", "");
                    if (data.StartsWith("\"p\":"))
                    {
                        //合計ポイント?
                        if (!(bool)MAIN_COMPONENTS.configSHOWROOMPointComment2.IsChecked) return false;
                        if (!(bool)MAIN_COMPONENTS.configSHOWROOMPointComment.IsChecked) send_flg = false;
                        string cc = clsSupport.commentSplit(data, "\"c\":0", ",");
                        data = clsSupport.commentSplit(data, ",", "");
                        string p = clsSupport.commentSplit(data, "\"p\":", ",");
                        comment = "現在の支援ゲージは" + p + "ptです";
                    }
                    else
                    {
                        //わからん
                        string u = clsSupport.commentSplit(data, "\"u\":", ",");
                        data = clsSupport.commentSplit(data, ",", "");
                        //わからん
                        string me = clsSupport.commentSplit(data, "\"me\":", "\",");
                        data = clsSupport.commentSplit(data, ",", "");
                        //コメント
                        comment = clsSupport.commentSplit(data, "\"m\":\"", "\",");
                        data = clsSupport.commentSplit(data, ",", "");
                        //わからん
                        string tt = clsSupport.commentSplit(data, "\"tt\":", ",");
                        data = clsSupport.commentSplit(data, ",", "");
                        //わからん
                        string t = clsSupport.commentSplit(data, "\"t\":", "}");
                    }
                }
            }
            //通常コメント(ギフト在り)
            else if (message.StartsWith(MSG + "\t{\"ua\":"))
            {
                //文字列処理中保存
                string data = clsSupport.commentSplit(message, MSG + "\t{", "");
                //ユーザーランク
                string user_rank = clsSupport.commentSplit(data, "\"ua\":", ",");
                data = clsSupport.commentSplit(data, ",", "");
                if (data.StartsWith("\"n\":"))
                {
                    //わからん
                    string n = clsSupport.commentSplit(data, "\"n\":", ",");
                    data = clsSupport.commentSplit(data, ",", "");
                }
                //アバターID
                string avaterID = clsSupport.commentSplit(data, "\"av\":", ",");
                data = clsSupport.commentSplit(data, ",", "");
                //わからん
                string aft = clsSupport.commentSplit(data, "\"aft\":", ",");
                data = clsSupport.commentSplit(data, ",", "");
                //わからん
                string d = clsSupport.commentSplit(data, "\"d\":", ",");
                data = clsSupport.commentSplit(data, ",", "");
                //ユーザー名
                fix_handle = clsSupport.commentSplit(data, "\"ac\":\"", "\",");
                data = clsSupport.commentSplit(data, ",", "");

                //ギフトの分かれ目
                if (data.IndexOf("\"cm\":\"") > -1)
                {
                    if (!(bool)MAIN_COMPONENTS.configSHOWROOMDefaultComment2.IsChecked) return false;
                    if (!(bool)MAIN_COMPONENTS.configSHOWROOMDefaultComment.IsChecked) send_flg = false;
                    //コメント
                    comment = clsSupport.commentSplit(data, "\"cm\":\"", "\",");
                    //カウント
                    if (int.TryParse(comment, out int num_data))
                    {
                        if ((1 <= num_data) && (num_data <= 50))
                        {
                            if (!(bool)MAIN_COMPONENTS.configSHOWROOMCountComment2.IsChecked) return false;
                            if (!(bool)MAIN_COMPONENTS.configSHOWROOMCountComment.IsChecked) send_flg = false;
                        }
                    }
                    data = clsSupport.commentSplit(data, ",", "");
                    //時間
                    string timeUNIX = clsSupport.commentSplit(data, "\"created_at\":", ",");
                    //UNIX→日付
                    if (Int64.TryParse(timeUNIX, out Int64 local_time)) time_data = DateTimeOffset.FromUnixTimeSeconds(local_time).ToLocalTime().ToString();

                    data = clsSupport.commentSplit(data, ",", "");
                    //ユーザーID
                    userID = clsSupport.commentSplit(data, "\"u\":", ",");
                    data = clsSupport.commentSplit(data, ",", "");
                    //わからん
                    string at = clsSupport.commentSplit(data, "\"at\":", ",");
                    data = clsSupport.commentSplit(data, ",", "");
                    //わからん
                    string t = clsSupport.commentSplit(data, "\"t\":", "\"}");
                }
                else
                {
                    //ギフト
                    //時間
                    string timeUNIX = clsSupport.commentSplit(data, "\"created_at\":", ",");
                    //UNIX→日付
                    if (Int64.TryParse(timeUNIX, out Int64 local_time)) time_data = DateTimeOffset.FromUnixTimeSeconds(local_time).ToLocalTime().ToString();

                    data = clsSupport.commentSplit(data, ",", "");
                    //ユーザーID
                    userID = clsSupport.commentSplit(data, "\"u\":", ",");
                    data = clsSupport.commentSplit(data, ",", "");
                    //わからん
                    string h = clsSupport.commentSplit(data, "\"h\":", ",");
                    data = clsSupport.commentSplit(data, ",", "");
                    //ギフト番号
                    string g = clsSupport.commentSplit(data, "\"g\":", ",");
                    data = clsSupport.commentSplit(data, ",", "");
                    //有料：２、無料：１
                    string gt = clsSupport.commentSplit(data, "\"gt\":", ",");
                    data = clsSupport.commentSplit(data, ",", "");
                    //わからん
                    string at = clsSupport.commentSplit(data, "\"at\":", ",");
                    data = clsSupport.commentSplit(data, ",", "");
                    //わからん
                    string t = clsSupport.commentSplit(data, "\"t\":", "\"}");


                    //ギフト番号
                    int giftNum = int.Parse(g);
                    if ((1501 <= giftNum) && (giftNum <= 1505))
                    {
                        comment = "種が" + fix_handle + "さんから届きました";
                        if (!(bool)MAIN_COMPONENTS.configSHOWROOMFreeGiftComment2.IsChecked) return false;
                        if (!(bool)MAIN_COMPONENTS.configSHOWROOMFreeGiftComment.IsChecked) send_flg = false;
                    }
                    else if ((1001 <= giftNum) && (giftNum <= 1005))
                    {
                        comment = "星が" + fix_handle + "さんから届きました";
                        if (!(bool)MAIN_COMPONENTS.configSHOWROOMGiftComment2.IsChecked) return false;
                        if (!(bool)MAIN_COMPONENTS.configSHOWROOMGiftComment.IsChecked) send_flg = false;
                    }
                    else
                    {
                        comment = "ギフトが" + fix_handle + "さんから届きました";
                        if (!(bool)MAIN_COMPONENTS.configSHOWROOMGiftComment2.IsChecked) return false;
                        if (!(bool)MAIN_COMPONENTS.configSHOWROOMGiftComment.IsChecked) send_flg = false;
                    }
                }
            }
            //テロップコメント
            else if (message.StartsWith(MSG + "\t{\"telops\":"))
            {
                if (!(bool)MAIN_COMPONENTS.configSHOWROOMTelopComment2.IsChecked) return false;
                if (!(bool)MAIN_COMPONENTS.configSHOWROOMTelopComment.IsChecked) send_flg = false;
                if (!message.Contains("{\"telops\":[],\"telop\":null,\""))
                {
                    //文字列処理中保存
                    string data = clsSupport.commentSplit(message, MSG + "\t{\"telops\":", "");
                    //テロップ色等
                    string telop_color = clsSupport.commentSplit(data, "[{\"color\":", ",\"");
                    data = clsSupport.commentSplit(data, "},", "");
                    //テロップ内容
                    comment = clsSupport.commentSplit(data, "\"text\":\"", ",\"");
                }
                else return false;
            }
            else return false;
            MAIN_COMPONENTS.SHOWROOMLiveCommentListView.Items.Add(new string[] { comment_num, fix_handle, userID, time_data, comment });

            //スクロール機能
            if ((bool)MAIN_COMPONENTS.configSHOWROOMScrollnable.IsChecked)
            {
                MAIN_COMPONENTS.SHOWROOMLiveCommentListView.SelectedIndex = MAIN_COMPONENTS.SHOWROOMLiveCommentListView.Items.Count - 1;
                MAIN_COMPONENTS.SHOWROOMLiveCommentListView.ScrollIntoView(MAIN_COMPONENTS.SHOWROOMLiveCommentListView.SelectedItem);
            }

            //SHOWROOMコメントをVCIに送る関係
            if (send_flg)
            {
                SHOWROOMLiveCommentSendToVCI(message, comment_num, fix_handle, comment);
            }
            return true;
        }

        //SHOWROOMコメントをVCIに送るやつ
        private void SHOWROOMLiveCommentSendToVCI(string message, string comment_num, string name, string comment)
        {
            //VCIにコメント送信する
            if (((bool)MAIN_COMPONENTS.configVCICommentSend.IsChecked) && ((bool)MAIN_COMPONENTS.configSHOWROOMCommentSend.IsChecked))
            {
                string data = "if vci.assets.IsMine then vci.message.Emit(\"comment\",\"";
                if ((bool)MAIN_COMPONENTS.configVCICommentSendAddBroadcast.IsChecked) data += "@SH：";
                if ((bool)MAIN_COMPONENTS.configVCICommentSendAddFixHandle.IsChecked) data += "@" + name + "：";
                comment.Replace("\"", "″");
                data += comment + "\") end";

                //コメント未処理分
                commentQue.Enqueue(data + "\n");
            }
        }

        //SHOWROOM用ページであるか確認/修正
        private string correctSHOWROOMURL(string url)
        {
            string correct_url = null;
            //番組情報から
            if (url.StartsWith("https://www.showroom-live.com/"))
            {
                correct_url = url;
                return correct_url;
            }
            //SHOWROOMではないと判断
            else correct_url = null;

            return correct_url;
        }

        //HTML情報取得
        private string getSHOWROOMHTML_INF(string url)
        {
            //HTML情報
            string html_INF = null;
            //HTML情報取得
            using (WebClient wc = new WebClient())
            {
                try
                {
                    html_INF = wc.DownloadString(url);
                }
                catch (WebException exc)
                {
                    html_INF = null;
                }
            }
            return html_INF;
        }

        //HTML情報から指定部分抜き出し
        private string getSHOWROOMHTML_PartINF(string htmlSTR)
        {
            //生放送中以外含まれないため
            if (htmlSTR.Contains(SHOWROOMLIVE_BROADCAST_KEY))
            {
                //除去
                string trim_data2 = clsSupport.commentSplit(htmlSTR, SHOWROOMLIVE_BROADCAST_KEY, "\",");
                //HTML情報
                string html_partINF = "SUB\t" + trim_data2;
                return html_partINF;
            }
            else
            {
                MessageBox.Show("対応していません");
                return null;
            }
        }

        //切断挑戦
        public async void TryDisonnect()
        {
            //ニコ生接続中
            if (conecttingLive)
            {
                conecttingLive = false;
                //ショールームにメッセージ
                ArraySegment<byte> return_buffer
                      = new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(conecttingLiveProgramWebsoketForComment));
                //メッセージ送る    
                await conecttingLiveProgramWebsoket.SendAsync(return_buffer, WebSocketMessageType.Text, true, System.Threading.CancellationToken.None);

            }
        }

        //接続待機状態
        public bool getConecttingWaitFlg()
        {
            return conecttingwait;
        }

        //接続状態
        public bool getConecttingFlg()
        {
            return conecttingLive;
        }
    }
}