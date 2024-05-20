using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsMenuEvents : MonoBehaviour
{
    private UIDocument _uiDocument;

    private Toggle _calibrationToggle;
    
    private List<string> _cameras = new List<string>();
    private DropdownField _camerasListView;
    
    private List<string> _resolutions = new List<string>();
    private DropdownField _resolutionsListView;

    private void OnEnable()
    {
        _uiDocument = GetComponent<UIDocument>();

        _calibrationToggle = _uiDocument.rootVisualElement.Q<Toggle>("CalibrationToggle");
        _calibrationToggle.RegisterCallback<ChangeEvent<bool>>(CalibrationToggleValueChanged);
        
        _camerasListView = _uiDocument.rootVisualElement.Q<DropdownField>("CameraDropdown");
        _cameras.Add("VCamera");
        _camerasListView.choices = _cameras;
        _camerasListView.index = 0;
        
        _resolutionsListView = _uiDocument.rootVisualElement.Q<DropdownField>("ResolutionDropdown");
        _resolutions.Add("800x600");
        _resolutionsListView.choices = _resolutions;
        _resolutionsListView.index = 0;
    }

    private void OnDisable()
    {
        _calibrationToggle.UnregisterCallback<ChangeEvent<bool>>(CalibrationToggleValueChanged);
    }

    private void CalibrationToggleValueChanged(ChangeEvent<bool> evt)
    {
        GetComponent<FaceDataCalibrationController>().
            OnCaliibrationValueChanged(evt.newValue);
    }
}
