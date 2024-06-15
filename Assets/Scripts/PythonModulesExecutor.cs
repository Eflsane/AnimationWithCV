using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class PythonModulesExecutor : MonoBehaviour
{
    
    public Process StartDetectionPythonModule(string absoluteExeEditorPathWin, string absoluteExeReleasePathWin, Process moduleProcess)
    {
        string pathToExe = Directory.GetCurrentDirectory();
        
        if (Application.isEditor)
        {
            pathToExe += absoluteExeEditorPathWin;
        }
        else
        {
            pathToExe += absoluteExeReleasePathWin;
        }

        if (moduleProcess != null)
            return moduleProcess;
        
        moduleProcess = new Process();

        moduleProcess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
        moduleProcess.StartInfo.FileName = $"{pathToExe}";
        moduleProcess.StartInfo.Arguments = "";

        moduleProcess.Start();

        return moduleProcess;
    }
    
    public Process StopEmotionDetectionPythonModule(Process moduleProcess)
    {
        if (moduleProcess == null || moduleProcess.HasExited)
            return null;
        
        print(moduleProcess.ProcessName + " " + moduleProcess.Id);
        //Process.GetProcessesByName(_cameraProcess.ProcessName)[1].Kill();
        moduleProcess.Kill();

        return null;
    }

    public Process StartEmoDetectionPythonModule(string absoluteExeEditorPathWin, string absoluteExeReleasePathWin, Process moduleProcess)
    {
        string pathToExe = Directory.GetCurrentDirectory();
        
        if (Application.isEditor)
        {
            pathToExe += absoluteExeEditorPathWin;
        }
        else
        {
            pathToExe += absoluteExeReleasePathWin;
        }

        if (moduleProcess != null)
            return moduleProcess;
        
        moduleProcess = new Process();

        moduleProcess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
        moduleProcess.StartInfo.FileName = $"{pathToExe}";
        moduleProcess.StartInfo.Arguments = "";

        moduleProcess.Start();

        return moduleProcess;
    }

    public void StartNonPythonModule(string pathToModule)
    {
        Process moduleProcess = new Process();

        string directoryPath = Path.GetDirectoryName(pathToModule);

        
        moduleProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
        moduleProcess.StartInfo.WorkingDirectory = $"{directoryPath}";
        moduleProcess.StartInfo.FileName = $"{pathToModule}";
        moduleProcess.StartInfo.Arguments = "";

        moduleProcess.Start();
    }
}
