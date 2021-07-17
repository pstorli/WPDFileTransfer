using System;
using System.Collections.ObjectModel;
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

                for (uint i = 0; i < count; i++)
                {
                    string str = Marshal.PtrToStringUni(ptr[i]);
                    char[] manufacturer = null;
                    uint manufactureLength = 0;
                    _deviceManager.GetDeviceManufacturer(str, manufacturer, ref manufactureLength);
                    if (0 < manufactureLength)
                    {
                        manufacturer = new char[manufactureLength];
                        _deviceManager.GetDeviceManufacturer(str, manufacturer, ref manufactureLength);
                        Console.WriteLine($"Manufactured by {new string(manufacturer)}");
                    }
                    Add(new PortableDevice(str));
                }
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