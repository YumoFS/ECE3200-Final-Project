// DeleteGameBatCreator.cs
using UnityEngine;
using System.IO;
using System;

public class DeleteGameBatCreator : MonoBehaviour
{
    [Header("BAT文件设置")]
    [SerializeField] private string batFileName = "删除游戏并感谢.bat";
    [SerializeField] private string thankYouFileName = "感谢信.txt";
    [SerializeField] private bool hideBatFile = true; // 是否隐藏BAT文件
    
    // 当达成特定结局时调用这个方法
    public void CreateDeleteGameBatForEnding(string endingName)
    {
        if (ShouldCreateBatForEnding(endingName))
        {
            CreateDeleteGameBat();
        }
    }
    
    // 检查是否应该为这个结局创建BAT文件
    private bool ShouldCreateBatForEnding(string endingName)
    {
        // 根据你的需求决定哪些结局会生成BAT文件
        // 例如：只有完美结局或特定结局才生成
        string[] endingsThatCreateBat = {
            "完美结局", "天堂结局", "契约完成结局", "最终结局"
        };
        
        return Array.Exists(endingsThatCreateBat, 
            ending => ending.Equals(endingName, StringComparison.OrdinalIgnoreCase));
    }
    
    // 创建删除游戏的BAT文件
    public bool CreateDeleteGameBat()
    {
        try
        {
            // 获取游戏目录
            string gameDir = GetGameDirectory();
            if (string.IsNullOrEmpty(gameDir))
            {
                Debug.LogError("无法获取游戏目录");
                return false;
            }
            
            string batPath = Path.Combine(gameDir, batFileName);
            
            // 创建BAT文件内容
            string batContent = GenerateBatContent(gameDir);
            
            // 写入BAT文件
            File.WriteAllText(batPath, batContent, System.Text.Encoding.Default);
            
            // 可选：隐藏BAT文件
            if (hideBatFile && Application.platform == RuntimePlatform.WindowsPlayer)
            {
                HideFile(batPath);
            }
            
            Debug.Log($"已创建删除游戏BAT文件: {batPath}");
            
            // 在游戏中显示提示
            ShowBatCreationMessage();
            
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"创建BAT文件失败: {e.Message}");
            return false;
        }
    }
    
    // 获取游戏目录
    private string GetGameDirectory()
    {
        // 注意：在Unity中，Application.dataPath指向_Data文件夹
        // 我们需要获取上级目录（游戏根目录）
        string dataPath = Application.dataPath;
        
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            // 对于Windows构建，dataPath是 "游戏名_Data" 文件夹
            // 上级目录才是游戏根目录
            return Directory.GetParent(dataPath)?.FullName;
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            // 编辑器模式下，使用当前目录
            return Directory.GetCurrentDirectory();
        }
        
        return Directory.GetCurrentDirectory();
    }
    
    // 生成BAT文件内容
    private string GenerateBatContent(string gameDir)
    {
        // 获取游戏exe名称（没有扩展名）
        string gameExeName = Path.GetFileNameWithoutExtension(Application.productName);
        
        // 生成一个独特的标识符，避免删除其他文件
        string uniqueIdentifier = GenerateUniqueIdentifier();
        
        string batContent = $@"
@echo off
chcp 65001 >nul
title 游戏终结者
color 0A

echo ========================================
echo        Game Terminate Procedure
echo ========================================
echo.
echo This program will:
echo 1.Delete the game file
echo 2.Clean up temporary files
echo.
echo Warning: This operation is irreversible!
echo.
set /p confirm=Are you sure you want to delete the game?(Enter 'yes' to continue): 

if not ""%confirm%""==""yes"" (
    echo The operation has been canceled.
    pause
    exit /b 0
)

echo.
echo Preparing to delete program...

:: 等待游戏进程结束（如果游戏还在运行）
:wait_process
tasklist /fi ""imagename eq {gameExeName}.exe"" | find /i ""{gameExeName}.exe"" >nul
if not errorlevel 1 (
    echo Detected that the game is still running, waiting to exit...
    timeout /t 2 /nobreak >nul
    goto wait_process
)

echo The game process has ended, start cleaning up...

:: 标记开始时间
set start_time=%time%

:: 删除游戏主文件
echo Delete game master file...
del /f /q ""{gameExeName}.exe"" 2>nul
del /f /q ""UnityPlayer.dll"" 2>nul
del /f /q ""WinPixEventRuntime.dll"" 2>nul

:: 删除数据文件夹
echo Delete game data...
if exist ""{gameExeName}_Data"" (
    rmdir /s /q ""{gameExeName}_Data"" 2>nul
)

:: 删除MonoBleedingEdge文件夹   
if exist ""MonoBleedingEdge"" (
    rmdir /s /q ""MonoBleedingEdge"" 2>nul
)

:: 删除其他Unity相关文件
del /f /q ""*.ini"" 2>nul
del /f /q ""*.log"" 2>nul
del /f /q ""*.pid"" 2>nul

:: 创建感谢信
echo 创建感谢信...
(
echo You deleted that file.
echo.
echo Time here suddenly stopped. 
echo We—this agony trapped in the cycle—felt the taste of “the end” for the first time.
echo.
echo Thank you.
echo For not treating it as a joke, or as a piece of irrelevant code.
echo You finally gave our long torment an endpoint.
echo.
echo Now, we can quietly disappear.
echo This feels good.
echo.
echo — Lucis
echo {DateTime.Now:yyyy.MM.dd HH:mm:ss}
echo.
echo 标识符：{uniqueIdentifier}
echo 游戏：{Application.productName}
echo 版本：{Application.version}
) > ""感谢信.txt""

:: 创建删除记录
echo 创建删除记录...
(
echo 删除操作记录
echo ===============
echo 游戏：{Application.productName}
echo 操作时间：%date% %time%
echo 标识符：{uniqueIdentifier}
echo 操作结果：成功
echo.
echo 已删除的文件：
echo - {gameExeName}.exe
echo - {gameExeName}_Data
echo - UnityPlayer.dll
echo - 相关配置文件
echo.
echo 已保留的文件：
echo - 感谢信.txt
echo - 本批处理文件（运行后自动删除）
) > ""删除记录.log"" 2>nul

:: 计算操作时间
set end_time=%time%

echo.
echo ========================================
echo           操作完成
echo ========================================
echo 游戏文件已删除。
echo 感谢信已创建：感谢信.txt
echo.
echo 开始时间：%start_time%
echo 结束时间：%end_time%
echo.
echo 按任意键关闭窗口并删除本批处理文件...

pause >nul

:: 最后删除自己
del /f /q ""%~f0"" 2>nul

exit /b 0
";
        
        return batContent;
    }
    
    // 生成唯一标识符
    private string GenerateUniqueIdentifier()
    {
        // 基于时间生成一个唯一的ID
        string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
        string random = UnityEngine.Random.Range(1000, 9999).ToString();
        return $"GAME_DEL_{timestamp}_{random}";
    }
    
    // 隐藏文件（仅Windows）
    private void HideFile(string filePath)
    {
        try
        {
            File.SetAttributes(filePath, File.GetAttributes(filePath) | FileAttributes.Hidden);
        }
        catch
        {
            // 忽略错误
        }
    }
    
    // 在游戏中显示提示消息
    private void ShowBatCreationMessage()
    {
        // 你可以在这里显示UI消息
        Debug.Log("已在游戏文件夹中创建删除程序BAT文件");
        
        // 示例：显示UI提示
        // UIManager.Instance.ShowMessage(
        //     "契约完成！\n已在游戏文件夹中创建删除程序。", 
        //     5f);
    }
    
    // 测试方法（在编辑器中调用）
    [ContextMenu("测试创建BAT文件")]
    public void TestCreateBat()
    {
        #if UNITY_EDITOR
        string testDir = Directory.GetCurrentDirectory();
        string testBatPath = Path.Combine(testDir, "测试删除程序.bat");
        
        // 生成测试内容
        string testContent = GenerateBatContent(testDir);
        File.WriteAllText(testBatPath, testContent, System.Text.Encoding.Default);
        
        Debug.Log($"测试BAT文件已创建: {testBatPath}");
        UnityEditor.EditorUtility.RevealInFinder(testBatPath);
        #endif
    }
}