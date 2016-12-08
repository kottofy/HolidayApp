using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Gpio;
using System.Diagnostics;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;

namespace HolidayApp
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const int
            GREEN_LED_PIN1 = 20,
            RED_LED_PIN1 = 21,
            GREEN_LED_PIN2 = 17, 
            RED_LED_PIN2 = 27, 
            GREEN_LED_PIN3 = 22, 
            RED_LED_PIN3 = 26;
        private GpioPin[] pins = new GpioPin[6];
        private int ledSwitch = 0;
        private DispatcherTimer timer;
        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush greenBrush = new SolidColorBrush(Windows.UI.Colors.Green);
        private SolidColorBrush grayBrush = new SolidColorBrush(Windows.UI.Colors.Gray); 
        private const string DeviceConnectionString = "";
        private static int MESSAGE_COUNT = 1;

        public MainPage()
        {
            InitializeComponent();
            
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(20);
            
            InitGPIO();
            timer.Tick += Timer_Tick;
            StartTimer();

#pragma warning disable 4014
            Start();
#pragma warning restore 4014
        }

        private void StartTimer()
        {
            timer.Start();
        }

        private void UpdateText(string message)
        {
            TweetText.Text = message;
        }
        
        public async Task Start()
        {
            try
            {
                DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString, TransportType.Amqp);

               // await SendEvent(deviceClient);
                await ReceiveCommands(deviceClient);

                Debug.WriteLine("Exited!\n");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in sample: {0}", ex.Message);
            }
        }

        async Task SendEvent(DeviceClient deviceClient)
        {
            string dataBuffer;

            Debug.WriteLine("Device sending {0} messages to IoTHub...\n", MESSAGE_COUNT);

            for (int count = 0; count < MESSAGE_COUNT; count++)
            {
                dataBuffer = string.Format("Msg from UWP: {0}_{1}", count, Guid.NewGuid().ToString());
                Message eventMessage = new Message(Encoding.UTF8.GetBytes(dataBuffer));
                Debug.WriteLine("\t{0}> Sending message: {1}, Data: [{2}]", DateTime.Now.ToLocalTime(), count, dataBuffer);

                await deviceClient.SendEventAsync(eventMessage);
            }
        }

         async Task ReceiveCommands(DeviceClient deviceClient)
        {
            Debug.WriteLine("\nDevice waiting for commands from IoTHub...\n");
            Message receivedMessage;
            string messageData;

            while (true)
            {
                receivedMessage = await deviceClient.ReceiveAsync();

                if (receivedMessage != null)
                {
                    messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                    Debug.WriteLine("\t{0}> Received message: {1}", DateTime.Now.ToLocalTime(), messageData);

                    await deviceClient.CompleteAsync(receivedMessage);
                    StartTimer();
                    UpdateText(messageData);
                }

                //  Note: In this sample, the polling interval is set to 
                //  10 seconds to enable you to see messages as they are sent.
                //  To enable an IoT solution to scale, you should extend this //  interval. For example, to scale to 1 million devices, set 
                //  the polling interval to 25 minutes.
                //  For further information, see
                //  https://azure.microsoft.com/documentation/articles/iot-hub-devguide/#messaging
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        

    private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();
            // Show an error if there is no GPIO controller

            if (gpio == null)
            {
                GpioStatus.Text = "There is no GPIO controller on this device.";

                return;
            }

            // OPEN PINS
            pins[0] = gpio.OpenPin(GREEN_LED_PIN1);
            pins[1] = gpio.OpenPin(RED_LED_PIN1);
            pins[2] = gpio.OpenPin(GREEN_LED_PIN2);
            pins[3] = gpio.OpenPin(RED_LED_PIN2);
            pins[4] = gpio.OpenPin(GREEN_LED_PIN3);
            pins[5] = gpio.OpenPin(RED_LED_PIN3);

            // SET DRIVE MODE
            foreach (GpioPin pin in pins)
            {
                pin.SetDriveMode(GpioPinDriveMode.Output);
            }

            // WRITE VALUE TO PINS
            foreach (GpioPin pin in pins)
            {
                pin.Write(GpioPinValue.Low);
            }

            GpioStatus.Text = "Happy Holidays!";
        }

        private void setAllLEDsOff (GpioPin[] pinArray)
        {
            foreach (GpioPin pin in pinArray)
            {
                pin.Write(GpioPinValue.High);
            }
        }

        private void setAllLEDsOn(GpioPin[] pinArray)
        {
            foreach (GpioPin pin in pinArray)
            {
                pin.Write(GpioPinValue.Low);
            }
        }

        private void Timer_Tick(object sender, object e)
        {
            switch (ledSwitch)
            {
                case 0:
                    setAllLEDsOff(pins);
                    pins[0].Write(GpioPinValue.Low);
                    LED.Fill = greenBrush;
                    ledSwitch++;
                    break;
                case 1:
                    setAllLEDsOff(pins);
                    pins[1].Write(GpioPinValue.Low);
                    LED.Fill = redBrush;
                    ledSwitch++;
                    break;
                case 2:
                    setAllLEDsOff(pins);
                    pins[2].Write(GpioPinValue.Low);
                    LED.Fill = greenBrush;
                    ledSwitch++;
                    break;
                case 3:
                    setAllLEDsOff(pins);
                    pins[3].Write(GpioPinValue.Low);
                    LED.Fill = redBrush;
                    ledSwitch++;
                    break;
                case 4:
                    setAllLEDsOff(pins);
                    pins[4].Write(GpioPinValue.Low);
                    LED.Fill = greenBrush;
                    ledSwitch++;
                    break;
                case 5:
                    setAllLEDsOff(pins);
                    pins[5].Write(GpioPinValue.Low);
                    LED.Fill = redBrush;
                    ledSwitch++;
                    break;
                case 6:
                    setAllLEDsOff(pins);
                    pins[4].Write(GpioPinValue.Low);
                    LED.Fill = greenBrush;
                    ledSwitch++;
                    break;
                case 7:
                    setAllLEDsOff(pins);
                    pins[3].Write(GpioPinValue.Low);
                    LED.Fill = redBrush;
                    ledSwitch++;
                    break;
                case 8:
                    setAllLEDsOff(pins);
                    pins[2].Write(GpioPinValue.Low);
                    LED.Fill = greenBrush;
                    ledSwitch++;
                    break;
                case 9:
                    setAllLEDsOff(pins);
                    pins[1].Write(GpioPinValue.Low);
                    LED.Fill = redBrush;
                    ledSwitch++;
                    break;
                case 10:
                    setAllLEDsOff(pins);
                    pins[0].Write(GpioPinValue.Low);
                    LED.Fill = redBrush;
                    ledSwitch++;
                    break;
                case 11:
                    setAllLEDsOff(pins);
                    LED.Fill = grayBrush;
                    ledSwitch++;
                    break;
                case 12:
                    pins[0].Write(GpioPinValue.High);
                    pins[2].Write(GpioPinValue.High);
                    pins[4].Write(GpioPinValue.High);
                    pins[1].Write(GpioPinValue.Low);
                    pins[3].Write(GpioPinValue.Low);
                    pins[5].Write(GpioPinValue.Low);
                    LED.Fill = redBrush;
                    ledSwitch++;
                    break;
                case 13:
                    setAllLEDsOff(pins);
                    LED.Fill = grayBrush;
                    ledSwitch++;
                    break;
                case 14:
                    pins[0].Write(GpioPinValue.Low);
                    pins[2].Write(GpioPinValue.Low);
                    pins[4].Write(GpioPinValue.Low);
                    pins[1].Write(GpioPinValue.High);
                    pins[3].Write(GpioPinValue.High);
                    pins[5].Write(GpioPinValue.High);
                    LED.Fill = greenBrush;
                    ledSwitch++;
                    break;
                case 15:
                    setAllLEDsOff(pins);
                    LED.Fill = grayBrush;
                    ledSwitch++;
                    break;
                case 16:
                    setAllLEDsOn(pins);
                    LED.Fill = grayBrush;
                    ledSwitch = 0;
                    timer.Stop();
                    break;        
                default:
                    setAllLEDsOff(pins);
                    LED.Fill = grayBrush;
                    ledSwitch = 0;
                    break;
            }
        }

    }
}
