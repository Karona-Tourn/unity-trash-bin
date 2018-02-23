using System;
using System.Collections.Generic;
using UnityEngine;

namespace TK.TrashBins
{
	/// <summary>
	/// Manager to controll all object bins
	/// </summary>
	public class TrashBinManager : MonoBehaviour
	{
		[SerializeField, Tooltip("Name of object bin manager that must be unique.")]
		private string uniqueName = "";

		[SerializeField, Tooltip("Should the manager is destroyed when load scene or not?")]
		private bool destroyOnLoad = false;

		[SerializeField]
		private TrashBin[] preloadBins = new TrashBin[0];

		/// <summary>
		/// Store bins
		/// </summary>
		private SortedDictionary<string, TrashBin> bins = new SortedDictionary<string, TrashBin> ();

		/// <summary>
		/// List of bin managers
		/// </summary>
		private static SortedDictionary<string, TrashBinManager> managers = new SortedDictionary<string, TrashBinManager> ();

		/// <summary>
		/// Object to lock in thread
		/// </summary>
		private object @lock = new object ();

		private void Awake ()
		{
			if (string.IsNullOrEmpty (uniqueName))
			{
				Debug.LogErrorFormat (this, "The object bin manager \"{0}\" must have a unique name", name);
				return;
			}

			if (managers.ContainsKey (uniqueName))
			{
				Debug.LogErrorFormat (this, "The name \"{0}\" is already used.", uniqueName);
				return;
			}

			if (destroyOnLoad)
			{
				DontDestroyOnLoad (gameObject);
			}

			managers.Add (uniqueName, this);
		}

		private void Start ()
		{
			// Preload for creating trashes
			int length = preloadBins.Length;
			for (int i = 0; i < length; i++)
			{
				CreateBin (preloadBins[i]);
			}
		}

		private void OnDestroy ()
		{
			managers.Remove (uniqueName);
		}

		private void InternalCreateBin (TrashBin newBin)
		{
			newBin.Setup (transform);
			bins.Add (newBin.Name, newBin);
		}

		public void CreateBin (TrashBin newBin)
		{
			lock (@lock)
			{
				InternalCreateBin (newBin);
			}
		}

		public void CreateBin (string binName, GameObject prefab, int preloadCount = 1, bool hasOwnRoot = false)
		{
			lock (@lock)
			{
				TrashBin newBin = new TrashBin (binName, prefab, preloadCount, hasOwnRoot);
				InternalCreateBin (newBin);
			}
		}

		/// <summary>
		/// Take out a trash from bin by with specific bin
		/// </summary>
		public GameObject TakeOut (TrashConfig config)
		{
			lock (@lock)
			{
				var bin = bins[config.binName];
				var trash = bin.TakeOut (config.isActive);

				trash.Root = config.parent;

				if (config.isLocal && trash.Root != null)
				{
					trash.transform.localPosition = config.position;
					trash.transform.localRotation = config.rotation;
				}
				else
				{
					trash.transform.position = config.position;
					trash.transform.rotation = config.rotation;
				}

				return trash.gameObject;
			}
		}

		/// <summary>
		/// Gets the manager.
		/// </summary>
		public static TrashBinManager Get (string managerName)
		{
			TrashBinManager manager = null;

			if (managers.TryGetValue (managerName, out manager))
			{
				return manager;
			}

			Debug.LogErrorFormat ("Bin manager \"{0}\" not found!", managerName);

			return null;
		}

		/// <summary>
		/// Take out a trash from bin by with the given manager name and bin name
		/// </summary>
		public static GameObject TakeOut (string managerName, TrashConfig config)
		{
			var manager = Get (managerName);
			if (manager == null)
			{
				return null;
			}
			var go = manager.TakeOut (config);
			return go;
		}

		/// <summary>
		/// Take trash back into the bin
		/// </summary>
		public static void TakeIn (GameObject go)
		{
			Trash trash = go.GetComponent<Trash> ();

			if (trash == null)
			{
				return;
			}

			trash.ThrowToBin ();
		}

		/// <summary>
		/// Take all using trashes back into bin at specific manager and bin
		/// </summary>
		public static void TakeInAll(string managerName, string binName)
		{
			var manager = Get (managerName);

			if (manager == null)
			{
				return;
			}

			TrashBin bin = null;

			if (manager.bins.TryGetValue (binName, out bin))
			{
				bin.TakeInAll ();
			}
		}

		/// <summary>
		/// Takes all using trashes back into bins at specific manager.
		/// </summary>
		public static void TakeInAll(string managerName)
		{
			var manager = Get (managerName);

			if (manager == null)
			{
				return;
			}

			foreach (var bin in manager.bins)
			{
				bin.Value.TakeInAll ();
			}
		}
	}
}
