using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private OVRHand _hand;
    [SerializeField] private OVRSkeleton _skeleton;
    [SerializeField] private GameObject _tempWritingVisualFeedback;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private TextMeshProUGUI _debugText;

    private bool _isWriting = false;
    private List<Vector3> _writingVisualFeedback = new List<Vector3>();

    public void IndexFingerStart()
    {
        _isWriting = true;
        _tempWritingVisualFeedback.SetActive(true);
    }
    
    public void IndexFingerEnd()
    {
        _isWriting = false;
        _tempWritingVisualFeedback.SetActive(false);
        _writingVisualFeedback.Clear();
        _lineRenderer.positionCount = 0;
    }

    private void Start()
    {
        _tempWritingVisualFeedback.SetActive(false);
    }
    
    private void Update()
    {
        if (_isWriting)
        {
            if (_hand.IsTracked && _skeleton.IsInitialized)
            {
                var indexFingerTipPos = GetIndexFingerTipPosition();
                _writingVisualFeedback.Add(indexFingerTipPos);
                _lineRenderer.positionCount = _writingVisualFeedback.Count;
                _lineRenderer.SetPositions(_writingVisualFeedback.ToArray());
            }
        }
    }
    
    private Vector3 GetIndexFingerTipPosition()
    {
        // TODO: improve finger tip position tracking accuracy
        var indexFingerTip = _skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Index3];
        var indexFingerTipPos = indexFingerTip.Transform.position;
        return indexFingerTipPos;
    }
}
