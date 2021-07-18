using System;
using System.IO;
using PortableDeviceApiLib;
using PortableDeviceTypesLib;
using _tagpropertykey = PortableDeviceApiLib._tagpropertykey;
using IPortableDeviceKeyCollection = PortableDeviceApiLib.IPortableDeviceKeyCollection;
using IPortableDeviceValues = PortableDeviceApiLib.IPortableDeviceValues;
using System.Runtime.InteropServices;

// Portable Device
namespace PortableDevices
{
    public class PortableDevice
    {        
        public bool _isConnected;
        public readonly IPortableDevice _device = new PortableDeviceApiLib.PortableDevice();
        public static string IS_PHONE_PLUGGED_IN = "Ensure that your Phone is plugged into this PC's usb port.";
        PortableDeviceFolder _root = null;
        /**
         * Constructor.
         */
        public PortableDevice (string deviceId, string name, string manufacturer, string description)
        {
            this.DeviceId = deviceId;
            this.Name = name;
            this.Manufacturer = manufacturer;
            this.Description = description;
        }

        /**
         * Device Id
         */
        public string DeviceId { get; private set; }
        public string Name { get; private set; }
        public string Manufacturer { get; private set; }
        public string Description { get; private set; }

        internal IPortableDevice PortableDeviceClass
        {
            get
            {
                return this._device;
            }
        }

        /**
         * Connect to device
         */
        public void Connect()
        {
            if (this._isConnected) { return; }

            var clientInfo = (IPortableDeviceValues)new PortableDeviceTypesLib.PortableDeviceValues();
            this._device.Open (this.DeviceId, clientInfo);
            _root = new PortableDeviceFolder("DEVICE", "DEVICE");
            this._isConnected = true;
        }

        /**
         * Disconnect from device
         */
        public void Disconnect()
        {
            if (!this._isConnected) { return; }
            this._device.Close();
            this._isConnected = false;
        }

        /**
         * Disconnect from then Connect to device
         */
        public void DisconnectConnect()
        {
            Disconnect();
            Connect();
        }
        
