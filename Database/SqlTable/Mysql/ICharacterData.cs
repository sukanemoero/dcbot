using System;
using System.Collections.Generic;
using System.Linq;
using dcbot.Exceptions.Database;
using dcbot.General;

namespace dcbot.Database.SqlTable.Mysql;

public interface ICharacterData : IDatabase
{
    public static uint SchoolCount = Sql.TypeCount(Tables.MainSchool);
    public static uint StarCount = Sql.TypeCount(Tables.MainBaseStar);
    protected interface IDataEnumIndex
    {
        public static uint Position(string p)
        {
            return p.ToLower() switch
            {
                "middle" => 1,
                "back" => 2,
                "front" => 3,
                _ => 0
            };
        }

        public static uint Role(string r)
        {
            return r.ToLower() switch
            {
                "attacker" or "dealer" => 1,
                "support" => 2,
                "healer" => 3,
                "t.s." => 4,
                _ => 0
            };
        }

        public static uint ArmorType(string ar)
        {
            return ar.ToLower() switch
            {
                "light" => 1,
                "heavy" => 2,
                "special" => 3,
                "elastic" => 4,
                _ => 0
            };
        }

        public static uint AttackType(string at)
        {
            return at.ToLower() switch
            {
                "explosive" => 1,
                "penetration" or "piercing" => 2,
                "mystic" => 3,
                "sonic" => 4,
                _ => 0
            };
        }

        public static uint WeaponType(string wt)
        {
            return wt.ToLower() switch
            {
                "ar" => 1,
                "ft" => 2,
                "gl" => 3,
                "hg" => 4,
                "mg" => 5,
                "mt" => 6,
                "rg" => 7,
                "rl" => 8,
                "sg" => 9,
                "smg" => 10,
                "sr" => 11,
                _ => 0
            };
        }

        public static uint SquadType(string st)
        {
            return st.ToLower() switch
            {
                "striker" => 1,
                "special" => 2,
                _ => 0
            };
        }

        public static uint School(string s)
        {
            return s.ToLower() switch
            {
                "abydos" => 1,
                "arius" => 2,
                "etc" => 3,
                "gehenna" => 4,
                "hyakkiyako" => 5,
                "millennium" => 6,
                "redwinter" => 7,
                "shanhaijing" => 8,
                "srt" => 9,
                "sakugawa" => 10,
                "trinity" => 11,
                "valkyrie" => 12,
                _ => 0
            };
        }
    }

    protected abstract record Tables
    {
        public const string Main = "characters";
        public static Database.Mysql.TableValue MainId => new(Main, "id");
        public static Database.Mysql.TableValue MainBaseStar => new(Main, "baseStar");
        public static Database.Mysql.TableValue MainPosition => new(Main, "position");
        public static Database.Mysql.TableValue MainRole => new(Main, "role");
        public static Database.Mysql.TableValue MainAttackType => new(Main, "attackType");
        public static Database.Mysql.TableValue MainArmorType => new(Main, "armorType");
        public static Database.Mysql.TableValue MainWeaponType => new(Main, "weaponType");
        public static Database.Mysql.TableValue MainSquadType => new(Main, "squadType");
        public static Database.Mysql.TableValue MainSchool => new(Main, "school");


        public const string Names = "character_names";
        public static Database.Mysql.TableValue NamesId => new(Names, "id");
        public static Database.Mysql.TableValue NamesName => new(Names, "name");
        public static Database.Mysql.TableValue NamesEnglishName => new(Names, "englishName");


        public const string Profiles = "character_profiles";
        public static Database.Mysql.TableValue ProfilesId => new(Profiles, "id");
        public static Database.Mysql.TableValue ProfilesProfile => new(Profiles, "profile");
        public static Database.Mysql.TableValue ProfilesEnglishProfile => new(Profiles, "englishProfile");

        public const string TerrainDamageDealt = "character_terrain_damage_dealt";
        public static Database.Mysql.TableValue TerrainDamageDealtId => new(TerrainDamageDealt, "id");
        public static Database.Mysql.TableValue TerrainDamageDealtCity => new(TerrainDamageDealt, "city");
        public static Database.Mysql.TableValue TerrainDamageDealtDesert => new(TerrainDamageDealt, "desert");
        public static Database.Mysql.TableValue TerrainDamageDealtIndoor => new(TerrainDamageDealt, "indoor");


        public const string TerrainShieldBlockRate = "character_terrain_shield_block_rate";
        public static Database.Mysql.TableValue TerrainShieldBlockRateId => new(TerrainShieldBlockRate, "id");
        public static Database.Mysql.TableValue TerrainShieldBlockRateCity => new(TerrainShieldBlockRate, "city");
        public static Database.Mysql.TableValue TerrainShieldBlockRateDesert => new(TerrainShieldBlockRate, "desert");
        public static Database.Mysql.TableValue TerrainShieldBlockRateIndoor => new(TerrainShieldBlockRate, "indoor");
    }

