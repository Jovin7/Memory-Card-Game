using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    [Header("Level Info")]
    public GridLayoutGroup LayoutGroup;
    private int row;
    private int column;
    private int totalCardCount;
    [SerializeField] private Transform cardPrefab;
    [SerializeField] private Transform cardParent;

    [SerializeField] private Sprite[] allSprites;
    [SerializeField] private List<Sprite> gameSprites = new List<Sprite>();

    public bool isSelectedFirst, isSelectedSecond;
    public int firstCard, secondCard;

    [Header("Menu Canvas")]
    [SerializeField] private Transform levelButtonParent;
    [SerializeField] private List<Button> allButtons = new List<Button>();

    [Header("Card Details")]
    [SerializeField] private Sprite cardBg;
    void Start()
    {
        allSprites = Resources.LoadAll<Sprite>("Sprites");
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    public void SetGridLayoutValues(int rowColumnValue)
    {
        string numberString = rowColumnValue.ToString();

        row = int.Parse(numberString[0].ToString());
        column = int.Parse(numberString[1].ToString());
        LayoutGroup.constraintCount = row;

        totalCardCount = row * column;
    }
    public void GenerateCards()
    {
        for (int i = 0; i < totalCardCount; i++)
        {
            Transform a = Instantiate(cardPrefab, cardParent);
            a.name = i.ToString();
        
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
        for(int i =0;i<objects.Length;i++)
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
       
        foreach(Button t in allButtons)
        {
            t.onClick.RemoveAllListeners();
            t.onClick.AddListener(() => { OnButtonClicked(); });
        }
    }


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
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    public void OnButtonClicked()
    {
        string name = EventSystem.current.currentSelectedGameObject.name;
        Debug.Log("u have clicked the button "+ name);

        if (!isSelectedFirst)
        {
            isSelectedFirst = true;
            firstCard = int.Parse(name);
            allButtons[firstCard].transform.GetComponent<Image>().sprite = gameSprites[firstCard];
        }
        else if(!isSelectedSecond)
        {
            isSelectedSecond = true;
            secondCard = int.Parse(name);
            allButtons[secondCard].transform.GetComponent<Image>().sprite = gameSprites[secondCard];
        }

        if(isSelectedFirst && isSelectedSecond)
        {
            if(gameSprites[firstCard].name == gameSprites[secondCard].name)
            {
                Debug.Log("Match");
                allButtons[secondCard].transform.GetComponent<Image>().enabled = false;
                allButtons[firstCard].transform.GetComponent<Image>().enabled = false;

            }
            else
            {
                Debug.Log("not Match");
                allButtons[secondCard].transform.GetComponent<Image>().sprite = cardBg;
                allButtons[firstCard].transform.GetComponent<Image>().sprite = cardBg;
            }
            isSelectedFirst = isSelectedSecond = false;
            firstCard = secondCard = -1;
        }
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
