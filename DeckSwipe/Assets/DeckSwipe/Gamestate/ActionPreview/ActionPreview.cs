using DeckSwipe.CardModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionPreview : MonoBehaviour
{
    private Color darkGreen = new Color(0,0.7f,0);
    private Color lightGreen = new Color(0, 1f, 0);
    private Color darkRed = new Color(0.7f, 0, 0);
    private Color lightRed = new Color(1f, 0, 0);

    [SerializeField]
    private CanvasGroup leftPreview;

    [SerializeField]
    private CanvasGroup rightPreview;

    [SerializeField]
    private Image leftTopImage;

    [SerializeField]
    private Image leftBottomImage;

    [SerializeField]
    private Image rightTopImage;

    [SerializeField]
    private Image rightBotomImage;

    public void ChangeLeftActionPreview(float currentStat, float outcomeStat)
    {
        ChangeActionPreview(currentStat, outcomeStat, leftBottomImage, leftTopImage, leftPreview);
    }

    public void ChangeRightActionPreview(float currentStat, float outcomeStat)
    {
        ChangeActionPreview(currentStat, outcomeStat, rightBotomImage, rightTopImage, rightPreview);
    }

    private void ChangeActionPreview(float currentStat, float outcomeStat, Image bottomImage, Image topImage, CanvasGroup preview)
    {
        preview.gameObject.SetActive(true);
        if (currentStat > outcomeStat)
        {
            bottomImage.fillAmount = outcomeStat;
            topImage.fillAmount = currentStat;
            bottomImage.color = lightRed;
            topImage.color = darkRed;
        }
        else if (currentStat < outcomeStat)
        {
            bottomImage.fillAmount = currentStat;
            topImage.fillAmount = outcomeStat;
            bottomImage.color = darkGreen;
            topImage.color = lightGreen;
        }
        else
        {
            preview.gameObject.SetActive(false);
        }
    }

    public void UpdateActionPreviewAlpha(float leftAlpha, float rightAlpha)
    {
        leftPreview.alpha = leftAlpha;
        rightPreview.alpha = rightAlpha;
    }
}
