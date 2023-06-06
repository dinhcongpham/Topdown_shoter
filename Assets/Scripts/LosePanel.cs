using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;

public class LosePanel : MonoBehaviour
{
    [System.Serializable]
    public class PlayerData
    {
        public List<ScoreData> scores = new List<ScoreData>();
    }

    [System.Serializable]
    public class ScoreData
    {
        public string time;
        public int score;
    }
    public TextMeshProUGUI score;
    public TextMeshProUGUI name_user;
    public TextMeshProUGUI hightestscore;
    public TextMeshProUGUI list_high_score;
    private int scoreI;
    public string displayname;

    private void Start()
    {
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        scoreI = FindObjectOfType<Killed>().currentKilled * 10;
        int second = FindObjectOfType<Timer>().second;
        int minute = FindObjectOfType<Timer>().minute;
        string time = minute.ToString() + ":" + second.ToString();
        GetAccountInfo();
        UpdateUserData(time, scoreI);
        SubmitScoreLeaderBoard(scoreI);
        GetTopThreePlayers();
        Time.timeScale = 0;
    }

    public void GetAccountInfo()
    {
        var request = new GetAccountInfoRequest();
        PlayFabClientAPI.GetAccountInfo(request, OnGetAccountInfoSuccess, OnError);
    }

    private void OnGetAccountInfoSuccess(GetAccountInfoResult result)
    {
        displayname = result.AccountInfo.TitleInfo.DisplayName;
        name_user.text = "Name: " + displayname;
    }

    public void UpdateUserData(string time, int score)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
                {
                    { "Time", time },
                    { "Score", score.ToString() }
                },
            Permission = UserDataPermission.Public
        };

        PlayFabClientAPI.UpdateUserData(request, OnUpdateUserDataSuccess, OnError);
    }

    private void OnUpdateUserDataSuccess(UpdateUserDataResult result)
    {
        score.text = "You get: " + scoreI.ToString() + " Score In This Turn";
        hightestscore.text = "Your highest score is: " + scoreI;
    }

    private void OnError(PlayFabError error)
    {

    }

    public void SubmitScoreLeaderBoard(int playerScore)
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate> {
            new StatisticUpdate {
                StatisticName = "Score",
                Value = playerScore
            }
        }
        }, result => OnStatisticsUpdated(result), FailureCallback);
    }

    private void OnStatisticsUpdated(UpdatePlayerStatisticsResult updateResult)
    {
        Debug.Log("Successfully submitted high score");
    }

    private void FailureCallback(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your API call. Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }


    private void GetTopThreePlayers()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "Score",
            StartPosition = 0,
            MaxResultsCount = 3
        };

        PlayFabClientAPI.GetLeaderboard(request, OnGetLeaderboardSuccess, OnError);
    }

    private void OnGetLeaderboardSuccess(GetLeaderboardResult result)
    {
        if (result.Leaderboard != null && result.Leaderboard.Count > 0)
        {
            for (int i = 0; i < result.Leaderboard.Count; i++)
            {
                var player = result.Leaderboard[i];
                string playerName = player.DisplayName;
                string playerScore = player.StatValue.ToString();
                if (displayname == playerName)
                    if(scoreI >= int.Parse(playerScore))
                    {
                        playerScore = scoreI.ToString();
                        hightestscore.text = "Your highest score is: " + playerScore; 
                    }
                    else
                    {
                        hightestscore.text = "Your highest score is: " + playerScore;
                    }
                list_high_score.text += "Player " + (i + 1) + ": " + playerName + " - Score: " + playerScore + "\n";
            }
        }
        else
        {
            Debug.Log("fail to get data");
        }
    }

    public void Hide()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
