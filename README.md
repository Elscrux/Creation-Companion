# Creation-Editor (wip name)

[![.NET](https://github.com/Elscrux/Creation-Editor/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Elscrux/Creation-Editor/actions/workflows/dotnet.yml)

## Download Latest Build
Download the latest version [here](https://github.com/Elscrux/Creation-Editor/actions).

## Contribute
Join the [Mutagen Discord](https://discord.gg/GdKZ3SH) and ping me there @Elscrux, so we can talk about the project. A basic project setup with issues is already in place and pull requests are always welcome.

## Overview
The goal is to create a modern IDE that replaces the Creation Kit for mod creation in Skyrim modding and potentially other Bethesda games.

The are many tools like xEdit that do a similar job, but nothing goes as far as trying to replace the Creation Kit itself. This is supposed to change with the power of [Mutagen](https://github.com/Mutagen-Modding/Mutagen "Mutagen"). Mutagen is the starting point for the development of the editor which would otherwise be a much larger undertaking.

### Why?
While being a great way for the community to create mods, the Creation Kit has a lot of issues. It is slow, prone to crashes, unintuitive in a lot of ways, undocumented, not easily expandable and more.

## Vision

### Fast
[Mutagen](https://github.com/Mutagen-Modding/Mutagen "Mutagen") already provides a fast library for modifying mods. Together with asyncrous loading in the background, the startup for the tool should be nearly instant.

### Familiar
Most early development will copy Creation Kit features and recreate them in a better and faster way with Mutagen. Experienced Creation Kit users should feel familiar and have an easy experience switching over from the Creation Kit.

### Documented
The Creation Kit doesn\'t have any kind of help integrated into the tool which adds to the already big learning curve. The Creation Editor is supposed to provide tooltips and helpful hints anywhere possible. Usually sources like [Creation Kit Wiki](https://ck.uesp.net "Creation Kit Wiki") will be used for this documentation.

### Intelligent
The Creation Editor is supposed to get a lot of quality of life features that assist you in creating mods. That could go in a similar direction as [Mutagen Analyzers](http://https://github.com/Mutagen-Modding/Mutagen.Bethesda.Analyzers "Mutagen Analyzers"). The goal is to automatically get feedback on what you do. There should be warnings for potential issues or suggestions for improvements as well as automatic ways to do both with the click of a button.

### Workflow
Say you want to create a mod using the Creation Kit and after a while you realize that you need to switch to xEdit in order to make a relevant tweak. This would mean closing the Creation Kit, loading up xEdit make that change and loading up the Creation Kit again. The Creation Editor should not only improve on this with fast loading times, but also an xEdit-like view to modify a mod. No need to switch the tool at all - everything can be done in one place.

### Productivity
Create custom scripts that can be recompiled at runtime to perform actions in batch and other simple tasks to help in a productive workflow without repitition, powered by Mutagen. There are more advanced patching tools and this doesn\'t intend to recreate them.

### Plugins
There should be support for custom 3rd party extensions or integrations that people can install.
