using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// https://www.youtube.com/watch?v=ltu27NLeIWc

public class SimpleRotationLoading : MonoBehaviour {
    public RectTransform _mainIcon;
    public float _timeStep;    
    public float _oneStepAngle;

    float _startTime;

    private void Start() {
        _startTime = Time.time;    
    }

    private void Update() {
        if (Time.time - _startTime >= _timeStep)
        {
            Vector3 iconAngle = _mainIcon.localEulerAngles;
            iconAngle.z+= _oneStepAngle;

            _mainIcon.localEulerAngles = iconAngle;

            _startTime= Time.time;
        }
    }
}
