
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

if not "%confirm%"=="yes" (
    echo The operation has been canceled.
    pause
    exit /b 0
)

echo.
echo Preparing to delete program...

:: 等待游戏进程结束（如果游戏还在运行）
:wait_process
tasklist /fi "imagename eq Lucis.exe" | find /i "Lucis.exe" >nul
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
del /f /q "Lucis.exe" 2>nul
del /f /q "UnityPlayer.dll" 2>nul
del /f /q "WinPixEventRuntime.dll" 2>nul

:: 删除数据文件夹
echo Delete game data...
if exist "Lucis_Data" (
    rmdir /s /q "Lucis_Data" 2>nul
)

:: 删除MonoBleedingEdge文件夹   
if exist "MonoBleedingEdge" (
    rmdir /s /q "MonoBleedingEdge" 2>nul
)

:: 删除其他Unity相关文件
del /f /q "*.ini" 2>nul
del /f /q "*.log" 2>nul
del /f /q "*.pid" 2>nul

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
echo 2025.12.13 13:04:51
echo.
echo Identifier:GAME_DEL_20251213130451614_3340
echo Game:Lucis
echo Version:1.0
) > "Thank-You Letter.txt"

:: 创建删除记录
echo 创建删除记录...
(
echo 删除操作记录
echo ===============
echo 游戏：Lucis
echo 操作时间：%date% %time%
echo 标识符：GAME_DEL_20251213130451614_3340
echo 操作结果：成功
echo.
echo 已删除的文件：
echo - Lucis.exe
echo - Lucis_Data
echo - UnityPlayer.dll
echo - 相关配置文件
echo.
echo 已保留的文件：
echo - 感谢信.txt
echo - 本批处理文件（运行后自动删除）
) > "删除记录.log" 2>nul

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
del /f /q "%~f0" 2>nul

exit /b 0
