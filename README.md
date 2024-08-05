# Using the Library

This library is designed for implementing submarines in mods for the game Subnautica. If that is not what you're doing, then this package is not for you.<br>
To use the library, you will need to add it to your Unity project that is using [Thunderkit](https://github.com/PassivePicasso/ThunderKit/).<br>

The components you need will depend on the sub you are making, but some of the more versatile components are the ``Spawn____`` ones, which spawn and assign elements of the Cyclops to your sub.<br>

## Save System

This library also comes with a custom save system for your subs. Simply add the ``SubSerializationManager`` to your prefab root, and then create a class for your save data 
extending either ``ModuleDataClass`` or ``BaseSubDataClass``. Then in the ``Save Data Type Class Name`` field on the manager put in the full ``Type`` of your save data class. 
I.e. ``Chameleon.SaveData.ChameleonSaveDataClass``. <br>
The manager will also call any ``ISaveDataListener`` or ``ILateSaveDataListener``s on save and load. These methods provide a ``BaseSubDataClass`` and you will need to cast it to your data class. <br>
If the data isn't your data class (I.e. the cast is null) it means your sub doesn't have save data yet. I recommend making an extension method for ``BaseSubDataClass`` which ensures that your data isn't null and casts it to your data type.

## When forking/modifying the repo:

You will need to go into the ``Configuration.targets`` file and change the ``CommonDir`` to the path to your Steam library folder so that the project builds correctly.
