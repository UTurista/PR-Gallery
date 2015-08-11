#r "System.Drawing"

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Reality.BF2;
using Reality.BF2.Build.Tools.Mapping;
using Reality.BF2.Data.DataTypes;
using Reality.Utils.ImageFormats.DirectDrawSurface;
using Reality.Utils.ImageFormats.Targa;

public class ScaleInfo
{
  public float MapCropX { get; set; }
  public float MapCropY { get; set; }
  public float MapCropWidth { get; set; }
  public float MapCropHeight { get; set; }
  public float MapSizeScaledX { get; set; }
  public float MapSizeScaledY { get; set; }
  public float CenterX { get; set; }
  public float CenterY { get; set; }
}

public class LineData
{
  public int Width { get; set; }
  public int NumElements { get; set; }
}

public class MapOverview
{
  private const int MapOverviewWidth = 500;
  private const int MapOverviewHeight = 600;
  private const int MapOverviewHalfWidth = 250;
  private const int MapImageX = 0;
  private const int MapImageY = 40;
  private const int MapImageWidth = 500;
  private const int MapImageHeight = 500;
  private const int MapImageHalfWidth = 250;
  private const int MapVehicleOffset = 50;
  
  // TODO: Make this read from python
  private const int SkirmishTeam1Tickets = 150;
  private const int SkirmishTeam2Tickets = 150;
  private const int VehicleWarfareTeam1Tickets = 200;
  private const int VehicleWarfareTeam2Tickets = 200;
  private const int InsurgencyCaches = 5;
  
  private static Color ColorNeutral = Color.FromArgb(255, 255, 255);
  private static Color ColorTeam1 = Color.FromArgb(207, 13, 13);
  private static Color ColorTeam2 = Color.FromArgb(3, 184, 242);
  private static Color ColorNeutralAlt = Color.FromArgb(204, 255, 255, 255);
  private static Color ColorTeam1Alt = Color.FromArgb(204, 207, 13, 13);
  private static Color ColorTeam2Alt = Color.FromArgb(204, 3, 184, 242);
  
  private static FontFamily _titleFont = null;
  
   public static void GenerateOverview(Gpo gpo, FontFamily titleFont)
  {
    string savePath = Path.Combine(Directories.CurrentModLevelsDirectory, gpo.LayerInfo.MapName, "info", String.Format("mapOverview_{0}_{1}.png", GameModeData.GetId(gpo.LayerInfo.GameMode), (int)gpo.LayerInfo.Layer));
    GenerateOverview(gpo, titleFont,  savePath );
  }

  public static void GenerateOverview(Gpo gpo, FontFamily titleFont,  string savePath )
  {
    _titleFont = titleFont;
    
    using (Bitmap overview = new Bitmap(MapOverviewWidth, MapOverviewHeight, PixelFormat.Format24bppRgb)) {
      Graphics overviewGraphics = Graphics.FromImage(overview);
      overviewGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
      overviewGraphics.SmoothingMode = SmoothingMode.AntiAlias;
      overviewGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
      overviewGraphics.CompositingQuality = CompositingQuality.HighQuality;
      overviewGraphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

      // draw map
      ScaleInfo scaleInfo = CalculateScaleInfo(gpo);
      DrawMinimap(ref overviewGraphics, gpo, scaleInfo);
      DrawCombatArea(ref overviewGraphics, gpo, scaleInfo);
      DrawRoutes(ref overviewGraphics, gpo, scaleInfo);
      DrawControlPoints(ref overviewGraphics, gpo, scaleInfo);
      DrawObjectives(ref overviewGraphics, gpo, scaleInfo);

      // draw map info
      overviewGraphics.FillRectangle(new SolidBrush(Color.FromArgb(0, 0, 0)), new Rectangle(0, 0, MapImageWidth, MapImageY));
      overviewGraphics.FillRectangle(new SolidBrush(Color.FromArgb(0, 0, 0)), new Rectangle(0, MapImageHeight + MapImageY, MapImageWidth, MapOverviewHeight - MapImageHeight -  MapImageY));
      DrawHeader(ref overviewGraphics, gpo);
      DrawFooter(ref overviewGraphics, gpo);

      overviewGraphics.Dispose();

      
      
      if (SaveAsDDS) {
        DdsImage.SaveImage(savePath, overview);
      } else {
        using (Image overviewImage = Image.FromHbitmap(overview.GetHbitmap())) {
          overviewImage.Save(savePath, ImageFormat.Png);
        }
      }
    }
  }
  
