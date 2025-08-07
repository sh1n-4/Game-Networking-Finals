using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using Newtonsoft.Json;

public class test : MonoBehaviour
{
    [Header("Player Details")]
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private TMP_InputField playerHealthInput;
    [SerializeField] private TMP_InputField playerScoreInput;
    [SerializeField] private TMP_InputField playerPosXInput;
    [SerializeField] private TMP_InputField playerPosYInput;
    [SerializeField] private TMP_InputField playerPosZInput;

    [Header("Buttons")]
    [SerializeField] private Button getDataButton;
    [SerializeField] private Button updateDataButton;


    private string apiUrl = "http://localhost:5277/api/Data";
    private PlayerData currentData;

    void Start()
    {
        if (getDataButton != null) getDataButton.onClick.AddListener(FetchData);
        if (updateDataButton != null) updateDataButton.onClick.AddListener(UpdateData);
    }

    public void FetchData()
    {
        StartCoroutine(GetPlayerDataCoroutine());
    }

    public void UpdateData()
    {
        int health = 0;
        int playerScore = 0;
        float posX = 0f, posY = 0f, posZ = 0f;

        int.TryParse(playerHealthInput.text, out health);
        int.TryParse(playerScoreInput.text, out playerScore);
        float.TryParse(playerPosXInput.text, out posX);
        float.TryParse(playerPosYInput.text, out posY);
        float.TryParse(playerPosZInput.text, out posZ);

        PlayerData data = new PlayerData
        {
            playerName = playerNameInput.text,
            playerHealth = health,
            score = playerScore,
            playerPosition = new PlayerPosition { X = posX, Y = posY, Z = posZ }
        };
        StartCoroutine(PostPlayerDataCoroutine(data));
        ClearData();
    }

    void ClearData()
    {
        playerNameInput.text = "";
        playerHealthInput.text = "";
        playerScoreInput.text = "";
        playerPosXInput.text = "";
        playerPosYInput.text = "";
        playerPosZInput.text = "";
    }

    IEnumerator GetPlayerDataCoroutine()
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("GET Error: " + request.error);
        }
        else
        {
            string json = request.downloadHandler.text;
            Debug.Log("Received JSON: " + json);
            currentData = JsonConvert.DeserializeObject<PlayerData>(json);


            playerNameInput.text = currentData.playerName;
            playerHealthInput.text = currentData.playerHealth.ToString();
            playerScoreInput.text = currentData.score.ToString();
            playerPosXInput.text = currentData.playerPosition.X.ToString();
            playerPosYInput.text = currentData.playerPosition.Y.ToString();
            playerPosZInput.text = currentData.playerPosition.Z.ToString();
        }
    }

    IEnumerator PostPlayerDataCoroutine(PlayerData data)
    {
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        Debug.Log("Sending JSON: " + json);

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"POST Error: {request.error}");
            Debug.LogError($"Response Code: {request.responseCode}");
            Debug.LogError($"Response Body: {request.downloadHandler.text}");
            Debug.LogError($"Sent JSON: {json}");
        }
        else
        {
            Debug.Log("Update successful!");
            Debug.Log("Server response: " + request.downloadHandler.text);
        }
    }
}

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int playerHealth; 
    public int score;
    public PlayerPosition playerPosition;
}

[System.Serializable]
public class PlayerPosition
{
    public float X;
    public float Y;
    public float Z;
}