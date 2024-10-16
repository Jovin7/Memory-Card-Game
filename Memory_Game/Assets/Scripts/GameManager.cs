using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Level Manager")]
    public int[] level;
    public TextMeshProUGUI[] highScore;
    public int levelCount = 0;
    public int totalMatchCount = 0;

    [Header("Level Info")]
    public GridLayoutGroup LayoutGroup;
    private int row;
    private int column;
    private int totalCardCount;
    [SerializeField] private Transform cardPrefab;
    [SerializeField] private Transform cardParent;
    private string filePath;
    [SerializeField] private Sprite[] allSprites;
    [SerializeField] private List<Sprite> gameSprites = new List<Sprite>();

    public bool isSelectedFirst, isSelectedSecond;
    public int firstCard, secondCard;

    public Queue<int> numberQueue = new Queue<int>();
    [Header("Canvas")]
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private GameObject gameOverCanvas;

    [Header("Menu Canvas")]
    [SerializeField] private Button StartButton;
    [SerializeField] private GameObject mask;
    [SerializeField] private Transform levelButtonParent;
    [SerializeField] private List<Button> allButtons = new List<Button>();

    [Header("Card Details")]
    [SerializeField] private Sprite cardBg;
    [SerializeField]
    private Sprite faceSprite, backSprite;

    [Header("Score")]
    [SerializeField] private TextMeshProUGUI matchCountText;
    [SerializeField] private TextMeshProUGUI turnCountText;
    [SerializeField] private TextMeshProUGUI ScoreText;
    private int matchCount;
    private int turnCount;
    private int Score;
    [Header("Audio ")]
    [SerializeField] private AudioSource audioSource;
    //[SerializeField] private AudioClip buttonClick;

    [Header("Save Data")]

     public LevelDataList saveDataList = new LevelDataList();
   // public List<LevelData> levelDataList = new List<LevelData>();


    private string filepath;

    [Header("Load Data")]
    public LevelDataList loadedData;
    void Start()
    {
       
        filepath = Path.Combine(Application.persistentDataPath, "emptyData.json");
        
        

        allSprites = Resources.LoadAll<Sprite>("Sprites");
        GenerateLevel();
    }
    public void GenerateLevel()
    {
        turnCount = 0;
        matchCount = 0;
        matchCountText.text = "0";
        turnCountText.text = "0";
        while (numberQueue.Count > 0)
        {
            numberQueue.Dequeue(); // Remove elements one by one
        }


        SetGridLayoutValues(level[levelCount]);
        EnableHighlight(levelCount);
        levelCount++;
    }
    public void NextLevel()
    {
        DestroyCards();
        GenerateLevel();
        gameCanvas.SetActive(true);
        gameOverCanvas.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
            LoadJson();
    }
    public void SetGridLayoutValues(int rowColumnValue)
    {
        string numberString = rowColumnValue.ToString();

        row = int.Parse(numberString[0].ToString());
        column = int.Parse(numberString[1].ToString());
        LayoutGroup.constraintCount = row;

        totalCardCount = row * column;
        StartButton.gameObject.SetActive(true);
        totalMatchCount = totalCardCount / 2;
    }

    public void GenerateCards()
    {
        for (int i = 0; i < totalCardCount; i++)
        {
            Transform a = Instantiate(cardPrefab, cardParent);
            a.name = i.ToString();
            a.GetComponent<Card>().cardId = i;
            a.gameObject.tag = "PuzzleButton";

        }
        GetButton();
        AddListeners();
        AddGameSprites();
        Shuffle(gameSprites);
    }
    public void GetButton()
    {
        allButtons.Clear();
        gameSprites.Clear();
        GameObject[] objects = GameObject.FindGameObjectsWithTag("PuzzleButton");
        for (int i = 0; i < objects.Length; i++)
        {
            Button btn = objects[i].GetComponent<Button>();
            if (btn != null)
            {
                allButtons.Add(btn);
            }
            else
            {
                Debug.LogWarning("Object " + objects[i].name + " does not have a Button component!");
            }
        }
    }
    public void AddListeners()
    {
        foreach (Button t in allButtons)
        {
            t.onClick.RemoveAllListeners();
            t.onClick.AddListener(() => { ButtonClickTest(); });
        }
    }

    // RotateCard(allButtons[firstCard].transform, gameSprites[firstCard]);
    //StartCoroutine(RotateCard(allButtons[firstCard].transform, gameSprites[firstCard]));
    void AddGameSprites()
    {
        int total = allButtons.Count;
        int index = 0;
        for (int i = 0; i < total; i++)
        {
            if (index == total / 2)
                index = 0;


            gameSprites.Add(allSprites[index]);
            index++;
        }
    }
    void Shuffle<T>(List<T> list)
    {
        int n = list.Count;

        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    public void OnButtonClicked()
    {
        string name = EventSystem.current.currentSelectedGameObject.name;



        if (!isSelectedFirst)
        {
            Debug.Log("1111");
            isSelectedFirst = true;
            firstCard = int.Parse(name);
            numberQueue.Enqueue(firstCard);
            allButtons[firstCard].transform.GetComponent<Image>().sprite = gameSprites[firstCard];

        }
        else if (!isSelectedSecond)
        {
            Debug.Log("22222");
            isSelectedSecond = true;
            secondCard = int.Parse(name);
            numberQueue.Enqueue(secondCard);
            allButtons[secondCard].transform.GetComponent<Image>().sprite = gameSprites[secondCard];
        }

        if (isSelectedFirst && isSelectedSecond)
        {
            Debug.Log("3333");

            if (gameSprites[firstCard].name == gameSprites[secondCard].name)
            {

                Debug.Log("Match44444");
                StartWaiting(2, () =>
                {
                    allButtons[secondCard].transform.GetComponent<Image>().enabled = false;
                    allButtons[firstCard].transform.GetComponent<Image>().enabled = false;
                    isSelectedFirst = isSelectedSecond = false;
                    firstCard = secondCard = -1;

                });


            }
            else
            {
                Debug.Log("not Match55555555555");
                StartWaiting(2, () =>
                {
                    allButtons[secondCard].transform.GetComponent<Image>().sprite = cardBg;
                    allButtons[firstCard].transform.GetComponent<Image>().sprite = cardBg;
                    isSelectedFirst = isSelectedSecond = false;
                    firstCard = secondCard = -1;
                    //  mask.SetActive(false);
                });

            }

        }
    }
    public void ButtonClickTest()
    {
        string name = EventSystem.current.currentSelectedGameObject.name;

        Debug.Log("1111");

        int cardIndex = int.Parse(name);
        numberQueue.Enqueue(cardIndex);
        allButtons[cardIndex].transform.GetComponent<Image>().sprite = gameSprites[cardIndex];
        Comparison();



    }
    void Comparison()
    {
        Debug.Log(" Comparison");
        if (numberQueue.Count < 2)
        {
            Debug.Log(" no Comparison");
            return;
        }

        int firstElement = numberQueue.Dequeue();


        int secondElement = numberQueue.Dequeue();

        turnCount++;
        turnCountText.text = turnCount.ToString();

        if (gameSprites[firstElement].name == gameSprites[secondElement].name)
        {
            matchCount++;
            matchCountText.text = matchCount.ToString();

            StartWaiting(1, () =>
            {
                allButtons[firstElement].transform.GetComponent<Image>().enabled = false;
                allButtons[secondElement].transform.GetComponent<Image>().enabled = false;

                totalMatchCount--;
                if (totalMatchCount == 0)
                {
                    Debug.Log("GameOver");
                    gameCanvas.SetActive(false);
                    gameOverCanvas.SetActive(true);
                    Score = matchCount * 10;
                    ScoreText.text = Score.ToString();


                    LevelData levelData = new LevelData(levelCount, Score);
                    saveDataList.data.Add(levelData);
                    //levelDataList.Add(levelData);
                    // SaveData(datat);
                    Save(saveDataList);
                }

            });
        }
        else
        {
            Debug.Log("doesnot match");
            StartWaiting(1, () =>
            {
                allButtons[firstElement].transform.GetComponent<Image>().sprite = cardBg;
                allButtons[secondElement].transform.GetComponent<Image>().sprite = cardBg;

            });

        }

    }

    public void Save(LevelDataList listdata)
    {
        string json = JsonUtility.ToJson(listdata, true);
        File.WriteAllText(filepath, json);
    }
    public void LoadJson()
    {
        string json = File.ReadAllText(filepath);
        LevelDataList levelData = JsonUtility.FromJson<LevelDataList>(json);
        loadedData.data = levelData.data;
        

    }
    void StartWaiting(float timeToWait, Action action)
    {
        StartCoroutine(Wait(timeToWait, action));
    }

    IEnumerator Wait(float timeToWait, Action action)
    {
        yield return new WaitForSeconds(timeToWait);
        action?.Invoke();
    }

    public void DestroyCards()
    {
        for (int i = 0; i < cardParent.childCount; i++)
        {
            Destroy(cardParent.GetChild(i).gameObject);
        }
    }

    public void EnableHighlight(int index)
    {
        for (int i = 0; i < levelButtonParent.childCount; i++)
        {
            if (i == index)
                levelButtonParent.GetChild(i).GetChild(0).gameObject.SetActive(true);
            else
                levelButtonParent.GetChild(i).GetChild(0).gameObject.SetActive(false);
        }
    }
    public void DisableHighlight()
    {
        foreach (Transform a in levelButtonParent.transform)
            a.GetChild(0).gameObject.SetActive(false);

    }





}

[System.Serializable]
public class LevelDataList
{
    public List<LevelData> data = new List<LevelData>();
}

[System.Serializable]
public class LevelData
{
    public int level;
    public int playerScore;



    public LevelData(int _level, int score)
    {
        level = _level;
        playerScore = score;
    }
}


