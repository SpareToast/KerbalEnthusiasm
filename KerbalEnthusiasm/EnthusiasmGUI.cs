using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerbalEnthusiasm
{
    public class EnthusiasmGui
    {
        private Rect _windowPosition = new Rect();
        public ConfigNode SettingsNode { get; private set; }
        public void OnDraw()
        {
            _windowPosition = GUILayout.Window(10, _windowPosition, OnWindow, "All Kerbals");
        }

        public void OnWindow(int windowID)
        {
            GUILayout.BeginHorizontal(GUILayout.Width(250f));
            GUILayout.Label("All Tracked Kerbals");
            GUILayout.EndHorizontal();
            if (EnthusiasmPersistance.TrackedKerbals().Count > 0)
            {
                foreach (KeyValuePair<string, KerbalStatus> k in EnthusiasmPersistance.TrackedKerbals())
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(k.Value.KerbalName);
                    GUILayout.Label(k.Value.Enthusiasm.ToString());
                    GUILayout.EndHorizontal();
                }
            }
            GUI.DragWindow();
        }
    }
}

