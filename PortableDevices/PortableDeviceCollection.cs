using System;
using System.Collections.ObjectModel;
using PortableDeviceApiLib;

// Portable Device Collection
namespace PortableDevices
{
    public class PortableDeviceCollection : Collection<PortableDevice>
    {
        private readonly PortableDeviceManager _deviceManager;

        public PortableDeviceCollection()
        {
            this._deviceManager = new PortableDeviceManager();
        }

        public void Refresh()
        {
            try { 
                this._deviceManager.RefreshDeviceList();

                // Determine how many WPD devices are connected
                string[] deviceIds = new string[1];
                uint count = 1;
                // the method should take null and 0 the first time to get the number of devices - this does not work 
                // https://docs.microsoft.com/windows/win32/api/portabledeviceapi/nf-portabledeviceapi-iportabledevicemanager-getdevices
                this._deviceManager.GetDevices(ref deviceIds[0], ref count);

                // Retrieve the device id for each connected device
                deviceIds = new string[count];
                this._deviceManager.GetDevices(ref deviceIds[0], ref count);
                foreach (var deviceId in deviceIds)
                {
                    Add (new PortableDevice(deviceId));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.Message);
            }
}
    }
}