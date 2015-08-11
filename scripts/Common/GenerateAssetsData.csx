#r "System.Drawing"

#load "Common/Files.csx"
#load "Common/Kits.csx"
#load "Common/Map.csx"
#load "Common/Pco.csx"

using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using Newtonsoft.Json;
using Reality.BF2;
using Reality.BF2.Console;
using Reality.BF2.Generic;
using Reality.Utils;
using Reality.Utils.ImageFormats.DirectDrawSurface;
using Reality.Utils.ImageFormats.Targa;
using Reality.Utils.Zip;
using System.IO.Compression;
using System.IO;

ScriptResult GenerateData(string path, IEnumerable<Gpo> gpos, ConcurrentDictionary<string, Color> colors)
{

    IEnumerable<string> pcoFiles = EnumerateFiles(Path.Combine(Directories.CurrentModDirectory, "objects", "vehicles"), "*.con", SearchOption.AllDirectories, 0);

    ConcurrentDictionary<string, string> vehicleNames = new ConcurrentDictionary<string, string>();
    int current = 0;
    int total = pcoFiles.Count();
    Parallel.ForEach(pcoFiles, new ParallelOptions()
    {
        MaxDegreeOfParallelism = Build.GetParallelism(),
    }, (item) => {
        string filename = Path.GetFileNameWithoutExtension(item);

        ScriptProgress.Text = "Parsing File " + filename;
        ScriptProgress.Value = current++;
        ScriptProgress.Maximum = total;

        string tweak = Path.Combine(Path.GetDirectoryName(item), filename + ".tweak");

        if (!File.Exists(tweak))
            tweak = String.Empty;

        ParsePco(ref vehicleNames, item, tweak);
    });
    if (vehicleNames == null)
        return new ScriptResult()
        {
            Success = false,
            Header = "Error",
            Message = "Could not get vehicle names"
        };


    total = gpos.Count();
    int count = 0;

    ScriptProgress.Indeterminate = false;
    ScriptProgress.Value = 0;
    ScriptProgress.Maximum = total;

    string assetsJson = "";

    assetsJson += "[";
    foreach (Gpo gpo in gpos)
    {
        if (count != 0)
            assetsJson += ",";

        ScriptProgress.Value = count++;
        ScriptProgress.Text = String.Format("Generating JSON: {0}/{1}", ScriptProgress.Value, ScriptProgress.Maximum);

        assetsJson += "{";
		
		Color color;
		colors.TryGetValue(gpo.LayerInfo.MapName, out color);
        assetsJson += String.Format("\"MapName\": \"{0}\", \"FriendlyMapName\": \"{1}\", \"GameMode\":\"{2}\",\"Layer\": {3}, \"MapSize\": {4}, \"Team1FriendlyName\":  \"{5} \",\"Team1FriendlyNameShort\":  \"{6}\",\"Team2FriendlyName\":  \"{7} \",\"Team2FriendlyNameShort\":  \"{8} \", \"Color\": \"{9}\", \"Spawners\":",
            gpo.LayerInfo.MapName,
            gpo.LayerInfo.FriendlyMapName,
            GameModeData.GetId(gpo.LayerInfo.GameMode),
            (int)gpo.LayerInfo.Layer,
            gpo.LayerInfo.MapSize,
            gpo.Init.Team1.FriendlyName,
            gpo.Init.Team1.FriendlyNameShort,
            gpo.Init.Team2.FriendlyName,
            gpo.Init.Team2.FriendlyNameShort,
			HexConverter(color)
        );

        assetsJson += "[";
        bool first = true;
        Dictionary<string, JsonVehicle> vehicles = new Dictionary<string, JsonVehicle>();
		
		
        foreach (GpoSpawner spawner in gpo.Spawners)
        {
				
            bool validObject1 = spawner.ObjectNameTeam1 != null && vehicleNames.ContainsKey(spawner.ObjectNameTeam1);
            bool validObject2 = spawner.ObjectNameTeam2 != null && vehicleNames.ContainsKey(spawner.ObjectNameTeam2);

		
			
			string name, frienlyName;
			uint team;
			if(validObject1){
				name = spawner.ObjectNameTeam1; 
				frienlyName = vehicleNames[spawner.ObjectNameTeam1];
				team = 1;
			}else if(validObject2){
				name = spawner.ObjectNameTeam2; 
				frienlyName = vehicleNames[spawner.ObjectNameTeam2];
				team = 2;
			}else{
				continue;
			}



            JsonVehicle jVehicle = new JsonVehicle(
                name,
               	frienlyName,
                spawner.MinSpawnDelay,
                spawner.MaxSpawnDelay,
                spawner.TimeToLive,
                team,
                spawner.SpawnDelayAtStart,
                spawner.ControlPointId
            );


            if (vehicles.ContainsKey(jVehicle.ToCode()))
                vehicles[jVehicle.ToCode()].increment();
            else
                vehicles.Add(jVehicle.ToCode(), jVehicle);

        }


        first = true;
        foreach(KeyValuePair<string, JsonVehicle> entry in vehicles)
        {
            if (!first)
            {
                assetsJson += ",";
            }
            first = false;
            assetsJson += "{";
            assetsJson += entry.Value.ToString();
            assetsJson += "}";
        }
        assetsJson += "]";
        assetsJson += "}";
    }
    assetsJson += "]";


    File.WriteAllText(path, assetsJson);
	using (FileStream compressedFileStream = File.Create(path + ".gz"))
	{
		using (GZipStream compressionStream = new GZipStream(compressedFileStream,
		   CompressionMode.Compress))
		{
			compressionStream.Write(System.Text.Encoding.UTF8.GetBytes(assetsJson), 0, assetsJson.Length);

		}
	}
	

    return new ScriptResult()
    {
        Success = true,
        Header = "Assets Data Generated",
        Message = "Assets data has been generated at:\n\n" + path
    };
}

