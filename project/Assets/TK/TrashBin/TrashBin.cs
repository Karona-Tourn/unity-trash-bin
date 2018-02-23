using System;
using System.Collections.Generic;
using UnityEngine;

namespace TK.TrashBins
{
	[Serializable]
	public class TrashBin
	{
		/// <summary>
		/// Bin name
		/// </summary>
		[SerializeField]
		private string name = "";

		/// <summary>
		/// Preload count as number of trashes to be created
		/// </summary>
		[SerializeField]
		private int preloadCount = 1;

		/// <summary>
		/// Prefab as trash sample to be created
		/// </summary>
		[SerializeField]
		private GameObject prefab = null;

		[SerializeField]
		private bool hasOwnRoot = false;

		private Transform root = null;
		private Queue<Trash> freeTrashes = new Queue<Trash> ();
		private SortedDictionary<int, Trash> busyTrashes = new SortedDictionary<int, Trash> ();

		public string Name
		{
			get { return name; }
		}

		public TrashBin (string name, GameObject prefab, int preloadCount, bool hasOwnRoot)
		{
			this.name = name;
			this.prefab = prefab;
			this.preloadCount = preloadCount;
			this.hasOwnRoot = hasOwnRoot;
			freeTrashes = new Queue<Trash> ();
			busyTrashes = new SortedDictionary<int, Trash> ();
		}

		private void CreateNewTrash ()
		{
			var go = GameObject.Instantiate (prefab, root);
			var trash = Trash.Create (go, this);
			trash.Active = false;
			freeTrashes.Enqueue (trash);
		}

		private void Preload (int count)
		{
			if (count <= 0)
			{
				return;
			}

			CreateNewTrash ();
			Preload (--count);
		}

		public bool Remove(Trash trash)
		{
			lock (this)
			{
				bool success = false;
				success |= busyTrashes.Remove (trash.Id);

				List<Trash> trashes = new List<Trash> (freeTrashes);
				var removed = trashes.Remove (trash);

				if (removed)
				{
					trash.Root = null;

					freeTrashes.Clear ();
					for (int i = trashes.Count - 1; i >= 0; i--)
					{
						freeTrashes.Enqueue (trashes [i]);
					}
				}

				success |= removed;

				return success;
			}
		}

		/// <summary>
		/// Take the trash back into the bin
		/// </summary>
		/// <param name="trash"></param>
		public bool TakeIn (Trash trash)
		{
			if (trash == null)
			{
#if UNITY_EDITOR
				Debug.LogWarning ("Trash is null, so it cannot be taken into the bin.");
#endif
				return false;
			}

			if (trash.Bin != this)
			{
#if UNITY_EDITOR
				Debug.LogWarning ("The trash does not belong to the bin.");
#endif
				return false;
			}

			bool ok = busyTrashes.Remove (trash.Id);

			if (ok)
			{
				trash.Active = false;
				trash.Root = root;
				freeTrashes.Enqueue (trash);
			}

			return ok;
		}

		public int TakeInAll()
		{
			lock (this)
			{
				int count = busyTrashes.Count;

				var trashes = new List<Trash> (busyTrashes.Values);

				for (int i = 0; i < count; i++)
				{
					trashes [i].ThrowToBin ();
				}

				return count;
			}
		}

		/// <summary>
		/// Take out a free trash out of the bin
		/// </summary>
		/// <returns></returns>
		public Trash TakeOut (bool isActive = true)
		{
			if (freeTrashes.Count > 0)
			{
				var trash = freeTrashes.Dequeue ();
				if (trash.Active != isActive)
				{
					trash.Active = isActive;
				}
				busyTrashes.Add (trash.Id, trash);
				return trash;
			}

			// Create a new trash
			CreateNewTrash ();

			return TakeOut ();
		}

		public void Setup (Transform root)
		{
			if (hasOwnRoot)
			{
				var go = new GameObject (name);
				go.transform.SetParent (root);
				this.root = go.transform;
			}
			else
			{
				this.root = root;
			}

			if (freeTrashes == null)
			{
				freeTrashes = new Queue<Trash> ();
			}

			if (busyTrashes == null)
			{
				busyTrashes = new SortedDictionary<int, Trash> ();
			}

			Preload (preloadCount);
		}
	}
}