  private static ScaleInfo CalculateScaleInfo(Gpo gpo)
  {
    ScaleInfo scaleInfo = new ScaleInfo();

    string originalPath = Path.Combine(Directories.CurrentModLevelsDirectory, gpo.LayerInfo.MapName, "hud", "minimap", "original.png");
    string minimapPath = Path.Combine(Directories.CurrentModLevelsDirectory, gpo.LayerInfo.MapName, "hud", "minimap", "ingamemap.dds");

    float mapSize = Convert.ToSingle(gpo.LayerInfo.MapSize * 1024);
    float mapHalfSize = mapSize - (mapSize / 2.0f);

    float minX = float.MaxValue;
    float minY = float.MinValue;
    float maxY = float.MaxValue;
    float maxX = float.MinValue;

    if (gpo.UseCombatArea) {
      foreach (var ca in gpo.CombatAreas) {
        foreach (var point in ca.Points) {
          if (point.X < minX)
            minX = point.X;
          if (point.X > maxX)
            maxX = point.X;

          if (point.Y > minY)
            minY = point.Y;
          if (point.Y < maxY)
            maxY = point.Y;
        }
      }
    } else {
      minX = -mapHalfSize;
      minY = mapHalfSize;
      maxY = -mapHalfSize;
      maxX = mapHalfSize;
    }

    if (minX < -mapHalfSize)
      minX = -mapHalfSize;
    if (maxY < -mapHalfSize)
      maxY = -mapHalfSize;
    if (maxX > mapHalfSize)
      maxX = mapHalfSize;
    if (minY > mapHalfSize)
      minY = mapHalfSize;

    float offsetX = 0.0f;
    float offsetY = 0.0f;

    float mapWidth = 1024.0f;
    float mapHeight = 1024.0f;

    if (File.Exists(originalPath)) {
      using (Image minimap = Image.FromFile(originalPath)) {
        mapWidth = minimap.Width;
        mapHeight = minimap.Height;
      }
    } else if (File.Exists(minimapPath)) {
      using (Bitmap minimap = DdsImage.FromFile(minimapPath)) {
        mapWidth = minimap.Width;
        mapHeight = minimap.Height;
      }
    }

    scaleInfo.MapCropX = (mapWidth / (mapSize / (minX + mapHalfSize)));
    scaleInfo.MapCropY = (mapWidth / (mapSize / (-minY + mapHalfSize)));
    scaleInfo.MapCropWidth = (mapWidth / (mapSize / (maxX + mapHalfSize)) - scaleInfo.MapCropX);
    scaleInfo.MapCropHeight = (mapWidth / (mapSize / (-maxY + mapHalfSize)) - scaleInfo.MapCropY);

    if (scaleInfo.MapCropWidth < scaleInfo.MapCropHeight) {
      scaleInfo.MapCropX -= (scaleInfo.MapCropHeight - scaleInfo.MapCropWidth) / 2.0f;
      scaleInfo.MapCropWidth = scaleInfo.MapCropHeight;
    } else if (scaleInfo.MapCropHeight < scaleInfo.MapCropWidth) {
      scaleInfo.MapCropY -= (scaleInfo.MapCropWidth - scaleInfo.MapCropHeight) / 2.0f;
      scaleInfo.MapCropHeight = scaleInfo.MapCropWidth;
    }

    float scaleX = ((float)MapImageWidth / (float)mapWidth) * ((float)mapWidth / (scaleInfo.MapCropWidth / (float)mapWidth));
    scaleInfo.MapSizeScaledX = mapSize / scaleX;

    float scaleY = ((float)MapImageHeight / (float)mapWidth) * ((float)mapWidth / (scaleInfo.MapCropHeight / (float)mapWidth));
    scaleInfo.MapSizeScaledY = mapSize / scaleY;

    scaleInfo.CenterX = MapImageHalfWidth - (MapImageHalfWidth + (minX / scaleInfo.MapSizeScaledX));
    scaleInfo.CenterY = (minY / scaleInfo.MapSizeScaledY);

    scaleInfo.CenterX += (MapImageWidth - (maxX / (float)scaleInfo.MapSizeScaledX + scaleInfo.CenterX)) / 2.0f + offsetX;
    scaleInfo.CenterY += (MapImageHeight - (scaleInfo.CenterY - maxY / (float)scaleInfo.MapSizeScaledY)) / 2.0f + offsetY;

    return scaleInfo;
  }

  private static void DrawMinimap(ref Graphics graphics, Gpo gpo, ScaleInfo scaleInfo)
  {
    string originalPath = Path.Combine(Directories.CurrentModLevelsDirectory, gpo.LayerInfo.MapName, "hud", "minimap", "original.png");
    string minimapPath = Path.Combine(Directories.CurrentModLevelsDirectory, gpo.LayerInfo.MapName, "hud", "minimap", "ingamemap.dds");

    if (File.Exists(originalPath)) {
      using (Image minimap = Image.FromFile(originalPath)) {
        graphics.DrawImage(minimap, new Rectangle(MapImageX, MapImageY, MapImageWidth, MapImageHeight), new Rectangle((int)scaleInfo.MapCropX, (int)scaleInfo.MapCropY, (int)scaleInfo.MapCropWidth, (int)scaleInfo.MapCropHeight), GraphicsUnit.Pixel);
      }
    } else if (File.Exists(minimapPath)) {
      using (Bitmap minimap = DdsImage.FromFile(minimapPath)) {
        graphics.DrawImage(minimap, new Rectangle(MapImageX, MapImageY, MapImageWidth, MapImageHeight), new Rectangle((int)scaleInfo.MapCropX, (int)scaleInfo.MapCropY, (int)scaleInfo.MapCropWidth, (int)scaleInfo.MapCropHeight), GraphicsUnit.Pixel);
      }
    }
  }

