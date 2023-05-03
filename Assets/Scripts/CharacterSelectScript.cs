using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class CharacterSelectScript : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Image image;
    public GameObject selectButton;
    public GameObject selectedButton;
    public GameObject buttons;
    public GameObject lockImage;
    int num = 0;

    CustomizeMenuManager customizeMenuManager;

    private void Start()
    {
        customizeMenuManager = FindObjectOfType<CustomizeMenuManager>();
    }

    public TextMeshProUGUI GetName()
    {
        return nameText;
    }
    public Image GetImage()
    {
        return image;
    }
    public GameObject GetButtonSelect()
    {
        return selectButton;
    }
    public GameObject GetButtonSelected()
    {
        return selectButton;
    }
    public GameObject GetLock()
    {
        return lockImage;
    }

    public void SetNum(int _num)
    {
        num = _num;
    }

    public void SelectedPressed()
    {
        customizeMenuManager.CharacterSelected(num);
        Selected(customizeMenuManager.GetCustomizationSelections(num).isSelected);
    }

    public void HideButtons(bool _hide)
    {
        buttons.SetActive(_hide);
    }

    public void Selected(bool _selected)
    {
        selectButton.SetActive(!_selected);
        selectedButton.SetActive(_selected);
    }
}
