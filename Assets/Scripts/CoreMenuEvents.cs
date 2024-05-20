using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;


public class CoreMenuEvents : MonoBehaviour
{
    private UIDocument _uiDocument;
    
    private Button _startButton;
    private Button _backgroundButton;
    private Button _settingsButton;
    private Button _exitButton;
    private Button _closeButton;

    Process _cameraProcess = null;

    [SerializeField] private FacialExpressionController facialExpressionController;
    
    // Start is called before the first frame update
    void Awake()
    {
        
    }

    private void OnEnable()
    {
        _uiDocument = GetComponent<UIDocument>();
        
        _startButton = _uiDocument.rootVisualElement.Q<Button>("StartCameraButton");
        _backgroundButton = _uiDocument.rootVisualElement.Q<Button>("BackgroundButton");
        _settingsButton = _uiDocument.rootVisualElement.Q<Button>("SettingsButton");
        _exitButton = _uiDocument.rootVisualElement.Q<Button>("ExitButton");
        _closeButton = _uiDocument.rootVisualElement.Q<Button>("CloseButton");
        
        
        
        
        _uiDocument.rootVisualElement.Q<VisualElement>("FreeSpace").RegisterCallback<ClickEvent>(RootDoubleClick);
        
        _startButton.RegisterCallback<ClickEvent>(StartButtonClick);
        _backgroundButton.RegisterCallback<ClickEvent>(BackgroundButtonClick);
        _settingsButton.RegisterCallback<ClickEvent>(SettingsButtonClick);
        _exitButton.RegisterCallback<ClickEvent>(ExitButtonClick);
        _closeButton.RegisterCallback<ClickEvent>(CloseButtonClick);
    }

    private void OnDisable()
    {
        _uiDocument.rootVisualElement.Q<VisualElement>("FreeSpace").UnregisterCallback<ClickEvent>(RootDoubleClick);
        
        _startButton.UnregisterCallback<ClickEvent>(StartButtonClick);
        _backgroundButton.UnregisterCallback<ClickEvent>(BackgroundButtonClick);
        _settingsButton.UnregisterCallback<ClickEvent>(SettingsButtonClick);
        _exitButton.UnregisterCallback<ClickEvent>(ExitButtonClick);
        _closeButton.UnregisterCallback<ClickEvent>(CloseButtonClick);
    }
    
    
    private void RootDoubleClick(ClickEvent evt)
    {
        var menus = _uiDocument.rootVisualElement.Query<VisualElement>(className: "AllMenus").ToList();

        foreach (var menu in menus)
        {
            if (menu.style.display == DisplayStyle.Flex)
                menu.style.display = DisplayStyle.None;
            else if (menu.name == "CoreMenuContainer") 
                menu.style.display = DisplayStyle.Flex;
        }
        
        
    }

    private void StartButtonClick(ClickEvent evt)
    {
        var cameraStartedElem = _uiDocument.rootVisualElement.Q<VisualElement>("CameraStartedElem");

        string pathToCameraExe = System.IO.Directory.GetCurrentDirectory();
        print(pathToCameraExe);

        if (Application.isEditor)
        {
            pathToCameraExe += "\\Assets\\PythonProgram\\face_landmarks_detection.exe";
            print(pathToCameraExe);
        }
        else
        {
            pathToCameraExe += "\\AnimationWithCV_Data\\StreamingAssets\\face_landmarks_detection.exe";
            print(pathToCameraExe);
        }
        
        print("Elem.Display=" + cameraStartedElem.style.display.ToString());
        
        if (cameraStartedElem.style.display == DisplayStyle.None)
        {
            cameraStartedElem.style.display = DisplayStyle.Flex;
            
            facialExpressionController.StartListeningTcp();

            _cameraProcess = new Process();

            _cameraProcess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            _cameraProcess.StartInfo.FileName = $"{pathToCameraExe}";
            _cameraProcess.StartInfo.Arguments = "";

            _cameraProcess.Start();
        }
        else
        {
            cameraStartedElem.style.display = DisplayStyle.None;

            if (_cameraProcess != null)
            {
                print(_cameraProcess.ProcessName + " " + _cameraProcess.Id);
                //Process.GetProcessesByName(_cameraProcess.ProcessName)[1].Kill();
                facialExpressionController.StopListeningTcp();
                _cameraProcess?.Close();
            }
            
        }
            
    }
    
    private void BackgroundButtonClick(ClickEvent evt)
    {
        throw new NotImplementedException();
    }

    private void SettingsButtonClick(ClickEvent evt)
    {
        var settingPanel = _uiDocument.rootVisualElement.Q<VisualElement>("SettingsMenuContainer");
        if(settingPanel.style.display == DisplayStyle.None)
            settingPanel.style.display = DisplayStyle.Flex;
        else
            settingPanel.style.display = DisplayStyle.None;
    }

    private void ExitButtonClick(ClickEvent evt)
    {
        Application.Quit(0);
    }
    
    private void CloseButtonClick(ClickEvent evt)
    {
        var menus = _uiDocument.rootVisualElement.Query<VisualElement>(className: "AllMenus").ToList();

        foreach (var menu in menus)
        {
            menu.style.display = DisplayStyle.None;
        }
    }
}
