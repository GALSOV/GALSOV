using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Galsov.Core.Galaxy.Models;
using Galsov.Core.Galaxy.Generation;

#nullable enable

namespace Galsov.UI.ViewModels
{
    public sealed class DebugGalaxyViewModel : INotifyPropertyChanged
    {
        // ---- backing fields ----
        private ulong _seed = 1;
        private int _width = 100;
        private int _height = 100;
        private int _starSystemCount = 200;
        private GalaxyDistributionPattern _distributionPattern = GalaxyDistributionPattern.Uniform;
        private int _edgeMargin = 0;
        private int _minSystemSpacing = 0;

        public DebugGalaxyViewModel()
        {
            GenerateCommand = new RelayCommand(_ => GenerateGalaxy());
            StarSystems = new ObservableCollection<StarSystemPointViewModel>();
        }

        // ---- options exposed to the UI ----

        public ulong Seed
        {
            get => _seed;
            set => SetField(ref _seed, value);
        }

        public int Width
        {
            get => _width;
            set => SetField(ref _width, value);
        }

        public int Height
        {
            get => _height;
            set => SetField(ref _height, value);
        }

        public int StarSystemCount
        {
            get => _starSystemCount;
            set => SetField(ref _starSystemCount, value);
        }

        public GalaxyDistributionPattern DistributionPattern
        {
            get => _distributionPattern;
            set => SetField(ref _distributionPattern, value);
        }

        public int EdgeMargin
        {
            get => _edgeMargin;
            set => SetField(ref _edgeMargin, value);
        }

        public int MinSystemSpacing
        {
            get => _minSystemSpacing;
            set => SetField(ref _minSystemSpacing, value);
        }

        private string? _errorMessage;

        public string? ErrorMessage
        {
            get => _errorMessage;
            set => SetField(ref _errorMessage, value);
        }

        // Collection of all available distribution patterns for the ComboBox
        public Array DistributionPatterns { get; } =
            Enum.GetValues(typeof(GalaxyDistributionPattern));

        // ---- generated stars (for display) ----

        public ObservableCollection<StarSystemPointViewModel> StarSystems { get; }

        // ---- commands ----

        public ICommand GenerateCommand { get; }

        // ---- INotifyPropertyChanged boilerplate ----

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        // ---- generation logic (will fill next) ----

        private void GenerateGalaxy()
        {
            // Clear any previous error
            ErrorMessage = string.Empty;

            try
            {
                // 1. Build options from the current VM state
                var options = new GalaxyGeneratorOptions
                {
                    Seed = Seed,
                    Width = Width,
                    Height = Height,
                    StarSystemCount = StarSystemCount,
                    DistributionPattern = DistributionPattern,
                    EdgeMargin = EdgeMargin,
                    MinSystemSpacing = MinSystemSpacing
                };

                // 2. Create the generator
                var generator = new GalaxyGenerator();

                // 3. Generate the galaxy
                var galaxy = generator.Generate(options);

                // 4. Convert star systems to display points (pixels)
                StarSystems.Clear();

                // Basic scaling factor: how many pixels per "tile".
                // We can tune this later or make it dynamic.
                const double scale = 6.0;

                foreach (var system in galaxy.StarSystems)
                {
                    StarSystems.Add(new StarSystemPointViewModel
                    {
                        Id = system.Id,
                        X = system.X * scale,
                        Y = system.Y * scale
                    });
                }
            }
            catch (Exception ex)
            {
                // For now, just expose the message; we can improve this later
                ErrorMessage = ex.Message;

                // Clear stars so we don't show stale data
                StarSystems.Clear();
            }
        }
    }

    // Simple display model for the canvas
    public sealed class StarSystemPointViewModel
    {
        public int Id { get; init; }

        // Pixel positions on the canvas
        public double X { get; init; }
        public double Y { get; init; }
    }

    // Minimal ICommand implementation (standard MVVM pattern)
    public sealed class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

        public void Execute(object? parameter) => _execute(parameter);
    }
}