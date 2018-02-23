using UnityEngine;

namespace TK.TrashBins
{
	/// <summary>
	/// Component as a trash placed into a trash bin
	/// </summary>
	public sealed class Trash : MonoBehaviour
	{
		private Transform _transform = null;

		/// <summary>
		/// The trash bin that the trash belongs to
		/// </summary>
		private TrashBin bin = null;

		public new Transform transform
		{
			get
			{
				if (_transform == null)
				{
					_transform = base.transform;
				}
				return _transform;
			}
		}

		/// <summary>
		/// Get Id of this trash
		/// </summary>
		public int Id
		{
			get { return gameObject.GetInstanceID (); }
		}

		/// <summary>
		/// Get the bin that this trash belongs to
		/// </summary>
		public TrashBin Bin
		{
			get { return bin; }
		}

		public Transform Root
		{
			get { return transform.parent; }
			set { transform.SetParent (value); }
		}

		public bool Active
		{
			get { return gameObject.activeSelf; }
			set { gameObject.SetActive (value); }
		}

		/// <summary>
		/// Invoked to throw the trash back into the bin
		/// </summary>
		public void ThrowToBin ()
		{
			if (bin == null)
			{
				return;
			}

			bin.TakeIn (this);
		}

		/// <summary>
		/// Create a new trash instance
		/// </summary>
		/// <param name="go">Target game object</param>
		/// <param name="bin">Bin that trash belongs to</param>
		public static Trash Create (GameObject go, TrashBin bin)
		{
			if (go == null)
			{
				return null;
			}

			var trash = go.GetComponent<Trash> ();

			if (trash)
			{
				if (trash.bin != null)
				{
					trash.bin.Remove (trash);
					trash.bin = null;
				}
			}
			else
			{
				trash = go.AddComponent<Trash> ();
			}

			trash.bin = bin;

			return trash;
		}
	}
}
