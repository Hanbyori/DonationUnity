using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ToonationMessage : MonoBehaviour
{
    [Serializable]
    public class ToonationData
    {
        public string message;
        public int amount;
        public string uid;
        public string account;
        public string name;
        public string image;
        public int acctype;
        public int test_noti;
        public int level;
        public string tts_locale;
        public string tts_provider;
        public int conf_idx;
        public string rec_link;
        public object video_info;
        public int video_begin;
        public int video_length;
        public string tts_link;
    }

    public void Parser(string s)
    {
        if (s == null) return;
        if (!s.Contains("code")) return;

        Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(s);

        string contentJson = JsonConvert.SerializeObject(data["content"]);
        ToonationData donation = JsonConvert.DeserializeObject<ToonationData>(contentJson);

        int eventIdx = Convert.ToInt32(data["code"]);

        if (eventIdx == 101)
        {
            // 여기에 이벤트 작성
            Debug.Log("Toonation 도네 닉네임 : " + donation.name);
            Debug.Log("Toonation 도네 금액 : " + donation.amount.ToString());
            Debug.Log("Toonation 도네 내용 : " + donation.message);
        }
    }
}