  private static void DrawCombatArea(ref Graphics overviewGraphics, Gpo gpo, ScaleInfo scaleInfo)
  {
    if (!gpo.UseCombatArea)
      return;

    foreach (GpoCombatArea combatArea in gpo.CombatAreas.OrderByDescending(x => x.Inverted)) {
      if (combatArea.Points.Count == 0)
        continue;

      List<PointF> points = new List<PointF>();
      foreach (Vector2 point in combatArea.Points)
        points.Add(new PointF(point.X / (float)scaleInfo.MapSizeScaledX + scaleInfo.CenterX + MapImageX, scaleInfo.CenterY - point.Y / (float)scaleInfo.MapSizeScaledY + MapImageY));

      Color caColor = Color.FromArgb(255, 117, 117, 117);

      if (combatArea.Team == 1)
        caColor = Color.FromArgb(102, 0, 0, 255);
      else if (combatArea.Team == 2)
        caColor = Color.FromArgb(102, 255, 0, 0);

      using (Bitmap bitmap = new Bitmap(MapImageX + MapImageWidth, MapImageY + MapImageHeight)) {
        using (Graphics graphics = Graphics.FromImage(bitmap)) {
          string combatAreaFile = Path.Combine(Directories.CurrentModDirectory, "readme", "assets", "builds", "extras", "CombatArea.png");
          if (File.Exists(combatAreaFile)) {
            using (Bitmap combatAreaImage = (Bitmap)Bitmap.FromFile(combatAreaFile)) {
              ColorMap[] colorMap = new ColorMap[1] {
                new ColorMap() {
                  OldColor = Color.FromArgb(255, 255, 255),
                  NewColor = caColor
                }
              };

              ImageAttributes attributes = new ImageAttributes();
              attributes.SetRemapTable(colorMap);

              graphics.DrawImage(combatAreaImage, new Rectangle(MapImageX, MapImageY, MapImageWidth, MapImageHeight), MapImageX, MapImageY, MapImageWidth, MapImageHeight, GraphicsUnit.Pixel, attributes);
              graphics.FillPolygon(new SolidBrush(Color.FromArgb(0, 255, 0)), points.ToArray());
              
              if (combatArea.Inverted) {
                Color pixelColor;
                for (int y = 0; y < bitmap.Height; y++) {
                  for (int x = 0; x < bitmap.Width; x++) {
                    pixelColor = bitmap.GetPixel(x, y);
                    if (pixelColor == Color.FromArgb(0, 255, 0) && combatAreaImage.GetPixel(x, y).A != 0) {
                      bitmap.SetPixel(x, y, caColor);
                    } else {
                      bitmap.SetPixel(x, y, Color.FromArgb(0, 0, 0, 0));
                    }
                  }
                }
              } else {
                bitmap.MakeTransparent(Color.FromArgb(0, 255, 0));
              }
            }
          } else {
            graphics.FillRectangle(new SolidBrush(caColor), new Rectangle(MapImageX, MapImageY, MapImageWidth, MapImageHeight));
            graphics.FillPolygon(new SolidBrush(Color.FromArgb(0, 255, 0)), points.ToArray());
            
            if (combatArea.Inverted) {
              Color pixelColor;
              for (int y = 0; y < bitmap.Height; y++) {
                for (int x = 0; x < bitmap.Width; x++) {
                  pixelColor = bitmap.GetPixel(x, y);
                  if (pixelColor == Color.FromArgb(0, 255, 0)) {
                    bitmap.SetPixel(x, y, caColor);
                  } else {
                    bitmap.SetPixel(x, y, Color.FromArgb(0, 0, 0, 0));
                  }
                }
              }
            } else {
              bitmap.MakeTransparent(Color.FromArgb(0, 255, 0));
            }
          }
        }

        overviewGraphics.DrawImage(bitmap, new Rectangle(0, 0, MapImageX + MapImageWidth, MapImageY + MapImageHeight), new Rectangle(0, 0, MapImageX + MapImageWidth, MapImageY + MapImageHeight), GraphicsUnit.Pixel);
      }
    }
  }

