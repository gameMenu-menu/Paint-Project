using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ColorButtonController : MonoBehaviour
{
    int gridIndex;
    public TMP_Text countText;
    public Image image;

    Color color;

    public void Init(int index, Color _color)
    {
        gridIndex = index;

        countText.text = gridIndex.ToString();

        color = _color;

        image.color = color;
    }

    public void ChooseColor()
    {
        ColorManager.Instance.OnChooseColorButton(gridIndex);
    }
}
