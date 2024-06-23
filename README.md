For unity projecct to work correctly you need to place these 2 files:
libburst-llvm-16.dylib
link: https://mega.nz/file/xuIXHRbQ#mD6gweeGVDDC4QG30R1fHHPHcOu0olkvWAdN7nkuyq4
path: \AnimationWithCV\Library\PackageCache\com.unity.burst@1.8.13\\.Runtime\libburst-llvm-16.dylib

PythonProgram folder
link: https://mega.nz/file/8y5QRAQa#maUeud3mbka3Tg_22Mj3IyfzmwoLL8q2gFkxjn0xAZs
path: AnimationWithCV\Assets\PythonProgram\


Last build:
link: https://mega.nz/file/MuYAQS4Q#ZbBFBLm5dZ0P7EnkCZifrTiXyHGYG2XzRIqeg3GHIG8
In folder with name of the build AnimationWithCV_Data\StreamingAssets you need also to unpack two archieves with Python modules:
emotion_detection: https://mega.nz/file/0zQW2a6D#vzymz5RL3_Tuqyq6Z5Zouoijtrz1DPkFE_DeszlyiXo
face_laandmarks_detection: https://mega.nz/file/1nJVSS7T#Lnae0KwVufclesjGo-P2j7tiAUNodcn09V9aCwqpDm4

You should unpack without creating new folders. It should be like this:
face_laandmarks_detection.7z -> AnimationWithCV_Data\StreamingAssets\face_laandmarks_detection.exe
emotion_detection.7z -> AnimationWithCV_Data\StreamingAssets\emotion_detection.bat + emotion_detection folder

To launch emotion detection also need to add .venv folder and download requirenments from file "requirenments.txt" with pip Python
