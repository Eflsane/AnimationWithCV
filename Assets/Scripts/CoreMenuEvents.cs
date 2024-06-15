using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using SimpleFileBrowser;
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
    private VisualElement _menusHideTooltip;

    private bool _isUIEnabled;
    private bool _isCameraStarted;
    private bool _isSettingsActive;
    

    private Process _cameraProcess = null;
    

    [SerializeField] private FacialExpressionController facialExpressionController;
    [SerializeField] private EmotionDetectionController emotionDetectionController;

    [SerializeField] private SpriteRenderer backgroundSprite;
    
    // Start is called before the first frame update
    void Awake()
    {
        _isUIEnabled = true;
        _isCameraStarted = false;
        _isSettingsActive = false;
        
        facialExpressionController.FaceDetectionActive += FaceDetectionActive;
        emotionDetectionController.EmotionDetectionActive += EmotionDetectionActive;
    }

    private void OnEnable()
    {
        _uiDocument = GetComponent<UIDocument>();
        
        _startButton = _uiDocument.rootVisualElement.Q<Button>("StartCameraButton");
        _backgroundButton = _uiDocument.rootVisualElement.Q<Button>("BackgroundButton");
        _settingsButton = _uiDocument.rootVisualElement.Q<Button>("SettingsButton");
        _exitButton = _uiDocument.rootVisualElement.Q<Button>("ExitButton");
        _closeButton = _uiDocument.rootVisualElement.Q<Button>("CloseButton");
        _menusHideTooltip = _uiDocument.rootVisualElement.Q<VisualElement>("MenusHideTooltip");


        _uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        
        
        _uiDocument.rootVisualElement.Q<VisualElement>("FreeSpace").RegisterCallback<ClickEvent>(RootDoubleClick);
        
        _startButton.RegisterCallback<ClickEvent>(StartButtonClick);
        _backgroundButton.RegisterCallback<ClickEvent>(BackgroundButtonClick);
        _settingsButton.RegisterCallback<ClickEvent>(SettingsButtonClick);
        _exitButton.RegisterCallback<ClickEvent>(ExitButtonClick);
        _closeButton.RegisterCallback<ClickEvent>(CloseButtonClick);
        
        var emotionStartElem = _uiDocument.rootVisualElement.Q<VisualElement>("EmotionDetectionIconsContainer");
        emotionStartElem.style.backgroundColor = new StyleColor(Color.yellow);
        emotionStartElem.style.display = DisplayStyle.Flex;
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

        
        if (_isUIEnabled) //menu.style.display == DisplayStyle.Flex
        {
            _isUIEnabled = false;
            _isSettingsActive = false;
            foreach (var menu in menus)
            {
                menu.style.display = DisplayStyle.None;
            }

            StartCoroutine(ShowMenusHideToolTip());
        }
        else 
        {
            _isUIEnabled = true;
            foreach (var menu in menus)
            {
                if (menu.name == "CoreMenuContainer")
                    menu.style.display = DisplayStyle.Flex;
            }
            
        }
        
    }

    private void StartButtonClick(ClickEvent evt)
    {
        var cameraStartedElem = _uiDocument.rootVisualElement.Q<VisualElement>("CameraStartedElem");
        var emotionStartElem = _uiDocument.rootVisualElement.Q<VisualElement>("EmotionDetectionIconsContainer");
        
        print("Elem.Display=" + cameraStartedElem.style.ToString());
        
        if (!_isCameraStarted) //cameraStartedElem.style.display == DisplayStyle.None || cameraStartedElem.style == null
        {
            _isCameraStarted = true;
            emotionStartElem.style.display = DisplayStyle.None;
            cameraStartedElem.style.backgroundColor = new StyleColor(Color.yellow);
            cameraStartedElem.style.display = DisplayStyle.Flex;
            
            emotionDetectionController.StopListeningEmotions();
            facialExpressionController.StartListeningFaceParams();
        }
        else
        {
            _isCameraStarted = false;
            cameraStartedElem.style.display = DisplayStyle.None;
            emotionStartElem.style.backgroundColor = new StyleColor(Color.yellow);
            emotionStartElem.style.display = DisplayStyle.Flex;

            facialExpressionController.StopListeningFaceParams();
            emotionDetectionController.StartListeningEmotions();
        }
            
    }
    
    private void FaceDetectionActive()
    {
        var cameraStartedElem = _uiDocument.rootVisualElement.Q<VisualElement>("CameraStartedElem");
        cameraStartedElem.style.backgroundColor = new StyleColor(Color.red);
    }

    private void EmotionDetectionActive()
    {
        var emotionStartElem = _uiDocument.rootVisualElement.Q<VisualElement>("EmotionDetectionIconsContainer");
        emotionStartElem.style.backgroundColor = new StyleColor(Color.green);
    }
    
    private void BackgroundButtonClick(ClickEvent evt)
    {
        FileBrowser.ShowLoadDialog(BackgroundLoadSuccess, BackgroundLoadCancel, FileBrowser.PickMode.Files);
    }

    private void BackgroundLoadSuccess(string[] filePaths)
    {
        string backgroundPath = filePaths[0];
        
        if (File.Exists(backgroundPath))
        {
            byte[] fileData = File.ReadAllBytes(backgroundPath);
            Texture2D loadedTexture = new Texture2D(256, 256);
            loadedTexture.LoadImage(fileData); // Auto-resizes the texture
            print("Loaded texture: " + loadedTexture);
            //Graphics.CopyTexture(loadedTexture, backgroundSprite.sprite.texture);
            /*Texture2D resizedTexture = Resize(loadedTexture,
                (int)backgroundSprite.size.x,
                (int)backgroundSprite.size.y);*/
            
            Sprite sp = Sprite.Create(loadedTexture, new Rect(0, 0, loadedTexture.width, loadedTexture.height),
                new Vector2(0.5f, 0.5f));
            backgroundSprite.sprite = sp;
            
            float cameraHeight = Camera.main.orthographicSize * 2f;
            float cameraWidth = cameraHeight * Camera.main.aspect;
            backgroundSprite.size = new Vector2(cameraWidth, cameraHeight);
            backgroundSprite.transform.localScale = Vector3.one;
        }
        else
        {
            print(backgroundPath + " does not exist");
        }
    }
    
    private void BackgroundLoadCancel()
    {
        return;
    }

    private void SettingsButtonClick(ClickEvent evt)
    {
        var settingPanel = _uiDocument.rootVisualElement.Q<VisualElement>("SettingsMenuContainer");
        if(!_isSettingsActive) //settingPanel.style.display == DisplayStyle.None
        {
            _isSettingsActive = true;
            settingPanel.style.display = DisplayStyle.Flex;
        }
        else
        {
            _isSettingsActive = false;
            settingPanel.style.display = DisplayStyle.None;
        }
    }

    private void ExitButtonClick(ClickEvent evt)
    {
        Application.Quit(0);
    }
    
    private void CloseButtonClick(ClickEvent evt)
    {
        var menus = _uiDocument.rootVisualElement.Query<VisualElement>(className: "AllMenus").ToList();

        _isUIEnabled = false;
        _isSettingsActive = false;
        foreach (var menu in menus)
        {
            menu.style.display = DisplayStyle.None;
        }
    }

    private IEnumerator ShowMenusHideToolTip()
    {
        //show tooltip
        _menusHideTooltip.style.display = DisplayStyle.Flex;
        yield return new WaitForSeconds(7);
        //hide tooltip
        _menusHideTooltip.style.display = DisplayStyle.None;
    }
    
    private Texture2D Resize(Texture2D texture, int targetWidth, int targetHeight)
    {
        RenderTexture rt = new RenderTexture(targetWidth, targetHeight, 24);
        RenderTexture.active = rt;
        Graphics.Blit(texture, rt);
        Texture2D result = new Texture2D(targetWidth, targetHeight);
        result.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
        result.Apply();
        return result;
    }

    public bool GetUIDocumentEnabled()
    {
        return _isUIEnabled;
    }

    public void SetUIDocumentEnabled(bool isUIEnabled)
    {
        _isUIEnabled = isUIEnabled;
    }
}