  private static void DrawRoutes(ref Graphics overviewGraphics, Gpo gpo, ScaleInfo scaleInfo)
  {
    var routes = GpoParser.ParseRoutes(gpo.ControlPoints);

    float previousX = Single.MinValue;
    float previousZ = Single.MinValue;
    uint previousTeam = 0;
    
    for (uint route = 1; route <= routes.Count; route++) {
      if (!routes.ContainsKey(route))
        continue;

      for (uint group = 0; group <= routes[route].Count; group++) {
        if (!routes[route].ContainsKey(group))
          continue;

        if (routes[route][group].Count == 0)
          continue;

        float centerX = routes[route][group].Average(x => x.Position.X);
        float centerZ = routes[route][group].Average(x => x.Position.Z);

        Color c;

        if (!routes[route][group].Any(o => o.Team != routes[route][group][0].Team)) {
          if (previousTeam == 1 && routes[route][group][0].Team == 1)
            c = ColorTeam1Alt;
          else if (previousTeam == 2 && routes[route][group][0].Team == 2)
            c = ColorTeam2Alt;
          else
            c = ColorNeutralAlt;

          previousTeam = routes[route][group][0].Team;
        } else {
          c = ColorNeutralAlt;
          previousTeam = 0;
        }

        for (int random = 0; random < routes[route][group].Count; random++) {
          if (previousX != Single.MinValue && previousZ != Single.MinValue) {
            int mapCenterX = Convert.ToInt32((centerX / scaleInfo.MapSizeScaledX) + scaleInfo.CenterX + MapImageX);
            int mapCenterZ = Convert.ToInt32(scaleInfo.CenterY - (centerZ / scaleInfo.MapSizeScaledY) + MapImageY);
            int mapX = Convert.ToInt32((routes[route][group][random].Position.X / scaleInfo.MapSizeScaledX) + scaleInfo.CenterX + MapImageX);
            int mapZ = Convert.ToInt32(scaleInfo.CenterY - (routes[route][group][random].Position.Z / scaleInfo.MapSizeScaledY) + MapImageY);

            overviewGraphics.DrawLine(new Pen(new SolidBrush(c), 2) { DashStyle = DashStyle.Dash }, new Point(mapCenterX, mapCenterZ), new Point(mapX, mapZ));
          }
        }

        if (previousX != Single.MinValue && previousZ != Single.MinValue) {
          int mapCenterX = Convert.ToInt32((previousX / scaleInfo.MapSizeScaledX) + scaleInfo.CenterX + MapImageX);
          int mapCenterZ = Convert.ToInt32(scaleInfo.CenterY - (previousZ / scaleInfo.MapSizeScaledY) + MapImageY);
          int mapX = Convert.ToInt32((centerX / scaleInfo.MapSizeScaledX) + scaleInfo.CenterX + MapImageX);
          int mapZ = Convert.ToInt32(scaleInfo.CenterY - (centerZ / scaleInfo.MapSizeScaledY) + MapImageY);

          overviewGraphics.DrawLine(new Pen(new SolidBrush(c), 2), new Point(mapCenterX, mapCenterZ), new Point(mapX, mapZ));
        }

        previousX = centerX;
        previousZ = centerZ;
      }

      previousX = Single.MinValue;
      previousZ = Single.MinValue;
    }
  }

  private static void DrawControlPoints(ref Graphics overviewGraphics, Gpo gpo, ScaleInfo scaleInfo)
  {
    foreach (var flag in gpo.ControlPoints) {
      Color color = ColorNeutralAlt;
      if (flag.Team == 1)
        color = ColorTeam1Alt;
      else if (flag.Team == 2)
        color = ColorTeam2Alt;

      string flagPath = Path.Combine(Directories.CurrentModDirectory, "menu", "HUD", "Texture", "Ingame", "flags", "icons", "minimap", "neutral", "minimap_cp.tga");
      if (flag.Team == 1)
        flagPath = Path.Combine(Directories.CurrentModDirectory, "menu", "HUD", "Texture", "Ingame", "flags", "icons", "minimap", gpo.Init.Team1.Name, "minimap_cp.tga");
      else if (flag.Team == 2)
        flagPath = Path.Combine(Directories.CurrentModDirectory, "menu", "HUD", "Texture", "Ingame", "flags", "icons", "minimap", gpo.Init.Team2.Name, "minimap_cp.tga");

      if (flag.UnableToChangeTeam) {
        flagPath = Path.Combine(Directories.CurrentModDirectory, "menu", "HUD", "Texture", "Ingame", "flags", "icons", "minimap", "neutral", "minimap_cpbase.tga");
        if (flag.Team == 1)
          flagPath = Path.Combine(Directories.CurrentModDirectory, "menu", "HUD", "Texture", "Ingame", "flags", "icons", "minimap", gpo.Init.Team1.Name, "minimap_cpbase.tga");
        else if (flag.Team == 2)
          flagPath = Path.Combine(Directories.CurrentModDirectory, "menu", "HUD", "Texture", "Ingame", "flags", "icons", "minimap", gpo.Init.Team2.Name, "minimap_cpbase.tga");
      }

      int x = Convert.ToInt32((flag.Position.X / scaleInfo.MapSizeScaledX) + scaleInfo.CenterX + MapImageX);
      int y = Convert.ToInt32(scaleInfo.CenterY - (flag.Position.Z / scaleInfo.MapSizeScaledY) + MapImageY);

      if (File.Exists(flagPath)) {
        using (Bitmap image = TargaImage.FromFile(flagPath)) {
          overviewGraphics.DrawImage(image, x - (image.Width / 2), y - (image.Width / 2));
        }
      } else {
        overviewGraphics.FillEllipse(new SolidBrush(Color.FromArgb(204, 204, 204)), x - 3, y - 3, 6, 6);
        overviewGraphics.DrawEllipse(new Pen(new SolidBrush(Color.FromArgb(0, 0, 0)), 2), x - 3, y - 3, 6, 6);
        overviewGraphics.FillRectangle(new SolidBrush(color), x + 2, y - 12, 14, 10);
        overviewGraphics.DrawRectangle(new Pen(new SolidBrush(Color.FromArgb(0, 0, 0)), 2), x + 2, y - 12, 14, 10);

        if (flag.UnableToChangeTeam) {
          overviewGraphics.DrawEllipse(new Pen(new SolidBrush(Color.FromArgb(255, 0, 0)), 2), x - 10, y - 10, 20, 20);
          overviewGraphics.DrawLine(new Pen(new SolidBrush(Color.FromArgb(255, 0, 0)), 2), new Point(x - 8, y - 8), new Point(x + 8, y + 8));
        }
      }
    }
  }

