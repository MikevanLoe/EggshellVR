using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mover3D : MonoBehaviour {
	public float MaxSpeed = 6;
	public float Acceleration = 2;
	public float MaxTurn = 2;
	public float WanderRadius = 2;
	public float WanderDistance = 5;
	public float WanderJitter = 2;

	private Vector3 WanderTarget;
	public Vector3 Heading;

	public bool SeekOn, AlignmentOn, CohesionOn, SeperationOn, FleeOn;

	private Vector3 _side;
	private Rigidbody rb;

	// Use this for initialization
	void Start () {
		if (rb != null)
			return;
		rb = GetComponent<Rigidbody> ();
		if (rb == null)
			throw new MissingComponentException ("Mover could not find Rigidbody component");
	}

	public Vector3 Seek(Vector3 target)
	{
		Vector3 DesiredVelocity = target - rb.position;

		DesiredVelocity.Normalize();
		DesiredVelocity *= MaxSpeed;
		return DesiredVelocity - rb.velocity;
	}
	
	public Vector3 Evade(Vector3 target)
	{
		if(Vector3.Distance(rb.position, target) > 5)
		{
			return new Vector3(0,0,0);
		}
		Vector3 DesiredVelocity = rb.position - target;
		DesiredVelocity.Normalize();
		DesiredVelocity *= MaxSpeed;
		
		return DesiredVelocity  - rb.velocity;
	}
	
	public void Accelerate(Vector3 Force)
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
		if (Mathf.Abs (rb.velocity.magnitude) > float.Epsilon) {
			Heading = rb.velocity;
			Heading.Normalize();
			_side = transform.right;
		}
	}

	public Vector3 Seperation(List<GameObject> neighbors)
	{
		Vector3 SteeringForce = new Vector2();
		for (int a=0; a<neighbors.Count; ++a)
		{
			//make sure this agent isn't included in the calculations and that
			//the agent being examined is close enough.
			if(neighbors[a] != gameObject)
			{
				Vector3 ToAgent = transform.position - neighbors[a].transform.position;
				//scale the force inversely proportional to the agent's distance
				//from its neighbor.
				ToAgent.Normalize();
				ToAgent *= MaxSpeed;
				SteeringForce += ToAgent/neighbors.Count;
			}
		}
		return SteeringForce;
	}

	public Vector3 Alignment(List<GameObject> neighbors)
	{
		//used to record the average heading of the neighbors
		Vector3 AverageHeading = new Vector3();
		//used to count the number of vehicles in the neighborhood
		int NeighborCount = 0;
		//iterate through all the tagged vehicles and sum their heading vectors
		for (int a=0; a<neighbors.Count; ++a)
		{
			//make sure *this* agent isn't included in the calculations and that
			//the agent being examined is close enough
			if(neighbors[a] != gameObject)
			{
				Vector3 neighborHeading = neighbors[a].GetComponent<Mover>().Heading;
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

	public Vector3 Cohesion(List<GameObject> neighbors)
	{
		//first find the center of mass of all the agents
		Vector3 CenterOfMass = new Vector2(), SteeringForce = new Vector2();
		int NeighborCount = 0;
		//iterate through the neighbors and sum up all the position vectors
		for (int a=0; a<neighbors.Count; ++a)
		{
			if(neighbors[a] != gameObject)
			{
				Vector3 neighborPos = neighbors[a].transform.position;
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

	public Vector3 Wander()
	{
		//first, add a small random vector to the target’s position (RandomClamped
		//returns a value between -1 and 1)

		WanderTarget += new Vector3(Random.Range(-1, 2) * WanderJitter,
		                            Random.Range(-1, 2) * WanderJitter,
	                             	Random.Range(-1, 2) * WanderJitter);

		//reproject this new vector back onto a unit circle
		WanderTarget.Normalize();

		//increase the length of the vector to the same as the radius
		//of the wander circle
		WanderTarget *= WanderRadius;

		//move the target into a position WanderDist in front of the agent
		Vector3 WanderForce = Heading * WanderDistance + WanderTarget;
		return WanderForce;
	}
}