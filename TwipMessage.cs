using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TwipMessage : MonoBehaviour
{
    [Serializable]
    public class TwipData
    {
        public string _id;
        public string nickname;
        public int amount;
        public string comment;
        public string watcher_id;
        public bool subbed;
        public bool repeat;
        public string ttstype;
        public string[] ttsurl;
        public object slotmachine_data;
        public Dictionary<string, object> effect;
        public string variation_id;
    }

    public void Parser(string s)
    {
        if (s == null) return;

        if (s.IndexOf('[') > 0)
        {
            string msg = s.Substring(s.IndexOf('['));

            int jsonStartIndex = msg.IndexOf("{");
            int jsonEndIndex = msg.LastIndexOf("}");

            if (jsonStartIndex >= 0 && jsonEndIndex >= 0)
            {
                string jsonPart = msg.Substring(jsonStartIndex, jsonEndIndex - jsonStartIndex + 1);
                string eventName = msg.Substring(2, jsonStartIndex - 4);

                TwipData donation = JsonConvert.DeserializeObject<TwipData>(jsonPart);

                if (eventName == "new donate")
                {
                    Debug.Log("Twip 도네 닉네임 : " + donation.nickname);
                    Debug.Log("Twip 도네 금액 : " + donation.amount.ToString());
                    Debug.Log("Twip 도네 내용 : " + donation.comment);
                }
            }
        }
    }
}
