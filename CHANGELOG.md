# 1.5.0.9 (10.01.2023)
- PlantUML version updated to 1.2023.0
- Added fluent and dark style icons
- Updated to common zoom and scroll behaviour Ctrl + mouse wheel to zoom and (Shift +) mouse wheel to scroll vertically and horizontally
- Added include option
- Added metadata with option to include generated document to SVG export
- Disabled refresh button after generation until document was changed again
- Grayed out disabled buttons
- Fixed potential crash if generation failed
- Fixed generation may takes exponentially long for diagrams producing big images
- Fixed setting Java path in options not applied without restarting Notepad++
- Switched order of Ok and Cancel button in options window
- Added information about used PlantUML version in about window
- Json.NET library updated to 13.0.2
- SVG.NET library updated to 3.4.4

# 1.4.0.8 (07.10.2022)
- Plugin target framework updated to .NET Framework 4.6.2
- PlantUML version updated to 1.2022.8
- Added support for multiple pages per document
- Added error if the input document is not UTF-8 encoded
- Added file name of generated diagram to bottom status bar
- Update plugin colors automatically if Notepad++ colors changed
- Fixed potential crash aborting the generation
- SVG.NET library updated to 3.4.3

# 1.3.0.7 (16.08.2022)
- PlantUML version updated to 1.2022.6
- Included necessary binaries for JLatexMath and Batik to enable generation of formulas
- Added support for multiple diagrams per document
- Removed corresponding line to error message if diagram generation fails due to a syntax error which is not supported for multiple diagrams
- Fixed using the text of the selected document in split mode
- Added buttons to zoom
- Used icons for all buttons
- Used loading circle instead of progress bar to show refreshing
- Added button to check for update in about window
- Used custom modified version of PlantUml.Net library

# 1.2.0.6 (11.06.2022)
- Added export of SVG files
- Added option to define the export size factor for PNG files
- Added context menu to copy to clipboard and export diagram
- Added corresponding line to error message if diagram generation fails due to a syntax error
- Switched settings file format from *.ini to *.xml
- Fixed crash generating large documents with more than 10000 characters
- Fixed bottom status bar overlapping the diagrams horizontal scroll bar

# 1.1.1.5 (27.05.2022)
- Removed shortcut Shift + U

# 1.1.0.4 (27.05.2022)
- Added support for dark interfaces
- Implemented cancellation of an ongoing generation
- Improved error message if Java was not found
- Extended icon to highlight itself and close preview if opened
- Added menu entry and shortcut Shift + U to refresh diagram
- Renamed settings to options

# 1.0.2.3 (22.05.2022)
- Fixed generation fails for release builds
- Removed error for generation of empty documents

# 1.0.1.2 (22.05.2022)
- Fixed generation fails if Notepad++ installation path contains special characters

# 1.0.0.1 (21.05.2022)
- Initial release
