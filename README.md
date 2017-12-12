# CoolWPFWritingTool
A customized text editor using WPF

**Attention**: For now there is only a german version of the tool available



## Simple Writer

The actual text editor and the main project for the Visual Studio solution

### Features
* Text editor using its own file type
* Multiple files at once editable
* Auto-Save options
* Customizable color schemes
* Seperate folder for fonts useable, so they are not needed to be installed
* Using own watermarks
* Usage of a side image possible
* Character tab
* Own text style
* Uses a seperate dictionary for names
* Dictionary editor
* Keyboard shortcuts for some styles
* Export options
	* xaml display of the FlowDocument
	* pdf 
	* rtf
* etc.

### How to's:

#### Setup fonts
* (optional) Compile the project
* Go to the "Fonts" folder inside your Debug/Release directory
* Add a folder (for example "ComputerFonts")
* Create a info.txt file and insert "Computer fonts" into it
* Add some fonts in the ComputerFonts folder, that are fitting in 


#### Change color schemes
* (optional) Compile the project 
* Go to the "ColorSet" Folder
* Open the Colors.xaml
* Change the values, that you like to change

**There is a helper tool in the planing stage to edit the colors**

## Font Organizer

A helper tool for organizing the fonts, that are used by the Simple Writer.

### Usage:
After opening the tool, you are prompted to select two folders. The first one defines the source, where unsorted fonts are located and the second one defines your main font directory, where all fonts are located.
