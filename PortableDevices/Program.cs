﻿using System;
using System.Linq;

/**  
 * Create a dir on your c: drive named Text "C:\Test"
 * Put in any files that you desire.
 * 
 * After program runs:
 *   On Phone: Visit dir Phone\Android\data\test
 *   On PC:    Visit dir c:\Test\CopiedBackfromPhone
 *   
 * To test Folder Copying:
 *   Set COPY_FOLDER = true;
 *   
 * To test File Copying:
 *   Set COPY_FOLDER = false;
 *   And ensure that file: "C:\Test\foo.txt" exists
 */
namespace PortableDevices
{
    class Program
    {
        private static Boolean COPY_FOLDER = true;

        // https://cgeers.wordpress.com/2012/04/17/wpd-transfer-content-to-a-device/
        static void Main()
        {
            String  error       = "";
            try
            {
                var devices = new PortableDeviceCollection();
                if (null == devices)
                {
                    error = "No devices found!";
                }
                else
                {
                    devices.Refresh();

                    var device = devices.First();
                    if (null == device)
                    {
                        error = "No device found!";
                    }
                    else
                    {                        
                        // Copy folder to device from pc.
                        error = copyToDevice (device);
                        if (String.IsNullOrEmpty(error))
                        {
                            error = @"Copied folder C:\Test to Phone\Android\data\test";
                        }
                        Console.WriteLine(error);

                        // Copy folder back to pc from device.
                        error = copyFromDevice(device);
                        if (String.IsNullOrEmpty(error))
                        {
                            error = @"Copied folder Phone\Android\data\test to c:\Test\CopiedBackfromPhone";
                        }

                        device.Disconnect();
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            finally
            {
                Console.WriteLine(error);
            }
        }

        /**
         * Copy test file to device.
         */
        public static String copyToDevice (PortableDevice device)
        {
            String error = "";

            try
            {
                // Try to find the data dir on the phone.
                String               phoneDir = @"Phone\Android\data";
                PortableDeviceFolder root     = device.Root();
                PortableDeviceFolder result   = root.FindDir (phoneDir);
                if (null == result)
                {
                    // Perhaps it was a tablet instead of a phone?
                    result = device.Root().FindDir(@"Tablet\Android\data");
                    phoneDir = @"Tablet\Android\data";
                }

                // Did we find a the desired folder on the device?
                if (null == result)
                {
                    error = phoneDir + " not found!";
                }
                else
                {
                    // Create remote test dir.
                    result = result.CreateDir (device, "test");

                    string pcDir = @"C:\Test\";

                    if (COPY_FOLDER)
                    {
                        // copy a whole folder. public void CopyFolderToPhone (PortableDevice device, String folderPath, String destPhonePath)
                        result.CopyFolderToPhone(device, pcDir, phoneDir);
                    }
                    else
                    {
                        // Or Copy a single file. 
                        device.TransferContentToDevice (result, pcDir + "foo.txt");
                    }
                }

            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return error;
        }

        /**
         * Copy test file to device.
         */
        public static String copyFromDevice (PortableDevice device)
        {
            String error = "";

            try
            {
                PortableDeviceFolder root   = device.Root();
                PortableDeviceObject result = root.FindDir (@"Phone\Android\data\test");
                if (null == result)
                {
                    // Perhaps it was a tablet instead of a phone?
                    result = root.FindDir(@"Tablet\Android\data\test");
                }

                // Did we find a the desired folder on the device?
                if (null == result)
                {
                    error = @"Dir Android\data not found!";
                }
                else if (result is PortableDeviceFolder)
                {                    
                    if (COPY_FOLDER)
                    {
                        // Copy a whole folder
                        ((PortableDeviceFolder)result).CopyFolderToPC(device, @"C:\Test\CopiedBackfromPhone");
                    }
                    else
                    {
                        // Or Copy a file
                        PortableDeviceFile file = ((PortableDeviceFolder)result).FindFile("foo.txt");                    
                        device.TransferContentFromDevice (file, @"C:\Test\CopiedBackfromPhone", "Copyfoo.txt");
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return error;
        }
    }   
}
