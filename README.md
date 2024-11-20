# Using the Library

This library is designed for implementing submarines in mods for the game Subnautica. If that is not what you're doing, then this package is not for you.

To use the library, you will need to add it to your Unity project that is using [Thunderkit](https://github.com/PassivePicasso/ThunderKit/).

The components you need will depend on the sub you are making, but some of the more versatile components are the ``Spawn____`` ones, which spawn and assign elements of the Cyclops to your sub.

For more extensive documentation, refer to this link: https://github.com/Indigocoder1/SubLibrary/tree/main

## When forking/modifying the repo:

You will need to go into the ``Configuration.targets`` file and change the ``CommonDir`` to the path to your Steam library folder so that the project builds correctly.