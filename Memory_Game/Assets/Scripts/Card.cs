using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public int cardId =-1;
    private bool coroutineAllowed, facedUp;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public IEnumerator RotateCard(Transform card, Sprite open,Sprite close)
    {
        coroutineAllowed = false;

        if (!facedUp)
        {
            Debug.Log("!facedUp");
            for (float i = 180f; i >= 0f; i -= 10f)
            {
                card.localRotation = Quaternion.Euler(0f, i, 0f);
                if (i == 90f)
                {
                    card.GetComponent<Image>().sprite = open;
                }
                yield return new WaitForSeconds(0.001f);
            }
        }

        else if (facedUp)
        {
            for (float i = 180f; i >= 0f; i -= 10f)
            {
                card.localRotation = Quaternion.Euler(0f, i, 0f);
                if (i == 90f)
                {
                    card.GetComponent<Image>().sprite = close;
                }
                yield return new WaitForSeconds(0.001f);
            }
        }

        coroutineAllowed = true;

        facedUp = !facedUp;
    }
}
