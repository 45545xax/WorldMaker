using System;
using SFB;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public partial class Map : MonoBehaviour
{
    Transform mainMenuPanelTransform = null;
    Transform setupPanelTransform = null;
    Transform worldNameTransform = null;
    Transform gradientPanelTransform = null;
    Transform contextMenuPanelTransform = null;
    Transform erosionPanelTransform = null;
    Transform riversPanelTransform = null;
    Transform noisePanelTransform = null;
    Transform inciseFlowPanelTransform = null;
    bool showTemperature = false;
    bool showGlobe = false;
    bool showBorders = false;
    bool doRuler = false;
    bool doTerrainBrush = false;
    public GameObject recentWorldButtonPrefab;

    GameObject raiseTerrainImage = null;
    GameObject lowerTerrainImage = null;
    ButtonToggle terrainToggleButton = null;
    ButtonToggle waypointToggleButton = null;
    Slider brushSizeSlider = null;
    Slider brushStrengthSlider = null;

    GameObject RaiseTerrainImage
    {
        get
        {
            if (raiseTerrainImage == null)
            {
                Canvas canvas = cam.GetComponentInChildren<Canvas>();
                if (canvas != null)
                {
                    foreach (Transform child in canvas.transform)
                    {
                        if (child.name == "Raise Terrain Image")
                        {
                            raiseTerrainImage = child.gameObject;
                        }
                    }
                }
            }
            return raiseTerrainImage;
        }
    }

    GameObject LowerTerrainImage
    {
        get
        {
            if (lowerTerrainImage == null)
            {
                Canvas canvas = cam.GetComponentInChildren<Canvas>();
                if (canvas != null)
                {
                    foreach (Transform child in canvas.transform)
                    {
                        if (child.name == "Lower Terrain Image")
                        {
                            lowerTerrainImage = child.gameObject;
                        }
                    }
                }
            }
            return lowerTerrainImage;
        }
    }

    ButtonToggle TerrainToggleButton
    {
        get
        {
            if (terrainToggleButton == null)
            {
                Canvas canvas = cam.GetComponentInChildren<Canvas>();
                if (canvas != null)
                {
                    foreach (Transform child in canvas.transform)
                    {
                        if (child.name == "Terrain Button")
                        {
                            terrainToggleButton = child.GetComponent<ButtonToggle>();
                        }
                    }
                }
            }
            return terrainToggleButton;
        }
    }

    ButtonToggle WaypointToggleButton
    {
        get
        {
            if (waypointToggleButton == null)
            {
                Canvas canvas = cam.GetComponentInChildren<Canvas>();
                if (canvas != null)
                {
                    foreach (Transform child in canvas.transform)
                    {
                        if (child.name == "Ruler Button")
                        {
                            waypointToggleButton = child.GetComponent<ButtonToggle>();
                        }
                    }
                }
            }
            return waypointToggleButton;
        }
    }

    Slider BrushSizeSlider
    {
        get
        {
            if (brushSizeSlider == null)
            {
                Canvas canvas = cam.GetComponentInChildren<Canvas>();
                if (canvas != null)
                {
                    foreach (Transform child in canvas.transform)
                    {
                        if (child.name == "Brush Size Slider")
                        {
                            brushSizeSlider = child.GetComponent<Slider>();
                        }
                    }
                }
            }
            return brushSizeSlider;
        }
    }

    Slider BrushStrengthSlider
    {
        get
        {
            if (brushStrengthSlider == null)
            {
                Canvas canvas = cam.GetComponentInChildren<Canvas>();
                if (canvas != null)
                {
                    foreach (Transform child in canvas.transform)
                    {
                        if (child.name == "Brush Strengh Slider")
                        {
                            brushStrengthSlider = child.GetComponent<Slider>();
                        }
                    }
                }
            }
            return brushStrengthSlider;
        }
    }

    public bool DoingTerrainBrush { get { return doTerrainBrush; } }

    public void SetTerrainBrushSize(float value)
    {
        if (doTerrainBrush)
        {
            if (terrainBrush != null)
            {
                TerrainBrush terrainBrushScript = terrainBrush.GetComponent<TerrainBrush>();
                terrainBrushScript.radius = BrushSizeSlider.value;
            }
        }
    }

    public bool ChangeTerrainBrushSize(float delta)
    {
        if (doTerrainBrush)
        {
            BrushSizeSlider.value += delta * 10;
            if (terrainBrush != null)
            {
                TerrainBrush terrainBrushScript = terrainBrush.GetComponent<TerrainBrush>();
                terrainBrushScript.radius = BrushSizeSlider.value;
            }
            return true;
        }
        return false;
    }

    public void SetTerrainBrushStrength(float falue)
    {

    }

    public bool ChangeTerrainBrushStrength(float delta)
    {
        if (doTerrainBrush)
        {
            BrushStrengthSlider.value += delta / 2;
            if (terrainBrush != null)
            {
                TerrainBrush terrainBrushScript = terrainBrush.GetComponent<TerrainBrush>();
                terrainBrushScript.strength = BrushStrengthSlider.value;
            }
            return true;
        }
        return false;
    }

    #region MapData
    public string UISeed
    {
        get
        {
            return mapSettings.Seed.ToString();
        }

        set
        {
            int iValue = 0;
            if (System.Int32.TryParse(value, out iValue))
            {
                mapSettings.Seed = iValue;
                GenerateSeeds();

                UndoErosion();
                //GenerateHeightMap();
                UpdateSurfaceMaterialProperties();
            }
            MapData.instance.Save();
        }
    }

    public string UIRadius
    {
        get
        {
            return mapSettings.RadiusInKm.ToString();
        }

        set
        {
            double dValue = 0;
            if (System.Double.TryParse(value, out dValue))
            {
                mapSettings.RadiusInKm = dValue;
            }
            MapData.instance.Save();
        }
    }

    public string UIHeightMap
    {
        get
        {
            return mapSettings.HeightMapPath;
        }

        set
        {
            mapSettings.HeightMapPath = value;
            if (mapSettings.UseImages)
            {
                UpdateSurfaceMaterialHeightMap();
            }
            MapData.instance.Save();
        }
    }

    public string UIMainTexture
    {
        get
        {
            return mapSettings.MainTexturePath;
        }

        set
        {
            mapSettings.MainTexturePath = value;
            if (mapSettings.UseImages)
            {
                UpdateSurfaceMaterialMainMap();
            }
            MapData.instance.Save();
        }
    }

    public string UILandMask
    {
        get
        {
            return mapSettings.LandMaskPath;
        }

        set
        {
            mapSettings.LandMaskPath = value;
            if (mapSettings.UseImages)
            {
                UpdateSurfaceMaterialLandMask();
            }
            MapData.instance.Save();
        }
    }

    [SerializeField]
    public Color DesertColor
    {
        get
        {
            return textureSettings.desertColor;
        }

        set
        {
            textureSettings.desertColor = value;
            UpdateSurfaceMaterialProperties();
            MapData.instance.Save();
        }
    }

    public string DesertRate1
    {
        get
        {
            return textureSettings.desertThreshold1.ToString();
        }

        set
        {
            double dValue = 0;
            if (System.Double.TryParse(value, out dValue))
            {
                textureSettings.desertThreshold1 = (float)dValue;
                UpdateSurfaceMaterialProperties();
                MapData.instance.Save();
            }
        }
    }

    public string DesertRate2
    {
        get
        {
            return textureSettings.desertThreshold2.ToString();
        }

        set
        {
            double dValue = 0;
            if (System.Double.TryParse(value, out dValue))
            {
                textureSettings.desertThreshold2 = (float)dValue;
                UpdateSurfaceMaterialProperties();
                MapData.instance.Save();
            }
        }
    }

    [SerializeField]
    public Color IceColor
    {
        get
        {
            return textureSettings.iceColor;
        }

        set
        {
            textureSettings.iceColor = value;
            UpdateSurfaceMaterialProperties();
            MapData.instance.Save();
        }
    }

    public string IceRate1
    {
        get
        {
            return textureSettings.iceTemperatureThreshold1.ToString();
        }

        set
        {
            double dValue = 0;
            if (System.Double.TryParse(value, out dValue))
            {
                textureSettings.iceTemperatureThreshold1 = (float)dValue;
                UpdateSurfaceMaterialProperties();
                MapData.instance.Save();
            }
        }
    }

    public string IceRate2
    {
        get
        {
            return textureSettings.iceTemperatureThreshold2.ToString();
        }

        set
        {
            double dValue = 0;
            if (System.Double.TryParse(value, out dValue))
            {
                textureSettings.iceTemperatureThreshold2 = (float)dValue;
                UpdateSurfaceMaterialProperties();
                MapData.instance.Save();
            }
        }
    }

    public string TextureWidth
    {
        get
        {
            return textureSettings.textureWidth.ToString();
        }
    }

    public void SetTextureWidth(string value)
    {
        if (value != null && value.Length > 0 && !updatingFieldCyclically)
        {
            int width = 1;
            try
            {
                width = Int32.Parse(value);
            }
            catch
            {
            }
            if (width > SystemInfo.maxTextureSize)
            {
                width = SystemInfo.maxTextureSize;
                UpdateUIInputField(setupPanelTransform, "Texture Width Text Box", width.ToString());
            }
            textureSettings.textureWidth = width;
            MapData.instance.Save();
        }
    }

    public string TextureHeight
    {
        get
        {
            return textureSettings.textureHeight.ToString();
        }
    }

    bool cyclicalNoiseTypeUpdate = false;
    public void SetNormalNoiseType(bool normalNoise)
    {
        textureSettings.Ridged = !normalNoise;
        if (!cyclicalNoiseTypeUpdate)
        {
            cyclicalNoiseTypeUpdate = true;
            UpdateUIToggle(noisePanelTransform, "Toggle Ridged Noise", textureSettings.Ridged);
            cyclicalNoiseTypeUpdate = false;
        }
        if (!firstUpdate)
        {
            UndoErosion();
            //GenerateHeightMap();
            UpdateSurfaceMaterialProperties();
            MapData.instance.Save();
        }
    }

    public void SetRidgedNoiseType(bool ridged)
    {
        textureSettings.Ridged = ridged;
        if (!cyclicalNoiseTypeUpdate)
        {
            cyclicalNoiseTypeUpdate = true;
            UpdateUIToggle(noisePanelTransform, "Toggle Regular Noise", !textureSettings.Ridged);
            cyclicalNoiseTypeUpdate = false;
        }
        if (!firstUpdate)
        {
            UndoErosion();
            //GenerateHeightMap();
            UpdateSurfaceMaterialProperties();
            MapData.instance.Save();
        }
    }

    public void SetFBMNoiseType(bool fbmNoise)
    {
        textureSettings.DomainWarping = !fbmNoise;
        if (!cyclicalNoiseTypeUpdate)
        {
            cyclicalNoiseTypeUpdate = true;
            UpdateUIToggle(noisePanelTransform, "Toggle Domain Warping Noise", textureSettings.DomainWarping);
            cyclicalNoiseTypeUpdate = false;
        }
        if (!firstUpdate)
        {
            UndoErosion();
            //GenerateHeightMap();
            UpdateSurfaceMaterialProperties();
            MapData.instance.Save();
        }
    }

    public void SetDomainWarpingNoiseType(bool domainWarping)
    {
        textureSettings.DomainWarping = domainWarping;
        if (!cyclicalNoiseTypeUpdate)
        {
            cyclicalNoiseTypeUpdate = true;
            UpdateUIToggle(noisePanelTransform, "Toggle FBM Noise", !textureSettings.DomainWarping);
            cyclicalNoiseTypeUpdate = false;
        }
        if (!firstUpdate)
        {
            UndoErosion();
            //GenerateHeightMap();
            UpdateSurfaceMaterialProperties();
            MapData.instance.Save();
        }
    }
    public void SetTextureHeight(string value)
    {
        if (value != null && value.Length > 0 && !updatingFieldCyclically)
        {
            int height = 1;
            try
            {
                height = Int32.Parse(value);
            }
            catch
            {
            }
            if (height > SystemInfo.maxTextureSize)
            {
                height = SystemInfo.maxTextureSize;
                UpdateUIInputField(setupPanelTransform, "Texture Height Text Box", height.ToString());
            }
            textureSettings.textureHeight = height;
            MapData.instance.Save();
        }
    }

    public void SetNoiseLayer1()
    {
        textureSettings.SelectedLayer = 1;
        MapData.instance.textureSettings = textureSettings;
        UpdateNoiseLayerFields();
    }

    public void SetNoiseLayer2()
    {
        textureSettings.SelectedLayer = 2;
        MapData.instance.textureSettings = textureSettings;
        UpdateNoiseLayerFields();
    }

    public void NewMapDetail(float value)
    {
        textureSettings.Detail = value;
        if (!firstUpdate)
        {
            MapData.instance.textureSettings = textureSettings;
            minHeights.Clear();
            maxHeights.Clear();

            UndoErosion();
            //GenerateHeightMap();
            UpdateSurfaceMaterialProperties();
            MapData.instance.Save();
        }
    }

    public void NewMapScaling(float value)
    {
        textureSettings.Scale = value;
        if (!firstUpdate)
        {
            MapData.instance.textureSettings = textureSettings;
            UndoErosion();
            //GenerateHeightMap();
            UpdateSurfaceMaterialProperties();
            MapData.instance.Save();
        }
    }

    public void NewSmoothness(float value)
    {
        textureSettings.Persistence = value;
        if (!firstUpdate)
        {
            MapData.instance.textureSettings = textureSettings;
            UndoErosion();
            //GenerateHeightMap();
            UpdateSurfaceMaterialProperties();
            MapData.instance.Save();
        }
    }

    public void NewLandMasses(float value)
    {
        textureSettings.Multiplier = value;
        if (!firstUpdate)
        {
            MapData.instance.textureSettings = textureSettings;
            UndoErosion();
            //GenerateHeightMap();
            UpdateSurfaceMaterialProperties();
            MapData.instance.Save();
        }
    }

    public void NewLayerStrength(float value)
    {
        textureSettings.LayerStrength = value;
        if (!firstUpdate)
        {
            MapData.instance.textureSettings = textureSettings;
            UndoErosion();
            //GenerateHeightMap();
            UpdateSurfaceMaterialProperties();
            MapData.instance.Save();
        }
    }

    public void NewHeightExponent(float value)
    {
        textureSettings.HeightExponent = value;
        if (!firstUpdate)
        {
            MapData.instance.textureSettings = textureSettings;
            UndoErosion();
            //GenerateHeightMap();
            UpdateSurfaceMaterialProperties();
            MapData.instance.Save();
        }
    }

    public void NewWaterLevel(float value)
    {
        textureSettings.waterLevel = value;
        if (!firstUpdate)
        {
            MapData.instance.textureSettings = textureSettings;
            //GenerateHeightMap();
            UpdateSurfaceMaterialProperties(false);
            MapData.instance.Save();
        }
    }

    public void NewHeightLimit(float value)
    {
        float value1 = GetUISlider(setupPanelTransform, "Height Limits Slider 1");
        float value2 = GetUISlider(setupPanelTransform, "Height Limits Slider 2");

        if (value1 <= value2)
        {
            textureSettings.minHeight = value1;
            textureSettings.maxHeight = value2;
        }
        else
        {
            textureSettings.minHeight = value2;
            textureSettings.maxHeight = value1;
        }

        if (!firstUpdate)
        {
            MapData.instance.LowestHeight = textureSettings.minHeight;
            MapData.instance.HighestHeight = textureSettings.maxHeight;
            MapData.instance.textureSettings = textureSettings;

            //GenerateHeightMap();
            UpdateSurfaceMaterialProperties(false);

            MapData.instance.Save();
        }
    }

    public void SliderDeselect()
    {
        //if (!firstUpdate)
        //{
        //    UndoErosion();
        //    GenerateHeightMap();
        //    UpdateSurfaceMaterialProperties();
        //}
    }

    public void WaterLevelDeselect()
    {
        if (!firstUpdate)
        {
            //GenerateHeightMap();
            UpdateSurfaceMaterialProperties(false);
        }
    }
    #endregion

    #region Erosion
    public void SetErosionNoiseMerge(float value)
    {
        textureSettings.erosionNoiseMerge = value;
        if (erodedHeightMap == null || originalHeightMap == null || mergedHeightMap == null)
            return;

        HeightMap2Texture();
        UpdateSurfaceMaterialHeightMap(true);
        MapData.instance.Save();
    }

    public void SetNumErosionInterations(string value)
    {
        erosionSettings.numErosionIterations = value.ToInt();
        MapData.instance.Save();
    }

    public void SetErosionBrushRadius(float value)
    {
        erosionSettings.erosionBrushRadius = (int)value;
        MapData.instance.Save();
    }

    public void SetErosionMaxDropletLifetime(string value)
    {
        erosionSettings.maxLifetime = value.ToInt();
        MapData.instance.Save();
    }

    public void SetErosionSedimentCapacity(string value)
    {
        erosionSettings.sedimentCapacityFactor = value.ToFloat();
        MapData.instance.Save();
    }

    public void SetErosionMinSedimentCapacity(string value)
    {
        erosionSettings.minSedimentCapacity = value.ToFloat();
        MapData.instance.Save();
    }

    public void SetErosionDepositSpeed(float value)
    {
        erosionSettings.depositSpeed = value;
        MapData.instance.Save();
    }

    public void SetErosionErodeSpeed(float value)
    {
        erosionSettings.erodeSpeed = value;
        MapData.instance.Save();
    }

    public void SetErosionEvaporateSpeed(float value)
    {
        erosionSettings.evaporateSpeed = value;
        MapData.instance.Save();
    }

    public void SetErosionGravity(string value)
    {
        erosionSettings.gravity = value.ToFloat();
        MapData.instance.Save();
    }

    public void SetErosionStartSpeed(float value)
    {
        erosionSettings.startSpeed = value;
        MapData.instance.Save();
    }

    public void SetErosionWaterSpeed(float value)
    {
        erosionSettings.startWater = value;
        MapData.instance.Save();
    }

    public void SetErosionInertia(float value)
    {
        erosionSettings.inertia = value;
        MapData.instance.Save();
    }
    #endregion

    #region InciseFlow
    public void NewFlowStrength(float value)
    {
        inciseFlowSettings.strength = value;
        MapData.instance.inciseFlowSettings = inciseFlowSettings;
        if (!firstUpdate)
        {
            PerformInciseFlow(false, false, false);
            MapData.instance.Save();
        }
    }

    public void NewFlowExponent(string value)
    {
        double dValue = 0;
        if (System.Double.TryParse(value, out dValue))
        {
            inciseFlowSettings.exponent = (float)dValue;
            MapData.instance.inciseFlowSettings = inciseFlowSettings;
            if (!firstUpdate)
            {
                PerformInciseFlow(false, false, false);
                MapData.instance.Save();
            }
        }
    }

    public void NewFlowAmount(string value)
    {
        double dValue = 0;
        if (System.Double.TryParse(value, out dValue))
        {
            inciseFlowSettings.amount = (float)dValue;
            MapData.instance.inciseFlowSettings = inciseFlowSettings;
            if (!firstUpdate)
            {
                PerformInciseFlow(false, false, false);
                MapData.instance.Save();
            }
        }
    }

    public void NewFlowMinAmount(float value)
    {
        inciseFlowSettings.minAmount = value;
        MapData.instance.inciseFlowSettings = inciseFlowSettings;
        if (!firstUpdate)
        {
            PerformInciseFlow(false, false, false);
            MapData.instance.Save();
        }
    }

    public void NewFlowChiselStrength(float value)
    {
        inciseFlowSettings.chiselStrength = value;
        MapData.instance.inciseFlowSettings = inciseFlowSettings;
        if (!firstUpdate)
        {
            PerformInciseFlow(false, false, false);
            MapData.instance.Save();
        }
    }

    public void NewFlowHeightInfluence(float value)
    {
        inciseFlowSettings.heightInfluence = value;
        MapData.instance.inciseFlowSettings = inciseFlowSettings;
        if (!firstUpdate)
        {
            PerformInciseFlow(false, false, false);
            MapData.instance.Save();
        }
    }

    public void NewFlowDistanceWeight(float value)
    {
        inciseFlowSettings.distanceWeight = Mathf.Pow(10, 3 * value);
        MapData.instance.inciseFlowSettings = inciseFlowSettings;
        if (!firstUpdate)
        {
            PerformInciseFlow(true, true, true);
            MapData.instance.Save();
        }
    }

    public void NewFlowUpwardsWeight(float value)
    {
        inciseFlowSettings.upwardWeight = Mathf.Pow(10, 3 * value);
        MapData.instance.inciseFlowSettings = inciseFlowSettings;
        if (!firstUpdate)
        {
            PerformInciseFlow(true, true, true);
            MapData.instance.Save();
        }
    }

    public void NewFlowPreBlur(float value)
    {
        inciseFlowSettings.preBlur = value;
        MapData.instance.inciseFlowSettings = inciseFlowSettings;
        if (!firstUpdate)
        {
            PerformInciseFlow(false, false, false);
            MapData.instance.Save();
        }
    }

    public void NewFlowPostBlur(float value)
    {
        inciseFlowSettings.postBlur = value;
        MapData.instance.inciseFlowSettings = inciseFlowSettings;
        if (!firstUpdate)
        {
            PerformInciseFlow(false, false, false);
            MapData.instance.Save();
        }
    }

    public void PlotRivers(bool plotRivers)
    {
        inciseFlowSettings.plotRivers = plotRivers;
        MapData.instance.inciseFlowSettings = inciseFlowSettings;
        if (!firstUpdate)
        {
            PerformInciseFlow(false, true, true);
            MapData.instance.Save();
        }

        if (inciseFlowSettings.plotRiversRandomly || inciseFlowSettings.plotRivers)
            planetSurfaceMaterial.SetInt("_IsFlowTexSet", 1);
        else
            planetSurfaceMaterial.SetInt("_IsFlowTexSet", 0);
    }

    public Color InciseFlowRiverColor
    {
        get
        {
            return inciseFlowSettings.riverColor;
        }

        set
        {
            inciseFlowSettings.riverColor = value;
            if (!firstUpdate)
            {
                InciseFlowPlotRivers();
                MapData.instance.Save();
            }
        }
    }

    public void NewRiverAmount1(float value)
    {
        inciseFlowSettings.riverAmount1 = value;
        MapData.instance.inciseFlowSettings = inciseFlowSettings;
        if (!firstUpdate)
        {
            InciseFlowPlotRivers();
            MapData.instance.Save();
        }
    }

    public void NewRiverAmount2(float value)
    {
        inciseFlowSettings.riverAmount2 = value;
        MapData.instance.inciseFlowSettings = inciseFlowSettings;
        if (!firstUpdate)
        {
            InciseFlowPlotRivers();
            MapData.instance.Save();
        }
    }

    public void PlotRiversRandomly(bool plotRivers)
    {
        inciseFlowSettings.plotRiversRandomly = plotRivers;
        MapData.instance.inciseFlowSettings = inciseFlowSettings;
        if (!firstUpdate)
        {
            PerformInciseFlow(false, true, true);
            MapData.instance.Save();
        }

        if (inciseFlowSettings.plotRiversRandomly || inciseFlowSettings.plotRivers)
            planetSurfaceMaterial.SetInt("_IsFlowTexSet", 1);
        else
            planetSurfaceMaterial.SetInt("_IsFlowTexSet", 0);
    }

    public void NewNumberOfRivers(string value)
    {
        inciseFlowSettings.numberOfRivers = value.ToInt();
        MapData.instance.inciseFlowSettings = inciseFlowSettings;
        if (!firstUpdate)
        {
            InciseFlowPlotRivers();
            MapData.instance.Save();
        }
    }

    public void NewStartingAlpha(float value)
    {
        inciseFlowSettings.startingAlpha = value;
        MapData.instance.inciseFlowSettings = inciseFlowSettings;
        if (!firstUpdate)
        {
            InciseFlowPlotRivers();
            MapData.instance.Save();
        }
    }
    #endregion

    #region PlotRivers
    public void SetNumRivers(string rivers)
    {
        plotRiversSettings.numIterations = rivers.ToInt();
        MapData.instance.Save();
    }

    public Color RiverColor
    {
        get
        {
            return plotRiversSettings.riverColor;
        }

        set
        {
            plotRiversSettings.riverColor = value;
            MapData.instance.Save();
        }
    }

    public void SetStartingAlpha(float startingAlpha)
    {
        plotRiversSettings.startingAlpha = startingAlpha;
        MapData.instance.Save();
    }

    public void SetFlowHeightDelta(string flowHeightDelta)
    {
        plotRiversSettings.flowHeightDelta = flowHeightDelta.ToFloat();
        MapData.instance.Save();
    }

    public void SetRiverBrushSize(float brushSize)
    {
        plotRiversSettings.brushSize = brushSize;
        MapData.instance.Save();
    }

    public void SetRiverBrushExponent(float brushExponent)
    {
        plotRiversSettings.brushExponent = brushExponent;
        MapData.instance.Save();
    }
    #endregion

    public void DeselectTextureSize()
    {
        Canvas canvas = null;
        if (cam != null)
            canvas = cam.GetComponentInChildren<Canvas>();

        if (setupPanelTransform == null)
            return;

        string strWidth  = GetUIInputField(setupPanelTransform, "Texture Width Text Box");

        int width = 0;
        try
        {
            width = Int32.Parse(strWidth);
        }
        catch
        {
        }

        if (textureSettings.textureWidth != width)
        {
            textureSettings.textureWidth = width;
            MapData.instance.Save();
        }
    }

    public bool GetUseImages()
    {
        return mapSettings.UseImages;
    }

    public void UseImages(Boolean useImages)
    {
        mapSettings.UseImages = useImages;
        MapData.instance.Save();
        if (!firstUpdate)
        {
            MapData.instance.Save();
            ReGenerateWorld(true, !useImages);
        }
    }

    public void SaveMainMap(bool doSave)
    {
        AppData.instance.SaveMainMap = doSave;
        AppData.instance.Save();
    }

    public void SaveHeightMap(bool doSave)
    {
        AppData.instance.SaveHeightMap = doSave;
        AppData.instance.Save();
    }

    public void SaveLandMask(bool doSave)
    {
        AppData.instance.SaveLandMask = doSave;
        AppData.instance.Save();
    }

    public void SaveNormalMap(bool doSave)
    {
        AppData.instance.SaveNormalMap = doSave;
        AppData.instance.Save();
    }

    public void SaveSpecularMap(bool doSave)
    {
        AppData.instance.SaveMainMap = doSave;
        AppData.instance.Save();
    }

    public void SaveTemperature(bool doSave)
    {
        AppData.instance.SaveTemperature = doSave;
        AppData.instance.Save();
    }

    public void SaveRivers(bool doSave)
    {
        AppData.instance.SaveRivers = doSave;
        AppData.instance.Save();
    }

    public void ExportAsCubemap(bool doExportAsCubemap)
    {
        AppData.instance.ExportAsCubemap = doExportAsCubemap;
        AppData.instance.Save();
    }

    public void TransparentOceans(bool doTransparentOceans)
    {
        AppData.instance.TransparentOceans = doTransparentOceans;
        AppData.instance.Save();
    }

    public void CubemapDimension(string cubemapDimension)
    {
        int dimension = cubemapDimension.ToInt();
        if (dimension > SystemInfo.maxTextureSize)
        {
            dimension = SystemInfo.maxTextureSize;
            UpdateUIInputField(contextMenuPanelTransform, "Cubemap Dimension Text Box", dimension.ToString());
        }
        AppData.instance.CubemapDimension = dimension;
        AppData.instance.Save();
    }

    public void CubemapDivisions(string cubemapDivisions)
    {
        int divisions = cubemapDivisions.ToInt();
        if (divisions > 10 || divisions < 0)
        {
            if (divisions > 10) divisions = 10;
            if (divisions < 0) divisions = 0;
            UpdateUIInputField(contextMenuPanelTransform, "Cubemap Subdivisions Text Box", divisions.ToString());
        }
        AppData.instance.CubemapDivisions = divisions;
        AppData.instance.Save();
    }

    public void KeepSeedOnRegenerate(bool keepSeed)
    {
        AppData.instance.KeepSeedOnRegenerate = keepSeed;
        AppData.instance.Save();
    }

    public void AutoRegenerateOnParameterChange(bool autoRegenerate)
    {
        AppData.instance.AutoRegenerate = autoRegenerate;
        AppData.instance.Save();
    }

    public void OnLandGradientChanged(GradientSlider gradientSlider)
    {
        textureSettings.landColorStages = gradientSlider.Stages;
        textureSettings.land1Color = gradientSlider.Colors;
        UpdateSurfaceMaterialProperties();
    }

    public void OnOnceanGradientChanged(GradientSlider gradientSlider)
    {
        textureSettings.oceanStages = gradientSlider.Stages;
        textureSettings.oceanColors = gradientSlider.Colors;
        UpdateSurfaceMaterialProperties();
    }

    private void SetupPanelVariables()
    {
        if (cam == null)
            return;

        Canvas canvas = cam.GetComponentInChildren<Canvas>();
        if (canvas == null)
            return;

        mainMenuPanelTransform = null;
        setupPanelTransform = null;
        worldNameTransform = null;
        gradientPanelTransform = null;
        erosionPanelTransform = null;
        riversPanelTransform = null;
        noisePanelTransform = null;
        inciseFlowPanelTransform = null;

        foreach (Transform canvasChildTransform in canvas.transform)
        {
            if (canvasChildTransform.name == "Main Menu Panel")
            {
                mainMenuPanelTransform = canvasChildTransform;
            }
            else if (canvasChildTransform.name == "World Menu Panel")
            {
                setupPanelTransform = canvasChildTransform;

                foreach (Transform setupPanelTransformChild in setupPanelTransform)
                {
                    if (setupPanelTransformChild.name == "Layer Properties Panel")
                    {
                        noisePanelTransform = setupPanelTransformChild;
                    }
                }
            }
            else if (canvasChildTransform.name == "Erosion Panel")
            {
                erosionPanelTransform = canvasChildTransform;
            }
            else if (canvasChildTransform.name == "Rivers Panel")
            {
                riversPanelTransform = canvasChildTransform;
            }
            else if (canvasChildTransform.name == "Incise Flow Panel")
            {
                inciseFlowPanelTransform = canvasChildTransform;
            }
            else if (canvasChildTransform.name == "World Name")
            {
                worldNameTransform = canvasChildTransform;
            }
            else if (canvasChildTransform.name == "Color Gradient Menu Panel")
            {
                gradientPanelTransform = canvasChildTransform;
            }
            else if (canvasChildTransform.name == "Context Menu")
            {
                contextMenuPanelTransform = canvasChildTransform;
            }
        }
    }

    private void UpdateMenuFields()
    {
        if (worldNameTransform != null)
        {
            GameObject worldName = worldNameTransform.gameObject;
            TMP_InputField worldNameTextMeshPro = worldName.GetComponent<TMP_InputField>();
            if (worldNameTextMeshPro != null)
                worldNameTextMeshPro.text = MapData.instance.WorldName;
        }

        if (setupPanelTransform != null)
        {
            UpdateUIInputField(setupPanelTransform, "Texture Width Text Box", TextureWidth);
            UpdateUIInputField(setupPanelTransform, "Texture Height Text Box", TextureHeight);
            // Setup the Seed Field
            UpdateUIInputField(setupPanelTransform, "Seed Text Box", mapSettings.Seed.ToString());
            // Setup the Radius Field.
            UpdateUIInputField(setupPanelTransform, "Radius Text Box", mapSettings.RadiusInKm.ToString("######0"));
            // Setup the Heightmap Field
            UpdateUIInputField(setupPanelTransform, "Heightmap Text Box", System.IO.Path.GetFileName(mapSettings.HeightMapPath));
            // Setup the Texture Field
            UpdateUIInputField(setupPanelTransform, "MainTexture Text Box", System.IO.Path.GetFileName(mapSettings.MainTexturePath));
            // Setup the Landmask Field
            UpdateUIInputField(setupPanelTransform, "LandMask Text Box", System.IO.Path.GetFileName(mapSettings.LandMaskPath));
            UpdateUISlider(setupPanelTransform, "Water Level Slider", textureSettings.waterLevel);
            UpdateUISlider(setupPanelTransform, "Height Limits Slider 1", MapData.instance.LowestHeight);
            UpdateUISlider(setupPanelTransform, "Height Limits Slider 2", MapData.instance.HighestHeight);
            UpdateUIToggle(setupPanelTransform, "Toggle Keep Seed", AppData.instance.KeepSeedOnRegenerate);
            UpdateUIToggle(setupPanelTransform, "Toggle Auto Regenerate", AppData.instance.AutoRegenerate);
            UpdateUIToggle(setupPanelTransform, "Toggle Use Images", mapSettings.UseImages);

            if (textureSettings.SelectedLayer == 1)
                SelectButton(setupPanelTransform, "Button Layer 1");
            else
                SelectButton(setupPanelTransform, "Button Layer 2");

            UpdateUIGradientSlider(gradientPanelTransform, textureSettings.landColorStages, textureSettings.land1Color, textureSettings.oceanStages, textureSettings.oceanColors);

            UpdateUIInputField(gradientPanelTransform, "Desert Range 1 Text Box", textureSettings.desertThreshold1.ToString());
            UpdateUIInputField(gradientPanelTransform, "Desert Range 2 Text Box", textureSettings.desertThreshold2.ToString());
            UpdateUIInputField(gradientPanelTransform, "Ice Range 1 Text Box", textureSettings.iceTemperatureThreshold1.ToString());
            UpdateUIInputField(gradientPanelTransform, "Ice Range 2 Text Box", textureSettings.iceTemperatureThreshold2.ToString());
        }

        UpdateNoiseLayerFields();

        if (gradientPanelTransform != null)
        {
            UpdateUIColorPanel(gradientPanelTransform, "Desert Color Panel", textureSettings.desertColor);
            UpdateUIColorPanel(gradientPanelTransform, "Ice Color Panel", textureSettings.iceColor);
        }

        if (contextMenuPanelTransform != null)
        {
            UpdateUIToggle(contextMenuPanelTransform, "Toggle Main Map", AppData.instance.SaveMainMap);
            UpdateUIToggle(contextMenuPanelTransform, "Toggle Height Map", AppData.instance.SaveHeightMap);
            UpdateUIToggle(contextMenuPanelTransform, "Toggle Land Mask", AppData.instance.SaveLandMask);
            UpdateUIToggle(contextMenuPanelTransform, "Toggle Normal Map", AppData.instance.SaveNormalMap);
            UpdateUIToggle(contextMenuPanelTransform, "Toggle Specular Map", AppData.instance.SaveSpecularMap);
            UpdateUIToggle(contextMenuPanelTransform, "Toggle Temperature", AppData.instance.SaveTemperature);
            UpdateUIToggle(contextMenuPanelTransform, "Toggle Rivers", AppData.instance.SaveRivers);
            UpdateUIToggle(contextMenuPanelTransform, "Toggle Export as Cubemap", AppData.instance.ExportAsCubemap);
            UpdateUIToggle(contextMenuPanelTransform, "Toggle Transparent Oceans", AppData.instance.TransparentOceans);
            UpdateUIInputField(contextMenuPanelTransform, "Cubemap Dimension Text Box", AppData.instance.CubemapDimension.ToString());
            UpdateUIInputField(contextMenuPanelTransform, "Cubemap Subdivisions Text Box", AppData.instance.CubemapDivisions.ToString());
        }

        if (erosionPanelTransform != null)
        {
            UpdateUISlider(erosionPanelTransform, "Noise Merge Slider", textureSettings.erosionNoiseMerge);
            UpdateUIInputField(erosionPanelTransform, "Iterations Text Box", erosionSettings.numErosionIterations.ToString());
            UpdateUISlider(erosionPanelTransform, "Brush Radius Slider", erosionSettings.erosionBrushRadius);
            UpdateUIInputField(erosionPanelTransform, "Lifetime Text Box", erosionSettings.maxLifetime.ToString());
            UpdateUIInputField(erosionPanelTransform, "Sediment Capacity Text Box", erosionSettings.sedimentCapacityFactor.ToString());
            UpdateUIInputField(erosionPanelTransform, "Min Sediment Capacity Text Box", erosionSettings.minSedimentCapacity.ToString());
            UpdateUISlider(erosionPanelTransform, "Deposit Speed Slider", erosionSettings.depositSpeed);
            UpdateUISlider(erosionPanelTransform, "Erode Speed Slider", erosionSettings.erodeSpeed);
            UpdateUISlider(erosionPanelTransform, "Evaporate Speed Slider", erosionSettings.evaporateSpeed);
            UpdateUIInputField(erosionPanelTransform, "Gravity Text Box", erosionSettings.gravity.ToString());
            UpdateUISlider(erosionPanelTransform, "Start Speed Slider", erosionSettings.startSpeed);
            UpdateUISlider(erosionPanelTransform, "Start Water Slider", erosionSettings.startWater);
            UpdateUISlider(erosionPanelTransform, "Inertia Slider", erosionSettings.inertia);
        }

        if (riversPanelTransform != null)
        {
            UpdateUIInputField(riversPanelTransform, "Rivers Text Box", plotRiversSettings.numIterations.ToString());
            UpdateUIInputField(riversPanelTransform, "River Erosion Text Box", plotRiversSettings.flowHeightDelta.ToString());
            UpdateUISlider(riversPanelTransform, "Starting Alpha Slider", plotRiversSettings.startingAlpha);
            UpdateUISlider(riversPanelTransform, "River Brush Size Slider", plotRiversSettings.brushSize);
            UpdateUISlider(riversPanelTransform, "River Brush Exponent Slider", plotRiversSettings.brushExponent);
            UpdateUIColorPanel(riversPanelTransform, "River Color Panel", plotRiversSettings.riverColor);
        }

        if (inciseFlowPanelTransform != null)
        {
            UpdateUISlider(inciseFlowPanelTransform, "Flow Strength Slider", inciseFlowSettings.strength);
            UpdateUIInputField(inciseFlowPanelTransform, "Flow Exponent Text Box", inciseFlowSettings.exponent.ToString());
            UpdateUIInputField(inciseFlowPanelTransform, "Flow Amount Text Box", inciseFlowSettings.amount.ToString());
            UpdateUISlider(inciseFlowPanelTransform, "Min Amount Slider", inciseFlowSettings.minAmount);
            UpdateUISlider(inciseFlowPanelTransform, "Flow Curve Strength Slider", inciseFlowSettings.chiselStrength);
            UpdateUISlider(inciseFlowPanelTransform, "Flow Height Influence Slider", inciseFlowSettings.heightInfluence);
            UpdateUISlider(inciseFlowPanelTransform, "Flow Distance Weight Slider", Mathf.Log10(inciseFlowSettings.distanceWeight) / 3);
            UpdateUISlider(inciseFlowPanelTransform, "Flow Uphill Weight Slider", Mathf.Log10(inciseFlowSettings.upwardWeight) / 3);
            UpdateUISlider(inciseFlowPanelTransform, "Flow Pre Blur Slider", inciseFlowSettings.preBlur);
            UpdateUISlider(inciseFlowPanelTransform, "Flow Post Blur Slider", inciseFlowSettings.postBlur);

            UpdateUIToggle(inciseFlowPanelTransform, "Toggle Plot Rivers", inciseFlowSettings.plotRivers);
            UpdateUIColorPanel(inciseFlowPanelTransform, "River Color Panel", inciseFlowSettings.riverColor);
            UpdateUISlider(inciseFlowPanelTransform, "River Amount 1 Slider", inciseFlowSettings.riverAmount1);
            UpdateUISlider(inciseFlowPanelTransform, "River Amount 2 Slider", inciseFlowSettings.riverAmount2);

            UpdateUIToggle(inciseFlowPanelTransform, "Toggle Plot Rivers Randomly", inciseFlowSettings.plotRiversRandomly);
            UpdateUIInputField(inciseFlowPanelTransform, "Number of Rivers Text Box", inciseFlowSettings.numberOfRivers.ToString());
            UpdateUISlider(inciseFlowPanelTransform, "Starting Alpha Slider", inciseFlowSettings.startingAlpha);
        }
    }

    private void UpdateNoiseLayerFields()
    {
        if (noisePanelTransform != null)
        {
            UpdateUIToggle(noisePanelTransform, "Toggle Regular Noise", !textureSettings.Ridged);
            UpdateUIToggle(noisePanelTransform, "Toggle Ridged Noise", textureSettings.Ridged);
            UpdateUIToggle(noisePanelTransform, "Toggle FBM Noise", !textureSettings.DomainWarping);
            UpdateUIToggle(noisePanelTransform, "Toggle Domain Warping Noise", textureSettings.DomainWarping);
            UpdateUISlider(noisePanelTransform, "Layer Strength Slider", textureSettings.LayerStrength);
            UpdateUISlider(noisePanelTransform, "Map Detail Slider", textureSettings.Detail);
            UpdateUISlider(noisePanelTransform, "Map Scaling Slider", textureSettings.Scale);
            UpdateUISlider(noisePanelTransform, "Smoothness Slider", textureSettings.Persistence);
            UpdateUISlider(noisePanelTransform, "Landmasses Slider", textureSettings.Multiplier);
            UpdateUISlider(noisePanelTransform, "Height Exponent Slider", textureSettings.HeightExponent);
        }
    }

    private string GetUIInputField(Transform panelTransform, string textBoxName)
    {
        Transform textBoxTransform = null;
        foreach (Transform setupPanelChildTransform in panelTransform.transform)
        {
            if (setupPanelChildTransform.name == textBoxName)
            {
                textBoxTransform = setupPanelChildTransform;
                break;
            }
        }

        if (textBoxTransform == null)
            return null;

        GameObject textBox = textBoxTransform.gameObject;
        TMP_InputField inputField = textBox.GetComponent<TMP_InputField>();

        if (inputField == null)
            return null;

        return inputField.text;
    }

    private void UpdateUIInputField(Transform panelTransform, string textBoxName, string newValue)
    {
        Transform textBoxTransform = null;
        foreach (Transform setupPanelChildTransform in panelTransform.transform)
        {
            if (setupPanelChildTransform.name == textBoxName)
            {
                textBoxTransform = setupPanelChildTransform;
                break;
            }
        }

        if (textBoxTransform == null)
            return;

        GameObject textBox = textBoxTransform.gameObject;
        TMP_InputField inputField = textBox.GetComponent<TMP_InputField>();

        if (inputField == null)
            return;

        inputField.text = newValue;
    }

    private void UpdateUIColorPanel(Transform panelTransform, string panelName, Color color)
    {
        Transform colorPanelTransform = null;
        foreach (Transform setupPanelChildTransform in panelTransform.transform)
        {
            if (setupPanelChildTransform.name == panelName)
            {
                colorPanelTransform = setupPanelChildTransform;
                break;
            }
        }

        if (colorPanelTransform == null)
            return;

        GameObject colorPanel = colorPanelTransform.gameObject;
        ColorBox colorBox = colorPanel.GetComponent<ColorBox>();

        if (colorBox == null)
            return;

        colorBox.Color = color;
    }

    private void UpdateUIInputFieldDirect(Transform panelTransform, string textBoxName, string newValue)
    {
        Transform textBoxTransform = null;
        foreach (Transform setupPanelChildTransform in panelTransform.transform)
        {
            if (setupPanelChildTransform.name == textBoxName)
            {
                textBoxTransform = setupPanelChildTransform;
                break;
            }
        }

        if (textBoxTransform == null)
            return;

        foreach (Transform setupPanelChildChildTransform in textBoxTransform)
        {
            GameObject childGO = setupPanelChildChildTransform.gameObject;
            TMPro.TextMeshProUGUI textMeshProUGUI = childGO.GetComponent<TMPro.TextMeshProUGUI>();
            if (textMeshProUGUI != null)
            {
                textMeshProUGUI.text = newValue;
                break;
            }
        }
    }

    private void UpdateUISlider(Transform panelTransform, string sliderName, float sliderValue)
    {
        Transform sliderTransform = null;
        foreach (Transform setupPanelChildTransform in panelTransform.transform)
        {
            if (setupPanelChildTransform.name == sliderName)
            {
                sliderTransform = setupPanelChildTransform;
                break;
            }
        }

        if (sliderTransform == null)
            return;

        GameObject sliderGO = sliderTransform.gameObject;
        Slider slider = sliderGO.GetComponent<Slider>();

        if (slider == null)
            return;

        MapSliderField mapSliderField = sliderGO.GetComponent<MapSliderField>();
        if (mapSliderField)
        {
            mapSliderField.Init(sliderValue);
        }
        else
        {
            slider.value = sliderValue;
        }
    }

    private float GetUISlider(Transform panelTransform, string sliderName)
    {
        Transform sliderTransform = null;
        foreach (Transform setupPanelChildTransform in panelTransform.transform)
        {
            if (setupPanelChildTransform.name == sliderName)
            {
                sliderTransform = setupPanelChildTransform;
                break;
            }
        }

        if (sliderTransform == null)
            return 0;

        GameObject sliderGO = sliderTransform.gameObject;
        Slider slider = sliderGO.GetComponent<Slider>();

        if (slider == null)
            return 0;

        return slider.value;
    }

    private void UpdateUIToggle(Transform panelTransform, string toggleName, bool toggleValue)
    {
        Transform toggleTransform = null;
        foreach (Transform setupPanelChildTransform in panelTransform.transform)
        {
            if (setupPanelChildTransform.name == toggleName)
            {
                toggleTransform = setupPanelChildTransform;
                break;
            }
        }

        if (toggleTransform == null)
            return;

        GameObject toggleGO = toggleTransform.gameObject;
        Toggle toggle = toggleGO.GetComponent<Toggle>();

        if (toggle == null)
            return;

        toggle.isOn = toggleValue;
    }

    private void UpdateUIGradientSlider(Transform gradientPanelTransform, float[] landColorStages, Color32[] land1Color, float[] oceanStages, Color32[] oceanColors)
    {
        foreach (Transform childTransform in gradientPanelTransform)
        {
            if (childTransform.name == "Base Slider")
            {
                GradientSlider gradientSlider = childTransform.gameObject.GetComponent<GradientSlider>();
                for (int i = 0; i < landColorStages.Length; i++)
                {
                    if (land1Color.Length <= i)
                        continue;

                    float level = landColorStages[i];
                    Color32 color = land1Color[i];

                    gradientSlider.CreateNewGradientPoint(level, color);
                }
            }
            else if (childTransform.name == "Ocean Slider")
            {
                GradientSlider gradientSlider = childTransform.gameObject.GetComponent<GradientSlider>();
                for (int i = 0; i < oceanStages.Length; i++)
                {
                    if (oceanColors.Length <= i)
                        continue;

                    float level = oceanStages[i];
                    Color32 color = oceanColors[i];

                    gradientSlider.CreateNewGradientPoint(level, color);
                }
            }
        }
    }

    void ShowErodingTerrainPanel()
    {
        Canvas canvas = cam.GetComponentInChildren<Canvas>();
        Component childComponent = canvas.GetChildWithName("Eroding Terrain Panel");
        if (childComponent == null || !(childComponent is RectTransform))
            return;

        RectTransform rectTransform = childComponent as RectTransform;
        Vector3 newPosition = new Vector3(0, 0, rectTransform.localPosition.z);
        rectTransform.localPosition = newPosition;
    }

    void ShowPluvialErodingPanel()
    {
        Canvas canvas = cam.GetComponentInChildren<Canvas>();
        Component childComponent = canvas.GetChildWithName("Pluvial Eroding Panel");
        if (childComponent == null || !(childComponent is RectTransform))
            return;

        RectTransform rectTransform = childComponent as RectTransform;
        Vector3 newPosition = new Vector3(0, 0, rectTransform.localPosition.z);
        rectTransform.localPosition = newPosition;
    }

    void SetupPluvialErodingPanel(int numPasses)
    {
        Canvas canvas = cam.GetComponentInChildren<Canvas>();
        Transform panelComponent = canvas.GetChildTransformNamed("Pluvial Eroding Panel");
        if (panelComponent == null || !(panelComponent is RectTransform))
            return;

        Transform sliderComponent = panelComponent.GetChildTransformNamed("Erosion Step Slider");
        Slider slider = sliderComponent.GetComponent<Slider>();
        if (slider != null)
        {
            slider.maxValue = numPasses;
            slider.value = 0;
        }
    }

    void UpdatePluvialErodingPanel(int pass)
    {
        Canvas canvas = cam.GetComponentInChildren<Canvas>();
        Transform panelComponent = canvas.GetChildTransformNamed("Pluvial Eroding Panel");
        if (panelComponent == null || !(panelComponent is RectTransform))
            return;

        Transform sliderComponent = panelComponent.GetChildTransformNamed("Erosion Step Slider");
        Slider slider = sliderComponent.GetComponent<Slider>();
        if (slider != null)
        {
            slider.value = pass;
        }
    }

    public void RunInciseFlowButton()
    {
        ApplyInciseFlow();
    }

    public void StartInciseFlowPreview()
    {
        PerformInciseFlow(true, true, true);
    }

    public void StopInciseFlowPreview()
    {
        inciseFlowMap = null;
        if (!isInciseFlowApplied)
        {
            flowMap = null;
            flowTexture = null;
            flowTextureRandom = null;
            planetSurfaceMaterial.SetInt("_IsFlowTexSet", 0);
        }

        HeightMap2Texture();
        UpdateSurfaceMaterialHeightMap(erodedHeightMap != null);
    }

    void HideErodingTerrainPanel()
    {
        Canvas canvas = cam.GetComponentInChildren<Canvas>();
        Component childComponent = canvas.GetChildWithName("Eroding Terrain Panel");
        if (childComponent == null || !(childComponent is RectTransform))
            return;

        RectTransform rectTransform = childComponent as RectTransform;
        Vector3 newPosition = new Vector3(0, -1680, rectTransform.localPosition.z);
        rectTransform.localPosition = newPosition;
    }

    void HidePluvialErodingPanel()
    {
        Canvas canvas = cam.GetComponentInChildren<Canvas>();
        Component childComponent = canvas.GetChildWithName("Pluvial Eroding Panel");
        if (childComponent == null || !(childComponent is RectTransform))
            return;

        RectTransform rectTransform = childComponent as RectTransform;
        Vector3 newPosition = new Vector3(0, -1680, rectTransform.localPosition.z);
        rectTransform.localPosition = newPosition;
    }

    void SetupPlottingRiversPanel(int numPasses)
    {
        Canvas canvas = cam.GetComponentInChildren<Canvas>();
        Transform panelComponent = canvas.GetChildTransformNamed("Plotting Rivers Panel");
        if (panelComponent == null || !(panelComponent is RectTransform))
            return;

        Transform sliderComponent = panelComponent.GetChildTransformNamed("Plotting Rivers Step Slider");
        Slider slider = sliderComponent.GetComponent<Slider>();
        if (slider != null)
        {
            slider.maxValue = numPasses;
            slider.value = 0;
        }
    }

    void ShowPlottingRiversPanel()
    {
        Canvas canvas = cam.GetComponentInChildren<Canvas>();
        Component childComponent = canvas.GetChildWithName("Plotting Rivers Panel");
        if (childComponent == null || !(childComponent is RectTransform))
            return;

        RectTransform rectTransform = childComponent as RectTransform;
        Vector3 newPosition = new Vector3(0, 0, rectTransform.localPosition.z);
        rectTransform.localPosition = newPosition;
    }

    void UpdatePlottingRiversPanel(int pass)
    {
        Canvas canvas = cam.GetComponentInChildren<Canvas>();
        Transform panelComponent = canvas.GetChildTransformNamed("Plotting Rivers Panel");
        if (panelComponent == null || !(panelComponent is RectTransform))
            return;

        Transform sliderComponent = panelComponent.GetChildTransformNamed("Plotting Rivers Step Slider");
        Slider slider = sliderComponent.GetComponent<Slider>();
        if (slider != null)
        {
            slider.value = pass;
        }
    }

    void HidePlottingRiversPanel()
    {
        Canvas canvas = cam.GetComponentInChildren<Canvas>();
        Component childComponent = canvas.GetChildWithName("Plotting Rivers Panel");
        if (childComponent == null || !(childComponent is RectTransform))
            return;

        RectTransform rectTransform = childComponent as RectTransform;
        Vector3 newPosition = new Vector3(0, -1680, rectTransform.localPosition.z);
        rectTransform.localPosition = newPosition;
    }

    #region RecentWorlds
    string recentWorldsObjectTextName = "Recent Worlds Text ";
    public void UpdateRecentWorldsPanel()
    {
        if (mainMenuPanelTransform == null)
            return;

        ActivateComponent(mainMenuPanelTransform, "Recent Worlds Text", AppData.instance.RecentWorlds.Count > 0);
        List<Transform> recentWorldsTransforms = mainMenuPanelTransform.GetAllChildrenNamed(recentWorldsObjectTextName);

        for (int i = 0; i < AppData.instance.RecentWorlds.Count; i++)
        {
            if (i < recentWorldsTransforms.Count)
            {
                AssertRecentWorldsObject(recentWorldsTransforms[i], i);
            }
            else
            {
                AddRecentWorldsObject(i);
            }
        }
        if (recentWorldsTransforms.Count > AppData.instance.RecentWorlds.Count)
        {
            for (int i = AppData.instance.RecentWorlds.Count; i < recentWorldsTransforms.Count; i++)
            {
                RemoveRecentWorldsObject(recentWorldsTransforms[i], i);
            }
        }

        RectTransform rectTransform = mainMenuPanelTransform as RectTransform;
        Vector2 prevAnchoredPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 25 +
            (AppData.instance.RecentWorlds.Count > 0 ? 10 : 0) +
            AppData.instance.RecentWorlds.Count * 10);
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, prevAnchoredPosition.y);
    }

    void SelectButton(Transform panelTransform, string buttonName)
    {
        foreach (Transform transform in panelTransform)
        {
            if (transform.name == buttonName)
            {
                ButtonExclusiveToggle button = transform.GetComponent<ButtonExclusiveToggle>();
                if (button == null)
                    return;

                button.OnClick();
                return;
            }
        }
    }

    void ActivateComponent(Transform transform, string name, bool active = true)
    {
        Component component = transform.GetChildWithName(name);
        if (component == null || component.gameObject == null)
            return;

        component.gameObject.SetActive(active);
    }

    void AssertRecentWorldsObject(Transform textObjectTransform, int index)
    {
        TextMeshProUGUI textMeshProUGUI = textObjectTransform.GetComponentInChildren<TextMeshProUGUI>();
        if (textMeshProUGUI == null)
            return;

        string fileName = Path.GetFileName(AppData.instance.RecentWorlds[index]);
        fileName = fileName.Replace(".json", "");
        textMeshProUGUI.text = fileName;
    }

    void AddRecentWorldsObject(int index)
    {
        string newObjectName = recentWorldsObjectTextName + index.ToString();
        GameObject gameObject = GameObject.Instantiate(recentWorldButtonPrefab, mainMenuPanelTransform);
        gameObject.name = newObjectName;
        TextMeshProUGUI textMeshProUGUI = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        RectTransform rectTransform = gameObject.transform as RectTransform;

        string fileName = Path.GetFileName(AppData.instance.RecentWorlds[index]);
        fileName = fileName.Replace(".json", "");
        textMeshProUGUI.text = fileName;

        RectTransform parentRectTransform = mainMenuPanelTransform as RectTransform;

        rectTransform.anchorMin = parentRectTransform.anchorMin;
        rectTransform.anchorMax = parentRectTransform.anchorMax;
        rectTransform.localScale = new Vector3(1, 1, 1);
        rectTransform.localPosition = new Vector3(10, 0, 0);
        rectTransform.anchoredPosition = new Vector2(5, 0 - (index * 10 + 32));
        rectTransform.sizeDelta = new Vector2(parentRectTransform.sizeDelta.x - 10, 10);

        Button button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(ClickRecentWorld);
    }

    void RemoveRecentWorldsObject(Transform textObjectTeansform, int index)
    {
        GameObject.DestroyImmediate(textObjectTeansform);
    }

    public void ClickRecentWorld()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                pointerId = -1,
            };

            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (RaycastResult raycastResult in results)
            {
                if (raycastResult.gameObject.name.Contains(recentWorldsObjectTextName))
                {
                    string name = raycastResult.gameObject.name;
                    name = name.Replace(recentWorldsObjectTextName, "");
                    int recentWorldId = name.ToInt();

                    if (recentWorldId < AppData.instance.RecentWorlds.Count)
                    {
                        string fileName = AppData.instance.RecentWorlds[recentWorldId];
                        if (File.Exists(fileName) && MapData.instance.Load(fileName))
                        {
                            textureSettings = MapData.instance.textureSettings;
                            mapSettings = MapData.instance.mapSettings;
                            erosionSettings = MapData.instance.erosionSettings;
                            plotRiversSettings = MapData.instance.plotRiversSettings;

                            ResetImages();
                            UndoErosion();
                            GenerateSeeds();
                            ReGenerateWorld(true);
                            UpdateMenuFields();
                            LoadTerrainTransformations();

                            AppData.instance.AddRecentWorld(fileName);
                            UpdateRecentWorldsPanel();
                            return;
                        }
                        else
                        {
                            AppData.instance.RemoveRecentWorld(fileName);
                        }
                        UpdateRecentWorldsPanel();
                        AppData.instance.Save();
                        return;
                    }
                }
            }
        }
    }
    #endregion
}
