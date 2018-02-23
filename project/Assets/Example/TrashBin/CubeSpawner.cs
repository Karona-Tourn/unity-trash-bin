using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{

	}

	private void SpawnCube ()
	{
		TK.TrashBins.TrashBinManager.Get ( "trash-bin" ).TakeOut ( new TK.TrashBins.TrashConfig () { binName = "cube", position = transform.position, isActive = true, rotation = Quaternion.Euler ( Random.value * 360f, Random.value * 360f, Random.value * 360f ) } );
	}

	// Update is called once per frame
	void Update ()
	{
		if ( Input.anyKeyDown )
		{
			SpawnCube ();
		}
	}
}
