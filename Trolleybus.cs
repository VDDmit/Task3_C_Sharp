using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Task2_1;

public interface IEmergencyService
{
    void Repair(Trolleybus trolleybus);
}

public class CityEmergency : IEmergencyService
{
    public void Repair(Trolleybus trolleybus)
    {
        Console.WriteLine($"[CityEmergency] Repairing trolleybus {trolleybus.Number}...");
        Thread.Sleep(2000);
        trolleybus.Fix();
    }
}

public class PrivateEmergency : IEmergencyService
{
    public void Repair(Trolleybus trolleybus)
    {
        Console.WriteLine($"[PrivateEmergency] Quickly fixing trolleybus {trolleybus.Number}...");
        Thread.Sleep(1000);
        trolleybus.Fix();
    }
}

public class MetroEmergency : IEmergencyService
{
    public void Repair(Trolleybus trolleybus)
    {
        Console.WriteLine($"[MetroEmergency] Metro unit repairing trolleybus {trolleybus.Number}.");
        Thread.Sleep(3000);
        trolleybus.Fix();
    }
}

public class Driver
{
    public string Name { get; }

    public Driver(string name)
    {
        Name = name;
    }

    public void ReconnectPoles(Trolleybus trolleybus)
    {
        Console.WriteLine($"[Driver: {Name}] Reconnecting poles on trolleybus {trolleybus.Number}...");
        Thread.Sleep(1000);
        trolleybus.ReconnectPoles();
    }
}

// Интерфейс устройства
public interface ITrolleybusDevice
{
    string DisplayText { get; }
    void Start();
}

public class Trolleybus : INotifyPropertyChanged, ITrolleybusDevice
{
    private static readonly Random random = new();

    public int Number { get; }
    public Driver Driver { get; }
    public IEmergencyService EmergencyService { get; }

    public bool IsOperational { get; private set; } = true;
    public bool PolesConnected { get; private set; } = true;

    public event Action? BrokenDown;
    public event Action? PoleDisengaged;
    public event PropertyChangedEventHandler? PropertyChanged; //смена свойства - увед UI

    public string DisplayText
    {
        get
        {
            if (!IsOperational) return $"Троллейбус {Number}: сломан";
            if (!PolesConnected) return $"Троллейбус {Number}: соскочили штанги";
            return $"Троллейбус {Number}: едет";
        }
    }

    public Trolleybus(int number, Driver driver, IEmergencyService emergencyService)
    {
        Number = number;
        Driver = driver;
        EmergencyService = emergencyService;

        // Привязка обработчиков событий
        BrokenDown += () => Task.Run(() => EmergencyService.Repair(this));
        PoleDisengaged += () => Task.Run(() => Driver.ReconnectPoles(this));
    }

    public void Start()
    {
        Task.Run(() =>
        {
            while (true)
            {
                SimulateOperation();
                Thread.Sleep(1000);
            }
        });
    }

    private void SimulateOperation()
    {
        if (!IsOperational || !PolesConnected)
        {
            OnPropertyChanged(nameof(DisplayText));
            return;
        }

        int chance = random.Next(100);
        if (chance < 10)
        {
            IsOperational = false;
            Console.WriteLine($"[Trolleybus {Number}] BROKEN DOWN");
            BrokenDown?.Invoke();
        }
        else if (chance < 25)
        {
            PolesConnected = false;
            Console.WriteLine($"[Trolleybus {Number}] POLES DISENGAGED");
            PoleDisengaged?.Invoke();
        }

        OnPropertyChanged(nameof(DisplayText));
    }

    public void Fix()
    {
        IsOperational = true;
        OnPropertyChanged(nameof(DisplayText));
    }

    public void ReconnectPoles()
    {
        PolesConnected = true;
        OnPropertyChanged(nameof(DisplayText));
    }

    public void InspectViaReflection()
    {
        Console.WriteLine($"[Reflection] Properties of Trolleybus {Number}:");
        foreach (var prop in typeof(Trolleybus).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            Console.WriteLine($"- {prop.Name}: {prop.GetValue(this)}");
        }
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}