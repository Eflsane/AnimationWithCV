using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceDataCalibrationController : MonoBehaviour
{
    [SerializeField] private FacialExpressionController _faceController;
        
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCaliibrationValueChanged(bool value)
    {
        _faceController.SetCalibrationValue(value);
    }
}
