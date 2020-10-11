using DeckSwipe.CardModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionPreview : MonoBehaviour
{
    private Color darkGreen = new Color(0,0.6f,0);
    private Color lightGreen = new Color(0, 1f, 0);
    private Color darkRed = new Color(0.6f, 0, 0);
    private Color lightRed = new Color(1f, 0, 0);
    private Color lightGrey = new Color(0.9f, 0.9f, 0.9f);

    [SerializeField]
    private GameObject leftPreviewGroup;

    [SerializeField]
    private GameObject rightPreviewGroup;

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
        ChangeActionPreview(currentStat, outcomeStat, leftBottomImage, leftTopImage, leftPreviewGroup);
    }

    public void ChangeRightActionPreview(float currentStat, float outcomeStat)
    {
        ChangeActionPreview(currentStat, outcomeStat, rightBotomImage, rightTopImage, rightPreviewGroup);
    }

    private void ChangeActionPreview(float currentStat, float outcomeStat, Image bottomImage, Image topImage, GameObject preview)
    {
        preview.SetActive(true);
        if (currentStat > outcomeStat)
        {
            bottomImage.fillAmount = outcomeStat;
            topImage.fillAmount = currentStat;
            //bottomImage.color = lightRed;
            bottomImage.color = lightGrey;
            topImage.color = darkRed;
        }
        else if (currentStat < outcomeStat)
        {
            bottomImage.fillAmount = currentStat;
            topImage.fillAmount = outcomeStat;
            //bottomImage.color = darkGreen;
            bottomImage.color = lightGrey;
            topImage.color = darkGreen;
        }
        else
        {
            preview.SetActive(false);
        }
    }

    public void UpdateActionPreviewAlpha(float leftAlpha, float rightAlpha)
    {
        leftPreview.alpha = leftAlpha;
        rightPreview.alpha = rightAlpha;
    }
}
