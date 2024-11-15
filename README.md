To keep track of sequences and branches of memory values for the game Star Fox Adventures. To be used with https://github.com/Goal3-14/K5E-Memory-Viewer which is a modified version of [zcanann's memory heap visualiser](https://github.com/zcanann/K5E) required to actually access the memory and send it to the mapper.



### Tags
You can tag memory values with predefined key information to colour the node and give an indication of what part of the manip its at. Left click will always tag the current memory value, right click will tag whatever memory value was last clicked. You can also add any custom text information to memory nodes using the inputs at the bottom right. Pressing these buttons before the program has started mapping will cause a crash currently.
### Save States
The program can keep track of and swap out savestates to selected parts of the memory map. Memory values which have savestates have an extra darker box around them. Currently the program can only use the 10th save state slot, specifically GSAE01.s10. To use this feature copy and paste the folder path to your dolhpin save state folder (the path should NOT inculde the GSAE01.s10 file) to the text file "Save State Folder Path".
The 'SaveStates' folder stores savestates and their corresponding memory addresses. States can be saved and loaded using the save and load buttons at the bottom right.
### Wait Result
We typically only care about the memory value immediatly before shooting fireblast, to stop the large amount of memory values while shooting being added to the map, in the bottom right buttons there's a 'Wait Result' button which will pause the map from updating until its either pressed again or you've tagged the final memory value.
### Graph Control
The map can be moved with right click and zoom in and out with mousewheel but the zoom is very dodgy. There are buttons in the display options to refocus the map back on current values and to reset zoom if needed. Theres also an option to toggle whether the graph visual will automatically follow the current memory value or not. 'Full Graph' lets you look at the whole graph freely, 'Main' just focuses on your current value and the values immediatly around it. Changing the window size will cause the graph edges to disconnect from the nodes, but this will fix itself once basically any action is done with the map.
### Save Menu
The input bar above the save button does nothing currently. Pressing New Root will add the current memory value as the start of a new tree while keeping the current data aswell. New Tree will clear all data then add your current memory value as the start of a new map. The 'Saved Maps' folder is where the memory maps are stores. 
