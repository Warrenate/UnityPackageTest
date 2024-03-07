goto start
命令使用方法：
    .\CreateTag.bat 'packageRootDir' branchName tagName
如：
    .\CreateTag.bat 'Math API' master 0.0.1

:start
echo off
set rootDir=%~1
set branch=%2
set tag=%3
git subtree split --prefix="Packages/%rootDir%" --branch %branch%
git tag %tag% %branch%
git push origin %tag%
git branch -D %branch%
@REM echo "press any key to continue..."
pause