    public class CharacterDataType(uint id)
    {
        protected internal uint Id { get; } = id;
        protected internal string Name { get; init; } = "";
        protected internal string Profile { get; init; } = "";

        private string _enName;
        private string _enProfile;

        protected internal string EnName
        {
            get => _enName ?? Name;
            set => _enName = value;
        }

        protected internal string EnProfile
        {
            get => _enProfile ?? Profile;
            set => _enProfile = value;
        }

        protected internal uint BaseStar { get; init; }
        protected internal string Position { get; init; } = "";
        protected internal string Role { get; init; } = "";
        protected internal string ArmorType { get; init; } = "";
        protected internal string AttackType { get; init; } = "";
        protected internal string WeaponType { get; init; } = "";
        protected internal string SquadType { get; init; } = "";
        protected internal string School { get; init; } = "";

        public class TerrainValue
        {
            protected internal uint City { get; init; }
            protected internal uint Desert { get; init; }
            protected internal uint Indoor { get; init; }
        }

        protected internal TerrainValue DamageDealt { get; set; }
        protected internal TerrainValue ShieldBlockRate { get; set; }
    }

    protected static void UpdateCharacter(CharacterDataType data)
    {
        Console.WriteLine("Inserting characters using database...");
        Console.WriteLine($"\tInserting character: {data.Id}");
        try
        {
            Sql.Insert(
                "characters",
                new Dictionary<string, object>
                {
                    { Tables.MainId.Value, data.Id },
                    { Tables.MainBaseStar.Value, data.BaseStar },
                    { Tables.MainPosition.Value, IDataEnumIndex.Position(data.Position) },
                    { Tables.MainRole.Value, IDataEnumIndex.Role(data.Role) },
                    { Tables.MainArmorType.Value, IDataEnumIndex.ArmorType(data.ArmorType) },
                    { Tables.MainAttackType.Value, IDataEnumIndex.AttackType(data.AttackType) },
                    { Tables.MainWeaponType.Value, IDataEnumIndex.WeaponType(data.WeaponType) },
                    { Tables.MainSquadType.Value, IDataEnumIndex.SquadType(data.SquadType) },
                    { Tables.MainSchool.Value, IDataEnumIndex.School(data.School) }
                },
                ["id"]
            );
            Console.WriteLine($"\tInserting character's name: `{data.Name}`");

            Sql.Insert(
                "character_names",
                new Dictionary<string, object>
                {
                    { Tables.NamesId.Value, data.Id },
                    { Tables.NamesName.Value, data.Name },
                    { Tables.NamesEnglishName.Value, data.EnName }
                },
                ["id"]
            );


            Console.WriteLine($"\tInserting character's profile: length: `{data.Profile.Length}`");

            Sql.Insert(
                "character_profiles",
                new Dictionary<string, object>
                {
                    { Tables.ProfilesId.Value, data.Id },
                    { Tables.ProfilesProfile.Value, data.Profile },
                    { Tables.ProfilesEnglishProfile.Value, data.EnProfile }
                },
                ["id"]
            );

            Console.WriteLine($"\tInserting character's armor...");

            if (data.DamageDealt != null)
                Sql.Insert(
                    "character_terrain_damage_dealt",
                    new Dictionary<string, object>
                    {
                        { Tables.TerrainDamageDealtId.Value, data.Id },
                        { Tables.TerrainDamageDealtCity.Value, data.DamageDealt.City },
                        { Tables.TerrainDamageDealtDesert.Value, data.DamageDealt.Desert },
                        { Tables.TerrainDamageDealtIndoor.Value, data.DamageDealt.Indoor }
                    },
                    ["id"]
                );
            else Console.WriteLine($"\tCharacter Terrain Damage Dealt Install Failed");
            if (data.ShieldBlockRate != null)
                Sql.Insert(
                    "character_terrain_shield_block_rate",
                    new Dictionary<string, object>
                    {
                        { Tables.TerrainShieldBlockRateId.Value, data.Id },
                        { Tables.TerrainShieldBlockRateCity.Value, data.ShieldBlockRate.City },
                        { Tables.TerrainShieldBlockRateDesert.Value, data.ShieldBlockRate.Desert },
                        { Tables.TerrainShieldBlockRateIndoor.Value, data.ShieldBlockRate.Indoor }
                    },
                    ["id"]
                );
            else Console.WriteLine($"\tCharacter Terrain Shield Block Rate Install Failed");
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Character '{data.Id}' insert error: {e.Message}");
        }
    }

