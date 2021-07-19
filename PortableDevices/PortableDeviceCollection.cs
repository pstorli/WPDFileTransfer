using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using PortableDeviceApiLib;

// Portable Device Collection
namespace PortableDevices
{
    public class PortableDeviceCollection : Collection<PortableDevice>, IDisposable
    {
        
        private readonly IPortableDeviceManager _deviceManager;

        public PortableDeviceCollection()
        {
            this._deviceManager = Activator.CreateInstance(typeof(CLSID_PortableDeviceManager)) as IPortableDeviceManager;
        }

        public void Dispose()
        {
            if (null != _deviceManager)
            {
                Marshal.ReleaseComObject(_deviceManager);
            }
        }

        delegate uint FieldGetter(string deviceId, char[] field, ref uint length);
        private string GetFieldValue(FieldGetter fieldGetter, string deviceId)
        {
            string result = null;

            char[] charField = null;
            uint fieldLength = 0;

            fieldGetter(deviceId, charField, ref fieldLength);
            if (0 < fieldLength)
            {
                charField = new char[fieldLength];
                fieldGetter(deviceId, charField, ref fieldLength);
                result = new string(charField);
            }

            return result;
        }

        public void Refresh()
        {
            try { 
                this._deviceManager.RefreshDeviceList();

                // Determine how many WPD devices are connected
                uint count = 0;
                // the method should take null and 0 the first time to get the number of devices 
                // https://docs.microsoft.com/windows/win32/api/portabledeviceapi/nf-portabledeviceapi-iportabledevicemanager-getdevices
                this._deviceManager.GetDevices(null, ref count);

                // Retrieve the device id for each connected device
                IntPtr[] ptr = new IntPtr[count];
                this._deviceManager.GetDevices(ptr, ref count);
                string[] devicesFound = new string[count];
                for (uint i = 0; i < count; i++)
                {
                    string deviceId = Marshal.PtrToStringUni(ptr[i]);
                    devicesFound[i] = deviceId;
                    string manufacturer = GetFieldValue(_deviceManager.GetDeviceManufacturer, deviceId);
                    string description = GetFieldValue(_deviceManager.GetDeviceDescription, deviceId);
                    string friendlyname = GetFieldValue(_deviceManager.GetDeviceFriendlyName, deviceId);
                    // check if the device is already in the collection before adding it (again)
                    if (!this.Any(d => d.DeviceId == deviceId))
                    {
                        Add(new PortableDevice(deviceId, friendlyname, manufacturer, description));
                    }
                }
                //get a collection of devices removed 
                var removedDevices = this.Where(d => !devicesFound.Contains(d.DeviceId)).ToArray();
                foreach(var removedDevice in removedDevices)
                {
                    Remove(removedDevice);
                }
                // free the memory allocated for the device ID strings
                if (ptr != null)
                {
                    for (uint i = 0; i < count; i++)
                    {
                        Marshal.FreeCoTaskMem(ptr[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.Message);
            }
}
    }
}