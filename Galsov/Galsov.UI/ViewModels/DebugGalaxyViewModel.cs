using Galsov.Core.Galaxy.Generation;
using Galsov.Core.Galaxy.Models;   // StarSystem lives here
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Shapes;
using Galsov.Core.Sessions;

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
        
        private GameSession? _gameSession;
        private int _turnNumber;

        private Galaxy? _galaxy;
        private StarSystemPointViewModel? _selectedStarPoint;
        private StarSystem? _selectedSystem;

        public DebugGalaxyViewModel()
        {
            GenerateCommand = new RelayCommand(_ => GenerateGalaxy());

            // Only valid once we actually have a GameSession.
            NextTurnCommand = new RelayCommand(
                _ => AdvanceTurn(),                      // <<-- NEW
                _ => GameSession != null);               // <<-- NEW

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
        public GameSession? GameSession
        {
            get => _gameSession;
            private set => SetField(ref _gameSession, value);
        }

        public int TurnNumber
        {
            get => _turnNumber;
            private set => SetField(ref _turnNumber, value);
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

        public Galaxy? Galaxy
        {
            get => _galaxy;
            private set => SetField(ref _galaxy, value);
        }

        // The point we clicked on the canvas
        public StarSystemPointViewModel? SelectedStarPoint
        {
            get => _selectedStarPoint;
            set
            {
                if (SetField(ref _selectedStarPoint, value))
                {
                    UpdateSelectionFromStarPoint(value);
                }
            }
        }

        // The underlying core StarSystem used for the detail panel      <<-- NEW LINE
        public StarSystem? SelectedSystem
        {
            get => _selectedSystem;
            private set => SetField(ref _selectedSystem, value);
        }

        // ---- commands ----

        public ICommand GenerateCommand { get; }
        public ICommand NextTurnCommand { get; }

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
        // ---- selection logic ----

        private void UpdateSelectionFromStarPoint(StarSystemPointViewModel? starPoint)
        {
            // Clear selection highlight on all star points
            foreach (var s in StarSystems)
            {
                s.IsSelected = false;
            }

            if (starPoint == null || _galaxy == null)
            {
                SelectedSystem = null;
                return;
            }

            // Mark this star as selected
            starPoint.IsSelected = true;

            // Look up the corresponding core StarSystem
            var system = _galaxy.StarSystems
                .FirstOrDefault(s => s.Id == starPoint.Id);

            SelectedSystem = system;
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
                    PlanetCount = system.PlanetCount,
                    IsSelected = system.IsSelected
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

                // 2. Create a new game session using the factory (GameSession is now the root).
                var session = GameSessionFactory.Create(options);   // <<-- NEW

                GameSession = session;                              // <<-- NEW
                TurnNumber = session.TurnNumber;                    // <<-- NEW

                // 3. Convert star systems from the session's galaxy into display points.
                StarSystems.Clear();

                var galaxy = session.Galaxy;                        // <<-- NEW
                var scale = Scale;

                foreach (var system in galaxy.StarSystems)
                {
                    StarSystems.Add(
                        new StarSystemPointViewModel
                        {
                            Id = system.Id,
                            TileX = system.X,
                            TileY = system.Y,
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
                GameSession = null;         // <<-- NEW
                TurnNumber = 0;             // <<-- NEW
            }
        }
        private void AdvanceTurn()
        {
            if (GameSession is null)
            {
                ErrorMessage = "No game session yet. Generate a galaxy first.";
                return;
            }

            try
            {
                GameSession.AdvanceTurn();
                TurnNumber = GameSession.TurnNumber;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
    }

    // Simple display model for the canvas
    public sealed class StarSystemPointViewModel : INotifyPropertyChanged
    {
        public int Id { get; init; }

        // Tile coordinates in the galaxy grid
        public int TileX { get; init; }
        public int TileY { get; init; }

        // Pixel positions on the canvas
        public double X { get; init; }
        public double Y { get; init; }

        // Star info + planet count for debug/tooltip
        public StarClass StarClass { get; init; }
        public int PlanetCount { get; init; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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