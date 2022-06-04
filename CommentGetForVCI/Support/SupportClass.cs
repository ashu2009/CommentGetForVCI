

namespace CommentGetForVCI
{
    class SupportClass
    {
        //User-Agent指定用
        public string USER_AGENT = "User-Agent";
        public string USER_AGENT_BROWSER = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:100.0) Gecko/20100101 Firefox/100.0";
        //コメント保存ファイル名
        public string COMMENT_NIKOLIVE_FILENAME = "NikoLiveComment.txt";
        public string COMMENT_SHOWROOM_FILENAME = "SHOWROOMComment.txt";
        //コテハン保存ファイル名
        public string FIXHANDLE_NIKOLIVE_FILENAME = "NikoLiveFixHandle.txt";
        public string FIXHANDLE_SHOWROOM_FILENAME = "SHOWROOMFixHandle.txt";


        //ニコ生コメント分割(コメント取得専用)
        public string commentSplit(string message, string split_start)
        {
            int string_pos = message.IndexOf(split_start);
            string string_data = message.Substring(string_pos + split_start.Length, message.Length - 3 - (string_pos + split_start.Length));
            return string_data;
        }

        //コメント分割
        public string commentSplit(string message, string split_start, string split_end)
        {
            int string_pos = message.IndexOf(split_start);
            string string_before = message.Substring(string_pos + split_start.Length);
            if (split_end == "") return string_before;
            string_pos = string_before.IndexOf(split_end);
            string string_data = string_before.Substring(0, string_pos);
            return string_data;
        }
    }
}