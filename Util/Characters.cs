using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using dcbot.Database.SqlTable.Mysql;
using Newtonsoft.Json;

namespace dcbot.Util;

public class Characters : ICharacterData
{
    private static readonly HttpClient Client = new();
    private const string UrlJp = "https://api.ennead.cc/buruaka/character?region=japan";
    private const string UrlEn = "https://api.ennead.cc/buruaka/character/";

    public static async Task InitByWebJson()
    {
        try
        {
            Console.WriteLine("Getting Characters Japanese Data...");
            var resJp = await Client.GetAsync(UrlJp);

            resJp.EnsureSuccessStatusCode();
            var contentJp = await resJp.Content.ReadAsStringAsync();
            var dataJp = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(contentJp);
            Console.WriteLine("Getting Characters English Data...");
            var resEn = await Client.GetAsync(UrlEn);

            resEn.EnsureSuccessStatusCode();

            var contentEn = await resEn.Content.ReadAsStringAsync();
            var dataEn = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(contentEn);

            var insertedEn =
                dataEn.Where(e => e.ContainsKey("id"))
                    .ToDictionary(
                        e => uint.TryParse((e.TryGetValue("id", out var d) ? d.ToString() : "") ?? string.Empty,
                            out var a)
                            ? a
                            : 0,
                        e => new Dictionary<string, string>
                        {
                            { "name", e.GetValueOrDefault("name", null).ToString() },
                            { "profile", e.GetValueOrDefault("profile", null).ToString() }
                        }
                    );


            Console.WriteLine("Getting Character...");
            foreach (var c in dataJp)
                if (c.TryGetValue("id", out var temp) && uint.TryParse(temp.ToString(), out var id))
                {
                    Console.WriteLine($"\tGet Character: {id}");

                    var ccc = new ICharacterData.CharacterDataType(id)
                    {
                        Name = c.GetValueOrDefault("name", "").ToString(),
                        Profile = c.GetValueOrDefault("profile", "").ToString(),
                        BaseStar = uint.TryParse(c.GetValueOrDefault("baseStar", "").ToString(), out var baseStar)
                            ? baseStar
                            : 0,
                        Position = c.GetValueOrDefault("position", "").ToString(),
                        Role = c.GetValueOrDefault("role", "").ToString(),
                        ArmorType = c.GetValueOrDefault("armorType", "").ToString(),
                        AttackType = c.GetValueOrDefault("bulletType", "").ToString(),
                        WeaponType = c.GetValueOrDefault("weaponType", "").ToString(),
                        SquadType = c.GetValueOrDefault("squadType", "").ToString(),
                        School = c.GetValueOrDefault("school", "").ToString()
                    };

                    if (insertedEn.TryGetValue(id, out var inserted))
                    {
                        ccc.EnName = inserted.GetValueOrDefault("name", null);
                        ccc.EnProfile = inserted.GetValueOrDefault("profile", null);
                    }


                    if (
                        c.TryGetValue("terrain", out var t) &&
                        t is Newtonsoft.Json.Linq.JObject terrain
                    )
                    {
                        var urban = GetData("urban", terrain);
                        var outdoor = GetData("outdoor", terrain);
                        var indoor = GetData("indoor", terrain);
                        ccc.DamageDealt = new ICharacterData.CharacterDataType.TerrainValue()
                        {
                            City = urban[0],
                            Desert = outdoor[0],
                            Indoor = indoor[0]
                        };
                        ccc.ShieldBlockRate = new ICharacterData.CharacterDataType.TerrainValue()
                        {
                            City = urban[1],
                            Desert = outdoor[1],
                            Indoor = indoor[1]
                        };

                        List<uint> GetData(string table, Newtonsoft.Json.Linq.JObject data)
                        {
                            return data.TryGetValue(table, out var value) &&
                                   value is Newtonsoft.Json.Linq.JObject jValue
                                ?
                                [
                                    jValue.TryGetValue("DamageDealt", out var damageDealt)
                                        ? uint.TryParse(damageDealt.ToString(), out var a) ? a : 0
                                        : 0,
                                    jValue.TryGetValue("ShieldBlockRate", out var shieldBlockRate)
                                        ? uint.TryParse(shieldBlockRate.ToString(), out var b) ? b : 0
                                        : 0
                                ]
                                : [0, 0];
                        }
                    }

                    ICharacterData.UpdateCharacter(ccc);
                }
                else
                {
                    Console.WriteLine("\tIsn't Character");
                }
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync($"Get Character Error: {ex.Message}");
        }
    }

    public static uint GetCharacterBaseStar(uint id)
    {
        return ICharacterData.GetCharacterBaseStar(id);
    }

    public static List<uint> GetCharacterIdsFromSchool(uint school)
    {
        return ICharacterData.GetCharacterIdsBySchool(school);
    }

    public static ICharacterData.CharacterDataType GetCharacter(uint id)
    {
        return ICharacterData.GetCharacterData(id);
    }
}