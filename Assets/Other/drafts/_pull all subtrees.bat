@echo off

set "names=database"
set "names=%names% dataview"
set "names=%names% dialogue"
set "names=%names% editing"
set "names=%names% extension"
set "names=%names% i18n"
set "names=%names% interactable"
set "names=%names% inventory"
set "names=%names% menu"
set "names=%names% pattern"
set "names=%names% splitscreen"
set "names=%names% tilemap3d"
set "names=%names% trigger"
set "names=%names% turnbased"
set "names=%names% utility"
set "names=%names% webrequest"

for %%i in (%names%) do (
	git subtree pull -P %%i %%i master --allow-unrelated-histories
)

pause