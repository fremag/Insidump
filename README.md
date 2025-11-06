# Insidump
Analyze .net memory dump file with terminal ui

<img width="1468" height="789" alt="image" src="https://github.com/user-attachments/assets/04049c4e-00a8-45b6-bada-18616cd0877b" />

It's **NOT** [MemoScope 2](https://github.com/fremag/MemoScope.Net)

MemoScope was written in 2015 with Winforms and its goal was to analyze dump files from .Net framwork 32 bit (< 4 Go)
I don't think a web app is a good choice here so no Blazor, React etc
WinForms is obsolete and I don't want to try WPF, MAUI, WinUI, Electron etc. 

So why not a text ui ? (I was a fan of [Norton Commander](https://fr.wikipedia.org/wiki/Norton_Commander) back in the days)
[Terminal.GUI](https://github.com/gui-cs/Terminal.Gui) has the tree table view I needed so let's go for this framework.
If it works with Linux through a terminal with ssh, I'll be happy :)

The goal is to open and analyze large dump files (~100 Go, it takes hours with Memoscope) and have most useful features:
* Types: display type infos and instances
* Thread: calls + stack
* Strings: show duplicated strings and memory wasted
* Patterns: analyze how some instances are kept in memory and try to find memory leaks

Of course, it uses [ClrMd](https://github.com/microsoft/clrmd) and it could not exist without it.

Why Insidump ? because it sounds like "Inside Dump" and [Inside Out](https://www.youtube.com/watch?v=mx2DsFOvXqw) from the Brecker Brothers.
