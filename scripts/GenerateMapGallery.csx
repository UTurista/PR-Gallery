#category "Generate Files"
#name "Generate MapGallery"
#desc "Generates all necessary files for the PR Gallery"

#r "System.Drawing"

#load "Common/Map.csx"
#load "Common/MapOverview.csx"
#load "Common/GenerateAssetsData.csx"
#load "Common/Image.csx"

using System.Collections.Concurrent;
using Reality.BF2.Generic;
using System.Drawing;
using System.Drawing.Text;

const bool SaveAsDDS = false;
static FontFamily TitleFont = null;

ScriptResult Preload()
{

    string nvcompress = Path.Combine(Directories.CurrentModDirectory, "readme", "assets", "builds", "extras", "nvcompress.exe");
    string nvdecompress = Path.Combine(Directories.CurrentModDirectory, "readme", "assets", "builds", "extras", "nvdecompress.exe");

    if (!File.Exists(nvcompress))
    {
        return new ScriptResult()
        {
            Success = false,
            Header = "Missing File",
            Message = "Cannot find " + nvcompress
        };
    }

    if (!File.Exists(nvdecompress))
    {
        return new ScriptResult()
        {
            Success = false,
            Header = "Missing File",
            Message = "Cannot find " + nvdecompress
        };
    }

    DdsImage.InitializeNVCompress(nvcompress);
    DdsImage.InitializeNVDecompress(nvdecompress);

    string titleFont = Path.Combine(Directories.CurrentModDirectory, "readme", "assets", "builds", "extras", "TitleFont.ttf");


    try
    {
        PrivateFontCollection fontCollection = new PrivateFontCollection();
        fontCollection.AddFontFile(titleFont);
        foreach (FontFamily ff in fontCollection.Families)
        {
            TitleFont = ff;
        }
    }
    catch (Exception e)
    {
        return new ScriptResult()
        {
            Success = false,
            Header = "Error",
            Message = e.ToString()
        };
    }

    if (TitleFont == null)
    {
        return new ScriptResult()
        {
            Success = false,
            Header = "Missing Font",
            Message = "Unable to load " + titleFont
        };
    }

    return null;
}




