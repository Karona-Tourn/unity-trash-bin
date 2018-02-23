using System;
using UnityEngine;

namespace TK.TrashBins
{
	/// <summary>
	/// Configuration for the taken-out trash
	/// </summary>
	public struct TrashConfig
	{
		/// <summary>
		/// Name of bin from which the trash is taken out
		/// </summary>
		public string binName;

		/// <summary>
		/// Parent for a taken-out trash
		/// </summary>
		public Transform parent;

		/// <summary>
		/// Pre-position for a taken-out trash
		/// </summary>
		public Vector3 position;

		/// <summary>
		/// Pre-rotation for a taken-out trash
		/// </summary>
		public Quaternion rotation;

		/// <summary>
		/// Tell if the taken-out trash will be positioned or rotated locally or globally
		/// </summary>
		public bool isLocal;

		public bool isActive;
	}
}