private static void ParsePco(ref ConcurrentDictionary<string, string> vehicleNames, string con, string tweak)
{
    string pco = String.Empty;
    ConUtils.GetFirstValue(con, "objecttemplate.create playercontrolobject\\s", out pco);

    if (String.IsNullOrWhiteSpace(pco))
    {
        return;
    }

    string hudname = String.Empty;
    ConUtils.GetFirstValue(tweak, "objecttemplate.vehiclehud.hudname\\s", out hudname);

    if (String.IsNullOrWhiteSpace(hudname))
    {
        return;
    }

    vehicleNames.AddOrUpdate(pco.ToLowerInvariant(), hudname, (key, value) => {
        return hudname;
    });
}




class JsonVehicle
{
    public string Name;
    public string FriendlyName;
    public uint  MinSpawnDelay;
    public uint  MaxSpawnDelay;
    public uint  TimeToLive;
    public uint  Team;
    public bool SpawnDelayAtStart;
    public uint  ControlPointId;
    public uint  Qt;


    public JsonVehicle(string name, string friendlyName, uint minSpawnDelay, uint maxSpawnDelay, uint timeToLive, uint team, bool spawnDelayAtStart, uint controlPointId)
    {
        this.Name = name;
        this.FriendlyName = friendlyName;
        this.MinSpawnDelay = minSpawnDelay;
        this.MaxSpawnDelay = maxSpawnDelay;
        this.TimeToLive = timeToLive;
        this.Team = team;
        this.SpawnDelayAtStart = spawnDelayAtStart;
        this.ControlPointId = controlPointId;
        this.Qt = 1;
    }


    public void increment(){
    	this.Qt += 1;
    }


    public string ToCode()
    {
        return Name + "_" + MinSpawnDelay + "_" + SpawnDelayAtStart;
    }

    public string ToString()
    {
        return String.Format("\"Name\": \"{0}\",\"FriendlyName\": \"{1}\", \"MinSpawnDelay\": {2}, \"MaxSpawnDelay\": {3}, \"TimeToLive\": {4}, \"Team\": {5}, \"SpawnDelayAtStart\": \"{6}\", \"ControlPointId\": {7}, \"Quantity\": {8}",
                        this.Name,
                        this.FriendlyName,
                        this.MinSpawnDelay,
                        this.MaxSpawnDelay,
                        this.TimeToLive,
                        this.Team,
                        this.SpawnDelayAtStart,
                        this.ControlPointId,
                        this.Qt
                );
    }

}