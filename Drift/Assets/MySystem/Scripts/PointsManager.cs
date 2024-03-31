using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PointsManager : MonoBehaviour
{
    [field: SerializeField] public float Points { get; private set; }
    [SerializeField] private float _pointsToWin;
    [SerializeField] private float _onGroundCheckDistance = 0.2f;
    [SerializeField] private float _onGroundCheckOffset = 0.1f;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _motivationMessageMinimumAngle = 30f;
    [SerializeField] private float _motivationMessageLastingInSeconds = 2f;
    [SerializeField] private float _minAngle = 10f;
    [SerializeField] private float _maxAngle = 175f;
    [SerializeField] private float _delayBeforeGainPoints = 3f;
    private float _angle = 0f;
    private bool _isPointsCalculating = false;
    private float _carSpeed;
    private int _motivationAngleHolder = 0;
    private float _motivationStartingPoints = 0;
    private TextMeshProUGUI _pointsText;
    private TextMeshProUGUI _motivatingText;
    private void Start()
    {
        if(SceneManager.GetActiveScene().name == "Garage")
        {
            Destroy(this);
            return;
        }
        Points = 0;
        _pointsText = GameObject.Find("PointsText").GetComponent<TextMeshProUGUI>();
        _motivatingText = GameObject.Find("MotivatingText").GetComponent<TextMeshProUGUI>();
        StartCoroutine(PointsDelayForSpawn());
    }

    private void Update()
    {
        if (!_isPointsCalculating) return;
        if (_angle < _minAngle || _angle > _maxAngle)
        {
            _motivationStartingPoints = 0;
            return;
        }
        var isOnGround = Physics.Raycast(transform.position + new Vector3(0f, _onGroundCheckOffset, 0f), Vector3.down, _onGroundCheckDistance, _layerMask);
        if (!isOnGround) return;
        Points += _angle * _carSpeed * Time.deltaTime;
        _pointsText.text = $"Points: {(int)Points}";
        MotivationCheck();
    }
    public void CalculatePoints(Vector3 movingVector, Vector3 directionVector)
    {
        _angle = Vector3.Angle(movingVector, directionVector);
        _carSpeed = movingVector.magnitude;
        //Debug.Log(movingVector);
    }

    public int FinishDrift()
    {
        return (int)Points;
    }
    private void MotivationCheck()
    {
        //Debug.Log($"{_angle}  {_carSpeed}");
        if (_angle < _motivationMessageMinimumAngle||_carSpeed<1f)
        {
            return;
        }
        if (_motivationStartingPoints == 0) _motivationStartingPoints = Points;
        else
        {
            int intedAngle = (int)_angle / 10;
            if (intedAngle != _motivationAngleHolder)
            {
                _motivationAngleHolder = intedAngle;
            }
            var pointsToDisplay = (int) (Points - _motivationStartingPoints);
            _motivatingText.text = $"DRIFT x {intedAngle}  points {pointsToDisplay}";
        }
        StartCoroutine(DestroyMotivationMessage());
    }

    private IEnumerator DestroyMotivationMessage()
    {
        yield return new WaitForSeconds(_motivationMessageLastingInSeconds);
        _motivatingText.text = $"";
        StopAllCoroutines();
    }

    
    private IEnumerator PointsDelayForSpawn()
    {
        yield return new WaitForSeconds(_delayBeforeGainPoints);
        _isPointsCalculating = true;
    }
}
