# Tracking
Record and play mouse and keyboard events on windows

Start the app and hit record, position the mouse where you want to begin before the countdown ends.
Do something with mouse and keyboard then when you want to finish hit NumLock on the keyboard.
Hit close to finish or choose from the options to insert some logic into the script.

when you finish you can save your script to a file. it doesn't matter the file extention, i like to put - .sqn.
The saved file is just a text file you can edit.

# Actions
these are the actions in the sequance file, you can edit, delete or insert actions.\
action structure are like this : action name|param1|param2| ...

delay the execution
```
delay|miliseconds
```

adds a number to existing var
```
AddNumber|var name|number
```

creates a var
```
AssignVar|var name|value
```

read a value from the registry to a var
```
ReadRegistry|VarName|RegistryPath|ValueName
```

write from var to registry
```
WriteRegistry|VarName|RegistryPath|ValueName 
```

mouse event, like move or left click
```
mouse|mouseEvent.dwExtraInfo|mouseEvent.flags|mouseEvent.mouseData|mouseEvent.nCode|mouseEvent.wParam|mouseEvent.flags|mouseEvent.x|mouseEvent.y
```

keyboard event
```
Keyboard|keyboardEvent.dwExtraInfo|keyboardEvent.flags|keyboardEvent.nCode|keyboardEvent.scanCode|keyboardEvent.time|keyboardEvent.vkCode|keyboardEvent.wParam
```

load and run a sequance file.
```
RunActionFile|sqn file path
```

holds the execution as long as in pixel position x,y there is a color RGB
```
WaitWhileColor|x|y|R|G|B
```

holds the execution until the color RGB is in pixel position x,y
```
WaitForColor|x|y|R|G|B
```

simulate typing the text parameters
```
KeyboardString|text
```

show message to user. holds the execution until user clicks OK.
```
ShowMessage|Title|Content
```

run an entire directory and all the sequance file in it.
```
RunDirectory|Dir path
```