  // TODO: Support gpm_objective
  private static void DrawObjectives(ref Graphics overviewGraphics, Gpo gpo, ScaleInfo scaleInfo)
  {
    if (gpo.LayerInfo.GameMode != GameMode.Insurgency)
      return;

    string markerPath = Path.Combine(Directories.CurrentModDirectory, "readme", "assets", "builds", "extras", "overviewicons", "insurgency.png");
    if (!File.Exists(markerPath))
      return;

    List<GpoSpawner> selectedCaches = new List<GpoSpawner>();
    int divisor = 1;
    double halfSize = (gpo.LayerInfo.MapSize * 1024) / 2.0;
    double margin = (gpo.LayerInfo.MapSize * 1024) / 16.0;

    IEnumerable<GpoSpawner> caches = gpo.Spawners
      .Where(x => (!String.IsNullOrWhiteSpace(x.ObjectNameTeam1) && x.ObjectNameTeam1.Equals("ammocache", StringComparison.OrdinalIgnoreCase)) ||
            (!String.IsNullOrWhiteSpace(x.ObjectNameTeam2) && x.ObjectNameTeam2.Equals("ammocache", StringComparison.OrdinalIgnoreCase)))
      .Where(x => x.Position.X > (-halfSize + margin) && x.Position.X < (halfSize - margin) &&
            x.Position.Z > (-halfSize + margin) && x.Position.Z < (halfSize - margin))
      ;//.Shuffle();

    while (true) {
      double minDistance = (gpo.LayerInfo.MapSize * 1024) / divisor++;
      foreach (var cache in caches) {
        if (selectedCaches.Count == 0)
          selectedCaches.Add(cache);
        else {
          bool add = true;
          foreach (var selectedCache in selectedCaches.ToList()) {
            if (cache.Position.DistanceTo(selectedCache.Position) < minDistance) {
              add = false;
              break;
            }
          }

          if (add)
            selectedCaches.Add(cache);
        }

        if (selectedCaches.Count >= InsurgencyCaches)
          break;
      }

      if (selectedCaches.Count >= InsurgencyCaches)
        break;
    }

    foreach (var selectedCache in selectedCaches) {
      int x = Convert.ToInt32((selectedCache.Position.X / scaleInfo.MapSizeScaledX) + scaleInfo.CenterX + MapImageX);
      int y = Convert.ToInt32(scaleInfo.CenterY - (selectedCache.Position.Z / scaleInfo.MapSizeScaledY) + MapImageY);
      using (Bitmap image = GetBitmap(markerPath)) {
        overviewGraphics.DrawImage(image, x - (image.Width / 2), y - (image.Width / 2));
      }
    }
  }

  private static void DrawHeader(ref Graphics overviewGraphics, Gpo gpo)
  {
    string mapName = gpo.LayerInfo.FriendlyMapName;
    string layerInfo = String.Format("{0} {1} ({2} km)", GameModeData.GetFullName(gpo.LayerInfo.GameMode), LayerData.GetLongName(gpo.LayerInfo.Layer), gpo.LayerInfo.MapSize);

    double mapNameSize = MeasureString(ref overviewGraphics, mapName, _titleFont, FontStyle.Regular, 24);
    double layerInfoSize = MeasureString(ref overviewGraphics, layerInfo, _titleFont, FontStyle.Regular, 18);

    int mapNameLeft = Convert.ToInt32((MapOverviewWidth - (mapNameSize - 5 + layerInfoSize)) / 2);
    int layerInfoLeft = Convert.ToInt32(mapNameLeft + mapNameSize - 5);

    DrawString(ref overviewGraphics, mapName, new Point(mapNameLeft, 7), Color.FromArgb(51, 153, 204), _titleFont, FontStyle.Regular, StringAlignment.Near, false, 24);
    DrawString(ref overviewGraphics, layerInfo, new Point(layerInfoLeft, 12), Color.FromArgb(255, 255, 255), _titleFont, FontStyle.Regular, StringAlignment.Near, false, 18);
  }

  private static void DrawFooter(ref Graphics overviewGraphics, Gpo gpo)
  {
    DrawBackgroundFlags(ref overviewGraphics, gpo);
    DrawFactionInfo(ref overviewGraphics, gpo);

    var spawnerCategories = GpoParser.CategorizeSpawners(gpo.Spawners);

    DrawVehicles(ref overviewGraphics, 1, spawnerCategories[1], 300, 549, 193, 42, 16, 16, 12, 10);
    DrawVehicles(ref overviewGraphics, 2, spawnerCategories[2], 50, 549, 193, 42, 16, 16, 12, 10);
  }

