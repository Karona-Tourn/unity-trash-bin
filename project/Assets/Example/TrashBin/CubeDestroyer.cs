using UnityEngine;

public class CubeDestroyer : MonoBehaviour
{
	private void OnCollisionEnter ( Collision collision )
	{
		TK.TrashBins.TrashBinManager.TakeIn ( collision.gameObject );
	}
}
