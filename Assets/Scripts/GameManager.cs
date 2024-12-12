using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private OVRHand _hand;
    [SerializeField] private OVRSkeleton _skeleton;
    [SerializeField] private List<LineRenderer> _writingVisualFeedbackGroup;
    [SerializeField] private int _segmentsPerCurve = 2; // interpolation factor, the bigger, the smoother & the slower
    [SerializeField] private TextMeshProUGUI _debugText;

    private bool _isWriting = false;
    private int _countWritingVisualFeedbackDisplayed = 0;
    private List<Vector3> _writingPointsLogged = new List<Vector3>();
    private List<Vector3> _writingVisualFeedback = new List<Vector3>();

    public void IndexFingerStart()
    {
        _isWriting = true;
        _debugText.text = "you are writing";
    }
    
    public void IndexFingerEnd()
    {
        if (_countWritingVisualFeedbackDisplayed >= _writingVisualFeedbackGroup.Count)
        {
            _countWritingVisualFeedbackDisplayed = 0;
        }
        else
        {
            _countWritingVisualFeedbackDisplayed += 1;
        }
        _writingPointsLogged.Clear();
        _isWriting = false;
        _debugText.text = "";
    }

    public void FinishWriting()
    {
        ClearWriting();
        _debugText.text = "finish writing";
    }


    public void RedoWriting()
    {
        ClearWriting();
        _debugText.text = "redo writing";
    }

    private void Start()
    {
        ClearWriting();
    }

    private void Update()
    {
        if (_isWriting)
        {
            if (_hand.IsTracked && _skeleton.IsInitialized)
            {
                var indexFingerTipPos = GetIndexFingerTipPosition();
                _writingPointsLogged.Add(indexFingerTipPos);
                DrawSmoothLine(_writingPointsLogged.ToArray());
            }
        }
    }
    
    private void ClearWriting()
    {
        _writingPointsLogged.Clear();
        foreach (var lineRenderer in _writingVisualFeedbackGroup)
        {
            lineRenderer.positionCount = 0;
        }
        _writingVisualFeedback.Clear();
    }

    
    private Vector3 GetIndexFingerTipPosition()
    {
        var indexFingerMid = _skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Index3];
        var indexFingerBottom = _skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Index2];
        var indexFingerTipPos = indexFingerMid.Transform.position * 2 - indexFingerBottom.Transform.position;
        return indexFingerTipPos;
    }

    private void DrawSmoothLine(Vector3[] points)
    {
        if (_writingPointsLogged.Count < 2) return;
        
        _writingVisualFeedback.Clear();
        
        for (int i = 0; i < points.Length - 1; i++)
        {
            if (i == 0 || i == points.Length - 2 || i == points.Length - 1) continue;
            
            Vector3 p0 = points[i - 1];
            Vector3 p1 = points[i];
            Vector3 p2 = points[i + 1];
            Vector3 p3 = points[i + 2];

            for (int j = 0; j < _segmentsPerCurve; j++)
            {
                float t = j / (float)_segmentsPerCurve;
                _writingVisualFeedback.Add(CatmullRom(p0, p1, p2, p3, t));
            }
        }

        // Add the final point to close the curve
        _writingVisualFeedback.Add(points[points.Length - 1]);

        // Apply the smoothed points to the LineRenderer
        _writingVisualFeedbackGroup[_countWritingVisualFeedbackDisplayed].positionCount = _writingVisualFeedback.Count;
        _writingVisualFeedbackGroup[_countWritingVisualFeedbackDisplayed].SetPositions(_writingVisualFeedback.ToArray());
    }
    
    private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        //Returns a position between 4 Vector3 with Catmull-Rom spline algorithm
        //http://www.iquilezles.org/www/articles/minispline/minispline.htm
        
        //The coefficients of the cubic polynomial (except the 0.5f * which I added later for performance)
        Vector3 a = 2f * p1;
        Vector3 b = p2 - p0;
        Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
        Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

        //The cubic polynomial: a + b * t + c * t^2 + d * t^3
        Vector3 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

        return pos;
    }
}
