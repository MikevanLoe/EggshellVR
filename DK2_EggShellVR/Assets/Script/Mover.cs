using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mover : MonoBehaviour {
	public float MaxSpeed;
	public float Acceleration;
	public float MaxTurn;
	public float WanderRadius;
	public float WanderDistance;
	public float WanderJitter;

	private Vector2 WanderTarget;
	public Vector2 Heading;

	public bool SeekOn, AlignmentOn, CohesionOn, SeperationOn, FleeOn;

	private Vector2 _side;
	private Rigidbody2D rb;

	// Use this for initialization
	void Start () {
		if (rb != null)
			return;
		rb = GetComponent<Rigidbody2D> ();
		if (rb == null)
			throw new MissingComponentException ("Mover could not find Rigidbody component");
	}

	public Vector2 Seek(Vector2 target)
	{
		Vector2 DesiredVelocity = target - rb.position;
		//Manual normalize, lol
		/*DesiredVelocity /= Mathf.Max(Mathf.Abs(DesiredVelocity.x), Mathf.Abs(DesiredVelocity.y));
		DesiredVelocity *= MaxSpeed;*/

		DesiredVelocity.Normalize();
		DesiredVelocity *= MaxSpeed;
		return DesiredVelocity - rb.velocity;
	}
	
	public Vector2 Evade(Vector2 target)
	{
		if(Vector2.Distance(rb.position, target) > 5)
		{
			return new Vector3(0,0,0);
		}
		Vector2 DesiredVelocity = rb.position - target;
		DesiredVelocity.Normalize();
		DesiredVelocity *= MaxSpeed;
		
		return DesiredVelocity;
	}
	
	public void Accelerate(Vector2 Force)
	{
		//Slow by mass
		Force /= rb.mass;
		//Decrease to acceleration
		Force = Force / MaxSpeed * Acceleration;
		//Add new force to speed
		rb.velocity += Force;
		//Limit speed to max
		float max = Mathf.Max(Mathf.Abs(rb.velocity.x), Mathf.Abs(rb.velocity.y));
		if(max > MaxSpeed)
		{
			rb.velocity = rb.velocity / max * MaxSpeed;
		}
		//Change heading if moving
		if (Mathf.Abs (rb.velocity.magnitude) > 0.001) {
			Heading = rb.velocity;
			Heading.Normalize();
			_side = Perpendicular(rb.position, Heading);
		}
	}

	public Vector2 Perpendicular(Vector2 P1, Vector2 P2)
	{
		Vector2 V = P2 - P1;
		Vector2 P3 = new Vector2(-V.y, V.x);
		P3.Normalize();
		return P3;
	}

	public Vector2 Seperation(List<GameObject> neighbors)
	{
		Vector2 SteeringForce = new Vector2();
		for (int a=0; a<neighbors.Count; ++a)
		{
			//make sure this agent isn't included in the calculations and that
			//the agent being examined is close enough.
			if(neighbors[a] != gameObject)
			{
				Vector2 ToAgent = transform.position - neighbors[a].transform.position;
				//scale the force inversely proportional to the agent's distance
				//from its neighbor.
				ToAgent.Normalize();
				ToAgent *= MaxSpeed;
				SteeringForce += ToAgent/neighbors.Count;
			}
		}
		return SteeringForce;
	}

	public Vector2 Alignment(List<GameObject> neighbors)
	{
		//used to record the average heading of the neighbors
		Vector2 AverageHeading = new Vector2();
		//used to count the number of vehicles in the neighborhood
		int NeighborCount = 0;
		//iterate through all the tagged vehicles and sum their heading vectors
		for (int a=0; a<neighbors.Count; ++a)
		{
			//make sure *this* agent isn't included in the calculations and that
			//the agent being examined is close enough
			if(neighbors[a] != gameObject)
			{
				Vector2 neighborHeading = neighbors[a].GetComponent<Mover>().Heading;
				AverageHeading += neighborHeading;
				++NeighborCount;
			}
		}
		//if the neighborhood contained one or more vehicles, average their
		//heading vectors.
		if (NeighborCount > 0)
		{
			AverageHeading /= (float)NeighborCount;
			AverageHeading -= Heading;
		}
		return AverageHeading;
	}

	public Vector2 Cohesion(List<GameObject> neighbors)
	{
		//first find the center of mass of all the agents
		Vector2 CenterOfMass = new Vector2(), SteeringForce = new Vector2();
		int NeighborCount = 0;
		//iterate through the neighbors and sum up all the position vectors
		for (int a=0; a<neighbors.Count; ++a)
		{
			if(neighbors[a] != gameObject)
			{
				Vector2 neighborPos = neighbors[a].transform.position;
				CenterOfMass += neighborPos;
				++NeighborCount;
			}
		}
		if (NeighborCount > 0)
		{
			//the center of mass is the average of the sum of positions
			CenterOfMass /= (float)NeighborCount;
			//now seek toward that position
			SteeringForce = Seek(CenterOfMass);
		}
		return SteeringForce;
	}

	public Vector2 Wander()
	{
		//first, add a small random vector to the target’s position (RandomClamped
		//returns a value between -1 and 1)

		WanderTarget += new Vector2(Random.Range(-1, 2) * WanderJitter,
	                             	Random.Range(-1, 2) * WanderJitter);

		//reproject this new vector back onto a unit circle
		WanderTarget.Normalize();

		//increase the length of the vector to the same as the radius
		//of the wander circle
		WanderTarget *= WanderRadius;

		//move the target into a position WanderDist in front of the agent
		Vector2 WanderForce = Heading + WanderTarget;
		return WanderForce;
	}
}