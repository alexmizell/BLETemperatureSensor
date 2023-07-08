using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace BLETemperatureSensor
{
    internal class Program
    {

        static DeviceInformation device = null;
        static async Task Main(string[] args)
        {

            Boolean notified = false;

            // Query for extra properties you want returned
            string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };

            DeviceWatcher deviceWatcher =
                        DeviceInformation.CreateWatcher(
                                BluetoothLEDevice.GetDeviceSelectorFromPairingState(false),
                                requestedProperties,
                                DeviceInformationKind.AssociationEndpoint);

            // Register event handlers before starting the watcher.
            // Added, Updated and Removed are required to get all nearby devices
            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Updated += DeviceWatcher_Updated;
            deviceWatcher.Removed += DeviceWatcher_Removed;

            // EnumerationCompleted and Stopped are optional to implement.
            deviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
            deviceWatcher.Stopped += DeviceWatcher_Stopped;

            // Start the watcher.
            deviceWatcher.Start();

            while (true)
            {
                if (device == null)
                {
                    Thread.Sleep(200);
                }
                else
                {

                    if (!notified)
                    {
                        //Console.WriteLine("Press \"any\" key to pair with IBS-TH2...");
                        //Console.ReadKey();

                        BluetoothLEDevice bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(device.Id);

                        Console.WriteLine("Attempting to pair with device");
                        GattDeviceServicesResult result = await bluetoothLeDevice.GetGattServicesAsync();

                        if (result.Status == GattCommunicationStatus.Success)
                        {
                            Console.WriteLine("Pairing succeeded");
                            var services = result.Services;
                            foreach (var service in services)
                            {

                                //IReadOnlyList<GattCharacteristic> characteristics2 = service.GetAllCharacteristics();

                                //foreach(var characteristic in characteristics2)
                                //{
                                //    //Console.WriteLine("UUID: " + service.Uuid.ToString());
                                //    Console.WriteLine("Characteristic:  " + characteristic.CharacteristicProperties.ToString());

                                //}

                                Console.WriteLine("Service UUID: " + service.Uuid);
                                //Console.WriteLine("UUID: " + service.ToString());
                                //Console.WriteLine("UUID: " + service.Session.ToString());
                                //Console.WriteLine("UUID: " + service.Device.ToString());

                                //if (service.Uuid.ToString() == "00001801-0000-1000-8000-00805f9b34fb")  // {00002a05-0000-1000-8000-00805f9b34fb}
                                //{
                                //    Console.WriteLine("Found temperature service...");
                                GattCharacteristicsResult charactiristicResult = await service.GetCharacteristicsAsync();

                                if (charactiristicResult.Status == GattCommunicationStatus.Success)
                                {
                                    var characteristics = charactiristicResult.Characteristics;
                                    foreach (var characteristic in characteristics)
                                    {
                                        //Console.WriteLine(characteristic);
                                        GattCharacteristicProperties properties = characteristic.CharacteristicProperties;

                                        if (properties.HasFlag(GattCharacteristicProperties.Read))
                                        {
                                            // This characteristic supports reading from it.
                                            Console.WriteLine("       " + characteristic.Uuid + " " + characteristic.UserDescription + " supports reading.");

                                            var characteristicUUID = characteristic.Uuid.ToString().Remove(8).Remove(0, 4);

                                            if (characteristicUUID == "" ||
                                                characteristicUUID == "" ||
                                                characteristicUUID == "" ||
                                                characteristicUUID == "" ||
                                                characteristicUUID == "" ||
                                                characteristicUUID == "" ||
                                                characteristicUUID == "" ||
                                                characteristicUUID == "" ||
                                                characteristicUUID == "fff1" ||
                                                characteristicUUID == "fff2" ||
                                                characteristicUUID == "fff3" ||
                                                characteristicUUID == "fff4" ||
                                                characteristicUUID == "fff5" ||
                                                characteristicUUID == "fff6" ||
                                                characteristicUUID == "fff7" ||
                                                characteristicUUID == "fff8" ||
                                                characteristicUUID == "")
                                            {

                                                GattReadResult result2 = await characteristic.ReadValueAsync();
                                                if (result.Status == GattCommunicationStatus.Success)
                                                {
                                                    var reader = DataReader.FromBuffer(result2.Value);
                                                    byte[] input = new byte[reader.UnconsumedBufferLength];


                                                    //reader.ReadBytes(input);

                                                    // The encoding and byte order need to match the settings of the writer 
                                                    // we previously used.
                                                    reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                                                    //reader.ByteOrder = Windows.Storage.Streams.ByteOrder.LittleEndian;

                                                    // Once we have written the contents successfully we load the stream.
                                                    //await reader.LoadAsync((uint)stream.Size);

                                                    //var receivedStrings = "";

                                                    // Keep reading until we consume the complete stream.
                                                    while (reader.UnconsumedBufferLength > 0)
                                                    {
                                                        // Note that the call to readString requires a length of "code units" 
                                                        // to read. This is the reason each string is preceded by its length 
                                                        // when "on the wire".

                                                        byte[] bytesToRead = new byte[reader.UnconsumedBufferLength];

                                                        reader.ReadBytes(bytesToRead);

                                                        foreach (byte b in bytesToRead)
                                                        {
                                                            char thisChar = (char)b;

                                                            Console.Write($"{thisChar}");


                                                        }
                                                        Console.WriteLine("");
                                                        foreach (byte b in bytesToRead)
                                                        {
                                                            string hexValue = b.ToString("X2");

                                                            Console.Write($"{hexValue} ");


                                                        }
                                                        Console.WriteLine("");
                                                        foreach (byte b in bytesToRead)
                                                        {

                                                            int decimalValue = (int)b;

                                                            Console.Write($"{decimalValue}, ");

                                                        }
                                                        Console.WriteLine("");
                                                    }

                                                    //Console.WriteLine(receivedStrings);
                                                    Console.WriteLine("");
                                                }

                                            }

                                        }
                                        if (properties.HasFlag(GattCharacteristicProperties.Write))
                                        {
                                            // This characteristic supports writing to it.
                                            Console.WriteLine("       " + characteristic.Uuid + " " + characteristic.UserDescription + " supports writing.");
                                        }
                                        if (properties.HasFlag(GattCharacteristicProperties.Notify))
                                        {
                                            // This characteristic supports subscribing to notifications.
                                            Console.WriteLine("       " + characteristic.Uuid + " " + characteristic.UserDescription + " supports notifications.");


                                            GattCommunicationStatus status = await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                                            GattClientCharacteristicConfigurationDescriptorValue.Notify);

                                            if (status == GattCommunicationStatus.Success)
                                            {

                                                Console.WriteLine("subscribed");
                                                Console.WriteLine("");
                                                characteristic.ValueChanged += Characteristic_ValueChanged;
                                                // Server has been informed of clients interest.
                                            }

                                        }




                                    }
                                }

                                Console.WriteLine("");

                            }
                        }



                        notified = true;

                    }
                    else // we are signed up for notifications
                    {

                        Console.WriteLine("waiting for notifications...");
                        Console.WriteLine("");

                        while (true)
                        {
                            Console.WriteLine("Press Any Key to Exit application");
                            Console.ReadKey();
                            break;
                        }
                    }
                }
            }
        }

        private static void DeviceWatcher_Stopped(DeviceWatcher sender, object args)
        {
            //throw new NotImplementedException();
        }

        private static void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            //throw new NotImplementedException();
        }

        private static void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            //throw new NotImplementedException();
        }

        private static void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            //throw new NotImplementedException();
        }

        private static void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            //Console.WriteLine(args.Id);
            //Console.WriteLine(args.Kind.ToString());
            //Console.WriteLine("");

            if (args.Id.Contains("49:22:07:07:03:b3"))
            {
                Console.WriteLine("Found the IBS-TH2 by MAC address.");

                device = args;

            }

             
        }

        private static void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var reader = DataReader.FromBuffer(args.CharacteristicValue);
            var flags = reader.ReadByte();
            var value = reader.ReadByte();
            Console.WriteLine($"{flags} - {value}");
        }

    }
}