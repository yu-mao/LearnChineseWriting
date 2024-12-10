using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private OVRHand _hand;
    [SerializeField] private OVRSkeleton _skeleton;
    [SerializeField] private GameObject _writingVisualFeedback;
    [SerializeField] private TextMeshProUGUI _debugText;

    private bool _isWriting = false;

    public void IndexFingerStart()
    {
        _debugText.text = "INDEX FINGER WRITING";
        _isWriting = true;
        _writingVisualFeedback.SetActive(true);
    }
    
    public void IndexFingerEnd()
    {
        _debugText.text = "...";
        _isWriting = false;
        _writingVisualFeedback.SetActive(false);
    }

    private void Start()
    {
        _writingVisualFeedback.SetActive(false);
    }
    
    private void Update()
    {
        if (_isWriting)
        {
            if (_hand.IsTracked && _skeleton.IsInitialized)
            {
                var indexFingerTip = _skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_IndexTip];
                Vector3 indexFingerTipPos = indexFingerTip.Transform.position;
                _writingVisualFeedback.transform.position = indexFingerTipPos;
            }
        }
    }
}
