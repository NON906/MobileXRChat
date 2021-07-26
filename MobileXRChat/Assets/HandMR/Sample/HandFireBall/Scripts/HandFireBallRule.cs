using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HandMR.HandFireBall
{
    public class HandFireBallRule : MonoBehaviour
    {
        enum ModeEnum
        {
            START,
            PLAY,
            RESULT,
        }

        public GameObject StartPrefab;
        public GameObject PlayPrefab;
        public GameObject ResultPrefab;

        ModeEnum mode_;

        public int Score
        {
            private set;
            get;
        } = 0;

        IEnumerator Start()
        {
            Physics.IgnoreLayerCollision(0, LayerMask.NameToLayer("PlaySpaceWall"));

            for (; ; )
            {
                if (mode_ == ModeEnum.START)
                {
                    GameObject startObj = Instantiate(StartPrefab);
                    startObj.GetComponentInChildren<Target>().BreakTarget.AddListener(BreakStartTarget);

                    while (mode_ == ModeEnum.START)
                    {
                        yield return null;
                    }

                    Destroy(startObj);
                }
                else if (mode_ == ModeEnum.PLAY)
                {
                    Score = 0;

                    GameObject playObj = Instantiate(PlayPrefab);
                    playObj.GetComponentInChildren<SpawnTarget>().AddScore = AddScore;

                    float remainingTime = 30f;
                    Text timeText = playObj.GetComponentInChildren<Text>();

                    while (remainingTime > 0f)
                    {
                        remainingTime -= Time.deltaTime;

                        timeText.text = string.Format("{0:00.00}", remainingTime);

                        yield return null;
                    }

                    Destroy(playObj);

                    mode_ = ModeEnum.RESULT;
                }
                else if (mode_ == ModeEnum.RESULT)
                {
                    GameObject resultObj = Instantiate(ResultPrefab);
                    resultObj.GetComponentInChildren<Target>().BreakTarget.AddListener(BreakStartTarget);

                    bool newRecord = false;
                    int hiScore = PlayerPrefs.GetInt("HiScore", 0);

                    if (hiScore < Score)
                    {
                        PlayerPrefs.SetInt("HiScore", Score);
                        PlayerPrefs.Save();

                        newRecord = true;
                    }

                    string textStr = "Score: " + Score;
                    if (newRecord)
                    {
                        textStr += "(New Record)";
                    }
                    textStr += "\n";
                    textStr += "High Score: " + hiScore + "\n";
                    textStr += "↓Restart↓";
                    resultObj.GetComponentInChildren<Text>().text = textStr;

                    while (mode_ == ModeEnum.RESULT)
                    {
                        yield return null;
                    }

                    Destroy(resultObj);
                }

                SpawnBullet.ClearBullets();
            }
        }

        public void BreakStartTarget()
        {
            mode_ = ModeEnum.PLAY;
        }

        public void AddScore()
        {
            Score += 10;
        }
    }
}
