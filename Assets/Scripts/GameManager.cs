using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private OVRHand _hand;
    [SerializeField] private OVRSkeleton _skeleton;
    [SerializeField] private GameObject _tempWritingVisualFeedback;
    [SerializeField] private GameObject _tempIndexFingerTrackingFeedback;
    [SerializeField] private int _writingWindowWidth = 4;
    [SerializeField] private TextMeshProUGUI _debugText;

    private bool _isWriting = false;
    private List<Vector3> _writingInCurrentWindow = new List<Vector3>();
    private List<Vector3> _writingSampledPoints = new List<Vector3>();
    private List<Vector3> _writingVisualFeedback = new List<Vector3>();

    public void IndexFingerStart()
    {
        // _debugText.text = "INDEX FINGER WRITING";
        _isWriting = true;
        _tempWritingVisualFeedback.SetActive(true);
    }
    
    public void IndexFingerEnd()
    {
        // _debugText.text = "...";
        _isWriting = false;
        _tempWritingVisualFeedback.SetActive(false);
        _writingVisualFeedback.Clear();
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
                AverageWritingPoints(indexFingerTipPos);
                
                // temporary visual feedback for debug
                _tempIndexFingerTrackingFeedback.transform.position = indexFingerTipPos;
            }
        }
    }

    private void AverageWritingPoints(Vector3 indexFingerTipPos)
    {
        if (_writingInCurrentWindow.Count <= _writingWindowWidth)
        {
            _writingInCurrentWindow.Add(indexFingerTipPos);
        }
        else
        {
            Vector3 sum = Vector3.zero;
            foreach (var writingPoint in _writingInCurrentWindow)
            {
                sum += writingPoint;
            }
            var averageWritingPoint = sum / _writingInCurrentWindow.Count;
            
            //TODO: delete debug
            _tempWritingVisualFeedback.transform.position = averageWritingPoint;
            _debugText.text = "Got an average point!";
            
            _writingSampledPoints.Add(averageWritingPoint);
            _writingInCurrentWindow.Clear();
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
