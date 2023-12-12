using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using WebSocketSharp;

public class DonationManager : MonoBehaviour
{
    public static DonationManager Instance = null;

    private WebSocket twip_ws;
    private WebSocket toonation_ws;

    private TwipMessage TwipMessage;
    private ToonationMessage ToonationMessage;

    private bool twipRecv;
    private bool toonationRecv;

    private string twipMsg;
    private string toonationMsg;

    [Serializable]
    public class Info
    {
        public bool use;
        public string url;
        [ReadOnly] public string token;
        [ReadOnly] public string version;
    }

    [Header("Twip 설정")]
    [SerializeField] private Info twip;

    [Header("Toonation 설정")]
    [SerializeField] private Info toonation;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        twip_ws = null;
        toonation_ws = null;

        TwipMessage = gameObject.AddComponent<TwipMessage>();
        ToonationMessage = gameObject.AddComponent<ToonationMessage>();

        twipRecv = false;
        toonationRecv = false;

        twipMsg = null;
        toonationMsg = null;

        StartCoroutine(GetToken());
    }

    private void Update()
    {
        if (twipRecv)
        {
            twipRecv = false;
            TwipMessage.Parser(twipMsg);
            twipMsg = null;
        }

        if (toonationRecv)
        {
            toonationRecv = false;
            ToonationMessage.Parser(toonationMsg);
            toonationMsg = null;
        }
    }

    private IEnumerator GetToken()
    {
        if (twip.use)
        {
            UnityWebRequest request = UnityWebRequest.Get(twip.url);
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError("Twip 호출 실패");
            }
            else
            {
                string token = request.downloadHandler.text;
                Match tokenMatch = Regex.Match(token, "window.__TOKEN__ = '(.+)';</script>");
                if (tokenMatch.Success && tokenMatch.Groups.Count > 1)
                {
                    twip.token = tokenMatch.Groups[1].Value;
                }
                else
                {
                    Debug.LogError("Twip 토큰 추출 실패");
                }

                string version = request.downloadHandler.text;
                Match versionMatch = Regex.Match(version, "version: \'(.+)\',");
                if (tokenMatch.Success && versionMatch.Groups.Count > 1)
                {
                    twip.version = versionMatch.Groups[1].Value;
                }
                else
                {
                    Debug.LogError("Twip 버전 추출 실패");
                }

                ConnectTwip();
            }
        }

        if (toonation.use)
        {
            UnityWebRequest request = UnityWebRequest.Get(toonation.url);
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError("Toonation 호출 실패");
            }
            else
            {
                string token = request.downloadHandler.text;
                Match tokenMatch = Regex.Match(token, "\"payload\"\\s*:\\s*\"([a-zA-Z0-9]+)\"");
                if (tokenMatch.Success && tokenMatch.Groups.Count > 1)
                {
                    toonation.token = tokenMatch.Groups[1].Value;
                    toonation.version = "NULL";
                }
                else
                {
                    Debug.LogError("Toonation 토큰 추출 실패");
                }

                ConnectToonation();
            }
        }
    }

    private void ConnectTwip()
    {
        if (toonation.token == null)
        {
            Debug.LogError("Twip 토큰 정보 없음");
            return;
        }

        try
        {
            string url = twip.url.Split('/').Last();
            string token = Uri.EscapeDataString(twip.token);
            string version = twip.version;
            string result = $"wss://io.mytwip.net/socket.io/?alertbox_key={url}&version={version}&token={token}&transport=websocket";

            twip_ws = new WebSocket(result);
            twip_ws.SslConfiguration.CheckCertificateRevocation = true;
            twip_ws.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;

            twip_ws.OnOpen += (sender, e) =>
            {
                Debug.Log("Twip 연결 성공");
                StartCoroutine(TwipPreventClose());
            };

            twip_ws.OnError += (sender, e) =>
            {
                Debug.LogError("Twip 에러 발생 : " + e.Message);
            };

            twip_ws.OnClose += (sender, e) =>
            {
                Debug.LogError("Twip 연결 끊김. 자동으로 재 연결 중...");
                Invoke("ConnectTwip", 10);
            };

            twip_ws.OnMessage += (sender, e) =>
            {
                twipMsg = e.Data;
                twipRecv = true;
            };

            twip_ws.Connect();
        }
        catch (Exception e)
        {
            Debug.LogError("Twip 연결 실패 : " + e.ToString());
        }
    }

    private void ConnectToonation()
    {
        if (toonation.token == null)
        {
            Debug.LogError("Toonation 토큰 정보 없음");
            return;
        }

        try
        {
            string result = $"wss://toon.at:8071/{toonation.token}";

            toonation_ws = new WebSocket(result);
            toonation_ws.SslConfiguration.CheckCertificateRevocation = true;
            toonation_ws.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;

            toonation_ws.OnOpen += (sender, e) =>
            {
                Debug.Log("Toonation 연결 성공");
                StartCoroutine(ToonationPreventClose());
            };

            toonation_ws.OnError += (sender, e) =>
            {
                Debug.LogError("Toonation 에러 발생 : " + e.Message);
            };

            toonation_ws.OnClose += (sender, e) =>
            {
                Debug.LogError("Toonation 연결 끊김. 자동으로 재 연결 중...");
                Invoke("ConnectTwip", 10);
            };

            toonation_ws.OnMessage += (sender, e) =>
            {
                toonationMsg = e.Data;
                toonationRecv = true;
            };

            toonation_ws.Connect();
        }
        catch (Exception e)
        {
            Debug.LogError("Toonation 연결 실패 : " + e);
        }
    }

    private IEnumerator TwipPreventClose()
    {
        while (true)
        {
            if (twip_ws != null && twip_ws.ReadyState == WebSocketState.Open)
            {
                twip_ws.Send("2");
                yield return new WaitForSeconds(22);
            }
            else
            {
                break;
            }
        }
    }

    private IEnumerator ToonationPreventClose()
    {
        while (true)
        {
            if (toonation_ws != null && toonation_ws.ReadyState == WebSocketState.Open)
            {
                toonation_ws.Ping();
                yield return new WaitForSeconds(12);
            }
            else
            {
                break;
            }
        }
    }
}