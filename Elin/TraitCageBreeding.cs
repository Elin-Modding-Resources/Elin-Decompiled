using UnityEngine;

public class TraitCageBreeding : TraitCage
{
	public override Vector3 GetRestrainPos => default(Vector3);

	public override bool AllowTraining => false;
}
