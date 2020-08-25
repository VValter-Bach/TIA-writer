# Graus

**disclaimer: It contains german words**

Graus is a small project to work with the TIA Portal Openness API. You manly want to use it if you are Mad, or if you want to Copy some functionailty.
It reads an File called minimum.xlsx (an Excel File) an creates an TIA project.
You need https://www.nuget.org/packages/DocumentFormat.OpenXml/
you need the excel file and the csvs in the same dir then the .exe

Feel free to write me a mail to Valentin.Assfalg@azo.com

**It wont work out of the Box for you**

## Data structure 

### PlantComponents
Plant components contains the datastructure for representing a plant. 

#### Unit
A unit is the Plant representation. it contains sveral PLCs.

#### PLC
A PLC is a PLC. It contains several ProcessCells and IOContorllers.

#### ProcessCell
A theortical unit to separate a Plant into different parts. e.g. a part with sveral silo could be a silopart. :D

#### EquipmentModule
A unit which fullfills a function. eg. a Silo with sieve could be an EquipmentModule. 

#### ContorllModule
A motor, a Valve, whatever you directly controll with plctags in an TIA Portal.

### IOs
The functionality for extarnal switch cabinets. 

#### IOContoller
A SwitchCabinet. 

#### IOCard 
A card that contains several IOs (Plc tags).

#### Tag
you have guessed it. Its a Tag.

## Parsing Data 
All csv Data are Parsed into an map, which are used in process later

#### SignalData
For a short name for a PLC tag we use SignalData, to know which data goes throug the wire. furthermore it contains info what datatype and reading operator are used for a signal.
SIGNAL_NAME;SIGNAL_SHORT;SIGNAL_READ_SIGN;SIGNAL_DATATYPE

#### ObjectIdentifier
If you want to add a specific item to TIA you need modell information, which are saved here. 
NAME;MODELL_NUMBER/VERSION

#### ControllMoudulesData
it defines what contorll module have what functionblock in the library. and what IO goes in the FB. The last two entries may repeat infinte.
CM_NAME;CM_NAME_IN_LIB;SIGNAL_NAME;FB_PARAMETER_NAME

#### EMCMF
This defines an EquipmentModule. It list which ContorllModules are related to it. and what functions they fullfill, the last two elements may repeat. 
EM_NAME;CM_NAME;CM_FUNCTION_NAME

## XML Generation
It is in the XML folder. for Usage go look in FBwriter in TiaWriter.cs
basic functionaty:
you want to call some FB, export that FB, use TiaXmlReader to read it and call TiaXmlWriter to generate FB, that you can export.

## Excel 
Sheet 1 is self explaining, Sheet two is more complex: 

At the top left it says "PLC;Rechner1". PLC is only a name, via which can contain model information. "Rechner1", on the other hand, is the Name that is assigned to the PLC in the project. All other cells in this line are subordinate areas of the PLC.
In the line below, the area "Rohstoffbereitstellung" is defined more precisely. The cell to the right are Equipmentmodules. The entry "ET mit ext. Siebung;IO_1;ET_01" is the system displays. The function module is "ET mit ext. Siebung".
It is connected to the switchcabinet with the name "IO_1". The block itself is named "ET_1". However, there are also entries like "Abscheider;IO_2". This module has a name that was not specified. The Area definitions in the left column consist of two parts. Before the Semicolon is the name of the area already mentioned in line 1. The area following The semicolon is the default name. 
In the "Druckerversorgung" area, after the semicolon "Abschneider", so the elements are called "Abschneider 1", "Abschneider 2" and so on.

Translated with www.DeepL.com/Translator (free version)
Use DeppL. Its awesome
