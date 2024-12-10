using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _debugText;

    public void IndexFingerStart()
    {
        _debugText.text = "INDEX FINGER START";
    }
    
    public void IndexFingerEnd()
    {
        _debugText.text = "INDEX FINGER END";
    }
}