  private static void DrawVehicles(ref Graphics overviewGraphics, int team, SortedDictionary<GpoSpawnerType, List<GpoSpawner>> vehicles, int x, int y, int maxWidth, int maxHeight, int width, int height, int xMargin, int yMargin)
  {
    var trimmedVehicles = vehicles.Where(v => v.Key != GpoSpawnerType.Other);

    int count = trimmedVehicles.Count();

    if (count == 0)
      return;

    Dictionary<int, LineData> lines = new Dictionary<int, LineData>();
    Dictionary<int, SortedDictionary<GpoSpawnerType, List<GpoSpawner>>> sortedVehicles = new Dictionary<int, SortedDictionary<GpoSpawnerType, List<GpoSpawner>>>();
    
    {
      int line = 0;
      lines.Add(line, new LineData() { Width = width, NumElements = 1 });
      int element = 0;

      while (++element < count) {
        if (lines[line].Width + xMargin + width > maxWidth) {
          line++;
          lines.Add(line, new LineData() { Width = width, NumElements = 1 });
        } else {
          lines[line].Width += xMargin + width;
          lines[line].NumElements++;
        }
      }
    }

    {
      int line = 0;
      int elements = 0;
      foreach (var vehicle in trimmedVehicles) {
        if (!sortedVehicles.ContainsKey(line))
          sortedVehicles.Add(line, new SortedDictionary<GpoSpawnerType, List<GpoSpawner>>());
        
        sortedVehicles[line].Add(vehicle.Key, vehicle.Value);

        if (++elements >= lines[line].NumElements) {
          line++;
          elements = 0;
        }
      }
    }

    int startY = Convert.ToInt32((maxHeight - ((height * lines.Count) + (yMargin * (lines.Count - 1)))) / 2.0);
    for (int i = 0; i < lines.Count; i++) {
      int xOffset = x + Convert.ToInt32((maxWidth - lines[i].Width) / 2.0);
      int yOffset = startY + i * (height + yMargin);

      foreach (var vehicle in sortedVehicles[i]) {
        if (vehicle.Value.All(v => (!String.IsNullOrWhiteSpace(v.ObjectNameTeam1) && v.ObjectNameTeam1.Contains("artillery")) ||
          (!String.IsNullOrWhiteSpace(v.ObjectNameTeam2) && v.ObjectNameTeam2.Contains("artillery")))) {
          DrawVehicle(ref overviewGraphics, team, vehicle.Key, Convert.ToInt32(vehicle.Value.First().MinSpawnDelay / 60), xOffset, y + yOffset, width, height, false);
        } else if (vehicle.Value.All(v => (!String.IsNullOrWhiteSpace(v.ObjectNameTeam1) && v.ObjectNameTeam1.Contains("mortar")) ||
          (!String.IsNullOrWhiteSpace(v.ObjectNameTeam2) && v.ObjectNameTeam2.Contains("mortar")))) {
          DrawVehicle(ref overviewGraphics, team, vehicle.Key, Convert.ToInt32(vehicle.Value.First().MinSpawnDelay / 60), xOffset, y + yOffset, width, height, false);
        } else {
          DrawVehicle(ref overviewGraphics, team, vehicle.Key, vehicle.Value.Count, xOffset, y + yOffset, width, height);
        }

        xOffset += width + xMargin;
      }
    }
  }

  private static void DrawVehicle(ref Graphics overviewGraphics, int team, GpoSpawnerType gpoSpawnerType, int count, int x, int y, int width, int height, bool isCount = true)
  {
    string path = Path.Combine(Directories.CurrentModDirectory, "readme", "assets", "builds", "extras", "overviewicons");
    switch (gpoSpawnerType) {
      case GpoSpawnerType.Logistics:
        path = Path.Combine(path, "logistics.png");
        break;
      case GpoSpawnerType.Truck:
        path = Path.Combine(path, "truck.png");
        break;
      case GpoSpawnerType.Boat:
        path = Path.Combine(path, "boat.png");
        break;
      case GpoSpawnerType.Bike:
        path = Path.Combine(path, "bike.png");
        break;
      case GpoSpawnerType.Car:
        path = Path.Combine(path, "car.png");
        break;
      case GpoSpawnerType.Atgm:
        path = Path.Combine(path, "atgm.png");
        break;
      case GpoSpawnerType.Apc:
        path = Path.Combine(path, "apc.png");
        break;
      case GpoSpawnerType.Ifv:
        path = Path.Combine(path, "ifv.png");
        break;
      case GpoSpawnerType.Tank:
        path = Path.Combine(path, "tank.png");
        break;
      case GpoSpawnerType.AntiAir:
        path = Path.Combine(path, "antiair.png");
        break;
      case GpoSpawnerType.TransportHeli:
        path = Path.Combine(path, "the.png");
        break;
      case GpoSpawnerType.AttackHeli:
        path = Path.Combine(path, "ahe.png");
        break;
      case GpoSpawnerType.Jet:
        path = Path.Combine(path, "jet.png");
        break;
      case GpoSpawnerType.Mortar:
        path = Path.Combine(path, "mortar.png");
        break;
      case GpoSpawnerType.Artillery:
        path = Path.Combine(path, "artillery.png");
        break;
      default:
        return;
    }

    Color color = ColorNeutral;
    if (team == 1)
      color = ColorTeam1;
    else if (team == 2)
      color = ColorTeam2;

    if (File.Exists(path)) {
      Bitmap flagImage = GetBitmap(path);

      ColorMap[] colorMap = new ColorMap[1] {
        new ColorMap() {
          OldColor = Color.FromArgb(255, 255, 255),
          NewColor = color
        }
      };

      ImageAttributes attributes = new ImageAttributes();
      attributes.SetRemapTable(colorMap);

      overviewGraphics.DrawImage(flagImage, new Rectangle(x, y, width, height), 0, 0, flagImage.Width, flagImage.Height, GraphicsUnit.Pixel, attributes);
      flagImage.Dispose();
    } else {
      overviewGraphics.FillRectangle(new SolidBrush(color), x, y, width, height);
      overviewGraphics.DrawRectangle(new Pen(new SolidBrush(Color.FromArgb(0, 0, 0)), 1), x, y, width, height);
    }

    Color textColor = Color.FromArgb(255, 255, 255);
    if (!isCount)
      textColor = Color.FromArgb(153, 153, 153);

    DrawString(ref overviewGraphics, count.ToString(), new Point(x + width, y + height - 8), textColor, _titleFont, FontStyle.Regular, StringAlignment.Center, true, 12);
  }

