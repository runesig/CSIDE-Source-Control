# CSIDE Source Control
### Wether we like it or not, the good old C/SIDE environment is going to be with us NAV developers for a long time yet.
### To maintain C/AL, code the C/SIDE Source Control will make it a lot easier for you to with its Git support.
#### Install from here: https://byxinternaldiag.blob.core.windows.net/csidesourcecontrol/publish.htm
#### or pull the source control and build it with Visual Studio.
#### You must also have Git installed which you can get from here: https://git-scm.com/download/win
## Usage
#### Start the application and go to the tab "Repository Setup" -> "Set Destination Folder"
#### This is where the source C/AL object files will be stored and te Git repository will be setup.
####
#### The really old versions of NAV does noly support exporting the text file with objects and dragging them into C/SIDE Source Control.
#### Remember: You can include ALL the objects in the txt file with objects you export from NAV. C/SIDE Source Control will split it automatically up and arrange it correctly.
#### For newer versions of C/SIDE the finsql.exe supports extracting objects. You can then use the "Import" function to automatically import objects into C/SIDE Source Control.
#### But you will have to setup server settings under "Repository Setup" -> "Server Setup"
#### Lookup the finsql.exe file and copy the other server settings from the Micosoft Dynamics NAV Development Environment under File->Database->Information in the Development Environment. 
