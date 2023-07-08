using System;
using System.Collections.Generic;
using Windows.Storage.Streams;
using Windows.Devices.Bluetooth.Advertisement;
using System.Text.RegularExpressions;

//using SDKTemplate;

namespace BLETemperatureSensor2
{
    internal class Program
    {
        static void Main(string[] args)
        {

            BluetoothLEAdvertisementWatcher watcher = new BluetoothLEAdvertisementWatcher();

            watcher.ScanningMode = BluetoothLEScanningMode.Active;

            // Attach a handler to process the received advertisement. 
            // The watcher cannot be started without a Received handler attached

            watcher.Received += OnAdvertisementReceived;

            // Attach a handler to process watcher stopping due to various conditions,
            // such as the Bluetooth radio turning off or the Stop method was called

            watcher.Stopped += OnAdvertisementWatcherStopped;

            watcher.Start();

            while (true)
            {

            }
        }

        private static async void OnAdvertisementReceived(BluetoothLEAdvertisementWatcher watcher, BluetoothLEAdvertisementReceivedEventArgs eventArgs)
        {
            // We can obtain various information about the advertisement we just received by accessing 
            // the properties of the EventArgs class

            // The timestamp of the event
            DateTimeOffset timestamp = eventArgs.Timestamp;

            // The type of advertisement
            BluetoothLEAdvertisementType advertisementType = eventArgs.AdvertisementType;

            // The received signal strength indicator (RSSI)
            Int16 rssi = eventArgs.RawSignalStrengthInDBm;

            // The local name of the advertising device contained within the payload, if any
            string localName = eventArgs.Advertisement.LocalName;

            // Check if there are any manufacturer-specific sections.
            // If there is, print the raw data of the first manufacturer section (if there are multiple).
            string manufacturerDataString = "";
            var manufacturerSections = eventArgs.Advertisement.ManufacturerData;

            var isScannable = eventArgs.IsScannable;
            var isScanReponse = eventArgs.IsScanResponse;
            var bluetoothAddress = eventArgs.BluetoothAddress;
            var blutetoothAddresType = eventArgs.BluetoothAddressType;
            var isDirected = eventArgs.IsDirected;
            var isAnon = eventArgs.IsAnonymous;
            Guid UUID = Guid.Empty;

            ulong input = bluetoothAddress;
            var tempMac = input.ToString("X");
            //tempMac is now 'E7A1F7842F17'

            var regex = "(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})";
            var replace = "$1:$2:$3:$4:$5:$6";
            var macAddress = Regex.Replace(tempMac, regex, replace);


            // if (eventArgs.Advertisement.ServiceUuids.Count > 0 ) { UUID = eventArgs.Advertisement.ServiceUuids[0]; }

            //if (manufacturerSections.Count > 0)

            var temperature = 0f;
            var battery = 0;
            var humidity = 0f;
            
            foreach(var section in manufacturerSections)
            {
                // Only print the first one of the list

                // UUID = eventArgs.Advertisement.ManufacturerData[0].
                
                var manufacturerData = section;
                var data = new byte[manufacturerData.Data.Length];
                using (var reader = DataReader.FromBuffer(manufacturerData.Data))
                {
                    reader.ReadBytes(data);
                }

                if(data.Length == 7)
                {
                    // Print the company ID + the raw data in hex format
                    manufacturerDataString = string.Format("0x{0}: length {1} : {2}",
                        manufacturerData.CompanyId.ToString("X4"),
                        manufacturerData.Data.Length,
                        BitConverter.ToString(data));
                    temperature = ((float)manufacturerData.CompanyId) / 100f;
                    humidity = ((data[1] << 8) + data[0]) / 100.0f;
                    battery = data[5];
                }

            }

            //if (advertisementType == BluetoothLEAdvertisementType.ScanResponse)
            //{
            // Display these information on the list
            //Console.WriteLine(string.Format("[{0}]: rssi={1}, bluetoothAddress=[{3}], isScannable=[{4}], name={6}], type={2}, MSD=[{5}]",
            //    timestamp.ToString("hh\\:mm\\:ss\\.fff"),
            //    rssi.ToString(),
            //    advertisementType.ToString(),
            //    macAddress.ToString(),
            //    isScannable.ToString(),
            //    manufacturerDataString, 
            //    localName,
            //    UUID.ToString()
            //    ));
            //}


            if (macAddress == "49:22:07:07:03:B3" && advertisementType == BluetoothLEAdvertisementType.ScanResponse)
            {
                Console.WriteLine(string.Format("[{0}]: bluetoothAddress=[{1}], temperature=[{2}], humidity=[{3}], battery=[{4}], manufacturerDataString=[{5}]",
                        timestamp.ToString("hh\\:mm\\:ss\\.fff"),
                        macAddress,
                        temperature,
                        humidity,
                        battery,
                        manufacturerDataString
                        //advertisementType.ToString()
                        //rssi.ToString(),
                        //localName,
                        //compa
                        //isScannable.ToString()
                        )); ;
            }

        }

        /// <summary>
        /// Invoked as an event handler when the watcher is stopped or aborted.
        /// </summary>
        /// <param name="watcher">Instance of watcher that triggered the event.</param>
        /// <param name="eventArgs">Event data containing information about why the watcher stopped or aborted.</param>
        private static async void OnAdvertisementWatcherStopped(BluetoothLEAdvertisementWatcher watcher, BluetoothLEAdvertisementWatcherStoppedEventArgs eventArgs)
        {

                Console.WriteLine(string.Format("Watcher stopped or aborted: {0}", eventArgs.Error.ToString()));

        }
    }
}