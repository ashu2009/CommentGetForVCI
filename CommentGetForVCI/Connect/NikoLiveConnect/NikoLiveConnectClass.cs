using System;
using System.Windows;
using System.Threading;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace CommentGetForVCI
{
    class NikoLiveConnectClass
    {
        //ニコ生視聴URL
        private readonly string NIKOLIVE_VIEW_URL = "https://live.nicovideo.jp/watch/lv";
        //ニコ生スレッドID前半部分
        private readonly string NIKOLIVE_THREAD_PART = "wss://a.live2.nicovideo.jp/unama/wsapi/v2/watch/";
        //UriとスレッドID取得時用
        private readonly string URI_THREAD_ID_SEND_MESS = "{\"type\":\"startWatching\",\"data\":{\"stream\":{\"quality\":\"high\",\"protocol\":\"hls\",\"latency\":\"low\",\"chasePlay\":false},\"room\":{\"protocol\":\"webSocket\",\"commentable\":true},\"reconnect\":false}}";
        //コメント取得時用
        private readonly string COMMENT_SEND_MESS_BEFORE = "[{\"ping\":{\"content\":\"rs:0\"}},{\"ping\":{\"content\":\"ps:0\"}},{\"thread\":{\"thread\":\"";
        private readonly string COMMENT_SEND_MESS_AFTER = "\",\"version\":\"20061206\",\"user_id\":\"guest\",\"res_from\":-";
        private readonly string COMMENT_SEND_MESS_AFTER2 = ",\"with_global\":1,\"scores\":1,\"nicoru\":0}},{\"ping\":{\"content\":\"pf:0\"}},{\"ping\":{\"content\":\"rf:0\"}}]";
        //コメント修正用
        private readonly string EMOTION_COMMENT = "/emotion ";
        private readonly string GIFT_COMMENT = "/gift";
        private readonly string FREEGIFT_COMMENT = "/gift vcast_free";
        private readonly string LIKECOME_COMMENT = "/info 10 「";
        private readonly string REQUEST_COMMENT = "/spi \\\"「";
        private readonly string ADVERTISEMENT_COMMENT = "/nicoad {\\\"version\\\":";
        private readonly string ADVERTISEMENT_INF_COMMENT = "/info 8 第";
        //コンポーネント使用時に使用
        private MainWindow MAIN_COMPONENTS = null;
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
        //現在接続中のニコ生コメント送るかどうか
        private bool LiveCommentAdd = false;
        //現在接続中のニコ生コメント送りはじめる番号
        private int LiveCommentSendNum = 0;
        //未送信コメント保持キュー
        private Queue<string> commentQue = null;


        //設定のイニシャライズ
        public NikoLiveConnectClass(MainWindow mainComponents, Queue<string> Que)
        {
            //コンポーネント保存
            MAIN_COMPONENTS = mainComponents;
            //未送信コメント保持キュー保存
            commentQue = Que;
        }

        //接続挑戦
        public async void tryConnect()
        {
            //接続待ち
            conecttingwait = true;
            //ニコ生未接続状態
            if (!conecttingLive)
            {
                //番組情報入力済み(正否は問わず)
                string URL = correctURL(MAIN_COMPONENTS.programINF_TXTBox.Text);
                //今見ているものは再読み込みしない
                if (conecttingProgramURL != URL)
                {
                    if ((MAIN_COMPONENTS.programINF_TXTBox.Text != null) && (URL != null))
                    {
                        //HTML情報取得
                        string html_INF = getHTML_INF(URL);
                        if ((html_INF != null) && (html_INF != ""))
                        {
                            //HTML情報からスレッドID取得
                            string threadID = getHTML_PartINF(html_INF);
                            if ((threadID != null) && (threadID != ""))
                            {
                                //ニコ生コメントスレッド接続
                                conectWebSocketToThread(URL, threadID);
                            }
                            else if (threadID != null) MessageBox.Show("スレッドIDを正常に取得できませんでした");
                        }
                        else MessageBox.Show("html情報を正常に取得できませんでした");
                    }
                    else MessageBox.Show("URLがニコ生でない、もしくは間違っています\nニコ生のURLかlv...を入力願います");
                }
                else MessageBox.Show("現在コメント取得先です");
            }
            else MessageBox.Show("新たに接続する前に切断してください");
        }

        //ウェブソケットと接続しコメント関係処理
        private async void conectWebSocketToThread(string URL, string wss_uri)
        {
            //クライアント側のWebSocketを定義
            ClientWebSocket ws = new ClientWebSocket();
            //接続先エンドポイントを指定
            var uri = new Uri(wss_uri);
            //User-AgentないとCONECTION_ERROR返ってくるため適当なの
            ws.Options.SetRequestHeader(clsSupport.USER_AGENT, clsSupport.USER_AGENT_BROWSER);
            try
            {
                var buffer = new byte[1024];
                //サーバに対し、接続を開始
                await ws.ConnectAsync(uri, CancellationToken.None);
                //所得情報確保用の配列を準備
                var segment = new ArraySegment<byte>(buffer);
                //サーバからのレスポンス情報を取得
                var result = await ws.ReceiveAsync(segment, CancellationToken.None);
                //エンドポイントCloseの場合、処理を中断
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    //ニコ生接続状態のフラグ制御(切断等)
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "end", CancellationToken.None);
                    nikoLiveDisconectedflgChange();
                    return;
                }

                //バイナリの場合は、当処理では扱えないため、処理を中断
                if (result.MessageType == WebSocketMessageType.Binary)
                {
                    //ニコ生接続状態のフラグ制御(切断等)
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "end", CancellationToken.None);
                    nikoLiveDisconectedflgChange();
                    return;
                }

                //URIとスレッドID取得関係処理
                getUriAndThreadId(URL, ws);
            }
            catch (WebSocketException exc)
            {
            }
            catch (NullReferenceException exc)
            {
            }
        }

        //URIとスレッドID取得
        private async void getUriAndThreadId(string URL, ClientWebSocket ws)
        {
            //メッセージを変換
            ArraySegment<byte> return_buffer
              = new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(URI_THREAD_ID_SEND_MESS));
            //メッセージ送る
            await ws.SendAsync(return_buffer, WebSocketMessageType.Text, true, System.Threading.CancellationToken.None);
            var buffer = new byte[1024];
            //ニコ生URIとスレッドIDデータ
            string uri_data = "";
            string thread_id_data = "";

            //情報取得待ちループ
            while (true)
            {
                //所得情報確保用の配列を準備
                var segment = new ArraySegment<byte>(buffer);
                //サーバからのレスポンス情報を取得
                var result = await ws.ReceiveAsync(segment, CancellationToken.None);
                //エンドポイントCloseの場合、処理を中断
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    //ニコ生接続状態のフラグ制御(切断等)
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "end", CancellationToken.None);
                    nikoLiveDisconectedflgChange();
                    return;
                }

                //バイナリの場合は、当処理では扱えないため、処理を中断
                if (result.MessageType == WebSocketMessageType.Binary)
                {
                    //ニコ生接続状態のフラグ制御(切断等)
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "end", CancellationToken.None);
                    nikoLiveDisconectedflgChange();
                    return;
                }

                //メッセージの最後まで取得
                int count = result.Count;
                while (!result.EndOfMessage)
                {
                    if (count >= buffer.Length)
                    {
                        //ニコ生接続状態のフラグ制御(切断等)
                        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "end", CancellationToken.None);
                        nikoLiveDisconectedflgChange();
                        return;
                    }
                    segment = new ArraySegment<byte>(buffer, count, buffer.Length - count);
                    result = await ws.ReceiveAsync(segment, CancellationToken.None);
                    count += result.Count;
                }

                //メッセージを取得
                var message = Encoding.UTF8.GetString(buffer, 0, count);
                //messageServerを含むとき
                if (message.Contains("messageServer"))
                {
                    //uri
                    uri_data = clsSupport.commentSplit(message, "uri\":\"", "\",\"");
                    //threadId
                    thread_id_data = clsSupport.commentSplit(message, "threadId\":\"", "\",\"");
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "OK",
              CancellationToken.None);
                    break;
                }
            }

            //ウェブソケットと接続しコメント関係処理
            conectWebSocketToComment(URL, uri_data, thread_id_data);
        }

        //ウェブソケットと接続しコメント関係処理
        private async void conectWebSocketToComment(string URL, string wss_uri, string thread_id_data)
        {
            //クライアント側のWebSocketを定義
            conecttingLiveProgramWebsoket = new ClientWebSocket();
            //接続先エンドポイントを指定
            var uri = new Uri(wss_uri);
            //User-AgentないとCONECTION_ERROR返ってくるため適当なの
            conecttingLiveProgramWebsoket.Options.SetRequestHeader(clsSupport.USER_AGENT, clsSupport.USER_AGENT_BROWSER);
            try
            {
                //サーバに対し、接続を開始
                await conecttingLiveProgramWebsoket.ConnectAsync(uri, CancellationToken.None);
                var buffer = new byte[1024];
                string directory = MAIN_COMPONENTS.configCommentFilePos2.Text;
                //ディレクトリ存在しなければ作成
                try
                {
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                }
                catch (IOException exe)
                {
                    System.Diagnostics.Trace.WriteLine("> " + exe);
                }

                //ファイル初期化/作成
                using (StreamWriter writer = new StreamWriter(MAIN_COMPONENTS.configCommentFilePos2.Text + "\\" + clsSupport.COMMENT_NIKOLIVE_FILENAME, false)) writer.Write("");

                //所得情報確保用の配列を準備
                var segment = new ArraySegment<byte>(buffer);
                //送るメッセージ(中にいくつかあるのでjson見送り)
                string Num = clsSupport.commentSplit(MAIN_COMPONENTS.configPrevGetCount2.SelectedItem.ToString(), "System.Windows.Controls.ComboBoxItem: ", "");
                string return_message = COMMENT_SEND_MESS_BEFORE + thread_id_data + COMMENT_SEND_MESS_AFTER + Num + COMMENT_SEND_MESS_AFTER2;

                //メッセージを変換
                ArraySegment<byte> return_buffer
                      = new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(return_message));
                //メッセージ送る    
                await conecttingLiveProgramWebsoket.SendAsync(return_buffer, WebSocketMessageType.Text, true, System.Threading.CancellationToken.None);

                //ニコ生接続状態のフラグ制御(接続等)
                nikoLiveConectedFlgChange(URL, return_message);
                await Task.Delay(500);


                int Count = 0;
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
                        nikoLiveDisconectedflgChange();
                        return;
                    }

                    //バイナリの場合は、当処理では扱えないため、処理を中断
                    if (result.MessageType == WebSocketMessageType.Binary)
                    {
                        //ニコ生接続状態のフラグ制御(切断等)
                        await conecttingLiveProgramWebsoket.CloseAsync(WebSocketCloseStatus.NormalClosure, "end", CancellationToken.None);
                        nikoLiveDisconectedflgChange();
                        return;
                    }
                    ++Count;
                    //メッセージの最後まで取得
                    int count = result.Count;
                    while (!result.EndOfMessage)
                    {
                        if (count >= buffer.Length)
                        {
                            //ニコ生接続状態のフラグ制御(切断等)
                            await conecttingLiveProgramWebsoket.CloseAsync(WebSocketCloseStatus.NormalClosure, "end", CancellationToken.None);
                            nikoLiveDisconectedflgChange();
                            return;
                        }
                        segment = new ArraySegment<byte>(buffer, count, buffer.Length - count);
                        result = await conecttingLiveProgramWebsoket.ReceiveAsync(segment, CancellationToken.None);
                        count += result.Count;
                    }

                    //メッセージを取得
                    var message = Encoding.UTF8.GetString(buffer, 0, count);
                    if (!message.StartsWith("{\"thread\":{\"resultcode\":0,\"thread\":\""))
                    {
                        //System.Diagnostics.Trace.WriteLine("> " + Count + "|||" + message);
                        if ((bool)MAIN_COMPONENTS.configNikoCommentLog.IsChecked)
                            using (StreamWriter writer = new StreamWriter(MAIN_COMPONENTS.configCommentFilePos2.Text + "\\" + clsSupport.COMMENT_NIKOLIVE_FILENAME, true)) writer.WriteLine(message);
                        //リストに追加
                        nikoLiveCommentAddList(message);
                    }

                    //接続切る
                    if (!conecttingLive)
                    {
                        //切断
                        conecttingLiveProgramWebsoket.Dispose();
                        //切断
                        nikoLiveDisconectedflgChange();
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

        //ニコ生のコメント反映
        private void nikoLiveCommentAddList(string message)
        {
            if (message.Contains("{\"chat\":{\"thread\":\""))
            {
                //コメント番号設定
                string comment_num = clsSupport.commentSplit(message, "\",\"no\":", ",\"");
                //コメント内容取得
                string comment = clsSupport.commentSplit(message, "content\":\"");

                //userID取得
                string userID = clsSupport.commentSplit(message, "user_id\":\"", "\",\"");
                //コテハン通常184判定
                string fix_handle = "184";
                int fix_handle_pos;

                //ディレクトリ存在しなければ作成
                if (!Directory.Exists(MAIN_COMPONENTS.configFixHandleFilePos2.Text)) Directory.CreateDirectory(MAIN_COMPONENTS.configFixHandleFilePos2.Text);
                //ファイル存在しなければ作成
                if (!File.Exists(MAIN_COMPONENTS.configFixHandleFilePos2.Text + "\\" + clsSupport.FIXHANDLE_NIKOLIVE_FILENAME))
                {
                    //ファイル初期化/作成
                    using (StreamWriter writer = new StreamWriter(MAIN_COMPONENTS.configFixHandleFilePos2.Text + "\\" + clsSupport.FIXHANDLE_NIKOLIVE_FILENAME, false)) writer.Write("");
                }
                else
                {
                    //コテハン登録済みのもの使用する場合
                    if ((bool)MAIN_COMPONENTS.configNikoLiveFixHandleUse.IsChecked)
                    {
                        StreamReader reader = new StreamReader(MAIN_COMPONENTS.configFixHandleFilePos2.Text + "\\" + clsSupport.FIXHANDLE_NIKOLIVE_FILENAME);
                        // ファイルの内容を1行ずつ読み込み比較
                        string line = null;

                        while ((line = reader.ReadLine()) != null)
                        {
                            //ユーザーIDにコテハンあれば使用
                            if (line.StartsWith(userID + ":"))
                            {
                                fix_handle = line.Substring((userID + ":").Length);
                            }
                        }
                        reader.Close();
                    }
                }

                //新規登録ありならば
                if ((bool)MAIN_COMPONENTS.configNikoLiveFixHandleAdd.IsChecked)
                {
                    //コメント内にコテハンあれば(/@は除く)
                    fix_handle_pos = comment.LastIndexOf("@");
                    int setpos = comment.LastIndexOf("/@");
                    //9-10=-1,-1-0=-1.-1-10not-1,-1+1not-1(@がない時)
                    if (fix_handle_pos != setpos && (fix_handle_pos == 0 || (setpos - fix_handle_pos) != -1))
                    {
                        //ギフトなどアカウント名に@ある時の対策
                        if ((!comment.StartsWith(GIFT_COMMENT)) || (!comment.StartsWith(ADVERTISEMENT_COMMENT)))
                        {
                            //コテハン
                            fix_handle = comment.Substring(fix_handle_pos + "@".Length);
                            StreamReader reader = new StreamReader(MAIN_COMPONENTS.configFixHandleFilePos2.Text + "\\" + clsSupport.FIXHANDLE_NIKOLIVE_FILENAME);
                            // ファイルの内容を1行ずつ読み込み比較
                            string line = null;
                            string write_data = "";
                            int line_count = 0;
                            while ((line = reader.ReadLine()) != null)
                            {
                                //指定ユーザーIDでないこと確認しつつ追加
                                if (!line.StartsWith(userID + ":"))
                                {
                                    //1行目以外の改行処理
                                    if (line_count != 0) write_data += "\n";
                                    write_data += line;
                                    //次分カウント
                                    ++line_count;
                                }
                            }
                            //1行目以外の改行処理
                            if (line_count != 0) write_data += "\n";
                            write_data += userID + ":" + fix_handle;
                            reader.Close();
                            //コテハン更新
                            using (StreamWriter writer = new StreamWriter(MAIN_COMPONENTS.configFixHandleFilePos2.Text + "\\" + clsSupport.FIXHANDLE_NIKOLIVE_FILENAME, false)) writer.Write(write_data);
                        }
                    }
                }

                //コメント時間取得(変換)
                string timeUNIX = clsSupport.commentSplit(message, "\"date\":", ",\"");
                string date = "取得不可";
                //UNIX→日付
                if (Int64.TryParse(timeUNIX, out Int64 local_time)) date = DateTimeOffset.FromUnixTimeSeconds(local_time).ToLocalTime().ToString();

                string correctComment = comment;
                if ((bool)!MAIN_COMPONENTS.configNikoCommentLaw.IsChecked) correctComment = commentCorrect(comment);
                //コメント列追加
                MAIN_COMPONENTS.nikoLiveCommentListView.Items.Add(new string[] { comment_num, fix_handle, userID, date, correctComment });

                //スクロール機能
                if ((bool)MAIN_COMPONENTS.configNikoLiveScrollEnable.IsChecked)
                {
                    MAIN_COMPONENTS.nikoLiveCommentListView.SelectedIndex = MAIN_COMPONENTS.nikoLiveCommentListView.Items.Count - 1;
                    MAIN_COMPONENTS.nikoLiveCommentListView.ScrollIntoView(MAIN_COMPONENTS.nikoLiveCommentListView.SelectedItem);
                }

                //全部送信フラグ
                if (((bool)MAIN_COMPONENTS.configNikoLiveGetAllCommentSend.IsChecked) && (LiveCommentSendNum == 0))
                {
                    //ニコ生コメント送れる番号か否か
                    LiveCommentAdd = true;
                    //ニコ生送るためのコメント番号
                    LiveCommentSendNum = 0;
                }

                //ニコ生コメントをVCIに送る関係
                nikoLiveCommentSendToVCI(message, comment_num, fix_handle, comment);
            }

            //コメント取得時の最終列
            if ((message == "{\"ping\":{\"content\":\"rf:0\"}}") && (!(bool)MAIN_COMPONENTS.configNikoLiveGetAllCommentSend.IsChecked))
            {
                //ニコ生コメント送れる番号か否か
                LiveCommentAdd = true;
                //ニコ生送るためのコメント番号
                LiveCommentSendNum = 0;
            }
        }

        //ニコ生コメントをVCIに送るやつ
        private void nikoLiveCommentSendToVCI(string message, string comment_num, string name, string comment)
        {
            //ニコ生コメント送れる番号ならば
            if (LiveCommentAdd)
            {
                //2度はない
                LiveCommentAdd = false;
                //コメントここから取得しはじめる
                LiveCommentSendNum = int.Parse(comment_num);
                //VCIにコメント送信する
                if (((bool)MAIN_COMPONENTS.configVCICommentSend.IsChecked) && ((bool)MAIN_COMPONENTS.configNikoLiveCommentSend.IsChecked) && (nikoLiveCommentAddEnable(comment)))
                {
                    string data = "if vci.assets.IsMine then vci.message.Emit(\"comment\",\"";
                    if ((bool)MAIN_COMPONENTS.configVCICommentSendAddBroadcast.IsChecked) data += "@Ni：";
                    if ((bool)MAIN_COMPONENTS.configVCICommentSendAddFixHandle.IsChecked) data += "@" + name + "：";
                    if ((bool)!MAIN_COMPONENTS.configNikoCommentLaw.IsChecked) comment = commentCorrect(comment);
                    comment.Replace("\"", "″");
                    data += comment + "\") end";

                    //コメント未処理分
                    commentQue.Enqueue(data + "\n");
                }
            }
            else if ((LiveCommentSendNum != 0) && (LiveCommentSendNum < int.Parse(comment_num)))
            {
                //VCIにコメント送信する
                if (((bool)MAIN_COMPONENTS.configVCICommentSend.IsChecked) && ((bool)MAIN_COMPONENTS.configNikoLiveCommentSend.IsChecked) && (nikoLiveCommentAddEnable(comment)))
                {
                    string data = "if vci.assets.IsMine then vci.message.Emit(\"comment\",\"";
                    if ((bool)MAIN_COMPONENTS.configVCICommentSendAddBroadcast.IsChecked) data += "@Ni：";
                    if ((bool)MAIN_COMPONENTS.configVCICommentSendAddFixHandle.IsChecked) data += "@" + name + "：";
                    if ((bool)!MAIN_COMPONENTS.configNikoCommentLaw.IsChecked) comment = commentCorrect(comment);
                    data += comment + "\") end";

                    //コメント未処理分
                    commentQue.Enqueue(data + "\n");
                }
            }
        }

        //コメント内容修正
        private string commentCorrect(string comment)
        {
            string correctComment = comment;
            int number = 0;
            if (correctComment.StartsWith("/gift vcast_free_"))
            {
                correctComment.Replace("/gift vcast_free_", "");

                number = correctComment.IndexOf(" \\\"");
                if (number > 0)
                {
                    correctComment = correctComment.Substring(number + " \\\"".Length);
                    number = correctComment.IndexOf("\\\" ");
                    if (number > 0)
                    {
                        string name = correctComment.Substring(0, number);
                        correctComment = correctComment.Substring(number + "\\\" ".Length);
                        number = correctComment.IndexOf("\\\"\\\" \\\"");
                        if (number > 0)
                        {
                            correctComment = correctComment.Substring(number + "\\\"\\\" \\\"".Length);
                            number = correctComment.IndexOf("\\\"");
                            if (number > 0)
                            {
                                string kind = correctComment.Substring(0, number);
                                return name + "さんから" + kind + "が送られました";
                            }
                            else return comment;
                        }
                        else return comment;
                    }
                    else return comment;
                }
                else return comment;
            }
            if (correctComment.StartsWith("/gift vcast_"))
            {
                correctComment.Replace("/gift vcast_", "");

                number = correctComment.IndexOf(" \\\"");
                if (number > 0)
                {
                    correctComment = correctComment.Substring(number + " \\\"".Length);
                    number = correctComment.IndexOf("\\\" ");
                    if (number > 0)
                    {
                        string name = correctComment.Substring(0, number);
                        correctComment = correctComment.Substring(number + "\\\" ".Length);
                        number = correctComment.IndexOf("\\\"\\\" \\\"");
                        if (number > 0)
                        {
                            correctComment = correctComment.Substring(number + "\\\"\\\" \\\"".Length);
                            number = correctComment.IndexOf("\\\"");
                            if (number > 0)
                            {
                                string kind = correctComment.Substring(0, number);
                                return name + "さんから" + kind + "が送られました";
                            }
                            else return comment;
                        }
                        else return comment;
                    }
                    else return comment;
                }
                else return comment;
            }
            if (correctComment.StartsWith("/nicoad {\\\"version\\\":"))
            {
                correctComment.Replace("/nicoad {\\\"version\\\":", "");

                number = correctComment.IndexOf("totalAdPoint\\\":");
                if (number > 0)
                {
                    correctComment = correctComment.Substring(number + "totalAdPoint\\\":".Length);
                    number = correctComment.IndexOf(",\\\"message\\\":\\\"");
                    if (number > 0)
                    {
                        string point = correctComment.Substring(0, number);
                        correctComment = correctComment.Substring(number + ",\\\"message\\\":\\\"".Length);
                        number = correctComment.IndexOf("\\\"");
                        if (number > 0)
                        {
                            string kind = correctComment.Substring(0, number);
                            return kind + " 合計" + point + "pt";
                        }
                        else return comment;
                    }
                    else return comment;
                }
                else return comment;
            }
            for (int count = 0; count < 50; count++)
            {
                if (comment.StartsWith("/info " + count.ToString() + " ")) return comment.Replace("/info " + count.ToString() + " ", "");
            }
            if (comment.StartsWith("/spi \\\""))
            {
                correctComment = correctComment.Replace("/spi \\\"", "");

                number = correctComment.LastIndexOf("\\\"");
                if (number > 0)
                {
                    return correctComment.Substring(0, number);
                }
                else return comment;
            }
            if (comment.StartsWith("/emotion ")) return comment.Replace("/emotion ", "");

            return comment;
        }

        //コメント内容送信可能
        private bool nikoLiveCommentAddEnable(string comment)
        {
            bool flg = true;
            //エモーション不許可
            if (comment.StartsWith(EMOTION_COMMENT))
            {
                if (!(bool)MAIN_COMPONENTS.configNikoLiveCommentEmotionSend.IsChecked)
                {
                    flg = false;
                }
                else return true;
            }
            //放送リクエスト不許可
            if (comment.StartsWith(REQUEST_COMMENT))
            {
                flg = false;
                if (!(bool)MAIN_COMPONENTS.configNikoLiveCommentRequestSend.IsChecked)
                {
                    return false;
                }
                else return true;
            }
            //好みで来る不許可
            if (comment.StartsWith(LIKECOME_COMMENT))
            {
                flg = false;
                if (!(bool)MAIN_COMPONENTS.configNikoLiveCommentLikeComeSend.IsChecked)
                {
                    return false;
                }
                else return true;
            }
            //無料ギフト不許可
            if (comment.StartsWith(FREEGIFT_COMMENT))
            {
                flg = false;
                if (!(bool)MAIN_COMPONENTS.configNikoLiveCommentFreeGiftSend.IsChecked)
                {
                    return false;
                }
                else return true;
            }
            //ギフト不許可
            if (comment.StartsWith(GIFT_COMMENT))
            {
                flg = false;
                if (!(bool)MAIN_COMPONENTS.configNikoLiveCommentGiftSend.IsChecked)
                {
                    return false;
                }
                else return true;
            }
            //広告不許可
            if (comment.StartsWith(ADVERTISEMENT_COMMENT) || comment.StartsWith(ADVERTISEMENT_INF_COMMENT))
            {
                flg = false;
                if (!(bool)MAIN_COMPONENTS.configNikoLiveCommentAdvertisementSend.IsChecked)
                {
                    return false;
                }
                else return true;
            }
            //通常コメントと認識
            if (flg)
            {
                //通常コメント不許可
                if (!(bool)MAIN_COMPONENTS.configNikoLiveDefaultCommentSend.IsChecked)
                {
                    return false;
                }
            }
            return flg;
        }

        //ニコ生接続状態のフラグ制御(切断等)
        private void nikoLiveDisconectedflgChange()
        {
            //接続待ち
            conecttingwait = false;
            //ニコ生接続解除
            conecttingLive = false;
            //ニコ生接続番組URL初期化
            conecttingProgramURL = "";
            //現在接続中のウェブソケットでコメント取得時必要
            conecttingLiveProgramWebsoketForComment = "";
            //ニコ生コメント送れる番号か否か
            LiveCommentAdd = false;
            //ニコ生送るためのコメント番号
            LiveCommentSendNum = 0;
        }

        //ニコ生接続状態のフラグ制御(接続等)
        private void nikoLiveConectedFlgChange(string threadID, string return_message)
        {
            //接続待ち
            conecttingwait = false;
            //接続時に前のデータ削除
            MAIN_COMPONENTS.nikoLiveCommentListView.Items.Clear();
            //ニコ生接続
            conecttingLive = true;
            //ニコ生接続番組URL初期化
            conecttingProgramURL = threadID;
            //現在接続中のウェブソケットでコメント取得時必要
            conecttingLiveProgramWebsoketForComment = return_message;
        }

        //ニコ生用ページであるか確認/修正
        private string correctURL(string url)
        {
            string correct_url = null;
            //番組情報から
            if (url.StartsWith("lv"))
            {
                correct_url = NIKOLIVE_VIEW_URL + url.Substring("lv".Length);
                return correct_url;
            }
            //ニコ生用ページである
            if (url.StartsWith(NIKOLIVE_VIEW_URL))
            {
                //url修正等
                int add_pos = -1;
                if ((add_pos = url.IndexOf("?")) != -1)
                {
                    correct_url = url.Substring(0, add_pos);
                }
                else correct_url = url;
            }
            //ニコ生ではないと判断
            else correct_url = null;

            return correct_url;
        }

        //HTML情報取得
        private string getHTML_INF(string url)
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
        private string getHTML_PartINF(string htmlSTR)
        {
            //生放送中以外含まれないため
            if (htmlSTR.Contains(NIKOLIVE_THREAD_PART))
            {
                //後半部分除去
                string trim_data2 = clsSupport.commentSplit(htmlSTR, NIKOLIVE_THREAD_PART, "&quot;");
                //後半部分除去
                string trim_data3 = clsSupport.commentSplit(htmlSTR, "frontendId&quot;:", ",&quot;");
                //HTML情報
                string html_partINF = NIKOLIVE_THREAD_PART + trim_data2 + "&frontend_id=" + trim_data3;
                return html_partINF;
            }
            else
            {
                MessageBox.Show("過去放送/公式放送には対応していません");
                return null;
            }
        }

        //切断挑戦
        public async void tryDisonnect()
        {
            //ニコ生接続中
            if (conecttingLive)
            {
                conecttingLive = false;
                //ニコ生にメッセージ
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