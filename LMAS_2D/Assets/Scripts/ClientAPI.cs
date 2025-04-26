using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class ClientAPI : MonoBehaviour
{
    [SerializeField] private string apiUrl = "https://localhost:8000";
    [SerializeField] private string playerId = "player1";
    
    // HTTP로 테스트하려면 이 플래그를 true로 설정
    [SerializeField] private bool useHttpInstead = true;

    private string ActualApiUrl => useHttpInstead ? apiUrl.Replace("https://", "http://") : apiUrl;

    [Serializable]
    private class Position
    {
        public float x;
        public float y;
        public float z;
    }

    [Serializable]
    private class GameData
    {
        public string player_id;
        public int score;
        public Position position;
    }

    [Serializable]
    private class GameResponse
    {
        public string status;
        public string message;
        public Dictionary<string, object> data;
    }

    void Start()
    {
        // 서버 연결 테스트
        StartCoroutine(TestServerConnection());
    }

    IEnumerator TestServerConnection()
    {
        Debug.Log($"연결 시도 중: {ActualApiUrl}");
        
        using (UnityWebRequest webRequest = UnityWebRequest.Get(ActualApiUrl))
        {
            // HTTPS 인증서 검증 무시 (자체 서명 인증서 사용 시)
            if (!useHttpInstead)
            {
                webRequest.certificateHandler = new BypassCertificate();
            }
            
            // 타임아웃 설정
            webRequest.timeout = 10;
            
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {webRequest.error}");
                Debug.LogError($"Response Code: {webRequest.responseCode}");
            }
            else
            {
                Debug.Log($"Server Response: {webRequest.downloadHandler.text}");
                // 연결 성공 후 플레이어 데이터 저장
                SavePlayerData(100, new Vector3(1, 2, 3));
            }
        }
    }

    public void SavePlayerData(int score, Vector3 position)
    {
        StartCoroutine(PostPlayerData(score, position));
    }

    IEnumerator PostPlayerData(int score, Vector3 position)
    {
        GameData data = new GameData
        {
            player_id = playerId,
            score = score,
            position = new Position { x = position.x, y = position.y, z = position.z }
        };

        string jsonData = JsonConvert.SerializeObject(data);
        
        using (UnityWebRequest webRequest = new UnityWebRequest($"{ActualApiUrl}/player", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            
            // HTTPS 인증서 검증 무시
            if (!useHttpInstead)
            {
                webRequest.certificateHandler = new BypassCertificate();
            }
            
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {webRequest.error}");
            }
            else
            {
                GameResponse response = JsonConvert.DeserializeObject<GameResponse>(webRequest.downloadHandler.text);
                Debug.Log($"Player data saved: {response.message}");
            }
        }
    }

    public void GetPlayerData()
    {
        StartCoroutine(FetchPlayerData());
    }

    IEnumerator FetchPlayerData()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get($"{ActualApiUrl}/player/{playerId}"))
        {
            if (!useHttpInstead)
            {
                webRequest.certificateHandler = new BypassCertificate();
            }
            
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {webRequest.error}");
            }
            else
            {
                GameResponse response = JsonConvert.DeserializeObject<GameResponse>(webRequest.downloadHandler.text);
                Debug.Log($"Player data received: {webRequest.downloadHandler.text}");
            }
        }
    }

    public void UpdatePlayerData(int score, Vector3 position)
    {
        StartCoroutine(PutPlayerData(score, position));
    }

    IEnumerator PutPlayerData(int score, Vector3 position)
    {
        GameData data = new GameData
        {
            player_id = playerId,
            score = score,
            position = new Position { x = position.x, y = position.y, z = position.z }
        };

        string jsonData = JsonConvert.SerializeObject(data);
        
        using (UnityWebRequest webRequest = new UnityWebRequest($"{ActualApiUrl}/player/{playerId}", "PUT"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            
            if (!useHttpInstead)
            {
                webRequest.certificateHandler = new BypassCertificate();
            }
            
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {webRequest.error}");
            }
            else
            {
                GameResponse response = JsonConvert.DeserializeObject<GameResponse>(webRequest.downloadHandler.text);
                Debug.Log($"Player data updated: {response.message}");
            }
        }
    }
}

// HTTPS 인증서 검증을 우회하는 클래스 (개발 환경에서만 사용)
public class BypassCertificate : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        // 모든 인증서 허용 (개발 용도로만 사용)
        return true;
    }
}