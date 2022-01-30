using System.Collections.Generic;
using UnityEngine;

namespace Cores.Components {

    [System.Serializable]
    public class Cooler : CoreComponent {

        public override int slotSize => Level.levels[levelIndex].slotSize;

        public Level level => Level.levels[levelIndex];

        public Core.TemperatureImpulse GetCoolImpulse () {
            var currentLevel = Level.levels[levelIndex];
            return new Core.TemperatureImpulse(){
                targetTemperature = GameState.DEFAULT_TEMPERATURE + currentLevel.temperatureDelta,
                impulseStrength = currentLevel.coolImpulseStrength
            };
        }

        [System.Serializable]
        public class Level {

            public static IReadOnlyList<Level> levels { get; private set; }

            [field: SerializeField] public int slotSize { get; private set; }
            [field: SerializeField] public float temperatureDelta { get; private set; }
            [field: SerializeField] public float coolImpulseStrength { get; private set; }
            [field: SerializeField, InlineProperty] public CoolerView.ViewInitData viewInit { get; private set; }

            public static void EnsureLevelsInitialized (IEnumerable<Level> inputLevels) {
                levels = levels ?? new List<Level>(inputLevels);
            }

        }
        
    }

}