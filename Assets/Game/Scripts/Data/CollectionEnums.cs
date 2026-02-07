public enum TypeEnemy { CHILD, FLY, TEEN, STRONG, WOMAN, BOSS }
public enum DiceType { NONE, DICE, SLOT, DECK, ENERGY, SPECIAL, }
public enum TypeRoll { DECK, ENEMY, SKILLS }
public enum Phases { NONE, ROLLING, PLANNING, ATTACKING, VICTORY, DEFEAT, TUTORIAL }
public enum PhaseLoading { NONE, ENTER, LOADING}
public enum DataProccess { NONE, ROWS, EACH_TIME_BATTLE, VOLUME_MASTER, VOLUME_MUSIC, VOLUME_SFX }
public enum BoardState { LOADING, WAITING, MOVING }
public enum TypeMusic { OPENING, MAIN, MAP, BATTLE, BLOODSHED, BOSS, CREDITS, TUTORIAL }
public enum TypeSFX { HIT, NAV, SUBMIT, BLOCK }
public enum SceneToLoad { NONE, MENU, MAP, COMBAT, CREDITS, TUTORIAL, STORY }
public enum TypeSaveData { BINARY, JASON, }
public enum TilePosition { CENTRAL, LEFT, RIGHT }
public enum TileType { ONCE, TWICE, THRICE }
public enum TypeTile
{
    /// <summary> Start point from map </summary>
    START,
    /// <summary> Single battle with a group </summary>
    BATTLE,
    /// <summary> Wave battles with various groups </summary>
    BLOODSHED,
    /// <summary> Boss at the map´s end </summary>
    BOSS,
    /// <summary> Choose a dice to add to deck </summary>
    DECK,
    /// <summary> Start point from map </summary>
    ROLL,
    /// <summary> Change Protection type </summary>
    PROTECTION,
    /// <summary> Choose Beetween Health, Energy or Skill dice </summary> 
    HEALTH,
    /// <summary> Choose Beetween Health, Energy or Skill dice </summary> 
    ENERGY,
    /// <summary> Choose Beetween Health, Energy or Skill dice </summary> 
    SKILL,
    /// <summary> Choose Beetween Health, Energy or Skill dice </summary> 
    SHUFFLE,
}
public enum TypeSkill 
{
    NONE,
    /// <summary> Change enemies stats </summary>
    ENEMIES_STATS,
    /// <summary> Deal Damage for all enemies </summary>
    ENEMIES_INDEFENSE,
    /// <summary> Change enemies defenses type </summary>
    ENEMIES_DEFENSES,
    /// <summary> Heal one health dice </summary>
    PLAYER_HEAL,
    /// <summary> Restore one energy dice </summary>
    PLAYER_RESTORE,
    /// <summary> Makes player invincible one turn </summary>
    PLAYER_INVINCIBLE,
}
public enum TypeDefense
{
    /// <summary> Damage must be higher than defense value, in tha case, damage is total </summary>
    FRAGILE,
    /// <summary> Damage must be higher than defense value, in tha case, damage will reduce with defense value </summary>
    ENDURANCE,
    /// <summary> Dice Value must fit as Even or Odd like defense value to pass</summary>
    EVEN_ODD,
    /// <summary> Dice value must equal with Attack value to pass</summary>
    MATCH,
    /// <summary> Dice value must equal with Attack value to pass</summary>
    FLIP,
}
public enum TypeRumble { NONE, BLOCK, ATTACK, ROLLING }
public enum DamageType
{
    /// <summary> NORMAL Damage </summary>
    NORMAL,
    /// <summary> GARLIC Damage </summary>
    GARLIC,
    /// <summary> STAKE Damage </summary>
    STAKE,
    /// <summary> ULTRAVIOLET Damage </summary>
    ULTRAVIOLET,
    /// <summary> SILVER Damage </summary>
    SILVER,
    /// <summary> HOLY Damage </summary>
    HOLY,
}
public enum SuckType
{
    /// <summary> NORMAL Damage </summary>
    NORMAL,
    /// <summary> GARLIC Damage </summary>
    HEALTH,
    /// <summary> STAKE Damage </summary>
    ENERGY,
}
