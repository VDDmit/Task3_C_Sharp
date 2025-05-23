using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Task2_1;

public partial class MainWindow : Window
{
    private ListBox listBox;
    private ObservableCollection<Trolleybus> devicesList;
    private Button addTrolleybusButton;
    private Button deleteDeviceButton;
    private Button reflectButton;
    private TextBlock reflectionOutput;

    private int counter = 0;

    public MainWindow()
    {
        InitializeComponent();

        // Привязки к элементам XAML
        listBox = this.FindControl<ListBox>("DevicesList");
        addTrolleybusButton = this.FindControl<Button>("AddTrolleybusButton");
        deleteDeviceButton = this.FindControl<Button>("DeleteDeviceButton");
        reflectButton = this.FindControl<Button>("ReflectButton");
        reflectionOutput = this.FindControl<TextBlock>("ReflectionOutput");

        // Инициализация списка
        devicesList = new ObservableCollection<Trolleybus>();
        listBox.ItemsSource = devicesList;

        // События
        addTrolleybusButton.Click += AddTrolleybus;
        deleteDeviceButton.Click += DeleteDevice;
        reflectButton.Click += ReflectSelected;
    }

    private void AddTrolleybus(object sender, RoutedEventArgs e)
    {
        var driver = new Driver($"Водитель #{counter + 1}");

        IEmergencyService service;
        int typeIndex = counter % 3;
        switch (typeIndex)
        {
            case 0:
                service = new CityEmergency();
                break;
            case 1:
                service = new PrivateEmergency();
                break;
            case 2:
                service = new MetroEmergency();
                break;
            default:
                service = new CityEmergency();
                break;
        }

        counter++;
        var trolley = new Trolleybus(counter, driver, service);
        devicesList.Add(trolley);
        trolley.Start();
    }

    private void DeleteDevice(object sender, RoutedEventArgs e)
    {
        if (listBox.SelectedItem is Trolleybus trolley)
        {
            devicesList.Remove(trolley);
        }
    }

    private void ReflectSelected(object sender, RoutedEventArgs e)
    {
        if (listBox.SelectedItem is Trolleybus trolley)
        {
            var props = typeof(Trolleybus).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            string result = "";

            foreach (var prop in props)
            {
                object? value = prop.GetValue(trolley);
                result += $"{prop.Name}: {value}\n";
            }

            reflectionOutput.Text = result;
        }
        else
        {
            reflectionOutput.Text = "Сначала выберите троллейбус из списка.";
        }
    }
}