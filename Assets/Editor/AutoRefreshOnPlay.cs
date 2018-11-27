using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Pison
{
	[InitializeOnLoad]
	public class AutoRefreshOnPlay
	{

		static AutoRefreshOnPlay()
		{
			EditorApplication.playModeStateChanged
				+= PlaymodeChanged;
			EditorApplication.LockReloadAssemblies();
		}
		
		static void PlaymodeChanged(PlayModeStateChange state)
		{
			if (state != PlayModeStateChange.ExitingEditMode) 
			{
				return;
			}

			Debug.Log("PISON: Reloading scripts");
			AssetDatabase.Refresh(ImportAssetOptions.Default);
		}
	}
}