  private static void DrawBackgroundFlags(ref Graphics overviewGraphics, Gpo gpo)
  {
    try {
      using (Bitmap bitmap = new Bitmap(MapImageWidth, MapOverviewHeight - (MapImageY + MapImageHeight), PixelFormat.Format32bppArgb)) {
        using (Graphics graphics = Graphics.FromImage(bitmap)) {
          graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
          graphics.SmoothingMode = SmoothingMode.AntiAlias;
          graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
          graphics.CompositingQuality = CompositingQuality.HighQuality;
          graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

          string team1Flag = Path.Combine(Directories.CurrentModDirectory, "menu", "external-zip", "flashmenu", "images", "joingame", String.Format("hugeflag_{0}.png", gpo.Init.Team1.Name));
          string team2Flag = Path.Combine(Directories.CurrentModDirectory, "menu", "external-zip", "flashmenu", "images", "joingame", String.Format("hugeflag_{0}.png", gpo.Init.Team2.Name));

          if (File.Exists(team1Flag)) {
            Bitmap flagImage = GetBitmap(team1Flag);
            using (Bitmap newFlagImage = flagImage.Clone(new Rectangle(0, 0, flagImage.Width, flagImage.Height), PixelFormat.Format32bppArgb)) {
              Color pixelColor;
              int max = (int)((newFlagImage.Width / (float)(MapOverviewHalfWidth + 50)) * 100);
              for (int y = 0; y < newFlagImage.Height; y++) {
                for (int x = 0; x < max; x++) {
                  pixelColor = newFlagImage.GetPixel(x, y);
                  newFlagImage.SetPixel(x, y, Color.FromArgb((int)((x / (float)max) * 255), pixelColor.R, pixelColor.G, pixelColor.B));
                }
              }
              graphics.DrawImage(newFlagImage, new Rectangle(MapOverviewHalfWidth - 50, 0, MapOverviewHalfWidth + 50, MapOverviewHeight - (MapImageY + MapImageHeight)), new Rectangle(0, 0, newFlagImage.Width, newFlagImage.Height), GraphicsUnit.Pixel);
            }
            flagImage.Dispose();
          }

          if (File.Exists(team2Flag)) {
            Bitmap flagImage = GetBitmap(team2Flag);
            using (Bitmap newFlagImage = flagImage.Clone(new Rectangle(0, 0, flagImage.Width, flagImage.Height), PixelFormat.Format32bppArgb)) {
              Color pixelColor;
              int max = (int)((newFlagImage.Width / (float)(MapOverviewHalfWidth + 50)) * 100);
              for (int y = 0; y < newFlagImage.Height; y++) {
                for (int x = newFlagImage.Width - max; x < newFlagImage.Width; x++) {
                  pixelColor = newFlagImage.GetPixel(x, y);
                  newFlagImage.SetPixel(x, y, Color.FromArgb((int)(((newFlagImage.Width - x) / (float)max) * 255), pixelColor.R, pixelColor.G, pixelColor.B));
                }
              }
              graphics.DrawImage(newFlagImage, new Rectangle(0, 0, MapOverviewHalfWidth + 50, MapOverviewHeight - (MapImageY + MapImageHeight)), new Rectangle(0, 0, flagImage.Width, flagImage.Height), GraphicsUnit.Pixel);
            }
            flagImage.Dispose();
          }
        }

        ColorMatrix matrix = new ColorMatrix() {
          Matrix33 = 0.1f
        };
        ImageAttributes attributes = new ImageAttributes();
        attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

        overviewGraphics.DrawImage(bitmap, new Rectangle(0, MapImageY + MapImageHeight, MapImageWidth, MapOverviewHeight - (MapImageY + MapImageHeight)), 0, 0, MapImageWidth, MapOverviewHeight - (MapImageY + MapImageHeight), GraphicsUnit.Pixel, attributes);
      }
    } catch (Exception) {
    }
  }

