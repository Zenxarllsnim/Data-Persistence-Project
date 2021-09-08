using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text Name;
    public GameObject NameInput;
    public GameObject GameOverText;
    public GameObject GameOverTextHighScore;
    public Text HighScore;
    public static MainManager Instance;

    private bool highflag = false;
    private bool m_Started = false;
    private int m_Points;
    private int high_score;
    private string highname;
    
    private bool m_GameOver = false;


    // Start is called before the first frame update

    private void Awake()
    {
        LoadScore();
      
    }
    void Start()
    {
        
        UpdateHighScore();
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
             if (Input.GetKeyDown(KeyCode.Home))
            {
                high_score = 0;
                highname = "Reset";
                UpdateHighScore();
                SaveScore();
                
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                highname = Name.text;
                NameInput.SetActive(false);
                UpdateHighScore();
                SaveScore();
                
                        
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
        if (m_Points >= high_score)
        {
            high_score = m_Points;
                        
            highflag = true;
        }
    }


    public void UpdateHighScore()
    {
        if (highname != "") {
            HighScore.text = "HighScore: " + high_score + " by " + highname;
        }
        else
        { HighScore.text = "HighScore: " + high_score; }

    }
    public void GameOver()
    {
        m_GameOver = true;
        if (highflag == false)
        {
            GameOverText.SetActive(true);
        }
        else if (highflag == true)
        {
            GameOverTextHighScore.SetActive(true);
            NameInput.SetActive(true);
            
            
            SaveScore();
           
        }
    }
    class SaveData
    {
        public int highscoresave;
        public string highname;
    }
    public void SaveScore()
    {
        SaveData data = new SaveData();
        data.highscoresave = high_score;
        data.highname = highname;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadScore()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            high_score = data.highscoresave;
            highname = data.highname;
        }
    }



}
