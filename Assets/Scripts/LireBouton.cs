using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LireBouton : MonoBehaviour
{
    private Button _button;
    private HighScoreTable _highScoreTable;

    private void Start()
    {
        _highScoreTable = FindObjectOfType<HighScoreTable>();
        _button = this.GetComponent<Button>();
        _button.onClick.AddListener(LireTexte);
    }

    public void LireTexte()
    {
        if (_highScoreTable != null)
        {
            _highScoreTable.AjouterLettre(this.GetComponentInChildren<TMP_Text>().text);
        }
    }

}
