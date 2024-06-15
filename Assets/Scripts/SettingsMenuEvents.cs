using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SimpleFileBrowser;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsMenuEvents : MonoBehaviour
{
    private UIDocument _uiDocument;

    private Toggle _calibrationToggle;
    
    private List<string> _cameras = new List<string>();
    private WebCamDevice[] _camDevices = Array.Empty<WebCamDevice>();
    private DropdownField _camerasListView;
    
    private List<Vector2> _resolutions = new List<Vector2>();
    private List<Vector2> _typicalResolutions = new List<Vector2>();
    private DropdownField _resolutionsListView;

    private TextField _obsTextField;
    private Button _obsBrowseButton;
    private Button _obsStartButton;
    
    private PythonModulesExecutor _pythonModulesExecutor;

    private void OnEnable()
    {
        _uiDocument = GetComponent<UIDocument>();

        _calibrationToggle = _uiDocument.rootVisualElement.Q<Toggle>("CalibrationToggle");
        _calibrationToggle.RegisterCallback<ChangeEvent<bool>>(CalibrationToggleValueChanged);
        
        _camerasListView = _uiDocument.rootVisualElement.Q<DropdownField>("CameraDropdown");
        _camerasListView.RegisterCallback<ChangeEvent<int>>(CamerasListViewValueChanged);
        StartCoroutine(FindCameras());
        
        _resolutionsListView = _uiDocument.rootVisualElement.Q<DropdownField>("ResolutionDropdown");
        _typicalResolutions.Add(new Vector2(512, 512));
        _typicalResolutions.Add(new Vector2(1280, 768));

        _obsTextField = _uiDocument.rootVisualElement.Q<TextField>("ObsTextField");
        _obsBrowseButton = _uiDocument.rootVisualElement.Q<Button>("ObsBrowseButton");
        _obsBrowseButton.RegisterCallback<ClickEvent>(ObsBrowse);
        
        _obsStartButton = _uiDocument.rootVisualElement.Q<Button>("ObsStartButton");
        _obsStartButton.RegisterCallback<ClickEvent>(ObsStart);
        _pythonModulesExecutor = GetComponent<PythonModulesExecutor>();
    }

    private void OnDisable()
    {
        _calibrationToggle.UnregisterCallback<ChangeEvent<bool>>(CalibrationToggleValueChanged);
        _obsBrowseButton.UnregisterCallback<ClickEvent>(ObsBrowse);
        _obsStartButton.UnregisterCallback<ClickEvent>(ObsStart);
    }
    
    private IEnumerator FindCameras()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            _camDevices = new WebCamDevice[WebCamTexture.devices.Length];
            Array.Copy(WebCamTexture.devices, _camDevices, WebCamTexture.devices.Length);
            foreach (var device in _camDevices)
            {
                _cameras.Add(device.name);
            }
            _camerasListView.choices = _cameras;
            _camerasListView.index = 0;
            print("camrera name:" + _camDevices[0].name);
            
            UpdateCameraRes(_camerasListView.index);
        }
    }
    
    private void CamerasListViewValueChanged(ChangeEvent<int> evt)
    {
        UpdateCameraRes(evt.newValue);
    }

    private void UpdateCameraRes(int cameraIndex)
    {
        if (_camDevices[cameraIndex].availableResolutions == null)
            _resolutions = _typicalResolutions;
        else
        {
            var maxCameraRes = _camDevices[cameraIndex].availableResolutions.Max();
            _resolutions =
                _typicalResolutions.Where(res => res.x <= maxCameraRes.width && res.y <= maxCameraRes.height).ToList();    
        }

        List<string> stringRes = new List<string>();
        foreach (var res in _resolutions)
        {
            stringRes.Add($"{res.x}x{res.y}");
        }
        _resolutionsListView.choices = stringRes;
        _resolutionsListView.index = 0;
    }

    private void CalibrationToggleValueChanged(ChangeEvent<bool> evt)
    {
        GetComponent<FaceDataCalibrationController>().
            OnCaliibrationValueChanged(evt.newValue);
    }
    
    private void ObsBrowse(ClickEvent evt)
    {
        FileBrowser.ShowLoadDialog(ObsLoadSuccess, ObsLoadCancel, FileBrowser.PickMode.Files);
    }

    private void ObsLoadSuccess(string[] filePaths)
    {
        string obsPath = filePaths[0];
        
        if (File.Exists(obsPath))
        {
            _obsTextField.value = obsPath;
            print("Loaded obs in: " + obsPath);
            
        }
        else
        {
            print(obsPath + " does not exist");
        }
    }
    
    private void ObsLoadCancel()
    {
        return;
    }
    
    private void ObsStart(ClickEvent evt)
    {
        _pythonModulesExecutor.StartNonPythonModule(GetObsPath());
    }

    public string GetObsPath()
    {
        return _obsTextField.value;
    }

    public int GetCurrentWebcamIndex()
    {
        return _camerasListView.index;
    }

    public Vector2 GetCurrentCameraRes()
    {
        return _resolutions[_resolutionsListView.index];
    }
}
