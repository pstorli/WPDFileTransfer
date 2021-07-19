# WPDFileTransfer
Copy files to/from your pc/phone using c#

Windows Portable Device file transfer

I first want to give Christophe Geer a really big hand. He is the only person that I have found that had any example of transferring data to a phone using WPD (Windows Portable device) in c#.

There were only a few bugs that I had to fix and only a few important features that were missing that I added.

The result are a set of PortableDevice files that can be used to do most of the needed CRUD operations.

I modified the PortableDevice files in the last series, but they can also be put into any of his other WPD articles. https://github.com/geersch/WPD <- github site to download the whole tutorial series.

https://cgeers.wordpress.com/2011/05/22/enumerating-windows-portable-devices/

https://cgeers.wordpress.com/2011/06/05/wpd-enumerating-content/

https://cgeers.wordpress.com/2011/08/13/wpd-transferring-content/

https://cgeers.wordpress.com/2012/04/15/wpd-deleting-resources/

https://cgeers.wordpress.com/2012/04/17/wpd-transfer-content-to-a-device/

The file Program.cs does a simple task of either copying a folder to/from the phone from your pc
or  copying a file to/from the phone from your pc.

See the top of that file for instructions.

The files that you may want to use in your project to copy to/from a phone are:

   PortableDevice.cs
   PortableDeviceCollection.cs
   PortableDeviceFile.cs
   PortableDeviceFolder.cs
   PortableDeviceObject.cs

   Enjoy!

   :) Pete