ScriptResult Main()
{
    ScriptResult preloadResult = Preload();
    if (preloadResult != null)
    {
        return preloadResult;
    }

    try
    {



        SimpleFilePromptWindowViewModel prompt = new SimpleFilePromptWindowViewModel("Select export directory...")
        {
            DescText = "Select export directory...",
            Filter = "",
            BrowseDirectory = true,
            AllowMultipleFiles = false,
            InputText = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
            DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
        };

        if (!ScriptUI.ShowDialog(prompt).Value)
        {
            return null;
        }

        ScriptProgress.Indeterminate = true;
        ScriptProgress.Show();
        ScriptProgress.Text = "Gettings GPOs";

        IEnumerable<Gpo> gpos = GetGpos();
        int count = 0;
        int total = gpos.Count();


List<String> list = new List<String>();

        Parallel.ForEach(gpos, new ParallelOptions()
        {
            MaxDegreeOfParallelism = Build.GetParallelism()
        }, (gpo) =>
        {
            ScriptProgress.Indeterminate = false;
            ScriptProgress.Value = count++;
            ScriptProgress.Maximum = total;
            ScriptProgress.Text = String.Format("Generating Overview: {0}: {1}/{2}", gpo, ScriptProgress.Value, ScriptProgress.Maximum);

            string dir = Path.Combine(prompt.SelectedPath, "img", "maps", gpo.LayerInfo.MapName);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

			lock(list){
				if(!list.Contains(gpo.LayerInfo.MapName))
					list.Add(gpo.LayerInfo.MapName);
			}
			
            MapOverview.GenerateOverview(gpo, TitleFont, Path.Combine(dir, String.Format("mapOverview_{0}_{1}.png", GameModeData.GetId(gpo.LayerInfo.GameMode), (int)gpo.LayerInfo.Layer)));
        });



        IEnumerable<string> maps = Directory.EnumerateDirectories(Directories.CurrentModLevelsDirectory)
       .AsParallel()
       .WithDegreeOfParallelism(Build.GetParallelism())
       .Select(p => Path.GetFileName(p))
       .Where(p => list.Contains(p) && File.Exists(Path.Combine(Directories.CurrentModLevelsDirectory, p, "info", "loadbackground1.png")))
       .OrderBy(p => p, new AlphanumericComparer());



        count = 0;
        total = maps.Count();

		ConcurrentDictionary<string, Color> mapColor = new ConcurrentDictionary<string, Color>();
        Parallel.ForEach(maps, new ParallelOptions()
        {
            MaxDegreeOfParallelism = Build.GetParallelism()
        }, (p) =>
        {
            ScriptProgress.Indeterminate = false;
            ScriptProgress.Value = count++;
            ScriptProgress.Maximum = total;
            ScriptProgress.Text = String.Format("Generating backgroundImage: {0}/{1}", ScriptProgress.Value, ScriptProgress.Maximum);


            Bitmap b = DdsImage.FromFile(Path.Combine(Directories.CurrentModLevelsDirectory, p, "info", "loadbackground1.png"));


			Bitmap thumbnail = new Bitmap(b, new Size(240, 135));
			
			mapColor.TryAdd(p, getDominantColor(thumbnail));
			
			string dir = Path.Combine(prompt.SelectedPath, "img", "maps", p);
            b.Save(Path.Combine(dir, "background.jpg"));
            thumbnail.Save(Path.Combine(dir, "tile.jpg"));
        });



        if (!Directory.Exists(Path.Combine(prompt.SelectedPath, "json")))
             Directory.CreateDirectory(Path.Combine(prompt.SelectedPath, "json"));
			 
        GenerateData(Path.Combine(prompt.SelectedPath, "json", "serverdata.json"), gpos, mapColor);


        return new ScriptResult()
        {
            Success = true,
            Header = "Gallery has been Generated",
            Message = "Assets data has been generated"
        };
    }
    catch (Exception e)
    {
        return new ScriptResult()
        {
            Success = false,
            Header = "Error",
            Message = e.ToString()
        };
    }
}

IEnumerable<Gpo> GetGpos()
{

    ScriptProgress.Title = "Generating Map Overviews...";
    ScriptProgress.Text = "Getting all maps and layers...";
    ScriptProgress.Indeterminate = true;
    ScriptProgress.Value = 0;
    ScriptProgress.Maximum = 0;
    ScriptProgress.Show();

    List<Gpo> gpos = Map.GetAllGpos()
      .OrderBy(x => x.LayerInfo.FriendlyMapName, new AlphanumericComparer())
      .ThenBy(x => x.LayerInfo.GameMode)
      .ThenBy(x => x.LayerInfo.Layer)
      .ToList();

    ObservableCollection<SelectionItem<Gpo>> items = new ObservableCollection<SelectionItem<Gpo>>(gpos.Select(x => new SelectionItem<Gpo>()
    {
        Data = x,
        DisplayText = x.ToString(),
        IsSelected = true
    }));

    ObservableCollection<SelectionHelper> selectionHelpers = new ObservableCollection<SelectionHelper>();

    foreach (GameMode g in Enum.GetValues(typeof(GameMode)))
    {
        if (g == GameMode.Unknown) continue;

        selectionHelpers.Add(new SelectionHelper()
        {
            Name = "Select All " + GameModeData.GetShortName(g),
            Predicate = () => {
                foreach (var item in items)
                {
                    item.IsSelected = item.Data.LayerInfo.GameMode == g;
                }
            }
        });
    }

    SelectionWindowViewModel prompt = new SelectionWindowViewModel("Select GPOs...")
    {
        DescText = "Select GPOs to generate map overviews for...",
        Items = new ObservableCollection<SelectionItem>(items.OfType<SelectionItem>()),
        AllowMultipleSelection = true,
        SelectionHelpers = selectionHelpers
    };

    if (!ScriptUI.ShowDialog(prompt).Value)
    {
        return null;
    }

    var selectedItems = items.Where(x => x.IsSelected);


    return selectedItems.Select(x => x.Data);
}


Main();