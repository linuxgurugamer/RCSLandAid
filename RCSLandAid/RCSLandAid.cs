using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KSP.UI.Screens;

using ClickThroughFix;
using ToolbarControl_NS;

namespace RCSLandAid
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
        void Start()
        {
            ToolbarControl.RegisterMod(RCSLandingAid.MODID, RCSLandingAid.MODNAME);
        }
    }

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class RCSLandingAid : MonoBehaviour
    {


        bool selectingTarget = false;
        //private IButton RCSla1Btn;
        private bool buttonCreated = false;
        //LineRenderer theLine = new LineRenderer();
        //public static bool forceSASup = true;
        //RCSLandingAidWindow RCSwin;
        Part lastRoot = new Part();
        //private ConfigNode RCSla;
        //ApplicationLauncherButton LAButton = null; //stock toolbar button instance
        ToolbarControl toolbarControl;

        //bool checkBlizzyToolbar = false;
        //Texture2D btnRed = new Texture2D(24, 24);
        //Texture2D btnBlue = new Texture2D(24, 24);
        //Texture2D btnRedEnable = new Texture2D(24, 24);
        //Texture2D btnBlueEnable = new Texture2D(24, 24);
        //Texture2D btnGray = new Texture2D(24, 24);
        bool showLAMenu = false;
        Rect LASettingsWin = new Rect(Screen.width - 200, 40, 160, 90);
        public static RCSLandingAidModule curVsl;
        int lastBtnState = 0;
        public static int curBtnState = 0;
        public static bool overWindow;
        public static RCSLandingAid thisModule;


        public void Start()
        {
            print("Landing Aid Ver. 3.2 start.");
            thisModule = this;
            //RenderingManager.AddToPostDrawQueue(0, LAOnDraw); //GUI window hook

            //RCSla = ConfigNode.Load(KSPUtil.ApplicationRootPath + "GameData/Diazo/RCSLandAid/RCSla.settings");

            //forceSASup = Convert.ToBoolean(RCSla.GetValue("ForceSAS"));    
            //FlightGlobals.ActiveVessel.OnFlyByWire += RCSLandAidControl;

            AddButtons();

        }

        internal const string MODID = "RCSLandAid_NS";
        internal const string MODNAME = "Horizontal Landing Aid";

        void AddButtons()
        {

            if (!buttonCreated)
            {
                toolbarControl = gameObject.AddComponent<ToolbarControl>();
                toolbarControl.AddToAllToolbars(onStockToolbarClick, onStockToolbarClick,
                    ApplicationLauncher.AppScenes.FLIGHT,
                    MODID,
                    "rcsLandAidButton",
                    "Diazo/RCSLandAid/PluginData/Textures/iconWhiteB-38",
                    "Diazo/RCSLandAid/PluginData/Textures/iconWhiteB-24",
                    MODNAME
                );
                toolbarControl.AddLeftRightClickCallbacks(LeftClick, RightClick);
                buttonCreated = true;
            }
        }

        public void OnGUI()
        {
            if (showLAMenu)
            {
                LASettingsWin = ClickThruBlocker.GUIWindow(67347792, LASettingsWin, DrawWin, "Settings", HighLogic.Skin.window);
                if (Input.mousePosition.x > LASettingsWin.x && Input.mousePosition.x < LASettingsWin.x + LASettingsWin.width && (Screen.height - Input.mousePosition.y) > LASettingsWin.y && (Screen.height - Input.mousePosition.y) < LASettingsWin.y + LASettingsWin.height)
                {
                    overWindow = true;

                }
                else
                {
                    overWindow = false;
                }
            }
        }

        public void DummyVoid()
        {

        }
        public void onStockToolbarClick()
        {
#if false
            //print("mouse " + Input.GetMouseButtonUp(1) + Input.GetMouseButtonDown(1));
            //superceeded by delegates in KSP 1.1
            if (Input.GetMouseButtonUp(1))
            {
                RightClick();
            }
            else
            {
                LeftClick();
            }
#endif
        }

        internal static void SetColors(LineRenderer l, Color start, Color end)
        {
            l.startColor = start;
            l.endColor = end;
        }
        internal static void SetWidth(LineRenderer l, float start, float end)
        {
            l.startWidth = start;
            l.endWidth = end;
        }

        public void SetHoverOn()
        {
            curVsl.controlState = 1;
            curVsl.targetSelected = false;
            selectingTarget = false;
            SetWidth(curVsl.theLine, 0, 0);

            toolbarControl.SetTexture("Diazo/RCSLandAid/PluginData/Textures/iconBlue-38", "Diazo/RCSLandAid/PluginData/Textures/iconBlue-24");
            showLAMenu = false;
            //              }
            selectingTarget = false;
            curVsl.targetSelected = false;
        }

        public void SetHoverOff()
        {
            curVsl.controlState = 0;
            curVsl.targetSelected = false;
            selectingTarget = false;
            SetWidth(curVsl.theLine, 0, 0);

            toolbarControl.SetTexture("Diazo/RCSLandAid/PluginData/Textures/iconWhiteB-38", "Diazo/RCSLandAid/PluginData/Textures/iconWhiteB-24");
            showLAMenu = false;
            //            }
        }

        public void SetHoldOn()
        {
            curVsl.controlState = 2;
            selectingTarget = true;
            SetColors(curVsl.theLine, Color.red, Color.red);

            toolbarControl.SetTexture("Diazo/RCSLandAid/PluginData/Textures/iconRed-38", "Diazo/RCSLandAid/PluginData/Textures/iconRed-24");
            showLAMenu = true;

        }

        public void SetHoldOnLink()
        {
            curVsl.controlState = 2;
            selectingTarget = true;
            SetColors(curVsl.theLine, Color.red, Color.red);

            toolbarControl.SetTexture("Diazo/RCSLandAid/PluginData/Textures/iconRed-38", "Diazo/RCSLandAid/PluginData/Textures/iconRed-24");
            //showLAMenu = true;

        }

        public void SetHoldOnHere()
        {
            curVsl.controlState = 2;
            selectingTarget = false;


            RaycastHit pHit;
            //FlightCamera FlightCam = FlightCamera.fetch;
            LayerMask pRayMask = 33792; //layermask does not ignore layer 0, why?
            Ray pRay = new Ray(FlightGlobals.ActiveVessel.transform.position, FlightGlobals.currentMainBody.position - FlightGlobals.ActiveVessel.transform.position); //FlightCam.mainCamera.ScreenPointToRay(Input.mousePosition);
            //Ray pRayDown = new Ray(FlightCamera. transform.position , FlightGlobals.currentMainBody.position);
            Vector3 hitLoc = new Vector3();
            if (Physics.Raycast(pRay, out pHit, 2000f, pRayMask)) //cast ray
            {
                hitLoc = pHit.point;
                // print(hitLoc);
                SetWidth(curVsl.theLine, 0, 1);
                curVsl.theLine.SetPosition(0, hitLoc);
                curVsl.theLine.SetPosition(1, hitLoc + ((hitLoc - FlightGlobals.ActiveVessel.mainBody.position).normalized) * 7);
                //if (!overWindow)
                //{
                //    if (Input.GetKeyDown(KeyCode.Mouse0))
                //    {
                //if (checkBlizzyToolbar)
                //{
                //    RCSla1Btn.Drawable = null;
                //}
                //else
                //{
                //    showLAMenu = false;
                //}
                selectingTarget = false;
                curVsl.targetLocation = hitLoc;
                curVsl.targetSelected = true;
                //}
                //}
            }


            //curVsl.targetLocation = curVsl.vessel.transform.position;
            SetColors(curVsl.theLine, Color.red, Color.red);

            toolbarControl.SetTexture("Diazo/RCSLandAid/PluginData/Textures/iconRed-38", "Diazo/RCSLandAid/PluginData/Textures/iconRed-24");
            //showLAMenu = true;

        }

        public void LeftClick()
        {

            Debug.Log("RCS LeftClick");
            if (curVsl != null)
            {
                //Debug.Log("RCS lcs " + curVsl.controlState);
                if (curVsl.controlState == 0)
                {
                    thisModule.SetHoverOn();
                }
                else
                {
                    thisModule.SetHoverOff();
                }
            }
        }



        public void RightClick()
        {

            Debug.Log("RCS Rightclick");
            //RCSLandingAid thisClass = FindObjectOfType<RCSLandingAid>();
            if (curVsl != null)
            {
                //Debug.Log("RCS cs " + curVsl.controlState);
                if (curVsl.controlState == 2)
                {
                    thisModule.SetHoverOn();

                }
                else
                {
                    thisModule.SetHoldOn();

                }
            }
        }

        public bool DataModulePresent(Vessel vsl)
        {
            try
            {
                foreach (Part p in vsl.Parts)
                {
                    if (p.Modules.Contains("RCSLandingAidModule"))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public void Update()
        {
            string errLine = "1";
            try
            {
                //Debug.Log("vsl nulla");
                if (!DataModulePresent(FlightGlobals.ActiveVessel) && curVsl != null)
                {
                    errLine = "1a";
                    //Debug.Log("vsl null");
                    SetColors(curVsl.theLine, Color.blue, Color.blue);
                    curVsl = null;
                    curBtnState = 0;
                }
                //Debug.Log("vsl nullb");
                errLine = "1b";
                if (curVsl == null && DataModulePresent(FlightGlobals.ActiveVessel) || curVsl != null && curVsl.vessel.rootPart != FlightGlobals.ActiveVessel.rootPart || curVsl != null && !curVsl.isMasterModule || curVsl != null && !FlightGlobals.ActiveVessel.parts.Contains(curVsl.part))
                {
                    //Debug.Log("vsl switch");
                    try
                    {
                        if (curVsl != null)
                        {
                            //Debug.Log("blue reset");
                            SetColors(curVsl.theLine, Color.blue, Color.blue);
                        }
                    }
                    catch
                    {
                        //Debug.Log("vsl nullb catch");
                    }
                    errLine = "3";
                    //bool mdlFound = false;
                    //foreach (Part p in FlightGlobals.ActiveVessel.parts)
                    //{
                    //    errLine = "4";
                    List<RCSLandingAidModule> dataModules = new List<RCSLandingAidModule>();
                    foreach (Part p in FlightGlobals.ActiveVessel.parts)
                    {
                        errLine = "4";
                        //foreach (TWR1Data td in p.Modules.OfType<TWR1Data>())
                        //{
                        dataModules.AddRange(p.Modules.OfType<RCSLandingAidModule>());
                        //}
                    }
                    errLine = "4a";
                    if (dataModules.Count == 0)
                    {
                        errLine = "4b";
                        curVsl = null;
                    }
                    else if (dataModules.Where(pm => pm.isMasterModule == true).Count() > 0)
                    {
                        errLine = "4c";
                        curVsl = dataModules.Where(pm => pm.isMasterModule == true).First();
                    }
                    else
                    {
                        errLine = "4d";
                        curVsl = dataModules.First();
                    }
                    errLine = "4e";
                    foreach (RCSLandingAidModule tdata in dataModules)
                    {
                        if (tdata == curVsl) //make sure our master is set
                        {
                            curVsl.isMasterModule = true;
                        }
                        else //all other modules are ignored
                        {
                            tdata.isMasterModule = false;
                            tdata.controlState = 0;
                        }
                    }
                    //foreach (RCSLandingAidModule la in p.Modules.OfType<RCSLandingAidModule>())
                    //{
                    //    if (!mdlFound)
                    //    {
                    //        curVsl = la;
                    //        mdlFound = true;
                    //        la.isMasterModule = true;
                    //        curVsl.theLine.SetColors(Color.red, Color.red);
                    //        //Debug.Log("td fnd");
                    //    }
                    //    else
                    //    {
                    //        la.isMasterModule = false;
                    //        //Debug.Log("td not found");
                    //    }
                    //}
                    //if (!mdlFound)
                    //{
                    //    //Debug.Log("vsl nulld not found");
                    //    curVsl = null;
                    //}
                    //if (p.Modules.Contains("TWR1Data"))
                    //{
                    //    errLine = "5";

                    //}
                    //errLine = "6";
                    //goto partFound;
                }
                //Debug.Log("vsl nulld");
                errLine = "7";
                //curVsl = null;
                errLine = "8";
                //curBtnState = 0;
                //   }
            }
            catch
            {
                print("LandAid hit Update catch" + errLine);
                curBtnState = 0;
                if (curVsl != null)
                {
                    SetColors(curVsl.theLine, Color.blue, Color.blue);
                }
                curVsl = null;
            }
            errLine = "9";
            // Debug.Log("LA " + Krakensbane.GetFrameVelocity().ToString() + "|" + Krakensbane.GetFrameVelocity().magnitude);
            try
            {
                //print("Height " + engageHeight);
                if (selectingTarget)
                {
                    RaycastHit pHit;
                    FlightCamera FlightCam = FlightCamera.fetch;
                    //LayerMask pRayMask = 33792; //layermask does not ignore layer 0, why?
                    LayerMask pRayMask = 32768; //hit only layer 15
                    Ray pRay = FlightCam.mainCamera.ScreenPointToRay(Input.mousePosition);
                    //Ray pRayDown = new Ray(FlightCamera. transform.position , FlightGlobals.currentMainBody.position);
                    Vector3 hitLoc = new Vector3();
                    if (Physics.Raycast(pRay, out pHit, 2000f, pRayMask)) //cast ray
                    {
                        hitLoc = pHit.point;
                        // print(hitLoc);
                        SetWidth(curVsl.theLine, 0, 1);
                        curVsl.theLine.SetPosition(0, hitLoc);
                        curVsl.theLine.SetPosition(1, hitLoc + ((hitLoc - FlightGlobals.ActiveVessel.mainBody.position).normalized) * 7);
                        if (!overWindow)
                        {
                            if (Input.GetKeyDown(KeyCode.Mouse0))
                            {

                                showLAMenu = false;

                                selectingTarget = false;
                                curVsl.targetLocation = hitLoc;
                                curVsl.targetSelected = true;
                            }
                        }
                    }
                }
                errLine = "10";
                //Debug.Log("LA " +curBtnState);
                if (lastBtnState != curBtnState)
                {
                    switch (curBtnState)
                    {
                        case 0:

                            //Debug.Log("LA2");
                            toolbarControl.SetTexture("Diazo/RCSLandAid/PluginData/Textures/iconWhiteB-38", "Diazo/RCSLandAid/PluginData/Textures/iconWhiteB-24");

                            break;
                        case 1:

                            //Debug.Log("LA4");
                            toolbarControl.SetTexture("Diazo/RCSLandAid/PluginData/Textures/iconBlue-38", "Diazo/RCSLandAid/PluginData/Textures/iconBlue-24");

                            break;
                        case 2:

                            // Debug.Log("LA6");
                            toolbarControl.SetTexture("Diazo/RCSLandAid/PluginData/Textures/iconBlueEnabled-38", "Diazo/RCSLandAid/PluginData/Textures/iconBlueEnabled-24");

                            break;
                        case 3:

                            // Debug.Log("LA8");
                            toolbarControl.SetTexture("Diazo/RCSLandAid/PluginData/Textures/iconRed-38", "Diazo/RCSLandAid/PluginData/Textures/iconRed-24");

                            break;
                        case 4:

                            // Debug.Log("LA10");
                            toolbarControl.SetTexture("Diazo/RCSLandAid/PluginData/Textures/iconRedEnabled-38", "Diazo/RCSLandAid/PluginData/Textures/iconRedEnabled-24");

                            break;
                    }
                    lastBtnState = curBtnState;
                }
            }
            catch (Exception e)
            {
                Debug.Log("Landing Aid Error " + e);
            }


        }

        public void OnDisable()
        {

            toolbarControl.OnDestroy();
            Destroy(toolbarControl);

            //RCSla.SetValue("EngageHeight", curVsl.engageHeight.ToString());
            //RCSla.SetValue("ForceSAS", forceSASup.ToString());
            // RCSla.Save(KSPUtil.ApplicationRootPath + "GameData/Diazo/RCSLandAid/RCSla.settings");
        }

        //public void UpdateButtons()
        //{
        //    if (checkBlizzyToolbar)
        //    {
        //        if (curVsl.controlState == 0)
        //        {
        //            RCSla1Btn.TexturePath = "Diazo/RCSLandAid/PluginData/Textures/iconWhiteB-24";
        //        }
        //        else if (curVsl.controlState == 1 && curVsl.inControl)
        //        {
        //            RCSla1Btn.TexturePath = "Diazo/RCSLandAid/PluginData/Textures/iconBlueEnabled-24";
        //        }
        //        else if (curVsl.controlState == 1 && !curVsl.inControl)
        //        {
        //            RCSla1Btn.TexturePath = "Diazo/RCSLandAid/PluginData/Textures/iconBlue-24";
        //        }
        //        else if (curVsl.controlState == 2 && curVsl.inControl)
        //        {
        //            RCSla1Btn.TexturePath = "Diazo/RCSLandAid/PluginData/Textures/iconRedEnabled-24";
        //        }
        //        else if (curVsl.controlState == 2 && !curVsl.inControl)
        //        {
        //            RCSla1Btn.TexturePath = "Diazo/RCSLandAid/PluginData/Textures/iconRed-24";
        //        }

        //    }
        //    else
        //    {
        //        if(curVsl.controlState == 0)
        //        {
        //            LAButton.SetTexture(btnGray);
        //        }
        //        if (curVsl.controlState == 1 && curVsl.inControl)
        //        {
        //            LAButton.SetTexture(btnBlueEnable);
        //        }
        //        else if (curVsl.controlState == 1 && !curVsl.inControl)
        //        {
        //            LAButton.SetTexture(btnBlue);
        //        }
        //        else if (curVsl.controlState == 2 && curVsl.inControl)
        //        {
        //            LAButton.SetTexture(btnRedEnable);
        //        }
        //        else if (curVsl.controlState == 2 && !curVsl.inControl)
        //        {
        //            LAButton.SetTexture(btnRed);
        //        }

        //    }
        //}

        public void DrawWin(int WindowID)
        {

            GUI.Label(new Rect(10, 20, 100, 20), "Engage At:");
            string engageHeightStr = curVsl.engageHeight.ToString();//same^
            engageHeightStr = GUI.TextField(new Rect(100, 20, 50, 20), engageHeightStr, 5);//same^
            try//same^
            {
                curVsl.engageHeight = Convert.ToInt32(engageHeightStr); //convert string to number
            }
            catch//same^
            {
                engageHeightStr = curVsl.engageHeight.ToString(); //conversion failed, reset change
            }

            GUI.Label(new Rect(10, 40, 100, 20), "Max Tip:");
            string maxTipStr = curVsl.maxTip.ToString();//same^
            maxTipStr = GUI.TextField(new Rect(100, 40, 50, 20), maxTipStr, 5);//same^
            try//same^
            {
                curVsl.maxTip = Convert.ToInt32(maxTipStr); //convert string to number
            }
            catch//same^
            {
                maxTipStr = curVsl.maxTip.ToString(); //conversion failed, reset change
            }

            GUI.Label(new Rect(10, 60, 100, 20), "Speed%:");
            string speedStr = (curVsl.aggresiveness * 100f).ToString("####0");//same^
            speedStr = GUI.TextField(new Rect(100, 60, 50, 20), speedStr, 5);//same^
            try//same^
            {
                curVsl.aggresiveness = (float)(Convert.ToDouble(speedStr) / 100); //convert string to number
            }
            catch//same^
            {
                speedStr = (curVsl.aggresiveness * 100f).ToString("####0"); //conversion failed, reset change
            }
            //if(curVsl.useTip)
            //{
            //    if(GUI.Button(new Rect(10,80,70,20),"Tip: Yes"))
            //    {
            //        curVsl.useTip = false;
            //    }
            //}
            //else
            //{
            //    if (GUI.Button(new Rect(10, 80, 70, 20), "Tip: No"))
            //    {
            //        curVsl.useTip = true;
            //    }
            //}
            //if (curVsl.useRCS)
            //{
            //    if (GUI.Button(new Rect(80, 80, 70, 20), "RCS: Yes"))
            //    {
            //        curVsl.useRCS = false;
            //    }
            //}
            //else
            //{
            //    if (GUI.Button(new Rect(80, 80, 70, 20), "RCS: No"))
            //    {
            //        curVsl.useRCS = true;
            //    }
            //}

        }
    }

    public class RCSLandingAidWindow : MonoBehaviour, IDrawable
    {
        public Rect RCSlaWin = new Rect(0, 0, 180, 90);

        public Vector2 Draw(Vector2 position)
        {

            var oldSkin = GUI.skin;
            GUI.skin = HighLogic.Skin;

            RCSlaWin.x = position.x;
            RCSlaWin.y = position.y;

            ClickThruBlocker.GUIWindow(22334567, RCSlaWin, DrawWin, "", GUI.skin.window);
            //RCSlaWin = GUILayout.Window(42334567, RCSlaWin, DrawWin, (string)null, GUI.skin.box);
            GUI.skin = oldSkin;
            if (Input.mousePosition.x > RCSlaWin.x && Input.mousePosition.x < RCSlaWin.x + RCSlaWin.width && (Screen.height - Input.mousePosition.y) > RCSlaWin.y && (Screen.height - Input.mousePosition.y) < RCSlaWin.y + RCSlaWin.height)
            {
                RCSLandAid.RCSLandingAid.overWindow = true;

            }
            else
            {
                RCSLandAid.RCSLandingAid.overWindow = false;
            }
            return new Vector2(RCSlaWin.width, RCSlaWin.height);
        }

        public void DrawWin(int WindowID)
        {

            GUI.Label(new Rect(10, 20, 100, 20), "Engage At:");
            string engageHeightStr = RCSLandingAid.curVsl.engageHeight.ToString();//same^
            engageHeightStr = GUI.TextField(new Rect(100, 20, 50, 20), engageHeightStr, 5);//same^
            try//same^
            {
                RCSLandingAid.curVsl.engageHeight = Convert.ToInt32(engageHeightStr); //convert string to number
            }
            catch//same^
            {
                engageHeightStr = RCSLandingAid.curVsl.engageHeight.ToString(); //conversion failed, reset change
            }

            GUI.Label(new Rect(10, 40, 100, 20), "Max Tip:");
            string maxTipStr = RCSLandingAid.curVsl.maxTip.ToString();//same^
            maxTipStr = GUI.TextField(new Rect(100, 40, 50, 20), maxTipStr, 5);//same^
            try//same^
            {
                RCSLandingAid.curVsl.maxTip = Convert.ToInt32(maxTipStr); //convert string to number
            }
            catch//same^
            {
                maxTipStr = RCSLandingAid.curVsl.maxTip.ToString(); //conversion failed, reset change
            }

            GUI.Label(new Rect(10, 60, 100, 20), "Speed%:");
            string speedStr = (RCSLandingAid.curVsl.aggresiveness * 100f).ToString("####0");//same^
            speedStr = GUI.TextField(new Rect(100, 60, 50, 20), speedStr, 5);//same^
            try//same^
            {
                RCSLandingAid.curVsl.aggresiveness = (float)(Convert.ToDouble(speedStr) / 100); //convert string to number
            }
            catch//same^
            {
                speedStr = (RCSLandingAid.curVsl.aggresiveness * 100f).ToString("####0"); //conversion failed, reset change
            }


        }

        public void Update()
        {

        }
    }
}
