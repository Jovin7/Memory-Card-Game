using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public int cardId = -1;

    private bool isSelected;
    // Start is called before the first frame update
    void Start()
    {

    }

    public IEnumerator RotateCard(Transform card, Sprite open, Sprite close)
    {
        Debug.Log("!isSelected");
        for (float i = 180f; i >= 0f; i -= 10f)
        {
            card.localRotation = Quaternion.Euler(0f, i, 0f);
            if (i == 90f)
            {
                if (!isSelected)
                    card.GetComponent<Image>().sprite = open;
                else
                    card.GetComponent<Image>().sprite = close;
            }
            yield return new WaitForSeconds(0.001f);
        }
        isSelected = !isSelected;

    }


    public IEnumerator FadeCard(Image cardImage, float duration)
    {
        Color originalColor = cardImage.color;  // Get the original color of the card
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, timeElapsed / duration);  // Gradually reduce alpha from 1 to 0
            cardImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);  // Set new alpha value
            timeElapsed += Time.deltaTime;
            yield return null;  // Wait until the next frame
        }

        // Ensure alpha is set to 0 at the end
        cardImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
    }
}
