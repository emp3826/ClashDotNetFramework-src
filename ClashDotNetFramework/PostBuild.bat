RD /S /Q %TargetDir%bin >NUL 2>&1

XCOPY /s /Y %SolutionDir%Binaries %TargetDir%bin\ >NUL

DEL /f %TargetDir%*.config >NUL 2>&1
DEL /f %TargetDir%*.pdb >NUL 2>&1
DEL /f %TargetDir%*.xml >NUL 2>&1

exit 0