  private static void DrawFactionInfo(ref Graphics overviewGraphics, Gpo gpo)
  {
    try {
      using (Bitmap bitmap = new Bitmap(MapImageWidth, MapOverviewHeight - (MapImageY + MapImageHeight), PixelFormat.Format32bppArgb)) {
        Graphics graphics = Graphics.FromImage(bitmap);
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        graphics.CompositingQuality = CompositingQuality.HighQuality;
        graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        string team1Flag = Path.Combine(Directories.CurrentModDirectory, "menu", "external-zip", "flashmenu", "images", "joingame", String.Format("flaglarge_{0}.png", gpo.Init.Team1.Name));
        string team2Flag = Path.Combine(Directories.CurrentModDirectory, "menu", "external-zip", "flashmenu", "images", "joingame", String.Format("flaglarge_{0}.png", gpo.Init.Team2.Name));

        if (File.Exists(team1Flag)) {
          try {
            Bitmap flagImage = GetBitmap(team1Flag);
            graphics.DrawImage(flagImage, new Rectangle(MapOverviewHalfWidth + 10, 8, 32, 24), new Rectangle(0, 0, flagImage.Width, flagImage.Height), GraphicsUnit.Pixel);
            flagImage.Dispose();
          } catch (Exception) {
            graphics.FillRectangle(new SolidBrush(ColorTeam1), new Rectangle(MapOverviewHalfWidth + 10, 8, 32, 24));
          }
        } else {
          graphics.FillRectangle(new SolidBrush(ColorTeam1), new Rectangle(MapOverviewHalfWidth + 10, 8, 32, 24));
        }

        if (File.Exists(team2Flag)) {
          try {
            Bitmap flagImage = GetBitmap(team2Flag);
            graphics.DrawImage(flagImage, new Rectangle(10, 8, 32, 24), new Rectangle(0, 0, flagImage.Width, flagImage.Height), GraphicsUnit.Pixel);
            flagImage.Dispose();
          } catch (Exception) {
            graphics.FillRectangle(new SolidBrush(ColorTeam2), new Rectangle(10, 8, 32, 24));
          }
        } else {
          graphics.FillRectangle(new SolidBrush(ColorTeam2), new Rectangle(10, 8, 32, 24));
        }

        graphics.DrawRectangle(new Pen(new SolidBrush(Color.FromArgb(0, 0, 0)), 2), new Rectangle(MapOverviewHalfWidth + 10, 8, 32, 24));
        graphics.DrawRectangle(new Pen(new SolidBrush(Color.FromArgb(0, 0, 0)), 2), new Rectangle(10, 8, 32, 24));

        uint team1Tickets = 0;
        uint team2Tickets = 0;

        if (gpo.Init.Tickets.ContainsKey(1))
          team1Tickets = gpo.Init.Tickets[1];
        if (gpo.Init.Tickets.ContainsKey(2))
          team2Tickets = gpo.Init.Tickets[2];

        if (gpo.LayerInfo.GameMode == GameMode.Skirmish) {
          team1Tickets = SkirmishTeam1Tickets;
          team2Tickets = SkirmishTeam2Tickets;
        } else if (gpo.LayerInfo.GameMode == GameMode.VehicleWarfare) {
          team1Tickets = VehicleWarfareTeam1Tickets;
          team2Tickets = VehicleWarfareTeam2Tickets;
        } else if (gpo.LayerInfo.GameMode == GameMode.Insurgency) {
          team1Tickets = InsurgencyCaches;
        }

        DrawString(ref graphics, team1Tickets.ToString(), new Point(MapOverviewHalfWidth + (MapVehicleOffset / 2) + 1, 32), ColorTeam1, _titleFont, FontStyle.Regular, StringAlignment.Center, true, 18);
        DrawString(ref graphics, team2Tickets.ToString(), new Point((MapVehicleOffset / 2) + 1, 32), ColorTeam2, _titleFont, FontStyle.Regular, StringAlignment.Center, true, 18);

        graphics.Dispose();

        overviewGraphics.DrawImage(bitmap, new Rectangle(0, MapImageY + MapImageHeight, MapImageWidth, MapOverviewHeight - (MapImageY + MapImageHeight)), 0, 0, MapImageWidth, MapOverviewHeight - (MapImageY + MapImageHeight), GraphicsUnit.Pixel);
      }
    } catch (Exception) {
    }
  }

  private static Bitmap GetBitmap(string filename)
  {
    byte[] format = new byte[4];
    using (FileStream fs = File.OpenRead(filename)) {
      fs.Read(format, 0, 4);
    }

    if (format.SequenceEqual(new byte[] { 0x44, 0x44, 0x53, 0x20 })) {
      return DdsImage.FromFile(filename);
    } else if (format.SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47 })) {
      return (Bitmap)Bitmap.FromFile(filename);
    } else {
      throw new Exception(filename + " is an invalid file format");
    }
  }

  private static void DrawString(ref Graphics graphics, string text, Point point, Color color, FontFamily fontFamily, FontStyle fontStyle, StringAlignment align, bool outline = false, uint size = 12)
  {
    StringFormat format = new StringFormat(StringFormat.GenericDefault) {
      Alignment = align
    };

    if (outline) {
      GraphicsPath textPath = new GraphicsPath();
      textPath.AddString(text, fontFamily, (int)fontStyle, size, new Point(point.X, point.Y), format);
      graphics.DrawPath(new Pen(new SolidBrush(Color.FromArgb(255, 0, 0, 0)), 3), textPath);
    }

    graphics.DrawString(text, new Font(fontFamily, size, fontStyle, GraphicsUnit.Pixel), new SolidBrush(color), point, format);
  }

  private static double MeasureString(ref Graphics graphics, string text, FontFamily fontFamily, FontStyle fontStyle, uint size = 12)
  {
    return graphics.MeasureString(text, new Font(fontFamily, size, fontStyle, GraphicsUnit.Pixel)).Width;
  }
}