        /**
         * Get the contents of the device.
         */
        public IPortableDeviceContent getContents()
        {
            IPortableDeviceContent contents = null;

            try
            {
                Connect();
                _device.Content (out contents);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
            
            return contents;
        }

        /**
         * The root folder on the device, also enumerate all files and folders on device.
         */
        public PortableDeviceFolder Root
        {
            get
            {
                return _root;
            }
        }


        /**
         * Get required properties.
         */
        private IPortableDeviceValues GetRequiredPropertiesForContentType(
            string fileName,
            string parentObjectId)
        {
            IPortableDeviceValues values = new PortableDeviceTypesLib.PortableDeviceValues() as IPortableDeviceValues;

            var WPD_OBJECT_PARENT_ID = new _tagpropertykey();
            WPD_OBJECT_PARENT_ID.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_PARENT_ID.pid = 3;
            values.SetStringValue(ref WPD_OBJECT_PARENT_ID, parentObjectId);

            FileInfo fileInfo = new FileInfo(fileName);
            var WPD_OBJECT_SIZE = new _tagpropertykey();
            WPD_OBJECT_SIZE.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_SIZE.pid = 11;
            values.SetUnsignedLargeIntegerValue(WPD_OBJECT_SIZE, (ulong)fileInfo.Length);

            var WPD_OBJECT_ORIGINAL_FILE_NAME = new _tagpropertykey();
            WPD_OBJECT_ORIGINAL_FILE_NAME.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_ORIGINAL_FILE_NAME.pid = 12;
            values.SetStringValue(WPD_OBJECT_ORIGINAL_FILE_NAME, Path.GetFileName(fileName));

            var WPD_OBJECT_NAME = new _tagpropertykey();
            WPD_OBJECT_NAME.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_NAME.pid = 4;
            values.SetStringValue(WPD_OBJECT_NAME, Path.GetFileName(fileName));

            return values;
        }

        /**
         * String to Prop Variant
         */
        private static void StringToPropVariant(
            string value,
            out PortableDeviceApiLib.tag_inner_PROPVARIANT propvarValue)
        {
            PortableDeviceApiLib.IPortableDeviceValues pValues =
                (PortableDeviceApiLib.IPortableDeviceValues)
                    new PortableDeviceTypesLib.PortableDeviceValues();

            var WPD_OBJECT_ID = new _tagpropertykey();
            WPD_OBJECT_ID.fmtid =
                new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_ID.pid = 2;

            pValues.SetStringValue(ref WPD_OBJECT_ID, value);

            pValues.GetValue(ref WPD_OBJECT_ID, out propvarValue);
        }

        /**
         * Friendly Name
         */
        public string FriendlyName
        {
            get
            {
                string propertyValue = "";

                try
                {
                    // make sure that we are not holding on to a file.
                    DisconnectConnect();

                    if (!this._isConnected)
                    {
                        throw new InvalidOperationException("Not connected to device.");
                    }

                    // Retrieve the properties of the device
                    IPortableDeviceContent content = getContents();
                    IPortableDeviceProperties properties;
                    content.Properties(out properties);

                    // Retrieve the values for the properties
                    IPortableDeviceValues propertyValues;
                    properties.GetValues("DEVICE", null, out propertyValues);

                    // Identify the property to retrieve
                    var property = new _tagpropertykey();
                    property.fmtid = new Guid(0x26D4979A, 0xE643, 0x4626, 0x9E, 0x2B, 0x73, 0x6D, 0xC0, 0xC9, 0x2F, 0xDC);
                    property.pid = 12;

                    // Retrieve the friendly name
                    
                    propertyValues.GetStringValue(ref property, out propertyValue);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw ex;
                }
                finally
                {
                    Disconnect();
                }
                return propertyValue;
            }
        }

        /**
         * Device Dump
         */
        public void DumpDevices()
        {
            Console.WriteLine();
            Console.WriteLine("Start Device Dump");
            Console.WriteLine();

            try
            {
                // make sure that we are not holding on to a file.
                DisconnectConnect();

                Console.WriteLine(FriendlyName);

                var folder = Root;
                foreach (var item in folder.Files)
                {
                    item.DisplayObject();
                }

                Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }

            Console.WriteLine();
            Console.WriteLine("End Device Dump");
            Console.WriteLine();
        }

        /**
         * Delete a file.
         */
        public void DeleteFile(PortableDeviceFile file)
        {
            try
            {
                // make sure that we are not holding on to a file.
                DisconnectConnect();

                IPortableDeviceContent content = getContents();

                var variant = new PortableDeviceApiLib.tag_inner_PROPVARIANT();
                StringToPropVariant(file.Id, out variant);

                PortableDeviceApiLib.IPortableDevicePropVariantCollection objectIds =
                    new PortableDeviceTypesLib.PortableDevicePropVariantCollection()
                    as PortableDeviceApiLib.IPortableDevicePropVariantCollection;
                objectIds.Add(variant);

                content.Delete(0, objectIds, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw (ex);
            }
            Disconnect();
        }

        /**
         * Copy To PC
         */
        public void TransferContentFromDevice(PortableDeviceFile file, string saveToPath, String fileName)
        {
            FileStream targetStream = null;

            try
            {
                // make sure that we are not holding on to a file.
                DisconnectConnect();

                // Make sure that the target directory exists.
                System.IO.Directory.CreateDirectory(saveToPath);

                IPortableDeviceContent content = getContents();

                IPortableDeviceResources resources;
                content.Transfer(out resources);

                PortableDeviceApiLib.IStream wpdStream;
                uint optimalTransferSize = 0;

                var property = new _tagpropertykey();
                property.fmtid = new Guid(0xE81E79BE, 0x34F0, 0x41BF, 0xB5, 0x3F, 0xF1, 0xA0, 0x6A, 0xE8, 0x78, 0x42);
                property.pid = 0;

                resources.GetStream(file.Id, ref property, 0, ref optimalTransferSize, out wpdStream);

                System.Runtime.InteropServices.ComTypes.IStream sourceStream = (System.Runtime.InteropServices.ComTypes.IStream)wpdStream;

                // var fileName = Path.GetFileName(file.Id);
                targetStream = new FileStream(Path.Combine(saveToPath, fileName), FileMode.Create, FileAccess.Write);

                // Get the total size.
                long length = file.size;
                long written = 0;
                long lPCt = 0;
                unsafe
                {
                    var buffer = new byte[1024];
                    int bytesRead;
                    do
                    {
                        sourceStream.Read(buffer, 1024, new IntPtr(&bytesRead));
                        targetStream.Write(buffer, 0, bytesRead);

                        written += 1024;
                        long PCt = length > 0 ? (100 * written) / length : 100;
                        if (PCt != lPCt)
                        {
                            lPCt = PCt;
                            Console.WriteLine("Progress: " + lPCt);
                        }
                    } while (bytesRead > 0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
            finally
            {
                if (null != targetStream) targetStream.Close();
                Disconnect();
            }
        }

        /**
         * Copy To portable device
         */
        public void TransferContentToDevice (PortableDeviceFolder parentFolder, string filePath)
        {
            PortableDeviceApiLib.IStream tempStream = null;
            System.Runtime.InteropServices.ComTypes.IStream targetStream = null;
            try
            {                
                string fileName = PortableDeviceFolder.last(filePath, "\\");

                // Remove existing remote file, if it exists.
                parentFolder.DeleteFile (this, fileName);

                // make sure that we are not holding on to a file.
                DisconnectConnect();

                string parentObjectId = parentFolder.Id;

                IPortableDeviceContent content = getContents();
                IPortableDeviceValues values = GetRequiredPropertiesForContentType(filePath, parentObjectId);

                uint optimalTransferSizeBytes = 0;
                content.CreateObjectWithPropertiesAndData(
                    values,
                    out tempStream,
                    ref optimalTransferSizeBytes,
                    null);

                targetStream = (System.Runtime.InteropServices.ComTypes.IStream)tempStream;

                long length = new System.IO.FileInfo(filePath).Length;
                long written = 0;
                long lPCt = 0;

                using (var sourceStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var buffer = new byte[optimalTransferSizeBytes];
                    int bytesRead;
                    do
                    {
                        bytesRead = sourceStream.Read(buffer, 0, (int)optimalTransferSizeBytes);
                        IntPtr PCbWritten = IntPtr.Zero;
                        targetStream.Write(buffer, bytesRead, PCbWritten);

                        written += bytesRead;
                        long PCt = length > 0 ? (100 * written) / length : 100;
                        if (PCt != lPCt)
                        {
                            lPCt = PCt;
                            Console.WriteLine("Progress: " + lPCt);
                        }
                    } while (bytesRead > 0);
                }
                targetStream.Commit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
            finally
            {
                if (null != targetStream) Marshal.ReleaseComObject(targetStream);
                if (null != tempStream) Marshal.ReleaseComObject(tempStream);
                Disconnect();
            }
        }
    }
}