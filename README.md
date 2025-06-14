# Insidump
Analyze .net memory dump file with terminal ui

![image](https://github.com/user-attachments/assets/e54da970-b3d5-4fae-ad78-652248682efb)

It's **NOT** [MemoScope 2](https://github.com/fremag/MemoScope.Net)

MemoScope was written in 2015 with Winforms and it's goal was to analyze  dump files from .Net framwork 32 bit (< 4 Go)
I don't think a wepapp is a good choice here so no Blazor, React etc
WinForms is obsolete and I don't want to try WPF, MAUI, WinUI, Electron etc. 
So why not a text ui ? (I was a fan of [Norton Commander](https://fr.wikipedia.org/wiki/Norton_Commander) back in the days ;)
[Terminal.GUI](https://github.com/gui-cs/Terminal.Gui) has the tree table view I needed so let's go for this framework.

The goal is to open and analyze large dump files (~100 Go, it takes hours with Memoscope) and have most useful features:
* Types: display type infos and instances
* Thread: calls + stack
* Strings: show duplicated strings and memory wasted
* Patterns: analyze how some instances are kept in memory and try to find memory leaks