    private static CharacterDataType.TerrainValue[] GetTerrainValues(uint id)
    {
        var mainId = Tables.MainId;
        var dataTerrainDamageDealt = Sql.FindWithJoinTable(
            [Tables.Main, Tables.TerrainDamageDealt],
            new Dictionary<Database.Mysql.TableValue, object> { { mainId, id } },
            new Dictionary<Database.Mysql.TableValue, Database.Mysql.TableValue>
            {
                { Tables.TerrainDamageDealtId, mainId }
            },
            null, 1
        );
        var dataTerrainShieldBlockRate = Sql.FindWithJoinTable(
            [Tables.Main, Tables.TerrainShieldBlockRate],
            new Dictionary<Database.Mysql.TableValue, object> { { mainId, id } },
            new Dictionary<Database.Mysql.TableValue, Database.Mysql.TableValue>
            {
                { Tables.TerrainShieldBlockRateId, mainId }
            },
            null, 1
        );

        return
        [
            dataTerrainDamageDealt is { Count: > 0 }
                ? new CharacterDataType.TerrainValue
                {
                    City = DataConverter.NumberConvert<uint>(Tables.TerrainDamageDealtCity.Value,
                        dataTerrainDamageDealt.First()),
                    Desert = DataConverter.NumberConvert<uint>(Tables.TerrainDamageDealtDesert.Value,
                        dataTerrainDamageDealt.First()),
                    Indoor = DataConverter.NumberConvert<uint>(Tables.TerrainDamageDealtIndoor.Value,
                        dataTerrainDamageDealt.First())
                }
                : null,
            dataTerrainShieldBlockRate is { Count: > 0 }
                ? new CharacterDataType.TerrainValue
                {
                    City = DataConverter.NumberConvert<uint>(Tables.TerrainShieldBlockRateCity.Value,
                        dataTerrainShieldBlockRate.First()),
                    Desert = DataConverter.NumberConvert<uint>(Tables.TerrainShieldBlockRateDesert.Value,
                        dataTerrainShieldBlockRate.First()),
                    Indoor = DataConverter.NumberConvert<uint>(Tables.TerrainShieldBlockRateIndoor.Value,
                        dataTerrainShieldBlockRate.First())
                }
                : null
        ];
    }

    protected static CharacterDataType GetCharacterData(uint id)
    {
        var mainId = Tables.MainId;
        var data = Sql.FindWithJoinTable(
            [Tables.Main, Tables.Names, Tables.Profiles],
            new Dictionary<Database.Mysql.TableValue, object> { { mainId, id } },
            new Dictionary<Database.Mysql.TableValue, Database.Mysql.TableValue>
            {
                { Tables.NamesId, mainId },
                { Tables.ProfilesId, mainId }
            },
            null,
            1
        );

        if (data is not { Count: > 0 }) throw new DatabaseNotFoundData($"Character data not found: {id}");

        var characterData = data.First();
        var characterTerrain = GetTerrainValues(id);

        return new CharacterDataType(id)
        {
            Name = DataConverter.StringConvert(Tables.NamesName.Value, characterData),
            EnName = DataConverter.StringConvert(Tables.NamesEnglishName.Value, characterData),
            Profile = DataConverter.StringConvert(Tables.ProfilesProfile.Value, characterData),
            EnProfile = DataConverter.StringConvert(Tables.ProfilesEnglishProfile.Value, characterData),
            BaseStar = DataConverter.NumberConvert<uint>(Tables.MainBaseStar.Value, characterData),
            AttackType = DataConverter.StringConvert(Tables.MainAttackType.Value, characterData),
            ArmorType = DataConverter.StringConvert(Tables.MainArmorType.Value, characterData),
            Position = DataConverter.StringConvert(Tables.MainPosition.Value, characterData),
            WeaponType = DataConverter.StringConvert(Tables.MainWeaponType.Value, characterData),
            Role = DataConverter.StringConvert(Tables.MainRole.Value, characterData),
            SquadType = DataConverter.StringConvert(Tables.MainSquadType.Value, characterData),
            School = DataConverter.StringConvert(Tables.MainSchool.Value, characterData),

            DamageDealt = characterTerrain[0],
            ShieldBlockRate = characterTerrain[1]
        };
    }

    private static Dictionary<string, object> GetCharacterBySql(uint id)
    {
        return Sql.Find(Tables.Main, new Dictionary<string, object> { { Tables.MainId.Value, id } }, 1).First();
    }

    protected static uint GetCharacterBaseStar(uint id)
    {
        var data = GetCharacterBySql(id);
        return data == null ? 0 : DataConverter.NumberConvert<uint>(Tables.MainBaseStar.Value, data);
    }

    protected static List<uint> GetCharacterIdsBySchool(uint school)
    {
        var data = Sql.Find(Tables.Main, new Dictionary<string, object> { { Tables.MainSchool.Value, school } }, 1);
        return data is not { Count: > 0 }
            ? []
            : data.Select(d => DataConverter.NumberConvert<uint>(Tables.MainId.Value, d)).ToList();
    }

    protected static List<uint> GetCharacterIdsBySchool(string school)
    {
        return GetCharacterIdsBySchool(IDataEnumIndex.School(school));
    }

    protected static uint GetRandomCharacter(uint baseStar)
    {
        var data = Sql.Find(Tables.Main, new Dictionary<string, object> { { Tables.MainBaseStar.Value, baseStar } }, 1,
            rand: true);
        return data is not { Count: > 0 }
            ? throw new DatabaseNotFoundData("Random Character error")
            : DataConverter.NumberConvert<uint>(Tables.MainId.Value, data.First());
    }
}