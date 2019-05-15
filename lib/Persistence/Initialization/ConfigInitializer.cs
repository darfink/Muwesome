using System.Collections.Generic;
using System.Linq;
using Muwesome.DomainModel.Components;
using Muwesome.DomainModel.Configuration;
using Muwesome.DomainModel.Entities;
using Muwesome.Persistence.Initialization.Utility;

namespace Muwesome.Persistence.Initialization {
  internal class ConfigInitializer {
    private readonly IPersistenceContextProvider persistenceContextProvider;
    private GameConfiguration gameConfiguration;
    private IContext context;

    /// <summary>Initializes a new instance of the <see cref="ConfigInitializer"/> class.</summary>
    public ConfigInitializer(IPersistenceContextProvider persistenceContextProvider) {
      this.persistenceContextProvider = persistenceContextProvider;
    }

    public void Initalize() {
      using (this.context = this.persistenceContextProvider.CreateContext()) {
        this.gameConfiguration = this.context.Create<GameConfiguration>();
        this.gameConfiguration.MapDefinitions.AddRange(this.CreateMapDefinitions());
        this.gameConfiguration.CharacterClasses.AddRange(this.CreateCharacterClasses());
        this.gameConfiguration.ItemDefinitions.AddRange(this.CreateItemDefinitions());
        this.context.SaveChanges();
      }
    }

    private IEnumerable<MapDefinition> CreateMapDefinitions() {
      var terrain = MapTerrain.FromStream(ResourceManager.GetResourceStream("Terrain1.att"));
      var lorencia = this.context.Create<MapDefinition>("Lorencia", (byte)0, terrain);
      yield return lorencia;
    }

    private IEnumerable<CharacterClass> CreateCharacterClasses() {
      var homeMap = this.gameConfiguration.MapDefinitions.First(m => m.Number == 0);
      var characterClass = this.context.Create<CharacterClass>("Dark Wizard", homeMap);
      characterClass.PointsPerLevel = 5;
      yield return characterClass;
    }

    private IEnumerable<ItemDefinition> CreateItemDefinitions() {
      var item = this.context.Create<ItemDefinition>("Short Sword", new ItemCode(group: 0, number: 0));
      item.Width = 1;
      item.Height = 3;
      yield return item;
    }
  }
}