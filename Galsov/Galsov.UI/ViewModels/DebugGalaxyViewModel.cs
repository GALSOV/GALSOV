using Galsov.Core.Galaxy.Generation;
using Galsov.Core.Galaxy.Models;   // StarSystem lives here
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Windows.Input;

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
        private double _scale = 6.0;
        private StarSystemPointViewModel? _selectedSystem;

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

        public double Scale
        {
            get => _scale;
            set
            {
                if (SetField(ref _scale, value))
                {
                    RescaleStars();
                }
            }
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
        // Currently selected star system
        public StarSystemPointViewModel? SelectedSystem
        {
            get => _selectedSystem;
            set => SetField(ref _selectedSystem, value);
        }

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

        private void RescaleStars()
        {
            if (StarSystems.Count == 0)
                return;

            // Take a copy of the existing points
            var existing = StarSystems.ToList();
            StarSystems.Clear();

            var scale = Scale;

            foreach (var system in existing)
            {
                StarSystems.Add(new StarSystemPointViewModel
                {
                    Id = system.Id,
                    TileX = system.TileX,
                    TileY = system.TileY,

                    // Recompute pixel positions from the tile coords + current scale
                    X = system.TileX * scale,
                    Y = system.TileY * scale,

                    // Keep the same debug info
                    StarClass = system.StarClass,
                    PlanetCount = system.PlanetCount
                });
            }
        }
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

                // Use the current Scale value to determine pixels per tile.
                var scale = Scale;

                foreach (var system in galaxy.StarSystems)
                {
                    StarSystems.Add(new StarSystemPointViewModel
                    {
                        Id = system.Id,
                        TileX = system.X,
                        TileY = system.Y,
                        X = system.X * scale,
                        Y = system.Y * scale,
                        StarClass = system.StarClass,
                        PlanetCount = system.Planets?.Count ?? 0,
                        Planets = system.Planets ?? new List<Planet>()
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

        // Tile coordinates in the galaxy grid
        public int TileX { get; init; }
        public int TileY { get; init; }

        // Pixel positions on the canvas
        public double X { get; init; }
        public double Y { get; init; }

        // New: star + planet info for debug/tooltip
        public StarClass StarClass { get; init; }
        public int PlanetCount { get; init; }
        public IReadOnlyList<Planet> Planets { get; init; } = Array.Empty<Planet>();
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